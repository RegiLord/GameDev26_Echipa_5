using System.Numerics;
using AI.Blackboard;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace AI.States
{
    public class MoveToTargetState : AIState
    {
        private MovementModule movementModule;
        private Vector3 target;
        private float stopRange = 5.0f;

        private float timer = 0.0f;
        private float timeout = 1.0f;
        
        public override void EnterState()
        {
            timer = 0.0f;
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

            AttackModule attackModule;
            if (controller.blackboard.TryGet(AIKeys.AttackModule, out attackModule))
            {
                if (attackModule is null)
                    return;
                stopRange = attackModule.AttackRange * 0.95f;
                Debug.Log(stopRange);
            }
        }

        public override void UpdateState()
        {
            timer += Time.deltaTime;
            if (target == Vector3.zero || !movementModule)
            {
                ExitState(false);
                return;
            }

            Debug.Log("Unit: " + controller.transform.position +
                      "Target: " + target +
                      "Distance: " + Vector3.Distance(controller.transform.position, target));
            
            // If it has attack module use the attack modules stop range
            bool arrived = movementModule.MoveTo(target, stopRange);
            if (arrived)
                ExitState(true);

            if (timer >= timeout)
                ExitState(false);
            
        }
    }
}