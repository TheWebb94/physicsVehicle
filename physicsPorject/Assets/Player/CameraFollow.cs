using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0f, 3f, -6f);

    void LateUpdate()
    {
        if (target == null) return;

        // Offset relative to the target's rotation
        Vector3 desiredPosition = target.TransformPoint(offset);

        // Smooth follow
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Look at the target
        transform.LookAt(target);
    }
}