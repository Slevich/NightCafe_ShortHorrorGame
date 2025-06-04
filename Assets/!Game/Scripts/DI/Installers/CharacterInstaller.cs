using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CharacterInstaller : MonoInstaller
{
    #region Fields
    [Header("Character input."), SerializeField] private CharacterInputHandler _inputHandler;
    [Header("Items selector."), SerializeField] private ItemsSelector _itemsSelector;
    [Header("Items manager."), SerializeField] private ItemsManager _itemsManager;
    [Header("Items grabber."), SerializeField] private ItemGrabber _grabber;
    [Header("Rig weight lerper."), SerializeField] private RigWeightLerper _rigWeightLerper;
    [Header("Sounds audio player."), SerializeField] private AudioPlayer _audioPlayer;
    [Header("Interaction manager."), SerializeField] private InteractionManager _interactionManager;
    #endregion

    #region Methods
    public override void InstallBindings ()
    {
        this.Container
            .Bind<CharacterInputHandler>()
            .FromInstance(this._inputHandler)
            .AsSingle();

        this.Container
            .Bind<ItemsSelector>()
            .FromInstance(this._itemsSelector)
            .AsSingle();

        this.Container
            .Bind<ItemsManager>()
            .FromInstance(this._itemsManager)
            .AsSingle();

        this.Container
            .Bind<ItemGrabber>()
            .FromInstance(this._grabber)
            .AsSingle();

        this.Container
            .Bind<RigWeightLerper>()
            .FromInstance(this._rigWeightLerper)
            .AsSingle();

        this.Container
            .Bind<AudioPlayer>()
            .FromInstance(this._audioPlayer)
            .AsSingle();

        this.Container
            .Bind<InteractionManager>()
            .FromInstance(this._interactionManager)
            .AsSingle();
    }
    #endregion
}
