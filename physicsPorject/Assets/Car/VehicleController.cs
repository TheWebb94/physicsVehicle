using UnityEngine;

public class VehicleController : MonoBehaviour
{
    [Header("Throttle / Brake")]
    [Range(0f, 1f)] public float throttle;          
    public float throttleAccellaration = 0.8f;       
    public float throttleDecay = 0.6f;
    public float brakeFactor = 2f;                   
    

    [Header("Steering")]
    [Range(-1f, 1f)] public float steering;          
    public float steerRate = 2.5f;
    public float steerReturnRate = 3.0f;
    public float steerDeadzone = 0.02f;

    // Movement parameters
    public float motorForce = 15000f;          // Force applied when accelerating
    public float maxSpeed = 50f;               // Maximum vehicle speed
    public float steeringSpeed = 2f;           // How fast steering adjusts
    public float maxSteeringAngle = 30f;       // Maximum wheel turn angle in degrees
    public float turningForce = 500f;          // Lateral force for turning
    private float maximumReverseSpeed = -0.35f;
    
    [Header("Handbrake")]
    public bool handbrake;

    
    private Rigidbody rb;
    public bool playerIsInCar = false;
    private GameObject player;
    private CharacterController playerController;
    [SerializeField] private GameObject exitLocation;
    private Wheel[] wheels;
    

    void Start()
        {
            rb = GetComponent<Rigidbody>();
            player = GameObject.FindGameObjectWithTag("Player");
            playerController = player.GetComponent<CharacterController>();
            wheels = GetComponentsInChildren<Wheel>();
        }
    
    public void UseVehicleController(GameObject currentVehicle)
    {
        playerIsInCar = true;
        
        HandleAccelleration();

        HandleDecelleration();

        HandleSteering();

        HandleHandbrake();
        
        ApplyMovementForces();

        HandleExitingOfVehicle();

        Debug.Log(throttle);
    }

    private void HandleExitingOfVehicle()
    {
        if (Input.GetKey(KeyCode.P))
        {
            playerIsInCar = false;
            playerController.isInCar = false;
            player.transform.position = exitLocation.transform.position;
            playerController.currentVehicle = null;
            playerController.mesh.SetActive(true);
            playerController.rb.isKinematic = false;
        }
    }


    private void HandleHandbrake()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            handbrake  = true;  
        }
    }

    private void HandleSteering()
    {
        if (Input.GetKey(KeyCode.A))
        {
            steering -= steeringSpeed * Time.deltaTime;
            steering = Mathf.Clamp(steering, -1f, 1f);
        }
        // Steering input - D key turns right (positive steering)
        else if (Input.GetKey(KeyCode.D))
        {
            steering += steeringSpeed * Time.deltaTime;
            steering = Mathf.Clamp(steering, -1f, 1f);
        }
        // Return steering to center when no input
        else
        {
            if (Mathf.Abs(steering) > 0.01f)
            {
                steering = Mathf.Lerp(steering, 0f, steeringSpeed * Time.deltaTime);
            }
            else
            {
                steering = 0f;
            }
        }
    }

    private void HandleDecelleration()
    {
        //decay throttle if no input
        if (throttle < 0f)
        {
            throttle += throttleDecay * Time.deltaTime;
        }
        // decelerate by decreasing throttle
        if (Input.GetKey(KeyCode.S))
        {
            if (throttle <= 0)
            {
                throttle -= throttleAccellaration * Time.deltaTime;
                if (throttle < maximumReverseSpeed)
                {
                    throttle = maximumReverseSpeed;
                }
            }
            else
            {
                throttle -= throttleAccellaration * brakeFactor * Time.deltaTime;
            }
        }
    }

    private void HandleAccelleration()
    {
        //decay throttle if not accellerating
        if (throttle > 0f)
        {
            throttle -= throttleDecay * Time.deltaTime;
        }

        //accellerate by increasing throttle
        if (Input.GetKey(KeyCode.W))
        {
            throttle += throttleAccellaration * Time.deltaTime;
            if (throttle > 1f)
            {
                throttle = 1f;
            }

        }
    }

    private void ApplyMovementForces()
    {
        if (rb == null) return;

        // Calculate current speed
        float currentSpeed = rb.linearVelocity.magnitude;

        // Apply forward force based on throttle - only at grounded wheels
        // Count grounded wheels
        int groundedWheelCount = 0;
        foreach (var wheel in wheels)
        {
            if (wheel.IsGrounded)
            {
                groundedWheelCount++;
            }
        }

        // Only apply force if at least one wheel is grounded
        if (groundedWheelCount > 0)
        {
            // Divide force equally among grounded wheels
            float forcePerWheel = motorForce * throttle * Time.deltaTime / groundedWheelCount;
            Vector3 forwardForce = transform.forward * forcePerWheel;

            // Apply force at each grounded wheel position
            foreach (var wheel in wheels)
            {
                if (wheel.IsGrounded)
                {
                    rb.AddForceAtPosition(forwardForce, wheel.ContactPoint, ForceMode.Acceleration);
                }
            }
        }
        

        // Apply turning force when steering and moving
        if (Mathf.Abs(steering) > 0.01f && currentSpeed > 0.5f)
        {
            // Calculate the steering angle in degrees
            float steeringAngle = steering * maxSteeringAngle;

            // Calculate turning force - stronger when moving faster
            float speedFactor = Mathf.Clamp01(currentSpeed / maxSpeed);
            Vector3 lateralForce = transform.right * steering * turningForce * speedFactor * Time.deltaTime;

            // Apply the lateral force for turning
            rb.AddForce(lateralForce, ForceMode.Acceleration);

            // Also rotate the vehicle body based on steering
            float rotationAmount = steering * currentSpeed * Time.deltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, rotationAmount, 0f));
        }

        // Apply handbrake - reduce velocity
        if (handbrake)
        {
            rb.linearVelocity *= 0.95f; // Reduce velocity by 5% each frame
        }

        // Clamp speed to max speed
        if (currentSpeed > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }
}

