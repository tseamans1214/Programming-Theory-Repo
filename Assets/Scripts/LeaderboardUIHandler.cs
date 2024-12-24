using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;

public class LeaderboardUIHandler : UserInterfaceManager
{
    [SerializeField] private List<TextMeshProUGUI> leaderboardPlayerTexts;
    [SerializeField] private List<TextMeshProUGUI> leaderboardScoreTexts;
    [SerializeField] private List<TextMeshProUGUI> leaderboardPlayerTextsLandscape;
    [SerializeField] private List<TextMeshProUGUI> leaderboardScoreTextsLandscape;

    [SerializeField] private List<TextMeshProUGUI> leaderboardPlayerTextsPortrait;
    [SerializeField] private List<TextMeshProUGUI> leaderboardScoreTextsPortrait;
    private int currentIndex = 0;
    private List<PlayerScore> leaderboardScores;

    private PlayerInput playerInput;

    private void OnEnable()
    {
        // Initialize and enable the Input System
        if (playerInput == null)
        {
            playerInput = new PlayerInput();
        }
        playerInput.UI.Enable();

        // Subscribe to the "Move" actions
        playerInput.UI.IncrementDown.performed += IncrementDown;
        playerInput.UI.IncrementUp.performed += IncrementUp;
    }

    private void OnDisable()
    {
        if (playerInput != null)
        {
            playerInput.UI.IncrementDown.performed -= IncrementDown;
            playerInput.UI.IncrementUp.performed -= IncrementUp;
            playerInput.UI.Disable();
        }
    }
    void Start() {
        currentIndex = 0;
        StartCoroutine(ScoreManager.Instance.GetScores(OnScoresReceived, OnScoresError));
        
    }

    


    private void OnScoresReceived(List<PlayerScore> scores)
    {
        leaderboardScores = scores;
        if (GameObject.Find("OrientationManager").GetComponent<OrientationManager>().GetCurrentCanvasName() == "Canvas Landscape") {
            SetToLandscapeElements();
            
        } else {
            SetToPortraitElements();
            //leaderboardPlayerTexts = leaderboardPlayerTextsPortrait;
        }
        PopulateLeaderboardScores();
    }
    private void PopulateLeaderboardScores() {
        var i=0;
        for (var j=currentIndex; j<leaderboardScores.Count && j<currentIndex+10; j++)
        {
            leaderboardPlayerTexts[i].text = (j+1) + ". " + leaderboardScores[j].player;
            leaderboardScoreTexts[i].text = FormatTime(leaderboardScores[j].score) + "";
            i++;
            
        }
        while (i < 10) {
                leaderboardPlayerTexts[i].text = (currentIndex+i+1) + ". " + "NA";
                leaderboardScoreTexts[i].text = "00:00:00";
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

    public void IncrementLeaderboardIndex(int incrementAmount) {
        if ((incrementAmount < 0 && currentIndex + incrementAmount >= 0) || 
            (incrementAmount > 0 && currentIndex + incrementAmount < 100)) {
            currentIndex += incrementAmount;
        }
        PopulateLeaderboardScores();
    }
    // Called when the input system triggers
    public void IncrementDown(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            IncrementLeaderboardIndex(-10);
        }
    }
    public void IncrementUp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            IncrementLeaderboardIndex(10);
        }
    }

    public override void SetToLandscapeElements() {
        if (leaderboardScores != null) {
            //StartCoroutine(ScoreManager.Instance.GetScores(OnScoresReceived, OnScoresError));
            leaderboardPlayerTexts = leaderboardPlayerTextsLandscape;
            leaderboardScoreTexts = leaderboardScoreTextsLandscape;
            PopulateLeaderboardScores();
        }
        //leaderboardPlayerTexts = leaderboardPlayerTextsLandscape;
        //PopulateLeaderboardScores();
    }
    public override void SetToPortraitElements() {
         if (leaderboardScores != null) {
            leaderboardPlayerTexts = leaderboardPlayerTextsPortrait;
            leaderboardScoreTexts = leaderboardScoreTextsPortrait;
            PopulateLeaderboardScores();
        }
        //leaderboardPlayerTexts = leaderboardPlayerTextsPortrait;
        //PopulateLeaderboardScores();
        
    }
}
