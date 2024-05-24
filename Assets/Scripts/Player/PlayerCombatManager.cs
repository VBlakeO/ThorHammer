using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerCombatManager : MonoBehaviour
{
    [SerializeField] private float comboResetTime = 1.5f;

    [Header("VFX")]
    [SerializeField] private Light lightningLight = null;
    [SerializeField] private GameObject[] lightning = null;
    [SerializeField] private ParticleSystem[] particle = null;
    [SerializeField] private float lightningAttackDuration = 1.5f;  
    [Space]

    [Header("Hammer")]
    public GameObject hammer = null;
    public HammerController fakeHammer = null;
    public bool hammerOnHand {get; private set;} = true; 
    [Space]

    [Header("ShockWave")]
    public Transform shockWaveOrigin = null;
    public float shockWaveRadius = 5f;
    public float shockWaveDamage = 200f;
    public LayerMask layerMask = 0;
    [Space]

    private float lastAttackTime = 0f;
    private float lightDuration = 0f;
    private int comboStep = 0;

    private Animator anim = null;
    private AudioList audioList = null;
    public AnimationCurve curve = null;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        audioList = GetComponent<AudioList>();
        hammerOnHand = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
            Attack();

        if (Time.time - lastAttackTime > comboResetTime)
        {
            comboStep = 0;
            anim.SetInteger("ComboStep", 0);
        }

        if (lightningLight.gameObject.activeInHierarchy)
        {
            if (lightDuration < 1f)
                lightDuration += Time.deltaTime * 2f;

            lightningLight.intensity = curve.Evaluate(lightDuration);
        }
    }

    private void Attack()
    {   
        if (!hammerOnHand)
            return;

        lastAttackTime = Time.time;

        comboStep++;
        
        if (comboStep > 3)
            comboStep = 1;

        anim.SetTrigger("Attack");
        anim.SetInteger("ComboStep", comboStep);
    }

    public void ThrowHammer()
    {   
        hammer.SetActive(false);

        audioList.PlayOnceAudioClip(0);
        
        fakeHammer.gameObject.SetActive(true);
        fakeHammer.ThrowHammer();
        hammerOnHand = false;
    }

    public void CallHammer()
    {
        fakeHammer.BackToHand();
        audioList.PlayOnceAudioClip(1);
    }

    public void GetHammer()
    {
        hammer.SetActive(true);
        hammerOnHand = true;
    }

    private void ShockWave()
    {
        RaycastHit[] hits = Physics.SphereCastAll(shockWaveOrigin.position, shockWaveRadius, transform.up, 1, layerMask, QueryTriggerInteraction.Ignore);
        
        List<BaseLivingBeing> enemys = new List<BaseLivingBeing>(); 

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.GetComponentInParent<BaseLivingBeing>())
            {
                BaseLivingBeing baseL = hits[i].transform.GetComponentInParent<BaseLivingBeing>();

                if (!enemys.Contains(baseL))
                    enemys.Add(baseL);
            }
        }

        for (int i = 0; i < enemys.Count; i++)
            enemys[i].ApplyDamage(shockWaveDamage);
    }

    public IEnumerator LightningAttackEffect()
    {
        ShockWave();

        lightningLight.gameObject.SetActive(true);

        audioList.PlayOnceAudioClip(2);

        for (int i = 0; i < lightning.Length; i++)
            lightning[i].SetActive(true);

        for (int i = 0; i < particle.Length; i++)
            particle[i].Play();

        yield return new WaitForSeconds(lightningAttackDuration);

        lightningLight.gameObject.SetActive(false);
        lightDuration = 0f;

        for (int i = 0; i < lightning.Length; i++)
            lightning[i].SetActive(false);
    }
}
