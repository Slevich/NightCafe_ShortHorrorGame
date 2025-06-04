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

    public Action InteractionButtonPressedCallback { get; set; }
    public Action InteractionButtonReleasedCallback { get; set; }
    public Action AccelerationButtonPressedCallback { get; set; }
    public Action AccelerationButtonReleasedCallback { get; set; }
    public Action CrouchButtonPressedCallback { get; set; }
    public Action CrouchButtonReleasedCallback { get; set; }
    public Action DropButtonPressedCallback { get; set; }
    public Action DropButtonReleasedCallback { get; set; }
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
    }

    private void Subscribe()
    {
        KeyboardMouseInfo info = InputHandler.InputInfo;

        info.InteractionButtonPressedCallback += delegate { InteractionButtonPressedCallback?.Invoke(); } ;
        info.InteractionButtonReleasedCallback += delegate { InteractionButtonReleasedCallback?.Invoke(); } ;

        info.AccelerationButtonPressedCallback += delegate { AccelerationButtonPressedCallback?.Invoke(); };
        info.AccelerationButtonReleasedCallback += delegate { AccelerationButtonReleasedCallback?.Invoke(); };

        info.CrouchingButtonPressedCallback += delegate { CrouchButtonPressedCallback?.Invoke(); } ;
        info.CrouchingButtonReleasedCallback += delegate { CrouchButtonReleasedCallback?.Invoke(); };

        info.DropButtonPressedCallback += delegate { DropButtonPressedCallback?.Invoke(); };
        info.DropButtonReleasedCallback +=  delegate { DropButtonReleasedCallback?.Invoke(); } ;
    }

    private void Dispose()
    {
        KeyboardMouseInfo info = InputHandler.InputInfo;

        info.InteractionButtonPressedCallback -= delegate { InteractionButtonPressedCallback?.Invoke(); };
        info.InteractionButtonReleasedCallback -= delegate { InteractionButtonReleasedCallback?.Invoke(); };

        info.AccelerationButtonPressedCallback -= delegate { AccelerationButtonPressedCallback?.Invoke(); };
        info.AccelerationButtonReleasedCallback -= delegate { AccelerationButtonReleasedCallback?.Invoke(); };

        info.CrouchingButtonPressedCallback -= delegate { CrouchButtonPressedCallback?.Invoke(); };
        info.CrouchingButtonReleasedCallback -= delegate { CrouchButtonReleasedCallback?.Invoke(); };

        info.DropButtonPressedCallback -= delegate { DropButtonPressedCallback?.Invoke(); };
        info.DropButtonReleasedCallback -= delegate { DropButtonReleasedCallback?.Invoke(); };
    }

    private void OnEnable () => Subscribe();
    private void OnDisable () => Dispose();
}
