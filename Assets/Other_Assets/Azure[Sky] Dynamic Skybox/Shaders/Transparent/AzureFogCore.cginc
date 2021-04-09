// Constants
#define PI 3.1415926535
#define Pi316 0.0596831
#define Pi14 0.07957747
#define MieG float3(0.4375f, 1.5625f, 1.5f)

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
uniform float _Azure_MieDepth;

// Compute fog scattering color
float3 ComputeFogScatteringColor(float3 worldPos)
{
	// Initializations
    float3 transmittance = float3(1.0, 1.0, 1.0);
    
    // Directions
    float3 viewDir = normalize(_WorldSpaceCameraPos - worldPos) * -1.0;
    float sunCosTheta = dot(viewDir, _Azure_SunDirection);
    float moonCosTheta = dot(viewDir, _Azure_MoonDirection);
    float r = length(float3(0.0, 50.0, 0.0));
    float sunRise = saturate(dot(float3(0.0, 500.0, 0.0), _Azure_SunDirection) / r);
    float moonRise = saturate(dot(float3(0.0, 500.0, 0.0), _Azure_MoonDirection) / r);
    float mieDepth = saturate(lerp(distance(_WorldSpaceCameraPos, worldPos) * _ProjectionParams.w * 4.0, 1.0, _Azure_MieDepth));
    
    // Optical depth
    //float zenith = acos(saturate(dot(float3(0.0, 1.0, 0.0), viewDir)));
    //float zenith = acos(length(viewDir.y));
    float zenith = acos(saturate(dot(float3(-1.0, 1.0, -1.0), length(viewDir)))) * _Azure_FogScatteringScale;
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

	return OutputColor;
}

// Apply fog scattering
float4 ApplyAzureFog(float4 fragOutput, float3 worldPos)
{
    // Fog color
    float3 fogScatteringColor = float3(0.0, 0.0, 0.0);
	#ifdef UNITY_PASS_FORWARDADD
		fogScatteringColor = float3(0.0, 0.0, 0.0);
	#else
        fogScatteringColor = ComputeFogScatteringColor(worldPos);
	#endif

	// Global fog
	float depth = distance(_WorldSpaceCameraPos, worldPos);
	float fog = smoothstep(-_Azure_GlobalFogSmooth, 1.25, depth / _Azure_GlobalFogDistance) * _Azure_GlobalFogDensity;
	float heightFogDistance = smoothstep(-_Azure_HeightFogSmooth, 1.25, depth / _Azure_HeightFogDistance);

	// Height fog
	float3 worldSpaceDirection = mul((float3x3)_Azure_UpDirectionMatrix, worldPos.xyz);
	float heightFog = saturate((worldSpaceDirection.y - _Azure_HeightFogStart) / (_Azure_HeightFogEnd + _Azure_HeightFogStart));
	heightFog = 1.0 - heightFog;
	heightFog *= heightFog;
	heightFog *= heightFogDistance;
	float fogFactor = saturate(fog + heightFog * _Azure_HeightFogDensity);

	// Apply fog
	#if defined(_ALPHAPREMULTIPLY_ON)
	fragOutput.a = lerp(fragOutput.a, 1.0, fogFactor);
	#endif
	fogScatteringColor = lerp(fragOutput.rgb, fogScatteringColor, fogFactor * lerp(fragOutput.a, 1.0, 2.0 - fogFactor));
	return float4(fogScatteringColor, fragOutput.a);
}

// Apply fog scattering to additive/multiply blend mode.
float4 ApplyAzureFog(float4 fragOutput, float3 worldPos, float4 fogColor)
{
    // Fog color
    float3 fogScatteringColor = float3(0.0, 0.0, 0.0);
	#ifdef UNITY_PASS_FORWARDADD
		fogScatteringColor = float3(0.0, 0.0, 0.0);
	#else
		fogScatteringColor = fogColor;
	#endif

	// Global fog
	float depth = distance(_WorldSpaceCameraPos, worldPos);
	float fog = smoothstep(-_Azure_GlobalFogSmooth, 1.25, depth / _Azure_GlobalFogDistance) * _Azure_GlobalFogDensity;
	float heightFogDistance = smoothstep(-_Azure_HeightFogSmooth, 1.25, depth / _Azure_HeightFogDistance);

	// Height fog
	float3 worldSpaceDirection = mul((float3x3)_Azure_UpDirectionMatrix, worldPos.xyz);
	float heightFog = saturate((worldSpaceDirection.y - _Azure_HeightFogStart) / (_Azure_HeightFogEnd + _Azure_HeightFogStart));
	heightFog = 1.0 - heightFog;
	heightFog *= heightFog;
	heightFog *= heightFogDistance;
	float fogFactor = saturate(fog + heightFog * _Azure_HeightFogDensity);

	// Apply fog
	#if defined(_ALPHAPREMULTIPLY_ON)
	fragOutput.a = lerp(fragOutput.a, 1.0, fogFactor);
	#endif
	fogScatteringColor = lerp(fragOutput.rgb, fogScatteringColor, fogFactor * lerp(fragOutput.a, 1.0, 2.0 - fogFactor));
	return float4(fogScatteringColor, fragOutput.a);
}

// DEPRECATED - backward compatibility (Actually, the projPos parameter is no longer needed.)
float4 ApplyAzureFog(float4 fragOutput, float4 projPos, float3 worldPos)
{
    // Fog color
    float3 fogScatteringColor = float3(0.0, 0.0, 0.0);
	#ifdef UNITY_PASS_FORWARDADD
		fogScatteringColor = float3(0.0, 0.0, 0.0);
	#else
        fogScatteringColor = ComputeFogScatteringColor(worldPos);
	#endif

	// Global fog
	float depth = distance(_WorldSpaceCameraPos, worldPos);
	float fog = smoothstep(-_Azure_GlobalFogSmooth, 1.25, depth / _Azure_GlobalFogDistance) * _Azure_GlobalFogDensity;
	float heightFogDistance = smoothstep(-_Azure_HeightFogSmooth, 1.25, depth / _Azure_HeightFogDistance);

	// Height fog
	float3 worldSpaceDirection = mul((float3x3)_Azure_UpDirectionMatrix, worldPos.xyz);
	float heightFog = saturate((worldSpaceDirection.y - _Azure_HeightFogStart) / (_Azure_HeightFogEnd + _Azure_HeightFogStart));
	heightFog = 1.0 - heightFog;
	heightFog *= heightFog;
	heightFog *= heightFogDistance;
	float fogFactor = saturate(fog + heightFog * _Azure_HeightFogDensity);

	// Apply fog
	#if defined(_ALPHAPREMULTIPLY_ON)
	fragOutput.a = lerp(fragOutput.a, 1.0, fogFactor);
	#endif
	fogScatteringColor = lerp(fragOutput.rgb, fogScatteringColor, fogFactor * lerp(fragOutput.a, 1.0, 2.0 - fogFactor));
	return float4(fogScatteringColor, fragOutput.a);
}