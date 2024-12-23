using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using mk.profanity;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TitleUIHandler : UserInterfaceManager
{
    [SerializeField] private TMP_InputField playerNameField;
    [SerializeField] private TextMeshProUGUI highscoreText;
    [SerializeField] private GameObject profanityPanel;
    [SerializeField] private GameObject profanityPanelL;
    [SerializeField] private GameObject profanityPanelP;

    ProfanityFilter profanityFilter;

    void Start()
    {
        if (GameObject.Find("OrientationManager").GetComponent<OrientationManager>().GetCurrentCanvasName() == "Canvas Landscape") {
            SetToLandscapeElements();
        } else {
            SetToPortraitElements();
        }
        profanityFilter = new ProfanityFilter();
        ScoreManager.Instance.LoadPlayerData();
        if (ScoreManager.Instance.highScorePlayerScore > 0) {
            highscoreText.text = ScoreManager.Instance.highScorePlayerName + " : " + FormatTime(ScoreManager.Instance.highScorePlayerScore);
        } else {
            highscoreText.text = "None Recorded";
        }
        #if UNITY_WEBGL
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Transform quitButton = GameObject.Find("Quit Button").transform;//Instance.gameOverMenu.transform.Find("Quit Button");
            if (quitButton != null)
            {
                quitButton.gameObject.SetActive(false);
            }
        }
        #endif
    }
    public void StartGame() {
        if (ScoreManager.Instance != null)
        {
            if (playerNameField.text != "") {
                ScoreManager.Instance.currentPlayerName = playerNameField.text;
            } else {
                ScoreManager.Instance.currentPlayerName = "No Name";
            }
        }
        AudioManager.Instance.StartAudio();
        SceneManager.LoadScene(1);
    }

    public void QuitGame() {
        #if UNITY_EDITOR
                EditorApplication.ExitPlaymode();
        #else
                Application.Quit(); // original code to quit Unity player
        #endif
    }

    public void GoToLeaderboard() {
        SceneManager.LoadScene(2);
        //LeaderboardDB.FetchPlayerScores();
    }
    public void GoToDirections() {
        SceneManager.LoadScene(3);
    }
    public void GoToCredits() {
        SceneManager.LoadScene(4);
    }

    private string FormatTime(float time)
    {
        // Convert total seconds to hours, minutes, and seconds
        int hours = Mathf.FloorToInt(time / 3600);
        int minutes = Mathf.FloorToInt((time % 3600) / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        // Return a formatted string in HH:MM:SS
        return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
    }
    public void CheckNameForProfanity() {
        bool containsProfanity = profanityFilter.ContainsProfanity(playerNameField.text); 
        if (containsProfanity) {
            profanityPanel.SetActive(true);
           
        } else {
            StartGame();
        }
    }

    public override void SetToLandscapeElements() {
        playerNameField = GameObject.FindGameObjectWithTag("PlayerNameInputL").GetComponent<TMP_InputField>();
        highscoreText = GameObject.FindGameObjectWithTag("HighscoreTextL").GetComponent<TextMeshProUGUI>();

        if (profanityPanel.activeSelf) {
            profanityPanel = profanityPanelL;
            profanityPanel.SetActive(true);
        } else {
            profanityPanel = profanityPanelL;
        }
        // Update Values
        if (ScoreManager.Instance) {
            ScoreManager.Instance.LoadPlayerData();
            if (ScoreManager.Instance.highScorePlayerScore > 0) {
                highscoreText.text = ScoreManager.Instance.highScorePlayerName 
                    + " : " + FormatTime(ScoreManager.Instance.highScorePlayerScore);
            } else {
                highscoreText.text = "None Recorded";
            }
        }
    }
    public override void SetToPortraitElements() {
        playerNameField = GameObject.FindGameObjectWithTag("PlayerNameInputP").GetComponent<TMP_InputField>();
        highscoreText = GameObject.FindGameObjectWithTag("HighscoreTextP").GetComponent<TextMeshProUGUI>();

        if (profanityPanel.activeSelf) {
            profanityPanel = profanityPanelP;
            profanityPanel.SetActive(true);
        } else {
            profanityPanel = profanityPanelP;
        }
        // Update Values
        if (ScoreManager.Instance) {
            if (ScoreManager.Instance.highScorePlayerScore > 0) {
                highscoreText.text = ScoreManager.Instance.highScorePlayerName 
                    + " : " + FormatTime(ScoreManager.Instance.highScorePlayerScore);
            } else {
                highscoreText.text = "None Recorded";
            }
        }
    }
}
