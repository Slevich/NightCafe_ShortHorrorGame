using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CharacterInstaller : MonoInstaller
{
    #region Fields
    [Header("Character input."), SerializeField] private CharacterInputHandler _inputHandler;
    #endregion

    #region Methods
    public override void InstallBindings ()
    {
        this.Container
            .Bind<CharacterInputHandler>()
            .FromInstance(this._inputHandler)
            .AsSingle();
    }
    #endregion
}
