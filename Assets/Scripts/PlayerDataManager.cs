using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public int xpPoints;
    public int energyPoints;
    public int waterPoints;
    public int currentScore;
    public int playerLevel;
    public List<string> inventoryItems = new List<string>();

    // Constructor to initialize with default values
    public PlayerData()
    {
        xpPoints = 0;
        energyPoints = 100;
        waterPoints = 100;
        currentScore = 0;
        playerLevel = 1;
    }
}

public class PlayerDataManager : MonoBehaviour
{
    private const string PlayerDataKey = "PlayerData";

    public PlayerData playerData = new PlayerData();

    private void Start()
    {
        LoadPlayerData();
    }

    // Save data to PlayerPrefs as JSON
    public void SavePlayerData()
    {
        string jsonData = JsonUtility.ToJson(playerData);
        PlayerPrefs.SetString(PlayerDataKey, jsonData);
        PlayerPrefs.Save();
        Debug.Log("Player data saved: " + jsonData);
    }

    // Load data from PlayerPrefs
    public void LoadPlayerData()
    {
        if (PlayerPrefs.HasKey(PlayerDataKey))
        {
            string jsonData = PlayerPrefs.GetString(PlayerDataKey);
            playerData = JsonUtility.FromJson<PlayerData>(jsonData);
            Debug.Log("Player data loaded: " + jsonData);
        }
        else
        {
            Debug.Log("No saved data found. Initializing with default values.");
        }
    }

    // Clear player data
    public void ResetPlayerData()
    {
        PlayerPrefs.DeleteKey(PlayerDataKey);
        playerData = new PlayerData();
        Debug.Log("Player data reset.");
    }

    // Example methods to update player data
    public void AddXp(int amount)
    {
        playerData.xpPoints += amount;
        SavePlayerData();
    }

    public void UpdateInventory(string item)
    {
        if (!playerData.inventoryItems.Contains(item))
        {
            playerData.inventoryItems.Add(item);
        }
        SavePlayerData();
    }

    public void SetPlayerLevel(int level)
    {
        playerData.playerLevel = level;
        SavePlayerData();
    }
}
