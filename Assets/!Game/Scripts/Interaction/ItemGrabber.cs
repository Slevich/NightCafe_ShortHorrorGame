using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;
using System;

public class ItemGrabber : MonoBehaviour
{
    [Header("Item sped to lerp into holder point."), SerializeField, Range(0f, 100f)] private float _itemLerpSpeed = 0.5f; 
    [field: Header("Event on grab item."), SerializeField] public UnityEvent<GrabbableItem> OnGrab { get; set; }
    [field: Header("Event on drop item."), SerializeField] public UnityEvent<GrabbableItem> OnDrop { get; set; }

    private CharacterInputHandler _inputHandler;
    private ItemsManager _itemsManager;
    private Transform _grabbableItemParent;
    private GameObject _currentlySelectedGameObject;
    private ItemsSelector _selector;
    private bool _canGrab = false;
    private ActionInterval _itemLerpInterval;
    private float _lerpIntervalUpdateTime = 0.01f;
    private float _distanceToStop = 0.1f;

    [Inject]
    public void Construct(ItemsManager Manager, ItemsSelector Selector, CharacterInputHandler InputHandler)
    {
        _itemsManager = Manager;
        _selector = Selector;
        _inputHandler = InputHandler;
        _grabbableItemParent = Manager.ItemsParent;
        _selector.OnSelectionCallback += (selectable) => CanGrab(selectable);
        _inputHandler.InteractionButtonPressCallback += (pressed) => Grab(pressed);
        _inputHandler.DropButtonPressCallback += (pressed) => Drop(pressed);
        _itemLerpInterval = new ActionInterval();
    }

    private void CanGrab(GameObject GrabbableObject)
    {
        if (GrabbableObject == null || _itemsManager.CurrentItem != null)
            _canGrab = false;
        else
            _canGrab = true;

        _currentlySelectedGameObject = GrabbableObject;
    }

    private void Grab(bool Pressed)
    {
        if (!Pressed)
            return;

        if(_canGrab && _currentlySelectedGameObject != null)
        {
            GrabbableItem grabbableItem = (GrabbableItem)(ComponentsSearcher.GetSingleComponentOfTypeFromObjectAndChildren(_currentlySelectedGameObject, typeof(GrabbableItem)));

            if (grabbableItem == null)
                return;

            OnGrab?.Invoke(grabbableItem);

            Transform itemParent = _itemsManager.CurrentItem.ItemParent;
            Transform itemStartParent = _itemsManager.CurrentItem.ItemStartParent;
            itemParent.SetParent(itemParent);

            ISelectable selectable = (Selectable)(ComponentsSearcher.GetSingleComponentOfTypeFromObjectAndChildren(itemParent.gameObject, typeof(ISelectable)));
            if(selectable != null)
            {
                selectable.Deselect();
                selectable.LockToCast = true;
            }    

            Collider itemCollider = (Collider)(ComponentsSearcher.GetSingleComponentOfTypeFromObjectAndChildren(itemParent.gameObject, typeof(Collider)));
            if (itemCollider != null)
                itemCollider.enabled = false;

            Rigidbody itemRigidbody = (Rigidbody)(ComponentsSearcher.GetSingleComponentOfTypeFromObjectAndChildren(itemParent.gameObject, typeof(Rigidbody)));
            if (itemRigidbody != null)
                itemRigidbody.isKinematic = true;

            LerpObjectToPositionAndRotation(itemParent, grabbableItem.Definition.PositionInHolder, grabbableItem.Definition.RotationInHolder);
        }
    }

    private void LerpObjectToPositionAndRotation(Transform origin, Vector3 targetPosition, Vector3 eulerAngles)
    {
        if (_itemLerpInterval != null && _itemLerpInterval.Busy)
            return;

        Action lerpAction = delegate
        {
            Vector3 originPosition = origin.localPosition;
            Quaternion targetRotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z);
            float distance = Vector3.Distance(originPosition, targetPosition);

            if(distance <= _distanceToStop)
            {
                origin.localPosition = targetPosition;
                origin.localRotation = targetRotation;
                _itemLerpInterval.Stop();
                return;
            }

            origin.localPosition = Vector3.Lerp(originPosition, targetPosition, _itemLerpSpeed * Time.fixedDeltaTime);
            origin.localRotation = Quaternion.Lerp(origin.localRotation, targetRotation, _itemLerpSpeed * Time.fixedDeltaTime);
        };

        _itemLerpInterval.StartInterval(_lerpIntervalUpdateTime, lerpAction);
    }

    private void Drop(bool Pressed)
    {
        if(!Pressed)
            return;

        if(_itemsManager.CurrentItem != null)
        {
            OnDrop?.Invoke(null);

            if(_itemLerpInterval != null && _itemLerpInterval.Busy)
                _itemLerpInterval.Stop();

            Transform itemParent = _itemsManager.CurrentItem.ItemParent;
            Transform itemStartParent = _itemsManager.CurrentItem.ItemStartParent;
            itemParent.SetParent(itemStartParent);

            ISelectable selectable = (Selectable)(ComponentsSearcher.GetSingleComponentOfTypeFromObjectAndChildren(itemParent.gameObject, typeof(ISelectable)));
            if (selectable != null)
            {
                selectable.LockToCast = false;
            }

            Collider itemCollider = (Collider)(ComponentsSearcher.GetSingleComponentOfTypeFromObjectAndChildren(itemParent.gameObject, typeof(Collider)));
            if (itemCollider != null)
                itemCollider.enabled = true;

            Rigidbody itemRigidbody = (Rigidbody)(ComponentsSearcher.GetSingleComponentOfTypeFromObjectAndChildren(itemParent.gameObject, typeof(Rigidbody)));
            if (itemRigidbody != null)
                itemRigidbody.isKinematic = false;

        }
    }
}
