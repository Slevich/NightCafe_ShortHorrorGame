using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColliderEntrance : MonoBehaviour
{
    #region Fields
    [Header("Collider to enter"), SerializeField] private Collider _collider;
    [Header("Event on enter collider."), SerializeField] private UnityEvent _onEnterCollider;
    [Header("Event on exit collider."), SerializeField] private UnityEvent _onExitCollider;
    #endregion

    #region Methods
    private void OnTriggerEnter (Collider other)
    {
        if (other == _collider)
            _onEnterCollider?.Invoke();
    }

    private void OnTriggerExit (Collider other)
    {
        if (other == _collider)
            _onExitCollider?.Invoke();
    }
    #endregion
}
