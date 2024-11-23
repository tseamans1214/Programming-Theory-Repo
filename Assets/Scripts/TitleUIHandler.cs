using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TitleUIHandler : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerNameField;
    [SerializeField] private TextMeshProUGUI highScoreText;

    void Start()
    {
        ScoreManager.Instance.LoadPlayerData();
        if (ScoreManager.Instance.highScorePlayerScore > 0) {
            highScoreText.text = ScoreManager.Instance.highScorePlayerName + " : " + FormatTime(ScoreManager.Instance.highScorePlayerScore);
        } else {
            highScoreText.text = "None Recorded";
        }
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
        SceneManager.LoadScene(1);
    }

    public void QuitGame() {
        #if UNITY_EDITOR
                EditorApplication.ExitPlaymode();
        #else
                Application.Quit(); // original code to quit Unity player
        #endif
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
}
