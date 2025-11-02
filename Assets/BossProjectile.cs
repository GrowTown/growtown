using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 5f;
    public float damage = 10f;
    public GameObject hitEffect;

    private Vector3 direction;

    void Start()
    {
        Destroy(gameObject, lifeTime);

        // Find the player and aim once when spawned
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            direction = (player.transform.position - transform.position).normalized;
            transform.LookAt(player.transform); // Orient projectile
        }
        else
        {
            direction = transform.forward; // fallback
        }
    }

    void Update()
    {
        // Move in the aimed direction
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boss"))
            return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit by boss projectile!");
            // TODO: call player health damage here
        }

        if (hitEffect != null)
            Instantiate(hitEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
