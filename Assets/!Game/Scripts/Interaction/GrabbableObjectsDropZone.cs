using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class GrabbableObjectsDropZone : MonoBehaviour, IInteractable
{
    #region Fields
    [Header("Grabbable item to drop into zone."), SerializeField] private GrabbableItemScriptable _item;
    [Header("Names of the active objects to check"), SerializeField] private string[] _namesToCheck;
    [Header("Check in object active?"), SerializeField] private bool _positiveCheck = false;
    [Header("Stack transform point."), SerializeField] private Transform _stackPoint;
    [Header("Item sped to lerp into holder point."), SerializeField, Range(0f, 100f)] private float _itemLerpSpeed = 0.5f;
    [Header("Count to end."), SerializeField] private int _targetItemsCount = 1;
    [Header("Current count."), SerializeField, ReadOnly] private int _currentItemsCount = 0;
    [Header("Event on count reach target."), SerializeField] private UnityEvent _onCountReached;

    private List<GrabbableItem> _itemsInZone = new List<GrabbableItem>();
    private GrabbableItem _currentItem;
    private ItemGrabber _grabber;
    private float _distanceToStop = 0.1f;
    private float _lerpIntervalUpdateTime = 0.01f;
    private ActionInterval _itemLerpInterval;
    #endregion

    #region Properties
    [field: Header("Interaction currently blocked?"), SerializeField, ReadOnly] public bool AlreadyInteracted { get; set; } = false;
    #endregion

    #region Methods
    private void Awake ()
    {
        _itemLerpInterval = new ActionInterval();
    }

    public string[] ReturnNamesToCheck() => _namesToCheck;

    public bool SomethingInHands (GrabbableItem Item, ItemGrabber Grabber)
    {
        if (_grabber == null)
            _grabber = Grabber;

        if(AlreadyInteracted)
            return false;

        _currentItem = Item;

        if(_currentItem == null)
            return false;


        bool sameItem = _item == Item.Definition;

        if(_namesToCheck.Length == 0)
            return sameItem;

        if(!sameItem)
            return false;

        int count = 0;
        foreach (string name in _namesToCheck)
        {
            bool state = Item.CheckActiveStateItem(name);

            if(state == _positiveCheck)
                count++;
        }

        return count == _namesToCheck.Length;
    }

    public void ActivateFirstStateItem()
    {
        if (_namesToCheck.Length == 0)
            return;

        _currentItem.ActivateStateItem(_namesToCheck.First());
    }

    public void Interact ()
    {
        if (AlreadyInteracted)
            return;

        if (_currentItem == null)
            return;

        if (_itemsInZone.Contains(_currentItem))
            return;

        _grabber.ForceDrop();
        _currentItem.ItemParent.SetParent(_stackPoint);
        LerpObjectToPositionAndRotation(_currentItem.ItemParent, new Vector3(0, 0.05f * _itemsInZone.Count, 0), _stackPoint.localEulerAngles);
        _itemsInZone.Add(_currentItem);
        _currentItemsCount += _currentItem.ItemsInStack;
        Debug.Log("Ïîëîæèë!");

        ISelectable selectable = (ISelectable)(ComponentsSearcher.GetSingleComponentOfTypeFromObjectAndChildren(this.gameObject, typeof(ISelectable)));

        if (selectable != null)
        {
            selectable.Deselect();
        }

        if (_currentItemsCount >= _targetItemsCount)
        {
            _onCountReached?.Invoke();
            Debug.Log("ÂÑ¨ ÏÎËÎÆÈË!");
            AlreadyInteracted = true;

            if(selectable != null)
            {
                selectable.LockToCast = true;
            }
        }
    }

    private void LerpObjectToPositionAndRotation (Transform origin, Vector3 targetPosition, Vector3 eulerAngles)
    {
        if (_itemLerpInterval != null && _itemLerpInterval.Busy)
            return;

        Action lerpAction = delegate
        {
            Vector3 originPosition = origin.localPosition;
            Quaternion targetRotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z);
            float distance = Vector3.Distance(originPosition, targetPosition);

            if (distance <= _distanceToStop)
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

    public void ResetInteraction ()
    {
        if (!AlreadyInteracted)
            return;

        AlreadyInteracted = false;
    }

    public void ReleaseObjectInZone()
    {
        if(_currentItem == null)
            return;

        Transform itemParent = _currentItem.ItemParent;
        itemParent.SetParent(_currentItem.ItemStartParent);

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

        _currentItem.EnableToGrab = true;
        _currentItem = null;
    }

    private void OnDisable ()
    {
        if (_itemLerpInterval != null && _itemLerpInterval.Busy)
            _itemLerpInterval.Stop();
    }
    #endregion
}
