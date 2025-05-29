using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System;
using UniRx;

public static class InputHandler
{
    #region Fields
    private static InputActions _inputActions;
    private static KeyboardMouseInfo _inputInfo;
    public static Subject<KeyboardMouseInfo> InputInfoStream;

    private static ActionUpdate _dataStreamUpdate;
    private static Action _dataStreamUpdateAction;
    #endregion

    #region Constructor
    static InputHandler ()
    {
        if (Application.isPlaying)
            Initialize();
    }
    #endregion

    #region Methods
    public static void Initialize ()
    {
        _inputActions = new InputActions();
        _dataStreamUpdate = new ActionUpdate();
        _inputInfo = new KeyboardMouseInfo(_inputActions.Character);
        InputInfoStream = new Subject<KeyboardMouseInfo>();

        _dataStreamUpdateAction = delegate
        {
            InputInfoStream.OnNext(_inputInfo);
        };
    }

    public static void EnableAllInput ()
    {
        _inputActions.Enable();
        _dataStreamUpdate.StartUpdate(_dataStreamUpdateAction);
    }

    public static void DisableAllInput ()
    {
        _inputActions.Disable();
        _dataStreamUpdate.StopUpdate();
    }

    public static void EnableCharacterInput()
    {
        _inputActions.Character.Enable();
    }

    public static void DisableCharacterInput()
    {
        _inputActions.Character.Disable();
    }

    public static void EnableUIInput ()
    {
        _inputActions.UI.Enable();
    }

    public static void DisableUIInput ()
    {
        _inputActions.UI.Disable();
    }
    #endregion
}
