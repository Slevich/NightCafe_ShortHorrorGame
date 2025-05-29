using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandlerDisplay : MonoBehaviour
{
    #region Fields
    [Header("All input is enabled?"), SerializeField, ReadOnly] private bool _allInputEnabled = false;
    [Header("Character input is enabled?"), SerializeField, ReadOnly] private bool _characterInputEnabled = true;
    [Header("UI input is enabled?"), SerializeField, ReadOnly] private bool _uiInputEnabled = true;
    #endregion

    #region Methods
    public void EnableAllInput ()
    {
        _allInputEnabled = true;
        InputHandler.EnableAllInput();
    }
    public void DisableAllInput()
    {
        _allInputEnabled = false;
        InputHandler.DisableAllInput();
    }

    public void EnableCharacterInput()
    {
        _characterInputEnabled = true;
        InputHandler.EnableCharacterInput();
    }

    public void DisableCharacterInput ()
    {
        _characterInputEnabled = false;
        InputHandler.DisableCharacterInput();
    }

    public void EnableUIInput ()
    {
        _uiInputEnabled = true;
        InputHandler.EnableUIInput();
    }

    public void DisableUIInput ()
    {
        _uiInputEnabled = false;
        InputHandler.DisableUIInput();
    }

    public void HideCursor()
    {
        Cursor.visible = false;
    }

    public void ShowCursor()
    {
        Cursor.visible = true;
    }
    #endregion
}
