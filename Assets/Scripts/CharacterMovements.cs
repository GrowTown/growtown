using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovements : MonoBehaviour
{

    public float speed = 5f;            
    public float turnSpeed = 10f;
    public Animator animator;
     CharacterController _controller; 

    private Vector3 moveDirection;

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
        float moveHorizontal = Input.GetAxis("Horizontal"); // A & D (Left & Right)
        float moveVertical = Input.GetAxis("Vertical");     // W & S (Forward & Back)

        moveDirection = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized;

        // Move the character using CharacterController
        _controller.Move(moveDirection * speed * Time.deltaTime);

        // Check if the character is moving
        bool isMoving = moveDirection.magnitude > 0f;

        // Set the correct animation based on movement
        if (/*moveHorizontal>0|| moveVertical>0*/isMoving)
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
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, turnSpeed * Time.deltaTime);
        }
    }
}


