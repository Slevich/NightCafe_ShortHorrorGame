using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIImageAnimation : MonoBehaviour
{
    [Header("Image to animate"), SerializeField] private Image _imageToAnimate;
    [Header("Target local scale to lerp."), SerializeField] private Vector3 _targetScale = Vector3.one;
    [Header("Colors to lerp."), SerializeField] private Color[] _colors;
    [Header("Animation duration."), SerializeField, Range(0f, 5f)] private float _duration = 0.5f;

    private Tween _colorTween;
    private Tween _scaleTween;
    private Color _startColor = Color.white;
    private Vector3 _startScale = Vector3.one;
    #region Fields
    #endregion

    #region Methods
    private void Awake ()
    {
        if (_imageToAnimate == null)
            return;

        _startColor = _imageToAnimate.color;
        _startScale = _imageToAnimate.rectTransform.localScale;
    }

    public void LerpToColorByIndex(int Index)
    {
        if (_imageToAnimate == null)
            return;

        if (_colors == null || _colors.Length == 0)
            return;

        if (Index < 0 || Index >= _colors.Length)
            return;

        Color targetColor = _colors[Index];
        ColorAnimation(targetColor);
    }

    public void LerpToStartColor()
    {
        if (_imageToAnimate == null)
            return;

        if (_colors == null || _colors.Length == 0)
            return;

        ColorAnimation(_startColor);
    }


    private void ColorAnimation(Color targetColor)
    {
        KillTween(_colorTween);

        _colorTween = _imageToAnimate.DOColor(targetColor, _duration);
        _colorTween.Play();
    }

    private void ScaleAnimation(Vector3 scale)
    {
        KillTween(_scaleTween);

        _scaleTween = _imageToAnimate.rectTransform.DOScale(scale, _duration);
        _scaleTween.Play();
    }

    public void ScaleToStart()
    {
        ScaleAnimation(_startScale);
    }

    public void ScaleToTarget()
    {
        ScaleAnimation(_targetScale);
    }

    private void KillTween(Tween tweenToKill)
    {
        if (tweenToKill != null && tweenToKill.active)
            tweenToKill.Kill();
    }

    public void StopAnimations()
    {
        KillTween(_colorTween);
        KillTween(_scaleTween);
    }
    #endregion
}
