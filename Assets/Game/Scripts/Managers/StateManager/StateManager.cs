using System;
using Game.Scripts.Enums;
using Game.Scripts.Managers.StateManager.Enums;
using MessagePipe;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Managers.StateManager
{
    public class StateManager : IInitializable
    {
        private IPublisher<GeneralEvents, object> _generalEventsPublisher;
        private static readonly Lazy<StateManager> Lazy = new(() => new StateManager());
        
        public static StateManager Instance
        {
            get { return Lazy.Value; }
        }
        
        public static CurrentGameState CurrentGameState { get; private set; }

        [Inject]
        private void Setup(IPublisher<GeneralEvents, object> generalEventsPublisher,
            ISubscriber<GeneralEvents, object> generalEventsSubscriber)
        {
            _generalEventsPublisher = generalEventsPublisher;
            
            /*var bag = DisposableBag.CreateBuilder();
            generalEventsSubscriber.Subscribe(GeneralEvents.OnLevelStarted, SetInitialReferencesAndStartLoop).AddTo(bag);
            _disposable = bag.Build();*/
        }
        public void Initialize()
        {
            ChangeState(CurrentGameState.Idle);
        }

        public void ChangeState(CurrentGameState newState)
        {
            CurrentGameState = newState;
            _generalEventsPublisher?.Publish(GeneralEvents.OnStateChanged,CurrentGameState);
            
            Debug.Log("State changed to : " + newState.ToString());
        }
    }
}
