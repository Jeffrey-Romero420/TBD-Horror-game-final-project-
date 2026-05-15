using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    [Header("Settings")]
    public float interactDistance = 2f;
    public Transform hidePosition;
    public bool isSafe = true;

    [Header("Detection while hiding")]
    public float noiseDetectionRange = 5f;

    private Transform player;
    private PlayerMovement playerMovement;
    private PlayerNoise playerNoise;
    private LookAround playerLook;
    private CharacterController characterController;
    private bool playerIsHiding = false;
    private Vector3 exitPosition;

    private float hideTime = -1f;
    private float exitDelay = 0.3f;

    public static bool playerHiding = false;
    public static bool isSafeHide = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerMovement = playerObj.GetComponent<PlayerMovement>();
            playerNoise = playerObj.GetComponent<PlayerNoise>();
            playerLook = playerObj.GetComponent<LookAround>();
            characterController = playerObj.GetComponent<CharacterController>();
        }
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (!playerIsHiding)
        {
            if (dist <= interactDistance && Input.GetKeyDown(KeyCode.E))
                EnterHidingSpot();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E) && Time.time >= hideTime + exitDelay)
                ExitHidingSpot();

            // ✅ Keep player locked to hide position
            if (characterController != null)
            {
                characterController.enabled = false;
                player.position = hidePosition.position;
            }
        }
    }

    void EnterHidingSpot()
    {
        if (hidePosition == null)
        {
            Debug.LogWarning("HidingSpot: hidePosition not assigned on " + gameObject.name);
            return;
        }

        playerIsHiding = true;
        playerHiding = true;
        isSafeHide = isSafe;
        hideTime = Time.time;

        exitPosition = player.position;

        // ✅ Disable CharacterController before moving player
        if (characterController != null)
            characterController.enabled = false;

        player.position = hidePosition.position;
        player.rotation = hidePosition.rotation;

        if (playerMovement != null) playerMovement.enabled = false;
        if (playerNoise != null) playerNoise.enabled = false;
        if (playerLook != null) playerLook.enabled = false;

        Debug.Log("Player hiding in " + gameObject.name);
    }

    void ExitHidingSpot()
    {
        playerIsHiding = false;
        playerHiding = false;
        isSafeHide = false;

        player.position = exitPosition;

        // ✅ Re-enable CharacterController on exit
        if (characterController != null)
            characterController.enabled = true;

        if (playerMovement != null) playerMovement.enabled = true;
        if (playerNoise != null) playerNoise.enabled = true;
        if (playerLook != null) playerLook.enabled = true;

        Debug.Log("Player exited hiding spot");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}