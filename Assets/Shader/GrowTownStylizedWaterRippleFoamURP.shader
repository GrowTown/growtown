Shader "GrowTown/StylizedWaterRippleFoamURP"
{
    Properties
    {
        _ShallowColor ("Shallow Color", Color) = (0.3, 0.7, 0.9, 1)
        _DeepColor ("Deep Color", Color) = (0.0, 0.2, 0.4, 1)
        _RippleColor ("Ripple Color", Color) = (1,1,1,0.4)
        _RippleStrength ("Ripple Strength", Range(0,1)) = 0.4
        _RippleSpeed ("Ripple Speed", Range(0.1,5)) = 1.0
        _RippleFrequency ("Ripple Frequency", Range(1,10)) = 4.0
        _RippleFade ("Ripple Fade", Range(0,10)) = 4.0
        _WaveSpeed ("Wave Speed", Range(0,3)) = 0.5
        _WaveScale ("Wave Scale", Range(0.1,5)) = 1.0
        _FoamColor ("Foam Color", Color) = (1,1,1,1)
        _FoamIntensity ("Foam Intensity", Range(0,1)) = 0.4
        _FoamRadius ("Foam Radius", Range(0,2)) = 0.5
        _StoneCount ("Number of Stones", Range(1,10)) = 1
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

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _ShallowColor;
                float4 _DeepColor;
                float4 _RippleColor;
                float4 _FoamColor;
                float _RippleStrength;
                float _RippleSpeed;
                float _RippleFrequency;
                float _RippleFade;
                float _WaveSpeed;
                float _WaveScale;
                float _FoamIntensity;
                float _FoamRadius;
                int _StoneCount;
            CBUFFER_END

            float4 _StonePositions[10];

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.worldNormal = TransformObjectToWorldNormal(IN.normalOS);
                OUT.positionHCS = TransformWorldToHClip(OUT.worldPos);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                // Base depth color
                float depthFactor = saturate((IN.worldPos.y + 1.0) * 0.5);
                half4 baseColor = lerp(_DeepColor, _ShallowColor, depthFactor);

                // Gentle wave motion overlay
                float wave = sin((IN.worldPos.x + _Time.y * _WaveSpeed) * _WaveScale) * 0.05 +
                             cos((IN.worldPos.z + _Time.y * _WaveSpeed * 1.3) * _WaveScale * 1.1) * 0.05;
                baseColor.rgb += wave * 0.1; // slight shimmer tint

                // Ripple accumulation
                float rippleTotal = 0.0;
                float foamTotal = 0.0;

                [unroll(10)]
                for (int i = 0; i < _StoneCount; i++)
                {
                    float3 stonePos = _StonePositions[i].xyz;
                    float dist = distance(IN.worldPos.xz, stonePos.xz);

                    // Ripple pattern
                    float wave = sin(dist * _RippleFrequency - _Time.y * _RippleSpeed);
                    wave *= exp(-dist * _RippleFade);
                    rippleTotal += wave;

                    // Foam ring near stone
                    float foam = smoothstep(_FoamRadius * 0.5, _FoamRadius, dist);
                    foam = (1.0 - foam) * _FoamIntensity * exp(-dist);
                    foamTotal += foam;
                }

                rippleTotal *= _RippleStrength;
                half4 rippleColor = _RippleColor * rippleTotal;

                // Combine ripple, foam, base
                half3 finalColor = baseColor.rgb + rippleColor.rgb + _FoamColor.rgb * foamTotal;

                return half4(finalColor, 0.9);
            }
            ENDHLSL
        }
    }
}
