using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInputInfo
{
    public Vector2 ReturnInputDirection ();
    public Vector2 ReturnPointerPosition ();
    public Vector2 ReturnPointerDelta ();

    public bool InteractionButtonPressed ();
    public bool AccelerationButtonPressed ();
    public bool ApproximationButtonPressed ();
}
