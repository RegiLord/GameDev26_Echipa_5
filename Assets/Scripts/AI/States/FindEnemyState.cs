using System.Collections.Generic;
using AI.Blackboard;
using UnityEngine;

namespace AI.States
{
    public class FindEnemyState : AIState
    {
        public FindEnemyState(AIController controller) : base(controller)
        {
        }

        public override void UpdateState()
        {
            var units = GameObject.FindGameObjectsWithTag("Unit");

            if (units.Length <= 1)
                return;
            
            int unitIndex = 0;
            do
            {
                unitIndex = Random.Range(0, units.Length);
            } while (units[unitIndex] == controller.gameObject);
            
            
            
            controller.blackboard.Set(AIKeys.Target, units[unitIndex].transform);
            ExitState();
        }
    }
}