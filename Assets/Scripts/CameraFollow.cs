using UnityEngine;

/// <summary>
/// Keeps the camera centered on the player, with optional smooth follow.
/// Attach this to the Main Camera.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target; // Drag Player here in Inspector

    [Header("Follow Settings")]
    public float smoothSpeed = 8f;
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Camera Bounds (optional)")]
    public bool useBounds = false;
    public float minX = -10f, maxX = 10f;
    public float minY = -6f, maxY = 6f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPos = target.position + offset;

        if (useBounds)
        {
            desiredPos.x = Mathf.Clamp(desiredPos.x, minX, maxX);
            desiredPos.y = Mathf.Clamp(desiredPos.y, minY, maxY);
        }

        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);
    }
}
