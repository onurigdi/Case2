using System;
using DG.Tweening;
using Game.Scripts.Config;
using Game.Scripts.Managers.Block.Enums;
using Game.Scripts.Pools;
using Game.Scripts.Utils.Helpers;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Game.Scripts.Managers.Block.Mono
{
    public class Block : MonoBehaviour
    {
        [SerializeField] private MeshRenderer mr;
        [SerializeField] private Rigidbody rb;
        
        private bool _chopped;
        public  bool IsChopped => _chopped;
        private bool _isFinishLine;
        public bool IsFinishLine => _isFinishLine;
        private bool _isLastBlock;
        public bool IsLastBlockToWin => _isLastBlock;
        private bool _blockDropped;
        public bool IsBlockDropped => _blockDropped;
        
        private Tween _pingPongTween;
        private Tween _delayTween;
        private PingPongDir _pingPongDir;
        private Material _activeMaterial;

        [Inject] private GameConfig _gameConfig;
        [Inject] private BlockPool _blockPool;
        public void Reset()
        {
            _chopped = false;
            _isFinishLine = false;
            _isLastBlock = false;
            _blockDropped = false;
            rb.isKinematic = true;
            _pingPongTween?.Kill();
            transform.rotation = Quaternion.identity;
        }

        private void OnDisable()
        {
            _pingPongTween?.Kill();
            _delayTween?.Kill();
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void SetScale(Vector3 scale)
        {
            transform.localScale = scale;
        }
        
        public void SetMaterial(Material newMat)
        {
        
            _activeMaterial = newMat;
            mr.material = _activeMaterial;
        }
        
        public void SetLastBlockToWin()
        {
            _isLastBlock = true;
        }
        
        public void SetAsFinishBlock()
        {
            _isFinishLine = true;
        }
        
        public void SetChopped()
        {
            _chopped = true;
        }
        
        public Material GetActiveMaterial()
        {
            return _activeMaterial;
        }
        
        public void StartPingPong()
        {
            StopPingPong();
            int startxPosition = Random.value > 0.5f ? 5 : -5;
            transform.position = VectorHelper.GetVectorWith(VectorHelper.Vector3Coord.x,transform.position,startxPosition);
            float speed = Mathf.Clamp( _gameConfig.slowestPingPongSpeed - (PersistentData.Level * 0.2f), 0.5f, _gameConfig.slowestPingPongSpeed);
            _pingPongTween = transform.DOMoveX(startxPosition * -1, speed).
                SetLoops(-1,LoopType.Yoyo).SetUpdate(UpdateType.Fixed).SetEase(Ease.Linear);
        }

        public void DropBlock()
        {
            _blockDropped = true;
            StopPingPong();
            rb.isKinematic = false;
            _delayTween?.Kill();
            _delayTween = DOVirtual.DelayedCall(3, () =>
            {
                _blockPool.DeSpawnBlock(this);
            });
        }
        
        public void StopPingPong()
        {
            _pingPongTween?.Kill();
        }
    }
}
