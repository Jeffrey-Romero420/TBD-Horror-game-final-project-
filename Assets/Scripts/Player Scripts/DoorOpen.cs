using UnityEngine;

public class DoorInteract : MonoBehaviour
{
    public float distance = 3f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, distance))
            {
                Debug.Log("Hit: " + hit.transform.name);

                Door door = hit.transform.GetComponentInParent<Door>();

                if (door != null)
                {
                    door.ToggleDoor();
                }
            }
        }
    }
}