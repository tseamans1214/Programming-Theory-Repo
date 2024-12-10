using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LeaderboardUIHandler : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> leaderboardTexts;
    void Start() {
        StartCoroutine(ScoreManager.Instance.GetScores(OnScoresReceived, OnScoresError));
        
    }

    


    private void OnScoresReceived(List<PlayerScore> scores)
    {
        var i=0;
        foreach (var score in scores)
        {
            leaderboardTexts[i].text = (i+1) + ". " + score.player + " : " + FormatTime(score.score);
            i++;
            
        }
        while (i < 10) {
                leaderboardTexts[i].text = (i+1) + ". " + "NA";
                i++;
        }
    }

    private void OnScoresError(string error)
    {
        Debug.LogError($"Failed to retrieve scores: {error}");
    }
    public void GoToMainMenu() {
        SceneManager.LoadScene(0);
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
