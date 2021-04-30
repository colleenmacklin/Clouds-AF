namespace UnityEngine.AzureSky
{
    internal static class AzureShaderUniforms
    {
        // Textures
        internal static readonly int SunTexture = Shader.PropertyToID("_Azure_SunTexture");
        internal static readonly int MoonTexture = Shader.PropertyToID("_Azure_MoonTexture");
        internal static readonly int StarFieldTexture = Shader.PropertyToID("_Azure_StarFieldTexture");
        internal static readonly int DynamicCloudTexture = Shader.PropertyToID("_Azure_DynamicCloudTexture");
        internal static readonly int StaticCloudTexture = Shader.PropertyToID("_Azure_StaticCloudTexture");
        
        // Directions
        internal static readonly int SunDirection = Shader.PropertyToID("_Azure_SunDirection");
        internal static readonly int MoonDirection = Shader.PropertyToID("_Azure_MoonDirection");
        internal static readonly int SunMatrix = Shader.PropertyToID("_Azure_SunMatrix");
        internal static readonly int MoonMatrix = Shader.PropertyToID("_Azure_MoonMatrix");
        internal static readonly int UpDirectionMatrix = Shader.PropertyToID("_Azure_UpDirectionMatrix");
        internal static readonly int StarfieldMatrix = Shader.PropertyToID("_Azure_StarFieldMatrix");
        
        // Scattering
        internal static readonly int ScatteringMode = Shader.PropertyToID("_Azure_ScatteringMode");
        internal static readonly int Kr = Shader.PropertyToID("_Azure_Kr");
        internal static readonly int Km = Shader.PropertyToID("_Azure_Km");
        internal static readonly int Rayleigh = Shader.PropertyToID("_Azure_Rayleigh");
        internal static readonly int Mie = Shader.PropertyToID("_Azure_Mie");
        internal static readonly int MieDistance = Shader.PropertyToID("_Azure_MieDepth");
        internal static readonly int Scattering = Shader.PropertyToID("_Azure_Scattering");
        internal static readonly int Luminance = Shader.PropertyToID("_Azure_Luminance");
        internal static readonly int Exposure = Shader.PropertyToID("_Azure_Exposure");
        internal static readonly int RayleighColor = Shader.PropertyToID("_Azure_RayleighColor");
        internal static readonly int MieColor = Shader.PropertyToID("_Azure_MieColor");
        internal static readonly int ScatteringColor = Shader.PropertyToID("_Azure_ScatteringColor");
        
        // Outer space
        internal static readonly int SunTextureSize = Shader.PropertyToID("_Azure_SunTextureSize");
        internal static readonly int SunTextureIntensity = Shader.PropertyToID("_Azure_SunTextureIntensity");
        internal static readonly int SunTextureColor = Shader.PropertyToID("_Azure_SunTextureColor");
        internal static readonly int MoonTextureSize = Shader.PropertyToID("_Azure_MoonTextureSize");
        internal static readonly int MoonTextureIntensity = Shader.PropertyToID("_Azure_MoonTextureIntensity");
        internal static readonly int MoonTextureColor = Shader.PropertyToID("_Azure_MoonTextureColor");
        internal static readonly int StarsIntensity = Shader.PropertyToID("_Azure_StarsIntensity");
        internal static readonly int MilkyWayIntensity = Shader.PropertyToID("_Azure_MilkyWayIntensity");
        internal static readonly int StarFieldColor = Shader.PropertyToID("_Azure_StarFieldColor");
        internal static readonly int StarFieldRotation = Shader.PropertyToID("_Azure_StarFieldRotationMatrix");
        
        // Fog scattering
        internal static readonly int FogScatteringScale = Shader.PropertyToID("_Azure_FogScatteringScale");
        internal static readonly int GlobalFogDistance = Shader.PropertyToID("_Azure_GlobalFogDistance");
        internal static readonly int GlobalFogSmoothStep = Shader.PropertyToID("_Azure_GlobalFogSmooth");
        internal static readonly int GlobalFogDensity = Shader.PropertyToID("_Azure_GlobalFogDensity");
        internal static readonly int HeightFogDistance = Shader.PropertyToID("_Azure_HeightFogDistance");
        internal static readonly int HeightFogSmoothStep = Shader.PropertyToID("_Azure_HeightFogSmooth");
        internal static readonly int HeightFogDensity = Shader.PropertyToID("_Azure_HeightFogDensity");
        internal static readonly int HeightFogStart = Shader.PropertyToID("_Azure_HeightFogStart");
        internal static readonly int HeightFogEnd = Shader.PropertyToID("_Azure_HeightFogEnd");

        // Clouds
        internal static readonly int DynamicCloudAltitude = Shader.PropertyToID("_Azure_DynamicCloudAltitude");
        internal static readonly int DynamicCloudDirection = Shader.PropertyToID("_Azure_DynamicCloudDirection");
        internal static readonly int DynamicCloudDensity = Shader.PropertyToID("_Azure_DynamicCloudDensity");
        internal static readonly int DynamicCloudColor1 = Shader.PropertyToID("_Azure_DynamicCloudColor1");
        internal static readonly int DynamicCloudColor2 = Shader.PropertyToID("_Azure_DynamicCloudColor2");
        internal static readonly int ThunderLightningEffect = Shader.PropertyToID("_Azure_ThunderLightningEffect");
        internal static readonly int StaticCloudInterpolator = Shader.PropertyToID("_Azure_StaticCloudInterpolator");
        internal static readonly int StaticCloudLayer1Speed = Shader.PropertyToID("_Azure_StaticCloudLayer1Speed");
        internal static readonly int StaticCloudLayer2Speed = Shader.PropertyToID("_Azure_StaticCloudLayer2Speed");
        internal static readonly int StaticCloudColor = Shader.PropertyToID("_Azure_StaticCloudColor");
        internal static readonly int StaticCloudScattering = Shader.PropertyToID("_Azure_StaticCloudScattering");
        internal static readonly int StaticCloudExtinction = Shader.PropertyToID("_Azure_StaticCloudExtinction");
        internal static readonly int StaticCloudSaturation = Shader.PropertyToID("_Azure_StaticCloudSaturation");
        internal static readonly int StaticCloudOpacity = Shader.PropertyToID("_Azure_StaticCloudOpacity");
    }
}