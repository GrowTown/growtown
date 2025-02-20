using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantGrowth : MonoBehaviour
{

    [SerializeField] internal SkinnedMeshRenderer plantMesh;
    internal Timer _initialGrowTimer;
    private Timer _afterwateredGrowTimer;
    private Timer _afterHarvestWitherTimer;
    [SerializeField] Transform[] tomatoSpawnPoints;
    float cuttingHight = 0.2f;
    private WaveManager waveManager;
    internal bool IsTileWatered;
    public int initialgrowthTime = 1;
    public int AfterWateredgrowthTime = 1;
    public int AfterHarvestWitherTime = 1;
    float _currentGrowth;
    float _afterWaterGrowth;
    float _afterHarvest;
    double _currentTimer;
    internal Coroutine InitialCoroutine;
    internal Coroutine AfterWateredCoroutine;
    internal Coroutine AfterHarvestCoroutine;

    public float CurrentGrowth
    {
        get => _currentGrowth;
        set => _currentGrowth = value;
    }

    public float CurrentGrowthAfterWater
    {
        get => _afterWaterGrowth;
        set => _afterWaterGrowth = value;
    }

    public float CurrentGrowthAfterHarvest
    {
        get => _afterHarvest;
        set => _afterHarvest = value;
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
                if (UI_Manager.Instance.FieldManager.CurrentFieldID == 2)
                {
                    GameManager.Instance.SetCropTimerBar(UI_Manager.Instance.FieldManager.CurrentFieldID, this.gameObject);

                }
                else if (UI_Manager.Instance.FieldManager.CurrentFieldID == 1)
                {
                    GameManager.Instance.SetCropTimerBar(UI_Manager.Instance.FieldManager.CurrentFieldID, this.gameObject);
                }
                else
                {
                    GameManager.Instance.SetCropTimerBar(UI_Manager.Instance.FieldManager.CurrentFieldID, this.gameObject);
                }
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
            CurrentGrowthAfterWater = growthProgress;
            if (UI_Manager.Instance.FieldManager.CurrentFieldID == 2)
            {
                GameManager.Instance.iswateredField3 = true;
                GameManager.Instance.SetCropTimerBar(UI_Manager.Instance.FieldManager.CurrentFieldID, this.gameObject);

            }
            else if (UI_Manager.Instance.FieldManager.CurrentFieldID == 1)
            {
                GameManager.Instance.iswateredField2= true;
                GameManager.Instance.SetCropTimerBar(UI_Manager.Instance.FieldManager.CurrentFieldID, this.gameObject);
            }
            else
            {
                GameManager.Instance.iswateredField1 = true;
                GameManager.Instance.SetCropTimerBar(UI_Manager.Instance.FieldManager.CurrentFieldID, this.gameObject);
            }
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

           /* StopCoroutine(AfterWateredCoroutine);
            Destroy(_afterwateredGrowTimer);*/
           
        });
    }

    public IEnumerator AfterHarvestPlantWither()
    {

        _afterHarvestWitherTimer = this.gameObject.AddComponent<Timer>();
        var updatedTime = Mathf.Max(0, (int)(AfterHarvestWitherTime * 60)); // Convert to seconds
        _afterHarvestWitherTimer.Initialize("Plant Wither - After Harvest", DateTime.Now, TimeSpan.FromSeconds(updatedTime));
        _afterHarvestWitherTimer.StartTimer();
        float totalGrowthTime = (float)(_afterHarvestWitherTimer.timeToFinish.TotalSeconds);
        while (_afterHarvestWitherTimer.secondsLeft > 0)
        {

            float growthProgress = Mathf.Lerp(CurrentGrowthAfterWater, 2.0f, (float)(1.0f - (_afterHarvestWitherTimer.secondsLeft / totalGrowthTime)));
            CurrentGrowthAfterHarvest = growthProgress;
            Debug.Log("AfterWatered :: " + _afterHarvestWitherTimer.secondsLeft);
            if (UI_Manager.Instance.FieldManager.CurrentFieldID == 2)
            {
                GameManager.Instance.isHarvestField3 = true;
                GameManager.Instance.SetCropTimerBar(UI_Manager.Instance.FieldManager.CurrentFieldID, this.gameObject);

            }
            else if (UI_Manager.Instance.FieldManager.CurrentFieldID == 1)
            {
                GameManager.Instance.isHarvestField2 = true;
                GameManager.Instance.SetCropTimerBar(UI_Manager.Instance.FieldManager.CurrentFieldID, this.gameObject);
            }
            else
            {
                GameManager.Instance.isHarvestField1 = true;
                GameManager.Instance.SetCropTimerBar(UI_Manager.Instance.FieldManager.CurrentFieldID, this.gameObject);
            }

            yield return null;
        }

        _afterHarvestWitherTimer.TimerFinishedEvent.AddListener(delegate
        {
            if (this.gameObject != null)
            {
               if(!GameManager.Instance.witheredPlants.Contains(this.gameObject))
                {
                    plantMesh.material.color=Color.red;
                    GameManager.Instance.witheredPlants.Add(this.gameObject);
                }
            }
            _afterHarvestWitherTimer.StopTimer(); 
            StopCoroutine(AfterHarvestCoroutine);
            Destroy(_afterHarvestWitherTimer);
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
        if (_afterwateredGrowTimer != null)
        {
            _afterwateredGrowTimer.StopTimer();
            StopCoroutine(AfterWateredCoroutine);
            Destroy(_afterwateredGrowTimer);
        }
       AfterHarvestCoroutine= StartCoroutine("AfterHarvestPlantWither");
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


