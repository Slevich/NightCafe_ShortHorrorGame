using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomLightAnimations : MonoBehaviour
{
    #region Fields
    [Header("Light animations."), SerializeField] private LightAnimations[] _animations;
    #endregion

    #region Methods
    public void PlayRandomAnimation()
    {
        if (_animations == null || _animations.Length == 0)
            return;

        int randomValue = Random.Range(0, _animations.Length);
        _animations[randomValue].LerpIntensityToRandom();
    }
    #endregion
}
