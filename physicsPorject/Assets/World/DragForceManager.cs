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
    private int hitCount;

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

    // Update is called once per frame
    void FixedUpdate()
    {
        CountNodeHits();
        FindCumulativeNormal();
        averageVectorNormal = cumulativeNormals / hitCount;
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

        Debug.Log($"Hits with nodes: {hitCount}");
        if (cubesArray == null || cubesArray.Length == 0)
        {
            return;
        }

    }

}
