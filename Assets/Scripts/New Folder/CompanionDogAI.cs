using System.Collections;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class CompanionDogAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator dogAnimator;

    [Header("Follow Settings")]
    public float followDistance = 2f;        // Ideal distance behind player
    public float followHeight = 0.5f;        // Keep dog slightly raised
    public float followWalkSpeed = 3f;
    public float followRunSpeed = 6f;
    public float rotationSmooth = 4f;

    [Header("Teleport Settings")]
    public float teleportDistance = 12f;     // If dog too far → teleport
    public float teleportYOffset = 0.1f;

    [Header("Idle Behaviour")]
    public float idleRandomTime = 5f;        // Every X seconds, choose a random idle
    private float idleTimer;

    private enum DogState { Idle, Walk, Run, Eat, Rest }
    private DogState currentState = DogState.Idle;

    void Start()
    {
        idleTimer = idleRandomTime;
    }

    void Update()
    {
        HandleFollow();
    }

    private void HandleFollow()
    {
        float dt = Time.deltaTime;
        float dist = Vector3.Distance(transform.position, player.position);

        // --- Teleport if too far ---
        if (dist > teleportDistance)
        {
            TeleportToPlayer();
            return;
        }

        // --- If very close, idle with random behavior ---
        if (dist < followDistance * 0.8f)
        {
            HandleIdle(dt);
            return;
        }

        // --- Otherwise, follow player ---
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        Vector3 targetPos = player.position - dirToPlayer * followDistance;
        targetPos.y = player.position.y + followHeight;

        float speed = (dist > followDistance * 2f) ? followRunSpeed : followWalkSpeed;
        transform.position = Vector3.Lerp(transform.position, targetPos, dt * speed);

        // --- Smooth rotation towards movement direction ---
        dirToPlayer.y = 0;
        if (dirToPlayer.sqrMagnitude > 0.01f)
        {
            Quaternion rot = Quaternion.LookRotation(dirToPlayer, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, dt * rotationSmooth);
        }

        // --- Pick correct animation ---
        if (speed >= followRunSpeed * 0.9f)
            SetDogState(DogState.Run);
        else
            SetDogState(DogState.Walk);
    }

    private void HandleIdle(float dt)
    {
        idleTimer -= dt;
        if (idleTimer <= 0f)
        {
            idleTimer = idleRandomTime;

            int choice = Random.Range(0, 3); // Idle, Eat, Rest
            switch (choice)
            {
                case 0: SetDogState(DogState.Idle); break;
                case 1: SetDogState(DogState.Eat); break;
                case 2: SetDogState(DogState.Rest); break;
            }
        }
    }

    private void TeleportToPlayer()
    {
        Vector3 tpPos = player.position - player.forward * followDistance;
        tpPos.y = player.position.y + teleportYOffset;
        transform.position = tpPos;
        transform.rotation = Quaternion.LookRotation(player.forward, Vector3.up);
        SetDogState(DogState.Idle);
    }

    private void SetDogState(DogState state)
    {
        if (state == currentState) return;
        currentState = state;

        switch (state)
        {
            case DogState.Idle: dogAnimator.SetInteger("animation", 0); break;
            case DogState.Walk: dogAnimator.SetInteger("animation", 1); break;
            case DogState.Run:  dogAnimator.SetInteger("animation", 2); break;
            case DogState.Eat:  dogAnimator.SetInteger("animation", 3); break;
            case DogState.Rest: dogAnimator.SetInteger("animation", 4); break;
        }
    }
}


