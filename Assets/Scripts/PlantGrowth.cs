using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;

public class PlantGrowth : MonoBehaviour
{

    [SerializeField] internal SkinnedMeshRenderer plantMesh;
    internal Timer _initialGrowTimer;
    private Timer _afterwateredGrowTimer;
    [SerializeField] Transform[] tomatoSpawnPoints;
    float cuttingHight = 0.2f;
    private WaveManager waveManager;
    internal bool IsTileWatered;
    public int initialgrowthTime = 1;
    public int AfterWateredgrowthTime = 1;
    float _currentGrowth;
    double _currentTimer;
    internal Coroutine InitialCoroutine;
    internal Coroutine AfterWateredCoroutine;


    public float CurrentGrowth
    {
        get => _currentGrowth;
        set => _currentGrowth = value;
    }

    public double CurrentTimer
    {
        get => _currentTimer;
        set => _currentTimer = value;
    }

    void Start()
    {
        waveManager = FindObjectOfType<WaveManager>();

    }

    internal bool isWateredDuringWithering = false;
    internal bool isNotWateredDuringWithering = false;
    internal IEnumerator InitialGrowPlant()
    {

        _initialGrowTimer = this.gameObject.AddComponent<Timer>();
        _initialGrowTimer.Initialize("Plant Growth - Initial", DateTime.Now, TimeSpan.FromMinutes(initialgrowthTime)); // Initial growth
        _initialGrowTimer.StartTimer();
        UI_Manager.Instance.isTimerOn = true;
        float totalGrowthTime = (float)_initialGrowTimer.timeToFinish.TotalSeconds;

        while (_initialGrowTimer.secondsLeft > 0)
        {
            Debug.Log("initialTimer :: " + _initialGrowTimer.secondsLeft);
            CurrentTimer = totalGrowthTime - _initialGrowTimer.secondsLeft;
            float growthProgress = (float)(1.0f - (_initialGrowTimer.secondsLeft / totalGrowthTime));
            // Update blend shape only up to 50%
            if (!IsTileWatered)
            {
                plantMesh.SetBlendShapeWeight(0, growthProgress * 100f);
                CurrentGrowth = growthProgress;
                if (growthProgress >= 0.5f && !isWateredDuringWithering)
                {
                    GameManager.Instance.Withering();
                    isNotWateredDuringWithering = true;
                    _initialGrowTimer.StopTimer();
                    if (!GameManager.Instance.isplantGrowthCompleted)
                    {
                        UI_Manager.Instance.FieldGrid.coveredtiles.Clear();
                        UI_Manager.Instance.TriggerZoneCallBacks.CompleteAction();
                        GameManager.Instance.isplantGrowthCompleted = true;
                    }
                    Destroy(_initialGrowTimer);
                    yield break;
                }
            }
            yield return null;
        }

    }
    public IEnumerator AfterWateredTileGrowth(double currentTimer)
    {

        _afterwateredGrowTimer = this.gameObject.AddComponent<Timer>();
        var updatedTime = Mathf.Max(0, (int)(AfterWateredgrowthTime * 60 - currentTimer)); // Convert to seconds
        _afterwateredGrowTimer.Initialize("Plant Growth - After Watering", DateTime.Now, TimeSpan.FromSeconds(updatedTime));
        _afterwateredGrowTimer.StartTimer();
        float totalGrowthTime = (float)(_afterwateredGrowTimer.timeToFinish.TotalSeconds);
        while (_afterwateredGrowTimer.secondsLeft > 0)
        {

            float growthProgress = Mathf.Lerp(CurrentGrowth, 1.0f, (float)(1.0f - (_afterwateredGrowTimer.secondsLeft / totalGrowthTime)));
            Debug.Log("AfterWatered :: " + _afterwateredGrowTimer.secondsLeft);

            if (!isNotWateredDuringWithering)
            {
                plantMesh.SetBlendShapeWeight(0, growthProgress * 100f);
                isWateredDuringWithering = true;
            }

            yield return null;
        }

        _afterwateredGrowTimer.TimerFinishedEvent.AddListener(delegate
        {
            OnGrowthComplete();

            StopCoroutine(AfterWateredCoroutine);
            Destroy(_afterwateredGrowTimer);
        });
    }




