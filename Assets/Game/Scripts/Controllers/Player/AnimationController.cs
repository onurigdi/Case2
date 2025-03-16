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
    public class AnimationController : MonoBehaviour
    {
        [SerializeField] private Animator animationController;
        private IDisposable _disposable;
        [Inject]
        private void Setup(ISubscriber<GeneralEvents,object> generalEventsSubscriber,
            ISubscriber<InputEvents,object> inputEventsSubscriber,StateManager stateManager)
        {
            var bag = DisposableBag.CreateBuilder();
            generalEventsSubscriber.Subscribe(GeneralEvents.OnStateChanged, OnStateChanged).AddTo(bag);
            _disposable = bag.Build();
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
        }

        private void OnStateChanged(object obj)
        {
            CurrentGameState newGameState = (CurrentGameState)obj;
            switch (newGameState)
            {
                case CurrentGameState.Waiting :
                case CurrentGameState.Idle : animationController.SetTrigger("Idle"); 
                    break;
                case CurrentGameState.Running : animationController.SetTrigger("Run");
                    break;
            }
        }
    }
}
