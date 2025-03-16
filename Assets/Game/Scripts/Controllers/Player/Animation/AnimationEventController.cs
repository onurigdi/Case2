using Game.Scripts.Enums;
using Game.Scripts.Managers.Audio;
using Game.Scripts.Managers.Audio.Enums;
using Game.Scripts.Managers.State;
using Game.Scripts.Managers.State.Enums;
using MessagePipe;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Controllers.Player.Animation
{
    public class AnimationEventController : MonoBehaviour
    {
        [SerializeField] private GroundChecker leftGroundChecker;
        [SerializeField] private GroundChecker rightGroundChecker;
        [Inject] private AudioManager _audioManager;
        [Inject] private StateManager _stateManager;

        public void CheckIsGroundedRight()
        {
            if (!rightGroundChecker.IsGrounded())
            {
                _stateManager.ChangeState(CurrentGameState.Fail);
            }
            else
                _audioManager.Play(RandomFootStepEnum()); 
        }
        
        public void CheckIsGroundedLeft()
        {
            if (!leftGroundChecker.IsGrounded())
            {
                _stateManager.ChangeState(CurrentGameState.Fail);
            } 
            else
                _audioManager.Play(RandomFootStepEnum()); 
        }

        private SoundType RandomFootStepEnum()
        {
            // i didnt like this system if i have time i would change here for performance considers
            //but for case project i leave it like that for polish
            int random = Random.Range(0, 5);
            switch (random)
            {
                case 0: return SoundType.FootStep0; break;
                case 1: return SoundType.FootStep1; break;
                case 2: return SoundType.FootStep2; break;
                case 3: return SoundType.FootStep3; break;
                case 4: return SoundType.FootStep4; break;
                default: return SoundType.FootStep0; break;
            }
        }
    }
}
