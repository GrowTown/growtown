using DG.Tweening;
using UnityEngine;


public class Enemy : MonoBehaviour
{
    [Header("Enemy Constraints")]
    public float health = 1f;
    private Transform target; // Field or player location
    private int index;
    public float speed = 3.5f;

    public Animator animator;
    public ParticleSystem gameObjectDestroyedEffect;
    [Header("Death Feedback")]
    public AudioClip deathSfx;
    public GameObject deathVfxPrefab;
    public Vector3 deathVfxOffset = Vector3.up * 0.5f;
    [Header("Death")]
    public string deathTrigger = "IsDead";
    public float destroyDelay = 1f;
    public string hammerTag = "Hammer";
    private bool isDead;
    private Collider cachedCollider;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        cachedCollider = GetComponent<Collider>();
    }

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

        // Offset the enemy's target position based on index
        Vector3 offset = new Vector3(index * .5f, 0, index * .5f);
        Vector3 newTargetPosition = target.position + offset;

        StartMoving(newTargetPosition);
    }
    private void StartMoving(Vector3 targetPosition)
    {
        if (target != null)
        {
            transform.DOMove(targetPosition, Vector3.Distance(transform.position, targetPosition) / speed)
                     .SetEase(Ease.Linear)
                     .OnComplete(() => OnReachedTarget());
        }
    }

    private void OnReachedTarget()
    {

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

    public void Die()
    {
        if (isDead)
            return;

        isDead = true;
        DOTween.Kill(transform); // Stop any ongoing movement

        if (cachedCollider != null)
            cachedCollider.enabled = false;

        if (animator != null && !string.IsNullOrWhiteSpace(deathTrigger))
            animator.SetTrigger(deathTrigger);

        if (deathVfxPrefab != null)
            Instantiate(deathVfxPrefab, transform.position + deathVfxOffset, Quaternion.identity);

        if (deathSfx != null)
            AudioSource.PlayClipAtPoint(deathSfx, transform.position);

        Destroy(gameObject, Mathf.Max(0f, destroyDelay));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isDead && other.CompareTag(hammerTag))
            Die();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isDead && collision.collider.CompareTag(hammerTag))
            Die();
    }

    private void OnDestroy()
    {
        if (gameObjectDestroyedEffect != null)
            gameObjectDestroyedEffect.Play();
    }
}


