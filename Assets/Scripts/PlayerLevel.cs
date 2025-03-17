using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

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
    private Dictionary<int, int> _levelThresholds=new Dictionary<int, int>();
    private int _currentXP = 0;
    private int _xpForCurrentLevel = 0;
    private int _xpForNextLevel = 0;
    int _levelOfPlayer=1;
    private const int ThresholdMargin = 5;
    private HashSet<int> _levelsAchieved = new HashSet<int>();
    

    public int CurrentPlayerLevel
    {
        get=>_levelOfPlayer;
        set
        {
            _levelOfPlayer = value;
            _previousLevel.text = _levelOfPlayer.ToString();
            UI_Manager.Instance.currentplayerLevelTxt.text=_levelOfPlayer.ToString();   
           // UI_Manager.Instance.ShopManager.OnLevelChanged(_levelOfPlayer);
        }
    }

    private void Start()
    {
        InitializeLevelThresholds();
        UpdateLevelXP();
        _xpSlider.minValue = 0;
        _xpSlider.value = 0;
        _xpSlider.maxValue = _xpForNextLevel - _xpForCurrentLevel;
        UI_Manager.Instance.currentplayerLevelTxt.text = _levelOfPlayer.ToString();
        _previousLevel.text = _levelOfPlayer.ToString();
        _nextLevel.text = (_levelOfPlayer + 1).ToString();

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
        int xpInCurrentLevel = xp - _xpForCurrentLevel;
        _xpSlider.value = Mathf.Clamp(xpInCurrentLevel, 0, _xpSlider.maxValue);

        foreach (var threshold in _levelThresholds)
        {
            int level = threshold.Key;
            int xpRequired = threshold.Value;
            if (xp >= xpRequired && xp <= xpRequired + ThresholdMargin && !_levelsAchieved.Contains(level))
            {
                CurrentPlayerLevel = level;
                _levelsAchieved.Add(level);
                _nextLevel.text =(level+1).ToString();
                UpdateLevelXP();
                UI_Manager.Instance.ShopManager.OnLevelChanged(_levelOfPlayer);
                UI_Manager.Instance.RewardsForLevel.LevelRewards($"level{level}");
                Debug.Log($"Level increased to {CurrentPlayerLevel} for XP: {xp}");
            }
        }
    }

    private void UpdateLevelXP()
    {
        _xpForCurrentLevel = _levelThresholds.ContainsKey(_levelOfPlayer) ? _levelThresholds[_levelOfPlayer] : 0;
        _xpForNextLevel = _levelThresholds.ContainsKey(_levelOfPlayer + 1) ? _levelThresholds[_levelOfPlayer + 1] : _xpForCurrentLevel + 100;
        _xpSlider.maxValue = _xpForNextLevel - _xpForCurrentLevel;
    }

}

