using UnityEngine;
using UnityEngine.UI;

public class Flashlight : MonoBehaviour
{
    [Header("Flashlight")]
    public Light flashlightLight;
    public KeyCode toggleKey = KeyCode.F;
    public bool hasFlashlight = false;    // ✅ starts false until picked up

    [Header("Pickup")]
    public float pickupDistance = 2.5f;
    public GameObject flashlightWorldObject; // the flashlight prop sitting in the world

    [Header("Battery")]
    public float maxBattery = 100f;
    public float drainRate = 5f;
    public float rechargeRate = 5f;
    public bool rechargesWhenOff = true;

    [Header("Flicker")]
    public bool flickerWhenLow = true;
    public float flickerThreshold = 20f;
    public float flickerSpeed = 0.05f;

    [Header("UI")]
    public Slider batterySlider;
    public GameObject batteryUI;          // the whole battery UI — hidden until picked up

    private bool isOn = false;
    private float currentBattery;
    private float flickerTimer;
    private Transform player;

    void Start()
    {
        currentBattery = maxBattery;

        if (flashlightLight != null)
            flashlightLight.enabled = false;

        // Hide battery UI until flashlight is picked up
        if (batteryUI != null)
            batteryUI.SetActive(false);

        if (batterySlider != null)
        {
            batterySlider.maxValue = maxBattery;
            batterySlider.value = maxBattery;
        }

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        // ✅ Check for pickup if not already collected
        if (!hasFlashlight)
        {
            if (flashlightWorldObject != null && player != null)
            {
                float dist = Vector3.Distance(player.position, flashlightWorldObject.transform.position);

                if (dist <= pickupDistance && Input.GetKeyDown(KeyCode.E))
                {
                    PickupFlashlight();
                }
            }
            return; // don't do anything else until picked up
        }

        // Toggle on/off
        if (Input.GetKeyDown(toggleKey))
        {
            if (!isOn && currentBattery <= 0f)
            {
                Debug.Log("Flashlight battery dead!");
                return;
            }

            isOn = !isOn;

            if (flashlightLight != null)
                flashlightLight.enabled = isOn;
        }

        // Battery drain
        if (isOn && currentBattery > 0f)
        {
            currentBattery -= drainRate * Time.deltaTime;

            // Flicker when low
            if (flickerWhenLow && currentBattery <= flickerThreshold)
            {
                flickerTimer -= Time.deltaTime;
                if (flickerTimer <= 0f)
                {
                    flashlightLight.enabled = !flashlightLight.enabled;
                    flickerTimer = flickerSpeed + Random.Range(0f, 0.1f);
                }
            }

            // Battery dead
            if (currentBattery <= 0f)
            {
                currentBattery = 0f;
                isOn = false;

                if (flashlightLight != null)
                    flashlightLight.enabled = false;

                Debug.Log("Flashlight battery dead!");
            }
        }

        // Recharge when off
        if (!isOn && rechargesWhenOff && currentBattery < maxBattery)
        {
            currentBattery += rechargeRate * Time.deltaTime;
            currentBattery = Mathf.Clamp(currentBattery, 0f, maxBattery);
        }

        if (batterySlider != null)
            batterySlider.value = currentBattery;
    }

    void PickupFlashlight()
    {
        hasFlashlight = true;

        // Hide the world flashlight prop
        if (flashlightWorldObject != null)
            flashlightWorldObject.SetActive(false);

        // Show battery UI
        if (batteryUI != null)
            batteryUI.SetActive(true);

        Debug.Log("Flashlight picked up!");
    }
}