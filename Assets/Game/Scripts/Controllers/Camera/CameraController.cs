using System;
using Cinemachine;
using Game.Scripts.Controllers.Player;
using Game.Scripts.Enums;
using Game.Scripts.Managers.State.Enums;
using MessagePipe;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Controllers.Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera vCam;
        [SerializeField] private CinemachineFreeLook vCamOrbital;
        private PlayerController _playerController;
        private IDisposable _disposable;
        private IDisposable _disposableOrbital;
        [Inject]
        public void Setup(PlayerController playerController,ISubscriber<GeneralEvents,object> generalEventSubscriber )
        {
            _playerController = playerController;
            var bag = DisposableBag.CreateBuilder();
            generalEventSubscriber.Subscribe(GeneralEvents.OnStateChanged, OnStateChanged).AddTo(bag);
            _disposable = bag.Build();
            AssignPlayerToFollow();
        }

        private void OnDisable()
        {
            _disposableOrbital?.Dispose();
            _disposable?.Dispose();
        }

        private void AssignPlayerToFollow()
        {
            vCam.Follow = _playerController.transform;
            vCamOrbital.Follow = _playerController.transform;
            vCamOrbital.LookAt = _playerController.transform;
        }

        private void OnStateChanged(object obj)
        {
            CurrentGameState newState = (CurrentGameState)obj;
            if (newState == CurrentGameState.Success)
                StartOrbitalCamera();
            else
                StopOrbitalCamera();
        }

        private void StartOrbitalCamera()
        { 
          
            vCam.gameObject.SetActive(false);
            vCamOrbital.gameObject.SetActive(true);  
            _disposableOrbital?.Dispose();
            _disposableOrbital = Observable.EveryUpdate().TakeUntilDisable(this).Subscribe((long l) =>
            {
                vCamOrbital.m_XAxis.Value += 40f * Time.deltaTime;
            });
            

        }

        private void StopOrbitalCamera()
        {
            vCam.gameObject.SetActive(true);
            vCamOrbital.gameObject.SetActive(false);  
            _disposableOrbital?.Dispose();
        }
        

    }
}
