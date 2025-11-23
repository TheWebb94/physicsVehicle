using UnityEngine;
using static UnityEngine.UI.Image;

public class SpawnRaycaster : MonoBehaviour
{
    [SerializeField] private float rayLength = 5f;
    public bool hitPlayer = false;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private string targetTag = "Player"; // Configurable target tag
    [SerializeField] private float originOffset = 0.05f;
    public Vector3 vectorNormal;
    [SerializeField] public bool drawDebugLines;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Default to Player mask if not set
        if (targetMask == 0)
        {
            targetMask = LayerMask.GetMask("Player");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 dir = -transform.forward;
        Vector3 origin = transform.position + dir * originOffset;

        if (UnityEngine.Physics.Raycast(origin, dir, out RaycastHit hit, rayLength, targetMask, QueryTriggerInteraction.Collide))
        {
            hitPlayer = hit.transform.CompareTag(targetTag);
            vectorNormal = hit.normal;
            if (drawDebugLines)
            {
                Debug.DrawRay(origin, dir * hit.distance, Color.green);
            }

        }
        else
        {
            hitPlayer = false;
            if (drawDebugLines)
            {
                Debug.DrawRay(origin, dir * rayLength, Color.red);
            }
        }

    }
}
