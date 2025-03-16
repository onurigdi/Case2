using System;
using Game.Scripts.Config;
using Game.Scripts.Enums;
using Game.Scripts.Managers.Input.Enums;
using Game.Scripts.Managers.State.Enums;
using Game.Scripts.Pools;
using Game.Scripts.Utils.Helpers;
using MessagePipe;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Managers.Block
{
    public class BlockManager : IDisposable,IInitializable
    {
        
        private IDisposable _disposable;
        [Inject] private ISubscriber<InputEvents,object> _inputEventsSubscriber;
        [Inject] private IPublisher<GeneralEvents,object> _generalEventsPublisher;
        [Inject] private GameConfig _gameConfig;
        [Inject] private BlockPool _blockPool;
        private Vector3 _lastBlockPosition;
        private Mono.Block _lastBlock;
        private Mono.Block _previousBlock;

        public void Initialize()
        {
            GenerateInitialBlocks();// initially spawn 3 block
            var bag = DisposableBag.CreateBuilder();
            _inputEventsSubscriber.Subscribe(InputEvents.OnChopBlockRequested, OnChopBlockRequested).AddTo(bag);
            _inputEventsSubscriber.Subscribe(InputEvents.OnGameStartRequested, OnGameStartRequested).AddTo(bag);
            _disposable = bag.Build();
        }

        private void OnChopBlockRequested(object obj)
        {
            StopAndChopBlock();
            SpawnNextBlock();
        }
        
        private void OnGameStartRequested(object obj)
        {
            SpawnNextBlock();

            /*IDisposable asd = Observable.Interval(TimeSpan.FromSeconds(0.9f)) test perfect combo
                .Subscribe((long l) => OnChopBlockRequested(null));*/
        }

        private void SpawnNextBlock()
        {
            SpawnBlock(0);
            _lastBlock?.StartPingPong();
        }

        private void GenerateInitialBlocks()
        {
            for (int i = 0; i < 3; i++)
            {
                SpawnBlock(0);
            }
        }

        private void SpawnBlock(float xPosition)
        {
            _previousBlock = _lastBlock;
            _lastBlock = _blockPool.Spawn();
            _lastBlockPosition = VectorHelper.GetVectorWith(VectorHelper.Vector3Coord.x,_lastBlockPosition,xPosition);
            _lastBlockPosition = VectorHelper.AddVectorWith(VectorHelper.Vector3Coord.z,_lastBlockPosition,_gameConfig.stackLength);
            
            _lastBlock.SetPosition(_lastBlockPosition);
            _lastBlock.SetScale(_previousBlock ? _previousBlock.transform.lossyScale : _gameConfig.defaultStackScale);
            _lastBlock.SetMaterial(_gameConfig.testMaterial);
        }
        
        void StopAndChopBlock()
        {
            _lastBlock.StopPingPong();
            
            Vector3 currentBlockPosVector = VectorHelper.GetXVector(_lastBlock.transform.position);
            Vector3 preciousBlockPosVector = VectorHelper.GetXVector(_previousBlock.transform.position);

            float gameOverDist = _lastBlock.transform.lossyScale.x * 0.5f + _previousBlock.transform.lossyScale.x * 0.5f;

            
            //if out of previous block then drop it
            if (Vector3.Distance(currentBlockPosVector, preciousBlockPosVector) > gameOverDist)
            {
                _lastBlock.DropBlock();
            }
            else if (Vector3.Distance(currentBlockPosVector, preciousBlockPosVector) <_gameConfig.perfectDist)
            {
                PerfectScore();            
            }
            else
            {
                //perfectScoreCounter = 0;
                ChopStack();
            }
            
            _generalEventsPublisher?.Publish(GeneralEvents.OnBlockChopped,_lastBlock);
        }
        
        private void ChopStack()
        {
            VectorHelper.Vector3Coord axis = VectorHelper.Vector3Coord.x;

            //GameManager.instance.SoundManager.PlaySplitSFX();

            Material oldMat = _lastBlock.GetActiveMaterial();
        
            float baseBlockPos = _previousBlock.transform.position.x;
            float baseBlockScale = _previousBlock.transform.lossyScale.x;
            float currentBlockPos = _lastBlock.transform.position.x;
        
            float relDist = Mathf.Abs(baseBlockPos - currentBlockPos);
        
            float remainingBlockScale = baseBlockScale - relDist;

            Mono.Block leftoverBlockPiece = _blockPool.Spawn();
            leftoverBlockPiece.SetPosition(_lastBlock.transform.position);
            leftoverBlockPiece.SetMaterial(oldMat);

            int sign = currentBlockPos < baseBlockPos ? -1 : 1;

            float remainingBlockPos = (baseBlockPos + sign * baseBlockScale * 0.5f) - sign * remainingBlockScale * 0.5f;
            float leftoverBlockScale = relDist;
            float leftoverBlockPos = remainingBlockPos + sign * remainingBlockScale * 0.5f + sign * leftoverBlockScale * 0.5f;

            leftoverBlockPiece.SetScale(VectorHelper.GetVectorWith(axis, _lastBlock.transform.lossyScale, leftoverBlockScale));
            leftoverBlockPiece.SetPosition(VectorHelper.GetVectorWith(axis, _lastBlock.transform.position, leftoverBlockPos));
            leftoverBlockPiece.DropBlock();
        
            _lastBlock.SetPosition(VectorHelper.GetVectorWith(axis, _lastBlock.transform.position, remainingBlockPos));
            _lastBlock.SetScale(VectorHelper.GetVectorWith(axis, _lastBlock.transform.lossyScale, remainingBlockScale));        
            //baseBlock = currentBlock.transform;
        }


        void PerfectScore()
        {
           /* perfectScoreCounter++;
            GameManager.instance.SoundManager.PlayPerfectSFX(perfectScoreCounter);*/
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}
