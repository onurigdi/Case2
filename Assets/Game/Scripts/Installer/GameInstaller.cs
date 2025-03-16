using System.Collections.Generic;
using Game.Scripts.Config;
using Game.Scripts.Controllers.Player;
using Game.Scripts.Enums;
using Game.Scripts.Managers.Audio;
using Game.Scripts.Managers.Audio.Models;
using Game.Scripts.Managers.Block;
using Game.Scripts.Managers.Block.Mono;
using Game.Scripts.Managers.Input;
using Game.Scripts.Managers.Input.Enums;
using Game.Scripts.Managers.State;
using MessagePipe;
using Zenject;
using Game.Scripts.Pools;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts.Installer
{
    public class GameInstaller : MonoInstaller
    {
        [Header("References")]
        public Block blockPrefab;
        public PlayerController playerController;
        [SerializeField] private List<SoundClip> audioClips;
        
        [Header("Configs")]
        public GameConfig config;
        

        public override void InstallBindings()
        {
            Container.BindInstances(config);
            Container.BindInstances(audioClips);
            Container.Bind<PlayerController>().FromInstance(playerController).AsSingle();
            #region Managers
            Container.BindInterfacesAndSelfTo<AudioManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<BlockManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<InputManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<StateManager>().AsSingle();
            #endregion
            #region Pool Bindings

            Container.BindMemoryPool<Block, BlockPool>()
                .WithInitialSize(40) 
                .FromComponentInNewPrefab(blockPrefab) 
                .UnderTransformGroup("BlockPool");             

            #endregion
            
            
            #region MessagePipeOptions
            var options = Container.BindMessagePipe();
            Container.BindMessageBroker<GeneralEvents,object>(options);
            Container.BindMessageBroker<InputEvents,object>(options);
            #endregion 
        }
    }
}
