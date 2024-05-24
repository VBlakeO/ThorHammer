using UnityEngine;

public class DamageBox : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private Transform objectToIgnore = null;
    [SerializeField] private Transform origin = null;

    private Collider[] myColliders = null;

    private void Start() 
    {
        myColliders = GetComponentsInParent<Collider>();
    }

    private void OnTriggerEnter(Collider other) 
    {   
        Vector3 damagePoint = origin != null ? origin.position : Vector3.zero;

        // Make sure you don't cause harm to yourself
        for (int i = 0; i < myColliders.Length; i++)
        {
            if (other == myColliders[i])
                 return;
        }

        if (other == objectToIgnore)
            return;

        other.GetComponent<IDamageableOrigin>()?.ApplyDamage(damage, damagePoint);
    }
}
