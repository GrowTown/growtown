using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerToolTip : MonoBehaviour
{
    public static TimerToolTip toolTipInstance;

    private Timer _timer;
    [SerializeField]
    private TextMeshProUGUI _nameText;
    [SerializeField]
    private Slider _progressbar;
    public  TextMeshProUGUI _timeLeftText;
    private bool countDown;
        

    private void Awake()
    {
        toolTipInstance = this;
        transform.parent.gameObject.SetActive(false);
    }

  
    void ShowTimer(GameObject go)
     {
        _timer=go.GetComponent<Timer>();

        if(_timer==null)
        {
            return;
        }

        _nameText.text = _timer.Name;
        countDown = true;
        transform.parent.gameObject.SetActive(true);
    }

    private void FixedUpdate()
    {
        if(countDown) 
        {
            _progressbar.value = (float)(1.0 - _timer.secondsLeft / _timer.timeToFinish.TotalSeconds);
            _timeLeftText.text = _timer.DisplayTime();
        }

    }

    public void HideTimer()
    {
        transform.parent.gameObject.SetActive(false);
        _timer=null;
        countDown = false;  
    }

    public static void ShowTimerStatic(GameObject go)
    {
        toolTipInstance.ShowTimer(go);

    }

    public static void HideTimerStatic()
    {
        toolTipInstance.HideTimer();
    }
}
