using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameStepsManager : MonoBehaviour
{
    #region Fields
    [Header("Current active step number."), SerializeField, ReadOnly] private int _pointer = 0;
    [Header("Game steps."), SerializeField] private GameStep[] _steps;
    [field: Space(5), Header("Event on all steps completed."), SerializeField] private UnityEvent OnAllStepsCompleted { get; set; }

    private GameStep _currentStep;
    private bool _stepsInProgress = false;
    #endregion

    #region Methods
    public void StartSteps()
    {
        if (_steps == null || _steps.Length == 0)
            return;

        if (_stepsInProgress)
            return;

        _stepsInProgress = true;
        GameStep startStep = _steps[_pointer];
        StartStep(startStep);
    }

    private void StartStep(GameStep startedStep)
    {
        if(startedStep != null)
        {
            startedStep.StartStep();
            _currentStep = startedStep;
        }
    }

    public void CompleteCurrentStep()
    {
        if (!_stepsInProgress)
            return;

        if (_currentStep == null)
            return;
        
        _currentStep.CorrectlyComplete();
        _pointer++;

        if (_pointer >= _steps.Length)
        {
            _pointer = _steps.Length - 1;
            OnAllStepsCompleted?.Invoke();
            _stepsInProgress = false;
            return;
        }

        StartStep(_steps[_pointer]);
    }

    public void WrongCompleteCurrentStep()
    {
        if (!_stepsInProgress)
            return;

        if (_currentStep == null)
            return;

        _currentStep.WrongComplete();
    }
    #endregion
}

[Serializable]
public class GameStep
{
    [Header("Step description"), SerializeField] private string _stepName = "иру";
    [field: Header("Step is currently active?"), SerializeField, ReadOnly] public bool IsActive { get; set; } = false;
    [field: Header("Step is currently finished?"), SerializeField, ReadOnly] public bool IsFinished { get; set; } = false;
    [field: Space(10)]
    [field: Space(5), Header("Event on step start."), SerializeField] private UnityEvent OnStepStart { get; set; }
    [field: Space(5), Header("Event on step correctly complete."), SerializeField] private UnityEvent OnStepCorrectComplete { get; set; }
    [field: Space(5), Header("Event on step tryign to wrong complete."), SerializeField] private UnityEvent OnStepWrongComplete { get; set; }

    public void StartStep()
    {
        if (IsActive)
            return;

        OnStepStart?.Invoke();
        IsActive = true;
    }

    public void CorrectlyComplete()
    {
        if(!IsActive)
            return;

        if (IsFinished)
            return;

        OnStepCorrectComplete?.Invoke();
        IsFinished = true;
    }

    public void WrongComplete ()
    {
        if (!IsActive)
            return;

        if (IsFinished)
            return;

        OnStepWrongComplete.Invoke();
    }
}
