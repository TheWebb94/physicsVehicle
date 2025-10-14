using Unity.VisualScripting;
using UnityEngine;

public class FollowMovementVector : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float leadDistance = 2f;

    // Keep a fallback direction for when the player is nearly stationary
    private Vector3 lastDir = Vector3.forward;

    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 vel;
            
            if(rb.linearVelocity != null)
        {
            vel = rb.linearVelocity;
        }
        else
        {
            vel = Vector3.zero;
        }


        // If velocity is small keep the previous direction
        if (vel.sqrMagnitude > 0.01f)
            lastDir = vel.normalized;

        // Stay ahead of the player along the movement vector
        Vector3 aheadPos = rb.position + lastDir * leadDistance;
        transform.position = aheadPos;

        Vector3 lookDir = lastDir;
        transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);
    }
}
