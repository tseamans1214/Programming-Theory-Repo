using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    private AudioSource audioSource;
    public List<AudioClip> playlist; // List of audio clips for the playlist
    private int currentSongIndex = 0; // Index of the current song

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Game" && GameManager.isGameOver == false) {
            // Check if the current song has finished playing
            if (!audioSource.isPlaying && playlist.Count > 0)
            {
                NextSong(); // Play the next song
            }
        }
    }

    void NextSong()
    {
        currentSongIndex = (currentSongIndex + 1) % playlist.Count; // Loop back to the first song
        PlaySong(currentSongIndex);
    }

     void PlaySong(int index)
    {
        if (index >= 0 && index < playlist.Count)
        {
            audioSource.clip = playlist[index];
            audioSource.Play();
        }
    }
    
    public void StartAudio()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            PlaySong(0);
        }
    }
    public void StopAudio() {
         if (audioSource != null) {
            audioSource.Stop();
        }
    }
}
