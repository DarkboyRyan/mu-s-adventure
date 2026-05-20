using UnityEngine;

// Attach this to the DarknessOverlay sprite inside chapter1_level3.
// Drag the Player transform into the Player field in the Inspector.
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerFlashlight : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Light Settings")]
    public float lightRadius = 2f;
    [Range(0.01f, 1f)]
    public float edgeSoftness = 0.4f;
    [Range(0.5f, 1f)]
    public float darknessAlpha = 0.97f;

    private Material _mat;

    static readonly int PlayerWorldPos = Shader.PropertyToID("_PlayerWorldPos");
    static readonly int LightRadius    = Shader.PropertyToID("_LightRadius");
    static readonly int LightSoftness  = Shader.PropertyToID("_LightSoftness");
    static readonly int DarknessAlpha  = Shader.PropertyToID("_DarknessAlpha");

    void Start()
    {
        _mat = GetComponent<SpriteRenderer>().material;
        PushUniforms();
    }

    void Update()
    {
        if (player == null) return;
        _mat.SetVector(PlayerWorldPos,
            new Vector4(player.position.x, player.position.y, 0f, 0f));
        PushUniforms();
    }

    void PushUniforms()
    {
        _mat.SetFloat(LightRadius,   lightRadius);
        _mat.SetFloat(LightSoftness, edgeSoftness);
        _mat.SetFloat(DarknessAlpha, darknessAlpha);
    }
}
