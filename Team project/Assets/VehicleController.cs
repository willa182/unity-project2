using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VehicleController : MonoBehaviour
{
    public float maxSpeed = 10f;
    public float acceleration = 1f;
    public float rotationSpeed = 100f;
    public float maxGas = 100f;
    public float gasConsumptionRate = 1f;
    public float maxHealth = 100f;
    public float damageAmount = 10f;

    private bool isDriving = false;
    private float currentGas;
    private float currentHealth;
    private bool isDamaged = false;
    private float currentSpeed = 0f;

    public Slider gasSlider;
    public Slider healthSlider;
    public Light[] vehicleLights;
    private bool lightsEnabled = false;

    public Transform player;
    public Transform exitPoint;
    private StaticCamera cameraScript;

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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isDriving || currentGas <= 0)
            {
                // If already driving, exit the vehicle
                ExitVehicle();
            }
            else
            {
                // If not driving, check if the player is near the exit point and enter the vehicle
                if (IsPlayerNearExitPoint())
                {
                    EnterVehicle();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            ToggleVehicleLights();
        }

        if (isDriving)
        {
            float translation = Input.GetAxis("Vertical");
            float rotation = Input.GetAxis("Horizontal") * rotationSpeed;

            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);
            translation *= currentSpeed * Time.deltaTime;

            transform.Translate(0, 0, translation);
            transform.Rotate(0, rotation, 0);

            currentGas -= gasConsumptionRate * Time.deltaTime;
            gasSlider.value = currentGas;

            if (currentGas <= 0)
            {
                isDriving = false;
            }
        }
    }

    private bool IsPlayerNearExitPoint()
    {
        // Calculate the distance between the player and the exit point
        float distance = Vector3.Distance(player.position, transform.position);

        // Adjust this distance threshold as needed
        float interactDistanceThreshold = 7.0f;

        return distance <= interactDistanceThreshold;
    }

    private void EnterVehicle()
    {
        // Set the player's position to the exit point
        player.position = exitPoint.position;

        // Notify the camera script that the player is now in this vehicle
        if (cameraScript != null)
        {
            cameraScript.SetCurrentVehicle(transform);
        }

        player.gameObject.SetActive(false); // Hide the player GameObject
        isDriving = true;
        gasSlider.gameObject.SetActive(true);
        healthSlider.gameObject.SetActive(true);
    }

    private void ExitVehicle()
    {
        player.position = GetExitPointPosition(); // Set player's position to the exit point
        player.gameObject.SetActive(true); // Show the player GameObject
        isDriving = false;
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
            return transform.position; // Fallback to the vehicle's position
        }
    }

    private void ToggleVehicleLights()
    {
        // Check if there are lights on the vehicle
        if (vehicleLights != null && vehicleLights.Length > 0)
        {
            // Toggle the lights
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