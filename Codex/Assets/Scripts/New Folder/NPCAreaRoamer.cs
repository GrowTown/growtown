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
    [SerializeField] private float minMoveDistance = 1.5f;

    [Header("Idle Timing")]
    [SerializeField] private float minIdleTime = 1.5f;
    [SerializeField] private float maxIdleTime = 3.5f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string speedParameter = "Speed";
    [SerializeField] private string stateParameter = "";
    [SerializeField] private int idleStateValue = 0;
    [SerializeField] private int walkStateValue = 1;
    [SerializeField] private bool disableRootMotion = true;

    private NavMeshAgent agent;
    private Vector3 currentDestination;
    private bool waitingForNextPoint;
    private Coroutine waitRoutine;
    private Vector3 lastPosition;
    private bool animatorValidated;
    private int? speedHash;
    private int? stateHash;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            // Let the NavMeshAgent control its own speed.
            agent.speed = moveSpeed;
            agent.stoppingDistance = stoppingDistance;
            agent.autoBraking = false;
            agent.updatePosition = false;
            agent.updateRotation = false;
        }

        if (disableRootMotion && animator != null)
        {
            animator.applyRootMotion = false;
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
        lastPosition = transform.position;
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        float actualSpeed = dt > 0f ? (transform.position - lastPosition).magnitude / dt : 0f;

        if (waitingForNextPoint)
        {
            if (agent != null)
            {
                agent.velocity = Vector3.zero;
                agent.nextPosition = transform.position;
            }

            UpdateAnimation(0f);
            lastPosition = transform.position;
            return;
        }

        if (agent != null)
        {
            if (!agent.pathPending)
            {
                Vector3 next = agent.nextPosition;
                transform.position = next;
            }

            Vector3 desiredVelocity = agent.desiredVelocity;
            if (desiredVelocity.sqrMagnitude > 0.0001f)
            {
                Quaternion desiredRotation = Quaternion.LookRotation(desiredVelocity.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
            }

            float remaining = agent.remainingDistance;
            if (!agent.pathPending && remaining <= stoppingDistance)
            {
                HandleArrival();
            }

            actualSpeed = desiredVelocity.magnitude;
            UpdateAnimation(actualSpeed);

        }
        else
        {
            SimpleMoveTowardsDestination();
            actualSpeed = dt > 0f ? (transform.position - lastPosition).magnitude / dt : 0f;
            UpdateAnimation(actualSpeed);
        }

        lastPosition = transform.position;
    }

    private void SimpleMoveTowardsDestination()
    {
        Vector3 current = transform.position;
        Vector3 target = new Vector3(currentDestination.x, current.y, currentDestination.z);

        Vector3 toTarget = target - current;
        float distance = toTarget.magnitude;

        if (distance <= stoppingDistance)
        {
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
    }

    private void HandleArrival()
    {
        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.velocity = Vector3.zero;
            agent.nextPosition = transform.position;
        }

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
            agent.nextPosition = transform.position;
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
                if (Vector3.Distance(transform.position, hit.position) < minMoveDistance)
                {
                    continue;
                }

                point = hit.position;
                return true;
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, candidate) < minMoveDistance)
            {
                continue;
            }

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
        EnsureAnimatorParameters();

        bool isMoving = speed > 0.2f;

        if (speedHash.HasValue)
        {
            animator.SetFloat(speedHash.Value, speed);
        }

        if (stateHash.HasValue)
        {
            animator.SetInteger(stateHash.Value, isMoving ? walkStateValue : idleStateValue);
        }
    }

    private void EnsureAnimatorParameters()
    {
        if (animatorValidated || animator == null) return;
        animatorValidated = true;

        speedHash = ResolveAnimatorParameterHash(speedParameter, AnimatorControllerParameterType.Float);
        stateHash = ResolveAnimatorParameterHash(stateParameter, AnimatorControllerParameterType.Int);
    }

    private int? ResolveAnimatorParameterHash(string parameterName, AnimatorControllerParameterType expectedType)
    {
        if (string.IsNullOrEmpty(parameterName)) return null;

        foreach (var param in animator.parameters)
        {
            if (param.name == parameterName)
            {
                if (param.type != expectedType)
                {
                    Debug.LogWarning($"Animator parameter '{parameterName}' is of type {param.type} but {expectedType} was expected.", this);
                    return null;
                }

                return Animator.StringToHash(parameterName);
            }
        }

        Debug.LogWarning($"Animator parameter '{parameterName}' was not found on {animator.gameObject.name}. Animation updates will be skipped for this parameter.", this);
        return null;
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
