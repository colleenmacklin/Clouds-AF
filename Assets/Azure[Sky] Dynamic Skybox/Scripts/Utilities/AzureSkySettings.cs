namespace UnityEngine.AzureSky
{
    public sealed class AzureSkySettings
    {
        // Scattering
        public float MolecularDensity = 2.545f;
        public Vector3 Wavelength = new Vector3(680.0f, 550.0f, 450.0f); // Visible wavelength 380 to 740
        public float Rayleigh = 1.5f;
        public float Mie = 1.0f;
        public float Scattering = 0.25f;
        public float Luminance = 1.5f;
        public float Exposure = 2.0f;
        public Color RayleighColor = Color.white;
        public Color MieColor = Color.white;
        public Color ScatteringColor = Color.white;
        
        // Outer space
        public float SunTextureSize = 1.5f;
        public float SunTextureIntensity = 1.0f;
        public Color SunTextureColor = Color.white;
        public float MoonTextureSize = 1.5f;
        public float MoonTextureIntensity = 1.0f;
        public Color MoonTextureColor = Color.white;
        public float StarsIntensity = 0.5f;
        public float MilkyWayIntensity = 0.0f;
        
        // Fog scattering
        public float FogScatteringScale = 1.0f;
        public float GlobalFogDistance = 1000.0f;
        public float GlobalFogSmooth = 0.25f;
        public float GlobalFogDensity = 1.0f;
        public float HeightFogDistance = 100.0f;
        public float HeightFogSmooth = 1.0f;
        public float HeightFogDensity = 0.0f;
        public float HeightFogStart = 0.0f;
        public float HeightFogEnd = 100.0f;
        
        // Clouds
        public float StaticCloudInterpolator = 0.0f;
        public float StaticCloudLayer1Speed = 0.0f;
        public float StaticCloudLayer2Speed = 0.0f;
        public Color StaticCloudColor = Color.white;
        public float StaticCloudScattering = 1.0f;
        public float StaticCloudExtinction = 1.5f;
        public float StaticCloudSaturation = 2.5f;
        public float StaticCloudOpacity = 1.25f;
        public float DynamicCloudAltitude = 7.5f;
        public float DynamicCloudDirection = 1.0f;
        public float DynamicCloudSpeed = 0.1f;
        public float DynamicCloudDensity = 0.75f;
        public Color DynamicCloudColor1 = Color.white;
        public Color DynamicCloudColor2 = Color.white;
        
        // Lighting
        public float DirectionalLightIntensity = 1.0f;
        public Color DirectionalLightColor = Color.white;
        public float EnvironmentIntensity = 1.0f;
        public Color EnvironmentAmbientColor = Color.white;
        public Color EnvironmentEquatorColor = Color.white;
        public Color EnvironmentGroundColor = Color.white;
        
        // Weather
        public float LightRainIntensity = 0.0f;
        public float MediumRainIntensity = 0.0f;
        public float HeavyRainIntensity = 0.0f;
        public float SnowIntensity = 0.0f;
        public Color RainColor = Color.white;
        public Color SnowColor = Color.white;
        public float LightRainSoundVolume = 0.0f;
        public float MediumRainSoundVolume = 0.0f;
        public float HeavyRainSoundVolume = 0.0f;
        public float LightWindSoundVolume = 0.0f;
        public float MediumWindSoundVolume = 0.0f;
        public float HeavyWindSoundVolume = 0.0f;
        public float WindSpeed = 0.0f;
        public float WindDirection = 0.0f;
    }
}