using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Game.Scripts.Config;
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
        private GameConfig _gameConfig;

        [Inject]
        private void Setup(ISubscriber<GeneralEvents,object> generalEventsSubscriber,
            ISubscriber<InputEvents,object> inputEventsSubscriber,
            StateManager stateManager,
            GameConfig gameConfig)
        {
            _gameConfig = gameConfig;
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
            Vector3 midPoint = position - Vector3.forward * (_gameConfig.stackLength / 2f);  
            Vector3 endPos = position;

            List<Vector3> newPath = new List<Vector3>();
            newPath.Add(startPos); //update start pos
            
            if (_moveTween != null && _moveTween.IsActive())  //if player moving actively
            {
                // get the old path points where player didnt already pass
                newPath.AddRange(_moveTween.PathGetDrawPoints().ToList().FindAll(x=> x.z > startPos.z));  
                _moveTween.Kill();
                
            }
            
            newPath.Add(midPoint);
            newPath.Add(endPos);

            _moveTween?.Kill();
            _moveTween = transform
                .DOPath(newPath.ToArray(), 2f, PathType.Linear, PathMode.Full3D)
                .SetSpeedBased()
                .SetLookAt(0.1f)
                .SetEase(Ease.Linear)
                .OnComplete(() => 
                {
                    if (_stateManager.CurrentGameState != CurrentGameState.Fail)
                    {
                        transform.rotation = Quaternion.identity;  
                        _stateManager.ChangeState(CurrentGameState.Waiting);  
                    }
                });
        }
        
    }
}
