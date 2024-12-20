using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterMovements : MonoBehaviour
{
    public Transform cam;
    [SerializeField]
    CinemachineFreeLook virtualCam;
    public Animator animator;
    private CharacterController _controller;
    private Vector3 _moveDirection;
    public Transform lockedPositionField; 
    public Transform lockedPositionMarket; 

    internal Quaternion lockedRotation; 
    internal bool isCameraLocked = false; 

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

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _animIDGrounded=Animator.StringToHash("Grounded");
    }

    void Update()
    {
        Shader.SetGlobalVector("_Player", transform.position);
        CharMovements();
        HandleJump();
        if(!isJumping)
        AdjustHeightofPlayer();
        //UpdateVirtualCamera();
    }

    void AdjustHeightofPlayer()
    {
        var height = new Vector3(transform.position.x, _charGroundPos, transform.position.z);
        transform.position = height;
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
        if (_animationBlend < 0.01f) _animationBlend = 0f; // Ensure idle is detected

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
                isJumping= true;
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
        animator.SetBool("IsJumping",false);
        _canJump = true;
        isJumping=false;
    }
     

    internal void CameraLock(string area)
    {
        if (area == "Field")
        {
            virtualCam.transform.position  = lockedPositionField.position;
            virtualCam.transform.rotation = lockedRotation;
            virtualCam.Follow = null;
           // virtualCam.LookAt = null;
        }
        if(area== "Market")
        {
            virtualCam.transform.position = lockedPositionMarket.position;
            virtualCam.transform.rotation = lockedRotation;
            virtualCam.Follow = null;
        }
    
        isCameraLocked = true;

    }
    internal void CameraUnlock()
    {       
        isCameraLocked = false;    
        virtualCam.Follow = this.transform;
        virtualCam.LookAt = this.transform;
    }

    void UpdateVirtualCamera()
    {
        if (isCameraLocked && virtualCam!= null)
        {
           // virtualCam.transform.position = lockedPosition;
            virtualCam.transform.rotation = lockedRotation;
        }
    }

   
}







