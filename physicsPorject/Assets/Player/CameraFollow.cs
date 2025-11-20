using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject target;
    public float positionSmoothSpeed = 10f;
    public float rotationSmoothSpeed = 5f;
    public Vector3 offset = new Vector3(0f, 3f, -6f);
    public GameObject playerRef;

    private bool playerIsInCar;
    private Vector3 currentVelocity;
    private Vector3 playerOffset = new Vector3(0f, 3f, -6f);
    private Vector3 vehicleOffset = new Vector3(0f, 7f, -20f);

    private void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
        target = playerRef;
        offset = playerOffset;
    }

    private void Update()
    {
        // Update target and offset only when player state changes
        bool wasInCar = playerIsInCar;
        playerIsInCar = playerRef.GetComponent<CharacterController>().isInCar;

        if (wasInCar != playerIsInCar)
        {
            if (!playerIsInCar)
            {
                offset = playerOffset;
                target = playerRef;
            }
            else
            {
                target = playerRef.GetComponent<CharacterController>().currentVehicle;
                offset = vehicleOffset;
            }
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Calculate desired position relative to target's rotation
        Vector3 desiredPosition = target.transform.position + target.transform.rotation * offset;

        // Smooth position follow using SmoothDamp for better damping
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, 1f / positionSmoothSpeed);

        // Calculate desired rotation (look at target from camera position)
        Vector3 directionToTarget = target.transform.position - transform.position;
        Quaternion desiredRotation = Quaternion.LookRotation(directionToTarget);

        // Smooth rotation follow to stay behind the player
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSmoothSpeed * Time.deltaTime);
    }
} 