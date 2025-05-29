using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class RotationByInput : MonoBehaviour
{
    #region Fields
    [Header("Rotation sensivity modifier."), SerializeField, Range(0f, 10f)] private float _rotationSensivity = 1.0f;
    [Header("Camera horizontal max offset."), SerializeField, Range(0f, 90f)] private float _horizontalRotationLimit = 60f;

    private Transform _cameraTransform;
    private Transform _bodyTransform;
    private CharacterInputHandler _inputHandler;
    private Vector2 _rotation = Vector2.zero;
    #endregion

    #region Methods
    [Inject]
    public void Construct (CharacterInputHandler InputHandler)
    {
        _inputHandler = InputHandler;
    }

    private void Awake ()
    {
        _cameraTransform = Camera.main.transform;
        _bodyTransform = transform;
    }

    private void Subscribe ()
    {
        if (_inputHandler != null)
        {
            _inputHandler.PointerDeltaCallback += (delta) => RotateByPointerDelta(delta);
        }
    }

    private void Dispose ()
    {
        if (_inputHandler != null)
        {
            _inputHandler.PointerDeltaCallback -= (delta) => RotateByPointerDelta(delta);
        }
    }

    public void RotateByPointerDelta (Vector2 PointerDelta)
    {
        if (PointerDelta == Vector2.zero)
            return;

        _rotation.x += PointerDelta.x * _rotationSensivity * Time.fixedDeltaTime;
        _rotation.y += PointerDelta.y * _rotationSensivity * Time.fixedDeltaTime;
        _rotation.y = Mathf.Clamp(_rotation.y, -_horizontalRotationLimit, _horizontalRotationLimit);

        var xQuat = Quaternion.AngleAxis(_rotation.x, Vector3.up);
        var yQuat = Quaternion.AngleAxis(_rotation.y, Vector3.left);

        _bodyTransform.localRotation = yQuat;
        _cameraTransform.localRotation = xQuat;
    }

    private void OnEnable () => Subscribe();
    private void OnDisable () => Dispose();
    #endregion
}
