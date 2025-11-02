using System.Collections;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedThrower : MonoBehaviour
{
    [Header("Seed Throw Settings")]
    public Transform throwPoint;
    public float throwForce = 5f;
    public int seedsPerThrow = 4;

    public void ThrowSeeds()
    {
        if (UI_Manager.Instance == null || UI_Manager.Instance.seed == null)
        {
            Debug.LogWarning("Seed prefab not assigned in UI_Manager!");
            return;
        }

        for (int i = 0; i < seedsPerThrow; i++)
        {
            // Spawn at throw point
            GameObject seed = Instantiate(UI_Manager.Instance.seed, throwPoint.position, Quaternion.identity);
            seed.transform.localScale = Vector3.one * 0.04f;

            Rigidbody rb = seed.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Use throwPoint.forward, not player transform.forward
                Vector3 throwDir = throwPoint.forward + new Vector3(
                    Random.Range(-0.1f, 0.1f),
                    0.2f,
                    Random.Range(-0.1f, 0.1f)
                );
                rb.AddForce(throwDir * throwForce, ForceMode.Impulse);
            }

            Destroy(seed, 5f); // cleanup after 5s
        }
    }
}
