using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{
    #region Fields
    [Header("Navmesh agent."), SerializeField] private NavMeshAgent _agent;
    [Header("Agent animator."), SerializeField] private Animator _animator;
    [Header("Animator trigger name to walk."), SerializeField] private string _triggerToWalk = "Walk";
    [Header("Animator trigger name to idle."), SerializeField] private string _triggerToIdle = "Idle";

    private ActionInterval _interval;
    private float _updateTimeStep = 0.01f;
    private float _stopDistance = 0.05f;
    private bool _isMoving = false;
    private EventContainer _onReachCurrentDestination = null;
    #endregion

    #region Methods
    private void Awake ()
    {
        _interval = new ActionInterval();
    }

    public void MoveToPointIfIdle(Transform Target)
    {
        MoveToPoint(Target);
    }

    public void DropAndMoveToNewPoint(Transform Target)
    {
        Stop();
        MoveToPoint(Target);
    }

    public void SetEventOnReachDestination(EventContainer EventOnReachDestination)
    {
        _onReachCurrentDestination = EventOnReachDestination;
    }

    private void MoveToPoint(Transform TargetPoint)
    {
        if (_agent == null)
            return;

        if (_isMoving)
            return;

        _agent.SetDestination(TargetPoint.position);

        if (_animator != null)
            _animator.SetTrigger(_triggerToWalk);

        Action onUpdateCall = delegate
        {
            float distance = Vector3.Distance(_agent.transform.position, TargetPoint.position);
            Vector3 velocity = _agent.velocity;

            if(velocity != Vector3.zero)
            {
                Quaternion _lookRotation = Quaternion.LookRotation(velocity);
                transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.fixedDeltaTime * 10f);
            }

            if(distance <= _stopDistance)
            {

                if (_onReachCurrentDestination != null)
                {
                    _onReachCurrentDestination.InvokeEventByInvokationType();
                    _onReachCurrentDestination = null;
                }

                Stop();
                _agent.SetDestination(_agent.transform.position);
            }
        };
        
        _interval.StartInterval(_updateTimeStep, onUpdateCall);
        _isMoving = true;
    }

    public void LookAtTransform(Transform Target)
    {
        Vector3 direction = (Target.position - _agent.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = lookRotation;
    }

    public void Stop()
    {
        if (!_isMoving)
            return;

        if (_interval != null && _interval.Busy)
            _interval.Stop();

        if (_animator != null)
            _animator.SetTrigger(_triggerToIdle);

        _onReachCurrentDestination = null;
        _isMoving = false;
    }

    private void OnDisable ()
    {
        Stop();
    }
    #endregion
}
