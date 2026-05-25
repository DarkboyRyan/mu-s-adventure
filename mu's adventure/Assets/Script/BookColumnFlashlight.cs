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
    private Vector3 _frozenPos;

    static readonly int LightWorldPos  = Shader.PropertyToID("_LightWorldPos");
    static readonly int LightRadius    = Shader.PropertyToID("_LightRadius");
    static readonly int LightSoftness  = Shader.PropertyToID("_LightSoftness");
    static readonly int LightActive    = Shader.PropertyToID("_LightActive");
    static readonly int DarknessFactor = Shader.PropertyToID("_DarknessFactor");

    void Start()
    {
        _sr  = GetComponent<SpriteRenderer>();
        _mat = _sr.material;
        _cam = Camera.main;

        // Start frozen at the sprite center so the circle doesn't jump on first entry
        _frozenPos = transform.position;

        _mat.SetFloat(LightActive,    1f);
        _mat.SetFloat(LightRadius,    lightRadius);
        _mat.SetFloat(LightSoftness,  edgeSoftness);
        _mat.SetFloat(DarknessFactor, darknessFactor);
    }

    void Update()
    {
        Vector3 screenPos = Input.mousePosition;
        screenPos.z = Mathf.Abs(_cam.transform.position.z);
        Vector3 worldPos = _cam.ScreenToWorldPoint(screenPos);

        Bounds b = _sr.bounds;
        bool over = worldPos.x >= b.min.x && worldPos.x <= b.max.x
                 && worldPos.y >= b.min.y && worldPos.y <= b.max.y;

        // Only advance the light position while the mouse is inside the sprite
        if (over)
            _frozenPos = worldPos;

        _mat.SetVector(LightWorldPos, new Vector4(_frozenPos.x, _frozenPos.y, 0f, 0f));
        _mat.SetFloat(LightActive,    1f);
        _mat.SetFloat(LightRadius,    lightRadius);
        _mat.SetFloat(LightSoftness,  edgeSoftness);
        _mat.SetFloat(DarknessFactor, darknessFactor);
    }
}
