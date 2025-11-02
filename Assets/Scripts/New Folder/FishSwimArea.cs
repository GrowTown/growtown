using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishRandomSwim : MonoBehaviour
{
    [Header("Swim Area")]
    public Transform swimAreaCenter; // Assign in inspector
    public Vector3 swimBounds = new Vector3(5, 2, 5);

    [Header("Movement Settings")]
    public float speed = 2f;
    public float turnSpeed = 2f;

    [Header("Idle Settings")]
    public float minWaitTime = 1f;
    public float maxWaitTime = 3f;

    private Vector3 targetPos;
    private float waitCounter;
    private bool waiting;

    void Start()
    {
        // If not assigned, use current position as center
        if (swimAreaCenter == null)
        {
            GameObject fallback = new GameObject("AutoSwimArea");
            fallback.transform.position = transform.position;
            swimAreaCenter = fallback.transform;
        }

        PickNewTarget();
    }

    void Update()
    {
        if (waiting)
        {
            waitCounter -= Time.deltaTime;
            if (waitCounter <= 0f)
            {
                waiting = false;
                PickNewTarget();
            }
        }
        else
        {
            SwimTowardsTarget();
        }
    }

    void PickNewTarget()
    {
        Vector3 randomOffset = new Vector3(
            Random.Range(-swimBounds.x, swimBounds.x),
            Random.Range(-swimBounds.y, swimBounds.y),
            Random.Range(-swimBounds.z, swimBounds.z)
        );

        targetPos = swimAreaCenter.position + randomOffset;
    }

    void SwimTowardsTarget()
    {
        Vector3 dir = (targetPos - transform.position).normalized;

        if (dir != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, turnSpeed * Time.deltaTime);
        }

        transform.position += transform.forward * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPos) < 0.5f)
        {
            waiting = true;
            waitCounter = Random.Range(minWaitTime, maxWaitTime);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (swimAreaCenter != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(swimAreaCenter.position, swimBounds * 2);
        }
    }
}
