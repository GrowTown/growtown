using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRelativeFog : MonoBehaviour
{
    [Header("Fog Settings")]
    public Color fogColor = Color.gray;
    public float fogDensity = 0.02f;
    public float clearRadius = 10f;   // clear bubble around the character
    public float fogThickness = 40f;  // distance until full fog

    [Header("References")]
    public Transform player; // assign your player character here

    void Start()
    {
        if (player == null)
        {
            Debug.LogWarning("No player assigned to CharacterRelativeFog!");
            enabled = false;
            return;
        }

        // Enable fog globally
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = fogDensity;
    }

    void LateUpdate()
    {
        if (player == null) return;

        // Keep fog relative to the player’s position
        float start = clearRadius;
        float end = clearRadius + fogThickness;

        RenderSettings.fogStartDistance = start;
        RenderSettings.fogEndDistance = end;
    }
}
