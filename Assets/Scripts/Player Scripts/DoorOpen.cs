using UnityEngine;

public class DoorInteract : MonoBehaviour
{
    public float distance = 3f;
    public Transform cameraTransform;

    // ✅ Set this to your Player's layer in the Inspector
    // In Unity: Edit > Project Settings > Tags and Layers, assign player to a layer
    // then exclude it here so the ray doesn't hit yourself
    public LayerMask ignoreLayers;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Transform origin = cameraTransform != null ? cameraTransform : transform;

            RaycastHit hit;

            // ✅ ~ignoreLayers inverts the mask so those layers are excluded
            if (Physics.Raycast(origin.position, origin.forward, out hit, distance, ~ignoreLayers))
            {
                Debug.Log("Hit: " + hit.transform.name);

                Door door = hit.transform.GetComponentInParent<Door>();
                if (door != null)
                    door.ToggleDoor();
            }
        }
    }
}