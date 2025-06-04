using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionTip : MonoBehaviour
{
    #region Fields
    [field: SerializeField] public string ButtonName { get; set; } = "[E]";
    [field: SerializeField] public string InteractionType { get; set; } = "Взять";
    [field: SerializeField] public string ItemName { get; set; } = "Предмет";
    #endregion

    private void Awake ()
    {
        GrabbableItem item = ((GrabbableItem)ComponentsSearcher.GetSingleComponentOfTypeFromObjectAndChildren(gameObject, typeof(GrabbableItem)));

        if (item != null)
            ItemName = item.Definition.Name;
    }
}
