using System.Collections;
using System.Collections.Generic;
//using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlobalAudioMuteToggle : MonoBehaviour
{
    //public static GlobalAudioMuteToggle Instance { get; private set; }
    private AudioSource[] audioSources;
    private AudioSource playerSource;
    private bool isMuted =false;
    public Sprite muteImage;   // The mute button image
    public Sprite unmuteImage; // The unmute button image
    private Image muteButtonImage;

    private void Awake()
    {
        // if (Instance != null)
        // {
        //     Destroy(gameObject);
        //     return;
        // }
        // Instance = this;
        // DontDestroyOnLoad(gameObject);

        // if (isMuted) {
        //     muteButton.gameObject.GetComponent<Image>().sprite = muteImage;
        // } else {
        //     muteButton.gameObject.GetComponent<Image>().sprite = unmuteImage;
        // }
    }

    void Start()
    {
        audioSources = FindObjectsOfType<AudioSource>();
        playerSource = GameObject.Find("Player").GetComponent<AudioSource>();
        muteButtonImage = GameObject.Find("Mute Button").GetComponent<Image>();
        if (AudioManager.Instance.isMuted()){
            playerSource.mute = true;
            isMuted = true;
            muteButtonImage.sprite = muteImage;
        } else {
            isMuted = false;
        }
        //muteButtonImage = GameObject.Find("Mute Button").GetComponent<Image>();
        //isMuted = false;
    } 
    void Update() {
    }

    public void ToggleMuteAll()
    {
        // audioSources = FindObjectsOfType<AudioSource>();
        // foreach (AudioSource source in audioSources)
        // {
        //     source.mute = !source.mute;
        // }
        playerSource.mute = !playerSource.mute;
        AudioManager.Instance.ToggleMute();
        isMuted = !isMuted;
        if (isMuted) {
            muteButtonImage.sprite = muteImage;
        } else {
            muteButtonImage.sprite = unmuteImage;
        }
        
    }
}
