using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public float baseNoiseRadius = 15f;

    void Awake()
    {
        instance = this;
    }

    public void MakeNoise(Vector3 position, float volume)
    {
        KillerAI.globalNoisePosition = position;
        KillerAI.noiseHeard = true;
    }
}