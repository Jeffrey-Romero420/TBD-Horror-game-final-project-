using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Speed")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 7f;
    public float crouchSpeed = 1.8f;

    [Header("Physics")]
    public float gravity = -20f;
    public float stepOffset = 0.4f;      // ✅ max step height CharacterController can climb
    public float slopeLimit = 50f;       // ✅ max slope angle it can walk up

    private CharacterController controller;
    private PlayerStamina playerStamina;
    private CrouchScript crouchScript;

    private float verticalVelocity;
    private Vector3 moveDirection;
    private float currentSpeed;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerStamina = GetComponent<PlayerStamina>();
        crouchScript = GetComponent<CrouchScript>();

        // ✅ Apply stair/slope settings to CharacterController
        controller.stepOffset = stepOffset;
        controller.slopeLimit = slopeLimit;
    }

    void Update()
    {
        bool isCrouching = crouchScript != null && crouchScript.isCrouching;

        Vector3 input = transform.right * Input.GetAxisRaw("Horizontal")
                      + transform.forward * Input.GetAxisRaw("Vertical");
        moveDirection = Vector3.Normalize(input);

        if (isCrouching)
            currentSpeed = crouchSpeed;
        else if (playerStamina != null && playerStamina.isSprinting)
            currentSpeed = sprintSpeed;
        else
            currentSpeed = walkSpeed;

        // Gravity
        if (controller.isGrounded)
            verticalVelocity = -2f;
        else
            verticalVelocity += gravity * Time.deltaTime;

        Vector3 move = moveDirection * currentSpeed;
        move.y = verticalVelocity;

        // ✅ Only move if CharacterController is active
        if (controller != null && controller.enabled)
            controller.Move(move * Time.deltaTime);
    }
}