using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BookColumnFlashlight : MonoBehaviour
{
    [Header("Light Settings")]
    public float lightRadius = 1.5f;
    [Range(0.01f, 1f)]
    public float edgeSoftness = 0.3f;
    [Range(0f, 0.2f)]
    public float darknessFactor = 0.05f;

    private SpriteRenderer _sr;
    private Material _mat;
    private Camera _cam;

    static readonly int LightWorldPos = Shader.PropertyToID("_LightWorldPos");
    static readonly int LightRadius   = Shader.PropertyToID("_LightRadius");
    static readonly int LightSoftness = Shader.PropertyToID("_LightSoftness");
    static readonly int LightActive   = Shader.PropertyToID("_LightActive");
    static readonly int DarknessFactor = Shader.PropertyToID("_DarknessFactor");

    void Start()
    {
        _sr  = GetComponent<SpriteRenderer>();
        _mat = _sr.material;
        _cam = Camera.main;

        // Push initial uniform values so the shader starts fully dark
        _mat.SetFloat(LightActive,    0f);
        _mat.SetFloat(LightRadius,    lightRadius);
        _mat.SetFloat(LightSoftness,  edgeSoftness);
        _mat.SetFloat(DarknessFactor, darknessFactor);
    }

    void Update()
    {
        Vector3 screenPos = Input.mousePosition;
        // Z depth: distance from camera to the sprite plane (camera is at z=-10, sprite at z=0)
        screenPos.z = Mathf.Abs(_cam.transform.position.z);
        Vector3 worldPos = _cam.ScreenToWorldPoint(screenPos);

        // Check if mouse is over this sprite's world bounds (ignore Z)
        Bounds b = _sr.bounds;
        bool over = worldPos.x >= b.min.x && worldPos.x <= b.max.x
                 && worldPos.y >= b.min.y && worldPos.y <= b.max.y;

        _mat.SetVector(LightWorldPos, new Vector4(worldPos.x, worldPos.y, 0f, 0f));
        _mat.SetFloat(LightActive,    over ? 1f : 0f);

        // Keep these in sync if tweaked at runtime in the Inspector
        _mat.SetFloat(LightRadius,    lightRadius);
        _mat.SetFloat(LightSoftness,  edgeSoftness);
        _mat.SetFloat(DarknessFactor, darknessFactor);
    }
}
