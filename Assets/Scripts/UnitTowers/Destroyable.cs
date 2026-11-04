using System;
using UnityEngine;

public class Destroyable : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] protected float maxHealth;
    
    public float Health
    {
        get => health; 
        set => health = Mathf.Clamp(value, 0, maxHealth);
    }
    public float MaxHealth
    {
        get => maxHealth;
        set
        {
            if (value <= 0) return;
            maxHealth = value;
            health = Mathf.Clamp(health, 0, maxHealth);
        }
    }

    public event Action HealthChanged;
    
    public void TakeDamage(float damage)
    {
        if (damage <= 0) return;
        
        health -= damage;
        if (health <= 0)
            OnDeath();
        
        HealthChanged?.Invoke();
    }

    protected void OnDeath()
    {
        Destroy(gameObject, 5.0f);
    }
}
