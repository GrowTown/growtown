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
    public int initialgrowthTime ;
    public int AfterWateredgrowthTime ;
    public int AfterHarvestWitherTime ;
    float _currentGrowth;
    float _afterWaterGrowth;
    float _afterHarvest;
    double _currentTimer;
    internal Coroutine InitialCoroutine;
    internal Coroutine AfterWateredCoroutine;
    internal Coroutine AfterHarvestCoroutine;
    internal PlantGrowthState currentGrowthState;



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
    internal IEnumerator InitialGrowPlant(FieldGrid fGrid)
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
                float remainingSeconds = (float)_initialGrowTimer.secondsLeft;
                float remainingMinutes = remainingSeconds / 60f;

                fGrid.FieldCropRemainingCount = remainingMinutes;

                if (fGrid.fieldID == 2)
                {
                    GameManager.Instance.SetCropTimerBar(fGrid.fieldID, this.gameObject);
                }

                else if (fGrid.fieldID == 1)
                {
                    GameManager.Instance.SetCropTimerBar(fGrid.fieldID, this.gameObject);
                }
                else
                {
                    GameManager.Instance.SetCropTimerBar(fGrid.fieldID, this.gameObject);
                }

                if (growthProgress >= 0.5f && !isWateredDuringWithering)
                {
                    Withering(fGrid);
                    isNotWateredDuringWithering = true;
                    _initialGrowTimer.StopTimer();
                    if (!fGrid.isInitialPlantGrowthCompleted)
                    {
                        fGrid.coveredtiles.Clear();
                        fGrid.CompleteAction();
                        fGrid.isInitialPlantGrowthCompleted = true;
                    }
                    Destroy(_initialGrowTimer);
                    yield break;
                }
            }
            yield return null;
        }

    }
    public IEnumerator AfterWateredTileGrowth(double currentTimer, FieldGrid fGrid)
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
            float remainingMinutes = (float)_afterwateredGrowTimer.secondsLeft / 60f;
            fGrid.FieldCropRemainingCount = remainingMinutes;
            

            if (fGrid.fieldID == 2)
            {

                GameManager.Instance.iswateredField3 = true;
                GameManager.Instance.SetCropTimerBar(fGrid.fieldID, this.gameObject);
            }
            else if (fGrid.fieldID == 1)
            {
                GameManager.Instance.iswateredField2 = true;
                GameManager.Instance.SetCropTimerBar(fGrid.fieldID, this.gameObject);
            }
            else
            {
                GameManager.Instance.iswateredField1 = true;
                GameManager.Instance.SetCropTimerBar(fGrid.fieldID, this.gameObject);
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
        var tileInfo = this.gameObject.transform.parent.parent.GetComponent<TileInfo>();
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
            if (tileInfo.fieldGrid.fieldID == 2)
            {
                GameManager.Instance.isHarvestField3 = true;
                GameManager.Instance.SetCropTimerBar(tileInfo.fieldGrid.fieldID, this.gameObject);

            }
            else if (tileInfo.fieldGrid.fieldID == 1)
            {
                GameManager.Instance.isHarvestField2 = true;
                GameManager.Instance.SetCropTimerBar(tileInfo.fieldGrid.fieldID, this.gameObject);
            }
            else
            {
                GameManager.Instance.isHarvestField1 = true;
                GameManager.Instance.SetCropTimerBar(tileInfo.fieldGrid.fieldID, this.gameObject);
            }

            yield return null;
        }

        _afterHarvestWitherTimer.TimerFinishedEvent.AddListener(delegate
        {
            if (this.gameObject != null)
            {
                if (!transform.parent.parent.GetComponentInParent<TileInfo>().fieldGrid.witheredPlants.Contains(this.gameObject))
                {
                    plantMesh.material.color = Color.red;
                    transform.parent.parent.GetComponentInParent<TileInfo>().fieldGrid.witheredPlants.Add(this.gameObject);
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
        AfterHarvestCoroutine = StartCoroutine("AfterHarvestPlantWither");
    }

    private void SpawnTomatoes()
    {
        var tileInfo = transform.parent.parent.GetComponentInParent<TileInfo>();
        if (tileInfo.fieldGrid.fieldID == 2)
        {

            foreach (Transform spawnPoint in tomatoSpawnPoints)
            {
                var insta = Instantiate(UI_Manager.Instance.tomato, spawnPoint.position, Quaternion.identity);
                tileInfo.fieldGrid.spawnTomatosForGrowth.Add(insta);
                insta.transform.SetParent(spawnPoint.transform);

            }
            Debug.Log("Tomatoes spawned at designated points.");
            var plantCount = tileInfo.fieldGrid.GrowthStartedPlants.Count;
            tileInfo.fieldGrid.spawnedTomatoesCount = plantCount * 5;
            if (tileInfo.fieldGrid.spawnTomatosForGrowth.Count == tileInfo.fieldGrid.spawnedTomatoesCount)
            {
                tileInfo.fieldGrid.isPlantGrowthCompleted = true;
                tileInfo.fieldGrid.FieldCropRemainingCount = 0;

            }
        }
        else
        {

            tileInfo.fieldGrid.isPlantGrowthCompleted = true;
            tileInfo.fieldGrid.FieldCropRemainingCount = 0;

        }
        
    }

    public void Withering(FieldGrid fGrid)
    {
        foreach (var item in fGrid.spawnPlantsForInitialGrowth)
        {
            if (!fGrid.GrowthStartedPlants.Contains(item))
            {
                item.GetComponent<PlantGrowth>().plantMesh.material.color = Color.red;
                fGrid.witheredPlants.Add(item);
            }
        }

    }



    bool loopOnce = false;
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            var tileInfo = transform.parent.parent.GetComponentInParent<TileInfo>();
            if (!GameManager.Instance.HasEnoughPoints(3, 0, tileInfo.fieldGrid)) return;
            if (!UI_Manager.Instance.sickleWeapon.activeSelf && tileInfo.fieldGrid.isCutting)
            {
                UI_Manager.Instance.sickleWeapon.SetActive(true);
            }

            if (tileInfo.fieldGrid.isCutting && UI_Manager.Instance.sickleWeapon.activeSelf)
            {
                this.gameObject.transform.DOMoveY(cuttingHight, 0.1f);
                GameManager.Instance.isHarvestCompleted = true;
                var fieldID = tileInfo.fieldGrid.fieldID;
                /*
                                if (UI_Manager.Instance.GrownPlantsToCut.Contains(this.gameObject))
                                {
                                    UI_Manager.Instance.GrownPlantsToCut.Remove(this.gameObject);
                                    Destroy(this.gameObject); 
                                }*/

                if (UI_Manager.Instance.GrownPlantsToCut.ContainsKey(fieldID))
                {
                    // Access the list for the current field
                    List<GameObject> plantList = UI_Manager.Instance.GrownPlantsToCut[fieldID];


                    // Loop through the plants associated with tiles
                    foreach (var entry in tileInfo.fieldGrid.spawnPlantsForGrowth)
                    {
                        GameObject tile = entry.Key;
                        List<GameObject> plants = entry.Value;

                        if (plants.Contains(this.gameObject))
                        {
                            plants.Remove(this.gameObject);

                            if (plants.Count == 0)
                            {
                                tile.GetComponent<TileInfo>().fieldGrid.AddCoveredTile(tile);
                            }

                            break;
                        }
                    }

                    if (plantList.Contains(this.gameObject))
                    {
                        plantList.Remove(this.gameObject);

                        if (plantList.Count == 0)
                        {

                            UI_Manager.Instance.GrownPlantsToCut.Remove(fieldID);
                            GameManager.Instance.ReSetCropTimerBar(fieldID);

                        }

                        if (!tileInfo.GetComponent<TileInfo>().fieldGrid.witheredPlants.Contains(this.gameObject))
                            UI_Manager.Instance.UIAnimationM.PlayMoveToUIAnimation(tileInfo.GetComponent<TileInfo>().fieldGrid.fieldPlantUIAnimation, UI_Manager.Instance.CharacterMovements.transform, UI_Manager.Instance.ShopManager.invetoryBagPos, 4);
                        Destroy(this.gameObject);

                    }
                }

            }
        }

    }

}

public enum PlantGrowthState
{
    Seedling,
    Growing,
    Mature,
    Withering
}


