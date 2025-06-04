using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class GrabbableItem : MonoBehaviour
{
    #region Fields
    [field: Header("Enable to grab."), SerializeField, ReadOnly] public bool EnableToGrab { get; set; } = true;
    [field: Header("Transform parent of the item."), SerializeField] public Transform ItemParent { get; set; }
    [field: Header("Tranform parent of the meshes."), SerializeField] public Transform MeshesParent { get; set; }
    [field: Header("Scriptable object with item's definition."), SerializeField] public GrabbableItemScriptable Definition { get; set; }
    [field: Header("Number of items in stack."), SerializeField, ReadOnly] public int ItemsInStack { get; set; } = 1;
    [field: Header("Gameobjects to check their active states"), SerializeField] private GrabbableItemActiveObjectState[] _states;

    public Transform ItemStartParent { get; private set; } = null;

    private void Awake ()
    {
        if (ItemParent == null)
        {
            ItemParent = transform;
        }
        else
        {
            ItemStartParent = ItemParent.parent;
        }
    }
    
    public bool CheckActiveStateItem(string CheckingName)
    {
        if(_states == null || _states.Length == 0)
            return false;

        IEnumerable<GrabbableItemActiveObjectState> states = _states.Where(state => state.Name == CheckingName);
        GrabbableItemActiveObjectState checkingState = null;

        if(states != null && states.Count() > 0)
        {
            checkingState = states.FirstOrDefault();
        }

        if(checkingState == null)
            return false;

        return checkingState.ItemIsActive();
    }

    public void ActivateStateItem(string CheckingName)
    {
        if(_states == null || _states.Length == 0)
            return;

        IEnumerable<GrabbableItemActiveObjectState> states = _states.Where(state => state.Name == CheckingName);
        GrabbableItemActiveObjectState checkingState = null;

        if (states != null && states.Count() > 0)
        {
            checkingState = states.FirstOrDefault();
        }

        if (checkingState == null)
            return;

        checkingState.ActivateItem();
    }
    #endregion
}

[Serializable]
public class GrabbableItemActiveObjectState
{
    [field: Header("Name of the object."), SerializeField] public string Name { get; set; } = "item";
    [field: Header("Object referenct."), SerializeField] private GameObject _activeItem;
    [field: Header("Event on object activation"), SerializeField] private UnityEvent _onObjectActivation;

    public bool ItemIsActive()
    {
        if(_activeItem == null)
            return false;

        return _activeItem.activeInHierarchy;
    }

    public void ActivateItem()
    {
        _activeItem.SetActive(true);
        _onObjectActivation?.Invoke();
    }
}
