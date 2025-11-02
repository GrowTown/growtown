using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float rotationSpeed = 5f;
    public float wanderRadius = 5f;
    public float idleTime = 2f;

    private Vector3 targetPosition;
    private float idleTimer;
    private bool isMoving;

    private Animator anim;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
        anim = GetComponent<Animator>();
        PickNewTarget();
    }

    void Update()
    {
        if (isMoving)
        {
            MoveToTarget();
        }
        else
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0)
            {
                PickNewTarget();
            }
        }
    }

    void PickNewTarget()
    {
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        targetPosition = startPosition + new Vector3(randomCircle.x, 0, randomCircle.y);

        // small chance to stay idle instead of moving
        if (Random.value < 0.3f)
        {
            isMoving = false;
            idleTimer = Random.Range(1f, idleTime);
            SetAnimation(false);
        }
        else
        {
            isMoving = true;
            SetAnimation(true);
        }
    }

    void MoveToTarget()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
        transform.position += direction * moveSpeed * Time.deltaTime;

        float distance = Vector3.Distance(transform.position, targetPosition);
        if (distance < 0.2f)
        {
            isMoving = false;
            idleTimer = Random.Range(1f, idleTime);
            SetAnimation(false);
        }
    }

    void SetAnimation(bool walking)
    {
        if (anim != null)
            anim.SetBool("isWalking", walking);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Application.isPlaying ? startPosition : transform.position, wanderRadius);
    }
}
