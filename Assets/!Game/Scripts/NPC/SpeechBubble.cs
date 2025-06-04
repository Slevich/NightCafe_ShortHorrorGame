using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpeechBubble : MonoBehaviour
{
    #region Fields
    [Header("Scale animation of the speech bubble."), SerializeField] private ScaleAnimation _scale;
    [Header("Speech bubble text."), SerializeField] private TextMeshPro _bubbleText;
    [Header("Look target."), SerializeField] private Transform _lookTarget;

    private ActionInterval _interval;
    private float _intervalTimeStep = 0.01f;
    private ActionTimer _timer;
    #endregion

    #region Methods
    private void Awake ()
    {
        _timer = new ActionTimer();
        _interval = new ActionInterval();
    }

    public void ChangeText(string NewText)
    {
        if (_bubbleText != null)
        {
            _bubbleText.text = NewText;
        }
    }

    public void Show()
    {
        if (_scale == null)
            return;

        _scale.ScaleToEnd();

        if(_lookTarget != null && _interval != null && !_interval.Busy)
        {
            Action onInverval = delegate
            {
                Vector3 direction = (_lookTarget.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * 10f);
            };

            _interval.StartInterval(_intervalTimeStep, onInverval);
        }
    }

    public void Hide()
    {
        if (_scale == null)
            return;

        _scale.ScaleToStart();

        if (_interval != null && _interval.Busy)
            _interval.Stop();
    }

    public void ShowAndHideWithDelay(float TimeToHide)
    {
        if (_timer != null && _timer.Busy)
            return;

        Show();

        Action timerAction = delegate
        {
            Hide();
        };

        _timer.StartTimerAndAction(TimeToHide, timerAction);
    }

    private void OnDisable ()
    {
        if(_timer != null && _timer.Busy)
            _timer.StopTimer();

        if (_interval != null && _interval.Busy)
            _interval.Stop();
    }
    #endregion
}
