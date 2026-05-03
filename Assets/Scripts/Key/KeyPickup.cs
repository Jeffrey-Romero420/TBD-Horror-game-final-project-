using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerKeys player = other.GetComponent<PlayerKeys>();

        if (player != null)
        {
            player.AddKey();
            Destroy(gameObject);
        }
    }
}