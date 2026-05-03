using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 2.5f;

    private PlayerStamina playerStamina;
    private CrouchScript crouchScript;

    private Vector3 moveDirection;
    private float currentSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerStamina = GetComponent<PlayerStamina>();
        crouchScript = GetComponent<CrouchScript>();
    }

    void Update()
    {
        // ✅ Read input in Update (responsive), apply movement in FixedUpdate (consistent physics)
        bool isCrouching = crouchScript != null && crouchScript.isCrouching;

        Vector3 movementInput = transform.right * Input.GetAxisRaw("Horizontal")
                              + transform.forward * Input.GetAxisRaw("Vertical");
        moveDirection = Vector3.Normalize(movementInput);

        if (isCrouching)
            currentSpeed = crouchSpeed;
        else if (playerStamina != null && playerStamina.isSprinting)
            currentSpeed = sprintSpeed;
        else
            currentSpeed = walkSpeed;
    }

    void FixedUpdate()
    {
        // ✅ Move in FixedUpdate so speed is framerate-independent
        rb.MovePosition(rb.position + moveDirection * currentSpeed * Time.fixedDeltaTime);
    }
}