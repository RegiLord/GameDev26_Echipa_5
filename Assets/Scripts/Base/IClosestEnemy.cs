
using System.Linq;
using UnityEngine;

public interface IClosestEnemy : ITargettingSystem
{
    public IDamageable GetTarget(float range, Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, range);

        var enemies = colliders
            .Select(collider => collider.GetComponent<IDamageable>())
            .Where(i => i != null).ToArray();
            
        
        // Core in case of no damageable object in range
        return null;
    }
}
