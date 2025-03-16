using Cinemachine;
using Game.Scripts.Controllers.Player;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Controllers.Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera vCam;
        private PlayerController _playerController;

        [Inject]
        public void Setup(PlayerController playerController)
        {
            _playerController = playerController;
            AssignPlayerToFollow();
        }

        private void AssignPlayerToFollow()
        {
            vCam.Follow = _playerController.transform;
        }
    }
}
