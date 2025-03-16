using System;
using System.Collections.Generic;
using Game.Scripts.Config;
using Game.Scripts.Controllers.Player;
using Game.Scripts.Enums;
using Game.Scripts.Managers.Audio;
using Game.Scripts.Managers.Audio.Enums;
using Game.Scripts.Managers.Input.Enums;
using Game.Scripts.Managers.State.Enums;
using Game.Scripts.Pools;
using Game.Scripts.Utils.Helpers;
using MessagePipe;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Game.Scripts.Managers.Block
{
    public class BlockManager : IDisposable,IInitializable
    {
        
        private IDisposable _disposable;
        [Inject] private ISubscriber<InputEvents,object> _inputEventsSubscriber;
        [Inject] private ISubscriber<GeneralEvents,object> _generalEventsSubscriber;
        [Inject] private IPublisher<GeneralEvents,object> _generalEventsPublisher;
        [Inject] private GameConfig _gameConfig;
        [Inject] private BlockPool _blockPool;
        [Inject] private PlayerController _playerController;
        [Inject] private AudioManager _audioManager;
        private Vector3 _lastBlockPosition;
        private Mono.Block _lastBlock;
        private Mono.Block _previousBlock;
        private int _perfectScoreCounter;

        private int _blockCountToWin;
        private Mono.Block _activeFinishBlock;
        public Mono.Block ActiveFinishBlock => _activeFinishBlock;
        private List<Mono.Block> _choppedBlocks = new List<Mono.Block>();

        public void Initialize()
        {
            GenerateInitialBlocks();// initially spawn 3 block
            var bag = DisposableBag.CreateBuilder();
            _inputEventsSubscriber.Subscribe(InputEvents.OnChopBlockRequested, OnChopBlockRequested).AddTo(bag);
            _inputEventsSubscriber.Subscribe(InputEvents.OnGameStartRequested, OnGameStartRequested).AddTo(bag);
            _inputEventsSubscriber.Subscribe(InputEvents.OnGameRestartRequested, OnGameRestartRequested).AddTo(bag);
            _generalEventsSubscriber.Subscribe(GeneralEvents.OnStateChanged, OnStateChanged).AddTo(bag);
            _disposable = bag.Build();
        }

        private void OnChopBlockRequested(object obj)
        {
            StopAndChopBlock();
            SpawnNextBlock();
        }
        
        private void OnGameStartRequested(object obj)
        {
            ResetCombo();
            SpawnNextBlock();
        }
        
        private void OnGameRestartRequested(object obj)
        {
            ResetCombo();
            RestartGame();
        }

        private void OnStateChanged(object obj)
        {
            if ((CurrentGameState)obj == CurrentGameState.Success)
            {
                //skip finishline block z position
                _lastBlockPosition = VectorHelper.AddVectorWith(VectorHelper.Vector3Coord.z,_lastBlockPosition,_gameConfig.stackLength);
                RandomGenerateNewLevel();
            }
        }

        private void RestartGame()
        {
            _blockPool.DespawnAllActiveBlocks();
            //all blocks despawned on restart.No need to despawn previous level blocks again
            _choppedBlocks.Clear();
            _lastBlockPosition = Vector3.zero;
            _lastBlock = null;
            _previousBlock = null;
            for (int i = 0; i < 3; i++)
            {
                SpawnBlock(0);
            }
            
            RandomGenerateNewLevel();
        }
        private void SpawnNextBlock()
        {
            // if last Block out of previous one and dropped totally dont spawn new. wait for player to fall
            if (_lastBlock.IsBlockDropped || _lastBlock.IsLastBlockToWin) 
                return;
            //spawn new blocks if level did not finish yet
            if (_choppedBlocks.Count < _blockCountToWin)
            {
                SpawnBlock(0);
                _lastBlock?.StartPingPong();
                _choppedBlocks.Add(_lastBlock);
                //flag the block as its last block
                if (_choppedBlocks.Count == _blockCountToWin)
                    _lastBlock?.SetLastBlockToWin();
            }
        }

        private void GenerateInitialBlocks()
        {
            for (int i = 0; i < 3; i++)
            {
                SpawnBlock(0);
            }

            RandomGenerateNewLevel();
        }
        
        //this method select random total block count to finish level depends on player level randomly
        //and create FinishLine Block
        private void RandomGenerateNewLevel()
        {
            ClearOldLevelBlocks();
            _blockCountToWin = Random.Range(5 + PersistentData.Level, 8 + PersistentData.Level);
            
            // (LevelMaxStackCount + 1) mean total blockcount + 1 for finishblock
            Vector3 finishPos = new Vector3(0, 0, (_playerController.transform.position.z) + ((_blockCountToWin + 1) * _gameConfig.stackLength));
            _activeFinishBlock = _blockPool.SpawnBlock();
            _activeFinishBlock.SetMaterial(_gameConfig.finishLineMaterial);
            _activeFinishBlock.SetPosition(finishPos);
            _activeFinishBlock.SetScale(_gameConfig.defaultStackScale);//finishline always default size
            _activeFinishBlock.SetAsFinishBlock();
        }

        private void ClearOldLevelBlocks()
        {
            foreach (var block in _choppedBlocks)
            {
                _blockPool.DeSpawnBlock(block);
            }
            _choppedBlocks.Clear();
        }
        
        

        private void SpawnBlock(float xPosition)
        {
            _previousBlock = _lastBlock;
            _lastBlock = _blockPool.SpawnBlock();
            _lastBlockPosition = VectorHelper.GetVectorWith(VectorHelper.Vector3Coord.x,_lastBlockPosition,xPosition);
            _lastBlockPosition = VectorHelper.AddVectorWith(VectorHelper.Vector3Coord.z,_lastBlockPosition,_gameConfig.stackLength);
            
            _lastBlock.SetPosition(_lastBlockPosition);
            _lastBlock.SetScale(_previousBlock ? _previousBlock.transform.lossyScale : _gameConfig.defaultStackScale);
            _lastBlock.SetMaterial(_gameConfig.GetRandomMaterial());
        }
        
        void StopAndChopBlock()
        {
            if (_choppedBlocks.Count == _blockCountToWin && _lastBlock.IsChopped)
                return;
            
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
                ResetCombo();
                ChopStack();
            }
            
            _generalEventsPublisher?.Publish(GeneralEvents.OnBlockChopped,_lastBlock);
        }

        private void ResetCombo()
        {
            _perfectScoreCounter = 0;
        }
        
        private void ChopStack()
        {
            _audioManager.Play(SoundType.ChopStack); 
            VectorHelper.Vector3Coord axis = VectorHelper.Vector3Coord.x;

            //GameManager.instance.SoundManager.PlaySplitSFX();

            Material oldMat = _lastBlock.GetActiveMaterial();
        
            float baseBlockPos = _previousBlock.transform.position.x;
            float baseBlockScale = _previousBlock.transform.lossyScale.x;
            float currentBlockPos = _lastBlock.transform.position.x;
        
            float relDist = Mathf.Abs(baseBlockPos - currentBlockPos);
        
            float remainingBlockScale = baseBlockScale - relDist;

            Mono.Block leftoverBlockPiece = _blockPool.SpawnBlock();
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
            _lastBlock.SetChopped();
            //baseBlock = currentBlock.transform;
        }


        void PerfectScore()
        {
            _lastBlock.SetChopped();
            _audioManager.PlayPitchIncreased(SoundType.BestFit,1 + (_perfectScoreCounter * 0.05f)); 
            _perfectScoreCounter++;
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}
