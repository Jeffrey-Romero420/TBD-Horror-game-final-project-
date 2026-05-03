using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    public float maxStamina = 100f;
    public float currentStamina;
    public float drainRate = 20f;
    public float regenRate = 15f;
    public float regenDelay = 2f;
    private float lastUseTime;
    public Slider staminaSlider;

    // ✅ PlayerMovement reads this — only set here, never in PlayerMovement
    public bool isSprinting { get; private set; } = false;

    private CrouchScript crouchScript;

    void Start()
    {
        currentStamina = maxStamina;
        staminaSlider.maxValue = maxStamina;
        crouchScript = GetComponent<CrouchScript>();
    }

    void Update()
    {
        bool isCrouching = crouchScript != null && crouchScript.isCrouching;
        bool wantsToSprint = Input.GetKey(KeyCode.LeftShift);

        Vector3 movementInput = transform.right * Input.GetAxisRaw("Horizontal")
                              + transform.forward * Input.GetAxisRaw("Vertical");
        bool isMoving = movementInput.magnitude > 0;

        if (isCrouching)
        {
            // Crouching: no sprinting, regen stamina
            isSprinting = false;

            if (currentStamina < maxStamina && Time.time > lastUseTime + regenDelay)
                currentStamina += regenRate * Time.deltaTime;
        }
        else if (wantsToSprint && isMoving && currentStamina > 0)
        {
            // ✅ Only sprint if actually moving AND has stamina
            isSprinting = true;
            currentStamina -= drainRate * Time.deltaTime;
            lastUseTime = Time.time;
        }
        else
        {
            // ✅ Explicitly stop sprinting — covers stamina = 0, shift released, not moving
            isSprinting = false;

            if (currentStamina < maxStamina && Time.time > lastUseTime + regenDelay)
                currentStamina += regenRate * Time.deltaTime;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        staminaSlider.value = currentStamina;
    }
}