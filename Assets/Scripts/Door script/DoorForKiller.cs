using UnityEngine;

public class DoorForKiller : MonoBehaviour

{
    public bool isOpen = false;
    public float openAngle = 90f;
    public float speed = 3f;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));
    }

    void Update()
    {
        if (isOpen)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, openRotation, Time.deltaTime * speed);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, closedRotation, Time.deltaTime * speed);
        }
    }

    public void OpenDoor()
    {
        isOpen = true;
    }
}