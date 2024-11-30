using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButterflyAnimationDelay : MonoBehaviour
{
    [SerializeField]
    private Animation butterflyAnimation;

    void Start()
    {
        butterflyAnimation.Stop();
        //butterflyAnimation = GetComponent<Animation>();

        if (butterflyAnimation != null)
        {
           
            float delay = Random.Range(0f, 3f);
            Invoke(nameof(PlayAnimationWithDelay), delay);
        }
    }

    private void PlayAnimationWithDelay()
    {
        butterflyAnimation.Play();
    }
}


