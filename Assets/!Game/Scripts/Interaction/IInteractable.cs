using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public bool SomethingInHands (GrabbableItem Item, ItemGrabber Grabber);
    public void Interact ();
    public void ResetInteraction ();
    public bool AlreadyInteracted { get; set; }
    public string[] ReturnNamesToCheck ();

}
