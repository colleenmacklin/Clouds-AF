Shader "NatureManufacture Shaders/Standard Shaders/Standard Specular Snow"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_BumpMap("BumpMap", 2D) = "bump" {}
		_BumpScale("BumpScale", Range( 0 , 2)) = 1
		_SpecularRGBSmoothnesA("Specular (RGB) Smoothnes (A)", 2D) = "white" {}
		_SpecularPower("Specular Power", Range( 0 , 2)) = 1
		_SmoothnessPower("Smoothness Power", Range( 0 , 2)) = 1
		_AmbientOcclusionG("Ambient Occlusion (G)", 2D) = "white" {}
		_AmbientOcclusionPower("Ambient Occlusion Power", Range( 0 , 1)) = 1
		_DetailMask("DetailMask", 2D) = "white" {}
		_DetailAlbedoPower("Detail Albedo Power", Range( 0 , 2)) = 0
		_DetailMapAlbedoRNyGNxA("Detail Map Albedo(R) Ny(G) Nx(A)", 2D) = "black" {}
		_DetailNormalMapScale("DetailNormalMapScale", Range( 0 , 5)) = 0
		[Toggle(_USESNOW_ON)] _UseSnow("Use Snow", Float) = 1
		[Toggle(_USEDYNAMICSNOWTSTATICMASKF_ON)] _UseDynamicSnowTStaticMaskF("Use Dynamic Snow (T) Static Mask (F)", Float) = 1
		_SnowMaskB("Snow Mask (B)", 2D) = "white" {}
		_SnowMaskPower("Snow Mask Power", Range( 0 , 10)) = 1
		_Snow_Amount("Snow_Amount", Range( 0 , 2)) = 0.13
		_Snow_AmountGrowSpeed("Snow_Amount Grow Speed", Range( 1 , 3)) = 2
		_TriplanarCoverFalloff("Triplanar Cover Falloff", Range( 1 , 10)) = 8
		_SnowAlbedoRGB("Snow Albedo (RGB)", 2D) = "white" {}
		_SnowTiling("Snow Tiling", Range( 0.0001 , 100)) = 15
		_SnowAlbedoColor("Snow Albedo Color", Color) = (1,1,1,1)
		_SnowNormalRGB("Snow Normal (RGB)", 2D) = "white" {}
		_SnowNormalScale("Snow Normal Scale", Range( 0 , 2)) = 1
		_SnowNormalCoverHardness("Snow Normal Cover Hardness", Range( 0 , 10)) = 1
		_Snow_SpecularRGBSmoothnessA("Snow_Specular (RGB) Smoothness (A)", 2D) = "white" {}
		_SnowSpecularPower("Snow Specular Power", Range( 0 , 2)) = 0
		_SnowSmoothnessPower("Snow Smoothness Power", Range( 0 , 2)) = 1
		_SnowAmbientOcclusionG("Snow Ambient Occlusion(G)", 2D) = "white" {}
		_SnowAmbientOcclusionPower("Snow Ambient Occlusion Power", Range( 0 , 1)) = 1
		_SnowMaxAngle("Snow Max Angle", Range( 0.001 , 90)) = 90
		_SnowHardness("Snow Hardness", Range( 1 , 10)) = 5
		_Snow_Min_Height("Snow_Min_Height", Range( -1000 , 10000)) = -1000
		_SnowHeightG("Snow Height (G)", 2D) = "white" {}
		_SnowHeightSharpness("Snow Height Sharpness", Range( 0 , 2)) = 0.3
		_Snow_Min_Height_Blending("Snow_Min_Height_Blending", Range( 0 , 500)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		ZTest LEqual
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature _USESNOW_ON
		#pragma shader_feature _USEDYNAMICSNOWTSTATICMASKF_ON
		#define ASE_TEXTURE_PARAMS(textureName) textureName

		#include "NM_indirect.cginc"
		#pragma multi_compile GPU_FRUSTUM_ON __
		#pragma instancing_options procedural:setup
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
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform float _BumpScale;
		uniform sampler2D _BumpMap;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform sampler2D _DetailMapAlbedoRNyGNxA;
		uniform float4 _DetailMapAlbedoRNyGNxA_ST;
		uniform float _DetailNormalMapScale;
		uniform sampler2D _DetailMask;
		uniform float4 _DetailMask_ST;
		uniform float _SnowNormalCoverHardness;
		uniform sampler2D _SnowNormalRGB;
		uniform float _SnowTiling;
		uniform float _TriplanarCoverFalloff;
		uniform float _SnowNormalScale;
		uniform float _Snow_Amount;
		uniform float _Snow_AmountGrowSpeed;
		uniform float _SnowMaxAngle;
		uniform float _SnowHardness;
		uniform float _Snow_Min_Height;
		uniform float _Snow_Min_Height_Blending;
		uniform sampler2D _SnowMaskB;
		uniform float4 _SnowMaskB_ST;
		uniform float _SnowMaskPower;
		uniform sampler2D _SnowHeightG;
		uniform float _SnowHeightSharpness;
		uniform float4 _Color;
		uniform float _DetailAlbedoPower;
		uniform sampler2D _SnowAlbedoRGB;
		uniform float4 _SnowAlbedoColor;
		uniform sampler2D _SpecularRGBSmoothnesA;
		uniform float _SpecularPower;
		uniform sampler2D _Snow_SpecularRGBSmoothnessA;
		uniform float _SnowSpecularPower;
		uniform float _SmoothnessPower;
		uniform float _SnowSmoothnessPower;
		uniform sampler2D _AmbientOcclusionG;
		uniform float _AmbientOcclusionPower;
		uniform sampler2D _SnowAmbientOcclusionG;
		uniform float _SnowAmbientOcclusionPower;


		inline float3 TriplanarSamplingSNF( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = ( tex2D( ASE_TEXTURE_PARAMS( topTexMap ), tiling * worldPos.zy * float2( nsign.x, 1.0 ) ) );
			yNorm = ( tex2D( ASE_TEXTURE_PARAMS( topTexMap ), tiling * worldPos.xz * float2( nsign.y, 1.0 ) ) );
			zNorm = ( tex2D( ASE_TEXTURE_PARAMS( topTexMap ), tiling * worldPos.xy * float2( -nsign.z, 1.0 ) ) );
			xNorm.xyz = half3( UnpackScaleNormal( xNorm, normalScale.y ).xy * float2( nsign.x, 1.0 ) + worldNormal.zy, worldNormal.x ).zyx;
			yNorm.xyz = half3( UnpackScaleNormal( yNorm, normalScale.x ).xy * float2( nsign.y, 1.0 ) + worldNormal.xz, worldNormal.y ).xzy;
			zNorm.xyz = half3( UnpackScaleNormal( zNorm, normalScale.y ).xy * float2( -nsign.z, 1.0 ) + worldNormal.xy, worldNormal.z ).xyz;
			return normalize( xNorm.xyz * projNormal.x + yNorm.xyz * projNormal.y + zNorm.xyz * projNormal.z );
		}


		inline float4 TriplanarSamplingSF( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = ( tex2D( ASE_TEXTURE_PARAMS( topTexMap ), tiling * worldPos.zy * float2( nsign.x, 1.0 ) ) );
			yNorm = ( tex2D( ASE_TEXTURE_PARAMS( topTexMap ), tiling * worldPos.xz * float2( nsign.y, 1.0 ) ) );
			zNorm = ( tex2D( ASE_TEXTURE_PARAMS( topTexMap ), tiling * worldPos.xy * float2( -nsign.z, 1.0 ) ) );
			return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
		}


		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float2 uv0_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float3 tex2DNode4 = UnpackScaleNormal( tex2D( _BumpMap, uv0_MainTex ), _BumpScale );
			float2 uv0_DetailMapAlbedoRNyGNxA = i.uv_texcoord * _DetailMapAlbedoRNyGNxA_ST.xy + _DetailMapAlbedoRNyGNxA_ST.zw;
			float4 tex2DNode205 = tex2D( _DetailMapAlbedoRNyGNxA, uv0_DetailMapAlbedoRNyGNxA );
			float2 appendResult11_g1 = (float2(tex2DNode205.a , tex2DNode205.g));
			float2 temp_output_4_0_g1 = ( ( ( appendResult11_g1 * float2( 2,2 ) ) + float2( -1,-1 ) ) * _DetailNormalMapScale );
			float2 break8_g1 = temp_output_4_0_g1;
			float dotResult5_g1 = dot( temp_output_4_0_g1 , temp_output_4_0_g1 );
			float temp_output_9_0_g1 = sqrt( ( 1.0 - saturate( dotResult5_g1 ) ) );
			float3 appendResult10_g1 = (float3(break8_g1.x , break8_g1.y , temp_output_9_0_g1));
			float3 temp_output_237_0 = appendResult10_g1;
			float3 normalizeResult202 = normalize( BlendNormals( tex2DNode4 , temp_output_237_0 ) );
			float2 uv_DetailMask = i.uv_texcoord * _DetailMask_ST.xy + _DetailMask_ST.zw;
			float4 tex2DNode195 = tex2D( _DetailMask, uv_DetailMask );
			float3 lerpResult193 = lerp( tex2DNode4 , normalizeResult202 , tex2DNode195.a);
			float3 normalizeResult201 = normalize( BlendNormals( UnpackScaleNormal( tex2D( _BumpMap, uv0_MainTex ), _SnowNormalCoverHardness ) , temp_output_237_0 ) );
			float temp_output_122_0 = ( 1.0 / _SnowTiling );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3 triplanar166 = TriplanarSamplingSNF( _SnowNormalRGB, ase_worldPos, ase_worldNormal, _TriplanarCoverFalloff, temp_output_122_0, _SnowNormalScale, 0 );
			float3 tanTriplanarNormal166 = mul( ase_worldToTangent, triplanar166 );
			float temp_output_225_0 = ( 4.0 - _Snow_AmountGrowSpeed );
			float clampResult224 = clamp( pow( ( _Snow_Amount / temp_output_225_0 ) , temp_output_225_0 ) , 0.0 , 2.0 );
			float clampResult89 = clamp( ase_worldNormal.y , 0.0 , 0.999999 );
			float temp_output_88_0 = ( _SnowMaxAngle / 45.0 );
			float clampResult98 = clamp( ( clampResult89 - ( 1.0 - temp_output_88_0 ) ) , 0.0 , 2.0 );
			float temp_output_83_0 = ( ( 1.0 - _Snow_Min_Height ) + ase_worldPos.y );
			float clampResult95 = clamp( ( temp_output_83_0 + 1.0 ) , 0.0 , 1.0 );
			float clampResult97 = clamp( ( ( 1.0 - ( ( temp_output_83_0 + _Snow_Min_Height_Blending ) / temp_output_83_0 ) ) + -0.5 ) , 0.0 , 1.0 );
			float clampResult103 = clamp( ( clampResult95 + clampResult97 ) , 0.0 , 1.0 );
			float temp_output_106_0 = ( pow( ( clampResult98 * ( 1.0 / temp_output_88_0 ) ) , _SnowHardness ) * clampResult103 );
			float3 lerpResult115 = lerp( normalizeResult201 , tanTriplanarNormal166 , ( saturate( ( ase_worldNormal.y * clampResult224 ) ) * temp_output_106_0 ));
			float2 uv0_SnowMaskB = i.uv_texcoord * _SnowMaskB_ST.xy + _SnowMaskB_ST.zw;
			float clampResult236 = clamp( ( tex2D( _SnowMaskB, uv0_SnowMaskB ).b * _SnowMaskPower ) , 0.0 , 1.0 );
			float4 triplanar175 = TriplanarSamplingSF( _SnowHeightG, ase_worldPos, ase_worldNormal, _TriplanarCoverFalloff, temp_output_122_0, 1.0, 0 );
			#ifdef _USEDYNAMICSNOWTSTATICMASKF_ON
				float staticSwitch226 = ( clampResult236 * ( saturate( ( ( ( (WorldNormalVector( i , lerpResult115 )).y * clampResult224 ) * ( ( clampResult224 * _SnowHardness ) * temp_output_106_0 ) ) * pow( triplanar175.y , _SnowHeightSharpness ) ) ) * 1.0 ) );
			#else
				float staticSwitch226 = clampResult236;
			#endif
			#ifdef _USESNOW_ON
				float staticSwitch229 = staticSwitch226;
			#else
				float staticSwitch229 = 1E-05;
			#endif
			float SnowCover231 = staticSwitch229;
			float3 lerpResult177 = lerp( lerpResult193 , lerpResult115 , SnowCover231);
			o.Normal = lerpResult177;
			float4 temp_output_44_0 = ( tex2D( _MainTex, uv0_MainTex ) * _Color );
			float4 temp_cast_0 = (( _DetailAlbedoPower * tex2DNode205.r )).xxxx;
			float4 blendOpSrc189 = temp_output_44_0;
			float4 blendOpDest189 = temp_cast_0;
			float4 lerpResult192 = lerp( temp_output_44_0 , (( blendOpDest189 > 0.5 ) ? ( 1.0 - ( 1.0 - 2.0 * ( blendOpDest189 - 0.5 ) ) * ( 1.0 - blendOpSrc189 ) ) : ( 2.0 * blendOpDest189 * blendOpSrc189 ) ) , ( _DetailAlbedoPower * tex2DNode195.a ));
			float4 triplanar162 = TriplanarSamplingSF( _SnowAlbedoRGB, ase_worldPos, ase_worldNormal, _TriplanarCoverFalloff, temp_output_122_0, 1.0, 0 );
			float4 lerpResult10 = lerp( lerpResult192 , ( triplanar162 * _SnowAlbedoColor ) , SnowCover231);
			o.Albedo = lerpResult10.xyz;
			float4 tex2DNode29 = tex2D( _SpecularRGBSmoothnesA, uv0_MainTex );
			float4 triplanar165 = TriplanarSamplingSF( _Snow_SpecularRGBSmoothnessA, ase_worldPos, ase_worldNormal, _TriplanarCoverFalloff, temp_output_122_0, 1.0, 0 );
			float4 break150 = triplanar165;
			float3 appendResult151 = (float3(break150.x , break150.y , break150.z));
			float4 lerpResult17 = lerp( ( tex2DNode29 * _SpecularPower ) , float4( ( appendResult151 * _SnowSpecularPower ) , 0.0 ) , SnowCover231);
			o.Specular = lerpResult17.rgb;
			float lerpResult28 = lerp( ( tex2DNode29.a * _SmoothnessPower ) , ( break150.w * _SnowSmoothnessPower ) , SnowCover231);
			o.Smoothness = lerpResult28;
			float clampResult67 = clamp( tex2D( _AmbientOcclusionG, uv0_MainTex ).g , ( 1.0 - _AmbientOcclusionPower ) , 1.0 );
			float4 triplanar170 = TriplanarSamplingSF( _SnowAmbientOcclusionG, ase_worldPos, ase_worldNormal, _TriplanarCoverFalloff, temp_output_122_0, 1.0, 0 );
			float clampResult69 = clamp( triplanar170.y , ( 1.0 - _SnowAmbientOcclusionPower ) , 1.0 );
			float lerpResult27 = lerp( clampResult67 , clampResult69 , SnowCover231);
			o.Occlusion = lerpResult27;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardSpecular keepalpha fullforwardshadows 

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
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
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
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandardSpecular o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandardSpecular, o )
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