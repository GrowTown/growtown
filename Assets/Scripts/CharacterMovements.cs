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
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float turnSpeed = 7f;
    public float turnSpeedVelocity = 0.1f;
    public float _charGroundPos = 3f;
    public float SpeedChangeRate = 10.0f;
    private float _speed;
    private float _animationBlend;
    public float backwardWalkSpeed = 3f; 
    public float backwardRunSpeed = 6f;

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

    [SerializeField] private Transform fieldTransform; 
    [SerializeField] private Collider fieldCollider; 
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

        }
    }
    private void GroundedCheck()
    {

        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);

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

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 cameraForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 cameraRight = Vector3.Scale(cam.right, new Vector3(1, 0, 1)).normalized;

        Vector3 inputDirection = (cameraForward * moveVertical + cameraRight * moveHorizontal).normalized;

        bool isRunning = Input.GetKey(KeyCode.LeftShift) && !GameManager.Instance.checkPlayerInZone;
        float targetSpeed = inputDirection == Vector3.zero ? 0f : (isRunning ? runSpeed : walkSpeed);

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

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        Vector3 moveDirection = inputDirection * _speed;
        _controller.Move(moveDirection * Time.deltaTime);

        if (inputDirection.magnitude > 0f)
        {
            Quaternion toRotation = Quaternion.LookRotation(inputDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, turnSpeed * Time.deltaTime);
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", _animationBlend); 
            animator.SetFloat("MotionSpeed", inputDirection.magnitude);
        }

        UpdateDogBehavior(inputDirection.magnitude > 0, isRunning, UI_Manager.Instance.IsPlayerInSecondZone);
    }



    bool isJumping;
    private void HandleJump()
    {
        if (Grounded)
        {
            _verticalVelocity = 0f;

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
            _dog.transform.position = Vector3.Lerp(_dog.transform.position, targetPosition, Time.deltaTime * currentDogSpeed);
            Quaternion lookRotation = Quaternion.LookRotation(transform.position - _dog.transform.position);
            _dog.transform.rotation = Quaternion.Slerp(_dog.transform.rotation, lookRotation, Time.deltaTime * followTurnSpeed);
        }
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







