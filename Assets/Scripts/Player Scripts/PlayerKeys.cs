using UnityEngine;

public class PlayerKeys : MonoBehaviour
{
    public int keysCollected = 0;
    public int keysNeeded = 3;

    // ✅ fixed: was Door type with Unlock(), now correctly ExitDoor with TryOpen()
    public ExitDoor escapeDoor;

    public void AddKey()
    {
        keysCollected++;
        Debug.Log("Key collected: " + keysCollected + " / " + keysNeeded);

        if (keysCollected >= keysNeeded)
        {
            Debug.Log("All keys collected! Escape door unlocked.");

            if (escapeDoor != null)
                escapeDoor.TryOpen();
            else
                Debug.LogWarning("PlayerKeys: escapeDoor not assigned in Inspector!");
        }
    }

    public bool HasAllKeys()
    {
        return keysCollected >= keysNeeded;
    }
}