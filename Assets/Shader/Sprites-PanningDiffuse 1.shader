Shader "Sprites/PanningDiffuseURP"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Speed ("Speed", float) = 0.0
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            
            Cull Off
            ZWrite Off
            Blend One OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // Properties
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _Color;
            float _Speed;
            float4 _RendererColor;
            float4 _Flip;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                float4 position = input.positionOS;

                // Flip logic
                position.xy *= _Flip.xy;

                // Pixel Snap (optional)
                #ifdef PIXELSNAP_ON
                position = UnityPixelSnap(position);
                #endif

                output.positionCS = TransformObjectToHClip(position.xyz);
                output.uv = input.uv;
                output.color = input.color * _Color * _RendererColor;
                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                float2 uv = input.uv;
                uv.x -= _Time.y * _Speed; // Pan UV over time

                float4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                float4 finalColor = texColor * input.color;

                return finalColor;
            }
            ENDHLSL
        }
    }

    Fallback "Hidden/InternalErrorShader"
}
