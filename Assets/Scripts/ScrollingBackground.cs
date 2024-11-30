using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground : Obstacle
{
    private Vector3 startPos;
    private float repeatWidth;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        repeatWidth = GetComponent<BoxCollider>().size.x / 2;
    }
    [SerializeField] private int direction;
    public override void Move() {
        transform.Translate(Vector3.forward * Time.deltaTime * verticleSpeed * direction);
        if (transform.position.z < startPos.z - 900) {
            transform.position = startPos;
        }
    }
}
