using System;
using UnityEngine;

public class AnimalsBehaviour : MonoBehaviour
{
    private enum CowState { WalkingToDestination, Eating, WalkingBack, Resting }
    private CowState currentState;

    public Animator animator; // Reference to the Animator component
    public Transform startPosition; // Starting point of the cow
    public Transform endPosition; // Destination point of the cow
    public float stepDuration = 2f; // Time for each step
    public float restDuration = 30f; // Rest time
    private Vector3 nextTarget; // Next target position
    private float stateStartTime; // Tracks when the current state started
    public float eatingDuration = 5f; // Eating time

    void Start()
    {
        currentState = CowState.WalkingToDestination;
        nextTarget = GetNextStep(startPosition.position, endPosition.position);
        stateStartTime = Time.time;
        SetInt("animation,1"); // Set walking animation
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
                WalkBehavior();
                break;

            case CowState.Eating:
                HandleEating();
                break;

            case CowState.WalkingBack:
                WalkBehavior();
                break;

            case CowState.Resting:
                HandleResting();
                break;
        }
    }

    private void WalkBehavior()
    {
        transform.position = Vector3.MoveTowards(transform.position, nextTarget, Time.deltaTime);

        if (Vector3.Distance(transform.position, nextTarget) < 0.1f)
        {
            currentState = CowState.Eating;
            stateStartTime = Time.time;
            SetInt("animation,4"); // Set eating animation
        }
    }

    private void HandleEating()
    {
        if (Time.time - stateStartTime >= eatingDuration)
        {
            // Determine the next state: Continue walking or transition to walking back
            if (currentState == CowState.WalkingToDestination)
            {
                nextTarget = GetNextStep(transform.position, endPosition.position);

                if (Vector3.Distance(transform.position, endPosition.position) < 0.1f)
                {
                    currentState = CowState.WalkingBack;
                    nextTarget = GetNextStep(endPosition.position, startPosition.position);
                }
            }
            else if (currentState == CowState.WalkingBack)
            {
                nextTarget = GetNextStep(transform.position, startPosition.position);

                if (Vector3.Distance(transform.position, startPosition.position) < 0.1f)
                {
                    currentState = CowState.Resting;
                    stateStartTime = Time.time;
                    SetInt("animation,5"); // Set resting animation
                    return;
                }
            }

            currentState = currentState == CowState.WalkingToDestination ? CowState.WalkingToDestination : CowState.WalkingBack;
            stateStartTime = Time.time;
            SetInt("animation,1"); 
        }
    }

    private void HandleResting()
    {
        if (Time.time - stateStartTime >= restDuration)
        {
            currentState = CowState.WalkingToDestination;
            nextTarget = GetNextStep(startPosition.position, endPosition.position);
            stateStartTime = Time.time;
            SetInt("animation,1"); // Set walking animation
        }
    }

    private Vector3 GetNextStep(Vector3 current, Vector3 target)
    {
        return Vector3.MoveTowards(current, target, Vector3.Distance(current, target) / 4); // Divide the distance into 4 steps
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
}




