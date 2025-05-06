using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMovements : MonoBehaviour
{
    public Transform cam;
    [SerializeField] CinemachineFreeLook freeLookCam;
    [SerializeField]private float normalRotationSpeed = 100f;
    [SerializeField]private float slowedRotationSpeed = 50f;
   
    public Animator animator;
    public Joystick joystick;
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
    public float backRunSpeed = -4f;
    public float backWalkSpeed = -2f;

    public float turnSpeed = 7f;
    public float turnSpeedVelocity = 0.1f;
    public float _charGroundPos = 3f;
    public float SpeedChangeRate = 10.0f;
    private float _speed;
    public float speedMultiplier = 1f;
    private float _animationBlend;
    public float backwardWalkSpeed = 3f;
    public float backwardRunSpeed = 6f;

    // Variables for jump logic
    [SerializeField] private float jumpHeight = 2.0f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpCooldown = 0.5f;

    private float _verticalVelocity;
    private bool _canJump = true;

    //for seed throw
    [Header("For seed throw")]
    public Transform throwPoint;   
    public float throwForce = 5f;  
    private int seedsPerThrow = 4;


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

    [Header("Gun Camera Constraints")]
    public float gunYawLimit = 60f; 
    public float gunPitchLimit = 60; 


    // Public fields for camera rotation
    public float CinemachineTargetYaw { get; private set; } 
    public float CinemachineTargetPitch { get; private set; } 
    public float CameraHeight = 1.5f;
    public float maxCameraHeight = 1.6f;
    private float currentYawLimit = 0f;
    private float currentPitchLimit = 0f;

    [SerializeField] private Transform fieldTransform;
    [SerializeField] private Collider fieldCollider;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    public float Sensitivity = 1.0f;

    private bool isAndroid;

    //Getting Character is Moving or Not
    private Vector3 lastPosition;
    private float idleTime = 0f;
    public float idleThreshold = 5f; 
    public float BottomClamp = -0f;
    public float TopClamp = 0f;

    private const float _threshold = 0.01f;
    [SerializeField] private GameObject CinemachineCameraTarget;
    public float CameraAngleOverride = 0.0f;
    public bool LockCameraPosition = false;

    private void Start()
    {
        // Detect if the game is running on Android
#if UNITY_ANDROID
        isAndroid = true;
#else
            isAndroid = false;
#endif

       
        // Disable joystick if not on Android
        if (!isAndroid && joystick != null)
        {
            joystick.gameObject.SetActive(false);
        }
        _controller = GetComponent<CharacterController>();
        _dogAnimator = _dog.GetComponent<Animator>();
        _animIDGrounded = Animator.StringToHash("Grounded");
        animationEvents.CropCycleAnimationEvent.AddListener(OnAnimationEvents);

        lastPosition = transform.position;
        CinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
        CinemachineTargetPitch = CinemachineCameraTarget.transform.rotation.eulerAngles.x;
    }

    void Update()
    {
        Shader.SetGlobalVector("_Player", transform.position);
        CharMovements();
        HandleJump();
        if (!isJumping)
            AdjustHeightofPlayer();
        //UpdateVirtualCamera();
        if (UI_Manager.Instance.WeaponAttackEvent.isGunActive)
        {
            UpdateConstraints();
            Camerarotation();
        }

        // Check if position has changed
        if (transform.position != lastPosition)
        {
            idleTime = 0f; // Reset idle timer
            lastPosition = transform.position; // Update last position
            ChangeOpacityOfJoystick(.5f);
        }
        else
        {
            idleTime += Time.deltaTime; // Increase idle time
        }

        // Check if idle time exceeded the threshold
        if (idleTime >= idleThreshold)
        {
            OnIdle();
            idleTime = 0f; // Reset after triggering (optional)
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();

           
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

    }

    void OnIdle()
    {
        Debug.Log("Character has been idle for 5 seconds!");
        ChangeOpacityOfJoystick(.2f);
    }

    private void ChangeOpacityOfJoystick(float opacity) 
    {

        var joysticjImage = joystick.gameObject.GetComponent<Image>();
        var joystickHandleImage = joystick.gameObject.transform.GetChild(0).GetComponent<Image>();

        Color colorJ = joysticjImage.color;
        colorJ.a = opacity;
        joysticjImage.color = colorJ;

        Color colorH = joystickHandleImage.color;
        colorH.a = opacity;
        joystickHandleImage.color = colorH;
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
            case "ReloadStarted":
                WeaponHandler.instance.OnWeaponAvtive(.2f, eventName);
                break;

            case "ReloadStop":
                break;

            case "ReloadReset":
                WeaponHandler.instance.OnWeaponAvtive(.2f, eventName);
                break;

        }
    }
 
    private Coroutine resetCameraCoroutine;

    void UpdateConstraints()
    {
        float targetYawLimit = UI_Manager.Instance.WeaponAttackEvent.isGunActive ? gunYawLimit : float.MaxValue;
        float targetPitchLimit = UI_Manager.Instance.WeaponAttackEvent.isGunActive ? gunPitchLimit : float.MaxValue;

        currentYawLimit = Mathf.Lerp(currentYawLimit, targetYawLimit, Time.deltaTime * 5f);
        currentPitchLimit = Mathf.Lerp(currentPitchLimit, targetPitchLimit, Time.deltaTime * 5f);
    }
    internal bool iscameraReset = false;


    /*   void Camerarotation()
       {
           // Reset camera to back of player when gun is active
           if (UI_Manager.Instance.WeaponAttackEvent.isGunActive && !iscameraReset)
           {
               StartResetCamera();
               iscameraReset = true;
               return; // Prevent further camera rotation while gun is active
           }

           float lookX = Input.GetAxisRaw("Mouse X") * Sensitivity;
           float lookY = Input.GetAxisRaw("Mouse Y") * Sensitivity;

           // If there is an input and camera position is not locked
           if ((Mathf.Abs(lookX) >= _threshold || Mathf.Abs(lookY) >= _threshold) && !LockCameraPosition)
           {
               CinemachineTargetYaw += lookX * Sensitivity;
               CinemachineTargetPitch -= lookY * Sensitivity; // Invert Y for natural control
           }

           // Apply constraints
           CinemachineTargetYaw = Mathf.Clamp(CinemachineTargetYaw, -currentYawLimit, currentYawLimit);
           CinemachineTargetPitch = Mathf.Clamp(CinemachineTargetPitch, -currentPitchLimit, currentPitchLimit);

           // Clamp rotation
           CinemachineTargetYaw = ClampAngle(CinemachineTargetYaw, float.MinValue, float.MaxValue);
           CinemachineTargetPitch = ClampAngle(CinemachineTargetPitch, BottomClamp, TopClamp);

           // Apply rotation to Cinemachine target
           CinemachineCameraTarget.transform.rotation = Quaternion.Euler(CinemachineTargetPitch + CameraAngleOverride, CinemachineTargetYaw, 0.0f);

           // Update camera height (if needed)
           Vector3 cameraPosition = CinemachineCameraTarget.transform.position;
           cameraPosition.y = Mathf.Clamp(cameraPosition.y, maxCameraHeight, float.MaxValue);
           CinemachineCameraTarget.transform.position = cameraPosition;
       }*/


    void Camerarotation()
    {
        // Reset camera when gun is activated, but allow further rotation
        if (UI_Manager.Instance.WeaponAttackEvent.isGunActive && !iscameraReset)
        {
            StartResetCamera();
            iscameraReset = true;
        }

        float lookX = Input.GetAxisRaw("Mouse X") * Sensitivity;
        float lookY = Input.GetAxisRaw("Mouse Y") * Sensitivity;

        if ((Mathf.Abs(lookX) >= _threshold || Mathf.Abs(lookY) >= _threshold) && !LockCameraPosition)
        {
            CinemachineTargetYaw += lookX * Sensitivity;
            CinemachineTargetPitch -= lookY * Sensitivity;
        }

        //  Fix: Ensure Clamping is Correct
        CinemachineTargetYaw = Mathf.Clamp(CinemachineTargetYaw, -currentYawLimit, currentYawLimit);
        CinemachineTargetPitch = Mathf.Clamp(CinemachineTargetPitch, BottomClamp, TopClamp);

        // Apply final rotation
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(CinemachineTargetPitch + CameraAngleOverride, CinemachineTargetYaw, 0.0f);
    }

    private void StartResetCamera()
    {
        if (resetCameraCoroutine != null)
        {
            StopCoroutine(resetCameraCoroutine);
        }
        resetCameraCoroutine = StartCoroutine(ResetCameraToBackCoroutine());
    }

    private IEnumerator ResetCameraToBackCoroutine()
    {
        float startYaw = CinemachineTargetYaw;
        float desiredYaw = transform.eulerAngles.y;
        float duration = 0.5f; // Adjust for faster/slower transition
        float elapsed = 0f;

        while (elapsed < duration)
        {
            CinemachineTargetYaw = Mathf.LerpAngle(startYaw, desiredYaw, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        CinemachineTargetYaw = desiredYaw; // Ensure final alignment
        resetCameraCoroutine = null;
    }
    private float ClampAngle(float angle, float min, float max)
    {
        while (angle < -360f) angle += 360f;
        while (angle > 360f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
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



    /* private void CharMovements()
     {
         float moveHorizontal;
         float moveVertical;

         if (isAndroid && joystick != null)
         {
             moveHorizontal = joystick.Horizontal;
             moveVertical = joystick.Vertical;
         }
         else
         {
             moveHorizontal = Input.GetAxis("Horizontal");
             moveVertical = Input.GetAxis("Vertical");
         }

         Vector3 inputDirection = new Vector3(moveHorizontal, 0f, moveVertical).normalized;
           inputDirection = Quaternion.AngleAxis(cam.rotation.eulerAngles.y, Vector3.up) * inputDirection.normalized;
         if (freeLookCam != null)
         {
             if (UI_Manager.Instance.IsPlayerInSecondZone)
             {
                 freeLookCam.m_XAxis.m_MaxSpeed = slowedRotationSpeed;
                // freeLookCam.m_YAxis.m_MaxSpeed = slowedRotationSpeed;
             }
             else
             {
                 freeLookCam.m_XAxis.m_MaxSpeed = normalRotationSpeed;
                 //freeLookCam.m_YAxis.m_MaxSpeed = normalRotationSpeed;
             }
         }


         bool isGunActive = UI_Manager.Instance.WeaponAttackEvent.isGunActive;
         float joystickMagnitude = new Vector2(joystick.Horizontal, joystick.Vertical).magnitude;

         bool isRunning = isAndroid ? new Vector2(joystick.Horizontal, joystick.Vertical).magnitude >= 0.9f && !UI_Manager.Instance.IsPlayerInSecondZone
             : Input.GetKey(KeyCode.LeftShift) && !UI_Manager.Instance.IsPlayerInSecondZone;


         if (animator != null)
         {
             if (isGunActive)
             {
                 if (isRunning)
                 {

                     animator.SetLayerWeight(2, 1f);
                     animator.SetLayerWeight(1, 0f);
                     animator.SetLayerWeight(0, 0f);
                     UI_Manager.Instance.WeaponAttackEvent.leftHandPos.weight = 0f;
                     UI_Manager.Instance.WeaponAttackEvent.aim.weight = 0f;
                 }
                 else
                 {

                     animator.SetLayerWeight(1, 1f);
                     animator.SetLayerWeight(2, 0f);
                     animator.SetLayerWeight(0, 1f);
                     UI_Manager.Instance.WeaponAttackEvent.leftHandPos.weight = 1f;
                     UI_Manager.Instance.WeaponAttackEvent.aim.weight = 0.4f;
                 }
             }
             else
             {

                 animator.SetLayerWeight(0, 1f);
                 animator.SetLayerWeight(1, 0f);
                 animator.SetLayerWeight(2, 0f);
             }

         }


         // Adjust movement speed
         float targetSpeed = inputDirection == Vector3.zero ? 0f :(isRunning ? runSpeed * speedMultiplier : walkSpeed * speedMultiplier);

         // Smooth speed adjustment
         float currentSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
         _speed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * SpeedChangeRate);

         // Move the player in the correct direction
         Vector3 moveDirection = inputDirection * _speed;
         _controller.Move(moveDirection * Time.deltaTime);

         // Rotate player to face movement direction
         if (inputDirection.magnitude > 0f)
         {
             Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
             transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, turnSpeed * Time.deltaTime);
         }

         // Update animations
         if (animator != null)
         {
             animator.SetFloat("Speed", _speed);
             animator.SetFloat("MotionSpeed", inputDirection.magnitude);
         }
         UpdateDogBehavior(inputDirection.magnitude > 0, isRunning, UI_Manager.Instance.IsPlayerInSecondZone);
     }*/


    private void CharMovements()
    {
        float moveHorizontal;
        float moveVertical;

        if (isAndroid && joystick != null)
        {
            moveHorizontal = joystick.Horizontal;
            moveVertical = joystick.Vertical;
        }
        else
        {
            moveHorizontal = Input.GetAxis("Horizontal");
            moveVertical = Input.GetAxis("Vertical");
        }

        Vector3 inputDirection = new Vector3(moveHorizontal, 0f, moveVertical).normalized;
        inputDirection = Quaternion.AngleAxis(cam.rotation.eulerAngles.y, Vector3.up) * inputDirection.normalized;

        // Detect movement direction relative to character forward
        bool isMovingBackward = Vector3.Dot(transform.forward, inputDirection) < -0.3f;
        bool isMovingForward = Vector3.Dot(transform.forward, inputDirection) > 0.3f;

        if (freeLookCam != null)
        {
            if (UI_Manager.Instance.IsPlayerInSecondZone)
            {
                freeLookCam.m_XAxis.m_MaxSpeed = slowedRotationSpeed;
            }
            else
            {
                freeLookCam.m_XAxis.m_MaxSpeed = normalRotationSpeed;
            }
        }

        bool isGunActive = UI_Manager.Instance.WeaponAttackEvent.isGunActive;
        float joystickMagnitude = new Vector2(joystick.Horizontal, joystick.Vertical).magnitude;

        // Modified running logic to account for backward movement
        bool isRunning = isAndroid ? joystickMagnitude >= 0.9f && !UI_Manager.Instance.IsPlayerInSecondZone
            : Input.GetKey(KeyCode.LeftShift) && !UI_Manager.Instance.IsPlayerInSecondZone;

        if (animator != null)
        {
            if (isGunActive)
            {
                if (isRunning)
                {
                    animator.SetLayerWeight(2, 1f);
                    animator.SetLayerWeight(1, 0f);
                    animator.SetLayerWeight(0, 0f);
                    UI_Manager.Instance.WeaponAttackEvent.leftHandPos.weight = 0f;
                    UI_Manager.Instance.WeaponAttackEvent.aim.weight = 0f;
                }
                else
                {
                    animator.SetLayerWeight(1, 1f);
                    animator.SetLayerWeight(2, 0f);
                    animator.SetLayerWeight(0, 1f);
                    UI_Manager.Instance.WeaponAttackEvent.leftHandPos.weight = 1f;
                    UI_Manager.Instance.WeaponAttackEvent.aim.weight = 0.4f;
                }
            }
            else
            {
                animator.SetLayerWeight(0, 1f);
                animator.SetLayerWeight(1, 0f);
                animator.SetLayerWeight(2, 0f);
            }
        }

        // Adjust movement speed based on direction and running state
        float targetSpeed;
        if (inputDirection == Vector3.zero)
        {
            targetSpeed = 0f;
        }
        else if (isRunning)
        {
            targetSpeed = isMovingBackward ? runSpeed * speedMultiplier * 0.8f : runSpeed * speedMultiplier;
        }
        else
        {
            targetSpeed = isMovingBackward ? walkSpeed * speedMultiplier * 0.7f : walkSpeed * speedMultiplier;
        }

        // Smooth speed adjustment
        float currentSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
        _speed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * SpeedChangeRate);

        // Move the player in the correct direction
        Vector3 moveDirection = inputDirection * _speed;
        _controller.Move(moveDirection * Time.deltaTime);

        // Rotate player to face movement direction (only if moving forward or strafing)
        if (inputDirection.magnitude > 0f && !isMovingBackward)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, turnSpeed * Time.deltaTime);
        }

        // Update animations with directional information
        if (animator != null)
        {
            // Calculate animation speed parameter
            float animationSpeed;
            if (isMovingBackward)
            {
                // Backward movement (negative values)
                animationSpeed = isRunning ? -6f : -3f;
                // Apply lerp to smooth transitions
                animationSpeed = Mathf.Lerp(animator.GetFloat("Speed"), animationSpeed, Time.deltaTime * SpeedChangeRate);
            }
            else if (isMovingForward)
            {
                // Forward movement (positive values)
                animationSpeed = isRunning ? 6f : 3f;
                // Apply lerp to smooth transitions
                animationSpeed = Mathf.Lerp(animator.GetFloat("Speed"), animationSpeed, Time.deltaTime * SpeedChangeRate);
            }
            else
            {
                // Strafing or stopping
                animationSpeed = Mathf.Lerp(animator.GetFloat("Speed"), 0f, Time.deltaTime * SpeedChangeRate);
            }

            animator.SetFloat("Speed", animationSpeed);
            animator.SetFloat("MotionSpeed", inputDirection.magnitude);
        }

        UpdateDogBehavior(inputDirection.magnitude > 0, isRunning, UI_Manager.Instance.IsPlayerInSecondZone);
    }
    internal Vector3 GetPlayerMovementDirection()
    {
        float moveHorizontal;
        float moveVertical;

        if (isAndroid && joystick != null)
        {
            // Use Joystick for Android
            moveHorizontal = joystick.Horizontal;
            moveVertical = joystick.Vertical;
        }
        else
        {
            // Use Keyboard for PC/WebGL
            moveHorizontal = Input.GetAxis("Horizontal");
            moveVertical = Input.GetAxis("Vertical");
        }

        Vector3 cameraForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 cameraRight = Vector3.Scale(cam.right, new Vector3(1, 0, 1)).normalized;

        Vector3 inputDirection = (cameraForward * moveVertical + cameraRight * moveHorizontal).normalized;
        return inputDirection;
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

    /*private void UpdateDogBehavior(bool isCharacterMoving, bool isCharacterRunning, bool isCameraLocked)
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

    private void UpdateDogBehavior(bool isCharacterMoving, bool isCharacterRunning, bool isCameraLocked)
    {
        if (isCameraLocked)
        {
            SetDogAnimation("Idle");
            return;
        }

        // Detect movement direction (using input for immediate response)
        Vector3 charInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        bool isMovingBackward = Vector3.Dot(transform.forward, charInput) < -0.3f;

        // Simplified animation states - only Run/Idle
        string dogStateParam = isCharacterMoving ? "Run" : "Idle";
        SetDogAnimation(dogStateParam);

        if (isCharacterMoving)
        {
            // Base follow parameters
            float baseFollowSpeed = followRunSpeed;
            Vector3 targetPosition = transform.position - transform.forward * followDistance;

            // Enhanced backward movement handling
            if (isMovingBackward)
            {
                // Increase speed multiplier based on character speed
                float speedMultiplier = isCharacterRunning ? 3f : 2f;
                baseFollowSpeed *= speedMultiplier;

                // Add dynamic horizontal offset to prevent merging
                float horizontalOffset = Mathf.PingPong(Time.time * 0.5f, 0.4f) - 0.2f;
                targetPosition += transform.right * horizontalOffset;
            }

            // Apply position with vertical offset
            targetPosition += Vector3.up * 0.5f;
            _dog.transform.position = Vector3.Lerp(
                _dog.transform.position,
                targetPosition,
                Time.deltaTime * baseFollowSpeed);

            // Make dog face same direction as character
            _dog.transform.rotation = Quaternion.Slerp(
                _dog.transform.rotation,
                transform.rotation,
                Time.deltaTime * followTurnSpeed);

            // Emergency distance check
            if (Vector3.Distance(_dog.transform.position, transform.position) < followDistance * 0.7f)
            {
                _dog.transform.position = transform.position - transform.forward * followDistance * 0.8f;
            }
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


        _dogAnimator.SetInteger(name, value);

    }

    public void ThrowSeeds()
    {
        List<GameObject>seedtoDestroy = new List<GameObject>();
        for (int i = 0; i < seedsPerThrow; i++)
        {
            GameObject seed = Instantiate(UI_Manager.Instance.seed, throwPoint.position, Quaternion.identity);
            seed.transform.localScale = Vector3.one * 0.04f;
            Rigidbody rb = seed.GetComponent<Rigidbody>();

            if (rb != null)
            {
                // Apply force with slight variation
                Vector3 throwDirection = transform.forward + new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f), 0, UnityEngine.Random.Range(-0.1f, 0.1f));
                rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
            }
            seedtoDestroy.Add(seed);
        }

       for (int i = 0; i < seedtoDestroy.Count; i++)
        {
            Destroy(seedtoDestroy[i]);
        }

    }

}







