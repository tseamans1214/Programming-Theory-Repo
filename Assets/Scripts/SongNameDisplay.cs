using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SongNameDisplay : UserInterfaceManager
{
    private TextMeshProUGUI songNameText;
    private int currentSongIndex;
    private List<string> authors;

    public float displayTime = 3f; // Time to display the song name
    public float fadeDuration = 1f; // Duration of the fade-out effect
    private Coroutine fadeCoroutine;
    private string currentText;

     
    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("OrientationManager").GetComponent<OrientationManager>().GetCurrentCanvasName() == "Canvas Landscape") {
            SetToLandscapeElements();
        } else {
            SetToPortraitElements();
        }
        currentSongIndex = AudioManager.Instance.currentSongIndex;
        authors = new List<string>();
        authors.Add("Matthew Pablo");
        authors.Add("Centurion_of_war");
        authors.Add("Juhani Junkala");
        authors.Add("HoliznaCC0");
        authors.Add("Nene");
        ShowSongName(AudioManager.Instance.playlist[currentSongIndex].name, authors[currentSongIndex]);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentSongIndex != AudioManager.Instance.currentSongIndex) {
            currentSongIndex = AudioManager.Instance.currentSongIndex;
            ShowSongName(AudioManager.Instance.playlist[currentSongIndex].name, authors[currentSongIndex]);
        }
    }

    public void ShowSongName(string songName, string author)
    {
        // Stop any ongoing fade-out to restart the display
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        //UpdateSongNameTextReference();

        // Set the song name and make the text fully visible
        currentText = "Now playing...\n" + songName + "\nBy:" + author;
        songNameText.text = currentText;
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

    void UpdateSongNameTextReference()
    {
        // Find all objects with the "PlayerScore" tag
        GameObject[] songNameTexts = GameObject.FindGameObjectsWithTag("SongNameText");

        foreach (GameObject obj in songNameTexts)
        {
            if (obj.activeInHierarchy) // Check if the GameObject is active
            {
                songNameText = obj.GetComponent<TextMeshProUGUI>();
                break;
            }
        }

        if (songNameText == null)
        {
            Debug.LogError("No active SongNameText text found!");
        }
    }

    public void UpdateSongText(int score)
    {
        if (songNameText == null)
            UpdateSongNameTextReference();

        if (songNameText != null)
        {
            songNameText.text = currentText;
        }
    }

    public override void SetToLandscapeElements() {
        songNameText = GameObject.FindGameObjectWithTag("SongNameTextL").GetComponent<TextMeshProUGUI>();
        //songNameText.text = currentText;
    }
    public override void SetToPortraitElements() {
        songNameText = GameObject.FindGameObjectWithTag("SongNameTextP").GetComponent<TextMeshProUGUI>();
        //songNameText.text = currentText;
    }
}
