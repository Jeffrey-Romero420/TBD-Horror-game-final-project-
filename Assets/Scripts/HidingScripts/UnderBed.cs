using UnityEngine;

public class UnderBed : MonoBehaviour
{
    [Header("Settings")]
    public float interactDistance = 2f;
    public Transform hidePosition;
    public Transform exitPoint;

    [Header("Detection while hiding")]
    public float killerDetectDistance = 2.5f;

    private Transform player;
    private PlayerMovement playerMovement;
    private PlayerNoise playerNoise;
    private CharacterController characterController;
    private bool playerIsHiding = false;
    private Vector3 exitPosition;

    private float hideTime = -1f;
    private float exitDelay = 0.3f;

    private Transform playerCamera;
    private Vector3 originalCameraLocalPos;

    private float originalCCHeight;
    private Vector3 originalCCCenter;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerMovement = playerObj.GetComponent<PlayerMovement>();
            playerNoise = playerObj.GetComponent<PlayerNoise>();
            characterController = playerObj.GetComponent<CharacterController>();

            Camera cam = playerObj.GetComponentInChildren<Camera>();
            if (cam != null)
            {
                playerCamera = cam.transform;
                originalCameraLocalPos = cam.transform.localPosition;
            }

            if (characterController != null)
            {
                originalCCHeight = characterController.height;
                originalCCCenter = characterController.center;
            }
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
            {
                ExitBed();
                return; // ✅ return immediately so nothing below runs after exit
            }

            HidingSpot.playerHiding = true;
            HidingSpot.isSafeHide = false;

            // Killer found you if too close
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

        // Disable CC, move player, shrink CC
        if (characterController != null)
            characterController.enabled = false;

        player.position = hidePosition.position;

        if (characterController != null)
        {
            characterController.height = 0.3f;
            characterController.center = new Vector3(0, 0.15f, 0);
        }

        if (playerCamera != null)
            playerCamera.localPosition = new Vector3(
                originalCameraLocalPos.x, -0.3f, originalCameraLocalPos.z);

        if (playerMovement != null) playerMovement.enabled = false;
        if (playerNoise != null) playerNoise.enabled = false;

        Debug.Log("Player hiding under bed");
    }

    void ExitBed()
    {
        playerIsHiding = false;
        HidingSpot.playerHiding = false;

        // Restore camera
        if (playerCamera != null)
            playerCamera.localPosition = originalCameraLocalPos;

        // Restore CC size
        if (characterController != null)
        {
            characterController.height = originalCCHeight;
            characterController.center = originalCCCenter;
        }

        // ✅ Move to exit point while CC is still disabled
        Vector3 targetExit = (exitPoint != null) ? exitPoint.position : exitPosition;
        player.position = targetExit;

        // ✅ Re-enable CC AFTER player is at exit position
        if (characterController != null)
            characterController.enabled = true;

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