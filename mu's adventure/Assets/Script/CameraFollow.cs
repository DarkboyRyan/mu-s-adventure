using UnityEngine;

/// <summary>
/// 视角摄像机跟随脚本。挂到 Main Camera 上，指定要跟随的目标即可。
/// 支持 2D（只跟 XY）和 3D（跟 XYZ），可调平滑度与偏移。
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("跟随目标")]
    [Tooltip("要跟随的物体（如玩家、画布中心等），不填则不会移动")]
    public Transform target;

    [Header("跟随模式")]
    [Tooltip("勾选则只在 XY 平面跟随（2D 游戏）；不勾选则 XYZ 全跟随（3D）")]
    public bool follow2D = true;

    [Header("偏移")]
    [Tooltip("摄像机相对目标的位置偏移")]
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("平滑")]
    [Tooltip("跟随平滑度，越大跟得越紧，越小越平滑")]
    [Range(1f, 30f)]
    public float smoothSpeed = 10f;

    [Header("边界（可选）")]
    [Tooltip("是否限制摄像机在矩形范围内移动")]
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
            targetPosition.z = transform.position.z; // 保持摄像机自身 Z
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
    /// 运行时切换跟随目标
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
