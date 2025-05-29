using System.Collections;
using System.Collections.Generic;
using System;
using UniRx;
using UnityEngine;

public class CharacterInputHandler : MonoBehaviour
{
    #region Properties
    public Action<Vector2> InputDirectionCallback { get; set; }
    public Action<Vector2> PointerPositionCallback { get; set; }
    public Action<Vector2> PointerDeltaCallback { get; set; }

    public Action<bool> InteractionButtonPressCallback { get; set; }
    public Action<bool> AccelerationButtonPressCallback { get; set; }
    public Action<bool> ApproximationButtonPressCallback { get; set; }
    #endregion

    private void Awake ()
    {
        InputHandler.InputInfoStream.Subscribe(info => GetInfo(info)).AddTo(this);
    }

    private void GetInfo(KeyboardMouseInfo info)
    {
        Vector2 inputDirection = info.ReturnInputDirection();
        InputDirectionCallback?.Invoke(inputDirection);

        Vector2 pointerPosition = info.ReturnPointerPosition();
        PointerPositionCallback?.Invoke(pointerPosition);

        Vector2 pointerDelta = info.ReturnPointerDelta();
        PointerDeltaCallback?.Invoke(pointerDelta);

        bool interactionButtonPressed = info.InteractionButtonPressed();
        InteractionButtonPressCallback?.Invoke(interactionButtonPressed);

        bool accelerationButtonPressed = info.AccelerationButtonPressed();
        AccelerationButtonPressCallback?.Invoke(accelerationButtonPressed);

        bool approximationButtonPressed = info.ApproximationButtonPressed();
        ApproximationButtonPressCallback?.Invoke(approximationButtonPressed);
    }
}
