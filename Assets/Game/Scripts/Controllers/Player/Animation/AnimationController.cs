using System;
using Game.Scripts.Enums;
using Game.Scripts.Managers.Input.Enums;
using Game.Scripts.Managers.State;
using Game.Scripts.Managers.State.Enums;
using MessagePipe;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Controllers.Player.Animation
{
    public class AnimationController : MonoBehaviour
    {
        [SerializeField] private Animator animationController;
        private Rigidbody[] _rbBodies;
        private IDisposable _disposable;
        private CurrentGameState _oldState;
        [Inject]
        private void Setup(ISubscriber<GeneralEvents,object> generalEventsSubscriber,
            ISubscriber<InputEvents,object> inputEventsSubscriber,StateManager stateManager)
        {
            var bag = DisposableBag.CreateBuilder();
            generalEventsSubscriber.Subscribe(GeneralEvents.OnStateChanged, OnStateChanged).AddTo(bag);
            _disposable = bag.Build();
            _rbBodies = GetComponentsInChildren<Rigidbody>();
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
        }

        private void OnStateChanged(object obj)
        {
            CurrentGameState newGameState = (CurrentGameState)obj;
            if (_oldState == newGameState)
                return;
            
            SetKinematic(newGameState != CurrentGameState.Fail);//handles ragdoll
            
            switch (newGameState)
            {
                case CurrentGameState.Waiting :
                case CurrentGameState.Idle : animationController.SetTrigger("Idle"); 
                    break;
                case CurrentGameState.Running : animationController.SetTrigger("Run");
                    break;
            }

            _oldState = newGameState;
        }
        
        void SetKinematic(bool newValue)
        {
            animationController.enabled = newValue;
            Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in bodies)
            {
                rb.isKinematic = newValue;
            }
        }
    }
}
