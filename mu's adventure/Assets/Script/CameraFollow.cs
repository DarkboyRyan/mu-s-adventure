using UnityEngine;

/// <summary>
/// Camera follow script. Attach to Main Camera and assign the target to follow.
/// Supports 2D (XY only) and 3D (XYZ), with adjustable smoothness and offset.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("Object to follow (e.g. player, canvas center). Leave empty to disable movement.")]
    public Transform target;

    [Header("Follow Mode")]
    [Tooltip("If checked, follow on XY plane only (2D). If unchecked, follow XYZ (3D).")]
    public bool follow2D = true;

    [Header("Offset")]
    [Tooltip("Camera position offset relative to target.")]
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Smoothing")]
    [Tooltip("Follow smoothness. Higher = snappier, lower = smoother.")]
    [Range(1f, 30f)]
    public float smoothSpeed = 10f;

    [Header("Bounds (Optional)")]
    [Tooltip("Clamp camera movement within a rectangle.")]
    public bool useBounds = false;
    public float minX = -100f, maxX = 100f;
    public float minY = -100f, maxY = 100f;

    private Vector3 _velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 targetPosition = target.position + offset;

        if (follow2D)
        {
            targetPosition.z = transform.position.z; // Keep camera Z
        }

        Vector3 smoothed = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref _velocity,
            1f / smoothSpeed
        );

        if (useBounds)
        {
            smoothed.x = Mathf.Clamp(smoothed.x, minX, maxX);
            smoothed.y = Mathf.Clamp(smoothed.y, minY, maxY);
        }

        transform.position = smoothed;
    }

    /// <summary>
    /// Change follow target at runtime.
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
