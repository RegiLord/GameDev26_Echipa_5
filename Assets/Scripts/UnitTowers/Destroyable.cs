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

    public delegate void HealthChangedDelegate(float health, float maxHealth);
    public event HealthChangedDelegate HealthChanged;
    
    public void TakeDamage(float damage)
    {
        if (damage <= 0) return;
        
        health -= damage;
        if (health <= 0)
        {
            health = 0;
            OnDeath();
        }

        HealthChanged?.Invoke(health, maxHealth);
    }

    protected void OnDeath()
    {
        Destroy(gameObject, 5.0f);
    }
}
