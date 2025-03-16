using System;
using DG.Tweening;
using Game.Scripts.Enums;
using Game.Scripts.Managers.Block.Mono;
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
        private Tween _moveTween;

        [Inject]
        private void Setup(ISubscriber<GeneralEvents,object> generalEventsSubscriber,
            ISubscriber<InputEvents,object> inputEventsSubscriber,StateManager stateManager)
        {
            _stateManager = stateManager;
            var bag = DisposableBag.CreateBuilder();
            generalEventsSubscriber.Subscribe(GeneralEvents.OnBlockChopped, OnBlockChopped).AddTo(bag);
            inputEventsSubscriber.Subscribe(InputEvents.OnGameStartRequested, OnGameStartRequested).AddTo(bag);
            _disposable = bag.Build();
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
            _moveTween?.Kill();
        }

        private void OnBlockChopped(object obj)
        {
            Block block = (Block)obj;
            RunToTarget(block.transform.position);
        }
        
        private void OnGameStartRequested(object obj)
        {
            _stateManager.ChangeState(CurrentGameState.Waiting);
        }

        private void RunToTarget(Vector3 position)
        {
            _stateManager.ChangeState(CurrentGameState.Running);
            Vector3 startPos = transform.position;
            Vector3 midPoint = position - Vector3.forward * (2.67322f / 2f);  
            Vector3 endPos = position;

            Vector3[] path = { startPos, midPoint, endPos };

            _moveTween = transform
                .DOPath(path, 2f, PathType.Linear, PathMode.Full3D)
                .SetSpeedBased()
                .SetLookAt(0.1f)
                .SetEase(Ease.Linear)
                .OnComplete(() => 
                {
                    transform.rotation = Quaternion.identity;  
                    _stateManager.ChangeState(CurrentGameState.Waiting);
                });
        }
        
    }
}
