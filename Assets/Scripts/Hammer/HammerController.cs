using System.Collections.Generic;
using DigitalRuby.LightningBolt;
using System.Collections;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HammerController : MonoBehaviour
{
    [SerializeField] private float launchForce = 30f;
    [SerializeField] private float backDuration = 0.8f;
    [SerializeField] private bool useSpecial = false;
    [Space]

    [SerializeField] private Transform hammerParent = null;
    [SerializeField] private Transform playerCamera = null;
    [Space]

    [SerializeField] private GameObject lightning = null;
    [SerializeField] private PlayerCombatManager playerCombatManager = null;
    [Space]
    
    [SerializeField] private float torqueAmount = 10f;
    [SerializeField] private Vector3 headOffset = Vector3.zero;
    [Space]

    [SerializeField] private LayerMask layerMask = 0;
    [SerializeField] private List<BaseLivingBeing> enemys;

    private Rigidbody rb = null;
    private DamageBox damageBox = null;

    private bool returning = false;
    private bool firstCollision = true;

    private void Awake() 
    {
        rb = GetComponent<Rigidbody>();
        damageBox = GetComponent<DamageBox>();
    }

    void FixedUpdate()
    {
        if (rb != null && !returning)
        {
            Vector3 gravityDirection = Vector3.down;
            Vector3 headDirection = transform.TransformPoint(headOffset) - rb.worldCenterOfMass;
            Vector3 torque = Vector3.Cross(headDirection.normalized, gravityDirection) * torqueAmount;

            rb.AddTorque(torque);
        }

        if (returning)
        {
            float distance = Vector3.Distance(transform.position, hammerParent.position);
            if (distance < 0.4f)
            {
                firstCollision = true;
                playerCombatManager.GetHammer();
                gameObject.SetActive(false);
            }
        }

        damageBox.enabled = rb.velocity.sqrMagnitude > 10f;
    }

    public void ThrowHammer()
    {
        returning = false;
        rb.isKinematic = false;

        transform.position = hammerParent.position;
        transform.rotation = hammerParent.rotation;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(playerCamera.forward * launchForce, ForceMode.Impulse);
    }

    public void BackToHand()
    {
        returning = true;

        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        DOTweenModulePhysics.DOMove(rb, hammerParent.position, backDuration);
        DOTweenModulePhysics.DORotate(rb, hammerParent.rotation.eulerAngles, backDuration, RotateMode.Fast);
    }

    private void OnCollisionEnter(Collision other) 
    {
        if (firstCollision)
        {
            if(useSpecial) Special();
            StartCoroutine(LightningAttackEffect());   
        }
    }

    public IEnumerator LightningAttackEffect()
    {
        firstCollision = false;
        lightning.GetComponent<LightningBoltScript>().StartPosition = new Vector3(transform.position.x, 9, transform.position.z);
        lightning.SetActive(true);

        yield return new WaitForSeconds(0.2f);

        lightning.SetActive(false);
    }

    private void Special()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 20f, transform.up, 1, layerMask, QueryTriggerInteraction.Ignore);
        
        enemys = new List<BaseLivingBeing>(); 

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.GetComponentInParent<BaseLivingBeing>())
            {
                BaseLivingBeing baseL = hits[i].transform.GetComponentInParent<BaseLivingBeing>();

                if (!enemys.Contains(baseL))
                    enemys.Add(baseL);
            }
        }

        rb.isKinematic = true;
        rb.angularVelocity = Vector3.zero;

        enemys.Sort((obj1, obj2) => Vector3.Distance(transform.position, obj1.transform.position).CompareTo(Vector3.Distance(transform.position, obj2.transform.position)));

        int test = 0;
        NextTarget();

        void NextTarget()
        {    
            if (returning)
                return;
            
            if (test < 0)
                return;
            
            if (test > enemys.Count - 1)
                test = enemys.Count - 1;

            DOTweenModulePhysics.DOMove(rb, enemys[test].transform.position + Vector3.up, 0.3f).OnComplete(() => 
            {
                if (test < enemys.Count - 1)
                {
                   test++;
                   NextTarget();
                }
                else
                {
                   rb.isKinematic = false;
                }
            });
        }
    }
}
