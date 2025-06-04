using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class MovementByInput : MonoBehaviour
{
    #region Fields
    [Header("Base speed."), SerializeField, Range(0.1f, 10f)] private float _walkSpeed = 1f;
    [Header("Acceleration (jog) speed."), SerializeField, Range(0.1f, 10f)] private float _jogSpeed = 1.5f;
    [Header("Time to reach idle speed (0) - stop (in seconds)."), SerializeField, Range(0f, 1f)] private float _timeToStop = 0.3f;
    [Header("Time to reach walk speed (in seconds)."), SerializeField, Range(0f, 1f)] private float _timeToReachWalkSpeed = 0.4f;
    [Header("Time to reach jog speed (in seconds)."), SerializeField, Range(0f, 1f)] private float _timeToReachJogSpeed = 0.6f;
    [Header("Event on current speed percentage change."), SerializeField] private UnityEvent<float> _onSpeedChangePercentage;
    [Header("Event on crouch."), SerializeField] private UnityEvent _onCrouch;
    [Header("Event on stand after crouch."), SerializeField] private UnityEvent _onStand;

    private float _currentSpeed = 0f;
    private float _targetSpeed = 0f;
    private bool _isAccelerate = false;
    private bool _isCrouch = false;
    private CharacterInputHandler _inputHandler;
    private ActionInterval _speedChangeInterval;
    private float _speedChangeTimeStep = 0.01f;
    private Vector2 _previousInputValue = Vector2.zero;
    private SpeedStates _currentSpeedState = SpeedStates.Idle;
    private StandStates _currentStandState = StandStates.Stand;
    #endregion

    #region Properties
    #endregion

    #region Methods
    [Inject]
    public void Construct (CharacterInputHandler InputHandler)
    {
        _inputHandler = InputHandler;
        _targetSpeed = 0f;
        _onSpeedChangePercentage?.Invoke(_targetSpeed);
        _speedChangeInterval = new ActionInterval();
        _currentSpeedState = SpeedStates.Idle;
    }

    private void OnEnable () => Subscribe();

    private void Subscribe()
    {
        if (_inputHandler != null)
        {
            _inputHandler.InputDirectionCallback += (direction) => ManageMovement(direction);
            _inputHandler.AccelerationButtonPressedCallback += delegate { Acceleration(true); Debug.Log("Ускорение!"); } ;
            _inputHandler.AccelerationButtonReleasedCallback += delegate { Acceleration(false); };
            _inputHandler.CrouchButtonPressedCallback += delegate { Crouching(true); };
            _inputHandler.CrouchButtonReleasedCallback += delegate { Crouching(false); };
        }
    }

    private void OnDisable () => Dispose();

    private void Dispose()
    {
        if (_inputHandler != null)
        {
            _inputHandler.InputDirectionCallback -= (direction) => ManageMovement(direction);
            _inputHandler.AccelerationButtonPressedCallback -= delegate { Acceleration(true); };
            _inputHandler.AccelerationButtonReleasedCallback -= delegate { Acceleration(false); };
            _inputHandler.CrouchButtonPressedCallback -= delegate { Crouching(true); };
            _inputHandler.CrouchButtonReleasedCallback -= delegate { Crouching(false); };
        }

        if(_speedChangeInterval != null && _speedChangeInterval.Busy)
            _speedChangeInterval.Stop();
    }

    private void ManageMovement(Vector2 InputDirection)
    {
        SpeedStates speedState = DefineSpeedStateByInput(InputDirection);

        if(speedState != _currentSpeedState)
        {
            _currentSpeedState = speedState;
            ManageSpeed(_currentSpeedState);
        }

        StandStates standState = DefineStandStateByInput();

        if(standState != _currentStandState)
        {
            _currentStandState = standState;
            ManageStand(_currentStandState);
        }

        if(_currentSpeedState != SpeedStates.Idle)
            _previousInputValue = InputDirection;

        if(_currentSpeed == 0 && _currentSpeedState == SpeedStates.Idle)
            return;

        MoveByInputDirection(_previousInputValue);
    }

    private SpeedStates DefineSpeedStateByInput(Vector2 InputDirection)
    {
        SpeedStates state = SpeedStates.Idle;

        if (InputDirection == Vector2.zero && _currentSpeedState != SpeedStates.Idle)
            state = SpeedStates.Idle;
        else if (InputDirection != Vector2.zero)
        {
            if (!_isAccelerate)
                state = SpeedStates.Walk;
            else if (_isAccelerate)
                state = SpeedStates.Jog;
        }
        
        return state;
    }

    private StandStates DefineStandStateByInput()
    {
        StandStates state = _isCrouch ? StandStates.Crouch : StandStates.Stand;
        return state;
    }

    private void ManageSpeed(SpeedStates speedState)
    {
        float targetSpeed = 0f;
        float reachTime = 0f;

        switch(speedState)
        {
            case SpeedStates.Idle:
                targetSpeed = 0;
                reachTime = _timeToStop;
                break;

            case SpeedStates.Walk:
                targetSpeed = _walkSpeed;
                reachTime = _timeToReachWalkSpeed;
                break;

            case SpeedStates.Jog:
                targetSpeed = _jogSpeed;
                reachTime = _timeToReachJogSpeed;
                break;
        }

        LerpSpeedToTargetValue(targetSpeed, reachTime);
    }

    private void ManageStand(StandStates standState)
    {
        switch (standState)
        {
            case StandStates.Stand:
                _onStand?.Invoke();
                break;

            case StandStates.Crouch:
                _onCrouch?.Invoke();
                break;
        }
    }

    private void LerpSpeedToTargetValue (float TargetSpeed, float TimeToReach)
    {
        if (_speedChangeInterval != null && _speedChangeInterval.Busy)
            _speedChangeInterval.Stop();

        float moduleLeftSpeed = MathF.Abs(TargetSpeed - _currentSpeed);
        float speedChangePerTimeStep = MathF.Round(moduleLeftSpeed / (TimeToReach / _speedChangeTimeStep), 3);

        Action speedChangeAction = delegate
        {
            if (_currentSpeed == TargetSpeed)
            {
                _speedChangeInterval.Stop();
                return;
            }

            float changedSpeed = 0f;

            if (_currentSpeed > TargetSpeed)
            {
                changedSpeed = _currentSpeed - speedChangePerTimeStep;

                if (changedSpeed <= TargetSpeed)
                    changedSpeed = TargetSpeed;
            }
            else if (_currentSpeed < TargetSpeed)
            {
                changedSpeed = _currentSpeed + speedChangePerTimeStep;

                if (changedSpeed >= TargetSpeed)
                    changedSpeed = TargetSpeed;
            }

            _currentSpeed = changedSpeed;
            float speedMaxPercentage = MathF.Round(_currentSpeed / _jogSpeed, 2);
            _onSpeedChangePercentage?.Invoke(speedMaxPercentage);
        };

        _speedChangeInterval.StartInterval(_speedChangeTimeStep, speedChangeAction);
    }

    private void MoveByInputDirection(Vector2 InputDirection)
    {
        transform.position = CalculateMovementTarget(InputDirection);
    }

    private Vector3 CalculateMovementTarget(Vector2 InputDirection)
    {
        Vector2 inputDirectionNormalized = InputDirection.normalized;
        Vector3 movementForwardDirection = transform.forward * inputDirectionNormalized.y;
        Vector3 movementRightDirection = transform.right * inputDirectionNormalized.x;
        Vector3 movementDirection = movementForwardDirection + movementRightDirection;
        Vector3 movementDirectionNormalized = movementDirection.normalized;
        Vector3 movementTarget = transform.position + movementDirectionNormalized;

        return Vector3.Lerp(transform.position, movementTarget, Time.fixedDeltaTime * _currentSpeed);
    }

    public void Acceleration (bool NeedToAccelerate) => _isAccelerate = NeedToAccelerate;

    public void Crouching(bool NeedToCrouch) => _isCrouch = NeedToCrouch;
    #endregion
}

public enum SpeedStates
{
    Idle,
    Walk,
    Jog
}

public enum StandStates
{
    Stand,
    Crouch
}