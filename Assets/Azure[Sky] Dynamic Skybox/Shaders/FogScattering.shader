Shader "Azure[Sky]/FogScattering"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
            HLSLPROGRAM
			
            #pragma vertex vertex_program
            #pragma fragment fragment_program
            #pragma target 3.0
            #include "UnityCG.cginc"

            //  Start: LuxWater
            #pragma multi_compile __ LUXWATER_DEFERREDFOG
            
            #if defined(LUXWATER_DEFERREDFOG)
                sampler2D _UnderWaterMask;
                float4 _LuxUnderWaterDeferredFogParams; // x: IsInsideWatervolume?, y: BelowWaterSurface shift, z: EdgeBlend
            #endif
            //  End: LuxWater
            
            // Constants
            #define PI 3.1415926535
            #define Pi316 0.0596831
            #define Pi14 0.07957747
            #define MieG float3(0.4375f, 1.5625f, 1.5f)
            
            // Textures
            uniform sampler2D _MainTex;
            uniform sampler2D_float _CameraDepthTexture;
            uniform float4x4  _FrustumCorners;
            uniform float4    _MainTex_TexelSize;

            // Directions
            uniform float3   _Azure_SunDirection;
            uniform float3   _Azure_MoonDirection;
            uniform float4x4 _Azure_SunMatrix;
            uniform float4x4 _Azure_MoonMatrix;
            uniform float4x4 _Azure_UpDirectionMatrix;
            uniform float4x4 _Azure_StarFieldMatrix;

            // Scattering
            uniform int    _Azure_ScatteringMode;
            uniform float3 _Azure_Rayleigh;
            uniform float3 _Azure_Mie;
            uniform float  _Azure_Scattering;
            uniform float  _Azure_Luminance;
            uniform float  _Azure_Exposure;
            uniform float4 _Azure_RayleighColor;
            uniform float4 _Azure_MieColor;
            uniform float4 _Azure_ScatteringColor;
            uniform float  _Azure_MieDepth;

            // Fog paramters
            uniform float _Azure_FogScatteringScale;
            uniform float _Azure_GlobalFogDistance;
            uniform float _Azure_GlobalFogSmooth;
            uniform float _Azure_GlobalFogDensity;
            uniform float _Azure_HeightFogDistance;
            uniform float _Azure_HeightFogSmooth;
            uniform float _Azure_HeightFogDensity;
            uniform float _Azure_HeightFogStart;
            uniform float _Azure_HeightFogEnd;

            // Mesh data
            struct Attributes
            {
                float4 vertex   : POSITION;
                float4 texcoord : TEXCOORD0;
            };

            // Vertex to fragment
            struct Varyings
            {
                float4 Position        : SV_POSITION;
                float2 screen_uv 	   : TEXCOORD0;
                float4 interpolatedRay : TEXCOORD1;
                float2 depth_uv        : TEXCOORD2;
            };

            // Vertex shader
            Varyings vertex_program (Attributes v)
            {
                Varyings Output = (Varyings)0;

                v.vertex.z = 0.1;
                Output.Position = UnityObjectToClipPos(v.vertex);
                Output.screen_uv = v.texcoord.xy;
                Output.depth_uv = v.texcoord.xy;
                #if UNITY_UV_STARTS_AT_TOP
                if (_MainTex_TexelSize.y < 0)
                    Output.screen_uv.y = 1 - Output.screen_uv.y;
                #endif

                // Based on Unity5.6 GlobalFog
                int index = v.texcoord.x + (2.0 * Output.screen_uv.y);
                Output.interpolatedRay   = _FrustumCorners[index];
                Output.interpolatedRay.xyz = mul((float3x3)_Azure_UpDirectionMatrix, Output.interpolatedRay.xyz);
                Output.interpolatedRay.w = index;

                return Output;
            }

            // Fragment shader
            float4 fragment_program (Varyings Input) : SV_Target
            {
                // Original scene
                float3 screen = tex2D(_MainTex, UnityStereoTransformScreenSpaceTex(Input.screen_uv)).rgb;

                // Reconstruct world space position and direction towards this screen pixel
                float depth = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture,UnityStereoTransformScreenSpaceTex(Input.depth_uv))));
                if(depth == 1.0) return float4(screen, 1.0);
                float mieDepth = saturate(lerp(depth * 4, 1.0, _Azure_MieDepth));

                // Directions
                float3 viewDir = normalize(depth * Input.interpolatedRay.xyz);
                float sunCosTheta = dot(viewDir, _Azure_SunDirection);
                float moonCosTheta = dot(viewDir, _Azure_MoonDirection);
                float r = length(float3(0.0, 50.0, 0.0));
                float sunRise = saturate(dot(float3(0.0, 500.0, 0.0), _Azure_SunDirection) / r);
                float moonRise = saturate(dot(float3(0.0, 500.0, 0.0), _Azure_MoonDirection) / r);

                // Optical depth
                //float zenith = acos(saturate(dot(float3(0.0, 1.0, 0.0), viewDir)));
                //float zenith = acos(length(viewDir.y));
                float zenith = acos(saturate(dot(float3(-1.0, 1.0, -1.0), depth))) * _Azure_FogScatteringScale;
                float z = (cos(zenith) + 0.15 * pow(93.885 - ((zenith * 180.0f) / PI), -1.253));
                float SR = 8400.0 / z;
                float SM = 1200.0 / z;

                // Extinction
                float3 fex = exp(-(_Azure_Rayleigh * SR  + _Azure_Mie * SM));
                float horizonExtinction = saturate((viewDir.y) * 1000.0) * fex.b;
                float moonExtinction = saturate((viewDir.y) * 2.5);
                float sunset = clamp(dot(float3(0.0, 1.0, 0.0), _Azure_SunDirection), 0.0, 0.5);
                float3 Esun = _Azure_ScatteringMode == 0 ? lerp(fex, (1.0 - fex), sunset) : _Azure_ScatteringColor;

                // Sun inScattering
                float  rayPhase = 2.0 + 0.5 * pow(sunCosTheta, 2.0);
                float  miePhase = MieG.x / pow(MieG.y - MieG.z * sunCosTheta, 1.5);
                float3 BrTheta  = Pi316 * _Azure_Rayleigh * rayPhase * _Azure_RayleighColor;
                float3 BmTheta  = Pi14  * _Azure_Mie * miePhase * _Azure_MieColor * sunRise * mieDepth;
                float3 BrmTheta = (BrTheta + BmTheta) / (_Azure_Rayleigh + _Azure_Mie);
                float3 inScatter = BrmTheta * Esun * _Azure_Scattering * (1.0 - fex);
                inScatter *= sunRise;

                // Moon inScattering
                rayPhase = 2.0 + 0.5 * pow(moonCosTheta, 2.0);
                miePhase = MieG.x / pow(MieG.y - MieG.z * moonCosTheta, 1.5);
                BrTheta  = Pi316 * _Azure_Rayleigh * rayPhase * _Azure_RayleighColor;
                BmTheta  = Pi14  * _Azure_Mie * miePhase * _Azure_MieColor * moonRise * mieDepth;
                BrmTheta = (BrTheta + BmTheta) / (_Azure_Rayleigh + _Azure_Mie);
                Esun = _Azure_ScatteringMode == 0 ? (1.0 - fex) : _Azure_ScatteringColor;
                float3 moonInScatter = BrmTheta * Esun * _Azure_Scattering * 0.1 * (1.0 - fex);
                //moonInScatter *= moonRise;
                moonInScatter *= 1.0 - sunRise;

                // Default night sky - When there is no moon in the sky
                BrmTheta = BrTheta / (_Azure_Rayleigh + _Azure_Mie);
                float3 skyLuminance = BrmTheta * _Azure_ScatteringColor * _Azure_Luminance * (1.0 - fex);

                // Output
                float3 OutputColor = inScatter + skyLuminance + moonInScatter;

                // Tonemapping
                OutputColor = saturate(1.0 - exp(-_Azure_Exposure * OutputColor));

                // Color correction
                OutputColor = pow(OutputColor, 2.2);
                #ifdef UNITY_COLORSPACE_GAMMA
                OutputColor = pow(OutputColor, 0.4545);
                #endif

                // Calcule fog distance
                float globalFog = smoothstep(-_Azure_GlobalFogSmooth, 1.25, depth * _ProjectionParams.z / _Azure_GlobalFogDistance) * _Azure_GlobalFogDensity;
                float heightFogDistance = smoothstep(-_Azure_HeightFogSmooth, 1.25, depth * _ProjectionParams.z / _Azure_HeightFogDistance);

                // Calcule height fog
                float3 worldSpaceDirection = mul((float3x3)_Azure_UpDirectionMatrix, _WorldSpaceCameraPos) + depth * Input.interpolatedRay.xyz;
                float heightFog = saturate((worldSpaceDirection.y - _Azure_HeightFogStart) / (_Azure_HeightFogEnd + _Azure_HeightFogStart));
                heightFog = 1.0 - heightFog;
                heightFog *= heightFog;
                heightFog *= heightFogDistance;
                heightFog *= _Azure_HeightFogDensity;
                float fog = saturate(globalFog + heightFog);

                //  Start: LuxWater
                #if defined(LUXWATER_DEFERREDFOG)
                    half4 fogMask = tex2D(_UnderWaterMask, UnityStereoTransformScreenSpaceTex(Input.screen_uv));
                    float watersurfacefrombelow = DecodeFloatRG(fogMask.ba);

                    //	Get distance and lower it a bit in order to handle edge blending artifacts (edge blended parts would not get ANY fog)
                    float dist = (watersurfacefrombelow - depth) + _LuxUnderWaterDeferredFogParams.y * _ProjectionParams.w;
                    //	Fade fog from above water to below water
                    float fogFactor = saturate ( 1.0 + _ProjectionParams.z * _LuxUnderWaterDeferredFogParams.z * dist ); // 0.125 
                    //	Clamp above result to where water is actually rendered
                    fogFactor = (fogMask.r == 1) ? fogFactor : 1.0;
                    //  Mask fog on underwarter parts - only if we are inside a volume (bool... :( )
                    if(_LuxUnderWaterDeferredFogParams.x)
                    {
                        fogFactor *= saturate( 1.0 - fogMask.g * 8.0);
                        if (dist < -_ProjectionParams.w * 4 && fogMask.r == 0 && fogMask.g < 1.0)
                        {
                            fogFactor = 1.0;
                        }
                    }
                    //	Tweak fog factor
                    fog *= fogFactor;
                #endif
                //  End: LuxWater

                OutputColor.rgb = lerp(screen.rgb, OutputColor.rgb, fog);
                return float4(OutputColor.rgb, 1.0);
            }
            
            ENDHLSL
		}
	}
}