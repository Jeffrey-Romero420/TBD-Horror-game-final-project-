using UnityEngine;

public class PlayerNoise : MonoBehaviour
{
    public float noiseRadius = 8f;
    public float noiseSpread = 2f;
    public float moveThreshold = 0.2f;
    public float noiseCooldown = 1.2f;

    // ✅ Crouching makes much less noise
    public float crouchMoveThreshold = 0.05f; // harder to trigger noise while crouched
    public float crouchNoiseCooldown = 2.5f;  // longer cooldown between noises while crouched
    public float crouchNoiseSpread = 0.5f;    // noise stays closer to player when crouched

    private Vector3 lastPosition;
    private float lastNoiseTime;
    private CrouchScript crouchScript;

    void Start()
    {
        lastPosition = transform.position;
        crouchScript = GetComponent<CrouchScript>();
    }

    void Update()
    {
        bool crouching = crouchScript != null && crouchScript.isCrouching;

        float threshold = crouching ? crouchMoveThreshold : moveThreshold;
        float cooldown = crouching ? crouchNoiseCooldown : noiseCooldown;

        float distanceMoved = Vector3.Distance(transform.position, lastPosition);

        if (distanceMoved > threshold && Time.time > lastNoiseTime + cooldown)
        {
            MakeNoise(crouching);
            lastNoiseTime = Time.time;
            lastPosition = transform.position;
        }
    }

    void MakeNoise(bool crouching)
    {
        Debug.Log(crouching ? "🔊 Quiet noise (crouching)" : "🔊 Noise made");

        float spread = crouching ? crouchNoiseSpread : noiseSpread;
        Vector3 noiseOffset = Random.insideUnitSphere * spread;
        noiseOffset.y = 0f;

        KillerAI.globalNoisePosition = transform.position + noiseOffset;
        KillerAI.noiseHeard = true;
    }

    // Call this from other scripts (footsteps, dropped objects, etc.)
    public void MakeNoiseAt(Vector3 position, float radius)
    {
        KillerAI.globalNoisePosition = position;
        KillerAI.noiseHeard = true;
    }
}