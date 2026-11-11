using AI.Blackboard;
using UnityEngine;

namespace AI.States
{
    public class AttackTargetState : AIState
    {
        
        public override void EnterState()
        {
            controller.blackboard.TryGet(AIKeys.TargetObject, out GameObject target);
            controller.blackboard.TryGet(AIKeys.AttackModule, out AttackModule attackModule);
            
            
            if (target && attackModule)
                attackModule.AttackTarget(target.gameObject);
            ExitState(true);
        }
    }
}