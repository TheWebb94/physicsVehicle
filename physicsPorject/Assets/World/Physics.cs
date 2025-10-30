using UnityEngine;


public class Physics : MonoBehaviour
{

    public float gravity = -9.81f;
    public Rigidbody rb;
    [SerializeField] private DragForceManager playerDragForce;

    public float A;         // A (cross-sectional area) = needs claculation based on player model
    public float dragCoefficient;        // drag coefficient
    private float airDensity;



   

    

    void Awake()
    {
        airDensity = 1.225f;        // p (air density) = 1.225 kg/m^3 at sea level 
        dragCoefficient = 0.3f; /* Cd (drag coefficient)    = 1.0 (approximate for a human)
                                                            = 0.47 (approximate for a sphere)
                                                            = 1.05 (approximate for a cube)
                                                            = 0.28 - 0.35 (approximate for a car) */

    }

    void FixedUpdate()
    {
        //Gravity
        rb.AddForce(new Vector3(0, gravity, 0), ForceMode.Acceleration);

        //drag equation Fd = 0.5 * p * v^2 * Cd * A
        float dragMagnitude = 0.5f * airDensity * VelocitySquared() * dragCoefficient * (float)GetCrossSectionalArea();
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
        Debug.Log(playerDragForce.crossSectionalArea);

        return  playerDragForce.crossSectionalArea;
    }
}
