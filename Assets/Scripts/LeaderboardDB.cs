using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Bson;
using MongoDB.Driver;
using dotenv.net;
using System;

public class LeaderboardDB : MonoBehaviour
{
    private MongoClient client;
    private IMongoDatabase database;

    void Start()
    {
        ConnectToDatabase();
        TestDatabaseConnection();
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

}
