using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlantGrowth : MonoBehaviour
{

    [SerializeField] private SkinnedMeshRenderer plantMesh; 
    private Timer _timer;
    [SerializeField] Transform[] tomatoSpawnPoints;
    private bool isWatered;
    float cuttingHight=1f;
   public void StartGrowth(bool isWatered)
    {
        if (isWatered)
        {
            _timer = this.gameObject.GetComponent<Timer>();
            if (_timer != null && UI_Manager.Instance.plantHolder != null)
            {
                StartCoroutine(GrowPlant());
                Debug.Log("Growth process started.");
            }
        }
    }

    void Start()
    {
        StartCoroutine(GrowPlant());
    }
    private IEnumerator GrowPlant()
    {
        yield return new WaitForSeconds(1);

        _timer = this.gameObject.GetComponent<Timer>();
        _timer.Initialize("Plant Growth", DateTime.Now, TimeSpan.FromMinutes(1)); 
        _timer.StartTimer();

        float totalGrowthTime = (float)_timer.timeToFinish.TotalSeconds;
        while (_timer.secondsLeft > 0)
        {
            float growthProgress = (float)(1.0f - (_timer.secondsLeft / totalGrowthTime));
            plantMesh.SetBlendShapeWeight(0, growthProgress * 100f); 
            yield return null;
        }
        TimerToolTip.ShowTimerStatic(this.gameObject);
        _timer.TimerFinishedEvent.AddListener(delegate {
            OnGrowthComplete();
            Destroy(_timer);
        });
    } 
/*    GrowPlant()
      {

          _timer.Initialize("Plant Growth", DateTime.Now, TimeSpan.FromMinutes(1));
          _timer.TimerFinishedEvent.AddListener(OnGrowthComplete);
          _timer.StartTimer();
          float totalGrowthTime = (float)_timer.timeToFinish.TotalSeconds;
          while (_timer.secondsLeft > 0)
          {
              float growthProgress = (float)(1.0f - (_timer.secondsLeft / totalGrowthTime));
              plantMesh.SetBlendShapeWeight(0, growthProgress * 100f); // Adjust blend shape for stem growth
          }
          yield return null; // Wait until the next frame
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
            Instantiate(UI_Manager.Instance.tomato, spawnPoint.position, Quaternion.identity);
        }
        Debug.Log("Tomatoes spawned at designated points.");
    }

    public void OnWaterTile()
    {
        if (UI_Manager.Instance.seedsBag.GetComponent<SeedSpawnerandSeedsBagTrigger>().CoveredTile != null)
        {
            isWatered = true;
            if (plantMesh != null)
            {
               // StartGrowth(isWatered);
                Debug.Log("Plant started growing after watering.");
            }
        }
    }

    internal bool isCutting;
    private void OnTriggerEnter(Collider other)
  {
        if (other.CompareTag("sickle"))
        {
            if (isCutting)
            {

                this.gameObject.transform.DOMoveY(cuttingHight, 0.1f);
                Destroy(this.gameObject);
            }
        }
    }

}


