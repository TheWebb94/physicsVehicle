using UnityEngine;

 

public class Wheel : MonoBehaviour
{
    [Header("Model")]
    [SerializeField] private GameObject wheelModel;
    [SerializeField] WheelType wheelType;

    [Header("Suspension")]
<<<<<<< HEAD
    [SerializeField] private float restLength = 0.8f;        // meters
    [SerializeField] private float springStrength = 15000f;   // N/m
=======
    [SerializeField] private float restLength = 0.35f;        // meters
    [SerializeField] private float springStrength = 35000f;   // N/m
>>>>>>> origin/claude/plan-next-features-01CCqJKVgSBJFq6Ve3ncmehC
    [SerializeField] private float damperStrength = 4500f;    // N·s/m
    [SerializeField] private float wheelRadius = 0.34f;       // meters
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private bool drawDebug;

    private Rigidbody carBody;
    private GameObject wheelVisual;
    private float lastLength;
    
    private float wheelRotationAngle = 0f;
    private VehicleController vc;

    // Public properties for force application
    public bool isGrounded { get; private set; }
    public Vector3 ContactPoint { get; private set; }

    private float wheelRotationAngle = 0f;
    private VehicleController vc;

    // Public properties for force application
    public bool isGrounded { get; private set; }
    public Vector3 ContactPoint { get; private set; }

    private void Awake()
    {
        // Find the car rigidbody on a parent
        carBody = GetComponentInParent<Rigidbody>();
        vc = GetComponentInParent<VehicleController>();
    }

    private void Start()
    {
        // Instantiate the visual for the wheel
        wheelVisual = Instantiate(wheelModel, transform);
        wheelVisual.transform.localPosition = Vector3.zero;
        wheelVisual.transform.localRotation = GetRotationForWheelType();
        wheelVisual.transform.localScale = Vector3.one;

        // Initialize lastLength to rest so damper starts calm
        lastLength = restLength;
    }

    private void FixedUpdate()

    {
        ApplySuspensionForces();
        UpdateWheelRotation();
<<<<<<< HEAD
=======
    }

    private void UpdateWheelRotation()
    {
        if (wheelVisual == null || carBody == null) return;

        // Calculate rolling rotation based on velocity
        // Distance traveled = velocity * time
        // Rotation angle = distance / radius (in radians, then convert to degrees)
        Vector3 localVelocity = transform.InverseTransformDirection(carBody.linearVelocity);

        float distanceTraveled = localVelocity.z * Time.fixedDeltaTime;
        float rotationDelta = (distanceTraveled / wheelRadius) * Mathf.Rad2Deg;

        wheelRotationAngle += rotationDelta;

        // Get steering angle (only for front wheels)
        float steeringAngle = 0f;
        if (vc != null && IsFrontWheel())
        {
            steeringAngle = vc.steering * vc.maxSteeringAngle;
        }

        // Apply rotation based on wheel type
        // Left wheels: rotate around local X-axis for rolling, local Y-axis for steering
        // Right wheels: need negated rotation due to 180-degree flip
        Quaternion steeringRotation = Quaternion.Euler(0f, steeringAngle, 0f);

        if (wheelType == WheelType.FrontRight || wheelType == WheelType.BackRight)
        {
            // Right side wheels are flipped 180 degrees on Y-axis, so negate the roll
            Quaternion rollingRotation = Quaternion.Euler(-wheelRotationAngle, 0f, 0f);
            wheelVisual.transform.localRotation = Quaternion.Euler(0f, 180f, 0f) * steeringRotation * rollingRotation;
        }
        else
        {
            // Left side wheels
            Quaternion rollingRotation = Quaternion.Euler(wheelRotationAngle, 0f, 0f);
            wheelVisual.transform.localRotation = steeringRotation * rollingRotation;
        }
>>>>>>> origin/claude/plan-next-features-01CCqJKVgSBJFq6Ve3ncmehC
    }

 

    private void UpdateWheelRotation()

