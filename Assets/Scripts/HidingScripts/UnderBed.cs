using UnityEngine;

public class UnderBed : MonoBehaviour
{
    [Header("Settings")]
    public float interactDistance = 2f;
    public Transform hidePosition;

    [Header("Detection while hiding")]
    public float killerDetectDistance = 2.5f;

    private Transform player;
    private PlayerMovement playerMovement;
    private PlayerNoise playerNoise;
    private CapsuleCollider capsuleCollider;
    private bool playerIsHiding = false;
    private Vector3 exitPosition;

    // Store original collider values to restore on exit
    private float originalColliderHeight;
    private Vector3 originalColliderCenter;
    private Vector3 originalScale;

    private float hideTime = -1f;
    private float exitDelay = 0.3f;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerMovement = playerObj.GetComponent<PlayerMovement>();
            playerNoise = playerObj.GetComponent<PlayerNoise>();
            capsuleCollider = playerObj.GetComponent<CapsuleCollider>();

            if (capsuleCollider != null)
            {
                originalColliderHeight = capsuleCollider.height;
                originalColliderCenter = capsuleCollider.center;
            }

            originalScale = playerObj.transform.localScale;
        }
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (!playerIsHiding)
        {
            if (dist <= interactDistance && Input.GetKeyDown(KeyCode.E))
                EnterBed();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E) && Time.time >= hideTime + exitDelay)
                ExitBed();

            HidingSpot.playerHiding = true;
            HidingSpot.isSafeHide = false;

            GameObject killerObj = GameObject.FindGameObjectWithTag("Killer");
            if (killerObj != null)
            {
                float killerDist = Vector3.Distance(killerObj.transform.position, player.position);
                if (killerDist <= killerDetectDistance)
                {
                    Debug.Log("Killer found player under bed!");
                    ExitBed();

                    PlayerHealth health = player.GetComponent<PlayerHealth>();
                    if (health != null) health.Die();
                }
            }
        }
    }

    void EnterBed()
    {
        if (hidePosition == null)
        {
            Debug.LogWarning("UnderBed: hidePosition not assigned on " + gameObject.name);
            return;
        }

        playerIsHiding = true;
        HidingSpot.playerHiding = true;
        HidingSpot.isSafeHide = false;
        hideTime = Time.time;

        exitPosition = player.position;
        player.position = hidePosition.position;

        // ✅ Flatten the capsule collider so player fits under the bed
        if (capsuleCollider != null)
        {
            capsuleCollider.height = 0.4f;
            capsuleCollider.center = new Vector3(0, 0.2f, 0);
        }

        // ✅ Flatten the player scale to look like they're lying down
        player.localScale = new Vector3(originalScale.x, 0.25f, originalScale.z);

        if (playerMovement != null) playerMovement.enabled = false;
        if (playerNoise != null) playerNoise.enabled = false;

        Debug.Log("Player hiding under bed");
    }

    void ExitBed()
    {
        playerIsHiding = false;
        HidingSpot.playerHiding = false;

        player.position = exitPosition;

        // ✅ Restore original collider and scale
        if (capsuleCollider != null)
        {
            capsuleCollider.height = originalColliderHeight;
            capsuleCollider.center = originalColliderCenter;
        }

        player.localScale = originalScale;

        if (playerMovement != null) playerMovement.enabled = true;
        if (playerNoise != null) playerNoise.enabled = true;

        Debug.Log("Player exited from under bed");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, killerDetectDistance);
    }
}