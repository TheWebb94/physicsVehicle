using UnityEngine;
using UnityEngine.Rendering.UI;

public class VehicleController : MonoBehaviour
{
    [Header("Throttle / Brake")] [Range(0f, 1f)]
    public float throttle;

    public float throttleAccellaration = 0.8f;
    public float throttleDecay = 0.6f;
    public float brakeFactor = 2f;


    [Header("Steering")] [Range(-1f, 1f)] public float steering;
    //public float steerRate = 2.5f;
    //public float steerReturnRate = 3.0f;    3 variables not inn use? old iplmentaion
    //public float steerDeadzone = 0.02f;

    // Movement parameters
<<<<<<< HEAD
    public float motorForce = 15000f; // Force applied when accelerating
    public float maxSpeed = 50f; // Maximum vehicle speed
    public float steeringSpeed = 4f; // How fast steering adjusts
    public float maxSteeringAngle = 30f; // Maximum wheel turn angle in degrees
    public float turningForce = 800f; // Lateral force for turning
=======
    public float motorForce = 15000f;          // Force applied when accelerating
    public float maxSpeed = 50f;               // Maximum vehicle speed
    public float steeringSpeed = 4f;           // How fast steering adjusts (increased for better response)
    public float maxSteeringAngle = 30f;       // Maximum wheel turn angle in degrees
    public float turningForce = 800f;          // Lateral force for turning (increased for sharper turns)
>>>>>>> origin/claude/plan-next-features-01CCqJKVgSBJFq6Ve3ncmehC
    private float maximumReverseSpeed = -0.35f;

    [Header("Handbrake")] 
    public bool handbrake;

    [Header("Angular Drag")]
<<<<<<< HEAD
    [SerializeField] private Vector3 customAngularDrag = new Vector3(5f, 2f, 5f); 
    [SerializeField] private bool useCustomAngularDrag = true;
    
=======
    [SerializeField] private Vector3 customAngularDrag = new Vector3(5f, 2f, 5f); // Roll, Yaw, Pitch
    [SerializeField] private bool useCustomAngularDrag = true;

>>>>>>> origin/claude/plan-next-features-01CCqJKVgSBJFq6Ve3ncmehC
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
        // Set drag values
        rb.linearDamping = 0.5f;   // Slight linear drag for stability
        
        // Only use built-in angular damping if not using custom
        if (!useCustomAngularDrag)
        {
<<<<<<< HEAD
            rb.angularDamping = 3.5f;  // basic angular drag for scenery/world objects/ player
        }
        else
        {
            rb.angularDamping = 0f;    // Disable built-in for use on car 
=======
            rb = GetComponent<Rigidbody>();
            player = GameObject.FindGameObjectWithTag("Player");
            playerController = player.GetComponent<CharacterController>();

            // Set drag values
            rb.linearDamping = 0.5f;   // Slight linear drag for stability

            // Only use built-in angular damping if not using custom
            if (!useCustomAngularDrag)
            {
                rb.angularDamping = 3.5f;  // Fallback: basic angular drag
            }
            else
            {
                rb.angularDamping = 0f;    // Disable built-in, we'll handle it ourselves
            }
        }

    void FixedUpdate()
    {
        // Apply custom angular drag every physics frame
        if (useCustomAngularDrag)
        {
            ApplyCustomAngularDrag();
>>>>>>> origin/claude/plan-next-features-01CCqJKVgSBJFq6Ve3ncmehC
        }
    }

    public void UseVehicleController(GameObject currentVehicle)
    {
        playerIsInCar = true;

        HandleAccelleration();

        HandleDecelleration();

        HandleSteering();

        HandleHandbrake();

        ApplyMovementForces();

        ApplyCustomAngularDrag();

        HandleExitingOfVehicle();
        
    }

    private void ApplyCustomAngularDrag()
    {
        if (rb == null) return;
        
        // Get current angular velocity
        Vector3 angularVelocity = rb.angularVelocity;

        // Apply drag torque opposing the rotation
        Vector3 dampingTorque = -Vector3.Scale(angularVelocity, customAngularDrag);

        // Apply the torque
        rb.AddTorque(dampingTorque, ForceMode.Acceleration);
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
            handbrake = true;
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

        // Apply forward force based on throttle - only when wheels are grounded
        // Count grounded wheels
        int groundedWheelCount = 0;

        foreach (var wheel in wheels)
        {
<<<<<<< HEAD
            if (wheel.isGrounded)
            {
                groundedWheelCount++;
            }
=======
            // Detect if moving forward or backward
            float forwardDot = Vector3.Dot(rb.linearVelocity.normalized, transform.forward);
            bool isMovingBackward = forwardDot < 0;

            // Invert steering when moving backward (realistic car behavior)
            float effectiveSteering = isMovingBackward ? -steering : steering;

            // Calculate the steering angle in degrees
            float steeringAngle = effectiveSteering * maxSteeringAngle;

            // Calculate turning force - stronger when moving faster
            float speedFactor = Mathf.Clamp01(currentSpeed / maxSpeed);
            Vector3 lateralForce = transform.right * effectiveSteering * turningForce * speedFactor * Time.deltaTime;

            // Apply the lateral force for turning
            rb.AddForce(lateralForce, ForceMode.Acceleration);

            // Also rotate the vehicle body based on steering
            float rotationAmount = effectiveSteering * currentSpeed * Time.deltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, rotationAmount, 0f));
>>>>>>> origin/claude/plan-next-features-01CCqJKVgSBJFq6Ve3ncmehC
        }

        // Only apply force if at least one wheel is grounded
        if (groundedWheelCount > 0)
        {
            // Apply force at center of mass to avoid unwanted torque
            Vector3 forwardForce = transform.forward * motorForce * throttle * Time.deltaTime;

            rb.AddForce(forwardForce, ForceMode.Acceleration);

            // Apply turning force when steering and moving
            if (Mathf.Abs(steering) > 0.01f && currentSpeed > 0.5f)
            {
                // Detect if moving forward or backward

                float forwardDot = Vector3.Dot(rb.linearVelocity.normalized, transform.forward);

                bool isMovingBackward = forwardDot < 0;

 

                // Invert steering when moving backward (realistic car behavior)

                float effectiveSteering = isMovingBackward ? -steering : steering;

 

                // Calculate the steering angle in degrees
                float steeringAngle = effectiveSteering * maxSteeringAngle;
                
                // Calculate turning force - stronger when moving faster
                float speedFactor = Mathf.Clamp01(currentSpeed / maxSpeed);
                Vector3 lateralForce = transform.right * effectiveSteering * turningForce * speedFactor * Time.deltaTime;

                // Apply the lateral force for turning
                rb.AddForce(lateralForce, ForceMode.Acceleration);
                
                // Also rotate the vehicle body based on steering
                float rotationAmount = effectiveSteering * currentSpeed * Time.deltaTime;
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

    private void ApplyCustomAngularDrag()
    {
        if (rb == null) return;

        // Get current angular velocity
        Vector3 angularVelocity = rb.angularVelocity;

        // Apply drag torque opposing the rotation
        // Per-axis damping: different values for roll (X), yaw (Y), and pitch (Z)
        Vector3 dampingTorque = -Vector3.Scale(angularVelocity, customAngularDrag);

        // Apply the torque
        rb.AddTorque(dampingTorque, ForceMode.Acceleration);
    }
}

