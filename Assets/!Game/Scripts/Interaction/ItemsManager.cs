using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

public class ItemsManager : MonoBehaviour
{
    #region Fields
    [Header("Currently holding item."), SerializeField, ReadOnly] private GrabbableItem _currentItem;
    [Header("Parent for items."), SerializeField] private Transform _itemsParent;
    [Header("Items preview."), SerializeField] private PreviewItem[] _items;

    private ItemGrabber _grabber;
    private RigWeightLerper _rigWeightLerper;
    #endregion

    #region Properties
    public Transform ItemsParent => _itemsParent;
    public GrabbableItem CurrentItem => _currentItem;
    #endregion

    #region Methods
    [Inject]
    public void Construct(RigWeightLerper Lerper, ItemGrabber Grabber)
    {
        Debug.Log("Прокинул!");
        _rigWeightLerper = Lerper;
        _grabber = Grabber;

        foreach (PreviewItem item in _items)
        {
            item.DestroyExample();
            item.SetRigWeightLerper(_rigWeightLerper);
        }
    }

    private void SetItem(GrabbableItem Item) => _currentItem = Item;

    private void OnEnable ()
    {
        if(_grabber != null)
        {
            _grabber.OnGrab.AddListener(SetItem);
            _grabber.OnDrop.AddListener(SetItem);
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
            _grabber.OnGrab.RemoveListener(SetItem);
            _grabber.OnDrop.RemoveListener(SetItem);
        }
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
                        itemScriptable.PositionInHolder = example.transform.localPosition;
                        itemScriptable.RotationInHolder = example.transform.localRotation.eulerAngles;
                        EditorUtility.SetDirty(itemScriptable);
                    }
                }
            }
            
            EditorGUI.indentLevel--;
            
            EditorUtility.SetDirty(property.serializedObject.targetObject);
        }
    }
}
#endif