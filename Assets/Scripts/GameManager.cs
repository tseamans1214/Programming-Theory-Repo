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

    [SerializeField] private float speedIncreaseInterval = 10f;
    private int numObstacleTypes = 1;
    public static int numLanes = 5;

    public Renderer lane0;
    public Renderer lane6;

    // Increase Difficulty
    // Every 10 seconds, increase speed by 1
    // After every 30 seconds, add a new block type
    // After 1 minute, add random chance for moving blocks starting with size 1 block
    // Add more lanes?
    // Add multiple blocks on one lane?

    // ADD TO APP description and credits page
    // Music by Matthew Pablo
    // www.matthewpablo.com

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
        UpdateDifficulty();
    }

    void UpdateDifficulty() {
        // Increase speed by 1 every 10 seconds
        if (elapsedTime > speedIncreaseInterval) {
            Obstacle.verticleSpeed += -2;
            speedIncreaseInterval += 10;
        }
        // Increase the number of block types every 30 seconds
        if (elapsedTime >= 30) {
            numObstacleTypes = 4;
        } else if (elapsedTime >= 20) {
            numObstacleTypes = 3;
        } else if (elapsedTime >= 10) {
            AddLanes();
            numObstacleTypes = 2;
        }
    }
    void AddLanes() {
        if (numLanes != 7) {
            MovingObstacle.xBoundary = 35;
            numLanes = 7;
            Player.currentLane++;

            Material lane0Material = Resources.Load<Material>("Materials/Rainbow/Violet");
            Material lane6Material = Resources.Load<Material>("Materials/Rainbow/Red");
            lane0.material = lane0Material;
            lane6.material = lane6Material;
            spawnRate = 0.5f;

        }
    }

    IEnumerator SpawnObstacle() {
        while (isGameOver == false) {
            // Wait for x seconds to spwawn
            yield return new WaitForSeconds(spawnRate);

            // Randomly select an obstacle prefab
            int randomIndex = Random.Range(0, numObstacleTypes);

            // Randomly make it a moving object
            //if (randomIndex == 0) {
                int randomChanceToMove = Random.Range(0, 2);
                if (randomChanceToMove == 0) {
                    randomIndex += 4;
                }
            //}

            spawningObstacle = obstaclePrefabs[randomIndex];
           
            Instantiate(spawningObstacle, GenerateSpawnPosition(spawningObstacle, randomIndex), spawningObstacle.transform.rotation);
            //int index = Random.Range(0, targets.Count);
            //Instantiate(targets[index]);
        }
    }

    private Vector3 GenerateSpawnPosition(GameObject obstacle, int spawningIndex) {
        // If index is >=4, it is a moving block so substract for to get the correct starting index for size of block
        if (spawningIndex >= 4) {
            spawningIndex -= 4;
        }
        float obstacleWidth = obstacle.GetComponent<Renderer>().bounds.size.x;
        // Get random number for starting lane position
        //Debug.Log("numLanes: " + numLanes);
        int randomNum;
        if (numLanes == 5) {
            randomNum = Random.Range(-2,3-spawningIndex);
        } else {
            randomNum = Random.Range(-3,4-spawningIndex);
        }

        // Change offset based on size of block
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
        Obstacle.verticleSpeed = -50f;
        speedIncreaseInterval = 10f;
        numObstacleTypes = 1;
        numLanes = 5;
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
