using System;
using UnityEngine;

public class AnimalsBehaviour : MonoBehaviour
{
    private enum CowState { WalkingToDestination, Walking,Turning,Eating, WalkingBack, Resting }
    private CowState currentState;

    public Animator animator; // Reference to the Animator component
    public Transform startPosition; // Starting point of the cow
    public Transform endPosition; // Destination point of the cow
    public float stepDuration = 2f; // Time for each step
    public float restDuration = 30f; // Rest time
    private Vector3 nextTarget; // Next target position
    private float stateStartTime; // Tracks when the current state started
    public float eatingDuration = 5f; // Eating time
    public float turnRadius = 2f; // Radius of the semi-circle turn
    public int turnSegments = 10; // Number of steps in the turn
    public float turnSpeed = 2f; // Speed for turning

    private int currentTurnStep = 0; // Step index for the semi-circle turn
    private Vector3[] turnPath; // Path points for the semi-circle turn
    public int stepsBeforeEating = 4; // Number of steps before eating
    private int currentStep;
    void Start()
    {
        currentState = CowState.Walking;
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
            case CowState.Walking:
                HandleWalking();
                break;
            case CowState.Turning:
                HandleTurning();
                break;
            case CowState.Eating:
                HandleEating();
                break;

            case CowState.WalkingBack:
                HandleWalking();
                break;

            case CowState.Resting:
                HandleResting();
                break;
        }
    }

    /*private void WalkBehavior()
    {
        transform.position = Vector3.MoveTowards(transform.position, nextTarget, Time.deltaTime);

        if (Vector3.Distance(transform.position, nextTarget) < 0.1f)
        {
            currentState = CowState.Eating;
            stateStartTime = Time.time;
            SetInt("animation,4"); // Set eating animation
        }
    }*/
    private void HandleWalking()
    {
        transform.position = Vector3.MoveTowards(transform.position, nextTarget, Time.deltaTime);

        if (Vector3.Distance(transform.position, nextTarget) < 0.1f)
        {
            currentStep++;
            if (currentStep % stepsBeforeEating == 0)
            {
                currentState = CowState.Eating;
                stateStartTime = Time.time;
                SetInt("animation,4"); // Set eating animation
            }
            else
            {
                nextTarget = GetNextStep(nextTarget, currentStep % stepsBeforeEating == 0 ? startPosition.position : endPosition.position);
            }
        }

        // Transition to turning state when reaching the destination
        if (currentStep % stepsBeforeEating == 0 && Vector3.Distance(transform.position, endPosition.position) < 0.1f)
        {
            PrepareForTurn(endPosition.position, true); // Turn away from the destination
            currentState = CowState.Turning;
        }
        else if (currentStep % stepsBeforeEating == 0 && Vector3.Distance(transform.position, startPosition.position) < 0.1f)
        {
            PrepareForTurn(startPosition.position, false); // Turn away from the start
            currentState = CowState.Turning;
        }
    }

    private void HandleTurning()
    {
        if (currentTurnStep < turnPath.Length)
        {
            transform.position = Vector3.MoveTowards(transform.position, turnPath[currentTurnStep], turnSpeed * Time.deltaTime);
            Vector3 direction = (turnPath[currentTurnStep] - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            }

            if (Vector3.Distance(transform.position, turnPath[currentTurnStep]) < 0.1f)
            {
                currentTurnStep++;
            }
        }
        else
        {
            currentState = CowState.Walking;
            nextTarget = currentStep % stepsBeforeEating == 0 ? startPosition.position : endPosition.position;
            stateStartTime = Time.time;
            SetInt("animation,1"); // Set walking animation
        }
    }

    private void PrepareForTurn(Vector3 pivotPoint, bool isTurningAway)
    {
        // Create semi-circle path
        Vector3 direction = isTurningAway ? (pivotPoint - transform.position).normalized : (transform.position - pivotPoint).normalized;
        Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;

        turnPath = new Vector3[turnSegments + 1];
        for (int i = 0; i <= turnSegments; i++)
        {
            float angle = Mathf.PI * i / turnSegments; // Semi-circle angle
            Vector3 offset = Mathf.Cos(angle) * direction * turnRadius + Mathf.Sin(angle) * perpendicular * turnRadius;
            turnPath[i] = pivotPoint + offset;
        }

        currentTurnStep = 0;
    }
    /* private void HandleEating()
     {
         if (Time.time - stateStartTime >= eatingDuration)
         {
             if (Vector3.Distance(transform.position, endPosition.position) >= 0.1f)
             {
                 currentState = CowState.WalkingToDestination;
             }
             else
             {
                 currentState = CowState.WalkingBack;
             }
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
             //currentState = currentState == CowState.WalkingToDestination ? CowState.WalkingToDestination : CowState.WalkingBack;
             stateStartTime = Time.time;
             SetInt("animation,1");
         }
     }*/
    private void HandleEating()
    {
        if (Time.time - stateStartTime >= eatingDuration)
        {
            currentState = CowState.WalkingToDestination;
            nextTarget = GetNextStep(transform.position, currentStep % stepsBeforeEating == 0 ? startPosition.position : endPosition.position);
            SetInt("animation,1"); // Set walking animation
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




