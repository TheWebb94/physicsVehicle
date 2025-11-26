using System;
using UnityEngine;


public class Physics : MonoBehaviour
{

    public float gravity = -9.81f;
    public Rigidbody rb;
    [SerializeField] private DragForceManager playerDragForce;

    public float A;         // A (cross-sectional area) = needs claculation based on player model
    public float dragCoefficient;        // drag coefficient
    private float airDensity;
    [SerializeField] private bool useComplexDrag;
    [SerializeField] public bool isPlayer;
    [SerializeField] CharacterController playerRef;

    public float friction = 5f;

    void Awake()
    {
        airDensity = 1.225f;        // p (air density) = 1.225 kg/m^3 at sea level 
                                    // maybe add a change in air density based on players y value (if impplementing a flying vehicle)
        dragCoefficient = 0.3f; /* Cd (drag coefficient)    = 1.0 (approximate for a human)
                                                            = 0.47 (approximate for a sphere)
                                                            = 1.05 (approximate for a cube)
                                                            = 0.28 - 0.35 (approximate for a car) */
    }

    void FixedUpdate()
    {
        ApplyGravity();
        ApplyDrag();
        ApplyAngularDrag();
        ApplyFriction();
    }

    private void ApplyAngularDrag()
    {
        if (playerDragForce != null)
        {
            Vector3 angularDragTorque = playerDragForce.angularDragTorque;
            rb.AddTorque(angularDragTorque, ForceMode.Force);
        }
    }

    private void ApplyFriction()
    {
        if(isPlayer)
        {
            if (playerRef.isGrounded)
            {
                Vector3 frictionForce = new Vector3 (-rb.linearVelocity.normalized.x, 0 , -rb.linearVelocity.normalized.z) * friction;
                rb.AddForce(frictionForce, ForceMode.Acceleration);
            }

        }
        else
        {
            // Apply friction for non-player objects if needed
            //Vector3 frictionForce = -rb.linearVelocity.normalized * friction;
            Vector3 frictionForce = new Vector3(-rb.linearVelocity.x, 0, -rb.linearVelocity.z) * friction;
            rb.AddForce(frictionForce, ForceMode.Acceleration);

        }
    }

    private void ApplyGravity()
    {
        //Gravity
        rb.AddForce(new Vector3(0, gravity, 0), ForceMode.Acceleration);
    }

    private void ApplyDrag()
    {
        float dragMagnitude;

        if (useComplexDrag)
        {
            //drag equation Fd = 0.5 * p * v^2 * Cd * A
            dragMagnitude = 0.5f * airDensity * VelocitySquared() * dragCoefficient * (float)GetCrossSectionalArea();
        }
        else
        {
            dragMagnitude = 0.1f; //simple constant drag
        }

        rb.AddForce(-rb.linearVelocity * dragMagnitude);
    }

    private float VelocitySquared()
    {
        return Mathf.Pow(rb.linearVelocity.magnitude, 2);
    }

    private float GetCrossSectionalArea()
    {
        var playerRef = UnityEngine.Object.FindFirstObjectByType<DragForceManager>();
        playerDragForce = playerRef;
        //Debug.Log(playerDragForce.crossSectionalArea);

        return  playerDragForce.crossSectionalArea;
    }
}
