using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardMouseInfo
{
    private InputActions.CharacterActions _characterActions;

    #region Constructor
    public KeyboardMouseInfo(InputActions.CharacterActions CharacterActions)
    {
        _characterActions = CharacterActions;
    }
    #endregion

    #region Methods
    public Vector2 ReturnInputDirection () => _characterActions.MovementDirection.ReadValue<Vector2>();
    public Vector2 ReturnPointerPosition () => _characterActions.PointerPosition.ReadValue<Vector2>();

    public Vector2 ReturnPointerDelta () => _characterActions.PointerDelta.ReadValue<Vector2>();

    public Action InteractionButtonPressedCallback { get; set; }
    public Action InteractionButtonReleasedCallback { get; set; }

    public Action AccelerationButtonPressedCallback { get; set; }
    public Action AccelerationButtonReleasedCallback { get; set; }

    public Action CrouchingButtonPressedCallback { get; set; }
    public Action CrouchingButtonReleasedCallback { get; set; }

    public Action DropButtonPressedCallback { get; set; }
    public Action DropButtonReleasedCallback { get; set; }

    public void Subscribe()
    {
        _characterActions.Interaction.started += delegate { InteractionButtonPressedCallback?.Invoke(); };
        _characterActions.Interaction.canceled += delegate { InteractionButtonReleasedCallback?.Invoke(); };

        _characterActions.Drop.started += delegate { DropButtonPressedCallback?.Invoke(); };
        _characterActions.Drop.canceled += delegate { DropButtonReleasedCallback?.Invoke(); };

        _characterActions.Acceleration.started += delegate { AccelerationButtonPressedCallback?.Invoke(); };
        _characterActions.Acceleration.canceled += delegate { AccelerationButtonReleasedCallback?.Invoke(); };

        _characterActions.Crouching.started += delegate { CrouchingButtonPressedCallback?.Invoke(); };
        _characterActions.Crouching.canceled += delegate { CrouchingButtonReleasedCallback?.Invoke(); };
    }

    public void Dispose()
    {
        _characterActions.Interaction.started -= delegate { InteractionButtonPressedCallback?.Invoke(); };
        _characterActions.Interaction.canceled -= delegate { InteractionButtonReleasedCallback?.Invoke(); };

        _characterActions.Drop.started -= delegate { DropButtonPressedCallback?.Invoke(); };
        _characterActions.Drop.canceled -= delegate { DropButtonReleasedCallback?.Invoke(); };

        _characterActions.Acceleration.started -= delegate { AccelerationButtonPressedCallback?.Invoke(); };
        _characterActions.Acceleration.canceled -= delegate { AccelerationButtonReleasedCallback?.Invoke(); };

        _characterActions.Crouching.started -= delegate { CrouchingButtonPressedCallback?.Invoke(); };
        _characterActions.Crouching.canceled -= delegate { CrouchingButtonReleasedCallback?.Invoke(); };
    }
    #endregion
}
