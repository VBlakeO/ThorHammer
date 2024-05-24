using UnityEngine.Events;
using UnityEngine.AI;
using UnityEngine;

public enum EnemyState{Idle, Patrol, Chasing}

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBase : MonoBehaviour
{
    public EnemyState enemyState = EnemyState.Patrol;
    [Space]

    [SerializeField] private Transform player = null;
    [SerializeField] private Vector3 targetPos = Vector3.zero;
    [SerializeField] private float smoothSpeed = 0.2f;
    [Space]

    private EnemyState initialEnemyState = EnemyState.Patrol;
    public UnityAction OnStateChange = null;

    private int currentPatrolPoint = 1;
    private Vector3[] patrolPoint = new Vector3[4];

    private bool hasTriggerAttack = true;

    [HideInInspector] public Animator anim = null;
    [HideInInspector] public NavMeshAgent agent = null;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start() 
    {
        SetPatrolPoints();
        Invoke(nameof(ResetAttack), 2f);

        initialEnemyState = enemyState;

        if (enemyState == EnemyState.Patrol)
        {
            targetPos = patrolPoint[currentPatrolPoint];
            agent.SetDestination(targetPos);
        }
    }

    void FixedUpdate()
    {
        if (!agent.enabled)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= 10f && enemyState != EnemyState.Chasing)
        {
            SetState(EnemyState.Chasing);
        }

        anim.SetFloat("Acceleration", agent.velocity.magnitude);

        if (enemyState == EnemyState.Patrol)
        {
            if (agent.remainingDistance < 2f)
            {
                currentPatrolPoint++;

                if (currentPatrolPoint > patrolPoint.Length - 1)
                    currentPatrolPoint = 0;

                targetPos = patrolPoint[currentPatrolPoint];
                agent.SetDestination(targetPos);
            }
        }

        if (enemyState == EnemyState.Chasing)
        {
            targetPos = player.position;
            agent.SetDestination(targetPos);

            if (distance < 1.8f) 
            {
                if (!hasTriggerAttack)
                {
                    anim.SetTrigger("Attack");
                    hasTriggerAttack = true;
                    Invoke(nameof(ResetAttack), 2f);
                }
            }

            if (player != null)
            {
                SmoothLookAt(); 
            }
        }
    }

    private void ResetAttack()
    {
        hasTriggerAttack = false;
    }

    private void SetPatrolPoints()
    {
        patrolPoint[0] = transform.position;
        patrolPoint[1] = transform.position + new Vector3(10, 0, 0);
        patrolPoint[2] = transform.position + new Vector3(10, 0, 10);
        patrolPoint[3] = transform.position + new Vector3(0, 0, 10);
    }

    void SmoothLookAt()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);
        }
    }

    public void SetState(EnemyState state)
    {
        enemyState = state;
        OnStateChange?.Invoke();
    }

    public void Revive()
    {
        SetState(initialEnemyState);

        hasTriggerAttack = true;
        transform.position = patrolPoint[0];

        Invoke(nameof(ResetAttack), 5f);
    }
}
