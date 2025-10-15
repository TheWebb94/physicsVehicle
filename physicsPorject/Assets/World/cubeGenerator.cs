using UnityEngine;

public class cubeGenerator : MonoBehaviour
{

    public Transform cubesParent;

    [SerializeField] private float cubeSize = 0.05f;
    [SerializeField] private float gridWidth = 1.5f;
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private float yOffset = 0.5f;
    public GameObject[] cubesArray;

    void Start()
    {
        int cubeCountPerAxis = Mathf.RoundToInt(gridWidth * 2 / cubeSize);
        int totalCubes = cubeCountPerAxis * cubeCountPerAxis;

        float cubeSpacing = cubeSize;

        cubesArray = new GameObject[totalCubes];
        int index = 0;

        //starting at -halfCubeCount to halfCubeCount to center the grid 
        for (int x = -cubeCountPerAxis / 2; x < cubeCountPerAxis / 2; x++)
        {
            for (int y = -cubeCountPerAxis / 2; y < cubeCountPerAxis / 2; y++)
            {
                Vector3 position = new Vector3(x * cubeSpacing, y * cubeSpacing - yOffset, transform.localScale.z);
                var cube = Instantiate(cubePrefab, position, Quaternion.identity, cubesParent);
                var raycaster = cube.AddComponent<SpawnRaycaster>();
                cube.transform.position = position;
                cube.transform.parent = cubesParent;
                cube.name = $"Cube_{x}_{y}";
                cube.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);

                // Add the cube to the array
                cubesArray[index] = cube;
                index++;
            }
        }
    }
}

