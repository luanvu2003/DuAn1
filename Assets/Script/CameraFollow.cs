using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 1, -10);
    public float smoothSpeed = 0.125f;

    public Collider2D mapBoundsCollider; // Gán GameObject có PolygonCollider2D hoặc BoxCollider2D

    private Vector2 minBounds;
    private Vector2 maxBounds;

    private float camHalfHeight;
    private float camHalfWidth;

    void Start()
    {
        if (mapBoundsCollider == null)
        {
            Debug.LogError("Map Bounds Collider chưa được gán!");
            return;
        }

        Bounds bounds = mapBoundsCollider.bounds;
        minBounds = bounds.min;
        maxBounds = bounds.max;

        Camera cam = Camera.main;
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = cam.aspect * camHalfHeight;
    }

    void LateUpdate()
    {
        if (target == null || mapBoundsCollider == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        float clampedX = Mathf.Clamp(smoothedPosition.x, minBounds.x + camHalfWidth, maxBounds.x - camHalfWidth);
        float clampedY = Mathf.Clamp(smoothedPosition.y, minBounds.y + camHalfHeight, maxBounds.y - camHalfHeight);

        transform.position = new Vector3(clampedX, clampedY, smoothedPosition.z);
    }
}
