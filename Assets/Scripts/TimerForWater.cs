using System;
using UnityEngine;
using UnityEngine.Events;

public class TimerForWater : MonoBehaviour
{
    //public string Name { get; private set; }
    private DateTime _startTime;
    private DateTime _finishTime;
    public TimeSpan timeToFinish { get; private set; }
    private bool _isRunning;

    public double secondsLeft { get; private set; }


    public void Initialize( DateTime start, TimeSpan time)
    {
       // Name = processName;
        _startTime = start;
        timeToFinish = time;
        _finishTime = start.Add(time);
       // TimerFinishedEvent = new UnityEvent();
    }
    public void StartTimer()
    {
        secondsLeft = timeToFinish.TotalSeconds;
        _isRunning = true;
    }



    // Update is called once per frame
    void Update()
    {
        if (_isRunning)
        {
            if (secondsLeft > 0)
            {
                secondsLeft -= Time.deltaTime;

            }
            else
            {
                // TimerFinishedEvent.Invoke();
                secondsLeft = 0;
                _isRunning = false;
            }
        }
    }

}
