using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class ItemsSelector : MonoBehaviour
{
    #region Fields
    [Header("Current selectable object."), SerializeField, ReadOnly] private GameObject _selectableObject;
    [Header("Event on every selection."), SerializeField] private UnityEvent _onEverySelection;
    [Header("Event on every deselection."), SerializeField] private UnityEvent _onEveryDeselection;

    public Action<GameObject> OnSelectionCallback { get; set; }
    private ISelectable _currentSelectable = null;
    private ItemsManager _itemsManager = null;
    private InteractionManager _interactionManager = null;
    #endregion

    #region Methods
    [Inject]
    public void Construct(ItemsManager manager, InteractionManager interactionManager)
    {
        _itemsManager = manager;
        _interactionManager = interactionManager;
    }

    public void ManageSelectables (GameObject CurrentCastedObject)
    {
        if (CurrentCastedObject == null)
        {
            Deselection();
        }
        else
        {
            ISelectable selectable = (ISelectable)(ComponentsSearcher.GetSingleComponentOfTypeFromObjectAndChildren(CurrentCastedObject, typeof(ISelectable)));

            if(selectable == null && _currentSelectable != null)
            {
                Deselection();
            }
            else if((selectable != null && _currentSelectable == null) || (selectable != null && selectable != _currentSelectable))
            {
                if(_currentSelectable != null && selectable != _currentSelectable)
                    Deselection();

                bool selectGrabbable = _itemsManager.IsValidItem(CurrentCastedObject);

                bool selectInteractable = _interactionManager.CanInteract(CurrentCastedObject);

                if (!selectGrabbable && !selectInteractable)
                    return;

                _currentSelectable = selectable;
                _selectableObject = CurrentCastedObject;
                Selection();
            }
        }

        OnSelectionCallback?.Invoke(_selectableObject);
    }

    private void Selection()
    {
        if (_currentSelectable != null)
        {
            _currentSelectable.Select();
            _onEverySelection?.Invoke();
        }
    }

    private void Deselection()
    {
        if (_currentSelectable != null)
        {
            _currentSelectable.Deselect();
            _currentSelectable = null;
            _selectableObject = null;
            _onEveryDeselection?.Invoke();
        }
    }
    #endregion
}
