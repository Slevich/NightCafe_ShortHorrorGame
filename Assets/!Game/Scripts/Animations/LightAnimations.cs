using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LightAnimations : MonoBehaviour
{
    #region Fields
    [Header("Light source."), SerializeField] private Light _light;
    [Header("Base animation duration."), SerializeField] private float _baseDuration = 0.5f;

    private static readonly float _maxRandomIntensity = 10f;
    private static readonly float _minRandomIntensity = 0f;
    private float _startIntensity = 0f;
    private Tween _lightTween;
    #endregion

    #region Properties
    public bool InProgress => _lightTween != null && _lightTween.IsPlaying();
    #endregion

    #region Methods
    private void OnValidate ()
    {
        if(_light == null && TryGetComponent<Light>(out Light light))
            _light = light;
    }

    private void Awake ()
    {
        if(_light != null)
            _startIntensity = _light.intensity;
    }
    
    public void LerpIntensityToValue(float TargetIntensity)
    {
        AnimateIntensity(TargetIntensity, _baseDuration);
    }

    public void LerpIntensityToRandom()
    {
        float targetIntensity = Random.Range(_minRandomIntensity, _maxRandomIntensity);
        AnimateIntensity(targetIntensity, _baseDuration);
    }

    private void AnimateIntensity(float targetIntensity, float duration)
    {
        if (_light == null)
            return;

        if (InProgress)
            return;

        _lightTween = _light.DOIntensity(targetIntensity, duration);
        _lightTween.Play();
    }

    private void OnDestroy ()
    {
        if(InProgress)
            _lightTween.Kill();
    }
    #endregion
}
