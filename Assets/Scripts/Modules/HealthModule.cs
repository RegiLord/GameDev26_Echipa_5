using System;
using UnityEngine;

public class HealthModule : Module
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float health;
    
    public float Health
    {
        get { return health; }
        set { health = Mathf.Clamp(value, 0, maxHealth); }
    }
    public float MaxHealth => maxHealth;
    
    private bool dead;
    public bool IsDead => dead;
    
    public event Action<float, float> HealthChanged;
    public event Action OnDeath;
    
    public void TakeDamage(float damage)
    {
        if (IsDead)
            return;
        if (damage == 0)
            return;
        
        health -= damage;
        if (health <= 0)
        {
            health = 0;
            dead = true;
            // Momentarily a naive implementation for debugging purposes
            Destroy(gameObject, 3.0f);
            OnDeath?.Invoke();
        }
        
        HealthChanged?.Invoke(health, maxHealth);
    }
}
