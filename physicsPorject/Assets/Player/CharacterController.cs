using System;
using System.Linq.Expressions;
using UnityEngine;
using PhysicsCar;

public class CharacterController : MonoBehaviour
{

    float acceleration = 1000;
    float maxSpeed = 4000;
    private Rigidbody rb;
    public bool isGrounded;
    [SerializeField] private float jumpForce = 20;

    float verticalMouseSpeed = 2.0f;
    float horizontalMouseSpeed = 2.0f;
    [SerializeField] bool isInCar;
    [SerializeField] VehicleController vc;
    Animator pa;

    public float hoverHeight = 1.2f;     // how high above ground to float
    public float hoverStrength = 75f;    // how strong the lift is
    public float gravity = -9.81f;       // fallback gravity
    public LayerMask groundMask = 7;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        isGrounded = false;
        isInCar = false;
        pa = GetComponent<Animator>();
        vc = GetComponent<VehicleController>();
    }

    // Update is called once per frame
    void Update()
    {
        DetermineController();
        AnimationHandler();
        Debug.Log(isGrounded);
    }

    private void FixedUpdate()
    {
        HoverOffGround();
        ResetSpeedIfSlow();
    }

    private void ResetSpeedIfSlow()
    {
        if( rb.linearVelocity.magnitude < 0.01f && !isInCar)
        {
            rb.linearVelocity = Vector3.zero;
        }
    }

    private void HoverOffGround()
    {
        // Hover Mechanic
        RaycastHit hit;
        if (UnityEngine.Physics.Raycast(transform.position, -Vector3.up, out hit, hoverHeight, groundMask))
        {
            float hoverTolerance = hoverHeight - hit.distance;
            float upwardSpeed = rb.linearVelocity.y;
            float lift = hoverTolerance * hoverStrength - upwardSpeed * 5f;
            rb.AddForce(Vector3.up * lift, ForceMode.Acceleration);
            isGrounded = true;

        }
        else
        {
            rb.AddForce(Vector3.up * gravity, ForceMode.Acceleration);
        }
    }

    private void AnimationHandler()
    {
        if (!isInCar)
        {
            switch (rb.linearVelocity.magnitude)
            {
                case < 0.1f:
                    pa.SetBool("isWalking", false);
                    pa.SetBool("isRunning", false);
                    break;
                case >= 0.01f and < 10f:
                    pa.SetBool("isWalking", true);
                    pa.SetBool("isRunning", false);
                    break;
                case >= 20f:
                    pa.SetBool("isRunning", true);
                    pa.SetBool("isWalking", false);
                    break;
            }
        }
    }

    private void DetermineController()
    {
        if (!isInCar)
        {
            UsePlayerController();
            pa.SetBool("isDriving", false);
        }
        else
        {
            // Use Vehicle Controller
            vc.UseVehicleController();
            pa.SetBool("isDriving", true);
        }
        Debug.Log(vc.throttle);
    }

    private void UsePlayerController()
    {
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(transform.forward * acceleration * Time.deltaTime, ForceMode.Acceleration);
            
            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }

        if (Input.GetKey(KeyCode.S))
        {
            rb.AddForce(-transform.forward * acceleration * Time.deltaTime, ForceMode.Acceleration);

            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }

        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(-transform.right * acceleration * Time.deltaTime, ForceMode.Acceleration);

            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }

        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(transform.right * acceleration * Time.deltaTime, ForceMode.Acceleration);

            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }

        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
            isGrounded = false;
        }

        float h = horizontalMouseSpeed * Input.GetAxis("Mouse X");

        transform.Rotate(0, h, 0);
    }
    
    enum characterController
    {
        Player,
        LandRover,
        RaceCar,
    }
}
