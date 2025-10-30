using System;
using System.Linq.Expressions;
using UnityEngine;

public class CharacterController : MonoBehaviour
{

    float acceleration = 1000;
    float maxSpeed = 4000;
    private Rigidbody rb;
    private bool isGrounded;
    [SerializeField] private float jumpForce = 5;

    float verticalMouseSpeed = 2.0f;
    float horizontalMouseSpeed = 2.0f;
    int currentController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        isGrounded = false;
        currentController = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //controllers need class with childs inheriting fromm it

        //make a new parent class called controllers
                //ceach inheriting class has its own implementation of the controls
        switch (currentController)
        {
            case (int)characterController.Player:
                UsePlayerController();
                return;
            case (int)characterController.LandRover:
                return;
            case (int)characterController.RaceCar:
                return;
            default:
                UsePlayerController();
                return;

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
