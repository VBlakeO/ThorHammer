using System.Collections;
using UnityEngine.Events;
using UnityEngine;
using System;


[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;
    [Space]

    [Header("Camera")]
    [SerializeField] private Camera playerCamera = null;
    [Range(30f, 170f)][SerializeField] private float initFOV = 90f;
    public float rotationSpeed = 5f;
    public float rotationSmoothness = 0.1f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -13.0f;


    [Header("Jump")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float jumpStaminaDrain = 15f;


    [Header("Initial Values")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float moveSmoothTime = 0.30f;


    [Header("Ground Test")]
    [SerializeField] private LayerMask layerMask = 1;
    [SerializeField] private float castRadius = 0.5f;
    [SerializeField] private float groundCheckRange = 0.75f;
    [SerializeField] private Vector3 originOfficet = Vector3.zero;


    [Header("===Components===")]
    private CharacterController controller = null;


    [Header("===PlayerMovement===")]
    [SerializeField] private bool cantMove = false;
    [SerializeField] private bool cantLook = false;
    [SerializeField] private bool cantJump = false;
    [Space]

    public float impulseForce = 1f;
    public float initialImpulseDuration = 1f;
    [Space]

    public UnityAction OnJump = null;
    public UnityAction OnLanding = null;
    public UnityAction OnFalling = null;

    //=============================================================

    // Movement Base
    private float speed = 0f;
    private Vector2 targetDir = Vector2.zero;
    private Vector2 currentDirVelocity = Vector2.zero;
    private Vector3 velocity = Vector3.zero;
    [HideInInspector] public Vector2 currentDir = Vector2.zero;

    // Movement Debug
    public float velocityY = 0f;
    [HideInInspector] public bool isJumping = false;
    [HideInInspector] public bool isGrounded = false;
    public bool isWalking = false;

    // GroudCheck
    private bool wasGrounded;

    //Slop
    private float slopeForce = 2.0f;
    private float slopeForceRayLength = 2.0f;

    private float impulseDuration = 0f;

    private void Awake()
    {
        Instance = this;

        controller = GetComponent<CharacterController>();

        playerCamera.fieldOfView = initFOV;

        speed = walkSpeed;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));


        Jump();

        if (wasGrounded != CheckGround())
        {
            GroundedChanged(CheckGround());
            wasGrounded = CheckGround();
        }
    }

    private void PlayerRoation()
    {
        if (isWalking && !cantLook)
        {
            float cameraRotationY = playerCamera.transform.rotation.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0f, cameraRotationY, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSmoothness * rotationSpeed);
        }
    }

    private void FixedUpdate()
    {
        PlayerRoation();
        Movement();

        if (impulseDuration > 0)
        {
            impulseDuration -= Time.deltaTime;

            if (impulseDuration < 0.3f)
                impulseDuration = 0f;

            controller.Move(transform.forward * impulseForce * impulseDuration * Time.deltaTime);
            PlayerExternalRotation();
        }
    }

    private void Movement()
    {
        velocityY += gravity * Time.deltaTime;

        if (velocityY < -3f)
            OnFalling?.Invoke();


        if (cantMove)
        {
            controller.Move(Vector3.up * velocityY * Time.deltaTime);
            return;
        }

        isWalking = targetDir.x != 0 || targetDir.y != 0 && CheckGround();

        targetDir.Normalize();
        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);

        if (CheckGround() && !isJumping)
            velocityY = 0;

        velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * speed + Vector3.up * velocityY;

        controller.Move(velocity * Time.deltaTime);

        if ((Mathf.Abs(targetDir.x) > 0 || Mathf.Abs(targetDir.y) > 0) && OnSlope())
            controller.Move(Vector3.down * controller.height / 2 * slopeForce * Time.deltaTime);
    }

    private void Jump()
    {
        if (Input.GetKeyDown(jumpKey) && CheckGround() && !cantJump)
        {
            OnJump?.Invoke();
            isJumping = true;
            velocityY = jumpForce;
            StartCoroutine(BackToGround());
        }
    }

    private bool OnSlope()
    {
        if (isJumping)
            return false;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, controller.height / 2 * slopeForceRayLength))
            if (hit.normal != Vector3.up)
                return true;

        return false;
    }

    private bool CheckGround()
    {
        Vector3 origin = transform.position + originOfficet;
        return Physics.SphereCast(origin, castRadius, -transform.up, out RaycastHit hit, groundCheckRange, layerMask, QueryTriggerInteraction.Ignore);
    }

    void GroundedChanged(bool state)
    {
        if (state)
        {
            isGrounded = true;

            if (velocityY < -5f)
            {
                OnLanding?.Invoke();
            }

            velocityY = 0f;
        }
        else
            isGrounded = false;
    }

    private IEnumerator BackToGround()
    {
        WaitForSeconds wfs = new WaitForSeconds(0.3f);
        yield return wfs;
        isJumping = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 origin = transform.position + originOfficet;
        Gizmos.DrawWireSphere(origin - transform.up * groundCheckRange, castRadius);
    }


    public void LockPlayer(bool _lock)
    {
        cantMove = _lock;
        cantLook = _lock;
        cantJump = _lock;
    }

    public void LockPlayerMovement()
    {
        cantMove = true;
        cantJump = true;
        cantLook = true;
        controller.enabled = false;

    }

    public void UnlockPlayerMovement()
    {
        cantMove = false;
        cantJump = false;
        cantLook = false;
        controller.enabled = true;
    }

    public void PlayerExternalRotation()
    {
        float cameraRotationY = playerCamera.transform.rotation.eulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(0f, cameraRotationY, 0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSmoothness * rotationSpeed);
    }

    public void AttackImpulse()
    {
        impulseDuration = initialImpulseDuration;
    }
}