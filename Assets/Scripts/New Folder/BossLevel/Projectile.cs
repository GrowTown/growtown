using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{

  public float damage = 20f;
    public ParticleSystem hitEffect;
    public AudioClip hitSound;
    public AudioSource audioSource;

    private void OnTriggerEnter(Collider other)
    {
        // Hit Player
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            if (hitEffect != null) Instantiate(hitEffect, transform.position, Quaternion.identity);
            if (audioSource != null && hitSound != null)
                audioSource.PlayOneShot(hitSound);
            Destroy(gameObject);
            return;
        }

        // Hit Environment (optional)
        if (other.CompareTag("Environment"))
        {
            if (hitEffect != null) Instantiate(hitEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
