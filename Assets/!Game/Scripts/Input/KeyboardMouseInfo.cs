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

    public bool InteractionButtonPressed () => _characterActions.Interaction.IsInProgress();

    public bool AccelerationButtonPressed () => _characterActions.Acceleration.IsInProgress();

    public bool ApproximationButtonPressed () => _characterActions.Approximation.IsInProgress();

    public bool CrouchingButtonPressed () => _characterActions.Crouching.IsInProgress();
    public bool DropButtonPressed() => _characterActions.Drop.IsInProgress();
    #endregion
}
