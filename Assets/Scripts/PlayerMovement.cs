using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    public float moveSpeed; 
    public float walkSpeed = 5f;
    public float runSpeed = 10f;







    void Update()
    {
        Vector3 movementInput = transform.right * Input.GetAxisRaw("Horizontal") + transform.forward * Input.GetAxisRaw("Vertical");
       movementInput = Vector3.Normalize(movementInput);
        rb.MovePosition(transform.position + movementInput * Time.deltaTime * moveSpeed);

    if (Input.GetKey(KeyCode.LeftShift))
    {
        moveSpeed = runSpeed;
    }
    else
    {
        moveSpeed = walkSpeed;
    }
    }
}
