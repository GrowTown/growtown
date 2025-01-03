using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{

 

    public string Name {  get; private set; }
    private DateTime _startTime;
    private DateTime _finishTime;
    public TimeSpan timeToFinish {  get; private set; }
    [HideInInspector]public UnityEvent TimerFinishedEvent;
    private bool _isRunning;

    public double secondsLeft {  get; private set; }


    public void Initialize(string processName,DateTime start,TimeSpan time)
    {
        Name = processName;
        _startTime = start;
        timeToFinish = time;
        _finishTime = start.Add(time);
        TimerFinishedEvent = new UnityEvent();
    }
    public void StartTimer()
    {
        secondsLeft=timeToFinish.TotalSeconds;
        _isRunning=true;
    }

    public void StopTimer()
    {
        _isRunning=false;
        secondsLeft = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(_isRunning)
        {
            if(secondsLeft > 0)
            {
                secondsLeft-= Time.deltaTime;
                
            }
            else
            {
                TimerFinishedEvent.Invoke();
                secondsLeft=0;
                _isRunning = false;
            }
        }
    }


    /// <summary>
    /// For Display Time in game
    /// </summary>
    /// <returns></returns>
    public string DisplayTime()
    {
        string text = "";
        TimeSpan timeLeft=TimeSpan.FromSeconds(secondsLeft);
        if(timeLeft.Hours!=0)
        {
            text += timeLeft.Hours + "h";
            text += timeLeft.Minutes + "min";
        }
        else if(timeLeft.Minutes!=0)
        {
            text += timeLeft.Minutes + "min";
            text += timeLeft.Seconds + "sec";
        }
        else if(secondsLeft > 0)
        {
            text += Mathf.FloorToInt((float)secondsLeft) + "sec";
        }
        else
        {
            text = "Finished";
        }
    return text;
    }
}
