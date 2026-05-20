Shader "UI/SpotlightOverlay"
{
    Properties
    {
        _MainTex     ("Texture",       2D)     = "white" {}
        _Color       ("Overlay Color", Color)  = (0.04, 0.05, 0.10, 0.82)
        _HolePos     ("Hole Position", Vector) = (0.5, 0.5, 0, 0)
        _HoleSize    ("Hole Size",     Vector) = (0.2, 0.1, 0, 0)
        _HoleRadius  ("Corner Radius", Float)  = 0.02
        _EdgeSoftness("Edge Softness", Float)  = 0.008
    }

    SubShader
    {
        Tags
        {
            "RenderType"  = "Transparent"
            "Queue"       = "Overlay"
            "RenderPipeline" = "UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        Lighting Off

        Pass
        {
            Name "SpotlightOverlay"

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float4 _HolePos;
                float4 _HoleSize;
                float  _HoleRadius;
                float  _EdgeSoftness;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv          = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            // Signed distance function para rectángulo redondeado
            float roundedBoxSDF(float2 p, float2 halfSize, float radius)
            {
                float2 q = abs(p) - halfSize + radius;
                return length(max(q, 0.0)) + min(max(q.x, q.y), 0.0) - radius;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv         = IN.uv;
                float2 holeCenter = _HolePos.xy;
                float2 holeHalf   = _HoleSize.xy * 0.5;

                float dist  = roundedBoxSDF(uv - holeCenter, holeHalf, _HoleRadius);
                float alpha = smoothstep(-_EdgeSoftness, _EdgeSoftness, dist);

                return half4(_Color.rgb, _Color.a * alpha);
            }
            ENDHLSL
        }
    }
}