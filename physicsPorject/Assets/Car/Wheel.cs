using UnityEngine;

public class Wheel : MonoBehaviour
{
    [Header("Model")]
    [SerializeField] private GameObject wheelModel;
    [SerializeField] WheelType wheelType;

    [Header("Suspension")]
    [SerializeField] private float restLength = 0.35f;        // meters
    [SerializeField] private float springStrength = 35000f;   // N/m
    [SerializeField] private float damperStrength = 4500f;    // N�s/m
    [SerializeField] private float wheelRadius = 0.34f;       // meters
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private bool drawDebug;

    [Header("Tire Grip")]
    [SerializeField] private float lateralGripCoefficient = 12000f;     // Lateral grip strength (N per m/s of slip)
    [SerializeField] private float longitudinalGripCoefficient = 10000f; // Forward/back grip strength
    [SerializeField] private float rollingResistance = 100f;             // Resistance when rolling
    [SerializeField] private bool applyTireForces = true;                // Toggle tire forces on/off

    private Rigidbody carBody;
    private GameObject wheelVisual;
    private float lastLength;
    private VehicleController vehicleController;

    private void Awake()
    {
        // Find the car rigidbody on a parent
        carBody = GetComponentInParent<Rigidbody>();
        // Find the vehicle controller on a parent
        vehicleController = GetComponentInParent<VehicleController>();
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
    }

    private void ApplySuspensionForces()
    {
        // Ray from the suspension attach point (this transform) downwards
        float maxRay = restLength + wheelRadius;
        bool hitGround = UnityEngine.Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, maxRay, groundMask);

        float currentLength = restLength; // default to fully extended if airborne

        if (hitGround)
        {
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

            // Apply tire grip forces if enabled
            if (applyTireForces)
            {
                ApplyTireForces(hit.point, totalForce);
            }

            // Move visual to match strut length (negative local Y goes �down� along -up)
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

    private void ApplyTireForces(Vector3 contactPoint, float normalForce)
    {
        if (carBody == null) return;

        // Calculate the velocity of the tire at the contact point
        Vector3 tireWorldVelocity = carBody.GetPointVelocity(contactPoint);

        // Project velocity onto wheel's forward and right axes
        float forwardSpeed = Vector3.Dot(tireWorldVelocity, transform.forward);
        float lateralSpeed = Vector3.Dot(tireWorldVelocity, transform.right);

        // Calculate load factor (how much weight is on this wheel)
        // More load = more grip, but with diminishing returns (Pacejka tire model simplified)
        float expectedLoadPerWheel = (carBody.mass * Mathf.Abs(Physics.gravity.y)) / 4f;
        float loadFactor = Mathf.Sqrt(Mathf.Clamp(normalForce / expectedLoadPerWheel, 0f, 2f));

        // === LATERAL FORCES (side-to-side grip for turning) ===
        // Resistance to sliding sideways - this is what makes the car turn
        Vector3 lateralForce = -transform.right * lateralSpeed * lateralGripCoefficient * loadFactor;

        // === LONGITUDINAL FORCES (forward/backward grip) ===
        Vector3 longitudinalForce = Vector3.zero;

        if (vehicleController != null)
        {
            // Get throttle and steering input from vehicle controller
            float throttle = vehicleController.throttle;
            float steering = vehicleController.steering;

            // Drive force (when accelerating)
            if (throttle > 0.01f)
            {
                // Apply drive force in the forward direction
                // Reduced by current forward speed to simulate wheel slip at high speeds
                float driveForce = throttle * longitudinalGripCoefficient * loadFactor;
                longitudinalForce += transform.forward * driveForce;
            }

            // Braking force (when throttle is negative from braking)
            if (throttle < -0.01f)
            {
                // Apply brake force opposite to current velocity
                float brakeForce = Mathf.Abs(throttle) * longitudinalGripCoefficient * loadFactor;
                longitudinalForce -= transform.forward * forwardSpeed * brakeForce * 0.1f;
            }
        }

        // Rolling resistance (always opposes motion)
        Vector3 rollingResistanceForce = -tireWorldVelocity.normalized * rollingResistance * loadFactor;

        // Combine all tire forces
        Vector3 totalTireForce = lateralForce + longitudinalForce + rollingResistanceForce;

        // Apply the combined force at the contact point
        carBody.AddForceAtPosition(totalTireForce, contactPoint, ForceMode.Force);

        // Debug visualization
        if (drawDebug)
        {
            Debug.DrawRay(contactPoint, lateralForce / 1000f, Color.blue);      // Blue = lateral grip
            Debug.DrawRay(contactPoint, longitudinalForce / 1000f, Color.yellow); // Yellow = drive/brake
        }
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
