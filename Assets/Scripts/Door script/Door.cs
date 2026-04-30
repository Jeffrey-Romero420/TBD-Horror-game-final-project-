using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isOpen = false;
    public float openAngle = 90f;
    public float speed = 3f;

    private Quaternion closedRot;
    private Quaternion openRot;

    void Start()
    {
        closedRot = transform.rotation;
        openRot = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));
    }

    void Update()
    {
        Quaternion target = isOpen ? openRot : closedRot;
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * speed);
    }

    // ✅ PLAYER uses this
    public void ToggleDoor()
    {
        isOpen = !isOpen;
    }

    // ✅ KILLER uses this
    public void OpenDoor()
    {
        isOpen = true;
    }
}