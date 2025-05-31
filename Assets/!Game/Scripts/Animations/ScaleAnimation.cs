using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ScaleAnimation : MonoBehaviour
{
    #region Fields
    [Header("Transform to scale"), SerializeField] private Transform _scaledTransform;

    [Header("End scale value."), SerializeField] private Vector3 _localScaleEndValue;
    [Header("Time to scale to end."), SerializeField, Range(0f, 5f)] private float _endScaleTime = 1f;
    [Header("End scale ease."), SerializeField] private Ease _endScaleEase = Ease.Linear;

    [Header("Start scale value."), SerializeField] private Vector3 _localScaleStartValue;
    [Header("Time to scale to start."), SerializeField, Range(0f, 5f)] private float _startScaleTime = 1f;
    [Header("start scale ease."), SerializeField] private Ease _startScaleEase = Ease.Linear;

    private Tween _currentTween;
    #endregion

    #region Methods
    private void Awake ()
    {
        if (_scaledTransform == null)
            return;

        _scaledTransform.localScale = _localScaleStartValue;
    }

    private void Scale(Vector3 targetScale, float duration, Ease ease)
    {
        AbortScaling();

        _currentTween = _scaledTransform.DOScale(targetScale, duration).SetEase(ease);
        _currentTween.Play();
    }

    public void ScaleToEnd () => Scale(_localScaleEndValue, _endScaleTime, _endScaleEase);

    public void ScaleToStart () => Scale(_localScaleStartValue, _startScaleTime, _startScaleEase);

    private void AbortScaling()
    {
        if (_currentTween != null && _currentTween.IsPlaying())
        {
            _currentTween.Kill();
            _currentTween = null;
        }
    }

    private void OnDisable () => AbortScaling();
    #endregion
}
