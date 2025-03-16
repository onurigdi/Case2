using System;
using Game.Scripts.Enums;
using Game.Scripts.Managers.Input.Enums;
using Game.Scripts.Managers.State.Enums;
using MessagePipe;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Managers.State
{
    public class StateManager : IDisposable
    {
        private IPublisher<GeneralEvents, object> _generalEventsPublisher;
        private IDisposable _disposable;

        public static CurrentGameState CurrentGameState { get; private set; }

        [Inject]
        private void Setup(IPublisher<GeneralEvents, object> generalEventsPublisher,
            ISubscriber<GeneralEvents, object> generalEventsSubscriber)
        {
            _generalEventsPublisher = generalEventsPublisher;
            ChangeState(CurrentGameState.Idle);
            
        }
        public void ChangeState(CurrentGameState newState)
        {
            CurrentGameState = newState;
            _generalEventsPublisher?.Publish(GeneralEvents.OnStateChanged,CurrentGameState);
            
            Debug.Log("State changed to : " + newState.ToString());
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}
