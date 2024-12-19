using UnityEngine;
using UnityEngine.UI;

public class GlobalAudioControls : MonoBehaviour
{
    private AudioSource[] audioSources;
    private AudioSource playerSource;
    private bool isMuted =false;
    public Sprite muteImage;   // The mute button image
    public Sprite unmuteImage; // The unmute button image
    private Image muteButtonImage;

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
    } 

    public void ToggleMuteAll()
    {
        playerSource.mute = !playerSource.mute;
        AudioManager.Instance.ToggleMute();
        isMuted = !isMuted;
        if (isMuted) {
            muteButtonImage.sprite = muteImage;
        } else {
            muteButtonImage.sprite = unmuteImage;
        }
        
    }
    public void PlayNextSong() {
        AudioManager.Instance.NextSong();
    }
}
