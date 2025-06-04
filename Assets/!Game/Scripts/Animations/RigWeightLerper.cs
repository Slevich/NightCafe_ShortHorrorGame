using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigWeightLerper : MonoBehaviour
{
    #region Fields
    [Header("Rigs to lerp their weights."), SerializeField] private Rig[] _rigs;
    [Header("Total time to rig."), SerializeField, Range(0f, 10f)] private float _totalTime;

    private ActionInterval _lerpInterval;
    private float _intervalTimeStep = 0.01f;
    #endregion

    #region Methods
    private void Awake ()
    {
        _lerpInterval = new ActionInterval();
    }

    public void SetWeights(float TargetValue)
    {
        if (_rigs == null || _rigs.Length == 0)
            return;

        foreach (var rig in _rigs)
        {
            rig.weight = TargetValue;
        }
    }

    public void LerpWeightsTo(float TargetValue)
    {
        if(_rigs == null || _rigs.Length == 0)
            return;

        if(_lerpInterval != null && _lerpInterval.Busy)
            _lerpInterval.Stop();

        TargetValue = Mathf.Clamp01(TargetValue);
        int totalCount = _rigs.Length;
        float valuePerInterval = MathF.Round(1 / (_totalTime / _intervalTimeStep), 2);
        float time = 0f;

        Action lerpAction = delegate
        {
            int count = 0;

            foreach (var rig in _rigs)
            {
                float currentWeight = rig.weight;

                if (TargetValue > currentWeight)
                {
                    currentWeight += valuePerInterval;

                    if(currentWeight >= TargetValue)
                        currentWeight = TargetValue;
                }
                else if(TargetValue < currentWeight)
                {
                    currentWeight -= valuePerInterval;
                    
                    if(currentWeight <= TargetValue)
                        currentWeight = TargetValue;
                }

                if (currentWeight == TargetValue)
                {
                    count++;
                }

                rig.weight = currentWeight;
            }

            time += _intervalTimeStep;

            if(count == totalCount)
            {
                _lerpInterval.Stop();
            }    
        };

        _lerpInterval.StartInterval(_intervalTimeStep, lerpAction);
    }

    private void StopLerping()
    {
        if(_lerpInterval != null && _lerpInterval.Busy)
            _lerpInterval.Stop();
    }

    public void OnDisable () => StopLerping();
    #endregion
}
