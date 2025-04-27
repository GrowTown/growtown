using UnityEngine;
using UnityEngine.Events;

public class AnimationEvent : UnityEvent<string>
{

}
public class AnimationEventTrigger : MonoBehaviour
{

    public AnimationEvent CropCycleAnimationEvent = new AnimationEvent();



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnAnimationEvent(string eventName)
    {
        CropCycleAnimationEvent.Invoke(eventName);
    }
    public void TriggerForCleaningEffectStart()
    {
        UI_Manager.Instance.cleanigEffect.SetActive(true);
        AudioManager.Instance.PlayMusic(AudioManager.Instance.cleaningMusic, AudioManager.Instance.sfxVolume);
    }
    public void TriggerForCleanigEffectStop()
    {
        UI_Manager.Instance.cleanigEffect.SetActive(false);
        
    }
    public void TriggerAnimationEvent()
    {
        //UI_Manager.Instance.seedsBag.GetComponent<SeedSpawnerandSeedsBagTrigger>().OnThrowSeed();
    }
    public void TriggerAnimationEvent1()
    {
        UI_Manager.Instance.seedsBag.GetComponent<SeedSpawnerandSeedsBagTrigger>().OnHandInBag();
    }

    public void TriggerToStartWater()
    {
        UI_Manager.Instance.waterEffect.SetActive(true);
        //UI_Manager.Instance.wateringTool.GetComponent<PourDetector>().StartPour();
    }

    public void TriggerShotGunAnimationEvent()
    {
        UI_Manager.Instance.seedsBag.GetComponent<SeedSpawnerandSeedsBagTrigger>().OnGunInHand();
    }

    /*    public void TriggerWaterStop()
        {

            UI_Manager.Instance.waterEffect.SetActive(false);
            //UI_Manager.Instance.wateringTool.GetComponent<PourDetector>().EndPour();
        }*/

}
