using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class TimerToolTip : MonoBehaviour
{
    public static TimerToolTip toolTipInstance;

    private Timer _timer;
    [SerializeField]
    private TextMeshProUGUI _nameText;
    [SerializeField]
    private TextMeshProUGUI _timeLeftText;
    [SerializeField] 
    private Slider _progressbar;
    private bool countDown;

    // Start is called before the first frame update
    void Start()
    {
        toolTipInstance=this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void ShowTimer(GameObject go)
    {
        _timer=go.GetComponent<Timer>();

        if(_timer!=null)
        {
            return;
        }

        _nameText.text = _timer.Name;
        countDown = true;
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
