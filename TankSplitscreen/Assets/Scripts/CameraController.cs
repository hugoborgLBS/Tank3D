using UnityEngine;

[ExecuteAlways]
public class CameraController : MonoBehaviour
{
    [Tooltip("Player 1 transform")]
    public Transform player1;
    [Tooltip("Player 2 transform (optional)")]
    public Transform player2;

    [Tooltip("Minimum orthographic half-size (zoom in)")]
    public float minZoom = 5f;
    [Tooltip("Maximum orthographic half-size (zoom out)")]
    public float maxZoom = 50f;

    [Tooltip("World-space buffer so players are not at the edge of the view")]
    public float edgeBuffer = 2f;

    [Tooltip("How quickly the camera moves to target position")]
    public float positionSmoothTime = 0.2f;
    [Tooltip("How quickly the camera zooms")]
    public float zoomSmoothTime = 0.25f;

    Camera cam;
    Vector3 positionVelocity;
    float zoomVelocity;

    void OnEnable()
    {
        cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;
    }

    // Allow runtime assignment of targets
    public void SetTargets(Transform p1, Transform p2)
    {
        player1 = p1;
        player2 = p2;
    }

    void LateUpdate()
    {
        if (cam == null) return;
        if (player1 == null && player2 == null) return;

        // Center between players (full 3D positions)
        Vector3 center = player2 == null ? player1.position : (player1.position + player2.position) * 0.5f;

        // Get camera basis
        Vector3 camRight = cam.transform.right;
        Vector3 camUp = cam.transform.up;

        float maxHalfWidth = 0f;
        float maxHalfHeight = 0f;

        // Consider both players
        void Consider(Transform t)
        {
            if (t == null) return;
            Vector3 offset = t.position - center;
            float x = Mathf.Abs(Vector3.Dot(offset, camRight));
            float y = Mathf.Abs(Vector3.Dot(offset, camUp));
            if (x > maxHalfWidth) maxHalfWidth = x;
            if (y > maxHalfHeight) maxHalfHeight = y;
        }

        Consider(player1);
        Consider(player2);

        maxHalfWidth += edgeBuffer;
        maxHalfHeight += edgeBuffer;

        if (cam.orthographic)
        {
            // Determine required orthographic half-size (vertical)
            float requiredHalfHeight = Mathf.Max(maxHalfHeight, maxHalfWidth / Mathf.Max(0.0001f, cam.aspect));
            float targetSize = Mathf.Clamp(requiredHalfHeight, minZoom, maxZoom);

            // Keep current distance from center along camera forward
            float currentDistance = Vector3.Dot((transform.position - center), -cam.transform.forward);
            if (!float.IsFinite(currentDistance) || currentDistance <= 0f) currentDistance = (minZoom + maxZoom) * 0.5f;

            Vector3 desiredPos = center - cam.transform.forward.normalized * currentDistance;

            // Smooth move and zoom
            transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref positionVelocity, positionSmoothTime);
            cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetSize, ref zoomVelocity, zoomSmoothTime);
        }
        else
        {
            // Perspective: compute distance so both players fit in view
            float fovRad = cam.fieldOfView * Mathf.Deg2Rad;
            float halfFov = fovRad * 0.5f;

            float distanceForHeight = maxHalfHeight / Mathf.Max(0.0001f, Mathf.Tan(halfFov));
            float distanceForWidth = maxHalfWidth / Mathf.Max(0.0001f, Mathf.Tan(halfFov) * cam.aspect);
            float requiredDistance = Mathf.Max(distanceForHeight, distanceForWidth);

            float minDistance = minZoom / Mathf.Max(0.0001f, Mathf.Tan(halfFov));
            float maxDistance = maxZoom / Mathf.Max(0.0001f, Mathf.Tan(halfFov));

            float clampedDistance = Mathf.Clamp(requiredDistance, minDistance, maxDistance);
            if (!float.IsFinite(clampedDistance) || clampedDistance <= 0f) clampedDistance = minDistance;

            Vector3 desiredPos = center - cam.transform.forward.normalized * clampedDistance;

            transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref positionVelocity, positionSmoothTime);
            // do not change rotation - camera rotation remains as set in editor
        }
    }
}