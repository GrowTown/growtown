using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int XP;
    public int Energy;
    public int Water;
    public int Score;
    public int Level;
    public bool StartPackBought;
    public Dictionary<string, int> Inventory = new Dictionary<string, int>(); 
}


public class LocalSaveManager : MonoBehaviour
{
    private string filePath;

    private void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "playerData.json");
        Debug.Log("Save path: " + filePath);
    }

    public void SavePlayerData(PlayerData data)
    {
        string json = JsonUtility.ToJson(new SerializablePlayerData(data));
        File.WriteAllText(filePath, json);
        Debug.Log("Player data saved locally: " + json);
    }

    public PlayerData LoadPlayerData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            SerializablePlayerData serializableData = JsonUtility.FromJson<SerializablePlayerData>(json);
            Debug.Log("Loaded data: " + json);
            return serializableData.ToPlayerData();
        }
        else
        {
            Debug.LogWarning("No save file found!");
            return new PlayerData();
        }
    }
}


[System.Serializable]
public class SerializablePlayerData
{
    public int XP;
    public int Energy;
    public int Water;
    public int Score;
    public int Level;
    public bool StartPackBought;
    public List<string> ItemNames = new List<string>();
    public List<int> ItemCounts = new List<int>();

    public SerializablePlayerData(PlayerData data)
    {
        XP = data.XP;
        Energy = data.Energy;
        Water = data.Water;
        Score = data.Score;
        Level = data.Level;
        StartPackBought= data.StartPackBought;

        foreach (var item in data.Inventory)
        {
            ItemNames.Add(item.Key);
            ItemCounts.Add(item.Value);
        }
    }

    public PlayerData ToPlayerData()
    {
        var data = new PlayerData
        {
            XP = XP,
            Energy = Energy,
            Water = Water,
            Score = Score,
            Level = Level,
            StartPackBought = StartPackBought

    };

        for (int i = 0; i < ItemNames.Count; i++)
        {
            data.Inventory[ItemNames[i]] = ItemCounts[i];
        }

        return data;
    }
}



