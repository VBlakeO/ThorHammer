using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class EnemyDebugState : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stateText = null; 
    [SerializeField] private Image baseLifeBar = null; 
    [SerializeField] private Image lifeBar = null; 

    private EnemyBase enemyBase = null;
    private EnemyLivingBeing enemyLivingBeing = null;

    // Start is called before the first frame update
    private void Awake()
    {
        enemyBase = GetComponentInParent<EnemyBase>();
        enemyLivingBeing = GetComponentInParent<EnemyLivingBeing>();

        enemyBase.OnStateChange += UpdateState;

        enemyLivingBeing.OnReceiveDamage += UpdateLife;
        enemyLivingBeing.OnReceiveHealing += UpdateLife;

        enemyLivingBeing.OnRevive += UpdateLife;
        enemyLivingBeing.OnRevive += EnableLifeBar;

        enemyLivingBeing.OnDie += DisableLifeBar;
    }

    private void Start() 
    {
        UpdateState();
        UpdateLife();
    }

    public void UpdateState()
    {
        stateText.text = enemyBase.enemyState.ToString(); 
    }

    private void UpdateLife()
    {
        lifeBar.fillAmount = enemyLivingBeing.GetCurrentLife() / enemyLivingBeing.GetMaxLife();
    }

    private void EnableLifeBar()
    {
        stateText.enabled = true;
        
        lifeBar.enabled = true;
        baseLifeBar.enabled = true;
    }

    private void DisableLifeBar()
    {
        stateText.enabled = false;
        
        lifeBar.enabled = false;
        baseLifeBar.enabled = false;
    }


    private void OnDestroy() 
    {
        enemyBase.OnStateChange -= UpdateState;

        enemyLivingBeing.OnReceiveDamage -= UpdateLife;
        enemyLivingBeing.OnReceiveHealing -= UpdateLife;

        enemyLivingBeing.OnRevive -= UpdateLife;
        enemyLivingBeing.OnRevive -= EnableLifeBar;
        
        enemyLivingBeing.OnDie -= DisableLifeBar;
    }
}
