using UnityEngine;

public class KillerNoise : MonoBehaviour
{
    public static Vector3 noisePosition;
    public static bool noiseHeard;

    public static void MakeNoise(Vector3 pos)
    {
        noisePosition = pos;
        noiseHeard = true;
    }
}