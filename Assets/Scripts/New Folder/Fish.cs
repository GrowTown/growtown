using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Fish
{
    public string fishName;
    public Sprite icon;
    public string rarity;
    public int value;
    public bool caught;
    public GameObject fightVisualPrefab;
    [Range(0.5f, 3f)] public float catchDifficulty = 1f;
}
