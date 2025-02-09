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
        DOTween.Kill(transform); // Stop any ongoing movement
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        gameObjectDestroyedEffect.Play();
    }
}


