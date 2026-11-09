using System;
using System.Threading;
using UnityEngine;

public class AttackModule : Module
{
    [SerializeField] private float attack;
    [SerializeField] private float attackSpeed;     // How many attacks per second
    [SerializeField] private float attackRange;

    bool inCooldown = false;
    
    public float Attack
    {
        get => attack;
        set => attack = value;
    }
    public float AttackSpeed
    {
        get => attackSpeed;
        set => attackSpeed =  value;
    }
    public float AttackRange
    {
        get => attackRange;
        set => attackRange = value;
    }
    public bool InCooldown => inCooldown;
    
    public event Action<float> OnAttack;
    
    public void AttackTarget(GameObject target)
    {
        float distance = Vector3.Distance(transform.position, target.transform.position);
        
        if (distance > attackRange)
            return; // OUT OF RANGE
        
        HealthModule healthModule = target.GetComponent<HealthModule>();
        if (!healthModule)
            return; // CANT ATTACK TARGET WITHOUT HEALTH
        
        if (inCooldown)
            return; // MUST WAIT BETWEEN ATTACKS

        inCooldown = true;
        OnAttack?.Invoke(attack);
        
        // momentarily a naive implementation mainly for debugging
        healthModule.TakeDamage(Attack);
        
        Invoke( nameof(resetCooldown), 1.0f / attackSpeed);
    }
    
    private void resetCooldown()
    {
        inCooldown = false;
    }
}
