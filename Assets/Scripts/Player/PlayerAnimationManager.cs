using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    private PlayerCombatManager playerCombat = null;
    private PlayerMovement playerMovement = null;
    private Animator anim = null;

    private bool throwingHammer = false;
    private bool hasTriggerThrow = false;
    private bool hasTriggerCall = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerCombat = GetComponent<PlayerCombatManager>();

        playerMovement.OnJump += Jump;
    }

    public void Update()
    {
        anim.SetFloat("FloatY", playerMovement.currentDir.y);
        anim.SetFloat("FloatX", playerMovement.currentDir.x);

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (playerCombat.hammerOnHand)
            {
                if (!hasTriggerThrow)
                {
                    anim.SetTrigger("Throw");
                    hasTriggerThrow = true;
                    hasTriggerCall = false;
                }
                throwingHammer = true;

            }
            else
            {
                hasTriggerThrow = false;

                if (!hasTriggerCall)
                {
                    anim.SetTrigger("Call");
                    hasTriggerCall = true;
                }
            }
        }

        if (throwingHammer)
            playerMovement.PlayerExternalRotation();
    }

    
    private void Jump()
    {
        anim.Play("Jump", 0, 0);
    }


    public void DeactivateThrowingHammer()
    {
        throwingHammer = false;
    }

    private void OnDestroy() 
    {
        if (playerMovement)
            playerMovement.OnJump -= Jump;
    }
}
