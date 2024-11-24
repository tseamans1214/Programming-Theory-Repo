using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacle : Obstacle
{
    //[SerializeField] private float verticleSpeed;
    [SerializeField] private float horizontalSpeed;
    [SerializeField] private float xBoundary = 25;

    void Start() {
        horizontalSpeed = Random.Range(horizontalSpeed / 2, horizontalSpeed);
        //horizontalSpeed = 20;
        xBoundary = 25;
        int randomNum = Random.Range(0, 2);
        if (randomNum == 1) {
            horizontalSpeed *= -1;
        }
    }
    public override void Move() {
        transform.Translate(Vector3.forward * Time.deltaTime * verticleSpeed);

        float obstacleWidth = GetComponent<Renderer>().bounds.size.x;
        if (((transform.position.x - (obstacleWidth / 2)) < -xBoundary) || ((transform.position.x + (obstacleWidth / 2)) > xBoundary)) {
            horizontalSpeed *= -1;
        }
        
        transform.Translate(Vector3.right * Time.deltaTime * horizontalSpeed);

        if (transform.position.z < -1000) {
            Destroy(gameObject);
        }
    }
}
