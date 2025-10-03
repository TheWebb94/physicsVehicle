using UnityEngine;

public class Physics : MonoBehaviour
{

    public float gravity = -9.81f;
    public Rigidbody rb;

    public float A;         // A (cross-sectional area) = needs claculation based on player model
    public float Cd;        // drag coefficient



    /* Cd (drag coefficient) = 1.0 (approximate for a human)
                             = 0.47 (approximate for a sphere)
                              = 1.05 (approximate for a cube)
                             = 0.28 - 0.35 (approximate for a car)

    */

    void FixedUpdate()
    {
        //Gravity
        rb.AddForce(new Vector3(0, gravity, 0), ForceMode.Acceleration);

        //wind resistance
        float dragMagnitude = 0.5f * 1.225f * Mathf.Pow(rb.linearVelocity.magnitude, 2) * Cd * A;
        rb.AddForce(-rb.linearVelocity.normalized * dragMagnitude, ForceMode.Force);

        // p (air density) = 1.225 kg/m^3 at sea level  
        // v (velocity) = rb.linearVelocity.magnitude

        // Cd (drag coefficient) = 1.0 (approximate for a human)
        // Fd = 0.5 * p * v^2 * Cd * A

        //raycast object from infront the vehicle in the direction of the movement vector, aiming towards the vehicle. 
    }
}
