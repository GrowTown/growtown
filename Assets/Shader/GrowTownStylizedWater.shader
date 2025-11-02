Shader \"GrowTown/StylizedWater\"
{
    Properties
    {
        _ShallowColor (\"Shallow Color\", Color) = (0.3, 0.7, 0.9, 1)
        _DeepColor (\"Deep Color\", Color) = (0.0, 0.25, 0.4, 1)
        _FoamColor (\"Foam Color\", Color) = (1,1,1,1)

        _FoamDepth (\"Foam Depth\", Range(0,3)) = 1.2
        _FoamIntensity (\"Foam Intensity\", Range(0,2)) = 1.0

        _NormalMap (\"Normal Map\", 2D) = \"bump\" {}
        _NormalStrength (\"Normal Strength\", Range(0,2)) = 0.6

        _WaveSpeed (\"Wave Speed\", Range(0,2)) = 0.5
        _WaveScale (\"Wave Scale\", Range(0,5)) = 3.0

        _WaterOpacity (\"Water Opacity\", Range(0,1)) = 0.85

        _CausticsTex (\"Caustics (Optional)\", 2D) = \"white\" {}
        _CausticsStrength (\"Caustics Strength\", Range(0,1)) = 0.2

        _RippleStrength (\"Ripple Normal Strength\", Range(0,1)) = 0.35
        _RippleFrequency (\"Ripple Frequency\", Range(0,20)) = 8.0
        _RippleSpeed (\"Ripple Speed\", Range(0,10)) = 3.0
        _RippleDepthFalloff (\"Ripple Depth Falloff\", Range(0.1,5)) = 1.5
        _RippleColorInfluence (\"Ripple Color Influence\", Range(0,0.5)) = 0.12
    }

    SubShader
    {
        Tags { \"RenderType\"=\"Transparent\" \"Queue\"=\"Transparent\" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include \"Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl\"

            TEXTURE2D(_NormalMap); SAMPLER(sampler_NormalMap);
            TEXTURE2D(_CausticsTex); SAMPLER(sampler_CausticsTex);

            float4 _ShallowColor, _DeepColor, _FoamColor;
            float _FoamDepth, _FoamIntensity;
            float _NormalStrength;
            float _WaveSpeed, _WaveScale;
            float _WaterOpacity;
            float _CausticsStrength;
            float _RippleStrength, _RippleFrequency, _RippleSpeed, _RippleDepthFalloff, _RippleColorInfluence;

            #define MAX_RIPPLES 16
            float4 _RippleSources[MAX_RIPPLES]; // xyz = world position, w = amplitude
            int _RippleSourceCount;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float depthFactor = saturate((IN.worldPos.y + _FoamDepth) * 0.5);
                half4 waterColor = lerp(_DeepColor, _ShallowColor, depthFactor);

                float2 uvWave = IN.uv * _WaveScale + _Time.y * _WaveSpeed;
                float3 normal = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, uvWave));
                normal = normalize(normal * _NormalStrength);

                float rippleOffset = 0.0f;
                int rippleCount = _RippleSourceCount;
                [loop]
                for (int i = 0; i < MAX_RIPPLES; ++i)
                {
                    if (i >= rippleCount)
                        break;

                    float4 source = _RippleSources[i];
                    float amplitude = source.w;
                    if (amplitude <= 0.0001f)
                        continue;

                    float2 toSource = IN.worldPos.xz - source.xz;
                    float dist = length(toSource);
                    float falloff = exp(-dist * _RippleDepthFalloff);
                    float phase = dist * _RippleFrequency - _Time.y * _RippleSpeed;
                    rippleOffset += sin(phase) * amplitude * falloff;
                }

                if (abs(rippleOffset) > 0.0001f)
                {
                    normal.xy += rippleOffset * _RippleStrength;
                    waterColor.rgb += rippleOffset * _RippleColorInfluence;
                }

                float foam = saturate(1.0 - depthFactor) * _FoamIntensity;

                float2 causticUV = IN.uv * 5 + float2(_Time.y * 0.1, _Time.y * 0.15);
                half4 caustics = SAMPLE_TEXTURE2D(_CausticsTex, sampler_CausticsTex, causticUV);
                waterColor.rgb += caustics.rgb * _CausticsStrength;

                waterColor.rgb += _FoamColor.rgb * foam;
                waterColor.a = _WaterOpacity;

                return waterColor;
            }
            ENDHLSL
        }
    }
}
