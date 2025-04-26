using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FieldGrid : MonoBehaviour
{
    [Header("Grid")]
    public GameObject cellPrefab;
    public int rows = 5;
    public int columns = 7;
    public float cellSpacing = 0.01f;

    [Header("Field")]
    float _fieldCropRemainingCount;
    internal int fieldCropTime;

    internal bool iswateringStarted = false;
    internal bool isThroughingseeds;
    internal bool isCutting = false;
    internal bool isPlantStartGrowing = false;
    internal bool isInitialPlantGrowthCompleted = false;
    internal bool isPlantGrowthCompleted = false;
    internal bool isAllPlantsWithered = false;
    internal bool checkPlayerInZone = false;
    internal bool updateValue = false;
    internal bool isOneWorkingActionCompleted = false;
    [SerializeField] internal int fieldID;
    [SerializeField] internal string fieldName;
    [SerializeField] internal int fieldHealth;
    [SerializeField] internal GameObject fieldPlantPrefab;

    [SerializeField] internal Sprite fieldPlantUIAnimation;

    int _currentStepID;

    public bool playerInZone = false;

    //internal HashSet<Vector3> coveredTiles = new HashSet<Vector3>();
    internal List<GameObject> tiles = new List<GameObject>();
    internal List<GameObject> coveredtiles = new List<GameObject>();
    internal List<GameObject> spawnedSeed = new List<GameObject>();
    internal List<GameObject> spawnPlantsForInitialGrowth = new List<GameObject>();
    internal Dictionary<GameObject, List<GameObject>> spawnPlantsForGrowth = new Dictionary<GameObject, List<GameObject>>();
    internal List<GameObject> GrowthStartedOnThisTile = new List<GameObject>();
    internal List<GameObject> GrowthStartedPlants = new List<GameObject>();
    internal List<GameObject> spawnTomatosForGrowth = new List<GameObject>();
    internal List<GameObject> witheredPlants = new List<GameObject>();
    internal int spawnedTomatoesCount;


    internal List<GameObject> coveredtiles1 = new List<GameObject>();
    internal List<GameObject> coveredtiles2 = new List<GameObject>();
    internal bool isTracking = false;
    internal bool checkedOnce;
    private PlayerAction currentAction;

    [Header("PanelAnimation")]
    public Vector2 hiddenPosition = new Vector2(0, 800);
    public Vector2 visiblePosition = new Vector2(0, 0);
    public float moveDuration = 0.5f;

    //public static FieldGrid Instance { get; private set; }

    #region Properties

    public float FieldCropRemainingCount
    {
        get => _fieldCropRemainingCount;
        set
        {
            _fieldCropRemainingCount = value;
            ShowCropRemainingTimer();

        }
    }
    #endregion


    private void Start()
    {
        /*if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }*/

        GenerateGrid();
        GetThePlantClassRefences();
    }

    void GetThePlantClassRefences()
    {
        if (fieldID == 2)
        {
            fieldPlantPrefab.GetComponent<PlantGrowth>();
        }
        else
        {
            fieldPlantPrefab.GetComponent<PlantGrowthWithOutFruits>();
        }
    }
    private void GenerateGrid()
    {
        Vector3 gridOffset = new Vector3((columns - 1) * cellSpacing / 2, 0, (rows - 1) * cellSpacing / 2);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 cellPosition = new Vector3(col * cellSpacing, 0f, row * cellSpacing) - gridOffset;
                GameObject cell = Instantiate(cellPrefab, transform.position + cellPosition, Quaternion.Euler(-90, cellPrefab.transform.rotation.y, cellPrefab.transform.rotation.z));

                cell.transform.SetParent(this.transform);
                tiles.Add(cell);
            }
        }
    }

    /// <summary>
    /// start the action
    /// </summary>StartCovering
    /// <param name="action"></param>
    public void StartCoverageTracking(PlayerAction action)
    {
        if (IsCoverageComplete())
            coveredtiles.Clear();
        /*if (UI_Manager.Instance.FieldManager.CurrentFieldID == 2)
        {
        }
        else if (UI_Manager.Instance.FieldManager.CurrentFieldID == 1)
        {
            if (IsCoverageComplete())
                coveredtiles1.Clear();
        }
        else
        {
            if (IsCoverageComplete())
                coveredtiles2.Clear();
        }
*/
        currentAction = action;
        isTracking = true;
        StartActionAnimation(action);
    }

    /// <summary>
    /// Stoping action
    /// </summary>
    public void StopCoverageTracking()
    {
        isTracking = false;
        GameManager.Instance.StopCurrentAnimations(); // Stop the action animation
    }

    /// <summary>
    /// Adding the player covered tiles
    /// </summary>
    /// <param name="tilePosition"></param>
    /// <param name="tileGo"></param>
    public void AddCoveredTile(/*Vector2Int tilePosition,*/GameObject tileGo)
    {
        if (!checkedOnce)
        {
            checkedOnce = true;
            if (!GameManager.Instance.HasEnoughPoints(5, 10, this)) return;
        }
        if (UI_Manager.Instance.FieldManager.fieldSteps[fieldID] == 1)
        {
            if (!GameManager.Instance.HasNotEnoughSeed(1, this) && isThroughingseeds) return;
        }
        if (!coveredtiles.Contains(tileGo))
        {
            coveredtiles.Add(tileGo);
            TochangetheTileColor(tileGo);
        }

        /*if (UI_Manager.Instance.FieldManager.CurrentFieldID == 2)
        {
            if (!coveredtiles.Contains(tileGo))
            {
                coveredtiles.Add(tileGo);
                TochangetheTileColor(tileGo);
            }

        }
        else if (UI_Manager.Instance.FieldManager.CurrentFieldID == 1)
        {
            if (!coveredtiles1.Contains(tileGo))
            {
                coveredtiles1.Add(tileGo);
                TochangetheTileColor(tileGo);
            }
        }
        else
        {
            if (!coveredtiles2.Contains(tileGo))
            {
                coveredtiles2.Add(tileGo);
                TochangetheTileColor(tileGo);
            }
        }*/
    }

    void TochangetheTileColor(GameObject tileGo)
    {
        switch (currentAction)
        {
            case PlayerAction.Clean:
                tileGo.GetComponent<TileInfo>().ChangeColor(new Color(1f, 0.9188f, 0.9188f, 1f));
                Debug.Log("Cleanig");
                break;
            case PlayerAction.Seed:

                tileGo.GetComponent<TileInfo>().ChangeColor(new Color(0.7095f, 0.7095f, 0.7095f, 1f));
                // Debug.Log("Seeding");
                break;
            case PlayerAction.Water:
                tileGo.GetComponent<TileInfo>().ChangeColor(new Color(0.9098039f, 0.6431373f, 0.6431373f, 1f));
                break;
            case PlayerAction.Harvest:
                tileGo.GetComponent<TileInfo>().ChangeColor(new Color(0.5224f, 0.9266f, 0.2523f, 1f));
                break;
        }
    }

    /// <summary>
    /// Checking the player covered all tiles are not
    /// </summary>
    /// <returns></returns>
    public bool IsCoverageComplete()
    {
        /*if (UI_Manager.Instance.FieldManager.CurrentFieldID == 2)
        {
            return coveredtiles.Count >= rows * columns;
        }
        else if (UI_Manager.Instance.FieldManager.CurrentFieldID == 1)
        {
            return coveredtiles1.Count >= rows * columns;
        }
        else
        {
        }*/
        return coveredtiles.Count >= rows * columns;

    }

    /// <summary>
    /// Getting the tile according to player position on the grid
    /// </summary>
    /// <returns></returns>
    internal Vector2Int GetPlayerTile()
    {
        Vector3 playerPos = UI_Manager.Instance.CharacterMovements.transform.position;
        int row = Mathf.RoundToInt(playerPos.z / cellSpacing);
        int col = Mathf.RoundToInt(playerPos.x / cellSpacing);
        return new Vector2Int(col, row);  // Ensure row/col order matches your grid
    }

    public void StartActionAnimation(PlayerAction action)
    {
        switch (action)
        {
            case PlayerAction.Clean:
                UI_Manager.Instance.cleaningTool.SetActive(true);
                UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(3, 1);
                break;
            case PlayerAction.Seed:
                if (!GameManager.Instance.HasEnoughPoints(5, 0, this)) return;
                if (!GameManager.Instance.HasNotEnoughSeed(1, this)) return;
                GameManager.Instance.ToDecreaseTHElandHealth(UI_Manager.Instance.FieldManager.CurrentFieldID, 5);
                isThroughingseeds = true;
                UI_Manager.Instance.seedsBag.gameObject.SetActive(true);
                UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(4, 1);
                break;
            case PlayerAction.Water:
                GameManager.Instance.TimerStartAfterPlants = true;
                isThroughingseeds = false;
                checkedOnce = false;
                if (!GameManager.Instance.HasEnoughPoints(2, 10, this)) return;
                UI_Manager.Instance.wateringTool.SetActive(true);
                UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(5, 1);

                break;
            case PlayerAction.Harvest:

                checkedOnce = false;
                if (!GameManager.Instance.HasEnoughPoints(3, 0, this)) return;
                isPlantStartGrowing = false;
                isCutting = true;
                UI_Manager.Instance.CharacterMovements.animator.SetLayerWeight(6, 1);
                UI_Manager.Instance.sickleWeapon.SetActive(true);
                break;
        }
    }

    internal void ForCropSeedDEduction()
    {
        if (fieldName == "TomatoField")
        {
            if (GameManager.Instance.CurrentTomatoSeedCount >= 1)
            {
                GameManager.Instance.CurrentTomatoSeedCount -= 1;
            }

        }
        else if (fieldName == "WheatField")
        {
            if (GameManager.Instance.CurrentWheatSeedCount >= 1)
            {
                GameManager.Instance.CurrentWheatSeedCount -= 1;
            }

        }
        else if (fieldName == "BeansField")
        {
            if (GameManager.Instance.CurrentBeansSeedCount >= 1)
            {
                GameManager.Instance.CurrentBeansSeedCount -= 1;
            }

        }
    }
    public void BeforeWaterTile()
    {
        foreach (var item in spawnPlantsForInitialGrowth)
        {

            var instance = item.GetComponent<PlantGrowth>();
            instance.InitialCoroutine = StartCoroutine((instance.InitialGrowPlant(this)));

        }
    }

    PlantGrowth PG;
    PlantGrowthWithOutFruits PGWO;
    public void OnWaterTile(GameObject tilego)
    {
        if (!GameManager.Instance.HasEnoughPoints(2, 10, this)) return;
        if (!GrowthStartedOnThisTile.Contains(tilego))
        {

            if (spawnPlantsForGrowth.ContainsKey(tilego))
            {
                GameManager.Instance.ToDecreaseTheWaterPoints();
                GameManager.Instance.DeductEnergyPoints(2);
                UI_Manager.Instance.PlayerXp.SuperXp(1);
                foreach (var item in spawnPlantsForGrowth[tilego])
                {
                    var PG = item.GetComponent<PlantGrowth>();
                    if (!PG.isNotWateredDuringWithering)
                    {
                        if (PG._initialGrowTimer != null)
                        {
                            PG._initialGrowTimer.StopTimer();
                            StopCoroutine(PG.InitialCoroutine);
                            Destroy(PG._initialGrowTimer);
                        }

                        PG.AfterWateredCoroutine = StartCoroutine(PG.AfterWateredTileGrowth(PG.CurrentTimer, this));
                        iswateringStarted = true;
                        if (!UI_Manager.Instance.GrowthStartedPlants1.ContainsKey(fieldName))
                            UI_Manager.Instance.GrowthStartedPlants1[fieldName] = new List<GameObject>();
                        if (!UI_Manager.Instance.GrowthStartedPlants1[fieldName].Contains(item))
                        {
                            UI_Manager.Instance.GrowthStartedPlants1[fieldName].Add(item);
                        }
                        if (!GrowthStartedPlants.Contains(item))
                        {
                            GrowthStartedPlants.Add(item);
                        }
                    }

                }

                // WaveManager.instance.StartEnemyWave();
            }
            GrowthStartedOnThisTile.Add(tilego);
        }
        foreach (var item in spawnedSeed)
        {
            Destroy(item);
        }
    }

    public void ShowCropRemainingTimer()
    {
        if (updateValue)
        {

            TimeSpan time = TimeSpan.FromMinutes(_fieldCropRemainingCount);
            UI_Manager.Instance.fieldCropTimer.text = $"{time.Minutes:D2}:{time.Seconds:D2}";
        }
    }

    public void ShowCropRemainingTimerPanel()
    {

            UI_Manager.Instance.fieldCropTimerShowPanel.GetComponent<RectTransform>().DOAnchorPos(visiblePosition, moveDuration).SetEase(Ease.OutBack);
    }
    internal void HideShowCropRemainingTimer()
    {
        UI_Manager.Instance.fieldCropTimerShowPanel.GetComponent<RectTransform>().DOAnchorPos(hiddenPosition, moveDuration).SetEase(Ease.OutBack);
    }

    public void CompleteAction()
    {
        GameManager.Instance.HideFieldPopup();
        var trigger=this.gameObject.GetComponentInChildren<TriggerZoneCallBacks>();
        int currentStep = UI_Manager.Instance.FieldManager.fieldSteps[fieldID];

        if (currentStep < trigger.actionSequence.Length - 1)
        {
            isOneWorkingActionCompleted = true;

            if (currentStep == 1)
            {
                isPlantStartGrowing = true;
            }

            currentStep++;
            UI_Manager.Instance.FieldManager.SaveFieldStep(fieldID, currentStep);
            //UI_Manager.Instance.FieldManager.CurrentStepID = currentStep;
            UI_Manager.Instance.oldcurrentStep = currentStep;
            if (currentStep == 2)
            {
                BeforeWaterTile();
            }

            if (currentStep == 3) // Additional logic for step 3
            {

                if (this.checkPlayerInZone)
                {
                    GameManager.Instance.ShowFieldPopup(trigger.actionSequence[currentStep], this);
                }
            }
            else
            {
                GameManager.Instance.ShowFieldPopup(trigger.actionSequence[currentStep], this);

            }
        }
    }

    internal void ResetField()
    {
        //cropseedingStarted = false;
        foreach (var item in coveredtiles)
        {
            var tile = item.GetComponent<TileInfo>();
            tile.seedsSpawned = false;
            tile.plantSpawned = false;
            tile.plantgrowth = false;
            tile.isCuttingStarted = false;
            tile._hasColorChanged = false;
            tile.isTileHasSeed = false;
        }
        UI_Manager.Instance.isWentInsideOnce = false;
        spawnPlantsForInitialGrowth.Clear();
        GrowthStartedOnThisTile.Clear();
        GrowthStartedPlants.Clear();
        spawnPlantsForGrowth.Clear();
        spawnTomatosForGrowth.Clear();
        witheredPlants.Clear();
        spawnedTomatoesCount = 0;

        UI_Manager.Instance.oldcurrentStep = -1;
        UI_Manager.Instance.FieldManager.CurrentStepID = -1;
        isCutting = false;
        isPlantGrowthCompleted = false;
        isInitialPlantGrowthCompleted = false;
        isAllPlantsWithered = false;
        foreach (var item in UI_Manager.Instance.fieldPopUpImg)
        {
            item.GetComponent<POPSelectionFunctionality>()._hasBeenClicked = false;
        }
        UI_Manager.Instance.FieldManager.fieldSteps[fieldID] = -1;
        //isResetetValues = true;
        // IsHarvestCount = false;
        /// IsTomatoHarvestCount = false;
        //UI_Manager.Instance.FieldGrid.coveredtiles.Clear();
        //UI_Manager.Instance.FieldManager.fieldSteps.Clear();

    }

}





