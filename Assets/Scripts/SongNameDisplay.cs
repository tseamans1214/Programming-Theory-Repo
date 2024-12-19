using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SongNameDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI songNameText;
    private int currentSongIndex;
    private List<string> authors;

    public float displayTime = 3f; // Time to display the song name
    public float fadeDuration = 1f; // Duration of the fade-out effect
    private Coroutine fadeCoroutine;
     
    // Start is called before the first frame update
    void Start()
    {
        currentSongIndex = AudioManager.Instance.currentSongIndex;
        authors = new List<string>();
        authors.Add("Matthew Pablo");
        authors.Add("Centurion_of_war");
        authors.Add("Juhani Junkala");
        ShowSongName(AudioManager.Instance.playlist[currentSongIndex].name, authors[currentSongIndex]);
        //songNameText.text = "Now playing... " + AudioManager.Instance.playlist[currentSongIndex].name;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentSongIndex != AudioManager.Instance.currentSongIndex) {
            currentSongIndex = AudioManager.Instance.currentSongIndex;
            ShowSongName(AudioManager.Instance.playlist[currentSongIndex].name, authors[currentSongIndex]);
            //songNameText.text = AudioManager.Instance.playlist[currentSongIndex].name;
            // Method to make text appear and vanish
        }
    }

    public void ShowSongName(string songName, string author)
    {
        // Stop any ongoing fade-out to restart the display
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        // Set the song name and make the text fully visible
        songNameText.text = "Now playing...\n" + songName + "\nBy:" + author;
        songNameText.color = new Color(songNameText.color.r, songNameText.color.g, songNameText.color.b, 1);

        // Start the fade-out coroutine after the display time
        fadeCoroutine = StartCoroutine(FadeOutText());
    }

    private IEnumerator FadeOutText()
    {
        // Wait for the display time
        yield return new WaitForSeconds(displayTime);

        // Gradually fade out the text
        float elapsedTime = 0f;
        Color originalColor = songNameText.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            songNameText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Ensure the text is fully transparent
        songNameText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
    }
}
