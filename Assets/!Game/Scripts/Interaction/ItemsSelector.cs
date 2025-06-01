using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsSelector : MonoBehaviour
{
    #region Fields
    [Header("Current selectable object."), SerializeField, ReadOnly] private GameObject _selectableObject;

    public Action<GameObject> OnSelectionCallback { get; set; }
    private ISelectable _currentSelectable = null;
    #endregion

    #region Methods
    public void ManageSelectables (GameObject CurrentCastedObject)
    {
        if(CurrentCastedObject == null)
        {
            if(_currentSelectable != null)
            {
                _currentSelectable.Deselect();
                _currentSelectable = null;
                _selectableObject = null;
            }

            return;
        }

        ISelectable selectable = (ISelectable)(ComponentsSearcher.GetSingleComponentOfTypeFromObjectAndChildren(CurrentCastedObject, typeof(ISelectable)));

        if(selectable != _currentSelectable)
        {
            if(_currentSelectable != null)
                _currentSelectable.Deselect();

            _currentSelectable = selectable;
            _selectableObject = CurrentCastedObject;

            if (_currentSelectable != null)
                _currentSelectable.Select();
        }

        OnSelectionCallback?.Invoke(_selectableObject);
    }
    #endregion
}
