Shader "Custom/TileCleanerShader"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _TargetColor ("Target Color", Color) = (1,1,1,1)
        _TouchPoint ("Touch Position", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        CGPROGRAM
        #pragma surface surf Standard

        struct Input
        {
            float2 uv_MainTex;
        };

        float4 _BaseColor;
        float4 _TargetColor;
        float4 _TouchPoint;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float distanceFromTouch = distance(IN.uv_MainTex, _TouchPoint.xy);
            float effect = smoothstep(_TouchPoint.z, 0.0, distanceFromTouch);
            o.Albedo = lerp(_TargetColor.rgb, _BaseColor.rgb, effect);
        }
        ENDCG
    }
}
