using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSetter : MonoBehaviour
{
    #region Fields
    private Animator _animator;
    private string _lastParameterName;
    #endregion

    #region Methods
    private void Awake ()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetParameterName(string ParameterName)
    {
        _lastParameterName = ParameterName;
    }

    public void SetFloat(float NewValue)
    {
        if (_animator == null)
            return;

        _animator.SetFloat(_lastParameterName, NewValue);
    }
    #endregion
}
