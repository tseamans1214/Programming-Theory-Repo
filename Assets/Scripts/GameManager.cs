using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // timer
    // timerHighScore
    public static bool isGameOver;
    public  float spawnRange = 9.0f;
    public float spawnRate = 1.0f;
    //private float cubeWidth;
    public GameObject obstaclePrefab;
    // Start is called before the first frame update
    void Start()
    {
        isGameOver = false;
        //cubeWidth = GetComponent<Renderer>().bounds.size.x;
        StartCoroutine(SpawnObstacle());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnObstacle() {
        while (isGameOver == false) {
            yield return new WaitForSeconds(spawnRate);
            Instantiate(obstaclePrefab, GenerateSpawnPosition(), obstaclePrefab.transform.rotation);
            //int index = Random.Range(0, targets.Count);
            //Instantiate(targets[index]);
        }
    }

    void SpawnObstacles() {


    }
    private Vector3 GenerateSpawnPosition() {
        float cubeWidth = obstaclePrefab.GetComponent<Renderer>().bounds.size.x;
        float spawnPosX = Random.Range(cubeWidth, cubeWidth);
        float spawnPosZ = Random.Range(-700, -700);
        int randomNum = Random.Range(-2,3);

        Vector3 randomPos = new Vector3((cubeWidth + 1) * randomNum , 83.04436f, -700);

        return randomPos;
    }
    void GameOver() {

    }
}
