using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class InteractionManager : MonoBehaviour
{
    #region Fields
    private ItemGrabber _grabber;
    private ItemsManager _itemManager;
    private CharacterInputHandler _characterInputHandler;
    private IInteractable _currentInteractable;
    private bool _subscribed = false;
    #endregion

    #region Methods
    [Inject]
    public void Construct(ItemGrabber Grabber, ItemsManager Manager, CharacterInputHandler InputHander)
    {
        _grabber = Grabber;
        _itemManager = Manager;
        _characterInputHandler = InputHander;

        if (_characterInputHandler != null && !_subscribed)
        {
            _characterInputHandler.InteractionButtonPressedCallback += Interaction;
            _subscribed = true;
        }
    }

    public bool CanInteract(GameObject InteractedGameobject)
    {
        if(InteractedGameobject != null && !InteractedGameobject.activeInHierarchy)
            return false;

        IInteractable interactable = (IInteractable)ComponentsSearcher.GetComponentFromObject(InteractedGameobject, typeof(IInteractable));
        _currentInteractable = interactable;

        if(interactable == null)
            return false;

        return interactable.SomethingInHands(_itemManager.CurrentItem, _grabber);
    }

    public void InteractWith(IInteractable interactable)
    {
        interactable.Interact();
    }

    public void Interaction()
    {
        if(_currentInteractable != null)
            _currentInteractable.Interact();
    }

    private void OnEnable ()
    {
        if(_characterInputHandler != null && !_subscribed)
        {
            _characterInputHandler.InteractionButtonPressedCallback += Interaction;
            _subscribed = true;
        }
    }

    private void OnDisable ()
    {
        if (_characterInputHandler != null && _subscribed)
        {
            _characterInputHandler.InteractionButtonPressedCallback -= Interaction;
            _subscribed = false;
        }
    }
    #endregion
}
