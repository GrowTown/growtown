using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Keeps an NPC wandering around a configurable volume.
/// Works with NavMeshAgent if present, otherwise falls back to simple steering.
/// </summary>
public class NPCAreaRoamer : MonoBehaviour
{
    [Header("Roaming Area")]
    [SerializeField] private Transform areaCenter;
    [SerializeField] private Vector3 areaSize = new Vector3(8f, 0f, 8f);

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rotationSpeed = 6f;
    [SerializeField] private float stoppingDistance = 0.4f;

    [Header("Idle Timing")]
    [SerializeField] private float minIdleTime = 1.5f;
    [SerializeField] private float maxIdleTime = 3.5f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string speedParameter = "Speed";
    [SerializeField] private string stateParameter = "animation";
    [SerializeField] private int idleStateValue = 0;
    [SerializeField] private int walkStateValue = 1;

    private NavMeshAgent agent;
    private Vector3 currentDestination;
    private bool waitingForNextPoint;
    private Coroutine waitRoutine;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            // Let the NavMeshAgent control its own speed.
            agent.speed = moveSpeed;
            agent.stoppingDistance = stoppingDistance;
            agent.autoBraking = false;
        }
    }

    private void Start()
    {
        // Default area center to this transform if nothing is supplied.
        if (areaCenter == null)
        {
            areaCenter = transform;
        }

        TrySetNextDestination(true);
    }

    private void Update()
    {
        if (waitingForNextPoint)
        {
            UpdateAnimation(0f);
            return;
        }

        if (agent != null)
        {
            float remaining = agent.remainingDistance;
            float speed = agent.velocity.magnitude;
            UpdateAnimation(speed);

            if (!agent.pathPending && remaining <= stoppingDistance)
            {
                HandleArrival();
            }
        }
        else
        {
            SimpleMoveTowardsDestination();
        }
    }

    private void SimpleMoveTowardsDestination()
    {
        Vector3 current = transform.position;
        Vector3 target = new Vector3(currentDestination.x, current.y, currentDestination.z);

        Vector3 toTarget = target - current;
        float distance = toTarget.magnitude;

        if (distance <= stoppingDistance)
        {
            UpdateAnimation(0f);
            HandleArrival();
            return;
        }

        Vector3 direction = toTarget.normalized;
        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(current, target, step);

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
        }

        UpdateAnimation(moveSpeed);
    }

    private void HandleArrival()
    {
        if (waitRoutine != null)
        {
            StopCoroutine(waitRoutine);
        }

        waitRoutine = StartCoroutine(WaitAndChooseNextPoint());
    }

    private IEnumerator WaitAndChooseNextPoint()
    {
        waitingForNextPoint = true;
        UpdateAnimation(0f);

        float waitTime = Random.Range(minIdleTime, maxIdleTime);
        yield return new WaitForSeconds(waitTime);

        waitingForNextPoint = false;
        TrySetNextDestination(false);
    }

    private void TrySetNextDestination(bool forceImmediate)
    {
        if (!GetRandomPointInArea(out currentDestination))
        {
            // If we failed to find a point, try again soon.
            if (!forceImmediate && waitRoutine == null)
            {
                waitRoutine = StartCoroutine(WaitAndChooseNextPoint());
            }
            return;
        }

        if (agent != null)
        {
            agent.isStopped = false;
            agent.speed = moveSpeed;
            agent.SetDestination(currentDestination);
        }
    }

    private bool GetRandomPointInArea(out Vector3 point)
    {
        Vector3 baseCenter = areaCenter != null ? areaCenter.position : transform.position;

        for (int i = 0; i < 10; i++)
        {
            float randomX = Random.Range(-areaSize.x * 0.5f, areaSize.x * 0.5f);
            float randomZ = Random.Range(-areaSize.z * 0.5f, areaSize.z * 0.5f);
            float randomY = areaSize.y <= 0f ? 0f : Random.Range(-areaSize.y * 0.5f, areaSize.y * 0.5f);

            Vector3 candidate = baseCenter + new Vector3(randomX, randomY, randomZ);

            if (agent != null)
            {
                if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 1.5f, NavMesh.AllAreas))
                {
                    point = hit.position;
                    return true;
                }
            }
            else
            {
                point = candidate;
                return true;
            }
        }

        point = transform.position;
        return false;
    }

    private void UpdateAnimation(float speed)
    {
        if (animator == null) return;

        if (!string.IsNullOrEmpty(speedParameter))
        {
            animator.SetFloat(speedParameter, speed);
        }

        if (!string.IsNullOrEmpty(stateParameter))
        {
            animator.SetInteger(stateParameter, speed > 0.1f ? walkStateValue : idleStateValue);
        }
    }

    private void OnDisable()
    {
        if (waitRoutine != null)
        {
            StopCoroutine(waitRoutine);
            waitRoutine = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Transform center = areaCenter != null ? areaCenter : transform;

        Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.35f);
        Gizmos.matrix = Matrix4x4.TRS(center.position, center.rotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, areaSize);

        Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.75f);
        Gizmos.DrawWireCube(Vector3.zero, areaSize);
    }
}
