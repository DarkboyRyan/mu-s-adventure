Shader "Custom/BookColumnFlashlight"
{
    Properties
    {
        _MainTex        ("Sprite Texture", 2D)      = "white" {}
        _Color          ("Tint",           Color)   = (1,1,1,1)

        // Set at runtime by BookColumnFlashlight.cs — do not edit manually
        _LightWorldPos  ("Light World Pos", Vector) = (0,0,0,0)
        _LightRadius    ("Light Radius",    Float)  = 1.5
        _LightSoftness  ("Edge Softness",   Float)  = 0.3
        _LightActive    ("Light Active",    Float)  = 0
        _DarknessFactor ("Darkness Factor", Float)  = 0.05
    }

    SubShader
    {
        Tags
        {
            "Queue"           = "Transparent"
            "RenderType"      = "Transparent"
            "RenderPipeline"  = "UniversalPipeline"
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
                float2 uv         : TEXCOORD0;
                float4 color      : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float4 color       : COLOR;
                float3 worldPos    : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float4 _LightWorldPos;
                float  _LightRadius;
                float  _LightSoftness;
                float  _LightActive;
                float  _DarknessFactor;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv          = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color       = IN.color;
                OUT.worldPos    = TransformObjectToWorld(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv)
                            * _Color * IN.color;

                // Distance from this pixel to the mouse world position
                float dist  = distance(IN.worldPos.xy, _LightWorldPos.xy);

                // Soft circle: full brightness inside inner edge, fades out to outer edge
                float inner = _LightRadius * (1.0 - _LightSoftness);
                float mask  = 1.0 - smoothstep(inner, _LightRadius, dist);

                // Brightness: between darknessFactor (fully dark) and 1 (fully lit)
                float bright = lerp(_DarknessFactor, 1.0, mask);

                // When mouse is not over the BookColumn, force full darkness
                bright = lerp(_DarknessFactor, bright, _LightActive);

                col.rgb *= bright;
                return col;
            }
            ENDHLSL
        }
    }

    FallBack "Sprites/Default"
}
