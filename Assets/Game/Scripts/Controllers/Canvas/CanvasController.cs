using System;
using Game.Scripts.Enums;
using Game.Scripts.Managers.State.Enums;
using Game.Scripts.Utils.Helpers;
using MessagePipe;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Controllers.Canvas
{
    public class CanvasController : MonoBehaviour
    {
        [SerializeField] private GameObject tapToStartText;
        [SerializeField] private GameObject tapToReStartText;
        [SerializeField] private TextMeshProUGUI txtLevelInfo;
        private IDisposable _disposable;
        [Inject]
        private void Setup(ISubscriber<GeneralEvents,object> generalEventsSubscriber)
        {
            var bag = DisposableBag.CreateBuilder();
            generalEventsSubscriber.Subscribe(GeneralEvents.OnStateChanged, OnStateChanged).AddTo(bag);
            generalEventsSubscriber.Subscribe(GeneralEvents.OnLevelChanged, UpdateLevelText).AddTo(bag);
            _disposable = bag.Build();
            UpdateLevelText(null);
        }

        private void OnStateChanged(object obj)
        {
            CurrentGameState newGameState = (CurrentGameState)obj;
            tapToStartText.SetActive(newGameState == CurrentGameState.Idle || newGameState == CurrentGameState.Success);
            tapToReStartText.SetActive(newGameState == CurrentGameState.Fail);
        }

        private void UpdateLevelText(object obj)
        {
            txtLevelInfo.text = $"Level {PersistentData.Level}";
        }
    }
}
