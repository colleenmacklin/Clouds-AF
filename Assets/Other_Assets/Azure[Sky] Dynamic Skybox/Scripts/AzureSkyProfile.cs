using System.Collections.Generic;

namespace UnityEngine.AzureSky
{
    [CreateAssetMenu(fileName = "Sky Profile", menuName = "Azure[Sky] Dynamic Skybox/New Sky Profile", order = 1)]
    public sealed class AzureSkyProfile : ScriptableObject
    {
        // Not included in the build
        #if UNITY_EDITOR
        public bool showScatteringGroup = true;
        public bool showOuterSpaceGroup = true;
        public bool showFogScatteringGroup = true;
        public bool showCloudsGroup = true;
        public bool showLightingGroup = true;
        public bool showWeatherGroup = true;
        public bool showOutputsGroup = false;
        #endif
	    
	    // Outputs
	    public AzureOutputProfile outputProfile;
	    public List<AzureOutputProperty> outputPropertyList = new List<AzureOutputProperty>();
        
        // Molecular density
        public AzureFloatProperty molecularDensity = new AzureFloatProperty
        (
            2.545f,
            AnimationCurve.Linear (0.0f, 2.545f, 24.0f, 2.545f),
            AnimationCurve.Linear (-1.0f, 2.545f, 1.0f, 2.545f),
            AnimationCurve.Linear (-1.0f, 2.545f, 1.0f, 2.545f)
        );
        
        // Visible wavelength
        public AzureFloatProperty wavelengthR = new AzureFloatProperty
        (
            680.0f,
            AnimationCurve.Linear (0.0f, 680.0f, 24.0f, 680.0f),
            AnimationCurve.Linear (-1.0f, 680.0f, 1.0f, 680.0f),
            AnimationCurve.Linear (-1.0f, 680.0f, 1.0f, 680.0f)
        );
        
        public AzureFloatProperty wavelengthG = new AzureFloatProperty
        (
            550.0f,
            AnimationCurve.Linear (0.0f, 550.0f, 24.0f, 550.0f),
            AnimationCurve.Linear (-1.0f, 550.0f, 1.0f, 550.0f),
            AnimationCurve.Linear (-1.0f, 550.0f, 1.0f, 550.0f)
        );
        
        public AzureFloatProperty wavelengthB = new AzureFloatProperty
        (
            450.0f,
            AnimationCurve.Linear (0.0f, 450.0f, 24.0f, 450.0f),
            AnimationCurve.Linear (-1.0f, 450.0f, 1.0f, 450.0f),
            AnimationCurve.Linear (-1.0f, 450.0f, 1.0f, 450.0f)
        );
        
        // Rayleigh multiplier
        public AzureFloatProperty rayleigh = new AzureFloatProperty
        (
            1.5f,
            AnimationCurve.Linear (0.0f, 1.5f, 24.0f, 1.5f),
            AnimationCurve.Linear (-1.0f, 1.5f, 1.0f, 1.5f),
            AnimationCurve.Linear (-1.0f, 1.5f, 1.0f, 1.5f)
        );
        
