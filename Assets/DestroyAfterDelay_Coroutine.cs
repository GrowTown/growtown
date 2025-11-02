using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class DestroyAfterDelay_Coroutine : MonoBehaviour
{
    [SerializeField] float delay = 3f;
    [SerializeField] Animator animator;       // optional
    [SerializeField] AudioSource audioSource; // optional

    void Start()
    {
        StartCoroutine(DelayedDestroy());
    }

    IEnumerator DelayedDestroy()
    {
        // optional: play an animation
        if (animator != null) animator.SetTrigger("Die");

        // optional: play a sound
        if (audioSource != null) audioSource.Play();

        // wait either for delay or for the clip/animation length (example waits delay)
        yield return new WaitForSeconds(delay);

        Destroy(gameObject);
    }
}
