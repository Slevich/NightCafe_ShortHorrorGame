using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class MovementByInput : MonoBehaviour
{
    #region Fields
    [Header("Base speed."), SerializeField, Range(0.1f, 10f)] private float _baseMovementSpeed = 1f;
    [Header("Acceleration speed."), SerializeField, Range(0.1f, 10f)] private float _accelerationSpeed = 1.5f;
    [Header("Event on current speed change."), SerializeField] private UnityEvent<float> _onSpeedChangePercentage;

    private float _currentSpeed = 0f;
    private float _targetSpeed = 0f;
    private bool _isAccelerate = false;
    private CharacterInputHandler _inputHandler;
    #endregion

    #region Properties
    #endregion

    #region Methods
    [Inject]
    public void Construct (CharacterInputHandler InputHandler)
    {
        _inputHandler = InputHandler;
        _targetSpeed = 0f;
        _currentSpeed = _targetSpeed;
        _onSpeedChangePercentage?.Invoke(0);
    }

    private void Subscribe()
    {
        if (_inputHandler != null)
        {
            _inputHandler.InputDirectionCallback += (direction) => MoveByInputDirection(direction);
            _inputHandler.AccelerationButtonPressCallback += (pressed) => Acceleration(pressed);
        }
    }

    private void Dispose()
    {
        if (_inputHandler != null)
        {
            _inputHandler.InputDirectionCallback -= (direction) => MoveByInputDirection(direction);
            _inputHandler.AccelerationButtonPressCallback -= (pressed) => Acceleration(pressed);
        }
    }

    public void MoveByInputDirection(Vector2 InputDirection)
    {
        if(InputDirection == Vector2.zero)
        {
            _onSpeedChangePercentage?.Invoke(0);
            return;
        }

        ManageMaxSpeed();

        Vector2 inputDirectionNormalized = InputDirection.normalized;
        Vector3 movementForwardDirection = transform.forward * inputDirectionNormalized.y;
        Vector3 movementRightDirection = transform.right * inputDirectionNormalized.x;
        Vector3 movementDirection = movementForwardDirection + movementRightDirection;
        Vector3 movementDirectionNormalized = movementDirection.normalized;
        Vector3 movementTarget = transform.position + movementDirectionNormalized;
        transform.position = Vector3.Lerp(transform.position, movementTarget, Time.fixedDeltaTime * _currentSpeed);
    }

    public void ManageMaxSpeed()
    {
        if (_isAccelerate)
            _targetSpeed = _accelerationSpeed;
        else
            _targetSpeed = _baseMovementSpeed;

        _currentSpeed = _targetSpeed;
        float speedMaxPercentage = MathF.Round(_currentSpeed / _accelerationSpeed, 2);
        _onSpeedChangePercentage?.Invoke(speedMaxPercentage);
    }

    public void Acceleration (bool NeedToAccelerate) => _isAccelerate = NeedToAccelerate;

    private void OnEnable () => Subscribe();
    private void OnDisable () => Dispose();
    #endregion
}
