using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VehicleController : MonoBehaviour
{
    public float maxSpeed = 10f;
    public float acceleration = 1f;
    public float deceleration = 2f;
    public float brakingDeceleration = 2f;
    public float rotationSpeed = 100f;
    public float maxGas = 100f;
    public float gasConsumptionRate = 1f;
    public float maxHealth = 100f;
    public float damageAmount = 10f;

    public float currentSpeed;
    private bool canDrive = false;
    private bool isWaitingToDrive = false;

    private bool isDriving = false;
    private float currentGas;
    private float currentHealth;
    private bool isDamaged = false;
    private bool isGrounded = false;

    public Slider gasSlider;
    public Slider healthSlider;

    public Light[] vehicleLights;
    private bool lightsEnabled = false;

    public Light[] additionalLights;
    private bool additionalLightsActive = false;

    public Transform player;
    public Transform exitPoint;
    private StaticCamera cameraScript;
    private SoundManager soundManager;

    void Start()
    {
        currentGas = maxGas;
        currentHealth = maxHealth;
        gasSlider.maxValue = maxGas;
        gasSlider.value = currentGas;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;

        player = GameObject.FindGameObjectWithTag("Player").transform;
        cameraScript = GameObject.FindObjectOfType<StaticCamera>();

        soundManager = SoundManager.instance;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isDriving || currentGas <= 0)
            {
                ExitVehicle();
            }
            else
            {
                if (IsPlayerNearExitPoint() && !isWaitingToDrive)
                {
                    EnterVehicle();
                    StartCoroutine(StartDrivingDelay());
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            ToggleVehicleLights();
        }

        if (isDriving)
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.1f);

            if (isGrounded)
            {
                HandleDrivingInput();
            }
            else
            {
                currentSpeed = 0f;
            }
        }
        else
        {
            currentSpeed = 0f;
        }
    }

    void HandleDrivingInput()
    {
        float translation = Input.GetAxis("Vertical");
        float rotation = 0f;

        if (Mathf.Abs(translation) > 0.1f || Mathf.Abs(currentSpeed) > 0.1f)
        {
            rotation = Input.GetAxis("Horizontal") * rotationSpeed;
        }

        if (translation > 0.1f)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);
            SetAdditionalLights(false);
        }
        else if (translation < -0.1f)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, -maxSpeed, brakingDeceleration * Time.deltaTime);
            SetAdditionalLights(true);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.deltaTime);
            SetAdditionalLights(false);
        }

        translation = currentSpeed * Time.deltaTime;
        transform.Translate(0, 0, translation);

        if (currentSpeed != 0)
        {
            transform.Rotate(0, rotation, 0);
        }

        currentGas -= gasConsumptionRate * Time.deltaTime;
        gasSlider.value = currentGas;

        if (currentGas <= 0)
        {
            isDriving = false;
            SetAdditionalLights(false);
        }
    }

    IEnumerator StartDrivingDelay()
    {
        isWaitingToDrive = true;
        soundManager.PlayCarStartUpSound();
        yield return new WaitForSeconds(4f);
        canDrive = true;
        isDriving = true;
        gasSlider.gameObject.SetActive(true);
        healthSlider.gameObject.SetActive(true);
        isWaitingToDrive = false;
    }

    private void SetAdditionalLights(bool active)
    {
        if (additionalLights != null && additionalLights.Length > 0)
        {
            additionalLightsActive = active;

            foreach (Light light in additionalLights)
            {
                light.enabled = additionalLightsActive;
            }
        }
    }

    private bool IsPlayerNearExitPoint()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        float interactDistanceThreshold = 7.0f;
        return distance <= interactDistanceThreshold;
    }

    private void EnterVehicle()
    {
        player.position = exitPoint.position;

        if (cameraScript != null)
        {
            cameraScript.SetCurrentVehicle(transform);
        }

        player.gameObject.SetActive(false);
        isDriving = true;
        gasSlider.gameObject.SetActive(true);
        healthSlider.gameObject.SetActive(true);
    }

    private void ExitVehicle()
    {
        player.position = GetExitPointPosition();
        player.gameObject.SetActive(true);
        isDriving = false;
        canDrive = false; // Reset the flag when exiting the vehicle
        gasSlider.gameObject.SetActive(false);
        healthSlider.gameObject.SetActive(false);

        if (cameraScript != null)
        {
            cameraScript.ClearCurrentVehicle();
        }
    }

    private Vector3 GetExitPointPosition()
    {
        Transform exitPointTransform = transform.Find("ExitPoint");
        if (exitPointTransform != null)
        {
            return exitPointTransform.position;
        }
        else
        {
            Debug.LogWarning("ExitPoint transform not found on the vehicle.");
            return transform.position;
        }
    }

    private void ToggleVehicleLights()
    {
        if (vehicleLights != null && vehicleLights.Length > 0)
        {
            lightsEnabled = !lightsEnabled;

            foreach (Light light in vehicleLights)
            {
                light.enabled = lightsEnabled;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            isDamaged = true;
            currentHealth -= damageAmount;
            healthSlider.value = currentHealth;

            if (currentHealth <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
