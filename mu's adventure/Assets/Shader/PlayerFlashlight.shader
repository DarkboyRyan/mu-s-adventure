Shader "Custom/PlayerFlashlight"
{
    Properties
    {
        // Set at runtime by PlayerFlashlight.cs
        _PlayerWorldPos ("Player World Pos", Vector)     = (0,0,0,0)
        _LightRadius    ("Light Radius",     Float)      = 2.0
        _LightSoftness  ("Edge Softness",    Float)      = 0.4
        _DarknessAlpha  ("Darkness Alpha",   Range(0,1)) = 0.97
    }

    SubShader
    {
        Tags
        {
            "Queue"          = "Transparent"
            "RenderType"     = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 worldPos    : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _PlayerWorldPos;
                float  _LightRadius;
                float  _LightSoftness;
                float  _DarknessAlpha;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.worldPos    = TransformObjectToWorld(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float dist  = distance(IN.worldPos.xy, _PlayerWorldPos.xy);
                float inner = _LightRadius * (1.0 - _LightSoftness);

                // 0 = inside the light circle (fully transparent)
                // 1 = outside the light circle (opaque black)
                float darkness = smoothstep(inner, _LightRadius, dist);

                return half4(0.0, 0.0, 0.0, darkness * _DarknessAlpha);
            }
            ENDHLSL
        }
    }
}
