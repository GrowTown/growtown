using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    internal bool isThroughingseeds;
    internal bool isCutting = false;
    internal bool isHarvestCompleted;
    internal bool isOneWorkingActionCompleted = false;
    internal bool isPlantStartGrowing;
    internal bool checkingForSeedingStarted;
    internal bool isplantGrowthCompleted;
    internal int spawnedTomatoesCount;

    int _currentFieldID;
    int _currentWheatSeedCount = 0;
    int _currentTomatoSeedCount = 0;
    int _currentStrawberriesSeedCount = 0;
    int _currentEnergyCount = 0;
    int _currentWaterCount = 0;
    bool _timerStartAfterPlants;
    public bool checkPlayerInZone;
    public bool checkForEnoughSeeds;
    

    Timer _timer;


    #region Properties
    public bool TimerStartAfterPlants
    {
        get => _timerStartAfterPlants;
        set
        {
            if (value == true)
            {
                StartCoroutine(ToIncreaseWaterPointsAccordingtoTime());
                _timerStartAfterPlants = value;
            }
        }
    }
    public int CurrentFieldID
    {
        get => _currentFieldID;
        set => _currentFieldID = value;
    }

    public int CurrentStrawberriesSeedCount
    {
        get => _currentStrawberriesSeedCount;
        set => _currentStrawberriesSeedCount = value;
    }
    public int CurrentWheatSeedCount
    {
        get => _currentWheatSeedCount;
        set => _currentWheatSeedCount = value;
    }
    public int CurrentTomatoSeedCount
    {
        get => _currentTomatoSeedCount;
        set => _currentTomatoSeedCount = value;
    }
    public int CurrentEnergyCount
    {
        get => _currentEnergyCount;
        set
        {
            UI_Manager.Instance.energyText.text = value.ToString();
            _currentEnergyCount = value;
        }
    }
    public int CurrentWaterCount
    {
        get => _currentWaterCount;
        set
        {
            UI_Manager.Instance.waterText.text = value.ToString();
            _currentWaterCount = value;
        }
    }

    #endregion

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
    }

    bool iscleanigStarted;

    #region Methods

    public void StartActionAnimation(PlayerAction action)
    {
        switch (action)
        {
            case PlayerAction.Clean:
                //if(!iscleanigStarted) UI_Manager.Instance.cleanigEffect.SetActive(true);
                UI_Manager.Instance.cleaningTool.SetActive(true);
                UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(2, 1);
                // Debug.Log("Cleaning");
                break;
            case PlayerAction.Seed:

                if (!HasEnoughPoints(5, 0)) return;
                isThroughingseeds = true;
                UI_Manager.Instance.seedsBag.gameObject.SetActive(true);
                UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(3, 1);
                // Debug.Log("Seeding");
                break;
            case PlayerAction.Water:

                TimerStartAfterPlants = true;
                isThroughingseeds = false;
                UI_Manager.Instance.FieldGrid.checkedOnce = false;
                if (!HasEnoughPoints(2, 10)) return;
                checkForEnoughSeeds=true;
                UI_Manager.Instance.wateringTool.SetActive(true);
                UI_Manager.Instance.seedsBag.GetComponent<SeedSpawnerandSeedsBagTrigger>().isTileHasSeed = false;
                UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(4, 1);
            
                break;
            case PlayerAction.Harvest:

                UI_Manager.Instance.FieldGrid.checkedOnce = false;
                if (!HasEnoughPoints(3, 0)) return;
                isPlantStartGrowing = false;
                isCutting = true;
                UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(5, 1);
                UI_Manager.Instance.sickleWeapon.SetActive(true);
                break;
        }
    }

    public void StopCurrentAnimations()
    {
        UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(2, 0);
        UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(3, 0);
        UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(4, 0);
        UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(5, 0);
        UI_Manager.Instance.cleaningTool.SetActive(false);
    }
    public void ShowFieldPopup(PlayerAction currentAction)
    {
        UI_Manager.Instance.ShowPopup(currentAction);
    }

    public void HideFieldPopup()
    {
        UI_Manager.Instance.HideFieldPopup();
    }

    internal void StartPlayerAction(PlayerAction action)
    {
        UI_Manager.Instance.TriggerZoneCallBacks.playerInZone = true;
        UI_Manager.Instance.FieldGrid.StartCoverageTracking(action);
    }

    public void StopCurrentAction()
    {
        UI_Manager.Instance.FieldGrid.StopCoverageTracking();
    }

    public void CompleteAction()
    {
        HideFieldPopup();
        if (UI_Manager.Instance.TriggerZoneCallBacks.currentStep < UI_Manager.Instance.TriggerZoneCallBacks.actionSequence.Length - 1)
        {
            isOneWorkingActionCompleted = true;
            if (UI_Manager.Instance.TriggerZoneCallBacks.currentStep == 1)
            {
                isPlantStartGrowing = true;  
            }
            UI_Manager.Instance.TriggerZoneCallBacks.currentStep++;
            UI_Manager.Instance.oldcurrentStep = UI_Manager.Instance.TriggerZoneCallBacks.currentStep;
            if (UI_Manager.Instance.TriggerZoneCallBacks.currentStep == 3)
            {
                if (UI_Manager.Instance.FieldManager.fieldSteps.ContainsKey(UI_Manager.Instance.FieldManager.CurrentFieldID))
                {
                    UI_Manager.Instance.FieldManager.fieldSteps[UI_Manager.Instance.FieldManager.CurrentFieldID] = UI_Manager.Instance.oldcurrentStep;
                }
               
                if (checkPlayerInZone)
               ShowFieldPopup(UI_Manager.Instance.TriggerZoneCallBacks.actionSequence[UI_Manager.Instance.TriggerZoneCallBacks.currentStep]);
            }
            else
            {
                ShowFieldPopup(UI_Manager.Instance.TriggerZoneCallBacks.actionSequence[UI_Manager.Instance.TriggerZoneCallBacks.currentStep]);
            }
        }
    }

    PlantGrowth Pg;

    internal bool cropseedingStarted;
    internal bool HasNotEnoughSeeds;
    internal void ForCropSeedDEduction()
    {
        if (CurrentTomatoSeedCount >= 1)
        {
            GameManager.Instance.CurrentTomatoSeedCount -= 1;
            UI_Manager.Instance.inventoryPanel.transform.GetChild(0).gameObject.GetComponent<SelectionFunctionality>().productCount.text = GameManager.Instance.CurrentTomatoSeedCount.ToString();
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
        if (!HasEnoughPoints(2, 10)) return;
        if (!UI_Manager.Instance.GrowthStartedOnThisTile.Contains(tilego))
        {

            if (UI_Manager.Instance.spawnPlantsForGrowth.ContainsKey(tilego))
            {
                ToDecreaseTheWaterPoints();
                DeductEnergyPoints(2);
                foreach (var item in UI_Manager.Instance.spawnPlantsForGrowth[tilego])
                {
                    var pg = item.GetComponent<PlantGrowth>();
                    if (!pg.isNotWateredDuringWithering)
                    {
                        if (pg._initialGrowTimer != null)
                        {
                            pg._initialGrowTimer.StopTimer();
                            StopCoroutine(pg.InitialCoroutine);
                            Destroy(pg._initialGrowTimer); // Cleanup the old timer
                        }

                        pg.AfterWateredCoroutine = StartCoroutine(pg.AfterWateredTileGrowth(pg.CurrentTimer));
                        if (!UI_Manager.Instance.GrowthStartedPlants.Contains(item))
                        {
                            UI_Manager.Instance.GrowthStartedPlants.Add(item);
                        }
                    }
                }
            }
            UI_Manager.Instance.GrowthStartedOnThisTile.Add(tilego);
        }
        foreach (var item in UI_Manager.Instance.spawnedSeed)
        {
            Destroy(item);
        }

    }
    public void Withering()
    {
        foreach (var item in UI_Manager.Instance.spawnPlantsForInitialGrowth)
        {
            if (!UI_Manager.Instance.GrowthStartedPlants.Contains(item))
            {
                item.GetComponent<PlantGrowth>().plantMesh.material.color = Color.red;
            }
        }
    }
    public void CounttheHarvest(int growPC)
    {
        int grownPlantCount = growPC;

        if (grownPlantCount == 100)
        {
            UI_Manager.Instance.scoreIn += 250; 
        }
        else if (grownPlantCount > 75)
        {
            UI_Manager.Instance.scoreIn += 200; 
        }
        else if (grownPlantCount > 50)
        {
            UI_Manager.Instance.scoreIn += 150; 
        }
        else if (grownPlantCount > 25)
        {
            UI_Manager.Instance.scoreIn += 100; 
        }
        else if (grownPlantCount >= 15)
        {
            UI_Manager.Instance.scoreIn += 50; 
        }
        else
        {
            UI_Manager.Instance.scoreIn += 0; 
        }

        UI_Manager.Instance.score.text = UI_Manager.Instance.scoreIn.ToString();
    }
    int HarvestCount;
    bool IsHarvestCount;
    public void HarvestDeductEnergy(GameObject tilego)
    {
        if (!HasEnoughPoints(3, 0)) return;
        var tileInfo = tilego.GetComponent<TileInfo>();
        if (!tileInfo.isCuttingStarted)
        {
            DeductEnergyPoints(3);
            tileInfo.isCuttingStarted = true;
        }
        if (UI_Manager.Instance.FieldGrid.IsCoverageComplete() && !IsHarvestCount)
        {
            if (!UI_Manager.Instance.ListOfHarvestCount.ContainsKey(HarvestCount))
            {
                UI_Manager.Instance.ListOfHarvestCount[HarvestCount] = new List<int>();
            }

            for (int i = 0; i < UI_Manager.Instance.GrowthStartedPlants.Count; i++)
            {
                UI_Manager.Instance.ListOfHarvestCount[HarvestCount].Add(i);
            }

            HarvestCount++;
            IsHarvestCount = true;
        }

    }
    public void StartPackToBuy()
    {
        UI_Manager.Instance.ShopManager.ToBuyTomato();
        UI_Manager.Instance.ShopManager.ToBuyCleaningTool();
        UI_Manager.Instance.ShopManager.ToBuyWateringTool();
        UI_Manager.Instance.ShopManager.ToBuyCuttingTool();
        UI_Manager.Instance.starterPackInfoPopUpPanel.SetActive(false);
        UI_Manager.Instance.sickleWeaponBT.interactable = false;
        UI_Manager.Instance.wateringWeaponBT.interactable = false;
        UI_Manager.Instance.cleaningWeaponBT.interactable = false;
    }

    bool isWaterPointDecreased;

    internal void DeductEnergyPoints(int amount)
    {
        CurrentEnergyCount -= amount;
        CurrentEnergyCount = Mathf.Clamp(CurrentEnergyCount, 0, int.MaxValue);

        // isEnergyDeducted = true;
    }
    public void ToDecreaseTheWaterPoints()
    {
        CurrentWaterCount -= 10;
        /*  if (!isWaterPointDecreased)
          {
              CurrentWaterCount -= 10;

              isWaterPointDecreased = true;
          }*/
    }

    public void ToBuyEnergyPoints()
    {
        int energyToAdd = 250;
        int energyMax = 500;

        if (UI_Manager.Instance.scoreIn >= 15)
        {
            if (CurrentEnergyCount < energyMax)
            {
                int availableSpace = energyMax - CurrentEnergyCount;
                CurrentEnergyCount += Mathf.Min(energyToAdd, availableSpace);
                UI_Manager.Instance.scoreIn -= 15;
                UI_Manager.Instance.score.text = UI_Manager.Instance.scoreIn.ToString();
                UI_Manager.Instance.energyText.text = CurrentEnergyCount.ToString();
            }
        }
        else
        {
            Debug.Log("Not enough score to buy energy points.");
        }
    }

    public void ToBuyWaterPoints()
    {
        if (UI_Manager.Instance.scoreIn >= 20)
        {
            if (CurrentWaterCount < 500)
            {

                CurrentWaterCount = 500;
                UI_Manager.Instance.waterText.text = CurrentWaterCount.ToString();
            }

        }
        else
        {
            Debug.Log("Not enough score to buy water points.");
        }
    }

    bool isTimerStarted;
    public IEnumerator ToIncreaseWaterPointsAccordingtoTime()
    {
        if (CurrentWaterCount < 500)
        {
            if (_timer == null)
            {
                _timer = this.gameObject.AddComponent<Timer>();
            }
            if (!isTimerStarted)
            {
                _timer.Initialize("Water", DateTime.Now, TimeSpan.FromMinutes(1));
                _timer.StartTimer();
                isTimerStarted = true;

            }

            // Loop while the timer is running
            while (_timer.secondsLeft > 0)
            {
                Debug.Log("Time Left :: " + _timer.secondsLeft);
                yield return null;
            }


            if (CurrentWaterCount < 500)
            {
                CurrentWaterCount += 10;
                CurrentWaterCount = Mathf.Clamp(CurrentWaterCount, 0, 500);
                //UI_Manager.Instance.waterText.text = CurrentWaterCount.ToString();
                isTimerStarted = false;
                StartCoroutine(ToIncreaseWaterPointsAccordingtoTime());
            }
            else
            {
                Destroy(_timer);
            }
        }
    }

    internal bool isResetetValues;
    public void ResetValues()
    {
        cropseedingStarted = false;
        foreach (var item in UI_Manager.Instance.FieldGrid.coveredtiles)
        {
            var tile = item.GetComponent<TileInfo>();
            tile.seedsSpawned = false;
            tile.plantSpawned = false;
            tile.plantgrowth = false;
            tile.isCuttingStarted = false;
        }
        IsHarvestCount = false;
        UI_Manager.Instance.isWentInsideOnce = false;
        //UI_Manager.Instance.FieldGrid.coveredtiles.Clear();
        UI_Manager.Instance.spawnPlantsForInitialGrowth.Clear();
        UI_Manager.Instance.FieldManager.fieldSteps.Clear();
        UI_Manager.Instance.GrowthStartedOnThisTile.Clear();
        UI_Manager.Instance.spawnPlantsForGrowth.Clear();
        UI_Manager.Instance.oldcurrentStep = -1;
        UI_Manager.Instance.TriggerZoneCallBacks.currentStep = 0;
        UI_Manager.Instance.FieldManager.fieldSteps.Remove(CurrentFieldID);
        isCutting = false;
        //isResetetValues = true;
        foreach (var item in UI_Manager.Instance.PopupImg)
        {
            item.GetComponent<POPSelectionFunctionality>()._hasBeenClicked = false;
        }
        isplantGrowthCompleted = false;

    }
    public bool HasNotEnoughSeed(int seedRequired)
    {
        if (CurrentTomatoSeedCount < seedRequired)
        {
            if (checkForEnoughSeeds)
            {
                UI_Manager.Instance.warningPopupPanelSeed.SetActive(true);
                UI_Manager.Instance.warningTextForSeed.text = "Not Enough Seeds";
                PanelManager.RegisterPanel(UI_Manager.Instance.warningPopupPanelSeed);
                HideFieldPopup();
                StopCurrentAction();
                checkForEnoughSeeds = false;
               // HasNotEnoughSeeds = true;
                return false;
            }
        }

        return true;
    }
    public bool HasEnoughPoints(int energyRequired, int waterRequired)
    {
        if (CurrentEnergyCount < energyRequired)
        {
            UI_Manager.Instance.ShowPopUpNotEnoughPoints("Not enough energy points to perform this action!");
            HideFieldPopup();
            StopCurrentAction();
            return false;
        }
        if (CurrentWaterCount < waterRequired)
        {
            UI_Manager.Instance.ShowPopUpNotEnoughPoints("Not enough water points to perform this action!");
            HideFieldPopup();
            StopCurrentAction();
            return false;
        }
        return true;
    }


    #endregion
}
