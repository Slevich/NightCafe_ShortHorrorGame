using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

public class ItemsManager : MonoBehaviour
{
    #region Fields
    [Header("Item sped to lerp into holder point."), SerializeField, Range(0f, 100f)] private float _itemLerpSpeed = 0.5f;
    [Header("Currently holding item."), SerializeField, ReadOnly] private GrabbableItem _currentItem;
    [Header("Objects in stack."), SerializeField, ReadOnly] private int _itemCount = 0;
    [Header("Parent for items."), SerializeField] private Transform _itemsParent;
    [Header("Items preview."), SerializeField] private PreviewItem[] _items;

    private int _maxStack = 1;
    private float _distanceToStop = 0.1f;
    private float _lerpIntervalUpdateTime = 0.01f;
    private ItemGrabber _grabber;
    private RigWeightLerper _rigWeightLerper;
    private ActionInterval _itemLerpInterval;
    private AudioPlayer _player;
    #endregion

    #region Properties
    public Transform ItemsParent => _itemsParent;
    public GrabbableItem CurrentItem => _currentItem;
    public int ItemCount => _itemCount;
    #endregion

    #region Methods
    [Inject]
    public void Construct(RigWeightLerper Lerper, ItemGrabber Grabber, AudioPlayer Player)
    {
        _rigWeightLerper = Lerper;
        _grabber = Grabber;
        _player = Player;

        foreach (PreviewItem item in _items)
        {
            item.DestroyExample();
            item.SetRigWeightLerper(_rigWeightLerper);
        }

        _itemLerpInterval = new ActionInterval();
    }

    public bool IsValidItem(GameObject selectableGameobject)
    {
        GrabbableItem grabbableItem = (GrabbableItem)ComponentsSearcher.GetSingleComponentOfTypeFromObjectAndChildren(selectableGameobject, typeof(GrabbableItem));
        return ItemValidate(grabbableItem);
    }

    private bool ItemValidate(GrabbableItem grabbableItem)
    {
        if (grabbableItem == null)
        {
            return false;
        }
        
        if(!grabbableItem.EnableToGrab)
            return false;

        if (_currentItem == null)
        {
            return true;
        }

        if (!_currentItem.Definition.Stackable)
        {
            return false;
        }

        if (grabbableItem.Definition.Name == _currentItem.Definition.Name)
        {
            return true;
        }    

        return false;
    }

    private void SetNewCurrentItem (GrabbableItem Item)
    {
        if (Item == null)
            return;

        if (_currentItem == null)
        {
            _currentItem = Item;
            _currentItem.EnableToGrab = false;
            _itemCount = Item.ItemsInStack;

            Transform itemParent = Item.ItemParent;
            itemParent.SetParent(ItemsParent);

            ISelectable selectable = (Selectable)(ComponentsSearcher.GetSingleComponentOfTypeFromObjectAndChildren(itemParent.gameObject, typeof(ISelectable)));
            if (selectable != null)
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

            LerpObjectToPositionAndRotation(itemParent, Item.Definition.PositionInHolder, Item.Definition.RotationInHolder);
            if (_player != null)
                _player.PlaySound(_currentItem.Definition.Clip);
        }
        else
        {
            if (!_currentItem.Definition.Stackable)
            {
                return;
            }    

            if (Item.Definition.Name != _currentItem.Definition.Name)
            {
                return;
            }

            if (_currentItem == Item)
            {
                return;
            }

            Transform itemParent = Item.ItemParent;
            Transform meshesParent = Item.MeshesParent;

            if (meshesParent != null)
            {
                _currentItem.ItemsInStack++;
                GameObject meshesObject = Instantiate(meshesParent.gameObject);
                meshesObject.transform.rotation = meshesParent.transform.rotation;
                meshesObject.transform.position = meshesParent.transform.position;
                meshesObject.transform.SetParent(_currentItem.MeshesParent.parent);
                Vector3 targetPosition = _currentItem.MeshesParent.localPosition + (Vector3.up * (0.01f * _currentItem.ItemsInStack));
                LerpObjectToPositionAndRotation(meshesObject.transform, targetPosition, _currentItem.MeshesParent.localRotation.eulerAngles);
                _itemCount = _currentItem.ItemsInStack;
            }

            if (_player != null)
                _player.PlaySound(_currentItem.Definition.Clip);

            Destroy(itemParent.gameObject);
        }
    }

