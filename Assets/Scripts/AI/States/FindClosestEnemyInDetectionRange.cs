using System.Linq;
using UnityEngine;

namespace AI.States
{
    public class FindClosestEnemyInDetectionRange : FindEnemyState
    {
        public override (Vector3, GameObject) GetTarget()
        {
            // Momentan ptr demo
            var units = GameObject.FindGameObjectsWithTag("Unit");

            units = units
                .Select(u => u.GetComponent<AIController>())
                .Where( u => u is not null && u.teamID != controller.teamID )
                .Select( u => u.gameObject )
                .ToArray();
            
            GameObject bestUnit = null;
            Vector3 bestPoint =  Vector3.zero;
            float bestDistance = float.MaxValue;

            for (int i = 0; i < units.Length; i++)
            {
                Collider collider = units[i].GetComponent<Collider>();
                
                // Cant attack something without collider
                if (collider is null)
                    continue;

                Vector3 closestPoint = collider.ClosestPoint(controller.transform.position);
                float distance = Vector3.Distance(controller.transform.position, collider.transform.position);
                
                // Check detection range
                if (distance > controller.detectionRange)
                    continue;

                if (bestUnit is null || distance < bestDistance)
                {
                    bestDistance = distance;
                    bestPoint = closestPoint;
                    bestUnit = units[i];
                }
            }

            if (bestUnit is null && controller.teamID != TeamID.Player && Core.Instance is not null)
                return (Core.Instance.Coll.ClosestPoint(controller.transform.position), Core.Instance.gameObject);
            
            // Temporary fix so units dont go up
            bestPoint.y = controller.transform.position.y;
            return (bestPoint, bestUnit);
        }
    }
}