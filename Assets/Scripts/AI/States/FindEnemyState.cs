using System.Collections.Generic;
using AI.Blackboard;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Analytics;

namespace AI.States
{
    public class FindEnemyState : AIState
    {
        public override void UpdateState()
        {
            var target = GetTarget();

            if (target.Item2 is null)
            {
                ExitState(false);
                return;
            }

            controller.blackboard.Set(AIKeys.Target, target.Item1);
            controller.blackboard.Set(AIKeys.TargetUnitData, target.Item2);
            
            ExitState(true);
        }
        
        public  virtual (Vector3, UnitData) GetTarget()
        {
            return (Vector3.zero, null);
        }
    }
}