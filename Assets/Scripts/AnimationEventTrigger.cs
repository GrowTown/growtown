using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerAnimationEvent()
    {
        UI_Manager.Instance.seedsBag.GetComponent<SeedSpawnerandSeedsBagTrigger>().OnThrowSeed();
    }
    public void TriggerAnimationEvent1()
    {
        UI_Manager.Instance.seedsBag.GetComponent<SeedSpawnerandSeedsBagTrigger>().OnHandInBag();
    }

    public void TriggerToStartWater()
    {
        UI_Manager.Instance.waterPartileGO.SetActive(true);
        //UI_Manager.Instance.wateringTool.GetComponent<PourDetector>().StartPour();
    }

    public void TriggerWaterStop()
    {

        UI_Manager.Instance.waterPartileGO.SetActive(false);
        //UI_Manager.Instance.wateringTool.GetComponent<PourDetector>().EndPour();
    }

}
