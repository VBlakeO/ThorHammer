using UnityEngine.AI;
using UnityEngine;

public class EnemyLivingBeing : ComplexLivingBeing, IDamageableOrigin
{
    [Space]
    [SerializeField] private float knockbackForce = 100f;
    [SerializeField] private float upwardRecoilForce = 9f;
    [Space]

    private float dotResult = 0f;
    
    private Animator anim = null;
    private NavMeshAgent agent = null;
    private Collider mainCollider = null;
    
    private Rigidbody[] rigidbodies = null;

    [HideInInspector] public EnemyBase enemy = null;

    public override void Awake()
    {
        base.Awake();

        anim = GetComponent<Animator>();
        enemy = GetComponent<EnemyBase>();
        agent = GetComponent<NavMeshAgent>();
        mainCollider =  GetComponent<Collider>();
        rigidbodies = GetComponentsInChildren<Rigidbody>();

        SetRagdoll(true);
    }

    private void Start() 
    {
        EnemyManager.Instance.enemyList.Add(this);
    }
    

    private void SetRagdoll(bool state)
    {
        foreach (var rb in rigidbodies)
        {
            rb.isKinematic = state;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public void ApplyDamage(float damage, Vector3 origin)
    {
        dotResult = 0f;

        Vector3 attackDirection = (transform.position - origin).normalized;
        dotResult = Vector3.Dot(attackDirection, transform.forward);

        if (origin == Vector3.zero)
            dotResult = 0f;

        enemy.SetState(EnemyState.Chasing);

        ApplyDamage(damage);

        if(!IsDead())
            anim.Play("Damage", 0 ,0);
    }

    public override void Die()
    {
        base.Die();

        anim.enabled = false;
        agent.enabled = false;
        mainCollider.enabled = false;
        
        SetRagdoll(false);

        ApplyInpulse(knockbackForce);
    }

    private void ApplyInpulse(float force)
    {
       Vector3 knockbackDirection = -transform.forward;
       
       float margin = 0.1f;
       if (dotResult > margin) 
            knockbackDirection = transform.forward;
       else if (dotResult < -margin) 
            knockbackDirection = -transform.forward;
       else 
            knockbackDirection = -transform.forward;

        foreach (var rb in rigidbodies)
            rb.AddForce(knockbackDirection * force + Vector3.up * upwardRecoilForce, ForceMode.Impulse); 
    }

    public override void BaseRevive()
    {
        base.BaseRevive();

        enemy.Revive();
        SetRagdoll(true);
        
        anim.enabled = true;
        agent.enabled = true;
        mainCollider.enabled = true;
    }
}
