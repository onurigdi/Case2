using System;
using Game.Scripts.Enums;
using Game.Scripts.Managers.State.Enums;
using MessagePipe;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Controllers.Canvas
{
    public class CanvasController : MonoBehaviour
    {
        [SerializeField] private GameObject tapToStartText;
        [SerializeField] private GameObject tapToReStartText;
        private IDisposable _disposable;
        [Inject]
        private void Setup(ISubscriber<GeneralEvents,object> generalEventsSubscriber)
        {
            var bag = DisposableBag.CreateBuilder();
            generalEventsSubscriber.Subscribe(GeneralEvents.OnStateChanged, OnStateChanged).AddTo(bag);
            _disposable = bag.Build();
        }

        private void OnStateChanged(object obj)
        {
            CurrentGameState newGameState = (CurrentGameState)obj;
            tapToStartText.SetActive(newGameState == CurrentGameState.Idle || newGameState == CurrentGameState.Success);
            tapToReStartText.SetActive(newGameState == CurrentGameState.Fail);
        }
    }
}