    private void RemoveCurrentItem(bool forceRemove)
    {
        if (_itemLerpInterval != null && _itemLerpInterval.Busy)
            _itemLerpInterval.Stop();

        Transform itemParent = _currentItem.ItemParent;
        Transform itemStartParent = _currentItem.ItemStartParent;
        itemParent.SetParent(itemStartParent);

        if (_currentItem.Definition.Stackable)
            _itemCount = 0;

        if (!forceRemove)
        {
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
        }

        _currentItem = null;
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

    private void OnEnable ()
    {
        if(_grabber != null)
        {

            _grabber.OnGrabCallback += (item) => SetNewCurrentItem(item);
            _grabber.OnDropCallback += (force) => RemoveCurrentItem(force);
        }
    }

    private void OnDisable ()
    {
        foreach (PreviewItem item in _items)
        {
            item.DestroyExample();
        }

        if (_grabber != null)
        {
            _grabber.OnGrabCallback -= (item) => SetNewCurrentItem(item);
            _grabber.OnDropCallback -= (force) => RemoveCurrentItem(force);
        }

        if (_itemLerpInterval != null && _itemLerpInterval.Busy)
            _itemLerpInterval.Stop();
    }
    #endregion
}

[Serializable, ExecuteInEditMode]
public class PreviewItem
{
    [SerializeField] private GrabbableItemScriptable _item;

    [SerializeField, HideInInspector] private GameObject _example;
    [SerializeField, HideInInspector] private Transform _parent;
    [SerializeField, HideInInspector] private RigWeightLerper _rigWeightLerper;

    public bool HasExample => _example != null;
    public GameObject Example => _example;

    public void SetParent(Transform Parent) => _parent = Parent;

    public void SetRigWeightLerper(RigWeightLerper lerper) => _rigWeightLerper = lerper;

    public void DestroyExample()
    {
        if(_example != null)
        {
            MonoBehaviour.DestroyImmediate(_example);
            _example = null;
        }
    }

    public GameObject SpawnExample ()
    {
        if(_item != null && _parent != null && _example == null)
        {
            GameObject example = MonoBehaviour.Instantiate(_item.Prefab, _parent);
            example.transform.localPosition = _item.PositionInHolder;
            Quaternion rotation = Quaternion.Euler(_item.RotationInHolder);
            example.transform.localRotation = rotation;
            _rigWeightLerper.SetWeights(1);
            return example;
        }

        return null;
    }

    public void UpdatePositionAndRotation()
    {
        if(_item != null)
        {
            _item.PositionInHolder = _example.transform.localPosition;
            _item.RotationInHolder = _example.transform.localRotation.eulerAngles;
            _rigWeightLerper.SetWeights(0);
        }
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(PreviewItem))]
public class PreviewItemDrawer : PropertyDrawer
{
    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
    {
        PreviewItem item = (PreviewItem)property.boxedValue;
        SerializedProperty itemProperty = property.FindPropertyRelative("_item");
        EditorGUILayout.PropertyField(itemProperty);
        GrabbableItemScriptable itemScriptable = (GrabbableItemScriptable)itemProperty.boxedValue;
        UnityEngine.Object prefab = itemScriptable != null ? itemScriptable.Prefab : null;

        if(itemScriptable != null)
        {
            EditorGUI.indentLevel++;
            
            bool exampleSpanwed = item.HasExample;
            string buttonName = exampleSpanwed ? "End preview." : "See preview.";
            
            if (prefab != null && Application.IsPlaying(prefab))
            {
                bool buttonPressed = GUILayout.Button(buttonName);
            
                if (buttonPressed)
                {
                    if (exampleSpanwed)
                        item.DestroyExample();
                    else
                    {
                        Transform spawnParent = (Transform)(property.serializedObject.FindProperty("_itemsParent").boxedValue);
                        item.SetParent(spawnParent);
                        GameObject spawnedObject = item.SpawnExample();
            
                        if (spawnedObject != null)
                        {
                            SerializedProperty exampleProperty = property.FindPropertyRelative("_example");
                            exampleProperty.objectReferenceValue = spawnedObject;
                        }
                    }
                }
            
                if (exampleSpanwed)
                {
                    GameObject example = item.Example;

                    if (example != null)
                    {
                        Debug.Log("Задал!");
                        itemScriptable.PositionInHolder = example.transform.localPosition;
                        itemScriptable.RotationInHolder = example.transform.localRotation.eulerAngles;
                        EditorUtility.SetDirty(itemScriptable);
                    }
                }
            }
            
            EditorGUI.indentLevel--;
        }
    }
}
#endif