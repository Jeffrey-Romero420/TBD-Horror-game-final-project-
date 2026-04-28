using UnityEngine;

public class PlayerNoise : MonoBehaviour
{
    public float walkNoise = 4f;
    public float runNoise = 10f;

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            MakeNoise(runNoise);
        }
        else
        {
            MakeNoise(walkNoise);
        }
    }

    void MakeNoise(float volume)
    {
        SoundManager.instance.MakeNoise(transform.position, volume);
    }
}