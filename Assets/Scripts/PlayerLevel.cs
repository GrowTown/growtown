using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class LevelThreshold
{
    public int level;
    public int xpThreshold;
}
public class PlayerLevel : MonoBehaviour
{
    [SerializeField] private List<LevelThreshold> levelThresholds = new List<LevelThreshold>();
    [SerializeField] private TextMeshProUGUI _previousLevel;
    [SerializeField] private TextMeshProUGUI _nextLevel;
    [SerializeField] private Slider _xpSlider;
    private Dictionary<int, int> _levelThresholds;
    int _levelOfPlayer=1;
    private const int ThresholdMargin = 5;
    private HashSet<int> _levelsAchieved = new HashSet<int>();
    

    public int CurrentPlayerLevel
    {
        get=>_levelOfPlayer;
        set
        {
            _levelOfPlayer = value;
            _previousLevel.text = UI_Manager.Instance.currentplayerLevelTxt.text;
            UI_Manager.Instance.currentplayerLevelTxt.text=_levelOfPlayer.ToString();
            _nextLevel.text = UI_Manager.Instance.currentplayerLevelTxt.text;
            UI_Manager.Instance.ShopManager.OnLevelChanged(_levelOfPlayer);
        }
    }

    private void Start()
    {
        InitializeLevelThresholds();
        UI_Manager.Instance.currentplayerLevelTxt.text = _levelOfPlayer.ToString();
    }

    private void InitializeLevelThresholds()
    {
        foreach (var threshold in levelThresholds)
        {
            _levelThresholds[threshold.level] = threshold.xpThreshold;
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

