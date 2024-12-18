using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

using System.Security.Cryptography;
using System.Text;
using UnityEngine.Networking;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public string currentPlayerName;
    public int currentPlayerScore;
    public string highScorePlayerName;
    public int highScorePlayerScore;

    private string secretKey = "FKE9F_HNV9E9JFNG!_F-5IHMCNDITG69";
    private string apiUrl = "https://cube-cruisin-backend.vercel.app/scores";
    //private string apiUrl = Environment.GetEnvironmentVariable("BACKEND_URL") + "";
    private void Awake()
    {
        // To prevent multiple ScoreManagers from being created, destroy it if it already exists
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadPlayerData();
    }

    // Class to store player save data
    [System.Serializable]
    class SaveData
    {
        public string playerName;
        public int playerScore;
    }

    public void SavePlayerData()
    {
        SaveData data = new SaveData();
        data.playerName = currentPlayerName;
        data.playerScore = currentPlayerScore;

        highScorePlayerName = currentPlayerName;
        highScorePlayerScore = currentPlayerScore;

        string json = JsonUtility.ToJson(data);
    
        File.WriteAllText(Application.persistentDataPath + "/highscore.json", json);
    }

    public void LoadPlayerData()
    {
        string path = Application.persistentDataPath + "/highscore.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            highScorePlayerName = data.playerName;
            highScorePlayerScore = data.playerScore;
        }
    }

    // Database Leaderboard Data
    // Post Score
    public void UploadScoreToLeaderboard() {
        Instance.StartCoroutine(PostScore(currentPlayerName, currentPlayerScore));
    }

    [System.Serializable]
    public class ScorePayload
    {
        public string player;
        public int score;
        public string hash;

        public ScorePayload(string player, int score, string hash)
        {
            this.player = player;
            this.score = score;
            this.hash = hash;
        }
    }


    public IEnumerator PostScore(string player, int score)
    {
        string hash = GenerateHash(player, score, secretKey);
        ScorePayload payload = new ScorePayload(player, score, hash);

        string jsonPayload = JsonUtility.ToJson(payload);

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] jsonToSend = Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Score posted successfully!");
        }
        else
        {
            Debug.LogError($"Error posting score: {request.error}");
            Debug.LogError($"Response: {request.downloadHandler.text}");
        }
    }



    private string GenerateHash(string player, int score, string secretKey)
    {
        string data = player + score + secretKey;
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }

    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            string wrappedJson = $"{{\"items\":{json}}}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(wrappedJson);
            return wrapper.items;
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] items;
        }
    }

    public IEnumerator GetScores(System.Action<List<PlayerScore>> onSuccess, System.Action<string> onError)
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            try
            {
                // Deserialize JSON response
                string jsonResponse = request.downloadHandler.text;
                PlayerScore[] scores = JsonHelper.FromJson<PlayerScore>(jsonResponse);
                onSuccess(new List<PlayerScore>(scores));
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error parsing response: {ex.Message}");
                onError?.Invoke("Failed to parse server response.");
            }
        }
        else
        {
            Debug.LogError($"Error fetching scores: {request.error}");
            onError?.Invoke($"Error fetching scores: {request.error}");
        }
    }
}
