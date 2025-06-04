using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;
using System;

public class ItemGrabber : MonoBehaviour
{
    [Header("Item sped to lerp into holder point."), SerializeField, Range(0f, 100f)] private float _itemLerpSpeed = 0.5f; 
    [field: Header("Event on grab item."), SerializeField] public UnityEvent OnGrab { get; set; }
    [field: Header("Event on drop item."), SerializeField] public UnityEvent OnDrop { get; set; }

    private CharacterInputHandler _inputHandler;
    private ItemsManager _itemsManager;
    private GameObject _currentlySelectedGameObject;
    private ItemsSelector _selector;
    private bool _canGrab = false;
    public Action<GrabbableItem> OnGrabCallback { get; set; }
    public Action<bool> OnDropCallback { get; set; }

    [Inject]
    public void Construct(ItemsManager Manager, ItemsSelector Selector, CharacterInputHandler InputHandler)
    {
        _itemsManager = Manager;
        _selector = Selector;
        _inputHandler = InputHandler;
        _selector.OnSelectionCallback += (selectable) => CanGrab(selectable);
        _inputHandler.InteractionButtonPressedCallback += Grab;
        _inputHandler.DropButtonPressedCallback += Drop;
    }

    private void CanGrab(GameObject GrabbableObject)
    {
        if (GrabbableObject == null)
            _canGrab = false;
        else
            _canGrab = true;

        _currentlySelectedGameObject = GrabbableObject;
    }

    private void Grab()
    {
        if(_canGrab && _currentlySelectedGameObject != null)
        {
            GrabbableItem grabbableItem = (GrabbableItem)(ComponentsSearcher.GetSingleComponentOfTypeFromObjectAndChildren(_currentlySelectedGameObject, typeof(GrabbableItem)));

            if (grabbableItem == null)
                return;

            if (!grabbableItem.EnableToGrab)
                return;

            bool validItem = _itemsManager.IsValidItem(_currentlySelectedGameObject);

            if (!validItem)
                return;

            OnGrabCallback?.Invoke(grabbableItem);

            OnGrab?.Invoke();
        }
    }

    private void Drop()
    {
        if(_itemsManager.CurrentItem != null)
        {
            OnDropCallback?.Invoke(false);
            OnDrop?.Invoke();
        }
    }

    public void ForceDrop()
    {
        if (_itemsManager.CurrentItem != null)
        {
            OnDropCallback?.Invoke(true);
            OnDrop?.Invoke();
        }
    }

    private void OnDisable ()
    {
        _inputHandler.InteractionButtonPressedCallback -= Grab;
        _inputHandler.DropButtonPressedCallback -= Drop;
    }
}
