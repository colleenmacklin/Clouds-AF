Shader "NatureManufacture Shaders/Road/Standard_Metallic_Road_Transparent"
{
	Properties
	{
		_AlphaNoiseTilling("Alpha Noise Tilling", Vector) = (0,0,0,0)
		_MainRoadColor("Main Road Color", Color) = (1,1,1,1)
		_SecondRoadColor("Second Road Color", Color) = (1,1,1,1)
		[Toggle(_INVERTVCOLORMASKSECONDROAD_ON)] _InvertVColorMaskSecondRoad("Invert VColor Mask Second Road", Float) = 0
		_MainRoadBrightness("Main Road Brightness", Float) = 1
		_SecondRoadBrightness("Second Road Brightness", Float) = 1
		_MainTex("Main Road Albedo_T", 2D) = "white" {}
		_TextureSample1("Second Road Albedo_T", 2D) = "white" {}
		[Toggle(_USESECONDROADALPHA_ON)] _UseSecondRoadAlpha("Use Second Road Alpha", Float) = 1
		_AlphaNoisePower("Alpha Noise Power", Range( 0 , 5)) = 0
		_BumpMap("Main Road Normal", 2D) = "bump" {}
		_BumpScale("Main Road BumpScale", Range( 0 , 5)) = 0
		_TextureSample2("Second Road Normal", 2D) = "bump" {}
		_Float0("Second Road BumpScale", Range( 0 , 5)) = 0
		_MetalicRAmbientOcclusionGHeightBEmissionA("Main Road Metallic (R) Ambient Occlusion (G) Height (B) Smoothness (A)", 2D) = "white" {}
		_TextureSample3("Second Road Metallic (R) Ambient Occlusion (G) Height (B) Smoothness (A)", 2D) = "white" {}
		_MainRoadMetalicPower("Main Road Metalic Power", Range( 0 , 2)) = 0
		_SecondRoadMetalicPower("Second Road Metalic Power", Range( 0 , 2)) = 0
		_MainRoadAmbientOcclusionPower("Main Road Ambient Occlusion Power", Range( 0 , 1)) = 1
		_SecondRoadAmbientOcclusionPower("Second Road Ambient Occlusion Power", Range( 0 , 1)) = 1
		_MainRoadSmoothnessPower("Main Road Smoothness Power", Range( 0 , 2)) = 1
		_SecondRoadSmoothnessPower("Second Road Smoothness Power", Range( 0 , 2)) = 1
		_MainRoadParallaxPower("Main Road Parallax Power", Range( -0.1 , 0.1)) = 0
		_SecondRoadParallaxPower("Second Road Parallax Power", Range( -0.1 , 0.1)) = 0
		_DetailMask("DetailMask (A)", 2D) = "white" {}
		_DetailMapAlbedoRNyGNxA("Detail Map Albedo(R) Ny(G) Nx(A)", 2D) = "white" {}
		_DetailAlbedoPower("Main Road Detail Albedo Power", Range( 0 , 2)) = 0
		_Float1("Second Road Detail Albedo Power", Range( 0 , 2)) = 0
		_DetailNormalMapScale("Main Road DetailNormalMapScale", Range( 0 , 5)) = 0
		_Float2("Second Road Detail Albedo Power", Range( 0 , 2)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
		[Header(Forward Rendering Options)]
		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[ToggleOff] _GlossyReflections("Reflections", Float) = 1.0
	}

	SubShader
	{
		Tags{ "RenderType" = "Custom" "Queue" = "AlphaTest+0" "Offset"="-2, -2" "ForceNoShadowCasting"="True" }
		Cull Back
		Offset  -2 ,-2
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma shader_feature _SPECULARHIGHLIGHTS_OFF
		#pragma shader_feature _GLOSSYREFLECTIONS_OFF
		#pragma shader_feature _INVERTVCOLORMASKSECONDROAD_ON
		#pragma multi_compile __ _USESECONDROADALPHA_ON
		#pragma surface surf Standard keepalpha decal:blend
		struct Input
		{
			float2 uv_texcoord;
			float3 viewDir;
			INTERNAL_DATA
			float4 vertexColor : COLOR;
		};

		uniform half _BumpScale;
		uniform sampler2D _BumpMap;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform sampler2D _MetalicRAmbientOcclusionGHeightBEmissionA;
		uniform float4 _MetalicRAmbientOcclusionGHeightBEmissionA_ST;
		uniform float _MainRoadParallaxPower;
		uniform sampler2D _DetailMapAlbedoRNyGNxA;
		uniform float4 _DetailMapAlbedoRNyGNxA_ST;
		uniform float _DetailNormalMapScale;
		uniform float _DetailAlbedoPower;
		uniform sampler2D _DetailMask;
		uniform float4 _DetailMask_ST;
		uniform half _Float0;
		uniform sampler2D _TextureSample2;
		uniform sampler2D _TextureSample1;
		uniform float4 _TextureSample1_ST;
		uniform sampler2D _TextureSample3;
		uniform float4 _TextureSample3_ST;
		uniform float _SecondRoadParallaxPower;
		uniform float _Float2;
		uniform float _MainRoadBrightness;
		uniform float4 _MainRoadColor;
		uniform float _SecondRoadBrightness;
		uniform float4 _SecondRoadColor;
		uniform float _Float1;
		uniform float _MainRoadMetalicPower;
		uniform float _SecondRoadMetalicPower;
		uniform float _MainRoadSmoothnessPower;
		uniform float _SecondRoadSmoothnessPower;
		uniform float _MainRoadAmbientOcclusionPower;
		uniform float _SecondRoadAmbientOcclusionPower;
		uniform float2 _AlphaNoiseTilling;
		uniform float _AlphaNoisePower;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv0_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float2 uv_MetalicRAmbientOcclusionGHeightBEmissionA = i.uv_texcoord * _MetalicRAmbientOcclusionGHeightBEmissionA_ST.xy + _MetalicRAmbientOcclusionGHeightBEmissionA_ST.zw;
			float2 Offset710 = ( ( tex2D( _MetalicRAmbientOcclusionGHeightBEmissionA, uv_MetalicRAmbientOcclusionGHeightBEmissionA ).b - 1 ) * i.viewDir.xy * _MainRoadParallaxPower ) + uv0_MainTex;
			float2 Offset728 = ( ( tex2D( _MetalicRAmbientOcclusionGHeightBEmissionA, Offset710 ).b - 1 ) * i.viewDir.xy * _MainRoadParallaxPower ) + Offset710;
			float2 Offset754 = ( ( tex2D( _MetalicRAmbientOcclusionGHeightBEmissionA, Offset728 ).b - 1 ) * i.viewDir.xy * _MainRoadParallaxPower ) + Offset728;
			float2 Offset778 = ( ( tex2D( _MetalicRAmbientOcclusionGHeightBEmissionA, Offset754 ).b - 1 ) * i.viewDir.xy * _MainRoadParallaxPower ) + Offset754;
			float3 tex2DNode4 = UnpackScaleNormal( tex2D( _BumpMap, Offset778 ), _BumpScale );
			float2 uv0_DetailMapAlbedoRNyGNxA = i.uv_texcoord * _DetailMapAlbedoRNyGNxA_ST.xy + _DetailMapAlbedoRNyGNxA_ST.zw;
			float4 tex2DNode486 = tex2D( _DetailMapAlbedoRNyGNxA, uv0_DetailMapAlbedoRNyGNxA );
			float2 appendResult11_g1 = (float2(tex2DNode486.a , tex2DNode486.g));
			float2 temp_output_4_0_g1 = ( ( ( appendResult11_g1 * float2( 2,2 ) ) + float2( -1,-1 ) ) * _DetailNormalMapScale );
			float2 break8_g1 = temp_output_4_0_g1;
			float dotResult5_g1 = dot( temp_output_4_0_g1 , temp_output_4_0_g1 );
			float temp_output_9_0_g1 = sqrt( ( 1.0 - saturate( dotResult5_g1 ) ) );
			float3 appendResult10_g1 = (float3(break8_g1.x , break8_g1.y , temp_output_9_0_g1));
			float3 temp_output_798_0 = appendResult10_g1;
			float2 uv0_DetailMask = i.uv_texcoord * _DetailMask_ST.xy + _DetailMask_ST.zw;
			float4 tex2DNode481 = tex2D( _DetailMask, uv0_DetailMask );
			float temp_output_484_0 = ( _DetailAlbedoPower * tex2DNode481.a );
			float3 lerpResult479 = lerp( tex2DNode4 , BlendNormals( tex2DNode4 , temp_output_798_0 ) , temp_output_484_0);
			float2 uv0_TextureSample1 = i.uv_texcoord * _TextureSample1_ST.xy + _TextureSample1_ST.zw;
			float2 uv_TextureSample3 = i.uv_texcoord * _TextureSample3_ST.xy + _TextureSample3_ST.zw;
			float2 Offset813 = ( ( tex2D( _TextureSample3, uv_TextureSample3 ).b - 1 ) * i.viewDir.xy * _SecondRoadParallaxPower ) + uv0_TextureSample1;
			float2 Offset823 = ( ( tex2D( _TextureSample3, Offset813 ).b - 1 ) * i.viewDir.xy * _SecondRoadParallaxPower ) + Offset813;
			float2 Offset835 = ( ( tex2D( _TextureSample3, Offset823 ).b - 1 ) * i.viewDir.xy * _SecondRoadParallaxPower ) + Offset823;
			float2 Offset847 = ( ( tex2D( _TextureSample3, Offset835 ).b - 1 ) * i.viewDir.xy * _SecondRoadParallaxPower ) + Offset835;
			float3 tex2DNode801 = UnpackScaleNormal( tex2D( _TextureSample2, Offset847 ), _Float0 );
			float3 lerpResult862 = lerp( tex2DNode801 , BlendNormals( tex2DNode801 , temp_output_798_0 ) , ( _Float2 * tex2DNode481.a ));
			#ifdef _INVERTVCOLORMASKSECONDROAD_ON
				float staticSwitch882 = ( 1.0 - i.vertexColor.r );
			#else
				float staticSwitch882 = i.vertexColor.r;
			#endif
			float3 lerpResult861 = lerp( lerpResult479 , lerpResult862 , staticSwitch882);
			o.Normal = lerpResult861;
			float4 tex2DNode1 = tex2D( _MainTex, Offset778 );
			float4 temp_output_77_0 = ( ( _MainRoadBrightness * tex2DNode1 ) * _MainRoadColor );
			float4 temp_cast_0 = (( _DetailAlbedoPower * tex2DNode486.r )).xxxx;
			float4 blendOpSrc474 = temp_output_77_0;
			float4 blendOpDest474 = temp_cast_0;
			float4 lerpResult480 = lerp( temp_output_77_0 , (( blendOpDest474 > 0.5 ) ? ( 1.0 - ( 1.0 - 2.0 * ( blendOpDest474 - 0.5 ) ) * ( 1.0 - blendOpSrc474 ) ) : ( 2.0 * blendOpDest474 * blendOpSrc474 ) ) , temp_output_484_0);
			float4 tex2DNode800 = tex2D( _TextureSample1, Offset847 );
			float4 temp_output_851_0 = ( ( _SecondRoadBrightness * tex2DNode800 ) * _SecondRoadColor );
			float4 temp_cast_1 = (( tex2DNode486.r * _DetailAlbedoPower )).xxxx;
			float4 blendOpSrc854 = temp_output_851_0;
			float4 blendOpDest854 = temp_cast_1;
			float4 lerpResult855 = lerp( temp_output_851_0 , (( blendOpDest854 > 0.5 ) ? ( 1.0 - ( 1.0 - 2.0 * ( blendOpDest854 - 0.5 ) ) * ( 1.0 - blendOpSrc854 ) ) : ( 2.0 * blendOpDest854 * blendOpSrc854 ) ) , ( _Float1 * tex2DNode481.a ));
			float4 lerpResult860 = lerp( lerpResult480 , lerpResult855 , staticSwitch882);
			o.Albedo = lerpResult860.rgb;
			float4 tex2DNode2 = tex2D( _MetalicRAmbientOcclusionGHeightBEmissionA, Offset778 );
			float4 tex2DNode802 = tex2D( _TextureSample3, Offset847 );
			float lerpResult874 = lerp( ( tex2DNode2.r * _MainRoadMetalicPower ) , ( tex2DNode802.r * _SecondRoadMetalicPower ) , staticSwitch882);
			o.Metallic = lerpResult874;
			float lerpResult871 = lerp( ( tex2DNode2.a * _MainRoadSmoothnessPower ) , ( tex2DNode802.a * _SecondRoadSmoothnessPower ) , staticSwitch882);
			o.Smoothness = lerpResult871;
			float clampResult96 = clamp( tex2DNode2.g , ( 1.0 - _MainRoadAmbientOcclusionPower ) , 1.0 );
			float clampResult868 = clamp( tex2DNode802.g , ( 1.0 - _SecondRoadAmbientOcclusionPower ) , 1.0 );
			float lerpResult873 = lerp( clampResult96 , clampResult868 , staticSwitch882);
			o.Occlusion = lerpResult873;
			float2 uv_TexCoord793 = i.uv_texcoord * _AlphaNoiseTilling;
			float simplePerlin2D779 = snoise( uv_TexCoord793 );
			float temp_output_791_0 = ( simplePerlin2D779 * _AlphaNoisePower );
			float temp_output_629_0 = ( tex2DNode1.a * _MainRoadColor.a );
			float clampResult788 = clamp( ( ( temp_output_791_0 * temp_output_629_0 ) + temp_output_629_0 ) , 0.0 , 1.0 );
			float temp_output_850_0 = ( tex2DNode800.a * _SecondRoadColor.a );
			float clampResult879 = clamp( ( ( temp_output_791_0 * temp_output_850_0 ) + temp_output_850_0 ) , 0.0 , 1.0 );
			#ifdef _USESECONDROADALPHA_ON
				float staticSwitch881 = clampResult879;
			#else
				float staticSwitch881 = clampResult788;
			#endif
			float lerpResult880 = lerp( clampResult788 , staticSwitch881 , staticSwitch882);
			o.Alpha = ( lerpResult880 * i.vertexColor.a );
		}

		ENDCG
	}
	Fallback "Diffuse"

}