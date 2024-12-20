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
    public static GameManager Instance { get; private set; } // ENCAPSULATION
    public static bool isGameOver;
    public List<GameObject> obstaclePrefabs;
    private GameObject spawningObstacle;
    //private AudioSource audioSource;
    
    private float elapsedTime = 0f; // Tracks time the player has been alive

    // GUI
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI highscoreText;
    [SerializeField] private GameObject gameOverMenu;

    // Variables for increasting difficulty
    [SerializeField] private float spawnRate = 1.0f;
    [SerializeField] private float spawnRateRange = 1.0f;
    [SerializeField] private float speedIncreaseInterval = 10f;
    private int numObstacleTypes = 1;
    private bool allowMovingObstacles = true;

    // Variables for adding new lanes
    public static int numLanes = 5;
    public GameObject lane0;
    public GameObject lane6;
    private Player player;
    public ObjectPooler objectPooler;

    void Awake() {
        StartGame();
    }
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        isGameOver = false;   

        // Set a default spawning obstacle
        if (obstaclePrefabs != null && obstaclePrefabs.Count > 0)
        {
            spawningObstacle = obstaclePrefabs[0];
        }
        player = GameObject.Find("Player").GetComponent<Player>();
        objectPooler = GameObject.Find("ObjectPooler").GetComponent<ObjectPooler>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimer();
        //UpdateDifficulty();
        TestDifficulty();
    }
    void TestDifficulty() {
        // Speed starts at -50
        // Increase speed by 1 every 10 seconds
        if (elapsedTime > speedIncreaseInterval) {
            Obstacle.verticleSpeed += -4;
            Debug.Log("Obstacle speed set to: " + Obstacle.verticleSpeed);
            speedIncreaseInterval += 10;

            // Increase the number of block types every 30 seconds
            if (speedIncreaseInterval == 140) {
                AddLanes();
                numObstacleTypes = 4;
                spawnRate = 0.3f;
            } else if (speedIncreaseInterval == 100) {
                RemoveLanes();
                numObstacleTypes = 3;
                spawnRate = 0.4f;
            } else if (speedIncreaseInterval == 80) {
                allowMovingObstacles = true;
                numObstacleTypes = 4;
                spawnRate = 0.3f;
                spawnRateRange = 0.3f;
                Obstacle.verticleSpeed = -100;
            } else if (speedIncreaseInterval == 60) {
                allowMovingObstacles = false;
                ClearObstacles();
                numObstacleTypes = 2;
                spawnRate = 0.1f;
                spawnRateRange = 0.2f;
                Obstacle.verticleSpeed = -150;
            } else if (speedIncreaseInterval == 40) {
                AddLanes();
                numObstacleTypes = 4;
                spawnRate = 0.4f;
                spawnRateRange = 0.4f;
            } else if (speedIncreaseInterval == 30) {
                numObstacleTypes = 3;
                spawnRate = 0.6f;
                spawnRateRange = 0.6f;
            } else if (speedIncreaseInterval == 20) {
                numObstacleTypes = 2;
                spawnRate = 0.8f;
                spawnRateRange = 0.8f;
            }
        }
        
    }
    void ClearObstacles() {
        Obstacle[] obstacles = FindObjectsOfType<Obstacle>();
        foreach (Obstacle obstacle in obstacles) {
            if (obstacle.GetType() != typeof(ScrollingBackground))
            {
                //Destroy(obstacle.gameObject); // Remove the object
                obstacle.gameObject.SetActive(false);
            }
        }
    }

    void UpdateDifficulty() {
        // Increase speed by 1 every 10 seconds
        if (elapsedTime > speedIncreaseInterval) {
            Obstacle.verticleSpeed += -2;
            Debug.Log("Obstacle speed set to: " + Obstacle.verticleSpeed);
            speedIncreaseInterval += 10;
        }
        // Increase the number of block types every 30 seconds
        if (elapsedTime >= 60) {
            // Add 2 new lanes
            AddLanes();
            numObstacleTypes = 4;
            spawnRate = 0.4f;
        } else if (elapsedTime >= 40) {
            numObstacleTypes = 3;
            spawnRate = 0.6f;
        } else if (elapsedTime >= 20) {
            numObstacleTypes = 2;
            spawnRate = 0.8f;
        }
    }
    void AddLanes() {
        if (numLanes != 7) {
            MovingObstacle.xBoundary = 35;
            numLanes = 7;
            Player.currentLane++;

            lane0.SetActive(true);
            lane6.SetActive(true);
        }
    }
    void RemoveLanes() {
        if (numLanes != 5) {
            ClearObstacles();
            MovingObstacle.xBoundary = 25;
            numLanes = 5;
            if (Player.currentLane == 1) { // If player is on the far left lane, move them right
                player.RotateAndMove(Vector3.back, Vector3.right);
                Player.currentLane = 1;
            } else if (Player.currentLane == 7) { // If player is on the far right lane, move them left
                player.RotateAndMove(Vector3.forward, Vector3.left);
                Player.currentLane = 5;
            } else {
                Player.currentLane--;
            }
            lane0.SetActive(false);
            lane6.SetActive(false);
        }
    }

    IEnumerator SpawnObstacle() {
        while (isGameOver == false) {
            float randomSpawnRate = Random.Range(spawnRate, spawnRate + spawnRateRange);
            // Wait for x seconds to spawn
            yield return new WaitForSeconds(randomSpawnRate);

            // Randomly select an obstacle prefab
            // int randomIndex = Random.Range(0, numObstacleTypes);
            int randomIndex = GetRandomObstacle();

            // Randomly make it a moving object
            if (allowMovingObstacles) {
                int randomChanceToMove = Random.Range(0, 4);
                if (randomChanceToMove == 0) {
                    randomIndex += 4;
                }
            }
            spawningObstacle = obstaclePrefabs[randomIndex];
            objectPooler.SpawnFromPool(spawningObstacle.name, GenerateSpawnPosition(spawningObstacle, randomIndex), spawningObstacle.transform.rotation);
            //Instantiate(spawningObstacle, GenerateSpawnPosition(spawningObstacle, randomIndex), spawningObstacle.transform.rotation);
        }
    }
    int GetRandomObstacle() {
        // Odds based on numObstacleTypes value
        // 1: 100
        // 2: 50|50
        // 3: 40|40|20
        // 4: 30|30|20|20
        switch(numObstacleTypes) {
            case 1: 
                return 0;
            case 2:
                return Random.Range(0, numObstacleTypes);
            case 3:
                int randomNum = Random.Range(0, 99);
                if (randomNum <= 40) {
                    return 0;
                } else if (randomNum <= 80) {
                    return 1;
                } else {
                    return 2;
                }
            case 4:
                randomNum = Random.Range(0, 99);
                if (randomNum <= 30) {
                    return 0;
                } else if (randomNum <= 60) {
                    return 1;
                } else if (randomNum <= 80) {
                    return 2;
                } else {
                    return 3;
                }
            default:
                return 0;
        }
    }
    

    private Vector3 GenerateSpawnPosition(GameObject obstacle, int spawningIndex) {
        // If index is >=4, it is a moving block so substract for to get the correct starting index for size of block
        if (spawningIndex >= 4) {
            spawningIndex -= 4;
        }
        float obstacleWidth = obstacle.GetComponent<Renderer>().bounds.size.x;
        // Get random number for starting lane position
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
        //audioSource = GetComponent<AudioSource>();
        Obstacle.verticleSpeed = -50f;
        MovingObstacle.xBoundary = 25;
        speedIncreaseInterval = 10f;
        numObstacleTypes = 1;
        numLanes = 5;
        allowMovingObstacles = true;
        //audioSource.Play();
        AudioManager.Instance.StartAudio();
        StartCoroutine(SpawnObstacle());
    }
    public static void GameOver() {
        AudioManager.Instance.StopAudio();
        isGameOver = true;
        if (ScoreManager.Instance.currentPlayerScore > ScoreManager.Instance.highScorePlayerScore) {
            ScoreManager.Instance.SavePlayerData();
            Instance.highscoreText.text = ScoreManager.Instance.highScorePlayerName + " : " + Instance.FormatTime(ScoreManager.Instance.highScorePlayerScore);
        }
        ScoreManager.Instance.UploadScoreToLeaderboard();
        //Instance.StartCoroutine(LeaderboardAPI.Instance.PostScore(ScoreManager.Instance.currentPlayerName, ScoreManager.Instance.currentPlayerScore));
        //LeaderboardDB.AddPlayerScore(ScoreManager.Instance.currentPlayerName, ScoreManager.Instance.currentPlayerScore);
        ScoreManager.Instance.currentPlayerScore = 0;
        Instance.gameOverMenu.gameObject.SetActive(true);
        #if UNITY_WEBGL
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Transform quitButton = Instance.gameOverMenu.transform.Find("Quit Button");
            if (quitButton != null)
            {
                quitButton.gameObject.SetActive(false);
            }
        }
        #endif
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

    private string FormatTime(float time) // ABSTRACTION
    {
        // Convert total seconds to hours, minutes, and seconds
        int hours = Mathf.FloorToInt(time / 3600);
        int minutes = Mathf.FloorToInt((time % 3600) / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        // Return a formatted string in HH:MM:SS
        return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
    }
}
