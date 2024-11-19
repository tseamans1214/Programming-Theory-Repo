using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float verticleSpeed;
    int width;
    int height;
    //Color color;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.isGameOver == false) {
            Move();
        }
    }

    public virtual void Move() {
        transform.Translate(Vector3.forward * Time.deltaTime * verticleSpeed);
    }
}
