using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SimpleInteractionZone : MonoBehaviour, IInteractable
{
    #region Fields
    [Header("Event on interaction"), SerializeField] private UnityEvent _onInteraction;
    [field: Header("Lock to interaction?"), SerializeField] public bool AlreadyInteracted { get; set; } = true;
    #endregion

    #region Fields
    public bool SomethingInHands (GrabbableItem Item, ItemGrabber Grabber)
    {
        return !AlreadyInteracted;
    }

    public void Interact ()
    {
        if (AlreadyInteracted)
            return;

        _onInteraction?.Invoke();
        AlreadyInteracted = true;
    }

    public void ResetInteraction ()
    {
        AlreadyInteracted = false;
    }

    public string[] ReturnNamesToCheck ()
    {
        return new string[0];
    }
    #endregion
}
