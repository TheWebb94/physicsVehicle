using System;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

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
    private GameObject currentVehicle;
    private GameObject? nearbyCar;
    Animator pa;

    private float yInput;
    private float yRotation;
    public float jumpHeightTolerance = 1f;     // how high above ground to float
    public LayerMask groundMask = 7;

    [SerializeField] private GameObject mesh;

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

    private void FixedUpdate()
    {
        ResetSpeedIfSlow();
        RotatePlayerWithMouseMovement();

    }

    private void RotatePlayerWithMouseMovement()
    {
        Vector3 targetRotation = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        if (targetRotation.magnitude > 0.1f)
        {
            mesh.transform.rotation = Quaternion.LookRotation(targetRotation == Vector3.zero ? transform.forward : targetRotation);
        }
        yInput = horizontalMouseSpeed * Input.GetAxis("Mouse X");

        yRotation += yInput;
        Quaternion quaternion = Quaternion.Euler(0, yRotation, 0);
        rb.MoveRotation(quaternion);
    }

    private void ResetSpeedIfSlow()
    {
        if (rb.linearVelocity.magnitude < 0.01f && !isInCar)
        {
            rb.linearVelocity = Vector3.zero;
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

            pa.SetFloat("RunSpeed", rb.linearVelocity.magnitude);
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
            currentVehicle.GetComponent<VehicleController>().UseVehicleController(currentVehicle);
            pa.SetBool("isDriving", true);
        }
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

        if(Input.GetKeyDown(KeyCode.E))
        {
            if(nearbyCar != null  && !isInCar)
            {
                isInCar = true;
                currentVehicle = nearbyCar;
                Debug.Log("Entered Car " + currentVehicle);
            }
           

        }
    }

    //check overlap with spherecollider for car entry
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Car")
        {
            nearbyCar = collision.gameObject;
   
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Car")
        {
            nearbyCar = null;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Floor") 
        {
            isGrounded = true;
        }
    }
}
