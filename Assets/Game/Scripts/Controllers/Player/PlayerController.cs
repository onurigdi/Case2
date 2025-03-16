using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Game.Scripts.Config;
using Game.Scripts.Enums;
using Game.Scripts.Managers.Block;
using Game.Scripts.Managers.Block.Mono;
using Game.Scripts.Managers.Input.Enums;
using Game.Scripts.Managers.State;
using Game.Scripts.Managers.State.Enums;
using Game.Scripts.Utils.Helpers;
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
        private BlockManager _blockManager;
        private Vector3 _initialPosition;

        [Inject]
        private void Setup(ISubscriber<GeneralEvents,object> generalEventsSubscriber,
            ISubscriber<InputEvents,object> inputEventsSubscriber,
            StateManager stateManager,
            GameConfig gameConfig,
            BlockManager blockManager)
        {
            _blockManager = blockManager;
            _gameConfig = gameConfig;
            _stateManager = stateManager;
            var bag = DisposableBag.CreateBuilder();
            generalEventsSubscriber.Subscribe(GeneralEvents.OnBlockChopped, OnBlockChopped).AddTo(bag);
            inputEventsSubscriber.Subscribe(InputEvents.OnGameStartRequested, OnGameStartRequested).AddTo(bag);
            inputEventsSubscriber.Subscribe(InputEvents.OnGameRestartRequested, OnGameRestartRequested).AddTo(bag);
            _disposable = bag.Build();
            _initialPosition = transform.position;
        }

        private void OnDisable()
        {
            _disposable?.Dispose();
            _moveTween?.Kill();
        }

        private void OnBlockChopped(object obj)
        {
            Block block = (Block)obj;
            RunToTarget(block);
        }
        
        private void OnGameStartRequested(object obj)
        {
            _stateManager.ChangeState(CurrentGameState.Waiting);
        }

        private void OnGameRestartRequested(object obj)
        {
            _moveTween?.Kill();
            transform.position = _initialPosition;
            _stateManager.ChangeState(CurrentGameState.Idle);
        }

        private void RunToTarget(Block newTargetBlock)
        {
            Vector3 position = newTargetBlock.transform.position;
            //if block out of previous block and dropped totally then do not turn forward to it. go players forward
            //or if its finish line no need to bend to platform middle the pathway. just go forward
            if (newTargetBlock.IsBlockDropped || newTargetBlock.IsFinishLine)
                position = VectorHelper.GetVectorWith(VectorHelper.Vector3Coord.x,position,transform.position.x);
            
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
                        if (newTargetBlock.IsLastBlockToWin)//if last block to win
                        {
                            RunToTarget(_blockManager.ActiveFinishBlock);// move it to finish block
                        }
                        else if (newTargetBlock.IsFinishLine)//if finish line then level win
                        {
                            _stateManager.ChangeState(CurrentGameState.Success);
                        }
                        else//else not finished yet change state to waiting
                            _stateManager.ChangeState(CurrentGameState.Waiting);
                        
                          
                    }
                });
        }
        
    }
}
