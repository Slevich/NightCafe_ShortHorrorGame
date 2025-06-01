using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableItem : MonoBehaviour
{
    #region Fields
    [field: Header("Transform parent of the item."), SerializeField] public Transform ItemParent { get; set; }
    [field: Header("Scriptable object with item's definition."), SerializeField] public GrabbableItemScriptable Definition { get; set; }

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
    #endregion
}
