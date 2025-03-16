using Game.Scripts.Config;
using Game.Scripts.Enums;
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
        [Header("Pool References")]
        public Block blockPrefab;
        
        [Header("Configs")]
        public GameConfig config;
        

        public override void InstallBindings()
        {
            Container.BindInstances(config);

            #region Managers
            Container.BindInterfacesAndSelfTo<BlockManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<InputManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<StateManager>().AsSingle();
            #endregion
            #region Pool Bindings

            Container.BindMemoryPool<Block, BlockPool>()
                .WithInitialSize(25) 
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
