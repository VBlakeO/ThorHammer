using UnityEngine;
using UnityEngine.Events;

public class ComplexLivingBeing : BaseLivingBeing
{
    [Space]
    [Header("Complex Living Being")]
    [Range(0, 1)]
    [SerializeField] private float armorProtection = 0;
    [SerializeField] private bool canHealing = true;

    public UnityAction OnReceiveDamage = null;
    public UnityAction OnReceiveHealing = null;
    public UnityAction OnRevive = null;
    public UnityAction OnDie = null;

    public override void ApplyDamage(float damage)
    {
        float damageReduction = 1 - armorProtection;
        damage *= damageReduction;

        if(damage > 0)
        {
            base.ApplyDamage(damage);
            OnReceiveDamage?.Invoke();
        }
    }

    public override void ApplyHealing(float damage)
    {
        if (canHealing)
        {
            base.ApplyHealing(damage);
            OnReceiveHealing?.Invoke();
        }
    }

    public override void BaseRevive()
    {
        base.BaseRevive();
        OnRevive?.Invoke();
    }


    public override void Die()
    {
        base.Die();
        OnDie?.Invoke();
    }
}
