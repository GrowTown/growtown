using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterMovements : MonoBehaviour
{

    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float turnSpeed = 7f;
    public float turnSpeedVelocity = 0.1f;
    public float _charGroundPos = 3f;
    public Transform cam;

    public Animator animator;

    private CharacterController _controller;
    private Vector3 _moveDirection;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Shader.SetGlobalVector("_Player", transform.position);
        CharMovements();
        AdjustHeightofPlayer();
    }

    void AdjustHeightofPlayer()
    {
        var height = new Vector3(transform.position.x, _charGroundPos, transform.position.z);
        transform.position = height;
    }

    /*    void CharMovements()
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            _moveDirection = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized;

            // Check if the Shift key is pressed for running
            bool isRunning = Input.GetKey(KeyCode.LeftShift);

            // Set speed based on running or walking
            float currentSpeed = isRunning ? runSpeed : walkSpeed;

            // Move the character using CharacterController

            // Set the correct animation based on movement and speed
            if (_moveDirection.magnitude > 0f)
            {
                if (isRunning)
                {
                    animator.SetBool("IsWalking", false);   // Stop walk animation
                    animator.SetBool("IsRunning", true);    // Play run animation
                }
                else
                {
                    animator.SetBool("IsRunning", false);   // Stop run animation
                    animator.SetBool("IsWalking", true);    // Play walk animation
                }
            }
            else
            {
                animator.SetBool("IsRunning", false);
                animator.SetBool("IsWalking", false);
            }

            // Rotate the character towards the movement direction
            if (_moveDirection.magnitude > 0f)
            {
                float targetAngle = Mathf.Atan2(_moveDirection.x, _moveDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSpeedVelocity, turnSpeed);
                Quaternion toRotation = Quaternion.LookRotation(_moveDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, turnSpeed * Time.deltaTime);
                Vector3 Direction=Quaternion.Euler(0f,targetAngle,0f)*Vector3.forward;
            _controller.Move(Direction * currentSpeed * Time.deltaTime);
            }

        }*/
    private void CharMovements()
    {
        // Get movement input
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Use the camera's forward and right to calculate movement relative to its orientation
        Vector3 cameraForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized; // Flatten the forward vector
        Vector3 cameraRight = Vector3.Scale(cam.right, new Vector3(1, 0, 1)).normalized;    // Flatten the right vector

        // Calculate movement direction relative to the camera
        _moveDirection = (cameraForward * moveVertical + cameraRight * moveHorizontal).normalized;

        // Check if the Shift key is pressed for running
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        // Set speed based on running or walking
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        // Move the character using CharacterController
        _controller.Move(_moveDirection * currentSpeed * Time.deltaTime);

        // Set the correct animation based on movement and speed
        if (_moveDirection.magnitude > 0f)
        {
            if (isRunning)
            {
                animator.SetBool("IsWalking", false);
                animator.SetBool("IsRunning", true);
            }
            else
            {
                animator.SetBool("IsRunning", false);
                animator.SetBool("IsWalking", true);
            }
        }
        else
        {
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsWalking", false);
        }

        // Rotate the character towards the movement direction
        if (_moveDirection.magnitude > 0f)
        {
            Quaternion toRotation = Quaternion.LookRotation(_moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, turnSpeed * Time.deltaTime);
        }
    }





    /*private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }*/
}







