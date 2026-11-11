using AI.Blackboard;
using Unity.VisualScripting;
using UnityEngine;

namespace AI.States
{
    public class MoveToTargetState : AIState
    {
        private MovementModule movementModule;
        private Transform target;
        
        public override void EnterState()
        {
            if (movementModule is null && !controller.blackboard.TryGet(AIKeys.MovementModule, out movementModule))
            {
                Debug.LogError("AI Controller: Failed to find movement module");
                return;
            }

            if (!controller.blackboard.TryGet(AIKeys.Target, out target))
            {
                Debug.LogError("AI Controller: Failed to find target");
                return;
            }
        }

        public override void UpdateState()
        {
            if (!target || !movementModule)
            {
                ExitState(false);
                return;
            }
            
            bool arrived = movementModule.MoveTo(target.position, 2.0f);
            if (arrived)
                ExitState(true);
        }
        
        
    }
}