    {
        if (wheelVisual == null || carBody == null) return;

        // Calculate rolling rotation based on velocity
        // Distance traveled = velocity * time
        // Rotation angle = distance / radius (in radians, then convert to degrees)
        Vector3 localVelocity = transform.InverseTransformDirection(carBody.linearVelocity);

        float distanceTraveled = localVelocity.z * Time.fixedDeltaTime;
        float rotationDelta = (distanceTraveled / wheelRadius) * Mathf.Rad2Deg;

        wheelRotationAngle += rotationDelta;

        // Get steering angle (only for front wheels)
        float steeringAngle = 0f;
        if (vc != null && IsFrontWheel())
        {
            steeringAngle = vc.steering * vc.maxSteeringAngle;
        }

        // Apply rotation based on wheel type
        // Left wheels: rotate around local X-axis for rolling, local Y-axis for steering
        // Right wheels: need negated rotation due to 180-degree flip
        Quaternion steeringRotation = Quaternion.Euler(0f, steeringAngle, 0f);

        if (wheelType == WheelType.FrontRight || wheelType == WheelType.BackRight)
        {
            // Right side wheels are flipped 180 degrees on Y-axis, so negate the roll
            Quaternion rollingRotation = Quaternion.Euler(-wheelRotationAngle, 0f, 0f);
            wheelVisual.transform.localRotation = Quaternion.Euler(0f, 180f, 0f) * steeringRotation * rollingRotation;
        }
        else
        {
            // Left side wheels
            Quaternion rollingRotation = Quaternion.Euler(wheelRotationAngle, 0f, 0f);
            wheelVisual.transform.localRotation = steeringRotation * rollingRotation;
        }
    }
    
    private void ApplySuspensionForces()
    {
        // Ray from the suspension attach point (this transform) downwards
        float maxRay = restLength + wheelRadius;

        bool hitGround = UnityEngine.Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, maxRay, groundMask);

        float currentLength = restLength; // default to fully extended if airborne

        if (hitGround)
        {
            // Update ground contact state
            isGrounded = true;
            ContactPoint = hit.point;

            // distance from attach point to contact minus wheel radius
            currentLength = Mathf.Clamp(hit.distance - wheelRadius, 0f, restLength);
            float compression = restLength - currentLength;

            // Suspension velocities (positive when compressing)
            float compressionVelocity = (lastLength - currentLength) / Time.fixedDeltaTime;

            // Spring: F = k * x    Damper: F = c * v
            float springForce = springStrength * compression;
            float damperForce = damperStrength * compressionVelocity;

            // Do not pull the car down if the strut extends past rest
            float totalForce = Mathf.Max(0f, springForce + damperForce);

            // Apply upwards along the strut axis at the attach point
            carBody.AddForceAtPosition(transform.up * totalForce, transform.position, ForceMode.Force);

            // Move visual to match strut length (negative local Y goes "down" along -up)
            if (wheelVisual)
            {
                var lp = wheelVisual.transform.localPosition;
                lp.y = -currentLength; // visual hub offset along strut
                wheelVisual.transform.localPosition = lp;
            }

            if (drawDebug)
            {
                Debug.DrawLine(transform.position, hit.point, Color.green);
                Debug.DrawRay(transform.position, transform.up * (totalForce / Mathf.Max(1f, carBody.mass)), Color.cyan);
            }
        }
        else
        {
            // Update ground contact state
            isGrounded = false;

            // Airborne: show wheel hanging at rest length
            if (wheelVisual)
            {
                var lp = wheelVisual.transform.localPosition;

                lp.y = -restLength;
                wheelVisual.transform.localPosition = lp;
            }
            if (drawDebug)
            {
                Debug.DrawRay(transform.position, -transform.up * maxRay, Color.red);
            }
        }
        lastLength = currentLength;
    }

    private bool IsFrontWheel()
    {
        return wheelType == WheelType.FrontLeft || wheelType == WheelType.FrontRight;
    }

    private Quaternion GetRotationForWheelType()
    {
        switch (wheelType)
        {
            case WheelType.FrontRight:
            case WheelType.BackRight:
                return Quaternion.Euler(0f, 180f, 0f);
            default:
                return Quaternion.identity;
        }
    }

    private enum WheelType { FrontLeft, FrontRight, BackLeft, BackRight }
}