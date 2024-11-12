using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float health = 1f;
    [SerializeField]private NavMeshAgent navAgent;
    private Transform target; // Field or player location
    public float speed = 3.5f;
    public Animator animator;
    //private NavMeshAgent navMeshAgent;

    void Awake()
    {
       // navAgent = GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// Initialize the enemy with a target and start moving toward it
    /// </summary>
    /// <param name="target"></param>
    public void Initialize(Transform target)
    {
        this.target = target;

        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }
        if(navAgent == null)
        {
            Debug.Log("Can't Find NavMesh");
        }
        navAgent.enabled = true;
        navAgent.speed = speed;
        navAgent.SetDestination(target.position);


      /*  if (animator != null)
        {
            animator.SetBool("enemyIsRunning", true); // Start running animation
        }*/
    }

    void Update()
    {

        if (navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            navAgent.isStopped = true;
         /*   if (animator != null)
            {
                animator.SetBool("enemyIsRunning", false); // Stop running animation
            }*/
        }
        // Check if enemy reached the target (e.g., field area)
        if (Vector3.Distance(transform.position, target.position) < 1f)
        {
           // animator.SetLayerWeight(1, 1);
            // Trigger any effects or damage to the field area
            //gameObject.SetActive(false);
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.SetActive(false );
        }
    }
}


