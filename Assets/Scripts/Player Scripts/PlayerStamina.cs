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

    public bool isSprinting;


    void Start()
    {
        currentStamina = maxStamina;
        staminaSlider.maxValue = maxStamina;
    }



    void Update()
    {

        Vector3 movementInput = transform.right * Input.GetAxisRaw("Horizontal") + transform.forward * Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.LeftShift) && currentStamina > 0 && movementInput.magnitude > 0) 
        {
            isSprinting = true;
            currentStamina -= drainRate * Time.deltaTime;
            lastUseTime = Time.time;
        }
        else
        {
            isSprinting = false;
        }

        if (!isSprinting && currentStamina < maxStamina)
        {
            if (Time.time > lastUseTime + regenDelay)
            {
                currentStamina += regenRate * Time.deltaTime;
            }
        }


        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        staminaSlider.value = currentStamina;


    }


 
}
