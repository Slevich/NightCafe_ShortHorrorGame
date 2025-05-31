using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemsManager : MonoBehaviour
{
    #region Fields
    [Header("Parent for items."), SerializeField] private Transform _itemsParent;
    [Header("Items preview."), SerializeField] private PreviewItem[] _items;

    #endregion

    #region Methods
    private void Awake ()
    {
        foreach(PreviewItem item in _items)
            item.DestroyExample();
    }

    #endregion
}

public enum Items
{
    Test
}


[Serializable, ExecuteInEditMode]
public class PreviewItem
{
    [SerializeField] private Items _itemType = Items.Test;
    [SerializeField] private GameObject _examplePrefab;
    [SerializeField] private Vector3 _position;
    [SerializeField] private Vector3 _rotation;

    [SerializeField, HideInInspector] private GameObject _example;
    [SerializeField, HideInInspector] private Transform _parent;
    [SerializeField, HideInInspector] private bool _expanded = false;

    public bool HasExample => _example != null;
    public GameObject Example => _example;

    public void SetParent(Transform Parent) => _parent = Parent;

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
        if(_examplePrefab != null && _parent != null && _example == null)
        {
            GameObject example = MonoBehaviour.Instantiate(_examplePrefab, _parent);
            example.transform.localPosition = _position;
            Quaternion rotation = Quaternion.Euler(_rotation);
            example.transform.localRotation = rotation;
            return example;
        }

        return null;
    }

    public void UpdatePositionAndRotation()
    {
        if(_example != null)
        {
            _position = _example.transform.localPosition;
            _rotation = _example.transform.localRotation.eulerAngles;
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
        SerializedProperty prefabPropertry = property.FindPropertyRelative("_examplePrefab");
        UnityEngine.Object prefab = prefabPropertry.objectReferenceValue;
        SerializedProperty itemTypeProperty = property.FindPropertyRelative("_itemType");

        int enumIndex = itemTypeProperty.enumValueIndex;
        string foldoutName = ((Items)(itemTypeProperty.boxedValue)).ToString() + "_Preview";

        SerializedProperty expandedProperty = property.FindPropertyRelative("_expanded");
        bool expanded = expandedProperty.boolValue;

        expanded = EditorGUILayout.Foldout(expanded, foldoutName);
        expandedProperty.boolValue = expanded;

        if (expanded)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(itemTypeProperty);

            EditorGUILayout.PropertyField(prefabPropertry);

            SerializedProperty positionProperty = property.FindPropertyRelative("_position");
            EditorGUILayout.PropertyField(positionProperty);

            SerializedProperty rotationProperty = property.FindPropertyRelative("_rotation");
            EditorGUILayout.PropertyField(rotationProperty);

            bool exampleSpanwed = item.HasExample;

            string buttonName = exampleSpanwed ? "End preview." : "See preview.";

            if(prefab != null)
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

                        if(spawnedObject != null)
                        {
                            SerializedProperty exampleProperty = property.FindPropertyRelative("_example");
                            exampleProperty.objectReferenceValue = spawnedObject;
                        }
                    }
                }

                if (exampleSpanwed)
                {
                    GameObject example = item.Example;
                    positionProperty.vector3Value = example.transform.localPosition;
                    rotationProperty.vector3Value = example.transform.localRotation.eulerAngles;
                }
            }

            EditorGUI.indentLevel--;
        }

        EditorUtility.SetDirty(property.serializedObject.targetObject);
        base.OnGUI(position, property, label);
    }
}
#endif