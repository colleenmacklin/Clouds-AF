// Constants
#define PI 3.1415926535
#define Pi316 0.0596831
#define Pi14 0.07957747
#define MieG float3(0.4375f, 1.5625f, 1.5f)

// Textures
uniform sampler2D   _Azure_SunTexture;
uniform sampler2D   _Azure_MoonTexture;
uniform sampler2D   _Azure_StaticCloudSourceTexture;
uniform sampler2D   _Azure_StaticCloudTargetTexture;
uniform samplerCUBE _Azure_StarFieldTexture;

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

// Fog scattering
uniform float _Azure_FogScatteringScale;
uniform float _Azure_GlobalFogDistance;
uniform float _Azure_GlobalFogSmooth;
uniform float _Azure_GlobalFogDensity;
uniform float _Azure_HeightFogDistance;
uniform float _Azure_HeightFogSmooth;
uniform float _Azure_HeightFogDensity;
uniform float _Azure_HeightFogStart;
uniform float _Azure_HeightFogEnd;

// Clouds
uniform float  _Azure_StaticCloudInterpolator;
uniform float  _Azure_StaticCloudLayer1Speed;
uniform float  _Azure_StaticCloudLayer2Speed;
uniform float4 _Azure_StaticCloudColor;
uniform float  _Azure_StaticCloudScattering;
uniform float  _Azure_StaticCloudExtinction;
uniform float  _Azure_StaticCloudDensity;
uniform float  _Azure_StaticCloudOpacity;
uniform float  _Azure_DynamicCloudAltitude;
uniform float  _Azure_DynamicCloudDirection;
uniform float  _Azure_DynamicCloudSpeed;
uniform float  _Azure_DynamicCloudDensity;
uniform float4 _Azure_DynamicCloudColor1;
uniform float4 _Azure_DynamicCloudColor2;
uniform float  _Azure_ThunderLightningEffect;