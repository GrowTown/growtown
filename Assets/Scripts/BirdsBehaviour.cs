using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdsBehaviour : MonoBehaviour
{
    Animator animator;
    void Start()
    {
        animator=GetComponent<Animator>();
        StartCoroutine(BehaviorCycle());
    }

    private IEnumerator BehaviorCycle()
    {
        while (true)
        {
            // idle animation
            SetInt("animation,0");
            // Walk for 4 seconds
            yield return new WaitForSeconds(4f);

            // Eating animation
            SetInt("animation,4"); 
           
            // Eat for 5 seconds
            yield return new WaitForSeconds(5f);

            // Resting animation
            SetInt("animation,5"); 
            yield return new WaitForSeconds(2f);
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
