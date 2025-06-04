using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformSetter : MonoBehaviour
{
    [SerializeField] private Transform _transformToSet;

    public void SetTranformPositionBasedOnTransform(Transform Target)
    {
        if (_transformToSet == null)
            return;

        _transformToSet.position = Target.position;
    }

    public void RotateToTransform(Transform Target)
    {
        if (_transformToSet == null)
            return;

        Vector3 direction = (Target.position - _transformToSet.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = lookRotation;
    }
}
