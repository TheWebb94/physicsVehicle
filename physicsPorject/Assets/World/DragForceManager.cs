using System;
using UnityEngine;

public class DragForceManager : MonoBehaviour
{

    [SerializeField] private Rigidbody rb;

    [SerializeField] private Transform cubesParent;
    [SerializeField] private GameObject cubesObject;
    private GameObject[] cubesArray;
    private Vector3 cumulativeNormals;
    private Vector3 averageVectorNormal;
    public float hitCount;
    [SerializeField] private float rayLength = 3f;
    public float crossSectionalArea;
    [SerializeField] private bool drawDebugLines;

    // Angular drag properties

    public Vector3 angularDragTorque { get; private set; }
    [SerializeField] private float angularDragCoefficient = 0.3f;
    [SerializeField] private float nodeArea = 0.01f;
    
    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var generator = UnityEngine.Object.FindFirstObjectByType<cubeGenerator>();
        if (generator != null)
        {
            cubesArray = generator.cubesArray;
        }
    }

    private void Update()
    {

        foreach (var cube in cubesArray)
        {
            var cr = cube.GetComponent<SpawnRaycaster>();
            if (drawDebugLines)
            {
                cr.drawDebugLines = true;
            }
            else
            {
                cr.drawDebugLines = false;
            }

        }
    }
        // Update is called once per frame
        void FixedUpdate()
        {
            ResetValues();

            CountNodeHits();

            FindCumulativeNormal();

            CalculateAngularDragTorque();

            SetValues();

        }

        private void CalculateAngularDragTorque()
            {
                if (rb == null || cubesArray == null || cubesArray.Length == 0)
                {
                    angularDragTorque = Vector3.zero;
                    return;
                }

                Vector3 totalTorque = Vector3.zero;
                Vector3 angularVelocity = rb.angularVelocity;

                // If no rotation, no angular drag
                if (angularVelocity.sqrMagnitude < 0.001f)
                {
                    angularDragTorque = Vector3.zero;
                    return;
                }

                float airDensity = 1.225f; // kg/m³ at sea level

                foreach (var cube in cubesArray)
                {
                    var raycaster = cube.GetComponent<SpawnRaycaster>();
                    if (raycaster != null && raycaster.hitPlayer)
                    {
                        // Get the position of this raycast node
                        Vector3 nodePosition = cube.transform.position;

                        // Vector from center of mass to this node
                        Vector3 r = nodePosition - rb.worldCenterOfMass;
                        
                        // Velocity at this point due to rotation: v = ω × r
                        Vector3 rotationalVelocity = Vector3.Cross(angularVelocity, r);
                        
                        // Drag force at this node: F_drag = 0.5 × ρ × v² × C_d × A
                        float velocitySquared = rotationalVelocity.sqrMagnitude;
                        float dragMagnitude = 0.5f * airDensity * velocitySquared * angularDragCoefficient * nodeArea;
                        
                        // Drag opposes motion
                        Vector3 dragForce = -rotationalVelocity.normalized * dragMagnitude;

                        // Torque from this node: t = r × F
                        Vector3 torqueContribution = Vector3.Cross(r, dragForce);
                        totalTorque += torqueContribution;
                    }
                }
                
                angularDragTorque = totalTorque;
            }
        

        private void LateUpdate()
    {
                Debug.DrawRay(new Vector3(rb.position.x, rb.position.y + 2, rb.position.z), averageVectorNormal * rayLength, Color.red, 0.02f);

    }
    private void SetValues()
    {
        averageVectorNormal = cumulativeNormals / hitCount;
        crossSectionalArea = hitCount / 1000;
    }

    private void ResetValues()
    {
        hitCount = 0;
        averageVectorNormal = new Vector3(0, 0, 0);
    }

    private void FindCumulativeNormal()
    {

       foreach(var cube in cubesArray)
        {
            var raycaster = cube.GetComponent<SpawnRaycaster>();
            if (raycaster != null && raycaster.hitPlayer)
            {
                cumulativeNormals += raycaster.vectorNormal;
            }

            
        }
    }

    private void CountNodeHits()
    {
        foreach (var cube in cubesArray)
        {
            var raycaster = cube.GetComponent<SpawnRaycaster>();
            if (raycaster != null && raycaster.hitPlayer)
            {
                hitCount++;
            }
        }

        //Debug.Log($"Hits with nodes: {hitCount}");
        if (cubesArray == null || cubesArray.Length == 0)
        {
            return;
        }

    }

}
