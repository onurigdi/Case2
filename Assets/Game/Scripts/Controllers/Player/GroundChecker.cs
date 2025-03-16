using Game.Scripts.Config;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Controllers.Player
{
    public class GroundChecker : MonoBehaviour
    {
        private LayerMask _groundLayer;
        [SerializeField] private ParticleSystem dustVFX;
        [Inject]
        private void Setup(GameConfig _gameConfig)
        {
            _groundLayer = _gameConfig.groundLayer;
        }
        
        public bool IsGrounded()
        {
            RaycastHit hit;
            float rayDistance = 0.5f;
            Vector3 rayPos = transform.position;        
            Ray ray = new Ray(rayPos, Vector3.down);
            Physics.Raycast(ray, out hit, rayDistance, _groundLayer);
#if UNITY_EDITOR
            Debug.DrawRay(rayPos, Vector3.down * rayDistance, Color.red, 3f);            
#endif
            if (hit.collider)//if grounded play dust vfx
                dustVFX.Play();
            return hit.collider;
         
        }
    }
}
