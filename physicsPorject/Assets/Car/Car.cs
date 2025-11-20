using System;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

    public class Car : MonoBehaviour
    {
        [Range(0f, 1f)] public float throttle;
        public float throttleAccellaration = 0.1f;
        [Range(-1f, 1f)] public float steering;
        public float brakeFactor = 2f;
        public bool handbrake;

        [SerializeField] private GameObject player;  
        private VehicleController vc;
    private Rigidbody rb;
        //private Engine engine;
        //private Suspension suspension;
        private Wheel[] wheels;
        public bool isPlayerInCar;


    private void Start()
    {
            rb = GetComponent<Rigidbody>();
            rb.mass = 1500f; 
            //engine = GetComponent<Engine>();
            

            vc = GetComponent<VehicleController>();
            wheels = GetComponentsInChildren<Wheel>();
            Debug.Log(wheels.Length + "wheels fond");       
    }
    private void FixedUpdate()
    {       //testin sspension

        if (isPlayerInCar)
        {
            // Force application is now handled by VehicleController at wheel positions
            throttle = vc.throttle;
            steering = vc.steering;
            brakeFactor = vc.brakeFactor;
            //ApplyThrottle();
            //ApplySteering();
            //ApplyBrakes();
        }
    }

    /*private void ApplyThrottle()
    {
            float targetThrottle = throttle;
            float currentThrottle = engine.currentThrottle;
            if (targetThrottle > currentThrottle)
            {
                currentThrottle += throttleAccellaration * Time.fixedDeltaTime;
                currentThrottle = Mathf.Min(currentThrottle, targetThrottle);
            }
            else if (targetThrottle < currentThrottle)
            {
                currentThrottle -= throttleAccellaration * Time.fixedDeltaTime;
                currentThrottle = Mathf.Max(currentThrottle, targetThrottle);
            }
            engine.currentThrottle = currentThrottle;
            .ApplyEngineForce();
    } */
}