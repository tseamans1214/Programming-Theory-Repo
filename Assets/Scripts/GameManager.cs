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

    void SpawnObstacles() {


    }
    // Working 
    // private Vector3 GenerateSpawnPosition() {
    //     float cubeWidth = obstaclePrefab.GetComponent<Renderer>().bounds.size.x;
    //     float spawnPosX = Random.Range(cubeWidth, cubeWidth);
    //     float spawnPosZ = Random.Range(-700, -700);
    //     int randomNum = Random.Range(-2,3);

    //     Vector3 randomPos = new Vector3((cubeWidth + 2) * randomNum , 83.04436f, -700);

    //     return randomPos;
    // }

    private Vector3 GenerateSpawnPosition(GameObject obstacle, int spawningIndex) {
        //int randomIndex = Random.Range(0, 2);
        //spawningObstacle = obstaclePrefabs[randomIndex];
        float obstacleWidth = obstacle.GetComponent<Renderer>().bounds.size.x;
        //float spawnPosX = Random.Range(cubeWidth, cubeWidth);
        //float spawnPosZ = Random.Range(-700, -700);
        int randomNum = Random.Range(-2,3-spawningIndex);

        int offset;
        if (spawningIndex == 1) {
            offset = 4;
        }
        else if (spawningIndex == 2) {
            offset = 8;
        } else if (spawningIndex == 3) {
            offset = 12;
        } else {
            offset = 0;
        }

        Vector3 randomPos = new Vector3((10 * randomNum) + offset , 83.04436f, -700);

        return randomPos;
    }
    public void StartGame() {
        isGameOver = false;
        StartCoroutine(SpawnObstacle());
    }
    public static void GameOver() {
        isGameOver = true;
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
}
