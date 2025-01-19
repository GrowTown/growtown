using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class PlayerLevel : MonoBehaviour
{
    int _levelOfPlayer=1;
    int firstLevel = 100;
    int secondLevel = 150;
    Levels plevel;

    public int CurrentPlayerLevel
    {
        get=>_levelOfPlayer;
        set
        {
            _levelOfPlayer = value;
            UI_Manager.Instance.currentplayerLevelTxt.text=_levelOfPlayer.ToString();
        }

    }


    private void Start()
    { 
        UI_Manager.Instance.currentplayerLevelTxt.text=_levelOfPlayer.ToString();
    }
    bool isLevel1=false;
    bool isLevel2=false;

    internal void UpdatetingthePlayerLevel(int Xp)
    {
        // Define acceptable ranges for XP thresholds
        const int level1Threshold = 100;
        const int level2Threshold = 150;
        const int thresholdMargin = 5; // Allowable deviation
        if (Xp >= level1Threshold && Xp - level1Threshold <= thresholdMargin && !isLevel1) // Within 100 to 105
        {
            CurrentPlayerLevel += 1;
            isLevel1 = true;
            GameManager.Instance.LevelRewards("level1");
            Debug.Log($"Level increased to {CurrentPlayerLevel} for XP: {Xp}");
        }
        else if (Xp >= level2Threshold && Xp - level2Threshold <= thresholdMargin && !isLevel2) // Within 150 to 155
        {
            CurrentPlayerLevel += 1;
            isLevel2 = true;
            GameManager.Instance.LevelRewards("level2");
            Debug.Log($"Level increased to {CurrentPlayerLevel} for XP: {Xp}");
        }
        else
        {
            Debug.Log($"XP: {Xp} did not meet any level-up criteria.");
        }
    }


}
public enum Levels
{
     level1=100,
    level2=150,
}
