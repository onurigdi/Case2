using Game.Scripts.Enums;
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
        [Inject] private StateManager _stateManager;

        public void CheckIsGroundedRight()
        {
            if (!rightGroundChecker.IsGrounded())
            {
                _stateManager.ChangeState(CurrentGameState.Fail);
            }
        }
        
        public void CheckIsGroundedLeft()
        {
            if (!leftGroundChecker.IsGrounded())
            {
                _stateManager.ChangeState(CurrentGameState.Fail);
            } 
        }
    }
}
