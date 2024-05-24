using UnityEngine;

public class BaseLivingBeing : MonoBehaviour, IDamageable, IMortal
{
    [Header("BaseLivingBeing")]
    [SerializeField] protected float maxLife = 100f;
    [SerializeField] protected float currentLife = 0f;
    
    public virtual void Awake() 
    {
        currentLife = maxLife;   
    }

    public virtual void ApplyDamage(float damage)
    {
        if(IsDead())
            return;

        currentLife -= damage;

        if(IsDead())
            Die();

    }

    public virtual void ApplyHealing(float healing)
    {
        currentLife += healing;

        if (currentLife > maxLife)
            currentLife = maxLife;
        
    }

    public virtual void BaseRevive()
    {
        currentLife = maxLife;
    }

    public virtual void Die()
    {

    }

    public float GetCurrentLife() => currentLife;

    public float GetMaxLife() => maxLife;

    public bool IsDead() => currentLife <= 0f;
}
