using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class RotationByInput : MonoBehaviour
{
    #region Fields
    [field: Header("Horizontal rotation sensivity modifier."), SerializeField, Range(0f, 100f)] public float HorizontalRotationSensivity { get; set; } = 1.0f;
    [Header("Local axis of the horizontal rotation (camera)."), SerializeField] public Axes _horizontalAxis = Axes.X;

    [field: Header("Vertical rotation sensivity modifier."), SerializeField, Range(0f, 100f)] public float VerticalRotationSensivity { get; set; } = 1.0f;
    [Header("Local axis of the vertical rotation (this transform)."), SerializeField] public Axes _verticalAxis = Axes.Y;

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

        float newVerticalAngle = PointerDelta.x * Time.fixedDeltaTime * VerticalRotationSensivity;
        _rotation.y += newVerticalAngle;

        float newHorizontalAngle = -PointerDelta.y * Time.fixedDeltaTime * HorizontalRotationSensivity;
        _rotation.x += newHorizontalAngle;
        _rotation.x = Mathf.Clamp(_rotation.x, -_horizontalRotationLimit, _horizontalRotationLimit);

        Vector3 verticalRotationAxis = AxesSelector.ReturnVector(_verticalAxis, (onXSelection: Vector3.right, onYSelection: Vector3.up, onZSelection: Vector3.forward));
        Quaternion verticalRotation = Quaternion.AngleAxis(_rotation.y, verticalRotationAxis);

        Vector3 horizontalRotationAxis = AxesSelector.ReturnVector(_horizontalAxis, (onXSelection: Vector3.right, onYSelection: Vector3.up, onZSelection: Vector3.forward));
        Quaternion horizontalRotation = Quaternion.AngleAxis(_rotation.x, horizontalRotationAxis);

        _bodyTransform.localRotation = verticalRotation;
        _cameraTransform.localRotation = horizontalRotation;
    }

    private void OnEnable () => Subscribe();
    private void OnDisable () => Dispose();
    #endregion
}
