using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public InteractionType Interaction { get; set; }

    public GameObject InteractableObject { get; set; }
}
