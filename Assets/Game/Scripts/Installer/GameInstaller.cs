using Game.Scripts.Enums;
using Game.Scripts.Managers.StateManager;
using MessagePipe;
using Zenject;
using Game.Scripts.Controllers.Stack.Mono;
using Game.Scripts.Pools;
using UnityEngine;

namespace Game.Scripts.Installer
{
    public class GameInstaller : MonoInstaller
    {
        [Header("Pool References")]
        public Stack stackPrefab;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<StateManager>().AsSingle().NonLazy();
            #region Pool Bindings

            Container.BindMemoryPool<Stack, StackPool>()
                .WithInitialSize(25) 
                .FromComponentInNewPrefab(stackPrefab) 
                .UnderTransformGroup("StackPool");             

            #endregion
            
            #region MessagePipeOptions
            var options = Container.BindMessagePipe();
            Container.BindMessageBroker<GeneralEvents,object>(options);
            #endregion 
        }
    }
}
