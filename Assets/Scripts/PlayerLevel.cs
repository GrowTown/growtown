using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerLevel : MonoBehaviour
{
    int _levelOfPlayer=1;
    private const int ThresholdMargin = 5;
    Levels plevel;
    private Dictionary<int, int> _levelThresholds = new Dictionary<int, int>();
    private HashSet<int> _levelsAchieved = new HashSet<int>(); 

    public int CurrentPlayerLevel
    {
        get=>_levelOfPlayer;
        set
        {
            _levelOfPlayer = value;
            UI_Manager.Instance.currentplayerLevelTxt.text=_levelOfPlayer.ToString();
            UI_Manager.Instance.ShopManager.OnLevelChanged(_levelOfPlayer);
        }

    }

    private void Start()
    {
        InitializeLevelThresholds();
        UI_Manager.Instance.currentplayerLevelTxt.text = _levelOfPlayer.ToString();
    }
    bool isLevel1=false;
    bool isLevel2=false;

    private void InitializeLevelThresholds()
    {
        for (int level = 2; level <= 10; level++)
        {
            _levelThresholds[level] = level * 50; 
        }
    }

    internal void UpdatePlayerLevel(int xp)
    {
        foreach (var threshold in _levelThresholds)
        {
            int level = threshold.Key;
            int xpRequired = threshold.Value;
            if (xp >= xpRequired && xp <= xpRequired + ThresholdMargin && !_levelsAchieved.Contains(level))
            {
                CurrentPlayerLevel = level;
                _levelsAchieved.Add(level); 

                UI_Manager.Instance.RewardsForLevel.LevelRewards($"level{level}");
                Debug.Log($"Level increased to {CurrentPlayerLevel} for XP: {xp}");
            }
        }
    }
}
public enum Levels
{
     level1=100,
    level2=150,
}
