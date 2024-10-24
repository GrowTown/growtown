using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovements : MonoBehaviour
{

    public float speed = 5f;            
    public float turnSpeed = 10f;
    public Animator animator;

    CharacterController _controller; 
    Vector3 _moveDirection;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
    }
    void Update()
    {
        CharMovements();
    }

    void CharMovements()
    {
        float moveHorizontal = Input.GetAxis("Horizontal"); 
        float moveVertical = Input.GetAxis("Vertical"); 
        _moveDirection = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized;

        // Move the character using CharacterController
        _controller.Move(_moveDirection * speed * Time.deltaTime);

        // Check if the character is moving
        bool isMoving = _moveDirection.magnitude > 0f;

        // Set the correct animation based on movement
        if (isMoving)
        {
            animator.SetBool("IsRunning", true);  // Play run animation
        }
        else
        {
            animator.SetBool("IsRunning", false); // Play idle animation
        }


        // Rotate the character towards the movement direction
        if (isMoving)
        {
            Quaternion toRotation = Quaternion.LookRotation(_moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, turnSpeed * Time.deltaTime);
        }
    }
}


