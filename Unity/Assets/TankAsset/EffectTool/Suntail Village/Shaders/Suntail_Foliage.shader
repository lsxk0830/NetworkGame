// Made with Amplify Shader Editor v1.9.1.8
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Raygeas/Suntail Foliage"
{
	Properties
	{
		[Header(Maps)][Space(10)][MainTexture]_Albedo("Albedo", 2D) = "white" {}
		_SmoothnessTexture("Smoothness", 2D) = "white" {}
		[Header(Settings)][Space(5)]_MainColor("Main Color", Color) = (1,1,1,0)
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_AlphaCutoff("Alpha Cutoff", Range( 0 , 1)) = 0.35
		[Header(Second Color Settings)][Space(5)][Toggle(_COLOR2ENABLE_ON)] _Color2Enable("Enable", Float) = 0
		_SecondColor("Second Color", Color) = (0,0,0,0)
		[KeywordEnum(World_Position,UV_Based)] _SecondColorOverlayType("Overlay Type", Float) = 0
		_SecondColorOffset("Offset", Float) = 0
		_SecondColorFade("Fade", Range( -1 , 1)) = 0.5
		_WorldScale("World Scale", Float) = 1
		[Header(Wind Settings)][Space(5)][Toggle(_ENABLEWIND_ON)] _EnableWind("Enable", Float) = 1
		_WindForce("Force", Range( 0 , 1)) = 0.3
		_WindWavesScale("Waves Scale", Range( 0 , 1)) = 0.25
		_WindSpeed("Speed", Range( 0 , 1)) = 0.5
		[Toggle(_ANCHORTHEFOLIAGEBASE_ON)] _Anchorthefoliagebase("Anchor the foliage base", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _ENABLEWIND_ON
		#pragma shader_feature_local _ANCHORTHEFOLIAGEBASE_ON
		#pragma shader_feature_local _COLOR2ENABLE_ON
		#pragma shader_feature_local _SECONDCOLOROVERLAYTYPE_WORLD_POSITION _SECONDCOLOROVERLAYTYPE_UV_BASED
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows nolightmap  nodynlightmap nodirlightmap dithercrossfade vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
		};

		uniform float _WindSpeed;
		uniform float _WindWavesScale;
		uniform float _WindForce;
		uniform float4 _MainColor;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _SecondColor;
		uniform float _WorldScale;
		uniform float _SecondColorOffset;
		uniform float _SecondColorFade;
		uniform sampler2D _SmoothnessTexture;
		uniform float4 _SmoothnessTexture_ST;
		uniform float _Smoothness;
		uniform float _AlphaCutoff;


		float3 mod3D289( float3 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 mod3D289( float4 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 permute( float4 x ) { return mod3D289( ( x * 34.0 + 1.0 ) * x ); }

		float4 taylorInvSqrt( float4 r ) { return 1.79284291400159 - r * 0.85373472095314; }

		float snoise( float3 v )
		{
			const float2 C = float2( 1.0 / 6.0, 1.0 / 3.0 );
			float3 i = floor( v + dot( v, C.yyy ) );
			float3 x0 = v - i + dot( i, C.xxx );
			float3 g = step( x0.yzx, x0.xyz );
			float3 l = 1.0 - g;
			float3 i1 = min( g.xyz, l.zxy );
			float3 i2 = max( g.xyz, l.zxy );
			float3 x1 = x0 - i1 + C.xxx;
			float3 x2 = x0 - i2 + C.yyy;
			float3 x3 = x0 - 0.5;
			i = mod3D289( i);
			float4 p = permute( permute( permute( i.z + float4( 0.0, i1.z, i2.z, 1.0 ) ) + i.y + float4( 0.0, i1.y, i2.y, 1.0 ) ) + i.x + float4( 0.0, i1.x, i2.x, 1.0 ) );
			float4 j = p - 49.0 * floor( p / 49.0 );  // mod(p,7*7)
			float4 x_ = floor( j / 7.0 );
			float4 y_ = floor( j - 7.0 * x_ );  // mod(j,N)
			float4 x = ( x_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 y = ( y_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 h = 1.0 - abs( x ) - abs( y );
			float4 b0 = float4( x.xy, y.xy );
			float4 b1 = float4( x.zw, y.zw );
			float4 s0 = floor( b0 ) * 2.0 + 1.0;
			float4 s1 = floor( b1 ) * 2.0 + 1.0;
			float4 sh = -step( h, 0.0 );
			float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
			float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
			float3 g0 = float3( a0.xy, h.x );
			float3 g1 = float3( a0.zw, h.y );
			float3 g2 = float3( a1.xy, h.z );
			float3 g3 = float3( a1.zw, h.w );
			float4 norm = taylorInvSqrt( float4( dot( g0, g0 ), dot( g1, g1 ), dot( g2, g2 ), dot( g3, g3 ) ) );
			g0 *= norm.x;
			g1 *= norm.y;
			g2 *= norm.z;
			g3 *= norm.w;
			float4 m = max( 0.6 - float4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0.0 );
			m = m* m;
			m = m* m;
			float4 px = float4( dot( x0, g0 ), dot( x1, g1 ), dot( x2, g2 ), dot( x3, g3 ) );
			return 42.0 * dot( m, px);
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float mulTime34 = _Time.y * ( _WindSpeed * 5 );
			float simplePerlin3D35 = snoise( ( ase_worldPos + mulTime34 )*_WindWavesScale );
			float temp_output_231_0 = ( simplePerlin3D35 * 0.01 );
			#ifdef _ANCHORTHEFOLIAGEBASE_ON
				float staticSwitch376 = ( temp_output_231_0 * pow( v.texcoord.xy.y , 2.0 ) );
			#else
				float staticSwitch376 = temp_output_231_0;
			#endif
			#ifdef _ENABLEWIND_ON
				float staticSwitch341 = ( staticSwitch376 * ( _WindForce * 30 ) );
			#else
				float staticSwitch341 = 0.0;
			#endif
			float Wind191 = staticSwitch341;
			float3 temp_cast_0 = (Wind191).xxx;
			v.vertex.xyz += temp_cast_0;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 tex2DNode1 = tex2D( _Albedo, uv_Albedo );
			float4 temp_output_10_0 = ( _MainColor * tex2DNode1 );
			float3 ase_worldPos = i.worldPos;
			float simplePerlin3D742 = snoise( ase_worldPos*_WorldScale );
			simplePerlin3D742 = simplePerlin3D742*0.5 + 0.5;
			#if defined(_SECONDCOLOROVERLAYTYPE_WORLD_POSITION)
				float staticSwitch360 = simplePerlin3D742;
			#elif defined(_SECONDCOLOROVERLAYTYPE_UV_BASED)
				float staticSwitch360 = i.uv_texcoord.y;
			#else
				float staticSwitch360 = simplePerlin3D742;
			#endif
			float SecondColorMask335 = saturate( ( ( staticSwitch360 + _SecondColorOffset ) * ( _SecondColorFade * 2 ) ) );
			float4 lerpResult332 = lerp( temp_output_10_0 , ( _SecondColor * tex2D( _Albedo, uv_Albedo ) ) , SecondColorMask335);
			#ifdef _COLOR2ENABLE_ON
				float4 staticSwitch340 = lerpResult332;
			#else
				float4 staticSwitch340 = temp_output_10_0;
			#endif
			float4 Albedo259 = staticSwitch340;
			o.Albedo = Albedo259.rgb;
			float2 uv_SmoothnessTexture = i.uv_texcoord * _SmoothnessTexture_ST.xy + _SmoothnessTexture_ST.zw;
			float4 Smoothness734 = saturate( ( tex2D( _SmoothnessTexture, uv_SmoothnessTexture ) * _Smoothness ) );
			o.Smoothness = Smoothness734.r;
			o.Alpha = 1;
			float OpacityMask263 = tex2DNode1.a;
			clip( OpacityMask263 - _AlphaCutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=19108
Node;AmplifyShaderEditor.CommentaryNode;765;-2709.372,-492.8225;Inherit;False;1077.163;488.021;;5;708;734;706;709;681;Smoothness;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;336;-3286.498,-1478.755;Inherit;False;1966.236;874.6056;;12;335;334;382;377;391;248;310;360;742;361;745;743;Second Color Mask;0.7,0.7,0.7,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;262;-5464.229,-1485;Inherit;False;2097.364;882.0443;;13;156;263;259;340;332;367;10;337;1;3;366;247;368;Albedo;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;366;-4899.025,-839.039;Inherit;True;Property;_TextureSample0;Texture Sample 0;18;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;247;-4815.405,-1025.55;Inherit;False;Property;_SecondColor;Second Color;6;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.02352945,0.2,0.02000001,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-4899.858,-1229.814;Inherit;True;Property;_LeavesTexture;Leaves Texture;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;3;-4818.124,-1419.369;Inherit;False;Property;_MainColor;Main Color;2;0;Create;True;0;0;0;False;2;Header(Settings);Space(5);False;1,1,1,0;0.1992481,0.5,0.4172932,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;66;-5464.272,-503.3702;Inherit;False;2680.3;740.17;;18;191;341;188;376;345;56;359;356;231;35;358;357;190;182;228;34;344;36;Wind;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-4495.411,-1330.155;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;367;-4498.595,-936.7111;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;337;-4516.115,-1106.451;Inherit;False;335;SecondColorMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;36;-5429.632,-246.7836;Inherit;False;Property;_WindSpeed;Speed;14;0;Create;False;0;0;0;False;0;False;0.5;0.2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;332;-4228.949,-1149.796;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScaleNode;344;-5132.634,-241.392;Inherit;False;5;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;340;-3897.969,-1336.19;Inherit;False;Property;_Color2Enable;Enable;5;0;Create;False;0;0;0;False;2;Header(Second Color Settings);Space(5);False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;259;-3610.187,-1336.708;Inherit;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleTimeNode;34;-4962.952,-241.6264;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;228;-4962.759,-409.6157;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;190;-4866.769,-126.1545;Inherit;False;Property;_WindWavesScale;Waves Scale;13;0;Create;False;0;0;0;False;0;False;0.25;0.411;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;182;-4716.685,-329.3925;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;357;-4481.184,2.437496;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NoiseGeneratorNode;35;-4503.841,-238.515;Inherit;True;Simplex3D;False;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0.4;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;358;-4402.588,132.5774;Inherit;False;Constant;_Float0;Float 0;15;0;Create;True;0;0;0;False;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;356;-4195.656,22.40697;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleNode;231;-4185.764,-234.2584;Inherit;False;0.01;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;56;-3949.659,38.50756;Inherit;False;Property;_WindForce;Force;12;0;Create;False;0;0;0;False;0;False;0.3;0.48;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;359;-3986.644,-116.5151;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;376;-3793.085,-237.1686;Inherit;False;Property;_Anchorthefoliagebase;Anchor the foliage base;15;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleNode;345;-3662.742,44.30282;Inherit;False;30;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;188;-3461.298,-114.811;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;263;-4526.999,-1188.709;Inherit;False;OpacityMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;341;-3278.123,-144.9097;Inherit;False;Property;_EnableWind;Enable;11;0;Create;False;0;0;0;False;2;Header(Wind Settings);Space(5);False;0;1;1;True;;Toggle;2;;;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;191;-3012.979,-143.4771;Inherit;False;Wind;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;156;-4591.908,-725.2592;Inherit;False;Property;_AlphaCutoff;Alpha Cutoff;4;0;Create;False;0;0;0;False;0;False;0.35;0.45;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;681;-2611.515,-164.4034;Inherit;False;Property;_Smoothness;Smoothness;3;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;709;-2261.066,-266.8371;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;706;-2073.048,-265.5156;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;734;-1876.22,-269.7158;Inherit;False;Smoothness;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;743;-3164.106,-1178.654;Inherit;False;Property;_WorldScale;World Scale;10;0;Create;True;0;0;0;False;0;False;1;3.44;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;745;-3179.796,-1356.744;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TextureCoordinatesNode;361;-2905.37,-1026.937;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NoiseGeneratorNode;742;-2934.464,-1279.077;Inherit;True;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;360;-2619.01,-1137.648;Inherit;False;Property;_SecondColorOverlayType;Overlay Type;7;0;Create;False;0;0;0;False;0;False;0;0;0;True;;KeywordEnum;2;World_Position;UV_Based;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;310;-2473.24,-973.8425;Inherit;False;Property;_SecondColorOffset;Offset;8;0;Create;False;0;0;0;False;0;False;0;-0.24;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;248;-2438.762,-830.45;Inherit;False;Property;_SecondColorFade;Fade;9;0;Create;False;0;0;0;False;0;False;0.5;0.82;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleNode;391;-2137.919,-825.9792;Inherit;False;2;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;377;-2203.554,-1054.288;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;382;-1939.294,-945.575;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;334;-1774.596,-945.0285;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;335;-1598.065,-949.3939;Inherit;False;SecondColorMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;267;-1012.164,-1078.396;Inherit;False;263;OpacityMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;236;-993.7571,-978.7477;Inherit;False;191;Wind;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;766;-1012.788,-1179.792;Inherit;False;734;Smoothness;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;764;-999.9052,-1270.015;Inherit;False;259;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;151;-754.8312,-1295.654;Float;False;True;-1;2;;0;0;Standard;Raygeas/Suntail Foliage;False;False;False;False;False;False;True;True;True;False;False;False;True;False;False;False;False;False;False;False;False;Off;0;False;;0;False;;False;3;False;;1;False;;False;0;Masked;0.5;True;True;0;False;TransparentCutout;;AlphaTest;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;5;False;;10;False;;0;1;False;;1;False;;1;False;;1;False;;0;False;0.01;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;True;_AlphaCutoff;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.TexturePropertyNode;368;-5210.191,-1022.857;Inherit;True;Property;_Albedo;Albedo;0;0;Create;True;0;0;0;False;3;Header(Maps);Space(10);MainTexture;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SamplerNode;708;-2632.067,-392.8372;Inherit;True;Property;_SmoothnessTexture;Smoothness;1;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
WireConnection;366;0;368;0
WireConnection;1;0;368;0
WireConnection;10;0;3;0
WireConnection;10;1;1;0
WireConnection;367;0;247;0
WireConnection;367;1;366;0
WireConnection;332;0;10;0
WireConnection;332;1;367;0
WireConnection;332;2;337;0
WireConnection;344;0;36;0
WireConnection;340;1;10;0
WireConnection;340;0;332;0
WireConnection;259;0;340;0
WireConnection;34;0;344;0
WireConnection;182;0;228;0
WireConnection;182;1;34;0
WireConnection;35;0;182;0
WireConnection;35;1;190;0
WireConnection;356;0;357;2
WireConnection;356;1;358;0
WireConnection;231;0;35;0
WireConnection;359;0;231;0
WireConnection;359;1;356;0
WireConnection;376;1;231;0
WireConnection;376;0;359;0
WireConnection;345;0;56;0
WireConnection;188;0;376;0
WireConnection;188;1;345;0
WireConnection;263;0;1;4
WireConnection;341;0;188;0
WireConnection;191;0;341;0
WireConnection;709;0;708;0
WireConnection;709;1;681;0
WireConnection;706;0;709;0
WireConnection;734;0;706;0
WireConnection;742;0;745;0
WireConnection;742;1;743;0
WireConnection;360;1;742;0
WireConnection;360;0;361;2
WireConnection;391;0;248;0
WireConnection;377;0;360;0
WireConnection;377;1;310;0
WireConnection;382;0;377;0
WireConnection;382;1;391;0
WireConnection;334;0;382;0
WireConnection;335;0;334;0
WireConnection;151;0;764;0
WireConnection;151;4;766;0
WireConnection;151;10;267;0
WireConnection;151;11;236;0
ASEEND*/
//CHKSM=201BA89D6CF852F261B974117A8A48C571CE0B87