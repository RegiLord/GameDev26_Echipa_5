using System.Collections.Generic;
using AI.Blackboard;
using UnityEngine;
using UnityEngine.Analytics;

namespace AI.States
{
    public class FindEnemyState : AIState
    {
        public FindEnemyState(AIController controller, AIState nextState) : base(controller, nextState) { }
        
        public override void UpdateState()
        {
            var targetTransform = GetTarget();
            
            controller.blackboard.Set(AIKeys.Target, targetTransform);
            ExitState();
        }

        public virtual Transform GetTarget()
        {
            var units = GameObject.FindGameObjectsWithTag("Unit");

            if (units.Length <= 1)
                return null;

            
            int unitIndex = 0;

            for (int i = 0; i < 5; i++)
            {
                unitIndex = Random.Range(0, units.Length);

                var unitController = units[unitIndex].GetComponent<AIController>();
                if (unitController.teamID != controller.teamID)
                    break;

                if (i == 4)
                {
                    if (controller.teamID != TeamID.Player && Core.Instance)
                        return Core.Instance.transform;
                    return null;
                }
            }

            return units[unitIndex].transform;
        }
    }
}