        // Mie multiplier
        public AzureFloatProperty mie = new AzureFloatProperty
        (
            1.0f,
            AnimationCurve.Linear (0.0f, 1.0f, 24.0f, 1.0f),
            AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f),
            AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f)
        );
        
        // Scattering intensity
        public AzureFloatProperty scattering = new AzureFloatProperty
        (
            0.25f,
            AnimationCurve.Linear (0.0f, 0.25f, 24.0f, 0.25f),
            AnimationCurve.Linear (-1.0f, 0.25f, 1.0f, 0.25f),
            AnimationCurve.Linear (-1.0f, 0.25f, 1.0f, 0.25f)
        );
        
        // Sky luminance
        public AzureFloatProperty luminance = new AzureFloatProperty
        (
            1.5f,
            AnimationCurve.Linear (0.0f, 1.5f, 24.0f, 1.5f),
            AnimationCurve.Linear (-1.0f, 1.5f, 1.0f, 1.5f),
            AnimationCurve.Linear (-1.0f, 1.5f, 1.0f, 1.5f)
        );
        
        // Exposure
        public AzureFloatProperty exposure = new AzureFloatProperty
        (
            2.0f,
            AnimationCurve.Linear (0.0f, 2.0f, 24.0f, 2.0f),
            AnimationCurve.Linear (-1.0f, 2.0f, 1.0f, 2.0f),
            AnimationCurve.Linear (-1.0f, 2.0f, 1.0f, 2.0f)
        );
        
        // Rayleigh color
        public AzureColorProperty rayleighColor = new AzureColorProperty
        (
            Color.white,
            new Gradient(),
            new Gradient(),
            new Gradient()
        );
        
        // Mie color
        public AzureColorProperty mieColor = new AzureColorProperty
        (
            Color.white,
            new Gradient(),
            new Gradient(),
            new Gradient()
        );
        
        // Transmittance color
        public AzureColorProperty scatteringColor = new AzureColorProperty
        (
            Color.white,
            new Gradient(),
            new Gradient(),
            new Gradient()
        );
        
        // Sun texture size
		public AzureFloatProperty sunTextureSize = new AzureFloatProperty
		(
			2.5f,
			AnimationCurve.Linear (0.0f, 2.5f, 24.0f, 2.5f),
			AnimationCurve.Linear (-1.0f, 2.5f, 1.0f, 2.5f),
			AnimationCurve.Linear (-1.0f, 2.5f, 1.0f, 2.5f)
		);
        
		// Sun texture intensity
		public AzureFloatProperty sunTextureIntensity = new AzureFloatProperty
		(
			1.0f,
			AnimationCurve.Linear (0.0f, 1.0f, 24.0f, 1.0f),
			AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f),
			AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f)
		);
		
		// Sun texture color
		public AzureColorProperty sunTextureColor = new AzureColorProperty
		(
			Color.white,
			new Gradient(),
			new Gradient(),
			new Gradient()
		);
		
		// Moon texture size
		public AzureFloatProperty moonTextureSize = new AzureFloatProperty
		(
			10.0f,
			AnimationCurve.Linear (0.0f, 10.0f, 24.0f, 10.0f),
			AnimationCurve.Linear (-1.0f, 10.0f, 1.0f, 10.0f),
			AnimationCurve.Linear (-1.0f, 10.0f, 1.0f, 10.0f)
		);
		
		// Moon texture intensity
		public AzureFloatProperty moonTextureIntensity = new AzureFloatProperty
		(
			1.0f,
			AnimationCurve.Linear (0.0f, 1.0f, 24.0f, 1.0f),
			AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f),
			AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f)
		);
		
		// Moon texture color
		public AzureColorProperty moonTextureColor = new AzureColorProperty
		(
			Color.white,
			new Gradient(),
			new Gradient(),
			new Gradient()
		);
		
		// Stars intensity
		public AzureFloatProperty starsIntensity = new AzureFloatProperty
		(
			0.5f,
			AnimationCurve.Linear (0.0f, 0.5f, 24.0f, 0.5f),
			AnimationCurve.Linear (-1.0f, 0.5f, 1.0f, 0.5f),
			AnimationCurve.Linear (-1.0f, 0.5f, 1.0f, 0.5f)
		);
		
		// Milky Way intensity
		public AzureFloatProperty milkyWayIntensity = new AzureFloatProperty
		(
			0.0f,
			AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
		);
		
		// Fog scattering scale
        public AzureFloatProperty fogScatteringScale = new AzureFloatProperty
        (
            1.0f,
            AnimationCurve.Linear (0.0f, 1.0f, 24.0f, 1.0f),
            AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f),
            AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f)
        );
        
        // Global fog distance
        public AzureFloatProperty globalFogDistance = new AzureFloatProperty
        (
	        1000.0f,
	        AnimationCurve.Linear (0.0f, 1000.0f, 24.0f, 1000.0f),
	        AnimationCurve.Linear (-1.0f, 1000.0f, 1.0f, 1000.0f),
	        AnimationCurve.Linear (-1.0f, 1000.0f, 1.0f, 1000.0f)
        );
        
        // Global fog smooth
        public AzureFloatProperty globalFogSmooth = new AzureFloatProperty
        (
            0.25f,
            AnimationCurve.Linear (0.0f, 0.25f, 24.0f, 0.25f),
            AnimationCurve.Linear (-1.0f, 0.25f, 1.0f, 0.25f),
            AnimationCurve.Linear (-1.0f, 0.25f, 1.0f, 0.25f)
        );
        
        // Global fog density
        public AzureFloatProperty globalFogDensity = new AzureFloatProperty
        (
            1.0f,
            AnimationCurve.Linear (0.0f, 1.0f, 24.0f, 1.0f),
            AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f),
            AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f)
        );
        
        // Height fog distance
        public AzureFloatProperty heightFogDistance = new AzureFloatProperty
        (
	        100.0f,
	        AnimationCurve.Linear (0.0f, 100.0f, 24.0f, 100.0f),
	        AnimationCurve.Linear (-1.0f, 100.0f, 1.0f, 100.0f),
	        AnimationCurve.Linear (-1.0f, 100.0f, 1.0f, 100.0f)
        );
        
        // Height fog smooth
        public AzureFloatProperty heightFogSmooth = new AzureFloatProperty
        (
            1.0f,
            AnimationCurve.Linear (0.0f, 1.0f, 24.0f, 1.0f),
            AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f),
            AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f)
        );
        
        // Height fog density
        public AzureFloatProperty heightFogDensity = new AzureFloatProperty
        (
            0.0f,
            AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
            AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
            AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
        );
        
        // Height fog start
        public AzureFloatProperty heightFogStart = new AzureFloatProperty
        (
            0.0f,
            AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
            AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
            AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
        );
        
        // Height fog end
        public AzureFloatProperty heightFogEnd = new AzureFloatProperty
        (
            100.0f,
            AnimationCurve.Linear (0.0f, 100.0f, 24.0f, 100.0f),
            AnimationCurve.Linear (-1.0f, 100.0f, 1.0f, 100.0f),
            AnimationCurve.Linear (-1.0f, 100.0f, 1.0f, 100.0f)
        );
        
        // Static cloud texture
        public Texture2D staticCloudTexture;
        
        // Static cloud layer1 rotation speed
        public AzureFloatProperty staticCloudLayer1Speed = new AzureFloatProperty
        (
            0.0f,
            AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
            AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
            AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
        );
        
        // Static cloud layer2 rotation speed
        public AzureFloatProperty staticCloudLayer2Speed = new AzureFloatProperty
        (
            0.0f,
            AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
            AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
            AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
        );
        
        // Static cloud scattering
        public AzureFloatProperty staticCloudScattering = new AzureFloatProperty
        (
            1.0f,
            AnimationCurve.Linear (0.0f, 1.0f, 24.0f, 1.0f),
            AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f),
            AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f)
        );
        
        // Static cloud extinction
        public AzureFloatProperty staticCloudExtinction = new AzureFloatProperty
        (
            1.5f,
            AnimationCurve.Linear (0.0f, 1.5f, 24.0f, 1.5f),
            AnimationCurve.Linear (-1.0f, 1.5f, 1.0f, 1.5f),
            AnimationCurve.Linear (-1.0f, 1.5f, 1.0f, 1.5f)
        );
        
        // Static cloud density
        public AzureFloatProperty staticCloudSaturation = new AzureFloatProperty
        (
            2.5f,
            AnimationCurve.Linear (0.0f, 2.5f, 24.0f, 2.5f),
            AnimationCurve.Linear (-1.0f, 2.5f, 1.0f, 2.5f),
            AnimationCurve.Linear (-1.0f, 2.5f, 1.0f, 2.5f)
        );
        
        // Static cloud opacity
        public AzureFloatProperty staticCloudOpacity = new AzureFloatProperty
        (
            1.25f,
            AnimationCurve.Linear (0.0f, 1.25f, 24.0f, 1.25f),
            AnimationCurve.Linear (-1.0f, 1.25f, 1.0f, 1.25f),
            AnimationCurve.Linear (-1.0f, 1.25f, 1.0f, 1.25f)
        );
        
        // Static cloud color
        public AzureColorProperty staticCloudColor = new AzureColorProperty
        (
	        Color.white,
	        new Gradient(),
	        new Gradient(),
	        new Gradient()
        );
        
        // Dynamic cloud altitude
        public AzureFloatProperty dynamicCloudAltitude = new AzureFloatProperty
        (
            7.5f,
            AnimationCurve.Linear (0.0f, 7.5f, 24.0f, 7.5f),
            AnimationCurve.Linear (-1.0f, 7.5f, 1.0f, 7.5f),
            AnimationCurve.Linear (-1.0f, 7.5f, 1.0f, 7.5f)
        );
        
        // Dynamic cloud direction
        public AzureFloatProperty dynamicCloudDirection = new AzureFloatProperty
        (
            0.0f,
            AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
            AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
            AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
        );
        
        // Dynamic cloud speed
        public AzureFloatProperty dynamicCloudSpeed = new AzureFloatProperty
        (
            0.1f,
            AnimationCurve.Linear (0.0f, 0.1f, 24.0f, 0.1f),
            AnimationCurve.Linear (-1.0f, 0.1f, 1.0f, 0.1f),
            AnimationCurve.Linear (-1.0f, 0.1f, 1.0f, 0.1f)
        );
        
        // Dynamic cloud density
        public AzureFloatProperty dynamicCloudDensity = new AzureFloatProperty
        (
            0.75f,
            AnimationCurve.Linear (0.0f, 0.75f, 24.0f, 0.75f),
            AnimationCurve.Linear (-1.0f, 0.75f, 1.0f, 0.75f),
            AnimationCurve.Linear (-1.0f, 0.75f, 1.0f, 0.75f)
        );
        
        // Dynamic cloud color 1
        public AzureColorProperty dynamicCloudColor1 = new AzureColorProperty
        (
            Color.white,
            new Gradient(),
            new Gradient(),
            new Gradient()
        );
        
        // Dynamic cloud color 2
        public AzureColorProperty dynamicCloudColor2 = new AzureColorProperty
        (
            Color.white,
            new Gradient(),
            new Gradient(),
            new Gradient()
        );
        
        // Directional light intensity
        public AzureFloatProperty directionalLightIntensity = new AzureFloatProperty
        (
	        1.0f,
	        AnimationCurve.Linear (0.0f, 1.0f, 24.0f, 1.0f),
	        AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f),
	        AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f)
        );
		
        // Directional light color
        public AzureColorProperty directionalLightColor = new AzureColorProperty
        (
	        Color.white,
	        new Gradient(),
	        new Gradient(),
	        new Gradient()
        );
		
        // Ambient intensity
        public AzureFloatProperty environmentIntensity = new AzureFloatProperty
        (
	        1.0f,
	        AnimationCurve.Linear (0.0f, 1.0f, 24.0f, 1.0f),
	        AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f),
	        AnimationCurve.Linear (-1.0f, 1.0f, 1.0f, 1.0f)
        );
		
        // Ambient color
        public AzureColorProperty environmentAmbientColor = new AzureColorProperty
        (
	        Color.white,
	        new Gradient(),
	        new Gradient(),
	        new Gradient()
        );
		
        // Ambient equator color
        public AzureColorProperty environmentEquatorColor = new AzureColorProperty
        (
	        Color.white,
	        new Gradient(),
	        new Gradient(),
	        new Gradient()
        );
		
        // Ambient ground color
        public AzureColorProperty environmentGroundColor = new AzureColorProperty
        (
	        Color.white,
	        new Gradient(),
	        new Gradient(),
	        new Gradient()
        );
        
        // Light rain intensity
		public AzureFloatProperty lightRainIntensity = new AzureFloatProperty
		(
			0.0f,
			AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
		);
		
		// Medium rain intensity
		public AzureFloatProperty mediumRainIntensity = new AzureFloatProperty
		(
			0.0f,
			AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
		);
		
		// Heavy rain intensity
		public AzureFloatProperty heavyRainIntensity = new AzureFloatProperty
		(
			0.0f,
			AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
		);
		
		// Snow intensity
		public AzureFloatProperty snowIntensity = new AzureFloatProperty
		(
			0.0f,
			AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
		);
		
		// Rain color
		public AzureColorProperty rainColor = new AzureColorProperty
		(
			Color.white,
			new Gradient(),
			new Gradient(),
			new Gradient()
		);
		
		// Snow color
		public AzureColorProperty snowColor = new AzureColorProperty
		(
			Color.white,
			new Gradient(),
			new Gradient(),
			new Gradient()
		);
		
		// Light rain sound volume
		public AzureFloatProperty lightRainSoundVolume = new AzureFloatProperty
		(
			0.0f,
			AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
		);
		
		// Medium rain sound volume
		public AzureFloatProperty mediumRainSoundVolume = new AzureFloatProperty
		(
			0.0f,
			AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
		);
		
		// Heavy rain sound volume
		public AzureFloatProperty heavyRainSoundVolume = new AzureFloatProperty
		(
			0.0f,
			AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
		);
		
		// Light wind sound volume
		public AzureFloatProperty lightWindSoundVolume = new AzureFloatProperty
		(
			0.0f,
			AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
		);
		
		// Medium wind sound volume
		public AzureFloatProperty mediumWindSoundVolume = new AzureFloatProperty
		(
			0.0f,
			AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
		);
		
		// Heavy wind sound volume
		public AzureFloatProperty heavyWindSoundVolume = new AzureFloatProperty
		(
			0.0f,
			AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
		);
		
		// Wind speed
		public AzureFloatProperty windSpeed = new AzureFloatProperty
		(
			0.0f,
			AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
		);
		
		// Wind direction
		public AzureFloatProperty windDirection = new AzureFloatProperty
		(
			0.0f,
			AnimationCurve.Linear (0.0f, 0.0f, 24.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f),
			AnimationCurve.Linear (-1.0f, 0.0f, 1.0f, 0.0f)
		);
    }
}