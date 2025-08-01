using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;      // Gán nhân vật vào đây trong Inspector
    public Vector3 offset = new Vector3(0, 1, -10);
    public float smoothSpeed = 0.125f;

    public Vector2 minBounds;     // Giới hạn trái-dưới
    public Vector2 maxBounds;     // Giới hạn phải-trên

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Áp dụng giới hạn
        float clampedX = Mathf.Clamp(smoothedPosition.x, minBounds.x, maxBounds.x);
        float clampedY = Mathf.Clamp(smoothedPosition.y, minBounds.y, maxBounds.y);

        transform.position = new Vector3(clampedX, clampedY, smoothedPosition.z);
    }
}