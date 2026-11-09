using AI.Blackboard;
using UnityEngine;

namespace AI.States
{
    public class AttackTargetState : AIState
    {
        public AttackTargetState(AIController controller) : base(controller) { }

        
        public override void EnterState()
        {
            controller.blackboard.TryGet(AIKeys.Target, out Transform target);
            controller.blackboard.TryGet(AIKeys.AttackModule, out AttackModule attackModule);
            
            
            if (target && attackModule)
                attackModule.AttackTarget(target.gameObject);
            ExitState();
        }
    }
}