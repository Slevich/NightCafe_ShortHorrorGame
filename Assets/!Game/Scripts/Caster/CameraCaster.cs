using System;
using UnityEngine;
using UnityEngine.Events;

public class CameraCaster : MonoBehaviour
{
    #region Fields
    [Header("Ray length."), SerializeField, Range(0f, 100f)] private float _rayLength = 1f;
    [Header("Casted sphere radius."), SerializeField, Range(0f, 10f)] private float _sphereRadius = 1f;

    [Header("Raycasted objects masks."), SerializeField] private LayerMask _mask = 0;
    [Space(10), Header("Unity event on hits something with the mask."), SerializeField] private UnityEvent<GameObject> _OnHitObject;

    private ActionInterval _casterInterval;
    private float _castingTimeStep = 0.05f;
    #endregion

    #region Methods
    private void Awake ()
    {
        _casterInterval = new ActionInterval();
    }

    public void StartCasting()
    {
        if(_casterInterval != null && _casterInterval.Busy)
            return;

        Action onCastingCallback = delegate
        {
            bool hitted = Physics.SphereCast(Camera.main.transform.position, _sphereRadius, Camera.main.transform.forward, out RaycastHit hit, _rayLength, _mask);
            GameObject hittedObject = null;

            if(hitted)
            {
                hittedObject = hit.collider.gameObject;
            }

            _OnHitObject?.Invoke(hittedObject);
        };

        _casterInterval.StartInterval(_castingTimeStep, onCastingCallback);
    }

    public void StopCasting()
    {
        if (_casterInterval != null && _casterInterval.Busy)
            _casterInterval.Stop();
    }

    private void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = Camera.main.transform.position + (Camera.main.transform.forward * _rayLength);
        Gizmos.DrawWireSphere(center, _sphereRadius);
    }

    private void OnEnable () => StartCasting();

    private void OnDisable () => StopCasting();
    #endregion
}
