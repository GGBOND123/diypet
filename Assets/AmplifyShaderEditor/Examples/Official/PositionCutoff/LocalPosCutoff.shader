// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/LocalPosCutoff"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Distribution("Distribution", Range( 0.1 , 10)) = 1
		_SmoothnessFactor("SmoothnessFactor", Range( 0 , 1)) = 0
		_StartPoint("StartPoint", Range( -10 , 10)) = 0.75
		_UnderwaterInfluence("UnderwaterInfluence", Range( 0 , 1)) = 0
		_Tint("Tint", Color) = (0.5294118,0.4264289,0,0)
		_Normals("Normals", 2D) = "bump" {}
		_Albedo("Albedo", 2D) = "white" {}
		_Occlusion("Occlusion", 2D) = "white" {}
		_Metallic("Metallic", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		ZTest LEqual
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform sampler2D _Normals;
		uniform float4 _Normals_ST;
		uniform float4 _Tint;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float _StartPoint;
		uniform float _Distribution;
		uniform float _UnderwaterInfluence;
		uniform sampler2D _Metallic;
		uniform float4 _Metallic_ST;
		uniform float _SmoothnessFactor;
		uniform sampler2D _Occlusion;
		uniform float4 _Occlusion_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normals = i.uv_texcoord * _Normals_ST.xy + _Normals_ST.zw;
			o.Normal = UnpackNormal( tex2D( _Normals, uv_Normals ) );
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 ase_vertexPos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float temp_output_15_0 = saturate( ( ( ase_vertexPos.xyz.y + _StartPoint ) / _Distribution ) );
			o.Albedo = lerp( _Tint , tex2D( _Albedo, uv_Albedo ) , clamp( temp_output_15_0 , _UnderwaterInfluence , 1.0 ) ).xyz;
			o.Emission = float4(0.5220588,0.3297548,0.1134128,0).rgb;
			float2 uv_Metallic = i.uv_texcoord * _Metallic_ST.xy + _Metallic_ST.zw;
			float4 temp_cast_3 = (( 1.0 - temp_output_15_0 )).xxxx;
			float4 temp_output_49_0 = ( tex2D( _Metallic, uv_Metallic ) + temp_cast_3 );
			o.Metallic = temp_output_49_0.x;
			float4 temp_cast_5 = (( 1.0 - temp_output_15_0 )).xxxx;
			o.Smoothness = ( temp_output_49_0 * _SmoothnessFactor ).x;
			float2 uv_Occlusion = i.uv_texcoord * _Occlusion_ST.xy + _Occlusion_ST.zw;
			o.Occlusion = tex2D( _Occlusion, uv_Occlusion ).x;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=10001
49;29;1544;834;1661.435;638.2424;2.127273;True;True
Node;AmplifyShaderEditor.CommentaryNode;27;-1268.119,489.6995;Float;False;1032.899;469.7996;Cutoff;9;15;2;16;5;14;30;54;47;50;;0;0
Node;AmplifyShaderEditor.RangedFloatNode;2;-1247.918,722.3019;Float;False;Property;_StartPoint;StartPoint;2;0;0.75;-10;10;0;1;FLOAT
Node;AmplifyShaderEditor.PosVertexDataNode;47;-1234.361,543.9348;Float;False;0;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;5;-1254.918,841.5022;Float;False;Property;_Distribution;Distribution;0;0;1;0.1;10;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;14;-997.0165,635.501;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;16;-876.0185,742.7026;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;21;-914.6158,-533.7644;Float;False;719.1993;440.2003;Color Stuff;4;20;19;18;17;;0;0
Node;AmplifyShaderEditor.SaturateNode;15;-712.8203,722.0024;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;18;-526.317,-278.5635;Float;True;Property;_Metallic;Metallic;8;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.OneMinusNode;50;-422.1988,688.3354;Float;False;1;0;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;54;-851.6987,535.1356;Float;False;Property;_UnderwaterInfluence;UnderwaterInfluence;3;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;17;-512.4171,-483.7644;Float;True;Property;_Albedo;Albedo;6;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;34;-108.9506,481.5498;Float;False;Property;_SmoothnessFactor;SmoothnessFactor;1;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;30;-492.2693,530.6258;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;53;-519.748,-93.96416;Float;False;Property;_Tint;Tint;4;0;0.5294118,0.4264289,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;49;0.6511464,115.0344;Float;False;2;0;FLOAT4;0.0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;182.0511,215.8345;Float;False;2;0;FLOAT4;0.0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.WorldPosInputsNode;59;-1229.238,316.9132;Float;False;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;60;-665.8712,170.1214;Float;False;Constant;_Color0;Color 0;9;0;0.5220588,0.3297548,0.1134128,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;20;-838.4163,-314.6636;Float;True;Property;_Occlusion;Occlusion;7;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;19;-864.6157,-466.2642;Float;True;Property;_Normals;Normals;5;0;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.LerpOp;52;-233.6482,-23.06393;Float;False;3;0;COLOR;0.0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;432.1003,45.80003;Float;False;True;2;Float;ASEMaterialInspector;0;Standard;ASESampleShaders/LocalPosCutoff;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;3;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;Relative;0;;-1;-1;-1;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0.0;False;2;FLOAT3;0.0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0.0;False;7;FLOAT3;0.0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;13;FLOAT3;0.0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;14;0;47;2
WireConnection;14;1;2;0
WireConnection;16;0;14;0
WireConnection;16;1;5;0
WireConnection;15;0;16;0
WireConnection;50;0;15;0
WireConnection;30;0;15;0
WireConnection;30;1;54;0
WireConnection;49;0;18;0
WireConnection;49;1;50;0
WireConnection;51;0;49;0
WireConnection;51;1;34;0
WireConnection;52;0;53;0
WireConnection;52;1;17;0
WireConnection;52;2;30;0
WireConnection;0;0;52;0
WireConnection;0;1;19;0
WireConnection;0;2;60;0
WireConnection;0;3;49;0
WireConnection;0;4;51;0
WireConnection;0;5;20;0
ASEEND*/
//CHKSM=51982DA6F0DE086716CE9CDA2ED6AF41FBF54600