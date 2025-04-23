using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    internal bool iswateringStarted = false;
    internal bool isThroughingseeds;
    internal bool isCutting = false;
    internal bool isPlantStartGrowing = false;
    internal int fieldID;
    internal string fieldName;
    internal float fieldHealth;
    int _currentStepID;

    void Start()
    {

    }

    void Update()
    {

    }

    internal Field Initialize(string name,int ID,float health)
    {
        this.fieldName = name;
        this.fieldID = ID;
        this.fieldHealth = health;
        return this;
    }

 /*   public void StartActionAnimation(PlayerAction action)
    {
        switch (action)
        {
            case PlayerAction.Clean:    
                UI_Manager.Instance.cleaningTool.SetActive(true);
                UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(3, 1);
                break;
            case PlayerAction.Seed:
                if (!GameManager.Instance.HasEnoughPoints(5, 0)) return;
                if (!GameManager.Instance.HasNotEnoughSeed(1)) return;
                GameManager.Instance.ToDecreaseTHElandHealth(UI_Manager.Instance.FieldManager.CurrentFieldID, 5);
                isThroughingseeds = true;
                UI_Manager.Instance.seedsBag.gameObject.SetActive(true);
                UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(4, 1);
                break;
            case PlayerAction.Water:
                  GameManager.Instance.TimerStartAfterPlants = true;
                isThroughingseeds = false;
                UI_Manager.Instance.FieldGrid.checkedOnce = false;
                if (!GameManager.Instance.HasEnoughPoints(2, 10)) return;
                UI_Manager.Instance.wateringTool.SetActive(true);
                UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(5, 1);

                break;
            case PlayerAction.Harvest:

                UI_Manager.Instance.FieldGrid.checkedOnce = false;
                if (!GameManager.Instance.HasEnoughPoints(3, 0)) return;
                isPlantStartGrowing = false;
                isCutting = true;
                UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(6, 1);
                UI_Manager.Instance.sickleWeapon.SetActive(true);
                break;
        }
    }

    public void BeforeWaterTile()
    {
        foreach (var item in UI_Manager.Instance.spawnPlantsForInitialGrowth)
        {
            var instance = item.GetComponent<PlantGrowth>();
            instance.InitialCoroutine = StartCoroutine((instance.InitialGrowPlant()));
        }
    }

    public void OnWaterTile(GameObject tilego)
    {
        if (!GameManager.Instance.HasEnoughPoints(2, 10)) return;
        if (!UI_Manager.Instance.GrowthStartedOnThisTile.Contains(tilego))
        {

            if (UI_Manager.Instance.spawnPlantsForGrowth.ContainsKey(tilego))
            {
                GameManager.Instance.ToDecreaseTheWaterPoints();
                GameManager.Instance.DeductEnergyPoints(2);
                UI_Manager.Instance.PlayerXp.SuperXp(1);
                foreach (var item in UI_Manager.Instance.spawnPlantsForGrowth[tilego])
                {
                    var pg = item.GetComponent<PlantGrowth>();

                    if (!pg.isNotWateredDuringWithering)
                    {
                        if (pg._initialGrowTimer != null)
                        {
                            pg._initialGrowTimer.StopTimer();
                            StopCoroutine(pg.InitialCoroutine);
                            Destroy(pg._initialGrowTimer);
                        }

                        pg.AfterWateredCoroutine = StartCoroutine(pg.AfterWateredTileGrowth(pg.CurrentTimer));
                        iswateringStarted = true;
                        if (UI_Manager.Instance.FieldManager.CurrentFieldID == 0)
                        {
                            if (!UI_Manager.Instance.GrowthStartedPlants1.ContainsKey("BeansHarvest"))
                                UI_Manager.Instance.GrowthStartedPlants1["BeansHarvest"] = new List<GameObject>();
                            if (!UI_Manager.Instance.GrowthStartedPlants1["BeansHarvest"].Contains(item))
                            {
                                UI_Manager.Instance.GrowthStartedPlants1["BeansHarvest"].Add(item);
                            }
                        }
                        else if (UI_Manager.Instance.FieldManager.CurrentFieldID == 1)
                        {
                            if (!UI_Manager.Instance.GrowthStartedPlants1.ContainsKey("WheatHarvest"))
                                UI_Manager.Instance.GrowthStartedPlants1["WheatHarvest"] = new List<GameObject>();
                            if (!UI_Manager.Instance.GrowthStartedPlants1["WheatHarvest"].Contains(item))
                            {
                                UI_Manager.Instance.GrowthStartedPlants1["WheatHarvest"].Add(item);
                            }
                        }
                        else
                        {
                            if (!UI_Manager.Instance.GrowthStartedPlants1.ContainsKey("TomatoHarvest"))
                                UI_Manager.Instance.GrowthStartedPlants1["TomatoHarvest"] = new List<GameObject>();
                            if (!UI_Manager.Instance.GrowthStartedPlants1["TomatoHarvest"].Contains(item))
                            {
                                UI_Manager.Instance.GrowthStartedPlants1["TomatoHarvest"].Add(item);
                            }
                        }
                        if (!UI_Manager.Instance.GrowthStartedPlants.Contains(item))
                        {
                            UI_Manager.Instance.GrowthStartedPlants.Add(item);
                        }
                    }
                }
                // WaveManager.instance.StartEnemyWave();
            }
            UI_Manager.Instance.GrowthStartedOnThisTile.Add(tilego);
        }
        foreach (var item in UI_Manager.Instance.spawnedSeed)
        {
            Destroy(item);
        }
    }*/
}
