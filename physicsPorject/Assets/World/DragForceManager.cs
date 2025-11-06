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
            SetValues();

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
