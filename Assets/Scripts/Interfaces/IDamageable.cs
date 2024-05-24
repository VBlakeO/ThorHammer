using UnityEngine;

public interface IDamageable
{
    public void ApplyDamage(float damage);
}

public interface IDamageableOrigin
{
    public void ApplyDamage(float damage, Vector3 origin);
}

public interface IMortal
{
    public void Die();
}
