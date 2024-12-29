using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimalsBehaviour : MonoBehaviour
{

    private enum CowState { WalkingToDestination, Eating, WalkingBack, Resting }
    private CowState currentState;

    public Animator animator; // Reference to the Animator component
    public Transform startPoint; // Starting point
    public Transform endPoint; // End point
    public Transform turnPoint; // Turn point for semi-circle
    public float speed = 2f; // Movement speed
    public float curveResolution = 50f; // Number of points in the semi-circular path
    public float eatingDuration = 5f; // Eating time
    public float restDuration = 30f; // Resting time

    private List<Vector3> pathPoints; // All the points along the path
    private int currentTargetIndex; // Current target index along the path
    private float stateStartTime; // Tracks when the current state started
    private bool isPathGenerated = false;
    private List<float> targetRotations;

    void Start()
    {
        currentState = CowState.WalkingToDestination;
        GeneratePath();
        currentTargetIndex = 0;
        stateStartTime = Time.time;
        SetInt("animation,1"); // Start walking animation
    }

    void Update()
    {
        HandleState();
    }

    private void HandleState()
    {
        switch (currentState)
        {
            case CowState.WalkingToDestination:
                MoveAlongPath(true); // Move forward along the path
                break;

            case CowState.Eating:
                HandleEating();
                break;

            case CowState.WalkingBack:
                MoveAlongPath(false); // Move backward along the path
                break;

            case CowState.Resting:
                HandleResting();
                break;
        }
    }

    private void HandleEating()
    {
        if (Time.time - stateStartTime >= eatingDuration)
        {
            if (currentState == CowState.WalkingToDestination)
            {
                currentState = CowState.WalkingBack;
                currentTargetIndex = pathPoints.Count - 1; // Start from the end of the path
            }
            stateStartTime = Time.time;
            SetInt("animation,1"); // Resume walking animation
        }
    }

    private void HandleResting()
    {
        if (Time.time - stateStartTime >= restDuration)
        {
            currentState = CowState.WalkingToDestination;
            currentTargetIndex = 0; // Restart from the beginning of the path
            stateStartTime = Time.time;
            SetInt("animation,1"); // Start walking animation
        }
    }

    private void MoveAlongPath(bool forward)
    {
        if (currentTargetIndex >= 0 && currentTargetIndex < pathPoints.Count)
        {
            Vector3 target = pathPoints[currentTargetIndex];
            float targetRotation = targetRotations[currentTargetIndex];

            // Move towards the target point
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

            // Smoothly rotate to the target rotation
            Quaternion desiredRotation = Quaternion.Euler(0, targetRotation, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * speed);

            // Check if reached the current target
            if (Vector3.Distance(transform.position, target) < 0.1f)
            {
                currentTargetIndex += forward ? 1 : -1; // Move forward or backward
            }
        }
        else
        {
            // Transition to Eating or Resting
            if (forward)
            {
                currentState = CowState.Eating;
                SetInt("animation,4"); // Eating animation
            }
            else
            {
                currentState = CowState.Resting;
                SetInt("animation,15"); // Resting animation
            }
            stateStartTime = Time.time;
        }
    }

    private void GeneratePath()
    {
        if (isPathGenerated) return;

        pathPoints = new List<Vector3>();
        targetRotations = new List<float>();

        // Add points for straight line (start to end)
        pathPoints.Add(startPoint.position);
        targetRotations.Add(0f); // Rotation for straight line

        pathPoints.Add(endPoint.position);
        targetRotations.Add(-60f); // Rotation at the end point

        // Add points for the semi-circular path
        Vector3 center = (endPoint.position + turnPoint.position) / 2; // Center of the semi-circle
        float radius = Vector3.Distance(center, endPoint.position);

        for (float t = 0; t <= 1f; t += 1f / curveResolution)
        {
            float angle = Mathf.PI * t; // Semi-circle angle (0 to 180 degrees)
            float x = center.x + radius * Mathf.Cos(angle);
            float z = center.z + radius * Mathf.Sin(angle);
            pathPoints.Add(new Vector3(x, endPoint.position.y, z));
            targetRotations.Add(-60f + (t * -60f)); // Adjust rotation smoothly along the semi-circle
        }

        // Add points for straight line (turn to start)
        pathPoints.Add(turnPoint.position);
        targetRotations.Add(-120f); // Rotation at the turn point

        pathPoints.Add(startPoint.position);
        targetRotations.Add(-120f); // Final rotation back at the start

        isPathGenerated = true;
    }

    public void SetInt(string parameter = "key,value")
    {
        char[] separator = { ',', ';' };
        string[] param = parameter.Split(separator);

        string name = param[0];
        int value = Convert.ToInt32(param[1]);

        Debug.Log(name + " " + value);

        animator.SetInteger(name, value);
    }

    private void OnDrawGizmos()
    {
        if (pathPoints == null) return;

        Gizmos.color = Color.red;
        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            Gizmos.DrawLine(pathPoints[i], pathPoints[i + 1]);
        }
    }
}






