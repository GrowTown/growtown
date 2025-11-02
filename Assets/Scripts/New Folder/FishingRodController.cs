using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FishingRodController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform lineStartPoint;   // tip of the rod
    [SerializeField] private GameObject bobberPrefab;    // bobber prefab
    [SerializeField] private float castDistance = 4f;    // how far the bobber spawns
    [SerializeField] private float waterHeight = 0.2f;   // y level for water surface

    private LineRenderer fishingLine;
    private GameObject currentBobber;

    private void Awake()
    {
        fishingLine = GetComponent<LineRenderer>();
        fishingLine.enabled = false;
        fishingLine.useWorldSpace = true;

        // basic setup
        fishingLine.startWidth = 0.03f;
        fishingLine.endWidth = 0.02f;

        // fallback material
        if (fishingLine.material == null)
        {
            fishingLine.material = new Material(Shader.Find("Sprites/Default"));
            fishingLine.startColor = Color.black;
            fishingLine.endColor = Color.black;
        }

        EnsureReferences();
    }

    private void OnValidate()
    {
        if (fishingLine == null)
            fishingLine = GetComponent<LineRenderer>();

        EnsureReferences();
    }

    private void EnsureReferences()
    {
        if (lineStartPoint == null)
        {
            lineStartPoint = transform.Find("Linerenderpoint");
            if (lineStartPoint == null)
                lineStartPoint = transform.Find("LineRendererPoint");
        }

        if (fishingLine != null && fishingLine.positionCount == 0)
        {
            fishingLine.positionCount = 2;
            fishingLine.SetPosition(0, transform.position);
            fishingLine.SetPosition(1, transform.position);
        }

        if (bobberPrefab == null)
            Debug.LogWarning($"{nameof(FishingRodController)}: bobberPrefab is not assigned.", this);

        if (lineStartPoint == null)
            Debug.LogWarning($"{nameof(FishingRodController)}: lineStartPoint is not assigned.", this);
    }

    public void ConfigureLineStart(Transform startPoint)
    {
        if (startPoint != null)
        {
            lineStartPoint = startPoint;
            if (fishingLine != null && fishingLine.enabled)
                fishingLine.SetPosition(0, lineStartPoint.position);
        }

        EnsureReferences();
    }

    // 🎬 Animation Event: CAST
    public void SpawnBobber()
    {
        // cleanup old
        if (currentBobber != null)
            Destroy(currentBobber);

        if (lineStartPoint == null)
        {
            Debug.LogWarning($"{nameof(FishingRodController)}: Cannot spawn bobber because lineStartPoint is missing.", this);
            return;
        }

        // spawn bobber in front of rod
        Vector3 spawnPos = lineStartPoint.position + transform.forward * castDistance;
        spawnPos.y = waterHeight;

        currentBobber = Instantiate(bobberPrefab, spawnPos, Quaternion.identity);

        fishingLine.positionCount = 2;
        fishingLine.enabled = true;
        fishingLine.SetPosition(0, lineStartPoint.position);
        fishingLine.SetPosition(1, currentBobber.transform.position);
    }

    private void Update()
    {
        if (fishingLine.enabled && lineStartPoint != null && currentBobber != null)
        {
            // update line each frame
            fishingLine.SetPosition(0, lineStartPoint.position);              // rod tip
            fishingLine.SetPosition(1, currentBobber.transform.position);     // bobber
        }
    }

    // 🎬 Animation Event: END (catch or fail)
    public void EndFishing()
    {
        if (currentBobber != null)
            Destroy(currentBobber);

        fishingLine.enabled = false;
        fishingLine.positionCount = 0;
    }
}
