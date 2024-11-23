using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] protected float verticleSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.isGameOver == false) {
            Move();
        }
    }

    public virtual void Move() {
        transform.Translate(Vector3.forward * Time.deltaTime * verticleSpeed);

        if (transform.position.z < -1000) {
            Destroy(gameObject);
        }
    }
}
