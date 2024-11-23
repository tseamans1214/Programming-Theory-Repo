using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public string currentPlayerName;
    public int currentPlayerScore;
    public string highScorePlayerName;
    public int highScorePlayerScore;
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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
}
