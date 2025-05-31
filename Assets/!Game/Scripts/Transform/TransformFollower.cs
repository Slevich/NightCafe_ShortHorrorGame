using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformFollower : MonoBehaviour
{
    #region Fields
    [Tooltip("Объект, за которым нужно следовать")]
    [Header("Follow target."), SerializeField] private Transform _target;

    [Tooltip("Насколько быстро объект следует?")]
    [Header("Follow speed modifier."), SerializeField, Range(0f, 100f)] private float _followSpeed = 10f;

    [Tooltip("Следовать за позицией?")]
    [Header("Following position?"), SerializeField] private bool _followPosition = true;

    [Tooltip("Следовать за вращением?")]
    [Header("Following rotation?"), SerializeField] private bool _followRotation = true;

    private ActionUpdate _update;
    #endregion

    #region Methods
    private void Awake ()
    {
        _update = new ActionUpdate();
    }

    public void StartFollowing()
    {
        if (_target == null)
            return;

        if (_update != null && _update.Busy)
            return;


        Action updateAction = delegate
        {
            if (_followPosition)
                transform.position = Vector3.Lerp(transform.position, _target.position, _followSpeed * Time.fixedDeltaTime); ;

            if (_followRotation)
                transform.rotation = _target.rotation;
        };

        _update.StartUpdate(updateAction);
    }

    public void StopFollowing()
    {
        if(_update != null && _update.Busy)
            _update.StopUpdate();
    }

    private void OnEnable () => StartFollowing();
    private void OnDisable () => StopFollowing();
    #endregion
}
