using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameDataManager : Singleton<GameDataManager>
{
    private static string savePath => Application.persistentDataPath + "/gameData.json";
    public static GameData gameData = new GameData();

    public static void Save()
    {
        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Game saved to: " + savePath);
    }

    public static void Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            gameData = JsonUtility.FromJson<GameData>(json);
            Debug.Log("Game loaded.");
        }
        else
        {
            Debug.LogWarning("No save file found, creating new data.");
            gameData = new GameData();
        }
    }
    
    public static void ResetData()
    {
        gameData = new GameData();  
        Save();  
        Debug.Log("Game data reset!");
    }

    private void Awake()
    {
        Load();
        DontDestroyOnLoad(gameObject);
    }

    private void OnApplicationQuit()
    {
        Save();
    }
}
