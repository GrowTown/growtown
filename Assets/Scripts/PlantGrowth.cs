using System;
using System.Collections;
using UnityEngine;

public class PlantGrowth : MonoBehaviour
{

    [SerializeField] private SkinnedMeshRenderer plantMesh; // Assign your plant's Skinned Mesh Renderer here
    private Timer _timer; // Reference to the timer on this object
    [SerializeField] Transform[] tomatoSpawnPoints;
    private bool isWatered;
    private bool isGrowing = false;

    private void Awake()
    {
        
    }
    private void Start()
    {
         _timer = UI_Manager.Instance.plantHolder.GetComponent<Timer>();
    }

    public void StartGrowth(bool isWatered)
    {
        if (isWatered && !isGrowing)
        {
            _timer=gameObject.GetComponent<Timer>();
            if (_timer != null)
            {

                _timer.Initialize("Plant Growth", DateTime.Now, TimeSpan.FromMinutes(1));
                _timer.TimerFinishedEvent.AddListener(OnGrowthComplete);
                StartCoroutine(GrowPlant());
                isGrowing = true;
                Debug.Log("Growth process started.");
            } 
        }
    }

    /* private IEnumerator GrowPlant()
     {
         // Ensure the timer is set up and running
         _timer.Initialize("Plant Growth", DateTime.Now, TimeSpan.FromMinutes(1)); // Adjust growth time here
         _timer.StartTimer(); // Starts the timer

         float totalGrowthTime = (float)_timer.timeToFinish.TotalSeconds;
         while (_timer.secondsLeft > 0)
         {
             // Calculate growth progress as a percentage (0 to 1)
             float growthProgress = (float)(1.0f - (_timer.secondsLeft / totalGrowthTime));

             // Set blend shape weights according to growth progress (0% to 100%)
             plantMesh.SetBlendShapeWeight(0, growthProgress * 100f); // Stem


             yield return null; // Wait until the next frame
         }
     }*/
    private IEnumerator GrowPlant()
    {
        _timer.StartTimer();
        float totalGrowthTime = (float)_timer.timeToFinish.TotalSeconds;

        while (_timer.secondsLeft > 0)
        {
            float growthProgress = (float)(1.0f - (_timer.secondsLeft / totalGrowthTime));
            plantMesh.SetBlendShapeWeight(0, growthProgress * 100f); // Adjust blend shape for stem growth
            yield return null; // Wait until the next frame
        }
    }
    private void OnGrowthComplete()
    {
        // Ensure final blend shapes are at 100%
        plantMesh.SetBlendShapeWeight(0, 100f);

        Debug.Log("Plant growth complete!");
        SpawnTomatoes();
        isGrowing = false;
        // Clean up the timer if necessary
        Destroy(_timer);
    }
    private void SpawnTomatoes()
    {
        foreach (Transform spawnPoint in tomatoSpawnPoints)
        {
            Instantiate(UI_Manager.Instance.tomato, spawnPoint.position, Quaternion.identity);
        }
        Debug.Log("Tomatoes spawned at designated points.");
    }

    public void OnWaterTile()
    {
        if (UI_Manager.Instance.seedsBag.GetComponent<SeedSpawnerandSeedsBagTrigger>().CoveredTile != null)
        {
            isWatered = true; // Mark tile as watered

            if (plantMesh != null)
            {
                StartGrowth(isWatered); // Start growth when watered
                Debug.Log("Plant started growing after watering.");
            }
        }
    }

}


