using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActionIntervalInstance : MonoBehaviour
{
    #region Fields
    [Header("Interval time step."), SerializeField, Range(0f, 10f)] private float _intervalTimeStep = 1f;
    [Header("Interval event."), SerializeField] private UnityEvent _onInterval;
    private ActionInterval _interval;
    #endregion

    #region Methods
    private void Awake ()
    {
        _interval = new ActionInterval();
    }

    public void StartInterval ()
    {
        if (_interval != null && _interval.Busy)
            return;

        Action onInterval = delegate { _onInterval?.Invoke(); };
        _interval.StartInterval(_intervalTimeStep, onInterval);
    }

    public void StopInterval ()
    {
        if (_interval != null && _interval.Busy)
            _interval.Stop();
    }

    private void OnDisable () => StartInterval();
    #endregion
}
