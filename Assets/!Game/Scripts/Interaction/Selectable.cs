using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Selectable : MonoBehaviour, ISelectable
{
    #region Properties
    [field: Header("Selectable is currently working by raycast?"), SerializeField, ReadOnly] public bool LockToCast { get; set; } = false;
    [field: Header("Object is currently selected?"), SerializeField, ReadOnly] public bool SelectedNow { get; set; } = false;
    [field: Header("Event called on selection."), SerializeField] public UnityEvent OnSelect { get; set; }
    [field: Header("Event called on deselection."), SerializeField] public UnityEvent OnDeselect { get; set; }
    #endregion

    #region Methods
    public void Select()
    {
        if(LockToCast)
            return;

        if (SelectedNow)
            return;

        SelectedNow = true;
        OnSelect?.Invoke();
    }

    public void Deselect()
    {
        if (LockToCast)
            return;

        if (!SelectedNow)
            return;

        SelectedNow = false;
        OnDeselect?.Invoke();
    }
    #endregion
}
