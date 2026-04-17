using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{

    public float maxStamina = 100f;
    public float currentStamina;
    public float drainRate = 20f;
    public float regenRate = 15f;
    public Slider staminaSlider;

    public bool isSprinting;


    void Start()
    {
        currentStamina = maxStamina;
        staminaSlider.maxValue = maxStamina;
    }



    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && currentStamina > 0) 
        {
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }

        if (isSprinting)
        {
            currentStamina -= drainRate * Time.deltaTime;
        }
        else if (currentStamina < maxStamina)
        {
            currentStamina += regenRate * Time.deltaTime;
        }


        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        staminaSlider.value = currentStamina;


    }


 
}
