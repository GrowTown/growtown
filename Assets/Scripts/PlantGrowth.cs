using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class PlantGrowth : MonoBehaviour
{

    [SerializeField] internal SkinnedMeshRenderer plantMesh;
    private Timer _timer;
    [SerializeField] Transform[] tomatoSpawnPoints;
    // private bool isWatered;
    float cuttingHight = 1f;
    private WaveManager waveManager;
    /*  public void StartGrowth(bool isWatered)
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
      }*/

    void Start()
    {
        waveManager = FindObjectOfType<WaveManager>();
        //StartCoroutine(GrowPlant());
    }
    internal IEnumerator GrowPlant()
    {
        yield return new WaitForSeconds(1);

        _timer = this.gameObject.GetComponent<Timer>();
        _timer.Initialize("Plant Growth", DateTime.Now, TimeSpan.FromMinutes(1));
        _timer.StartTimer();

        float totalGrowthTime = (float)_timer.timeToFinish.TotalSeconds;
        while (_timer.secondsLeft > 0)
        {
            float growthProgress = (float)(1.0f - (_timer.secondsLeft / totalGrowthTime));
            if (growthProgress >= 0.5f && !UI_Manager.Instance.waveStarted)
            {
                waveManager.StartWave();
                UI_Manager.Instance.waveStarted = true; // Set flag to true to prevent further calls
            }

            plantMesh.SetBlendShapeWeight(0, growthProgress * 100f);
            yield return null;
        }

        _timer.TimerFinishedEvent.AddListener(delegate
        {
            OnGrowthComplete();
            Destroy(_timer);
        });
    }

    private void OnMouseDown()
    {
        TimerToolTip.ShowTimerStatic(this.gameObject);
    }


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
        if (UI_Manager.Instance.spawnTomatosForGrowth.Count > 120)
        {
            UI_Manager.Instance.isPlantGrowthCompleted = true;
        }
    }

    public void OnWaterTile()
    {
        /* if (UI_Manager.Instance.seedsBag.GetComponent<SeedSpawnerandSeedsBagTrigger>().CoveredTile != null)
         {
             isWatered = true;
             if (plantMesh != null)
             {
                // StartGrowth(isWatered);
                 Debug.Log("Plant started growing after watering.");
             }
         }*/
    }


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
                this.gameObject.transform.DOMoveY(cuttingHight, 0.5f);
                GameManager.Instance.isHarvestCompleted = true;
                Destroy(this.gameObject);
            }

        }
    }

}


