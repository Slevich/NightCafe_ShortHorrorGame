using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Selectable : MonoBehaviour, ISelectable
{
    #region Fields
    [field: Header("Object is currently selected?"), SerializeField, ReadOnly] public bool SelectedNow { get; set; }
    [Header("Event called on selection."), SerializeField] public UnityEvent _onSelect;
    [Header("Event called on deselection."), SerializeField] public UnityEvent _onDeselect;
    #endregion

    #region Methods
    public void Select()
    {
        if (SelectedNow)
            return;

        SelectedNow = true;
        _onSelect?.Invoke();
    }

    public void Deselect()
    {
        if (!SelectedNow)
            return;

        SelectedNow = false;
        _onDeselect?.Invoke();
    }
    #endregion
}
