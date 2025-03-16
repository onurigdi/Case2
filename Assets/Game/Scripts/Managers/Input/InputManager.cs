using System;
using Game.Scripts.Enums;
using Game.Scripts.Managers.Input.Enums;
using Game.Scripts.Managers.State;
using Game.Scripts.Managers.State.Enums;
using MessagePipe;
using UniRx;
using Zenject;

namespace Game.Scripts.Managers.Input
{
    public class InputManager : IDisposable
    {
        private IDisposable _disposable;
        private IDisposable _inputDisposable;
        private IPublisher<InputEvents,object> _inputEventsPublisher;


        [Inject]
        private void Setup(
            ISubscriber<GeneralEvents,object> generalEventsSubscriber,
            IPublisher<InputEvents,object> inputEventsPublisher)
        {
            _inputEventsPublisher = inputEventsPublisher;
            var bag = DisposableBag.CreateBuilder();
            generalEventsSubscriber.Subscribe(GeneralEvents.OnStateChanged, OnStateChanged);
            _disposable = bag.Build();
        }
        public void Dispose()
        {
            _disposable?.Dispose();
            _inputDisposable?.Dispose();
        }

        private void OnStateChanged(object obj)
        {
            CurrentGameState newState = (CurrentGameState)obj;
            if (newState == CurrentGameState.Waiting || newState == CurrentGameState.Idle || newState == CurrentGameState.Running)
                StartInputListener();
            else
                StopInputListener();
        }
        
        private void StartInputListener()
        {
            _inputDisposable?.Dispose();
            _inputDisposable = Observable.EveryUpdate().Subscribe((long l) =>
            {
                if (UnityEngine.Input.GetMouseButtonDown(0)) //old input system
                {
                    switch (StateManager.CurrentGameState)
                    {
                        case CurrentGameState.Idle :
                            _inputEventsPublisher?.Publish(InputEvents.OnGameStartRequested,null);
                            break;
                        case CurrentGameState.Running:
                        case CurrentGameState.Waiting:  
                            _inputEventsPublisher?.Publish(InputEvents.OnChopBlockRequested,null);
                            break;
                    }
                }
                     
            });
        }
        
        private void StopInputListener()
        {
            _inputDisposable?.Dispose();
        }
    }
}
