using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    private PlayerInput playerInput;

    private void OnEnable()
    {
        // Initialize and enable the Input System
        if (playerInput == null)
        {
            playerInput = new PlayerInput();
        }
        playerInput.Player.Enable();

        // Subscribe to the "Move" actions
        playerInput.Player.MoveLeft.performed += MoveLeft;
        playerInput.Player.MoveRight.performed += MoveRight;
        playerInput.Player.NextSong.performed += NextSong;
    }

    private void OnDisable()
    {
        if (playerInput != null)
        {
            playerInput.Player.MoveLeft.performed -= MoveLeft;
            playerInput.Player.MoveRight.performed -= MoveRight;
            playerInput.Player.NextSong.performed -= NextSong;
            playerInput.Player.Disable();
        }
    }

    void Start()
    {
        // Initialize target rotation and position
        targetRotation = transform.rotation;
        targetPosition = transform.position;

        // Calculate the width of the cube
        cubeWidth = GetComponent<Renderer>().bounds.size.x;

        currentLane = 3;

        playerAudio = GetComponent<AudioSource>();

        playerInput.Player.MoveLeft.performed += MoveLeft;
        playerInput.Player.MoveRight.performed += MoveRight;
    }

    void Update()
    {
        if (GameManager.isGameOver == false) {
            PlayerMovement();
        }
    }

    void PlayerMovement() {
        // Smoothly rotate and move the cube
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, rotationSpeed * Time.deltaTime);
    }

    public void MoveLeft(InputAction.CallbackContext ctx) {
        if (currentLane > 1 && GameManager.isGameOver == false)
         {
            playerAudio.PlayOneShot(jumpSound, 1.0f);
            RotateAndMove(Vector3.forward, Vector3.left);
            currentLane += -1;
        }
        
    }

    public void MoveRight(InputAction.CallbackContext ctx) {
        if (currentLane < GameManager.numLanes && GameManager.isGameOver == false)
        {
            playerAudio.PlayOneShot(jumpSound, 1.0f);
            RotateAndMove(Vector3.back, Vector3.right);
            currentLane += 1;
        }
        
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
            GetComponent<Collider>().enabled = false;
            GetComponent<MeshRenderer>().enabled = false;

        }
    }

    // Player Menu button shortcuts
    public void NextSong(InputAction.CallbackContext ctx) {
        AudioManager.Instance.NextSong();
    }
}
