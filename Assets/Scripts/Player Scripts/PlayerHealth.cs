using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public bool isDead = false;
    public float restartDelay = 2f;
    public float slowMotionScale = 0.4f;

    private Rigidbody rb;               // ✅ fixed: was CharacterController, but player uses Rigidbody
    private float deathTime = -1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isDead && deathTime > 0f)
        {
            if (Time.unscaledTime >= deathTime + restartDelay)
                RestartScene();
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("💀 Player Died");

        // ✅ Stop Rigidbody movement instead of CharacterController
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // Disable movement and noise scripts
        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null) movement.enabled = false;

        PlayerNoise noise = GetComponent<PlayerNoise>();
        if (noise != null) noise.enabled = false;

        // Slow motion death effect
        Time.timeScale = slowMotionScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        deathTime = Time.unscaledTime;
    }

    void RestartScene()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        KillerAI.noiseHeard = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}