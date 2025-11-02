using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BossController : MonoBehaviour
{
    [Header("References")]
    public Transform player;                 // Target (assigned by ArenaTrigger)
    public NavMeshAgent agent;               // NavMeshAgent for movement
    public Animator anim;                    // Animator for animations
    public GameObject projectilePrefab;      // Boss projectile prefab
    public Transform firePoint;              // Firing position (like a gun/barrel)
    public BossHealth health;                // Reference to boss health

    [Header("Combat Settings")]
    public float chaseRange = 20f;           // How far the boss starts chasing
    public float attackRange = 10f;          // Range to start shooting
    public float fireCooldown = 2f;          // Delay between attacks
    public float projectileSpeed = 15f;      // Speed of fired projectiles
    public float damagePerHit = 20f;         // Damage applied to player

    private float fireTimer = 0f;
    private bool isDead = false;

    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
        if (anim == null)
            anim = GetComponent<Animator>();
        if (health == null)
            health = GetComponent<BossHealth>();

        agent.updateRotation = false;  // We rotate manually
    }

    void Update()
    {
        if (isDead || player == null || health == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        // ✅ CHASE the player if far
        if (distance > attackRange && distance <= chaseRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            anim.Play("Run_guard_AR"); // your running animation
        }

        // ✅ ATTACK when close enough
        else if (distance <= attackRange)
        {
            agent.isStopped = true;
            FacePlayer();

            fireTimer -= Time.deltaTime;
            if (fireTimer <= 0f)
            {
                AttackPlayer();
                fireTimer = fireCooldown;
            }
        }

        // ✅ IDLE if player is far away
        else if (distance > chaseRange)
        {
            agent.isStopped = true;
            anim.Play("Idle_Guard_AR");
        }
    }

    private void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f;
        if (direction.magnitude > 0.01f)
        {
            Quaternion lookRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 5f);
        }
    }

    private void AttackPlayer()
    {
        if (anim != null)
            anim.Play("Shoot_SingleShot_AR");

        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 dir = (player.position - firePoint.position).normalized;
                rb.velocity = dir * projectileSpeed;
            }

            Projectile p = projectile.GetComponent<Projectile>();
            if (p != null)
                p.damage = damagePerHit;

            Destroy(projectile, 3f); // cleanup
        }
    }

    public void OnDeath()
    {
        isDead = true;
        agent.isStopped = true;
        anim.Play("Die");
    }
}
