using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovements : MonoBehaviour
{
    public Transform cam;
    [SerializeField]
    CinemachineFreeLook virtualCam;
    public Animator animator;
    Animator _dogAnimator;
    [SerializeField]
    GameObject _dog;
    [SerializeField] private float followDistance = 2.0f; 
    [SerializeField] private float followSpeed = 5.0f;
    [SerializeField] private float followRunSpeed = 6.0f;
    [SerializeField] private float followTurnSpeed = 5.0f;
    private CharacterController _controller;
    private Vector3 _moveDirection;
    public Transform lockedPositionField;
    public Transform lockedPositionMarket;

    internal Quaternion lockedRotation;
    internal bool isCameraLocked = false;
    internal bool isPlayerEnterZone = false;

    //Walk,Run variables
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float turnSpeed = 7f;
    public float turnSpeedVelocity = 0.1f;
    public float _charGroundPos = 3f;
    public float SpeedChangeRate = 10.0f;
    private float _speed;
    private float _animationBlend;


    // Variables for jump logic
    [SerializeField] private float jumpHeight = 2.0f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpCooldown = 0.5f;

    private float _verticalVelocity;
    private bool _canJump = true;

    // Ground Check variables
    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;

    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;

    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;
    private int _animIDGrounded;

    [Header("CropCycleAnimationEvents")]
    public AnimationEventTrigger animationEvents;

    [SerializeField] private Transform fieldTransform; // Reference to the field
    [SerializeField] private Collider fieldCollider; // Renderer of the field to calculate bounds
    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _dogAnimator = _dog.GetComponent<Animator>();
        _animIDGrounded = Animator.StringToHash("Grounded");
        animationEvents.CropCycleAnimationEvent.AddListener(OnAnimationEvents);
    }
    void Update()
    {
        Shader.SetGlobalVector("_Player", transform.position);
        CharMovements();
        HandleJump();
        if (!isJumping)
            AdjustHeightofPlayer();
        //UpdateVirtualCamera();
    }
    void AdjustHeightofPlayer()
    {
        var height = new Vector3(transform.position.x, _charGroundPos, transform.position.z);
        transform.position = height;
        var dogheight = new Vector3(_dog.transform.position.x, _charGroundPos, _dog.transform.position.z);
        _dog.transform.position = dogheight;
    }
    void OnAnimationEvents(string eventName)
    {
        Debug.Log(eventName);
        switch (eventName)
        {
            case "water_Start":
                UI_Manager.Instance.waterEffect.SetActive(true);
                break;
            case "water_Stop":
                UI_Manager.Instance.waterEffect.SetActive(false);
                break;
            case "refill_Mag1":
                //RefillMag();
                break;
            case "attach_Mag1":
                // AttachMag();
                break;
        }
    }
    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);

        // update animator if using character
        if (animator)
        {
            animator.SetBool(_animIDGrounded, Grounded);
        }
    }

    /*   private void CharMovements()
       {
           // Get movement input
           float moveHorizontal = Input.GetAxis("Horizontal");
           float moveVertical = Input.GetAxis("Vertical");

           // Use the camera's forward and right to calculate movement relative to its orientation
           Vector3 cameraForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized; // Flatten the forward vector
           Vector3 cameraRight = Vector3.Scale(cam.right, new Vector3(1, 0, 1)).normalized;     // Flatten the right vector

           // Calculate movement direction relative to the camera
           Vector3 inputDirection = (cameraForward * moveVertical + cameraRight * moveHorizontal).normalized;

           // Determine if running
           bool isRunning = Input.GetKey(KeyCode.LeftShift);

           // Calculate target speed
           float targetSpeed = inputDirection == Vector3.zero ? 0f : (isRunning ? runSpeed : walkSpeed);

           // Smooth acceleration and deceleration
           float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
           if (currentHorizontalSpeed < targetSpeed - 0.1f || currentHorizontalSpeed > targetSpeed + 0.1f)
           {
               _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * SpeedChangeRate);
               _speed = Mathf.Round(_speed * 1000f) / 1000f;
           }
           else
           {
               _speed = targetSpeed;
           }

           // Set blend tree speed
           _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
           if (_animationBlend < 0.01f) _animationBlend = 0f;

           // Apply movement
           Vector3 moveDirection = inputDirection * _speed;
           _controller.Move(moveDirection * Time.deltaTime);

           // Rotate the character if there's movement input
           if (inputDirection.magnitude > 0f)
           {
               Quaternion toRotation = Quaternion.LookRotation(inputDirection, Vector3.up);
               transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, turnSpeed * Time.deltaTime);
           }

           // Update animator parameters
           if (animator != null)
           {
               animator.SetFloat("Speed", _animationBlend); // Use "Speed" for blend tree transitions
               animator.SetFloat("MotionSpeed", inputDirection.magnitude); // Normalize motion input
           }

       }*/

    private void CharMovements()
    {
        // Get movement input
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Use the camera's forward and right to calculate movement relative to its orientation
        Vector3 cameraForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 cameraRight = Vector3.Scale(cam.right, new Vector3(1, 0, 1)).normalized;

        // Calculate movement direction relative to the camera
        Vector3 inputDirection = (cameraForward * moveVertical + cameraRight * moveHorizontal).normalized;

        bool isRunning = Input.GetKey(KeyCode.LeftShift) && !GameManager.Instance.checkPlayerInZone ;

        // Calculate target speed
        float targetSpeed = inputDirection == Vector3.zero ? 0f : (isRunning ? runSpeed : walkSpeed);

        // Smooth acceleration and deceleration
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
        if (currentHorizontalSpeed < targetSpeed - 0.1f || currentHorizontalSpeed > targetSpeed + 0.1f)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * SpeedChangeRate);
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        // Set blend tree speed
        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        // Apply movement
        Vector3 moveDirection = inputDirection * _speed;
        _controller.Move(moveDirection * Time.deltaTime);

        // Rotate the character if there's movement input
        if (inputDirection.magnitude > 0f)
        {
            Quaternion toRotation = Quaternion.LookRotation(inputDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, turnSpeed * Time.deltaTime);
        }

        // Update the animator parameters
        if (animator != null)
        {
            animator.SetFloat("Speed", _animationBlend); // Use "Speed" for blend tree transitions
            animator.SetFloat("MotionSpeed", inputDirection.magnitude); // Normalize motion input
        }

        // Updating the dog behavior
        // UpdateDogBehavior(inputDirection.magnitude > 0 ? (isRunning ? 2 : 1) : 0);
        UpdateDogBehavior(inputDirection.magnitude > 0, isRunning, GameManager.Instance.checkPlayerInZone);
    }

    bool isJumping;
    private void HandleJump()
    {
        // Check if the player is grounded
        if (Grounded)
        {
            _verticalVelocity = 0f;

            // Trigger jump if the spacebar is pressed and cooldown allows it
            if (Input.GetKeyDown(KeyCode.Space) && _canJump)
            {
                isJumping = true;
                _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                animator.SetTrigger("IsJumping");
                StartCoroutine(JumpCooldown());
            }
        }
        else
        {
            // Apply gravity when not grounded
            _verticalVelocity += gravity * Time.deltaTime;
        }
    }
    private IEnumerator JumpCooldown()
    {
        _canJump = false;
        yield return new WaitForSeconds(jumpCooldown);
        animator.SetBool("IsJumping", false);
        _canJump = true;
        isJumping = false;
    }

    float speed = 0.5f;


    /*   private void UpdateDogBehavior(int state)
    {

        string dogStateParam = state == 0 ? "Idle" : (state == 1 ? "Walk" : "Run");
        SetDogAnimation(dogStateParam);


        if (state != 0) // Not idle
        {
            Vector3 targetPosition = transform.position - transform.forward * followDistance + Vector3.up *0.5f;
            _dog.transform.position = Vector3.Lerp(_dog.transform.position, targetPosition, Time.deltaTime * followSpeed);
            Quaternion lookRotation = Quaternion.LookRotation(transform.position - _dog.transform.position);
            _dog.transform.rotation = Quaternion.Slerp(_dog.transform.rotation, lookRotation, Time.deltaTime * followTurnSpeed);
        }
    }*/
    /*  private void UpdateDogBehavior(bool isCharacterMoving, bool isCharacterRunning, bool isCameraLocked)
      {
          if (isCameraLocked)
          {
              SetDogAnimation("Idle");
              return;
          }
          string dogStateParam = isCharacterMoving ? "Run" : "Idle";
          SetDogAnimation(dogStateParam);
          float currentDogSpeed = isCharacterRunning ? followRunSpeed : followSpeed;
          if (isCharacterMoving)
          {
              Vector3 targetPosition = transform.position - transform.forward * followDistance + Vector3.up * 0.5f;
              _dog.transform.position = Vector3.Lerp(_dog.transform.position, targetPosition, Time.deltaTime * currentDogSpeed);
              Quaternion lookRotation = Quaternion.LookRotation(transform.position - _dog.transform.position);
              _dog.transform.rotation = Quaternion.Slerp(_dog.transform.rotation, lookRotation, Time.deltaTime * followTurnSpeed);
          }
      }*/

    /* private void UpdateDogBehavior(bool isCharacterMoving, bool isCharacterRunning, bool isCameraLocked)
     {
         if (isCameraLocked)
         {
             SetDogAnimation("Idle");
             return;
         }

         string dogStateParam = isCharacterMoving ? "Run" : "Idle";
         SetDogAnimation(dogStateParam);

         float currentDogSpeed = isCharacterRunning ? followRunSpeed : followSpeed;

         if (isCharacterMoving)
         {
             Vector3 targetPosition = transform.position - transform.forward * followDistance + Vector3.up * 0.5f;

             // Ensure the dog does not walk inside the field collider
             if (fieldTransform.TryGetComponent<Collider>(out var fieldCollider))
             {
                 Bounds fieldBounds = fieldCollider.bounds;

                 // Check if the target position is within the field's bounds
                 if (fieldBounds.Contains(targetPosition))
                 {
                     // Adjust the position to stay outside the field
                     targetPosition = fieldBounds.ClosestPoint(targetPosition);
                 }
             }

             _dog.transform.position = Vector3.Lerp(_dog.transform.position, targetPosition, Time.deltaTime * currentDogSpeed);

             Quaternion lookRotation = Quaternion.LookRotation(transform.position - _dog.transform.position);
             _dog.transform.rotation = Quaternion.Slerp(_dog.transform.rotation, lookRotation, Time.deltaTime * followTurnSpeed);
         }
     }*/
    private List<Vector3> calculatedPath = new List<Vector3>();
    private bool isAvoidingField = false;

    private void UpdateDogBehavior(bool isCharacterMoving, bool isCharacterRunning, bool isCameraLocked)
    {
        if (isCameraLocked)
        {
            SetDogAnimation("Idle");
            return;
        }

        string dogStateParam = isCharacterMoving ? "Run" : "Idle";
        SetDogAnimation(dogStateParam);

        float currentDogSpeed = isCharacterRunning ? followRunSpeed : followSpeed;

        if (isCharacterMoving)
        {
            Vector3 targetPosition = transform.position - transform.forward * followDistance + Vector3.up * 0.5f;

            if (fieldTransform.TryGetComponent<Collider>(out var fieldCollider))
            {
                Vector3 directionToTarget = targetPosition - _dog.transform.position;

                // Check if the field is nearby using raycasting
                if (Physics.Raycast(_dog.transform.position, directionToTarget.normalized, out RaycastHit hit, 3.0f))
                {
                    if (hit.collider == fieldCollider)
                    {
                        // Field detected; calculate avoidance path
                        if (!isAvoidingField)
                        {
                            isAvoidingField = true;
                            CalculatePathAvoidingField(_dog.transform.position, targetPosition, fieldCollider);
                        }
                    }
                }
                else
                {
                    // No field detected; stop avoidance
                    isAvoidingField = false;
                    calculatedPath.Clear();
                }
            }

            // Move the dog along the calculated path or directly toward the target
            if (isAvoidingField && calculatedPath.Count > 0)
            {
                Vector3 nextWaypoint = calculatedPath[0];
                _dog.transform.position = Vector3.MoveTowards(_dog.transform.position, nextWaypoint, Time.deltaTime * currentDogSpeed);

                if (Vector3.Distance(_dog.transform.position, nextWaypoint) < 0.1f)
                {
                    calculatedPath.RemoveAt(0); // Move to the next waypoint
                }

                // Rotate the dog to face the next waypoint
                if (calculatedPath.Count > 0)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(nextWaypoint - _dog.transform.position);
                    _dog.transform.rotation = Quaternion.Slerp(_dog.transform.rotation, lookRotation, Time.deltaTime * followTurnSpeed);
                }
            }
            else
            {
                // Follow directly when no avoidance is needed
                _dog.transform.position = Vector3.Lerp(_dog.transform.position, targetPosition, Time.deltaTime * currentDogSpeed);

                // Rotate the dog to face the player
                Quaternion lookRotation = Quaternion.LookRotation(transform.position - _dog.transform.position);
                _dog.transform.rotation = Quaternion.Slerp(_dog.transform.rotation, lookRotation, Time.deltaTime * followTurnSpeed);
            }
        }
    }

    /// <summary>
    /// Calculates a path avoiding the field collider to reach the target position.
    /// </summary>
    private void CalculatePathAvoidingField(Vector3 startPosition, Vector3 targetPosition, Collider fieldCollider)
    {
        calculatedPath.Clear();
        calculatedPath.Add(startPosition);

        Vector3 currentPosition = startPosition;

        while (true)
        {
            // Raycast to check if the path to the target is clear
            Vector3 directionToTarget = targetPosition - currentPosition;
            if (!Physics.Raycast(currentPosition, directionToTarget.normalized, out RaycastHit hit, directionToTarget.magnitude) || hit.collider != fieldCollider)
            {
                // Path is clear; add the target position and break
                calculatedPath.Add(targetPosition);
                break;
            }

            // If obstructed, calculate a new direction to move alongside the field
            Vector3 hitPoint = hit.point;
            Vector3 hitNormal = hit.normal;

            // Calculate a new direction perpendicular to the collider surface
            Vector3 newDirection = Vector3.Cross(hitNormal, Vector3.up).normalized;
            Vector3 avoidancePoint = hitPoint + newDirection * 0.5f;

            // Add the avoidance point to the path
            calculatedPath.Add(avoidancePoint);

            // Update the current position to the avoidance point
            currentPosition = avoidancePoint;

            // Limit iterations to prevent infinite loops
            if (calculatedPath.Count > 50)
            {
                Debug.LogWarning("Pathfinding failed: Too many iterations.");
                break;
            }
        }
    }

    /// <summary>
    /// Draws the calculated path in the Unity Editor using Gizmos.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (calculatedPath == null || calculatedPath.Count < 2)
            return;

        Gizmos.color = Color.blue;

        for (int i = 0; i < calculatedPath.Count - 1; i++)
        {
            Gizmos.DrawLine(calculatedPath[i], calculatedPath[i + 1]);
            Gizmos.DrawSphere(calculatedPath[i], 0.1f);
        }

        Gizmos.DrawSphere(calculatedPath[calculatedPath.Count - 1], 0.1f);
    }

    private void SetDogAnimation(string state)
    {
        switch (state)
        {
            case "Idle":
                SetInt("animation,0");
                break;
            case "Walk":
                SetInt("animation,1");
                break;
            case "Run":
                SetInt("animation,2");
                break;
        }
    }
    public void SetInt(string parameter = "key,value")
    {
        char[] separator = { ',', ';' };
        string[] param = parameter.Split(separator);

        string name = param[0];
        int value = Convert.ToInt32(param[1]);

        Debug.Log(name + " " + value);

        _dogAnimator.SetInteger(name, value);

    }





}







