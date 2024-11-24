using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float rotationSpeed = 500f; // Speed of rotation for smooth transitions

    private Quaternion targetRotation; // Target rotation
    private Vector3 targetPosition;    // Target position
    private float cubeWidth;           // Width of the cube
    public static int currentLane;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private AudioClip explodeSound;
    [SerializeField] private AudioClip jumpSound;
    private AudioSource playerAudio;

    void Start()
    {
        // Initialize target rotation and position
        targetRotation = transform.rotation;
        targetPosition = transform.position;

        // Calculate the width of the cube
        cubeWidth = GetComponent<Renderer>().bounds.size.x;

        currentLane = 3;

        playerAudio = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (GameManager.isGameOver == false) {
            PlayerMovement();
        }
    }

    void PlayerMovement() {
        // Check for key presses and set target rotation/position
        if (Input.GetKeyDown(KeyCode.LeftArrow) && currentLane > 1)
        {
            playerAudio.PlayOneShot(jumpSound, 1.0f);
            RotateAndMove(Vector3.forward, Vector3.left);
            currentLane += -1;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && currentLane < GameManager.numLanes)
        {
            playerAudio.PlayOneShot(jumpSound, 1.0f);
            RotateAndMove(Vector3.back, Vector3.right);
            currentLane += 1;
        }

        // Smoothly rotate and move the cube
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, rotationSpeed * Time.deltaTime);
    }

    void RotateAndMove(Vector3 rotationDirection, Vector3 moveDirection)
    {
        // Update target rotation
        targetRotation *= Quaternion.Euler(rotationDirection * 90);

        // Update target position
        targetPosition += moveDirection * (cubeWidth + 2);
    }

    private void OnCollisionEnter(Collision other)
    {
        // if player collides with an obstacle, explode and run GameOver method
        if (other.gameObject.CompareTag("Obstacle"))
        {
            playerAudio.PlayOneShot(explodeSound, 1.0f);
            Instantiate(explosionPrefab, transform.position, transform.rotation);
            GameManager.GameOver();
            Debug.Log("Game Over!");
            GetComponent<Collider>().enabled = false;
            GetComponent<MeshRenderer>().enabled = false;

        }
    }
}
