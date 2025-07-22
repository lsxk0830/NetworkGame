// Made with Amplify Shader Editor v1.9.1.8
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Raygeas/Suntail Surface"
{
	Properties
	{
		[Header(Maps)][Space(10)][MainTexture]_Albedo("Albedo", 2D) = "white" {}
		[Normal]_Normal("Normal", 2D) = "bump" {}
		_MetallicSmoothness("Metallic/Smoothness", 2D) = "white" {}
		[HDR]_Emission("Emission", 2D) = "white" {}
		[Header(Settings)][Space(5)]_Color("Color", Color) = (1,1,1,0)
		[HDR]_EmissionColor("Emission", Color) = (0,0,0,1)
		_NormalScale("Normal", Float) = 1
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_SurfaceSmoothness("Smoothness", Range( 0 , 1)) = 0
		[KeywordEnum(Metallic_Alpha,Albedo_Alpha)] _SmoothnessSource("Smoothness Source", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma multi_compile_local _SMOOTHNESSSOURCE_METALLIC_ALPHA _SMOOTHNESSSOURCE_ALBEDO_ALPHA
		#pragma multi_compile __ LOD_FADE_CROSSFADE
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows dithercrossfade 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float _NormalScale;
		uniform float4 _Color;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform sampler2D _Emission;
		uniform float4 _Emission_ST;
		uniform float4 _EmissionColor;
		uniform float _Metallic;
		uniform sampler2D _MetallicSmoothness;
		uniform float4 _MetallicSmoothness_ST;
		uniform float _SurfaceSmoothness;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float3 Normal75 = UnpackScaleNormal( tex2D( _Normal, uv_Normal ), _NormalScale );
			o.Normal = Normal75;
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 tex2DNode2 = tex2D( _Albedo, uv_Albedo );
			float4 Albedo19 = ( _Color * tex2DNode2 );
			o.Albedo = Albedo19.rgb;
			float2 uv_Emission = i.uv_texcoord * _Emission_ST.xy + _Emission_ST.zw;
			float4 Emission259 = ( tex2D( _Emission, uv_Emission ) * _EmissionColor );
			o.Emission = Emission259.rgb;
			float2 uv_MetallicSmoothness = i.uv_texcoord * _MetallicSmoothness_ST.xy + _MetallicSmoothness_ST.zw;
			float4 tex2DNode239 = tex2D( _MetallicSmoothness, uv_MetallicSmoothness );
			float Metallic262 = ( _Metallic * tex2DNode239.r );
			o.Metallic = Metallic262;
			float AlbedoSmoothness267 = tex2DNode2.a;
			#if defined(_SMOOTHNESSSOURCE_METALLIC_ALPHA)
				float staticSwitch266 = tex2DNode239.a;
			#elif defined(_SMOOTHNESSSOURCE_ALBEDO_ALPHA)
				float staticSwitch266 = AlbedoSmoothness267;
			#else
				float staticSwitch266 = tex2DNode239.a;
			#endif
			float Smoothness263 = ( staticSwitch266 * _SurfaceSmoothness );
			o.Smoothness = Smoothness263;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=19108
Node;AmplifyShaderEditor.CommentaryNode;41;-3931.34,-522.0928;Inherit;False;882.8816;528.3103;;5;19;3;1;267;2;Albedo;0.5180138,0.6980392,0.4901961,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;79;-3933.684,83.87437;Inherit;False;1233.12;563.5208;;9;263;262;240;241;54;242;266;239;268;Metallic/Smoothness;0.8,0.7843137,0.5607843,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;260;-2656.033,90.67461;Inherit;False;928.7275;551.0918;;4;243;259;244;245;Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;267;-3527.626,-158.0849;Inherit;False;AlbedoSmoothness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;80;-2982.077,-334.7795;Inherit;False;947.0427;336.418;;3;75;175;6;Normal;0.6251274,0.49,0.7,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;268;-3783.368,479.2715;Inherit;False;267;AlbedoSmoothness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1;-3770.7,-437.7401;Inherit;False;Property;_Color;Color;4;0;Create;False;0;0;0;False;2;Header(Settings);Space(5);False;1,1,1,0;0.65,0.5223636,0.4549999,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;266;-3424.959,369.4602;Inherit;False;Property;_SmoothnessSource;Smoothness Source;9;0;Create;True;0;0;0;True;0;False;1;0;0;True;;KeywordEnum;2;Metallic_Alpha;Albedo_Alpha;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;242;-3410.059,222.8964;Inherit;False;Property;_Metallic;Metallic;7;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;54;-3408.912,498.6462;Inherit;False;Property;_SurfaceSmoothness;Smoothness;8;0;Create;False;0;0;0;False;0;False;0;0.35;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;241;-3075.658,277.2964;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-3480.47,-355.6588;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;240;-3075.657,413.2965;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;262;-2907.48,272.5011;Inherit;False;Metallic;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;19;-3293.519,-360.0127;Inherit;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;263;-2907.48,408.5011;Inherit;False;Smoothness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;175;-2893.299,-149.9733;Inherit;False;Property;_NormalScale;Normal;6;0;Create;False;0;1;Option1;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;75;-2342.497,-243.0138;Inherit;False;Normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;2;-3854.088,-253.4901;Inherit;True;Property;_Albedo;Albedo;0;0;Create;True;0;0;0;False;3;Header(Maps);Space(10);MainTexture;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;239;-3855.599,272.9095;Inherit;True;Property;_MetallicSmoothness;Metallic/Smoothness;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;6;-2677.043,-242.7635;Inherit;True;Property;_Normal;Normal;1;1;[Normal];Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;245;-2452.454,425.9594;Inherit;False;Property;_EmissionColor;Emission;5;1;[HDR];Create;False;0;0;0;False;0;False;0,0,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;244;-2171.552,319.2592;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;259;-1991.764,315.6757;Inherit;False;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;243;-2544.036,200.9282;Inherit;True;Property;_Emission;Emission;3;1;[HDR];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;261;-1406.79,49.23279;Inherit;False;259;Emission;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;264;-1409.783,139.4218;Inherit;False;262;Metallic;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;265;-1423.783,230.4219;Inherit;False;263;Smoothness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;20;-1408.011,-156.6996;Inherit;False;19;Albedo;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;77;-1406.473,-56.72939;Inherit;False;75;Normal;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-1127.36,-21.22983;Float;False;True;-1;2;;0;0;Standard;Raygeas/Suntail Surface;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;5;False;;10;False;;0;5;False;;10;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;True;;1;Pragma;multi_compile __ LOD_FADE_CROSSFADE;False;;Custom;False;0;0;;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;267;0;2;4
WireConnection;266;1;239;4
WireConnection;266;0;268;0
WireConnection;241;0;242;0
WireConnection;241;1;239;1
WireConnection;3;0;1;0
WireConnection;3;1;2;0
WireConnection;240;0;266;0
WireConnection;240;1;54;0
WireConnection;262;0;241;0
WireConnection;19;0;3;0
WireConnection;263;0;240;0
WireConnection;75;0;6;0
WireConnection;6;5;175;0
WireConnection;244;0;243;0
WireConnection;244;1;245;0
WireConnection;259;0;244;0
WireConnection;0;0;20;0
WireConnection;0;1;77;0
WireConnection;0;2;261;0
WireConnection;0;3;264;0
WireConnection;0;4;265;0
ASEEND*/
//CHKSM=FA806A83378469DDCD141BC80AFF7B7E270AF769