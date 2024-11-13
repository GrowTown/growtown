using DG.Tweening;
using UnityEngine;


public class Enemy : MonoBehaviour
{
    /*  public float health = 1f;
      [SerializeField] private NavMeshAgent navAgent;
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
          if (navAgent == null)
          {
              Debug.Log("Can't Find NavMesh");
          }
          navAgent.enabled = true;
          navAgent.speed = speed;
          navAgent.SetDestination(target.position);


          if (animator != null)
          {
              animator.SetBool("enemyIsRunning", true); // Start running animation
          }
      }

      void Update()
      {

          if (navAgent.remainingDistance <= navAgent.stoppingDistance)
          {
              navAgent.isStopped = true;
              if (animator != null)
              {
                  animator.SetBool("enemyIsRunning", false); // Stop running animation
              }
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
  */
    public float health = 1f;
    private Transform target; // Field or player location
    public float speed = 3.5f;
    public Animator animator;
    int index = 0;
    private bool isMoving = false;

    /// <summary>
    /// Initialize the enemy with a target and start moving toward it
    /// </summary>
    /// <param name="target"></param>
    public void Initialize(Transform target, int index)
    {
        this.target = target;
        this.index = index;

        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }

        StartMoving();
    }

    private void StartMoving()
    {
        if (target != null)
        {
            isMoving = true;

            // Move the enemy towards the target's position with DOTween
            transform.DOMove(target.position, Vector3.Distance(transform.position, target.position) / speed)
                     .SetEase(Ease.Linear)
                     .OnComplete(() => OnReachedTarget());
        }
    }

    private void OnReachedTarget()
    {
        isMoving = false;
        //animator.SetTrigger("Attack");
        if (index ==2 )
        {
           
            transform.rotation = target.rotation;
        }
        else if(index==3 ) 
        {
           
            transform.rotation = target.rotation;
        }
        // Start oscillating movement along the x-axis after reaching the target
        Vector3 leftPosition = transform.position + new Vector3(-0.5f, 0, 0);
        Vector3 rightPosition = transform.position + new Vector3(0.5f, 0, 0);

        // Create an infinite loop going back and forth between left and right positions
        transform.DOPath(new Vector3[] { leftPosition, rightPosition }, 1f, PathType.Linear)
                 .SetEase(Ease.InOutSine)
                 .SetLoops(-1, LoopType.Yoyo);
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
        isMoving = false;
        DOTween.Kill(transform); // Stop any ongoing movement
        gameObject.SetActive(false);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("hammer"))
        {
            gameObject.SetActive(false);
        }
    }
}


