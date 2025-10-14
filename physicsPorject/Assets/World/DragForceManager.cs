using System;
using UnityEngine;

public class DragForceManager : MonoBehaviour
{

    [SerializeField] private Rigidbody rb;

    [SerializeField] private Transform cubesParent;
    [SerializeField] private GameObject cubesObject;
    private GameObject[] cubesArray;


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
            Debug.Log($"Linked {cubesArray.Length} cubes from generator.");
        }
        else
        {
            Debug.LogWarning("No cubeGenerator found!");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CountNodeHits();
    }

    private void CountNodeHits()
    {
        int hitCount = 0;
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
            Debug.LogWarning("Cube array is empty in DragForceManager!");
            return;
        }

    }

}
