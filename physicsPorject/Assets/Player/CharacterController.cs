using System;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterController : MonoBehaviour
{

    float acceleration = 1000;
    float maxSpeed = 4000;
    public Rigidbody rb;
    public bool isGrounded;
    [SerializeField] private float jumpForce = 20;

    float verticalMouseSpeed = 2.0f;
    float horizontalMouseSpeed = 2.0f;
    public bool isInCar;
    public GameObject currentVehicle;
    private GameObject? nearbyCar;
    Animator pa;

    private float yInput;
    private float yRotation;
    public float jumpHeightTolerance = 1f;     // how high above ground to float
    public LayerMask groundMask = 7;

    public GameObject mesh;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        isGrounded = false;
        isInCar = false;
        pa = GetComponent<Animator>();
        Camera.main.GetComponent<CameraFollow>().target = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        AnimationHandler();

        SetPlayerController();
        
    }
    
    private void FixedUpdate()
    {
        ResetSpeedIfSlow();
        RotatePlayerWithMouseMovement();
        SetVehicleController();
    }
    
    private void SetPlayerController()
    {
        if (!isInCar)
        {
            UsePlayerController();
            pa.SetBool("isDriving", false);
        }
    }

    private void SetVehicleController()
    {
        if (isInCar)
        {
            currentVehicle.GetComponent<VehicleController>().UseVehicleController(currentVehicle);
            pa.SetBool("isDriving", true);
        }
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

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (nearbyCar != null && !isInCar)
            {
                Debug.Log("E key pressed");  
                EnterCar();
            }
        }
    }
    
    private void EnterCar()
    {
        isInCar = true;
        currentVehicle = nearbyCar;

        Camera.main.GetComponent<CameraFollow>().target = currentVehicle.gameObject;

        
        // Disable player movement physics
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;

        // Hide player model if needed
        mesh.SetActive(false);

        
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            nearbyCar = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            if (nearbyCar == other.gameObject)
                nearbyCar = null;
        }
    }

    //check for nearbyu car or floor 
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Floor") 
        {
            isGrounded = true;
        }
    }
}
