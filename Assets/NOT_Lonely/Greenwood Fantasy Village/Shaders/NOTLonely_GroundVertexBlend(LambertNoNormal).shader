// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "NOT_Lonely/Ground Vertex Blend (Lambert, No normal)"
{
	Properties
	{
		_TriplanarHardness("Triplanar Hardness", Range( 1.1 , 25)) = 5
		[NoScaleOffset][SingleLineTexture]_TextureY01("Grass, Albedo(RGB)", 2D) = "white" {}
		_ScaleandOffsetY01("Grass Scale and Offset", Vector) = (1,1,0,0)
		[NoScaleOffset][SingleLineTexture]_BlendY01("Grass Blend Map", 2D) = "white" {}
		_HeightBlendHardness("Height Blend Hardness", Range( 0 , 1)) = 1
		[NoScaleOffset][SingleLineTexture]_TextureY02("Ground_1, Albedo(RGB)", 2D) = "white" {}
		_ScaleandOffsetY02("Ground_1 Scale and Offset", Vector) = (1,1,0,0)
		[NoScaleOffset][SingleLineTexture]_TextureY03("Ground_2, Albedo(RGB)", 2D) = "white" {}
		_ScaleandOffsetY03("Ground_2 Scale and Offset", Vector) = (1,1,0,0)
		[NoScaleOffset][SingleLineTexture]_TextureXZ("Rock, Albedo(RGB)", 2D) = "white" {}
		_ScaleandOffsetXZ("Rock Scale and Offset", Vector) = (1,1,0,0)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldPos;
			float4 vertexColor : COLOR;
		};

		uniform float _TriplanarHardness;
		uniform sampler2D _BlendY01;
		uniform float4 _ScaleandOffsetY01;
		uniform float _HeightBlendHardness;
		uniform sampler2D _TextureXZ;
		uniform float4 _ScaleandOffsetXZ;
		uniform sampler2D _TextureY01;
		uniform sampler2D _TextureY02;
		uniform float4 _ScaleandOffsetY02;
		uniform sampler2D _TextureY03;
		uniform float4 _ScaleandOffsetY03;


		float3 PowerNormal218( float3 worldNormal, float triplanarHardness )
		{
			float3 powerNormal;	
			powerNormal = abs(worldNormal);
			powerNormal = pow(max(0.0001, powerNormal - 0.2) * 7,  triplanarHardness);
			powerNormal = normalize(max(powerNormal, 0.0001));
			powerNormal /= powerNormal.x + powerNormal.y + powerNormal.z;
			return powerNormal;
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Normal = float3(0,0,1);
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float3 normalizeResult64_g21 = normalize( (WorldNormalVector( i , float3(0,0,1) )) );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult14_g21 = dot( normalizeResult64_g21 , ase_worldlightDir );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 worldNormal218 = ase_worldNormal;
			float triplanarHardness218 = _TriplanarHardness;
			float3 localPowerNormal218 = PowerNormal218( worldNormal218 , triplanarHardness218 );
			float3 break109 = localPowerNormal218;
			float3 break11_g22 = i.vertexColor.rgb;
			float2 appendResult41_g20 = (float2(ase_worldPos.x , ase_worldPos.z));
			float4 temp_output_1_0_g20 = ( _ScaleandOffsetY01 * 10.0 );
			float2 temp_output_216_5 = ( ( appendResult41_g20 / (temp_output_1_0_g20).xy ) + (temp_output_1_0_g20).zw );
			float smoothstepResult15_g22 = smoothstep( 0.7 , 1.0 , ( ( 1.0 - max( ( 1.0 - max( ( break109.y * (sign( ase_worldNormal )).y ) , 0.0 ) ) , max( break11_g22.y , break11_g22.z ) ) ) + ( tex2D( _BlendY01, temp_output_216_5 ).r * ( _HeightBlendHardness * 0.78 ) ) ));
			float temp_output_152_0 = smoothstepResult15_g22;
			float temp_output_152_5 = saturate( ( break11_g22.z - smoothstepResult15_g22 ) );
			float temp_output_152_6 = saturate( ( break11_g22.y - smoothstepResult15_g22 ) );
			float2 appendResult40_g20 = (float2(ase_worldPos.z , ase_worldPos.y));
			float4 temp_output_4_0_g20 = ( 10.0 * _ScaleandOffsetXZ );
			float2 temp_output_24_0_g20 = (temp_output_4_0_g20).xy;
			float2 temp_output_25_0_g20 = (temp_output_4_0_g20).zw;
			float2 appendResult42_g20 = (float2(( 1.0 - ase_worldPos.x ) , ase_worldPos.y));
			float4 temp_output_2_0_g20 = ( _ScaleandOffsetY02 * 10.0 );
			float4 temp_output_3_0_g20 = ( 10.0 * _ScaleandOffsetY03 );
			float4 temp_output_42_0_g21 = ( ( ( 1.0 - saturate( ( temp_output_152_0 + temp_output_152_5 + temp_output_152_6 ) ) ) * ( ( tex2D( _TextureXZ, ( ( ( appendResult40_g20 + float2( 0.33,0.33 ) ) / temp_output_24_0_g20 ) + temp_output_25_0_g20 ) ) * break109.x ) + ( tex2D( _TextureXZ, ( ( appendResult41_g20 / temp_output_24_0_g20 ) + temp_output_25_0_g20 ) ) * break109.y ) + ( tex2D( _TextureXZ, ( ( ( appendResult42_g20 + float2( -0.33,-0.33 ) ) / temp_output_24_0_g20 ) + temp_output_25_0_g20 ) ) * break109.z ) ) ) + ( ( ( tex2D( _TextureY01, temp_output_216_5 ) * temp_output_152_0 ) + ( tex2D( _TextureY02, ( ( appendResult41_g20 / (temp_output_2_0_g20).xy ) + (temp_output_2_0_g20).zw ) ) * temp_output_152_5 ) + ( tex2D( _TextureY03, ( ( appendResult41_g20 / (temp_output_3_0_g20).xy ) + (temp_output_3_0_g20).zw ) ) * temp_output_152_6 ) ) * break109.y ) );
			o.Emission = ( ( ( ( ase_lightColor * 1 ) * max( dotResult14_g21 , 0.0 ) ) + float4( float3(0,0,0) , 0.0 ) ) * float4( (temp_output_42_0_g21).rgb , 0.0 ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Unlit keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float4 tSpace0 : TEXCOORD1;
				float4 tSpace1 : TEXCOORD2;
				float4 tSpace2 : TEXCOORD3;
				half4 color : COLOR0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.color = v.color;
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				surfIN.vertexColor = IN.color;
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=18933
0;73;2560;1286;3019.295;-642.5082;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;219;-5362.468,655.6072;Inherit;False;802.2231;321.4517;Normal direct mask;6;10;200;218;109;244;245;;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldNormalVector;200;-5226.508,705.6072;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;10;-5312.468,861.059;Inherit;False;Property;_TriplanarHardness;Triplanar Hardness;0;0;Create;False;0;0;0;False;0;False;5;0;1.1;25;0;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;218;-4994.748,740.1404;Inherit;False;float3 powerNormal@	$powerNormal = abs(worldNormal)@$powerNormal = pow(max(0.0001, powerNormal - 0.2) * 7,  triplanarHardness)@$powerNormal = normalize(max(powerNormal, 0.0001))@$powerNormal /= powerNormal.x + powerNormal.y + powerNormal.z@$return powerNormal@;3;Create;2;True;worldNormal;FLOAT3;0,0,0;In;;Inherit;False;True;triplanarHardness;FLOAT;0;In;;Inherit;False;PowerNormal;True;False;0;;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SignOpNode;244;-4966.573,880.0302;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;109;-4712.245,732.8732;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.CommentaryNode;75;-5773.126,1084.162;Inherit;False;1220.052;1061.045;UV coordinates;10;230;229;228;226;227;64;61;62;63;216;;1,1,1,1;0;0
Node;AmplifyShaderEditor.ComponentMaskNode;245;-4803.339,874.748;Inherit;False;False;True;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;61;-5649.556,1178.122;Inherit;False;Property;_ScaleandOffsetY01;Grass Scale and Offset;4;0;Create;False;0;0;0;False;0;False;1,1,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;227;-5413.232,1575.871;Inherit;False;Constant;_Float0;Float 0;12;0;Create;True;0;0;0;False;0;False;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;63;-5660.93,1626.001;Inherit;False;Property;_ScaleandOffsetY03;Ground_2 Scale and Offset;10;0;Create;False;0;0;0;False;0;False;1,1,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;246;-4453.315,811.2053;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;62;-5659.25,1390.805;Inherit;False;Property;_ScaleandOffsetY02;Ground_1 Scale and Offset;8;0;Create;False;0;0;0;False;0;False;1,1,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;64;-5629.739,1857.881;Inherit;False;Property;_ScaleandOffsetXZ;Rock Scale and Offset;12;0;Create;False;0;0;0;False;0;False;1,1,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMaxOpNode;247;-4297.14,827.4948;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;226;-5221.819,1431.437;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;229;-5223.232,1659.871;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;89;-3785.482,1590.44;Inherit;False;1025.636;518.6749;Blend Map;5;83;86;152;14;248;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;230;-5219.232,1779.871;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;228;-5223.232,1551.871;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FunctionNode;216;-5036.634,1587.79;Inherit;False;NOTLonely_GroundVertexBlend_UVcoordinate;-1;;20;099b126e870f78747a4bfba7e2948403;0;4;1;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;0,0,0,0;False;6;FLOAT2;5;FLOAT2;6;FLOAT2;7;FLOAT2;0;FLOAT2;55;FLOAT2;29
Node;AmplifyShaderEditor.WireNode;129;-3806.994,2136.733;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;235;-3816.672,2175.309;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;127;-3814.834,2227.099;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;83;-3765.103,1832.882;Inherit;False;Property;_HeightBlendHardness;Height Blend Hardness;6;0;Create;False;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;191;-3909.952,1858.762;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;248;-3433.008,1824.156;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.78;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;128;-3785.838,2240.197;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;232;-3943.376,2607.892;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WireNode;144;-3939.876,2486.479;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WireNode;145;-3932.604,2727.165;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WireNode;130;-3780.321,2152.327;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;236;-3784.827,2201.183;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;192;-3876.93,1889.947;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;27;-3782.75,2387.335;Inherit;False;885.134;808.7349;Ground_01 input;3;79;28;80;;1,1,1,1;0;0
Node;AmplifyShaderEditor.VertexColorNode;86;-3732.011,1932.991;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;14;-3741.482,1640.441;Inherit;True;Property;_BlendY01;Grass Blend Map;5;2;[NoScaleOffset];[SingleLineTexture];Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;122;-3856.439,346.1325;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;22;-3783.52,1186.783;Inherit;False;389.3765;323.8153;Ground_01 input;1;23;;1,1,1,1;0;0
Node;AmplifyShaderEditor.WireNode;233;-3907.548,2621.327;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WireNode;131;-2867.289,2230.761;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;188;-3898.153,2740.652;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WireNode;187;-3911.153,2501.652;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;152;-3251.163,1789.279;Inherit;False;NOTLonely_GroundVertexBlend_TopTexturesMasks;-1;;22;42dddd6b014cdd142b23116d9318c762;0;4;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;3;FLOAT;0;FLOAT;5;FLOAT;6
Node;AmplifyShaderEditor.CommentaryNode;16;-3774.374,501.9578;Inherit;False;391.5285;310.1388;Grass input;1;11;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;21;-3778.095,846.0515;Inherit;False;379.3777;309.5123;Ground_01 input;1;17;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TexturePropertyNode;79;-3744.044,2937.794;Inherit;True;Property;_TextureXZ;Rock, Albedo(RGB);11;2;[NoScaleOffset];[SingleLineTexture];Create;False;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.WireNode;237;-2867.827,2276.183;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;132;-2870.137,2318.937;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;238;-2831.827,2294.183;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;133;-2829.014,2244.845;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;231;-3313.825,2637.326;Inherit;True;Property;_TextureSample0;Texture Sample 0;18;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;240;-2690.016,1706.819;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;123;-3790.942,330.4131;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;80;-3297.28,2841.54;Inherit;True;Property;_xz1;xz1;18;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;17;-3728.095,896.0515;Inherit;True;Property;_TextureY02;Ground_1, Albedo(RGB);7;2;[NoScaleOffset];[SingleLineTexture];Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;11;-3721.862,560.8777;Inherit;True;Property;_TextureY01;Grass, Albedo(RGB);3;2;[NoScaleOffset];[SingleLineTexture];Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;23;-3733.52,1236.783;Inherit;True;Property;_TextureY03;Ground_2, Albedo(RGB);9;2;[NoScaleOffset];[SingleLineTexture];Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;28;-3301.254,2437.335;Inherit;True;Property;_xz0;xz0;18;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;134;-2836.518,2336.322;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;100;-2678.681,1300.507;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;234;-2539.526,2533.996;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;136;-2534.378,2716.383;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;114;-2407.443,857.1937;Inherit;False;418.5251;229.3083;Combine top textures;2;110;104;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;135;-2535.02,2389.946;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;96;-2689.295,936.8679;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;121;-2402.385,327.7929;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;241;-2549.396,1711.129;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;92;-2748.267,627.2844;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;242;-2404.793,1708.227;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;120;-2357.847,374.952;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;104;-2353.756,907.1937;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;239;-2285.063,1893.255;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;110;-2128.596,920.8292;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;243;-2091.745,1712.929;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;148;-1598.054,1234.815;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;220;-1384.193,1229.13;Inherit;False;Lambert Light;1;;21;9be9b95d80559e74dac059ac0a4060cf;0;2;42;COLOR;0,0,0,0;False;52;FLOAT3;0,0,0;False;2;COLOR;0;FLOAT;57
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;249;-1075.645,1212.975;Float;False;True;-1;2;;0;0;Unlit;NOT_Lonely/Ground Vertex Blend (Lambert, No normal);False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;218;0;200;0
WireConnection;218;1;10;0
WireConnection;244;0;200;0
WireConnection;109;0;218;0
WireConnection;245;0;244;0
WireConnection;246;0;109;1
WireConnection;246;1;245;0
WireConnection;247;0;246;0
WireConnection;226;0;61;0
WireConnection;226;1;227;0
WireConnection;229;0;227;0
WireConnection;229;1;63;0
WireConnection;230;0;227;0
WireConnection;230;1;64;0
WireConnection;228;0;62;0
WireConnection;228;1;227;0
WireConnection;216;1;226;0
WireConnection;216;2;228;0
WireConnection;216;3;229;0
WireConnection;216;4;230;0
WireConnection;129;0;109;0
WireConnection;235;0;109;1
WireConnection;127;0;109;2
WireConnection;191;0;247;0
WireConnection;248;0;83;0
WireConnection;128;0;127;0
WireConnection;232;0;216;55
WireConnection;144;0;216;0
WireConnection;145;0;216;29
WireConnection;130;0;129;0
WireConnection;236;0;235;0
WireConnection;192;0;191;0
WireConnection;14;1;216;5
WireConnection;122;0;109;1
WireConnection;233;0;232;0
WireConnection;131;0;130;0
WireConnection;188;0;145;0
WireConnection;187;0;144;0
WireConnection;152;1;14;1
WireConnection;152;2;248;0
WireConnection;152;3;86;0
WireConnection;152;4;192;0
WireConnection;237;0;236;0
WireConnection;132;0;128;0
WireConnection;238;0;237;0
WireConnection;133;0;131;0
WireConnection;231;0;79;0
WireConnection;231;1;233;0
WireConnection;240;0;152;0
WireConnection;240;1;152;5
WireConnection;240;2;152;6
WireConnection;123;0;122;0
WireConnection;80;0;79;0
WireConnection;80;1;188;0
WireConnection;17;1;216;6
WireConnection;11;1;216;5
WireConnection;23;1;216;7
WireConnection;28;0;79;0
WireConnection;28;1;187;0
WireConnection;134;0;132;0
WireConnection;100;0;23;0
WireConnection;100;1;152;6
WireConnection;234;0;231;0
WireConnection;234;1;238;0
WireConnection;136;0;80;0
WireConnection;136;1;134;0
WireConnection;135;0;28;0
WireConnection;135;1;133;0
WireConnection;96;0;17;0
WireConnection;96;1;152;5
WireConnection;121;0;123;0
WireConnection;241;0;240;0
WireConnection;92;0;11;0
WireConnection;92;1;152;0
WireConnection;242;0;241;0
WireConnection;120;0;121;0
WireConnection;104;0;92;0
WireConnection;104;1;96;0
WireConnection;104;2;100;0
WireConnection;239;0;135;0
WireConnection;239;1;234;0
WireConnection;239;2;136;0
WireConnection;110;0;104;0
WireConnection;110;1;120;0
WireConnection;243;0;242;0
WireConnection;243;1;239;0
WireConnection;148;0;243;0
WireConnection;148;1;110;0
WireConnection;220;42;148;0
WireConnection;249;2;220;0
ASEEND*/
//CHKSM=B5FD0AE2664A309531369EE2CB7BEE56EAA47A36