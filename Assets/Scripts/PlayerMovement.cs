using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    private float currentSpeed;
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public PlayerStamina data;
    
    private PlayerStamina playerStamina;



    void Start()
    {
        rb = GetComponent<Rigidbody>();

        playerStamina = GetComponent<PlayerStamina>();
    }




    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && playerStamina.currentStamina > 0) 
        {
            currentSpeed = sprintSpeed;
            playerStamina.isSprinting = true; // Signal the stamina script to drain
        } 
        else 
        {
            // ✅ TASK B: "If stamina reaches zero, force speed back to walkSpeed"
            // (This also runs automatically if the player simply lets go of Shift)
            currentSpeed = walkSpeed;
            playerStamina.isSprinting = false; // Signal the stamina script to stop draining
        }



        
    }

    

   
}
