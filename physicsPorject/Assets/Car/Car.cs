using System;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

namespace PhysicsCar
{

    public class VehicleController : MonoBehaviour
    {
        [Range(0f,1f)] public float throttle;
        public float throttleAccellaration = 0.1f;
        [Range(-1f, 1f)] public float steering;
        public float brakeFactor = 2f;
        public bool handbrake;

      
        public void UseVehicleController()
        {
            //decay throttle if not accellerating
            if (throttle > 0f)
            {
                throttle -= throttleAccellaration / 2f * Time.deltaTime;
            }

            //accellerate by increasing throttle
            if (Input.GetKey(KeyCode.W))
            {
                throttle += throttleAccellaration * Time.deltaTime;
                if (throttle > 1f)
                {
                    throttle = 1f;
                }
            }

            // deccellerate by decreasing throttle
            if (Input.GetKey(KeyCode.S))
            {
                throttle -= throttleAccellaration * brakeFactor * Time.deltaTime;
            }


            if (Input.GetKey(KeyCode.A))
            {

            }

            if (Input.GetKey(KeyCode.D))
            {

            }

            if (Input.GetKey(KeyCode.Space))
            {

            }
        }
    }  
}

