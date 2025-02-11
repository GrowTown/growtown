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
    internal bool checkPlayerInZone;
    internal bool checkForEnoughSeeds;
    internal int HarvestCount;

    int _currentFieldID;
    int _currentWheatSeedCount = 0;
    int _currentTomatoSeedCount = 0;
    int _currentStrawberriesSeedCount = 0;
    int _currentEnergyCount = 0;
    int _currentWaterCount = 0;
    int _currentPasticideCount = 0;
    int _currentscoreIn = 0;
    PlayerAction _currentAction;
    bool _timerStartAfterPlants;
    Timer _timer;


    #region Properties

    public int CurrentScore
    {
        get => _currentscoreIn;
        set
        {
            _currentscoreIn = value;
            UI_Manager.Instance.score.text = _currentscoreIn.ToString();
        }
    }
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
        set 
         {
            _currentStrawberriesSeedCount = value;
            if (UI_Manager.Instance.InventoryManager.inventoryItems.ContainsKey("BeansSeed"))
            {
                var item = UI_Manager.Instance.InventoryManager.inventoryItems["BeansSeed"];
                item.Itemcount = _currentTomatoSeedCount;
            }
        }
    }
    public int CurrentWheatSeedCount
    {
        get => _currentWheatSeedCount;
        set
        {
            _currentWheatSeedCount = value;
            if (UI_Manager.Instance.InventoryManager.inventoryItems.ContainsKey("WheatSeed"))
            {
                var item = UI_Manager.Instance.InventoryManager.inventoryItems["WheatSeed"];
                item.Itemcount = _currentTomatoSeedCount;
            }
        }
    }
    public int CurrentTomatoSeedCount
    {
        get => _currentTomatoSeedCount;
        set
        {
            _currentTomatoSeedCount = value;
            if (UI_Manager.Instance.InventoryManager.inventoryItems.ContainsKey("TomatoSeed"))
            {
                   var item=UI_Manager.Instance.InventoryManager.inventoryItems["TomatoSeed"];
                   item.Itemcount = _currentTomatoSeedCount;
            }
              
        }
    }
    public int CurrentEnergyCount
    {
        get => _currentEnergyCount;
        set
        {
            _currentEnergyCount = Mathf.Clamp(value, 0, 500);
            UI_Manager.Instance.energyText.text = value.ToString();
            UI_Manager.Instance.energySlider.value = _currentEnergyCount / (float)500;
        }
    }
    public int CurrentWaterCount
    {
        get => _currentWaterCount;
        set
        {
             _currentWaterCount = Mathf.Clamp(value, 0, 500);
            UI_Manager.Instance.waterText.text = value.ToString();
            UI_Manager.Instance.waterSlider.value= _currentWaterCount/(float)500;
        }
    }
    public int CurrentPasticideCount
    {
        get => _currentPasticideCount;
        set
        {
            _currentPasticideCount = value;
            UI_Manager.Instance.pasticideCount.text = _currentPasticideCount.ToString();
        }
    }
    public PlayerAction CurrentAction
    {
        get => _currentAction;
        set => _currentAction = value;
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
                UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(3, 1);
                // Debug.Log("Cleaning");
                break;
            case PlayerAction.Seed:
             /*   if (!HasEnoughLandHealth(5)) return;*/
                if (!HasEnoughPoints(5, 0)) return;
                if (!HasNotEnoughSeed(1)) return;
                ToDecreaseTHElandHealth(UI_Manager.Instance.FieldManager.CurrentFieldID, 5);
                isThroughingseeds = true;
                UI_Manager.Instance.seedsBag.gameObject.SetActive(true);
                UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(4, 1);
                // Debug.Log("Seeding");
                break;
            case PlayerAction.Water:

                TimerStartAfterPlants = true;
                isThroughingseeds = false;
                UI_Manager.Instance.FieldGrid.checkedOnce = false;
                if (!HasEnoughPoints(2, 10)) return;
                checkForEnoughSeeds = true;
                UI_Manager.Instance.wateringTool.SetActive(true);
                UI_Manager.Instance.seedsBag.GetComponent<SeedSpawnerandSeedsBagTrigger>().isTileHasSeed = false;
                UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(5, 1);

                break;
            case PlayerAction.Harvest:

                UI_Manager.Instance.FieldGrid.checkedOnce = false;
                if (!HasEnoughPoints(3, 0)) return;
                isPlantStartGrowing = false;
                isCutting = true;
                UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(6, 1);
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

    /*public void CompleteAction()
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
    }*/

    PlantGrowth Pg;

    internal bool cropseedingStarted;
    internal bool HasNotEnoughSeeds;
    internal void ForCropSeedDEduction()
    {
        if (CurrentTomatoSeedCount >= 1)
        {
            CurrentTomatoSeedCount -= 1;
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
            CurrentScore += 250;
        }
        else if (grownPlantCount > 75)
        {
            CurrentScore += 200;
        }
        else if (grownPlantCount > 50)
        {
            CurrentScore += 150;
        }
        else if (grownPlantCount > 25)
        {
            CurrentScore += 100;
        }
        else if (grownPlantCount >= 15) 
        {
            CurrentScore += 50;
        }
        else
        {
            CurrentScore += 0;
        }

    }
    bool IsHarvestCount;
     public void HarvestDeductEnergy(GameObject tilego)
    {
        if (!HasEnoughPoints(3, 0)) return;
        var tileInfo = tilego.GetComponent<TileInfo>();
        if (!tileInfo.isCuttingStarted)
        {
            DeductEnergyPoints(3);
            UI_Manager.Instance.PlayerXp.SuperXp(4);
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
    List<string>startPackName= new List<string>() { "CleaningTool", "WateringTool", "CuttingTool", "TomatoSeed" };
    public void StartPackToBuy()
    {
        UI_Manager.Instance.ShopManager.ToBuyTomato();
        UI_Manager.Instance.ShopManager.ToBuyCleaningTool();
        UI_Manager.Instance.ShopManager.ToBuyWateringTool();
        UI_Manager.Instance.ShopManager.ToBuyCuttingTool();
        UI_Manager.Instance.starterPackInfoPopUpPanel.SetActive(false);
        foreach (var item in startPackName)
        {
            var shopItemHolder = UI_Manager.Instance.ShopManager.FindShopItemHolder(item);
            if (shopItemHolder != null)
            {
                UI_Manager.Instance.InventoryManager.AddToInventory(shopItemHolder);
            }
        }
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

    }

    public void ToBuyEnergyPoints()
    {
        int energyToAdd = 250;
        int energyMax = 500;

        if (CurrentScore >= 15)
        {
            if (CurrentEnergyCount < energyMax)
            {
                int availableSpace = energyMax - CurrentEnergyCount;
                CurrentEnergyCount += Mathf.Min(energyToAdd, availableSpace);
                CurrentScore -= 15;
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
        if (CurrentScore >= 20)
        {
            if (CurrentWaterCount < 500)
            {
                CurrentScore -= 20;
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
            tile._hasColorChanged = false;
        }
        IsHarvestCount = false;
        UI_Manager.Instance.isWentInsideOnce = false;
        //UI_Manager.Instance.FieldGrid.coveredtiles.Clear();
        UI_Manager.Instance.spawnPlantsForInitialGrowth.Clear();
        //UI_Manager.Instance.FieldManager.fieldSteps.Clear();
        UI_Manager.Instance.GrowthStartedOnThisTile.Clear();
        UI_Manager.Instance.spawnPlantsForGrowth.Clear();
        UI_Manager.Instance.oldcurrentStep = -1;
        UI_Manager.Instance.FieldManager.CurrentStepID=-1;
        UI_Manager.Instance.FieldManager.fieldSteps[CurrentFieldID].Clear();
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
            UI_Manager.Instance.warningPopupPanelSeed.SetActive(true);
            UI_Manager.Instance.warningTextForSeed.text = "Not Enough Seeds";
            PanelManager.RegisterPanel(UI_Manager.Instance.warningPopupPanelSeed);
            HideFieldPopup();
            StopCurrentAction();
            checkForEnoughSeeds = false;
            // HasNotEnoughSeeds = true;
            return false;
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
  /*  public bool HasEnoughLandHealth(int lhRequired)
    {
       if (UI_Manager.Instance.LandHealth.CurrentLandHealth < lhRequired)
        {
            UI_Manager.Instance.warningPasticidePopUpPanel.SetActive(true);
            PanelManager.RegisterPanel(UI_Manager.Instance.warningPasticidePopUpPanel);
            HideFieldPopup();
            StopCurrentAction();
            return false;
        }
        return true;
    }*/
    public  void ToIncreaseLandHealthUsePasticide(int fieldID, int deduct)
    {

        if (fieldID == 0)
        {
            UI_Manager.Instance.lhHolderTransform.GetChild(3).gameObject.GetComponent<LandHealth>().LandHealthIncrease(deduct);
        }
        else if (fieldID == 1)
        {
            UI_Manager.Instance.lhHolderTransform.GetChild(1).gameObject.GetComponent<LandHealth>().LandHealthIncrease(deduct);
        }
        else
        {
            UI_Manager.Instance.lhHolderTransform.GetChild(0).gameObject.GetComponent<LandHealth>().LandHealthIncrease(deduct);
        }

        CurrentPasticideCount -= 1;
        Debug.Log("LAND is healing");
    }
   
    internal bool isShowingnewLand=false;
    internal IEnumerator ShowBoughtLand(string landname)
    {
       var Cam = UI_Manager.Instance.CharacterMovements.gameObject.GetComponent<CamerasSwitch>();
            UI_Manager.Instance.marketPopUp.SetActive(false);
        if (landname == "wheat")
        {
            Cam.SwitchToCam(3);
            Cam.activeCamera.LookAt = UI_Manager.Instance.wheatFieldArea.transform;
        }
        else
        {
            Cam.SwitchToCam(4);
            Cam.activeCamera.LookAt = UI_Manager.Instance.carrotFieldArea.transform;
        }
        
      
        yield return new WaitForSeconds(10f);
        //Cam.virtualCams[3].LookAt = UI_Manager.Instance.CharacterMovements.gameObject.transform;
        isShowingnewLand = false;
        UI_Manager.Instance.marketPopUp.SetActive(true);
        Cam.SwitchToCam(2);
        Cam.activeCamera.LookAt = UI_Manager.Instance.CharacterMovements.gameObject.transform;

    }

    internal void ToDecreaseTHElandHealth(int fieldID,int deduct)
    {
        if (fieldID == 0)
        {
            UI_Manager.Instance.lhHolderTransform.GetChild(3).gameObject.GetComponent<LandHealth>().LandHealthDecrease(deduct);
        }
        else if (fieldID == 1)
        {
            UI_Manager.Instance.lhHolderTransform.GetChild(1).gameObject.GetComponent<LandHealth>().LandHealthDecrease(deduct);
        }
        else
        {
            UI_Manager.Instance.lhHolderTransform.GetChild(0).gameObject.GetComponent<LandHealth>().LandHealthDecrease(deduct);
        }
    }

    public void PesticideboughtCount(int fieldID, bool check)
    {
        if (fieldID == 0)
        {
            UI_Manager.Instance.lhHolderTransform.GetChild(3).gameObject.GetComponent<LandHealth>().isPasticidsBought =check;
            UI_Manager.Instance.lhHolderTransform.GetChild(0).gameObject.GetComponent<LandHealth>().CurrentPasticideCount += 1;
        }
        else if (fieldID == 1)
        {
            UI_Manager.Instance.lhHolderTransform.GetChild(1).gameObject.GetComponent<LandHealth>().isPasticidsBought = check;
            UI_Manager.Instance.lhHolderTransform.GetChild(0).gameObject.GetComponent<LandHealth>().CurrentPasticideCount += 1;
        }
        else
        {
            UI_Manager.Instance.lhHolderTransform.GetChild(0).gameObject.GetComponent<LandHealth>().isPasticidsBought = check;
            UI_Manager.Instance.lhHolderTransform.GetChild(0).gameObject.GetComponent<LandHealth>().CurrentPasticideCount +=1;
        }
    }
    #endregion
}
