Shader "GrowTown/StylizedWater"
{
    Properties
    {
        _ShallowColor ("Shallow Color", Color) = (0.4, 0.8, 1, 1)
        _DeepColor ("Deep Color", Color) = (0.0, 0.2, 0.4, 1)
        _FoamColor ("Foam Color", Color) = (1,1,1,1)

        _FoamDepth ("Foam Depth", Range(0,2)) = 0.5
        _FoamIntensity ("Foam Intensity", Range(0,2)) = 1.0

        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalStrength ("Normal Strength", Range(0,2)) = 0.8

        _WaveSpeed ("Wave Speed", Range(0,2)) = 0.5
        _WaveScale ("Wave Scale", Range(0.1,10)) = 3.0

        _Opacity ("Water Opacity", Range(0,1)) = 0.9
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            // Properties
            float4 _ShallowColor;
            float4 _DeepColor;
            float4 _FoamColor;
            float _FoamDepth;
            float _FoamIntensity;
            float _NormalStrength;
            float _WaveSpeed;
            float _WaveScale;
            float _Opacity;

            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv * _WaveScale + _Time.y * _WaveSpeed;
                OUT.worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Sample normal map for waves
                float3 normal = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, IN.uv));
                normal.xy *= _NormalStrength;

                // Depth-based coloring (fake by Y position)
                float depthFactor = saturate((IN.worldPos.y + 1.0) * 0.5);
                half4 waterColor = lerp(_DeepColor, _ShallowColor, depthFactor);

                // Foam at edges
                float foam = saturate((1 - depthFactor) * _FoamIntensity);
                waterColor = lerp(waterColor, _FoamColor, foam * _FoamDepth);

                waterColor.a = _Opacity;
                return waterColor;
            }
            ENDHLSL
        }
    }
}
