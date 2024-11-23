using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{
    // timer
    // timerHighScore
    public static bool isGameOver;
    public  float spawnRange = 9.0f;
    public float spawnRate = 1.0f;
    //private float cubeWidth;
    public GameObject obstaclePrefab;
    public List<GameObject> obstaclePrefabs;
    private GameObject spawningObstacle;
    [SerializeField] private GameObject gameOverMenu;
    private AudioSource audioSource;
    [SerializeField] private TextMeshProUGUI timerText; // Assign a UI Text element in the Inspector (optional)
    private float elapsedTime = 0f; // Tracks time the player has been alive

    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI highscoreText;

    public static GameManager Instance { get; private set; }

    void Awake() {
        StartGame();
    }
    // Start is called before the first frame update
    void Start()
    {
        // if (Instance != null)
        // {
        //     Destroy(gameObject);
        //     return;
        // }
        Instance = this;
        //DontDestroyOnLoad(gameObject);
        isGameOver = false;   

        // Set a default spawning obstacle
        if (obstaclePrefabs != null && obstaclePrefabs.Count > 0)
        {
            spawningObstacle = obstaclePrefabs[0];
        } 
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimer();
    }

    IEnumerator SpawnObstacle() {
        while (isGameOver == false) {
            yield return new WaitForSeconds(spawnRate);
            // Randomly select an obstacle prefab
            int randomIndex = Random.Range(0, obstaclePrefabs.Count);
            spawningObstacle = obstaclePrefabs[randomIndex];
            Instantiate(spawningObstacle, GenerateSpawnPosition(spawningObstacle, randomIndex), spawningObstacle.transform.rotation);
            //int index = Random.Range(0, targets.Count);
            //Instantiate(targets[index]);
        }
    }

    private Vector3 GenerateSpawnPosition(GameObject obstacle, int spawningIndex) {
        //int randomIndex = Random.Range(0, 2);
        //spawningObstacle = obstaclePrefabs[randomIndex];
        float obstacleWidth = obstacle.GetComponent<Renderer>().bounds.size.x;
        //float spawnPosX = Random.Range(cubeWidth, cubeWidth);
        //float spawnPosZ = Random.Range(-700, -700);
        int randomNum = Random.Range(-2,3-spawningIndex);

        int offset;
        if (spawningIndex == 1) {
            offset = 5;
        }
        else if (spawningIndex == 2) {
            offset = 10;
        } else if (spawningIndex == 3) {
            offset = 15;
        } else {
            offset = 0;
        }

        Vector3 randomPos = new Vector3((10 * randomNum) + offset , 83.04436f, -700);

        return randomPos;
    }
    public void StartGame() {
        // Set Player and High Score Text fields
        playerNameText.text = "Player: " + ScoreManager.Instance.currentPlayerName;
        if (ScoreManager.Instance.highScorePlayerScore > 0) {
            highscoreText.text = ScoreManager.Instance.highScorePlayerName 
                + " : " + FormatTime(ScoreManager.Instance.highScorePlayerScore);
        } else {
            highscoreText.text = "None Recorded";
        }
        isGameOver = false;
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(SpawnObstacle());
    }
    public static void GameOver() {
        Instance.StopMusic();
        isGameOver = true;
        if (ScoreManager.Instance.currentPlayerScore > ScoreManager.Instance.highScorePlayerScore) {
            ScoreManager.Instance.SavePlayerData();
            Instance.highscoreText.text = ScoreManager.Instance.highScorePlayerName + " : " + Instance.FormatTime(ScoreManager.Instance.highScorePlayerScore);
        }
        ScoreManager.Instance.currentPlayerScore = 0;
        Instance.gameOverMenu.gameObject.SetActive(true);
    }
    public void RestartGame() {
        SceneManager.LoadScene(1);
        StartGame();
    }
    public void ReturnToMainMenu() {
        SceneManager.LoadScene(0);
    }
    public void QuitGame() {
        #if UNITY_EDITOR
                EditorApplication.ExitPlaymode();
        #else
                Application.Quit(); // original code to quit Unity player
        #endif
    }

    private void StopMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    private void UpdateTimer() {
        if (isGameOver == false)
        {
            // Increment the timer
            elapsedTime += Time.deltaTime;
            // Update Playerscore
            ScoreManager.Instance.currentPlayerScore = Mathf.FloorToInt(elapsedTime);

            // Update the timer display (optional)
            if (timerText != null)
            {
                timerText.text = "Time: " + FormatTime(elapsedTime);
            }
        }
    }

    private string FormatTime(float time)
    {
        // Convert total seconds to hours, minutes, and seconds
        int hours = Mathf.FloorToInt(time / 3600);
        int minutes = Mathf.FloorToInt((time % 3600) / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        // Return a formatted string in HH:MM:SS
        return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
    }
}
