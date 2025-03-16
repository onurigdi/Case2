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
        
        private Tween _pingPongTween;
        private PingPongDir _pingPongDir;
        private Material _activeMaterial;

        [Inject] private GameConfig _gameConfig;
        [Inject] private BlockPool _blockPool;
        public void Reset()
        {
            rb.isKinematic = true;
            _pingPongTween?.Kill();
            transform.rotation = Quaternion.identity;
        }

        private void OnDisable()
        {
            _pingPongTween?.Kill();
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
        
        public Material GetActiveMaterial()
        {
            return _activeMaterial;
        }
        
        public void StartPingPong()
        {
            StopPingPong();
            int startxPosition = Random.value > 0.5f ? 5 : -5;
            transform.position = VectorHelper.GetVectorWith(VectorHelper.Vector3Coord.x,transform.position,startxPosition);
            float speed = Mathf.Clamp( _gameConfig.slowestPingPongSpeed - (PersistentData.Level * 0.2f), 1, _gameConfig.slowestPingPongSpeed);
            _pingPongTween = transform.DOMoveX(startxPosition * -1, speed).
                SetLoops(-1,LoopType.Yoyo).SetUpdate(UpdateType.Fixed).SetEase(Ease.Linear);
        }

        public void DropBlock()
        {
            StopPingPong();
            rb.isKinematic = false;
            DOVirtual.DelayedCall(3, () =>
            {
                _blockPool.Despawn(this);
            });
        }
        
        public void StopPingPong()
        {
            _pingPongTween?.Kill();
        }
        
    }
}
