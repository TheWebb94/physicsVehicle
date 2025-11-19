using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject target;
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0f, 3f, -6f);
    public GameObject playerRef;
    private bool playerIsInCar;


    private void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
        
    }

    private void Update()
    {
        playerIsInCar = playerRef.GetComponent<CharacterController>().isInCar;
    }

    void LateUpdate()
    {
        if (target == null) return;
        
        
        
        if (!playerIsInCar)
        { 
            offset = new Vector3(0f, 3f, -6f);
            target = playerRef;
        }
        else
        {
            target = playerRef.GetComponent<CharacterController>().currentVehicle;
            offset = new Vector3(0f, 7f, -20f);
        }
        
        
        // Offset relative to the target's rotation
        Vector3 desiredPosition = target.transform.TransformPoint(offset);

        // Smooth follow
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        
        // Look at the target
        
        transform.LookAt(target.transform);


    }
} 