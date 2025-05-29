using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public sealed class SceneInstaller : MonoInstaller
{
    #region Fields
    [Header("Input.")] 
    [SerializeField] private InputHandlerDisplay _inputDisplay;


    #endregion

    //[Inject]
    //public void Construct()
    //{

    //}

    public override void InstallBindings ()
    {
        this.Container
            .Bind<InputHandlerDisplay>()
            .FromInstance(this._inputDisplay)
            .AsSingle();
    }
}
