using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Bson;
using MongoDB.Driver;
using dotenv.net;
using System;

public class LeaderboardDB : MonoBehaviour
{
    public static LeaderboardDB Instance;
    private MongoClient client;
    private IMongoDatabase database;

    private void Awake()
    {
        // To prevent multiple LeaderboardDB from being created, destroy it if it already exists
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        ConnectToDatabase();
    }

    private void ConnectToDatabase()
    {
        // Load the .env file
        DotEnv.Load();
        
        // Read values from the .env file
        string connectionString = Environment.GetEnvironmentVariable("MONGODB_URL");
        
        client = new MongoClient(connectionString);
        database = client.GetDatabase("cube-cruisin");
        Debug.Log("Connected to MongoDB!");
    }

    private void TestDatabaseConnection()
    {
        var collection = database.GetCollection<BsonDocument>("testCollection");
        var document = new BsonDocument { { "name", "Unity User" }, { "age", 25 } };
        collection.InsertOne(document);
        Debug.Log("Document inserted into MongoDB!");
    }

    public static void AddPlayerScore(string playerName, int playerScore) {
        var collection = Instance.database.GetCollection<BsonDocument>("highscores");
        var document = new BsonDocument { { "name", playerName }, { "score", playerScore } };
        collection.InsertOne(document);
        Debug.Log("Document inserted into MongoDB highscores!");
    }

}
