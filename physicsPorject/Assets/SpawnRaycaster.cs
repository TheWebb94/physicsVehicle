using UnityEngine;
using static UnityEngine.UI.Image;

public class SpawnRaycaster : MonoBehaviour
{
    [SerializeField] private float rayLength = 3f;
    public bool hitPlayer = false;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private float originOffset = 0.05f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMask = LayerMask.GetMask("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 dir = -transform.forward;
        Vector3 origin = transform.position + dir * originOffset;

        if (UnityEngine.Physics.Raycast(origin, dir, out RaycastHit hit, rayLength, playerMask, QueryTriggerInteraction.Collide))
        {
            hitPlayer = hit.transform.CompareTag("Player");
            Debug.DrawRay(origin, dir * hit.distance, Color.green);

            hitPlayer = true;
        }
        else
        {
            hitPlayer = false;
            Debug.DrawRay(origin, dir * rayLength, Color.red);
        }

    }
}