    /* private void OnMouseDown()
     {
         TimerToolTip.ShowTimerStatic(this.gameObject);
     }*/

    private void OnGrowthComplete()
    {
        plantMesh.SetBlendShapeWeight(0, 100f);
        Debug.Log("Plant growth complete!");
        SpawnTomatoes();
    }


    private void SpawnTomatoes()
    {
        foreach (Transform spawnPoint in tomatoSpawnPoints)
        {
            var insta = Instantiate(UI_Manager.Instance.tomato, spawnPoint.position, Quaternion.identity);
            insta.transform.SetParent(spawnPoint.transform);
            UI_Manager.Instance.spawnTomatosForGrowth.Add(insta);

        }
        Debug.Log("Tomatoes spawned at designated points.");
        var plantCount = UI_Manager.Instance.GrowthStartedPlants.Count;
        GameManager.Instance.spawnedTomatoesCount = plantCount * 5;
        if (UI_Manager.Instance.spawnTomatosForGrowth.Count == GameManager.Instance.spawnedTomatoesCount)
        {
            UI_Manager.Instance.isPlantGrowthCompleted = true;
        }
    }
    bool loopOnce = false;
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            if (!UI_Manager.Instance.sickleWeapon.activeSelf && GameManager.Instance.isCutting)
            {
                UI_Manager.Instance.sickleWeapon.SetActive(true);
            }

            if (GameManager.Instance.isCutting && UI_Manager.Instance.sickleWeapon.activeSelf)
            {
                this.gameObject.transform.DOMoveY(cuttingHight, 0.1f);
                GameManager.Instance.isHarvestCompleted = true;
                /*
                                if (UI_Manager.Instance.GrownPlantsToCut.Contains(this.gameObject))
                                {
                                    UI_Manager.Instance.GrownPlantsToCut.Remove(this.gameObject);
                                    Destroy(this.gameObject); 
                                }*/

                if (UI_Manager.Instance.GrownPlantsToCut.ContainsKey(UI_Manager.Instance.FieldManager.CurrentFieldID))
                {
                    // Access the list for the current field
                    List<GameObject> plantList = UI_Manager.Instance.GrownPlantsToCut[UI_Manager.Instance.FieldManager.CurrentFieldID];


                    // Loop through the plants associated with tiles
                    foreach (var entry in UI_Manager.Instance.spawnPlantsForGrowth)
                    {
                        GameObject tile = entry.Key; // Tile GameObject
                        List<GameObject> plants = entry.Value; // List of plants on this tile

                        if (plants.Contains(this.gameObject)) // Check if this plant is part of the current tile
                        {
                            plants.Remove(this.gameObject); // Remove the plant from the list

                            // Check if all plants for this tile are destroyed
                            if (plants.Count == 0)
                            {
                                // Change the tile color (or mark it as covered)
                                UI_Manager.Instance.FieldGrid.AddCoveredTile(tile);
                            }

                            break; // Exit the loop once the plant is handled
                        }
                    }

                    if (plantList.Contains(this.gameObject))
                    {
                        plantList.Remove(this.gameObject); // Remove the GameObject from the list

                        // Optionally check if the list is empty and perform additional actions
                        if (plantList.Count == 0)
                        {
                            // Perform any cleanup or special handling for an empty list
                            UI_Manager.Instance.GrownPlantsToCut.Remove(UI_Manager.Instance.FieldManager.CurrentFieldID);
                        }

                        Destroy(this.gameObject); // Destroy the GameObject

                    }
                }

            }
        }

    }

}


