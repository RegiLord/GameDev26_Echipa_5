using System;

public interface IDamageable
{
    public void TakeDamage(float damage);
    public event Action OnDeath;
}
