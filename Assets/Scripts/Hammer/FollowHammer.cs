using UnityEngine;

public class FollowHammer : MonoBehaviour
{
    public Transform target = null;
    public float followSpeed = 8f;

    void FixedUpdate() =>  transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * followSpeed);
}
