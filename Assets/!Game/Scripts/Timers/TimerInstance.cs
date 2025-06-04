using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimerInstance : MonoBehaviour
{
    #region Fields
    [Header("Event on timer ends."), SerializeField] private UnityEvent _onTimerEnds;
    [Header("Timer duration in seconds."), SerializeField, Range(0f, 10f)] private float _duration = 1f;

    private ActionTimer _timer;
    #endregion

    #region Methods
    private void Awake ()
    {
        _timer = new ActionTimer();
    }

    public void StartTimer()
    {
        if (_timer != null && _timer.Busy)
            return;

        Action onTimerEndsCallback = delegate
        {
            _onTimerEnds?.Invoke();
        };

        _timer.StartTimerAndAction(_duration, onTimerEndsCallback);
    }

    public void StopTimer()
    {
        if (_timer != null && _timer.Busy)
            _timer.StopTimer();
    }

    private void OnDisable () => StopTimer();
    #endregion
}
