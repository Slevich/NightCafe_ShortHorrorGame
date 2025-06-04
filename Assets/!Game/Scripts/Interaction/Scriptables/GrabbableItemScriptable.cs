using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemScriptable", menuName = "ScriptableObjects/GrabbableItemScriptable", order = 1)]
public class GrabbableItemScriptable : ScriptableObject
{
    #region Properties
    [field: Header("Item name."), SerializeField] public string Name { get; set; } = "Name";
    [field: Header("Items can stack with another this type."), SerializeField] public bool Stackable { get; set; } = false;
    [field: Header("Sound on grab."), SerializeField] public AudioClip Clip { get; set; }
    [field: Header("Example prefab."), SerializeField] public GameObject Prefab { get; set; }
    [field: Header("LocalPosition in holder."), SerializeField] public Vector3 PositionInHolder { get; set; } = Vector3.zero;
    [field: Header("LocalPosition in holder."), SerializeField] public Vector3 RotationInHolder { get; set; } = Vector3.zero;
    #endregion
}
