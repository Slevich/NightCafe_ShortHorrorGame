using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsSelector : MonoBehaviour
{
    #region Fields
    [Header("Current selectable object parent."), SerializeField, ReadOnly] private Transform _selectableObjectParent;

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
            }

            return;
        }

        ISelectable selectable = (ISelectable)(ComponentsSearcher.GetSingleComponentOfTypeFromObjectAndChildren(CurrentCastedObject, typeof(ISelectable)));

        if(selectable != _currentSelectable)
        {
            if(_currentSelectable != null)
                _currentSelectable.Deselect();

            _currentSelectable = selectable;

            if (_currentSelectable != null)
                _currentSelectable.Select();
        }
    }
    #endregion
}
