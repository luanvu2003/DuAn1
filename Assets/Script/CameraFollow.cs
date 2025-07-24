using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;      // Gán nhân vật vào đây trong Inspector
    public Vector3 offset = new Vector3(0, 1, -10);
    public float smoothSpeed = 0.125f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);
    }
}
