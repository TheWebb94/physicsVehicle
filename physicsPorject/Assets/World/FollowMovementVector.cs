using UnityEngine;

public class FollowMovementVector : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CharacterController playerRef;
    [SerializeField] private float leadDistance = 2f;
    [SerializeField] private float followSmooth = 10f;

    private Transform targetTransform;

    private Vector3 lastDir = Vector3.forward;

    void Awake()
    {
        // Only autoget if not assigned manually
        if (!playerRef)
            playerRef = GetComponent<CharacterController>();
    }

    void FixedUpdate()
    {
        DetermineTarget();

        Vector3 vel = Vector3.zero;

        if (rb != null)
            vel = rb.linearVelocity;

        // maintain last direction if almost stationary
        if (vel.sqrMagnitude > 0.01f)
            lastDir = vel.normalized;

        // target point in front of player/vehicle
        Vector3 aheadPos = targetTransform.position + lastDir * leadDistance;

        // SMOOTH movement instead of teleporting
        transform.position = Vector3.Lerp(transform.position, aheadPos, Time.deltaTime * followSmooth);

        // Smooth rotation
        if (lastDir != Vector3.zero)
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.LookRotation(lastDir, Vector3.up),
                Time.deltaTime * followSmooth
            );
    }

    private void DetermineTarget()
    {
        if (playerRef.isInCar)
        {
            targetTransform = playerRef.currentVehicle.transform;
            rb = playerRef.currentVehicle.GetComponent<Rigidbody>();
        }
        else
        {
            targetTransform = playerRef.transform;

            // character has no rigidbody ? use controller velocity if available
            rb = playerRef.GetComponent<Rigidbody>(); // this will be null
        }
    }
}
