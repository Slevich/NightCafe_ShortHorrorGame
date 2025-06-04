using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformAxisOffsetFollower : MonoBehaviour
{
    #region Fields
    [Header("Transform to follow the axis with start offset."), SerializeField] private Transform _followTransform;
    [Header("Min difference to follow."), SerializeField, Range(0f, 10f)] private float _minDistance = 0.2f;

    private Vector3 _startPosition = Vector3.zero;
    private Vector3 _globalOffset = Vector3.zero;
    private Vector3 _localOffset = Vector3.zero;
    private ActionUpdate _update;
    #endregion

    #region Methods
    private void Awake ()
    {
        if (_followTransform == null)
            return;

        _update = new ActionUpdate();
        _startPosition = transform.position;
        Vector3 targetStartPosition = _followTransform.position;
        _globalOffset = _startPosition - targetStartPosition;

        _localOffset = (_followTransform.InverseTransformPoint(_startPosition) - _followTransform.localPosition);
    }

    public void StartFollowing()
    {
        if(_followTransform == null)
            return;

        Action updateAction = delegate
        {
            Vector3 targetPosition = _followTransform.position;
            Vector3 currentFollowerPosition = transform.position;
            Vector3 currentOffset = targetPosition - currentFollowerPosition;

            if (Vector3.Distance(_globalOffset, currentOffset) < _minDistance)
                return;

            Vector3 newLocalPosition = _followTransform.localPosition + _localOffset;
            Vector3 newGlobalPosition = _followTransform.parent.TransformPoint(newLocalPosition);

            transform.position = newGlobalPosition;
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
