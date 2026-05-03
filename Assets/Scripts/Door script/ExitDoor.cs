using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    public PlayerKeys player;
    public bool isOpen = false;
    public float openAngle = 90f;
    public float speed = 3f;           // ✅ added: door now animates open like the others

    private Quaternion closedRot;
    private Quaternion openRot;

    void Start()
    {
        isOpen = false;
        closedRot = transform.rotation;
        openRot = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));
    }

    void Update()
    {
        // ✅ animates smoothly open when unlocked
        Quaternion target = isOpen ? openRot : closedRot;
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * speed);
    }

    public void TryOpen()
    {
        if (player.keysCollected >= player.keysNeeded)
        {
            isOpen = true;
            Debug.Log("Exit door opened!");
        }
        else
        {
            Debug.Log("Need " + (player.keysNeeded - player.keysCollected) + " more key(s)!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isOpen) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Escaped!");
            // trigger win screen here
        }
    }
}