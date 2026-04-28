using UnityEngine;

public class LookAround : MonoBehaviour
{
    public float sensitivity;
    public Transform cameraTransform;
    private float xRotation = 0;
    private float yRotation = 0;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update ()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        yRotation += mouseX;

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        transform.localRotation = Quaternion.Euler(0, yRotation, 0);
    }








































}