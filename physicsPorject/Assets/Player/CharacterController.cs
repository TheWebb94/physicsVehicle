using System;
using System.Linq.Expressions;
using UnityEngine;
using PhysicsCar;

public class CharacterController : MonoBehaviour
{

    float acceleration = 1000;
    float maxSpeed = 4000;
    private Rigidbody rb;
    private bool isGrounded;
    [SerializeField] private float jumpForce = 5;

    float verticalMouseSpeed = 2.0f;
    float horizontalMouseSpeed = 2.0f;
    [SerializeField] bool isInCar;
    VehicleController vc;
    Animator pa;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        isGrounded = false;
        isInCar = false;
        pa = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        DetermineController();
        AnimationHandler();
    }

    private void AnimationHandler()
    {
        if (!isInCar)
        {
            switch (rb.linearVelocity.magnitude)
            {
                case < 0.01f:
                    pa.SetBool("isWalking", false);
                    pa.SetBool("isRunning", false);
                    break;
                case >= 0.01f and < 10f:
                    pa.SetBool("isWalking", true);
                    pa.SetBool("isRunning", false);
                    break;
                case >= 10f:
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


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor" && isGrounded == false)
        {
            Debug.Log("grounded");
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            isGrounded = false;
            Debug.Log("notgrounded)");
        }
    }

    
    enum characterController
    {
        Player,
        LandRover,
        RaceCar,
    }
}
