using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MovementByInput : MonoBehaviour
{
    #region Fields
    [Header("Speed modifier of the base movement."), SerializeField, Range(0.1f, 10f)] private float _baseMovementSpeed = 1f;

    private CharacterInputHandler _inputHandler;
    #endregion

    #region Methods
    [Inject]
    public void Construct (CharacterInputHandler InputHandler)
    {
        _inputHandler = InputHandler;
    }

    private void Subscribe()
    {
        if (_inputHandler != null)
        {
            _inputHandler.InputDirectionCallback += (direction) => MoveByInputDirection(direction);
        }
    }

    private void Dispose()
    {
        if (_inputHandler != null)
        {
            _inputHandler.InputDirectionCallback -= (direction) => MoveByInputDirection(direction);
        }
    }

    public void MoveByInputDirection(Vector2 InputDirection)
    {
        if(InputDirection == Vector2.zero)
            return;

        Vector2 inputDirectionNormalized = InputDirection.normalized;
        Vector3 movementForwardDirection = transform.forward * inputDirectionNormalized.y;
        Vector3 movementRightDirection = transform.right * inputDirectionNormalized.x;
        Vector3 movementDirection = movementForwardDirection + movementRightDirection;
        Vector3 movementDirectionNormalized = movementDirection.normalized;
        Vector3 movementTarget = transform.position + movementDirectionNormalized;
        transform.position = Vector3.Lerp(transform.position, movementTarget, Time.fixedDeltaTime * _baseMovementSpeed);
    }

    private void OnEnable () => Subscribe();
    private void OnDisable () => Dispose();
    #endregion
}
