using System;
using System.Collections;
using UnityEngine;

public class AnimalsBehaviour : MonoBehaviour
{

    public Transform startPoint; // The starting point
    public Transform endPoint;   // The ending point
    public float speed = 2f;     // Movement speed

    private Animator animator;   // Reference to the Animator
    private Transform targetPoint; // Current target point
    private bool isEating = false; // To track eating state

    void Start()
    {
        // Assign the Animator component
        animator = GetComponent<Animator>();

        // Set the initial target point to the end point
        targetPoint = endPoint;

        // Start the behavior cycle
        StartCoroutine(BehaviorCycle());
    }

    void Update()
    {
        if (!isEating)
        {
            // Move towards the target point
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, step);

            // Rotate to face the target point
            Vector3 direction = (targetPoint.position - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speed);
            }

            // Check if the cow has reached the target point
            if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
            {
                // Swap target points
                targetPoint = targetPoint == startPoint ? endPoint : startPoint;
            }
        }
    }

    private IEnumerator BehaviorCycle()
    {
        while (true)
        {
            // Walking animation
            SetInt("animation,1"); 

            // Walk for 4 seconds
            yield return new WaitForSeconds(4f);

            // Eating animation
            SetInt("animation,4"); 
            isEating = true;

            // Eat for 5 seconds
            yield return new WaitForSeconds(5f);

            // Resting animation
            SetInt("animation,5"); 
            yield return new WaitForSeconds(2f); 

            isEating = false;
        }
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


