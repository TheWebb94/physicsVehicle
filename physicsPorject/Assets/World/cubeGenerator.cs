using UnityEngine;

public class cubeGenerator : MonoBehaviour
{

    public Transform cubesParent;

    [SerializeField] private float cubeSize = 0.05f;
    [SerializeField] private float gridWidth = 1.5f;
    private int halfCubeCount => Mathf.RoundToInt(gridWidth * 2 / cubeSize);


    void Start()
    {
        float cubeSpacing = cubeSize;

        //starting at -halfCubeCount to halfCubeCount to center the grid 
        for (int x = -halfCubeCount; x < halfCubeCount; x++)
        {
            for (int y = -halfCubeCount; y < halfCubeCount; y++)
            {
                Vector3 position = new Vector3(x * cubeSpacing, y * cubeSpacing, transform.localScale.z);
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = position;
                cube.transform.parent = cubesParent;
                cube.name = $"Cube_{x}_{y}";
                cube.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);

                SpawnRaycaster(position, Vector3.forward);

            }
        }
    }


    void SpawnRaycaster(Vector3 location, Vector3 dir)
    {
        UnityEngine.Physics.Raycast(location, dir, out RaycastHit hit);
        Debug.DrawRay(location, dir * 10, Color.red, 0.01f);


    }
}