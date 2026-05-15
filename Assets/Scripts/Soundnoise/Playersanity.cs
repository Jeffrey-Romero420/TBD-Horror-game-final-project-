using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerSanity : MonoBehaviour
{
    [Header("Sanity")]
    public float maxSanity = 100f;
    public float currentSanity;
    public float sanityDrainRate = 10f;     // lost per second when killer is close
    public float sanityRegenRate = 5f;      // gained per second when killer is far
    public float killerDrainRange = 10f;    // distance at which sanity starts draining

    [Header("Effects")]
    public Volume postProcessVolume;         // assign URP Global Volume in Inspector
    public AudioSource sanityAudioSource;    // assign AudioSource for fake sounds
    public AudioClip[] fakeFootstepClips;   // assign some footstep sounds
    public LookAround lookAround;            // assign LookAround script

    [Header("Sensitivity Effect")]
    public float normalSensitivity = 360f;
    public float lowSanitySensitivity = 600f; // harder to control at low sanity

    [Header("Fake Sound Thresholds")]
    public float fakeSoundThreshold = 50f;   // sanity % below which fake sounds play
    public float fakeSoundCooldown = 8f;
    private float fakeSoundTimer;

    private Transform killer;
    private Vignette vignette;
    private DepthOfField dof;
    private float sanityPercent => currentSanity / maxSanity;

    void Start()
    {
        currentSanity = maxSanity;

        // Find killer by tag
        GameObject killerObj = GameObject.FindGameObjectWithTag("Killer");
        if (killerObj != null) killer = killerObj.transform;

        // Get post processing effects
        if (postProcessVolume != null)
        {
            postProcessVolume.profile.TryGet(out vignette);
            postProcessVolume.profile.TryGet(out dof);
        }

        fakeSoundTimer = fakeSoundCooldown;
    }

    void Update()
    {
        HandleSanityDrain();
        HandleEffects();
        HandleFakeSounds();
    }

    void HandleSanityDrain()
    {
        if (killer == null) return;

        float distToKiller = Vector3.Distance(transform.position, killer.position);

        if (distToKiller <= killerDrainRange)
        {
            // ✅ Drain faster the closer the killer is
            float drainMultiplier = 1f - (distToKiller / killerDrainRange);
            currentSanity -= sanityDrainRate * drainMultiplier * Time.deltaTime;
        }
        else
        {
            // Regen when killer is far
            currentSanity += sanityRegenRate * Time.deltaTime;
        }

        currentSanity = Mathf.Clamp(currentSanity, 0f, maxSanity);
    }

    void HandleEffects()
    {
        float insanity = 1f - sanityPercent; // 0 = sane, 1 = insane

        // ✅ Vignette gets darker as sanity drops
        if (vignette != null)
        {
            vignette.active = true;
            vignette.intensity.value = Mathf.Lerp(0.2f, 0.8f, insanity);
            vignette.color.value = Color.black;
        }

        // ✅ Blur gets worse as sanity drops
        if (dof != null)
        {
            dof.active = insanity > 0.4f;
            if (dof.active)
                dof.gaussianMaxRadius.value = Mathf.Lerp(0f, 3f, insanity);
        }

        // ✅ Mouse sensitivity gets harder to control at low sanity
        if (lookAround != null)
        {
            lookAround.sensitivity = Mathf.Lerp(
                normalSensitivity, lowSanitySensitivity, insanity);
        }
    }

    void HandleFakeSounds()
    {
        if (currentSanity > fakeSoundThreshold) return;
        if (fakeFootstepClips == null || fakeFootstepClips.Length == 0) return;
        if (sanityAudioSource == null) return;

        fakeSoundTimer -= Time.deltaTime;

        if (fakeSoundTimer <= 0f)
        {
            // ✅ Play a random fake footstep from a random direction
            AudioClip clip = fakeFootstepClips[Random.Range(0, fakeFootstepClips.Length)];
            sanityAudioSource.panStereo = Random.Range(-1f, 1f); // left or right ear
            sanityAudioSource.PlayOneShot(clip);

            // Reset timer with some randomness so it doesn't feel mechanical
            fakeSoundTimer = fakeSoundCooldown + Random.Range(-2f, 3f);

            Debug.Log("👻 Fake footstep played");
        }
    }
}