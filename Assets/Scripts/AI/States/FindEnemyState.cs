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
            controller.blackboard.Set(AIKeys.TargetObject, target.Item2);
            
            ExitState(true);
        }
        
        public virtual (Vector3, GameObject) GetTarget()
        {
            // Momentan doar pentru teste
            var units = GameObject.FindGameObjectsWithTag("Unit");

            if (units.Length <= 1)
                return default;

            
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
                        return (Core.Instance.transform.position, Core.Instance.gameObject);
                    return default;
                }
            }

            return (units[unitIndex].transform.position, units[unitIndex]);
        }
    }
}