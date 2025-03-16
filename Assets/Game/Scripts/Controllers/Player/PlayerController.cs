using System;
using Game.Scripts.Enums;
using Game.Scripts.Managers.Input.Enums;
using Game.Scripts.Managers.State;
using Game.Scripts.Managers.State.Enums;
using MessagePipe;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Controllers.Player
{
    public class PlayerController : MonoBehaviour
    {
        private IDisposable _disposable;
        private StateManager _stateManager;

        [Inject]
        private void Setup(ISubscriber<GeneralEvents,object> generalEventsSubscriber,
            ISubscriber<InputEvents,object> inputEventsSubscriber,StateManager stateManager)
        {
            _stateManager = stateManager;
            var bag = DisposableBag.CreateBuilder();
            inputEventsSubscriber.Subscribe(InputEvents.OnChopBlockRequested, OnMoveRequested).AddTo(bag);
            inputEventsSubscriber.Subscribe(InputEvents.OnGameStartRequested, OnGameStartRequested).AddTo(bag);
            _disposable = bag.Build();
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
        }

        private void OnMoveRequested(object obj)
        {
            _stateManager.ChangeState(CurrentGameState.Running);
        }
        
        private void OnGameStartRequested(object obj)
        {
            _stateManager.ChangeState(CurrentGameState.Waiting);
        }
        
    }
}
