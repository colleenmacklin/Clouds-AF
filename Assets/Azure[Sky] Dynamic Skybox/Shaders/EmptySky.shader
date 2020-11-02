Shader "Azure[Sky]/EmptySky"
{
    Properties
    {
        _Azure_SunTexture ("Sun Texture (RGB) ", 2D) = "" {}
        _Azure_MoonTexture ("Moon Texture (RGB) ", 2D) = "" {}
        _Azure_StarFieldTexture ("Starfield Texture (RGB) ", Cube) = "" {}
    }
    
    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" "IgnoreProjector"="True" }
	    Cull Back     // Render side
		Fog{Mode Off} // Don't use fog
     	ZWrite Off    // Don't draw to depth buffer

        Pass
        {
            HLSLPROGRAM
            
            #pragma vertex vertex_program
            #pragma fragment fragment_program
            #pragma target 3.0
            #include "UnityCG.cginc"

            // Constants
            #define PI 3.1415926535
            #define Pi316 0.0596831
            #define Pi14 0.07957747
            #define MieG float3(0.4375f, 1.5625f, 1.5f)
            
            // Textures
            uniform sampler2D   _Azure_SunTexture;
            uniform sampler2D   _Azure_MoonTexture;
            uniform samplerCUBE _Azure_StarFieldTexture;

            // Directions
            uniform float3   _Azure_SunDirection;
            uniform float3   _Azure_MoonDirection;
            uniform float4x4 _Azure_SunMatrix;
            uniform float4x4 _Azure_MoonMatrix;
            uniform float4x4 _Azure_UpDirectionMatrix;
            uniform float4x4 _Azure_StarFieldMatrix;

            // Scattering
            uniform float  _Azure_FogScatteringScale;
            uniform int    _Azure_ScatteringMode;
            uniform float3 _Azure_Rayleigh;
            uniform float3 _Azure_Mie;
            uniform float  _Azure_Scattering;
            uniform float  _Azure_Luminance;
            uniform float  _Azure_Exposure;
            uniform float4 _Azure_RayleighColor;
            uniform float4 _Azure_MieColor;
            uniform float4 _Azure_ScatteringColor;

            // Outer Space
            uniform float  _Azure_SunTextureSize;
            uniform float  _Azure_SunTextureIntensity;
            uniform float4 _Azure_SunTextureColor;
            uniform float  _Azure_MoonTextureSize;
            uniform float  _Azure_MoonTextureIntensity;
            uniform float4 _Azure_MoonTextureColor;
            uniform float  _Azure_StarsIntensity;
            uniform float  _Azure_MilkyWayIntensity;
            uniform float4 _Azure_StarFieldColor;

            // Raytracing moon sphere
            bool iSphere(in float3 origin, in float3 direction, in float3 position, in float radius, out float3 normalDirection)
            {
                float3 rc = origin - position;
                float c = dot(rc, rc) - (radius * radius);
                float b = dot(direction, rc);
                float d = b * b - c;
                float t = -b - sqrt(abs(d));
                float st = step(0.0, min(t, d));
                normalDirection = normalize(-position + (origin + direction * t));
                
                if (st > 0.0) { return true; }
                return false;
            }

            // Mesh data
            struct Attributes
            {
                float4 vertex : POSITION;
            };

            // Vertex to fragment
            struct Varyings
            {
                float4 Position : SV_POSITION;
                float3 WorldPos : TEXCOORD0;
                float3 SunPos   : TEXCOORD1;
                float3 MoonPos  : TEXCOORD2;
                float3 StarPos  : TEXCOORD3;
            };

            // Vertex shader
            Varyings vertex_program (Attributes v)
            {
                Varyings Output = (Varyings)0;
                
                Output.Position = UnityObjectToClipPos(v.vertex);
                Output.WorldPos = normalize(mul((float3x3)unity_WorldToObject, v.vertex.xyz));
                Output.WorldPos = normalize(mul((float3x3)_Azure_UpDirectionMatrix, Output.WorldPos));

                Output.SunPos = mul((float3x3)_Azure_SunMatrix, v.vertex.xyz) * _Azure_SunTextureSize;
                Output.StarPos  = mul((float3x3)_Azure_StarFieldMatrix, Output.WorldPos);
                Output.MoonPos = mul((float3x3)_Azure_MoonMatrix, v.vertex.xyz) * 0.75 * _Azure_MoonTextureSize;
                Output.MoonPos.x *= -1.0;

                return Output;
            }

            // Fragment shader
            float4 fragment_program (Varyings Input) : SV_Target
            {
                // Directions
                float3 viewDir = normalize(Input.WorldPos);
                float sunCosTheta = dot(viewDir, _Azure_SunDirection);
                float moonCosTheta = dot(viewDir, _Azure_MoonDirection);
                float r = length(float3(0.0, 50.0, 0.0));
                float sunRise = saturate(dot(float3(0.0, 500.0, 0.0), _Azure_SunDirection) / r);
                float moonRise = saturate(dot(float3(0.0, 500.0, 0.0), _Azure_MoonDirection) / r);

                // Optical depth
                float zenith = acos(saturate(dot(float3(0.0, 1.0, 0.0), viewDir))) * _Azure_FogScatteringScale;
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
                float3 BmTheta  = Pi14  * _Azure_Mie * miePhase * _Azure_MieColor * sunRise;
                float3 BrmTheta = (BrTheta + BmTheta) / (_Azure_Rayleigh + _Azure_Mie);
                float3 inScatter = BrmTheta * Esun * _Azure_Scattering * (1.0 - fex);
                inScatter *= sunRise;

                // Moon inScattering
                rayPhase = 2.0 + 0.5 * pow(moonCosTheta, 2.0);
                miePhase = MieG.x / pow(MieG.y - MieG.z * moonCosTheta, 1.5);
                BrTheta  = Pi316 * _Azure_Rayleigh * rayPhase * _Azure_RayleighColor;
                BmTheta  = Pi14  * _Azure_Mie * miePhase * _Azure_MieColor * moonRise;
                BrmTheta = (BrTheta + BmTheta) / (_Azure_Rayleigh + _Azure_Mie);
                Esun = _Azure_ScatteringMode == 0 ? (1.0 - fex) : _Azure_ScatteringColor;
                float3 moonInScatter = BrmTheta * Esun * _Azure_Scattering * 0.1 * (1.0 - fex);
                //moonInScatter *= moonRise;
                moonInScatter *= 1.0 - sunRise;

                // Default night sky - When there is no moon in the sky
                BrmTheta = BrTheta / (_Azure_Rayleigh + _Azure_Mie);
                float3 skyLuminance = BrmTheta * _Azure_ScatteringColor * _Azure_Luminance * (1.0 - fex);

                // Sun texture
                float3 sunTexture = tex2D( _Azure_SunTexture, Input.SunPos + 0.5).rgb * _Azure_SunTextureColor * _Azure_SunTextureIntensity;
                sunTexture = pow(sunTexture, 2.0);
                sunTexture *= fex.b * saturate(sunCosTheta);

                // Moon sphere
                float3 rayOrigin = float3(0.0, 0.0, 0.0);//_WorldSpaceCameraPos;
                float3 rayDirection = viewDir;
                float3 moonPosition = _Azure_MoonDirection * 38400.0 * _Azure_MoonTextureSize;
                float3 normalDirection = float3(0.0, 0.0, 0.0);
                float3 moonColor = float3(0.0, 0.0, 0.0);
                float4 moonTexture = saturate(tex2D( _Azure_MoonTexture, Input.MoonPos.xy + 0.5) * moonCosTheta);
                float moonMask = 1.0 - moonTexture.a;
                if(iSphere(rayOrigin, rayDirection, moonPosition, 17370.0, normalDirection))
                {
                    float moonSphere = max(dot(normalDirection, _Azure_SunDirection), 0.0) * moonTexture.a * 2.0;
                    moonColor = moonTexture.rgb * moonSphere * _Azure_MoonTextureColor * _Azure_MoonTextureIntensity * moonExtinction;
                }

                // Starfield
                float4 starTex = texCUBE(_Azure_StarFieldTexture, Input.StarPos);
                float3 stars = starTex.rgb * pow(starTex.a, 2.0) * _Azure_StarsIntensity;
                float3 milkyWay = pow(starTex.rgb, 1.5) * _Azure_MilkyWayIntensity;
                float3 starfield = (stars + milkyWay) * _Azure_StarFieldColor * horizonExtinction * moonMask;

                // Output
                float3 OutputColor = inScatter + sunTexture + skyLuminance + moonInScatter + moonColor + starfield;

                // Tonemapping
                OutputColor = saturate(1.0 - exp(-_Azure_Exposure * OutputColor));

                // Color correction
                OutputColor = pow(OutputColor, 2.2);
                #ifdef UNITY_COLORSPACE_GAMMA
                OutputColor = pow(OutputColor, 0.4545);
                #endif

                return float4(OutputColor, 1.0);
            }
            
            ENDHLSL
        }
    }
}