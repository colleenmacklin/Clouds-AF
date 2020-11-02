using System;
using UnityEngine.Events;
using System.Collections.Generic;

namespace UnityEngine.AzureSky
{
    [ExecuteInEditMode]
    [AddComponentMenu("Azure[Sky]/Azure Sky Controller")]
    public class AzureSkyController : MonoBehaviour
    {
        // Not included in the build
        #if UNITY_EDITOR
        public bool showReferencesHeaderGroup = true;
        public bool showProfilesHeaderGroup = false;
        public bool showEventsHeaderGroup = false;
        public bool showOptionsHeaderGroup = false;
        public bool showOutputsHeaderGroup = false;
        #endif
        
        // References
        public Transform sunTransform;
        public Transform moonTransform;
        public Light directionalLight;
        public Material skyMaterial;
        public Material fogMaterial;
        public Shader emptySkyShader;
        public Shader staticCloudShader;
        public Shader dynamicCloudShader;
        
        // Sky settings
        public AzureSkySettings settings = new AzureSkySettings();
        public float timeOfDay;
        public float sunElevation;
        public float moonElevation;
        
        // Options
        public AzureScatteringMode scatteringMode = AzureScatteringMode.Automatic;
        public AzureCloudMode cloudMode = AzureCloudMode.EmptySky;
        public AzureShaderUpdateMode shaderUpdateMode = AzureShaderUpdateMode.Global;
        public Vector3 starFieldPosition = Vector3.zero;
        public Vector3 starFieldColor = Vector3.one;
        public float dayTransitionTime = 0.0f;
        public float mieDepth = 1.0f;

        // Profiles
        public AzureSkyProfile defaultProfile;
        public AzureSkyProfile currentProfile;
        public AzureSkyProfile targetProfile;
        private AzureSkyProfile m_nextDayProfile;
        
        // Lists
        public List<AzureSkyProfile> defaultProfileList = new List<AzureSkyProfile>();
        public List<AzureGlobalWeather> globalWeatherList = new List<AzureGlobalWeather>();
        public List<AzureWeatherZone> weatherZoneList = new List<AzureWeatherZone>();
        
        // Global weather transition
        public float globalWeatherTransitionProgress = 0.0f;
        public float globalWeatherTransitionTime = 0.0f;
        public float globalWeatherStartTransitionTime = 0.0f;
        public float defaultWeatherTransitionTime = 10.0f;
        public int globalWeatherIndex = -1;
        public bool isGlobalWeatherChanging = false;
        
        // Local weather zones
        public Transform weatherZoneTrigger;
        private Vector3 m_weatherZoneTriggerPosition;
        private Vector3 m_weatherZoneClosestPoint;
        private float m_weatherZoneClosestDistanceSqr;
        private float m_weatherZoneDistance;
        private float m_weatherZoneBlendDistanceSqr;
        private float m_weatherZoneInterpolationFactor;
        private Collider m_weatherZoneCollider;
        
        // Events
        public UnityEvent onMinuteChange = new UnityEvent();
        public UnityEvent onHourChange = new UnityEvent();
        public UnityEvent onDayChange = new UnityEvent();
        
        // Clouds utilities
        public Texture2D staticCloudSource;
        public Texture2D staticCloudTarget;
        private Vector2 m_dynamicCloudDirection;
        private float m_staticCloudLayer1Speed, m_staticCloudLayer2Speed;
        
        // Outputs
        public AzureOutputProfile outputProfile;
        private AzureOutputType m_outputType;

        private void OnEnable()
        {
            if (skyMaterial)
                RenderSettings.skybox = skyMaterial;
        }

        private void Start()
        {
            m_dynamicCloudDirection = Vector2.zero;
            globalWeatherIndex = -1;
            defaultProfile = defaultProfileList[0];
            currentProfile = defaultProfile;
            targetProfile = defaultProfile;
            UpdateMaterialSettings();

            // First update of the shader uniforms
            UpdateProfiles();
            if (shaderUpdateMode == AzureShaderUpdateMode.ByMaterial)
            {
                UpdateSkySettings(skyMaterial);
                UpdateSkySettings(fogMaterial);
            }
            else { UpdateSkySettings(); }
        }

        private void Update()
        {
            // Clouds movement
            m_dynamicCloudDirection = ComputeCloudPosition();
            m_staticCloudLayer1Speed += settings.StaticCloudLayer1Speed * Time.deltaTime;
            m_staticCloudLayer2Speed += settings.StaticCloudLayer2Speed * Time.deltaTime;
            if (m_staticCloudLayer1Speed >= 1.0f) { m_staticCloudLayer1Speed -= 1.0f; }
            if (m_staticCloudLayer2Speed >= 1.0f) { m_staticCloudLayer2Speed -= 1.0f; }
            skyMaterial.SetVector(AzureShaderUniforms.DynamicCloudDirection, m_dynamicCloudDirection);
            skyMaterial.SetFloat(AzureShaderUniforms.StaticCloudLayer1Speed, m_staticCloudLayer1Speed);
            skyMaterial.SetFloat(AzureShaderUniforms.StaticCloudLayer2Speed, m_staticCloudLayer2Speed);

            // Update shader uniforms
            UpdateProfiles();
            if (shaderUpdateMode == AzureShaderUpdateMode.ByMaterial)
            {
                UpdateSkySettings(skyMaterial);
                UpdateSkySettings(fogMaterial);
            }
            else { UpdateSkySettings(); }

            // Editor only
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                defaultProfile = defaultProfileList[0];
                currentProfile = defaultProfile;
            }
            if (skyMaterial)
                RenderSettings.skybox = skyMaterial;
            #endif
            
            // Environment lighting
            directionalLight.intensity = settings.DirectionalLightIntensity;
            directionalLight.color = settings.DirectionalLightColor;
            RenderSettings.ambientIntensity = settings.EnvironmentIntensity;
            RenderSettings.ambientLight = settings.EnvironmentAmbientColor;
            RenderSettings.ambientSkyColor = settings.EnvironmentAmbientColor;
            RenderSettings.ambientEquatorColor = settings.EnvironmentEquatorColor;
            RenderSettings.ambientGroundColor = settings.EnvironmentGroundColor;
        }

        /// <summary>
        /// Changes the global weather with a smooth transition.
        /// </summary>
        /// Set the index to -1 if you want to reset the global weather back to the default day profile.
        /// <param name="index">The target profile number in the "global weather profiles" list.</param>
        public void SetNewWeatherProfile(int index)
        {
            switch (index)
            {
                // Back to the default day profile currently in use by sky manager
                case -1:
                    if (defaultProfile)
                    {
                        targetProfile = defaultProfile;
                        globalWeatherTransitionTime = defaultWeatherTransitionTime;
                        globalWeatherIndex = index;
                    }
                    break;
				
                // Changes the global weather to the corresponding profile index in the global weather list
                default:
                    if (globalWeatherList[index].profile)
                    {
                        targetProfile = globalWeatherList[index].profile;
                        globalWeatherTransitionTime = globalWeatherList[index].transitionTime;
                        globalWeatherIndex = index;
                    }
                    break;
            }
			
            // Starts the global weather transition progress
            globalWeatherTransitionProgress = 0.0f;
            globalWeatherStartTransitionTime = Time.time;
            isGlobalWeatherChanging = true;
        }
        
        /// <summary>
        /// Performs the default profile transition when the time changes to the next calendar day at 24 o'clock.
        /// </summary>
        public void PerformDayTransition()
        {
            // Make the transition to the next day only if the profile is different from the current profile and there is no weather profile in use.
            m_nextDayProfile = defaultProfileList[Random.Range(0, defaultProfileList.Count)];
            defaultProfile = m_nextDayProfile;
            if (m_nextDayProfile != currentProfile && globalWeatherIndex < 0)
            {
                if (dayTransitionTime > 0)
                {
                    SetNewDayProfile(m_nextDayProfile, dayTransitionTime);
                }
                else
                {
                    SetNewDayProfile(m_nextDayProfile);
                }
            }
        }
        
        public void OnDayChange()
        {
            onDayChange?.Invoke();
            PerformDayTransition();
        }
        
        /// <summary>
        /// Changes the current day profile with transition.
        /// </summary>
        public void SetNewDayProfile(AzureSkyProfile profile, float transitionTime)
        {
            targetProfile = profile;
            globalWeatherTransitionTime = transitionTime;
						
            // Starts the global weather transition progress
            globalWeatherTransitionProgress = 0.0f;
            globalWeatherStartTransitionTime = Time.time;
            isGlobalWeatherChanging = true;
        }
		
        /// <summary>
        /// Changes the current day profile without transition.
        /// </summary>
        public void SetNewDayProfile(AzureSkyProfile profile)
        {
            currentProfile = profile;
        }
        
        public float GetOutputFloatValue(int index)
        {
            if (outputProfile)
            {
                m_outputType = outputProfile.outputList[index].type;
                if (m_outputType == AzureOutputType.Slider || m_outputType == AzureOutputType.TimelineCurve || m_outputType == AzureOutputType.SunCurve || m_outputType == AzureOutputType.MoonCurve)
                {
                    return outputProfile.outputList[index].floatOutput;
                }
                else
                {
                    Debug.LogWarning("You are trying to get a float output, but the output type is set to " + m_outputType);
                }
            }
            
            return 0.0f;
        }

        public Color GetOutputColorValue(int index)
        {
            if (outputProfile)
            {
                m_outputType = outputProfile.outputList[index].type;
                if (m_outputType == AzureOutputType.Color || m_outputType == AzureOutputType.TimelineGradient || m_outputType == AzureOutputType.SunGradient || m_outputType == AzureOutputType.MoonGradient)
                {
                    return outputProfile.outputList[index].colorOutput;
                }
                else
                {
                    Debug.LogWarning("You are trying to get a color output, but the output type is set to " + m_outputType);
                }
            }
            
            return Color.black;
        }

        public void UpdateMaterialSettings()
        {
            switch (cloudMode)
            {
                case AzureCloudMode.EmptySky:
                    skyMaterial.shader = emptySkyShader;
                    break;
                
                case AzureCloudMode.StaticClouds:
                    skyMaterial.shader = staticCloudShader;
                    break;
                
                case AzureCloudMode.DynamicClouds:
                    skyMaterial.shader = dynamicCloudShader;
                    break;
            }
        }
        
        public void UpdateSkySettings(Material mat)
        {
            // Scattering
            mat.SetInt(AzureShaderUniforms.ScatteringMode, (int) scatteringMode);
            mat.SetVector(AzureShaderUniforms.Rayleigh, ComputeRayleigh() * settings.Rayleigh);
            mat.SetVector(AzureShaderUniforms.Mie, ComputeMie() * settings.Mie);
            mat.SetFloat(AzureShaderUniforms.Scattering, settings.Scattering * 60.0f);
            mat.SetFloat(AzureShaderUniforms.Luminance, settings.Luminance);
            mat.SetFloat(AzureShaderUniforms.Exposure, settings.Exposure);
            mat.SetVector(AzureShaderUniforms.RayleighColor, settings.RayleighColor);
            mat.SetVector(AzureShaderUniforms.MieColor, settings.MieColor);
            mat.SetVector(AzureShaderUniforms.ScatteringColor, settings.ScatteringColor);

            // Outer Space
            mat.SetFloat(AzureShaderUniforms.SunTextureSize, settings.SunTextureSize);
            mat.SetFloat(AzureShaderUniforms.SunTextureIntensity, settings.SunTextureIntensity);
            mat.SetVector(AzureShaderUniforms.SunTextureColor, settings.SunTextureColor);
            mat.SetFloat(AzureShaderUniforms.MoonTextureSize, settings.MoonTextureSize);
            mat.SetFloat(AzureShaderUniforms.MoonTextureIntensity, settings.MoonTextureIntensity);
            mat.SetVector(AzureShaderUniforms.MoonTextureColor, settings.MoonTextureColor);
            mat.SetFloat(AzureShaderUniforms.StarsIntensity, settings.StarsIntensity);
            mat.SetFloat(AzureShaderUniforms.MilkyWayIntensity, settings.MilkyWayIntensity);
            mat.SetVector(AzureShaderUniforms.StarFieldColor, starFieldColor);

            // Fog scattering
            mat.SetFloat(AzureShaderUniforms.FogScatteringScale, settings.FogScatteringScale);
            mat.SetFloat(AzureShaderUniforms.GlobalFogDistance, settings.GlobalFogDistance);
            mat.SetFloat(AzureShaderUniforms.GlobalFogSmooth, settings.GlobalFogSmooth);
            mat.SetFloat(AzureShaderUniforms.GlobalFogDensity, settings.GlobalFogDensity);
            mat.SetFloat(AzureShaderUniforms.HeightFogDistance, settings.HeightFogDistance);
            mat.SetFloat(AzureShaderUniforms.HeightFogSmooth, settings.HeightFogSmooth);
            mat.SetFloat(AzureShaderUniforms.HeightFogDensity, settings.HeightFogDensity);
            mat.SetFloat(AzureShaderUniforms.HeightFogStart, settings.HeightFogStart);
            mat.SetFloat(AzureShaderUniforms.HeightFogEnd, settings.HeightFogEnd);
            mat.SetFloat(AzureShaderUniforms.MieDepth, mieDepth);

            // Clouds
            mat.SetTexture(AzureShaderUniforms.StaticCloudSourceTexture, staticCloudSource);
            mat.SetTexture(AzureShaderUniforms.StaticCloudTargetTexture, staticCloudTarget);
            mat.SetFloat(AzureShaderUniforms.StaticCloudInterpolator, settings.StaticCloudInterpolator);
            mat.SetFloat(AzureShaderUniforms.StaticCloudScattering, settings.StaticCloudScattering);
            mat.SetFloat(AzureShaderUniforms.StaticCloudExtinction, settings.StaticCloudExtinction);
            mat.SetFloat(AzureShaderUniforms.StaticCloudSaturation, settings.StaticCloudSaturation);
            mat.SetFloat(AzureShaderUniforms.StaticCloudOpacity, settings.StaticCloudOpacity);
            mat.SetVector(AzureShaderUniforms.StaticCloudColor, settings.StaticCloudColor);
            mat.SetFloat(AzureShaderUniforms.DynamicCloudAltitude, settings.DynamicCloudAltitude);
            mat.SetFloat(AzureShaderUniforms.DynamicCloudDensity, Mathf.Lerp (25.0f, 0.0f, settings.DynamicCloudDensity));
            mat.SetVector(AzureShaderUniforms.DynamicCloudColor1, settings.DynamicCloudColor1);
            mat.SetVector(AzureShaderUniforms.DynamicCloudColor2, settings.DynamicCloudColor2);
        }

        public void UpdateSkySettings()
        {
            // Scattering
            Shader.SetGlobalInt(AzureShaderUniforms.ScatteringMode, (int)scatteringMode);
            Shader.SetGlobalVector(AzureShaderUniforms.Rayleigh, ComputeRayleigh() * settings.Rayleigh);
            Shader.SetGlobalVector(AzureShaderUniforms.Mie, ComputeMie() * settings.Mie);
            Shader.SetGlobalFloat(AzureShaderUniforms.Scattering, settings.Scattering * 60.0f);
            Shader.SetGlobalFloat(AzureShaderUniforms.Luminance, settings.Luminance);
            Shader.SetGlobalFloat(AzureShaderUniforms.Exposure, settings.Exposure);
            Shader.SetGlobalVector(AzureShaderUniforms.RayleighColor, settings.RayleighColor);
            Shader.SetGlobalVector(AzureShaderUniforms.MieColor, settings.MieColor);
            Shader.SetGlobalVector(AzureShaderUniforms.ScatteringColor, settings.ScatteringColor);

            // Outer Space
            Shader.SetGlobalFloat(AzureShaderUniforms.SunTextureSize, settings.SunTextureSize);
            Shader.SetGlobalFloat(AzureShaderUniforms.SunTextureIntensity, settings.SunTextureIntensity);
            Shader.SetGlobalVector(AzureShaderUniforms.SunTextureColor, settings.SunTextureColor);
            Shader.SetGlobalFloat(AzureShaderUniforms.MoonTextureSize, settings.MoonTextureSize);
            Shader.SetGlobalFloat(AzureShaderUniforms.MoonTextureIntensity, settings.MoonTextureIntensity);
            Shader.SetGlobalVector(AzureShaderUniforms.MoonTextureColor, settings.MoonTextureColor);
            Shader.SetGlobalFloat(AzureShaderUniforms.StarsIntensity, settings.StarsIntensity);
            Shader.SetGlobalFloat(AzureShaderUniforms.MilkyWayIntensity, settings.MilkyWayIntensity);
            Shader.SetGlobalVector(AzureShaderUniforms.StarFieldColor, starFieldColor);

            // Fog scattering
            Shader.SetGlobalFloat(AzureShaderUniforms.FogScatteringScale, settings.FogScatteringScale);
            Shader.SetGlobalFloat(AzureShaderUniforms.GlobalFogDistance, settings.GlobalFogDistance);
            Shader.SetGlobalFloat(AzureShaderUniforms.GlobalFogSmooth, settings.GlobalFogSmooth);
            Shader.SetGlobalFloat(AzureShaderUniforms.GlobalFogDensity, settings.GlobalFogDensity);
            Shader.SetGlobalFloat(AzureShaderUniforms.HeightFogDistance, settings.HeightFogDistance);
            Shader.SetGlobalFloat(AzureShaderUniforms.HeightFogSmooth, settings.HeightFogSmooth);
            Shader.SetGlobalFloat(AzureShaderUniforms.HeightFogDensity, settings.HeightFogDensity);
            Shader.SetGlobalFloat(AzureShaderUniforms.HeightFogStart, settings.HeightFogStart);
            Shader.SetGlobalFloat(AzureShaderUniforms.HeightFogEnd, settings.HeightFogEnd);
            Shader.SetGlobalFloat(AzureShaderUniforms.MieDepth, mieDepth);

            // Clouds
            Shader.SetGlobalTexture(AzureShaderUniforms.StaticCloudSourceTexture, staticCloudSource);
            Shader.SetGlobalTexture(AzureShaderUniforms.StaticCloudTargetTexture, staticCloudTarget);
            Shader.SetGlobalFloat(AzureShaderUniforms.StaticCloudInterpolator, settings.StaticCloudInterpolator);
            Shader.SetGlobalFloat(AzureShaderUniforms.StaticCloudScattering, settings.StaticCloudScattering);
            Shader.SetGlobalFloat(AzureShaderUniforms.StaticCloudExtinction, settings.StaticCloudExtinction);
            Shader.SetGlobalFloat(AzureShaderUniforms.StaticCloudSaturation, settings.StaticCloudSaturation);
            Shader.SetGlobalFloat(AzureShaderUniforms.StaticCloudOpacity, settings.StaticCloudOpacity);
            Shader.SetGlobalVector(AzureShaderUniforms.StaticCloudColor, settings.StaticCloudColor);
            Shader.SetGlobalFloat(AzureShaderUniforms.DynamicCloudAltitude, settings.DynamicCloudAltitude);
            Shader.SetGlobalFloat(AzureShaderUniforms.DynamicCloudDensity, Mathf.Lerp(25.0f, 0.0f, settings.DynamicCloudDensity));
            Shader.SetGlobalVector(AzureShaderUniforms.DynamicCloudColor1, settings.DynamicCloudColor1);
            Shader.SetGlobalVector(AzureShaderUniforms.DynamicCloudColor2, settings.DynamicCloudColor2);
        }

        /// <summary>
        /// Gets the current sky setting if there is no global weather transition or local weather zone influence.
        /// </summary>
        /// Computes the global weather transition.
        /// <summary>
        /// Computes local weather zones influence.
        /// </summary>
        /// Returns the final sky setting resulting from the profile blending process.
        private void UpdateProfiles()
        {
            if (!isGlobalWeatherChanging)
            {
                // Gets the current sky setting when there is no global weather transition or local weather zone influence
                GetDefaultSettings();
            }
            else
            {
                // Runs the global weather transition
                globalWeatherTransitionProgress = Mathf.Clamp01((Time.time - globalWeatherStartTransitionTime) / globalWeatherTransitionTime);

                // Performs the global weather blend
                ApplyGlobalWeatherTransition(currentProfile, targetProfile, globalWeatherTransitionProgress);
                
                // Ends the global weather transition
                if (Math.Abs(globalWeatherTransitionProgress - 1.0f) <= 0.0f)
                {
                    isGlobalWeatherChanging = false;
                    globalWeatherTransitionProgress = 0.0f;
                    globalWeatherStartTransitionTime = 0.0f;
                    currentProfile = targetProfile;
                }
            }

            // Computes weather zones influence
            // Based on Unity's Post Processing v2
            if (!weatherZoneTrigger)
                return;
            
            m_weatherZoneTriggerPosition = weatherZoneTrigger.position;
                
            // Traverse all weather zones in the weather zone list
            foreach (var weatherZone in weatherZoneList)
            {
                // Skip if the list index is null
                if (weatherZone == null)
                    continue;
                    
                // If weather zone has no collider, skip it as it's useless
                m_weatherZoneCollider = weatherZone.GetComponent<Collider>();
                if (!m_weatherZoneCollider)
                    continue;
                    
                if (!m_weatherZoneCollider.enabled)
                    continue;
                    
                // Find closest distance to weather zone, 0 means it's inside it
                m_weatherZoneClosestDistanceSqr = float.PositiveInfinity;

                m_weatherZoneClosestPoint = m_weatherZoneCollider.ClosestPoint(m_weatherZoneTriggerPosition); // 5.6-only API
                m_weatherZoneDistance = ((m_weatherZoneClosestPoint - m_weatherZoneTriggerPosition) / 2f).sqrMagnitude;

                if (m_weatherZoneDistance < m_weatherZoneClosestDistanceSqr)
                    m_weatherZoneClosestDistanceSqr = m_weatherZoneDistance;

                m_weatherZoneCollider = null;
                m_weatherZoneBlendDistanceSqr = weatherZone.blendDistance * weatherZone.blendDistance;
                    
                // Weather zone has no influence, ignore it
                // Note: Weather zone doesn't do anything when `closestDistanceSqr = blendDistSqr` but
                //       we can't use a >= comparison as blendDistSqr could be set to 0 in which
                //       case weather zone would have total influence
                if (m_weatherZoneClosestDistanceSqr > m_weatherZoneBlendDistanceSqr)
                    continue;
                    
                // Weather zone has influence
                m_weatherZoneInterpolationFactor = 1f;

                if (m_weatherZoneBlendDistanceSqr > 0f)
                    m_weatherZoneInterpolationFactor = 1f - (m_weatherZoneClosestDistanceSqr / m_weatherZoneBlendDistanceSqr);

                // No need to clamp01 the interpolation factor as it'll always be in [0;1[ range
                ApplyWeatherZonesInfluence(weatherZone.profile, m_weatherZoneInterpolationFactor);
            }
        }
        
        /// <summary>
        /// Update the sky setting when there is no global weather transition or local weather zone influence.
        /// </summary>
        private void GetDefaultSettings()
        {
            // Scattering
            settings.MolecularDensity = currentProfile.molecularDensity.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.Wavelength.x = currentProfile.wavelengthR.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.Wavelength.y = currentProfile.wavelengthG.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.Wavelength.z = currentProfile.wavelengthB.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.Rayleigh = currentProfile.rayleigh.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.Mie = currentProfile.mie.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.Scattering = currentProfile.scattering.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.Luminance = currentProfile.luminance.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.Exposure = currentProfile.exposure.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.RayleighColor = currentProfile.rayleighColor.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.MieColor = currentProfile.mieColor.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.ScatteringColor = currentProfile.scatteringColor.GetValue(timeOfDay, sunElevation, moonElevation);
            
            // Outer space
            settings.SunTextureSize = currentProfile.sunTextureSize.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.SunTextureIntensity = currentProfile.sunTextureIntensity.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.SunTextureColor = currentProfile.sunTextureColor.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.MoonTextureSize = currentProfile.moonTextureSize.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.MoonTextureIntensity = currentProfile.moonTextureIntensity.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.MoonTextureColor = currentProfile.moonTextureColor.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.StarsIntensity = currentProfile.starsIntensity.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.MilkyWayIntensity = currentProfile.milkyWayIntensity.GetValue(timeOfDay, sunElevation, moonElevation);
            
            // Fog scattering
            settings.FogScatteringScale = currentProfile.fogScatteringScale.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.GlobalFogDistance = currentProfile.globalFogDistance.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.GlobalFogSmooth = currentProfile.globalFogSmooth.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.GlobalFogDensity = currentProfile.globalFogDensity.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.HeightFogDistance = currentProfile.heightFogDistance.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.HeightFogSmooth = currentProfile.heightFogSmooth.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.HeightFogDensity = currentProfile.heightFogDensity.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.HeightFogStart = currentProfile.heightFogStart.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.HeightFogEnd = currentProfile.heightFogEnd.GetValue(timeOfDay, sunElevation, moonElevation);

            // Clouds
            settings.StaticCloudInterpolator = 0.0f;
            staticCloudSource = currentProfile.staticCloudTexture;
            staticCloudTarget = currentProfile.staticCloudTexture;
            settings.StaticCloudLayer1Speed = currentProfile.staticCloudLayer1Speed.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.StaticCloudLayer2Speed = currentProfile.staticCloudLayer2Speed.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.StaticCloudColor = currentProfile.staticCloudColor.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.StaticCloudScattering = currentProfile.staticCloudScattering.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.StaticCloudExtinction = currentProfile.staticCloudExtinction.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.StaticCloudSaturation = currentProfile.staticCloudSaturation.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.StaticCloudOpacity = currentProfile.staticCloudOpacity.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.DynamicCloudAltitude = currentProfile.dynamicCloudAltitude.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.DynamicCloudDirection = currentProfile.dynamicCloudDirection.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.DynamicCloudSpeed = currentProfile.dynamicCloudSpeed.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.DynamicCloudDensity = currentProfile.dynamicCloudDensity.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.DynamicCloudColor1 = currentProfile.dynamicCloudColor1.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.DynamicCloudColor2 = currentProfile.dynamicCloudColor2.GetValue(timeOfDay, sunElevation, moonElevation);
            
            // Lighting
            settings.DirectionalLightIntensity = currentProfile.directionalLightIntensity.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.DirectionalLightColor = currentProfile.directionalLightColor.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.EnvironmentIntensity = currentProfile.environmentIntensity.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.EnvironmentAmbientColor = currentProfile.environmentAmbientColor.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.EnvironmentEquatorColor = currentProfile.environmentEquatorColor.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.EnvironmentGroundColor = currentProfile.environmentGroundColor.GetValue(timeOfDay, sunElevation, moonElevation);
            
            // Weather
            settings.LightRainIntensity = currentProfile.lightRainIntensity.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.MediumRainIntensity = currentProfile.mediumRainIntensity.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.HeavyRainIntensity = currentProfile.heavyRainIntensity.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.SnowIntensity = currentProfile.snowIntensity.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.RainColor = currentProfile.rainColor.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.SnowColor = currentProfile.snowColor.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.LightRainSoundVolume = currentProfile.lightRainSoundVolume.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.MediumRainSoundVolume = currentProfile.mediumRainSoundVolume.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.HeavyRainSoundVolume = currentProfile.heavyRainSoundVolume.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.LightWindSoundVolume = currentProfile.lightWindSoundVolume.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.MediumWindSoundVolume = currentProfile.mediumWindSoundVolume.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.HeavyWindSoundVolume = currentProfile.heavyWindSoundVolume.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.WindSpeed = currentProfile.windSpeed.GetValue(timeOfDay, sunElevation, moonElevation);
            settings.WindDirection = currentProfile.windDirection.GetValue(timeOfDay, sunElevation, moonElevation);
            
            // Outputs
            if (!outputProfile || !currentProfile.outputProfile) return;
            if (outputProfile != currentProfile.outputProfile || outputProfile.outputList.Count <= 0) return;
            for (int i = 0; i < outputProfile.outputList.Count; i++)
            {
                m_outputType = outputProfile.outputList[i].type;
                if (m_outputType == AzureOutputType.Slider || m_outputType == AzureOutputType.TimelineCurve || m_outputType == AzureOutputType.SunCurve || m_outputType == AzureOutputType.MoonCurve)
                {
                    outputProfile.outputList[i].floatOutput = currentProfile.outputPropertyList[i].GetFloatValue(timeOfDay, sunElevation, moonElevation);
                }
                else
                {
                    outputProfile.outputList[i].colorOutput = currentProfile.outputPropertyList[i].GetColorValue(timeOfDay, sunElevation, moonElevation);
                }
            }
        }
        
        /// <summary>
        /// Blends the profiles when there is a global weather transition.
        /// </summary>
        private void ApplyGlobalWeatherTransition(AzureSkyProfile from, AzureSkyProfile to, float t)
        {
            // Scattering
            settings.MolecularDensity = FloatInterpolation(from.molecularDensity.GetValue(timeOfDay, sunElevation, moonElevation), to.molecularDensity.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.Wavelength.x = FloatInterpolation(from.wavelengthR.GetValue(timeOfDay, sunElevation, moonElevation), to.wavelengthR.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.Wavelength.y = FloatInterpolation(from.wavelengthG.GetValue(timeOfDay, sunElevation, moonElevation), to.wavelengthG.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.Wavelength.z = FloatInterpolation(from.wavelengthB.GetValue(timeOfDay, sunElevation, moonElevation), to.wavelengthB.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.Rayleigh = FloatInterpolation(from.rayleigh.GetValue(timeOfDay, sunElevation, moonElevation), to.rayleigh.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.Mie = FloatInterpolation(from.mie.GetValue(timeOfDay, sunElevation, moonElevation), to.mie.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.Scattering = FloatInterpolation(from.scattering.GetValue(timeOfDay, sunElevation, moonElevation), to.scattering.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.Luminance = FloatInterpolation(from.luminance.GetValue(timeOfDay, sunElevation, moonElevation), to.luminance.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.Exposure = FloatInterpolation(from.exposure.GetValue(timeOfDay, sunElevation, moonElevation), to.exposure.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.RayleighColor = ColorInterpolation(from.rayleighColor.GetValue(timeOfDay, sunElevation, moonElevation), to.rayleighColor.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.MieColor = ColorInterpolation(from.mieColor.GetValue(timeOfDay, sunElevation, moonElevation), to.mieColor.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.ScatteringColor = ColorInterpolation(from.scatteringColor.GetValue(timeOfDay, sunElevation, moonElevation), to.scatteringColor.GetValue(timeOfDay, sunElevation, moonElevation), t);
            
            // Outer space
            settings.SunTextureSize = FloatInterpolation(from.sunTextureSize.GetValue(timeOfDay, sunElevation, moonElevation), to.sunTextureSize.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.SunTextureIntensity = FloatInterpolation(from.sunTextureIntensity.GetValue(timeOfDay, sunElevation, moonElevation), to.sunTextureIntensity.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.SunTextureColor = ColorInterpolation(from.sunTextureColor.GetValue(timeOfDay, sunElevation, moonElevation), to.sunTextureColor.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.MoonTextureSize = FloatInterpolation(from.moonTextureSize.GetValue(timeOfDay, sunElevation, moonElevation), to.moonTextureSize.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.MoonTextureIntensity = FloatInterpolation(from.moonTextureIntensity.GetValue(timeOfDay, sunElevation, moonElevation), to.moonTextureIntensity.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.MoonTextureColor = ColorInterpolation(from.moonTextureColor.GetValue(timeOfDay, sunElevation, moonElevation), to.moonTextureColor.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.StarsIntensity = FloatInterpolation(from.starsIntensity.GetValue(timeOfDay, sunElevation, moonElevation), to.starsIntensity.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.MilkyWayIntensity = FloatInterpolation(from.milkyWayIntensity.GetValue(timeOfDay, sunElevation, moonElevation), to.milkyWayIntensity.GetValue(timeOfDay, sunElevation, moonElevation), t);
            
            // Fog scattering
            settings.FogScatteringScale = FloatInterpolation(from.fogScatteringScale.GetValue(timeOfDay, sunElevation, moonElevation), to.fogScatteringScale.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.GlobalFogDistance = FloatInterpolation(from.globalFogDistance.GetValue(timeOfDay, sunElevation, moonElevation), to.globalFogDistance.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.GlobalFogSmooth = FloatInterpolation(from.globalFogSmooth.GetValue(timeOfDay, sunElevation, moonElevation), to.globalFogSmooth.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.GlobalFogDensity = FloatInterpolation(from.globalFogDensity.GetValue(timeOfDay, sunElevation, moonElevation), to.globalFogDensity.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.HeightFogDistance = FloatInterpolation(from.heightFogDistance.GetValue(timeOfDay, sunElevation, moonElevation), to.heightFogDistance.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.HeightFogSmooth = FloatInterpolation(from.heightFogSmooth.GetValue(timeOfDay, sunElevation, moonElevation), to.heightFogSmooth.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.HeightFogDensity = FloatInterpolation(from.heightFogDensity.GetValue(timeOfDay, sunElevation, moonElevation), to.heightFogDensity.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.HeightFogStart = FloatInterpolation(from.heightFogStart.GetValue(timeOfDay, sunElevation, moonElevation), to.heightFogStart.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.HeightFogEnd = FloatInterpolation(from.heightFogEnd.GetValue(timeOfDay, sunElevation, moonElevation), to.heightFogEnd.GetValue(timeOfDay, sunElevation, moonElevation), t);
            
            // Clouds
            settings.StaticCloudInterpolator = globalWeatherTransitionProgress;
            staticCloudSource = currentProfile.staticCloudTexture;
            staticCloudTarget = targetProfile.staticCloudTexture;
            settings.StaticCloudLayer1Speed = FloatInterpolation(from.staticCloudLayer1Speed.GetValue(timeOfDay, sunElevation, moonElevation), to.staticCloudLayer1Speed.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.StaticCloudLayer2Speed = FloatInterpolation(from.staticCloudLayer2Speed.GetValue(timeOfDay, sunElevation, moonElevation), to.staticCloudLayer2Speed.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.StaticCloudColor = ColorInterpolation(from.staticCloudColor.GetValue(timeOfDay, sunElevation, moonElevation), to.staticCloudColor.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.StaticCloudScattering = FloatInterpolation(from.staticCloudScattering.GetValue(timeOfDay, sunElevation, moonElevation), to.staticCloudScattering.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.StaticCloudExtinction = FloatInterpolation(from.staticCloudExtinction.GetValue(timeOfDay, sunElevation, moonElevation), to.staticCloudExtinction.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.StaticCloudSaturation = FloatInterpolation(from.staticCloudSaturation.GetValue(timeOfDay, sunElevation, moonElevation), to.staticCloudSaturation.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.StaticCloudOpacity = FloatInterpolation(from.staticCloudOpacity.GetValue(timeOfDay, sunElevation, moonElevation), to.staticCloudOpacity.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.DynamicCloudAltitude = FloatInterpolation(from.dynamicCloudAltitude.GetValue(timeOfDay, sunElevation, moonElevation), to.dynamicCloudAltitude.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.DynamicCloudDirection = FloatInterpolation(from.dynamicCloudDirection.GetValue(timeOfDay, sunElevation, moonElevation), to.dynamicCloudDirection.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.DynamicCloudSpeed = FloatInterpolation(from.dynamicCloudSpeed.GetValue(timeOfDay, sunElevation, moonElevation), to.dynamicCloudSpeed.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.DynamicCloudDensity = FloatInterpolation(from.dynamicCloudDensity.GetValue(timeOfDay, sunElevation, moonElevation), to.dynamicCloudDensity.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.DynamicCloudColor1 = ColorInterpolation(from.dynamicCloudColor1.GetValue(timeOfDay, sunElevation, moonElevation), to.dynamicCloudColor1.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.DynamicCloudColor2 = ColorInterpolation(from.dynamicCloudColor2.GetValue(timeOfDay, sunElevation, moonElevation), to.dynamicCloudColor2.GetValue(timeOfDay, sunElevation, moonElevation), t);
            
            // Lighting
            settings.DirectionalLightIntensity = FloatInterpolation(from.directionalLightIntensity.GetValue(timeOfDay, sunElevation, moonElevation), to.directionalLightIntensity.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.DirectionalLightColor = ColorInterpolation(from.directionalLightColor.GetValue(timeOfDay, sunElevation, moonElevation), to.directionalLightColor.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.EnvironmentIntensity = FloatInterpolation(from.environmentIntensity.GetValue(timeOfDay, sunElevation, moonElevation), to.environmentIntensity.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.EnvironmentAmbientColor = ColorInterpolation(from.environmentAmbientColor.GetValue(timeOfDay, sunElevation, moonElevation), to.environmentAmbientColor.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.EnvironmentEquatorColor = ColorInterpolation(from.environmentEquatorColor.GetValue(timeOfDay, sunElevation, moonElevation), to.environmentEquatorColor.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.EnvironmentGroundColor = ColorInterpolation(from.environmentGroundColor.GetValue(timeOfDay, sunElevation, moonElevation), to.environmentGroundColor.GetValue(timeOfDay, sunElevation, moonElevation), t);
            
            // Weather
            settings.LightRainIntensity = FloatInterpolation(from.lightRainIntensity.GetValue(timeOfDay, sunElevation, moonElevation), to.lightRainIntensity.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.MediumRainIntensity = FloatInterpolation(from.mediumRainIntensity.GetValue(timeOfDay, sunElevation, moonElevation), to.mediumRainIntensity.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.HeavyRainIntensity = FloatInterpolation(from.heavyRainIntensity.GetValue(timeOfDay, sunElevation, moonElevation), to.heavyRainIntensity.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.SnowIntensity = FloatInterpolation(from.snowIntensity.GetValue(timeOfDay, sunElevation, moonElevation), to.snowIntensity.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.RainColor = ColorInterpolation(from.rainColor.GetValue(timeOfDay, sunElevation, moonElevation), to.rainColor.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.SnowColor = ColorInterpolation(from.snowColor.GetValue(timeOfDay, sunElevation, moonElevation), to.snowColor.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.LightRainSoundVolume = FloatInterpolation(from.lightRainSoundVolume.GetValue(timeOfDay, sunElevation, moonElevation), to.lightRainSoundVolume.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.MediumRainSoundVolume = FloatInterpolation(from.mediumRainSoundVolume.GetValue(timeOfDay, sunElevation, moonElevation), to.mediumRainSoundVolume.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.HeavyRainSoundVolume = FloatInterpolation(from.heavyRainSoundVolume.GetValue(timeOfDay, sunElevation, moonElevation), to.heavyRainSoundVolume.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.LightWindSoundVolume = FloatInterpolation(from.lightWindSoundVolume.GetValue(timeOfDay, sunElevation, moonElevation), to.lightWindSoundVolume.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.MediumWindSoundVolume = FloatInterpolation(from.mediumWindSoundVolume.GetValue(timeOfDay, sunElevation, moonElevation), to.mediumWindSoundVolume.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.HeavyWindSoundVolume = FloatInterpolation(from.heavyWindSoundVolume.GetValue(timeOfDay, sunElevation, moonElevation), to.heavyWindSoundVolume.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.WindSpeed = FloatInterpolation(from.windSpeed.GetValue(timeOfDay, sunElevation, moonElevation), to.windSpeed.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.WindDirection = FloatInterpolation(from.windDirection.GetValue(timeOfDay, sunElevation, moonElevation), to.windDirection.GetValue(timeOfDay, sunElevation, moonElevation), t);
            
            // Outputs
            if (!outputProfile || !from.outputProfile || !to.outputProfile) return;
            if (outputProfile != from.outputProfile || outputProfile != to.outputProfile || outputProfile.outputList.Count <= 0) return;
            for (int i = 0; i < outputProfile.outputList.Count; i++)
            {
                m_outputType = outputProfile.outputList[i].type;
                if (m_outputType == AzureOutputType.Slider || m_outputType == AzureOutputType.TimelineCurve || m_outputType == AzureOutputType.SunCurve || m_outputType == AzureOutputType.MoonCurve)
                {
                    outputProfile.outputList[i].floatOutput = FloatInterpolation(from.outputPropertyList[i].GetFloatValue(timeOfDay, sunElevation, moonElevation), to.outputPropertyList[i].GetFloatValue(timeOfDay, sunElevation, moonElevation), t);
                }
                else
                {
                    outputProfile.outputList[i].colorOutput = ColorInterpolation(from.outputPropertyList[i].GetColorValue(timeOfDay, sunElevation, moonElevation), to.outputPropertyList[i].GetColorValue(timeOfDay, sunElevation, moonElevation), t);
                }
            }
        }
        
        /// <summary>
        /// Computes local weather zones influence.
        /// </summary>
        private void ApplyWeatherZonesInfluence(AzureSkyProfile climateZoneProfile, float t)
        {
            // Scattering
            settings.MolecularDensity = FloatInterpolation(settings.MolecularDensity, climateZoneProfile.molecularDensity.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.Wavelength.x = FloatInterpolation(settings.Wavelength.x, climateZoneProfile.wavelengthR.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.Wavelength.y = FloatInterpolation(settings.Wavelength.y, climateZoneProfile.wavelengthG.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.Wavelength.z = FloatInterpolation(settings.Wavelength.z, climateZoneProfile.wavelengthB.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.Rayleigh = FloatInterpolation(settings.Rayleigh, climateZoneProfile.rayleigh.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.Mie = FloatInterpolation(settings.Mie, climateZoneProfile.mie.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.Scattering = FloatInterpolation(settings.Scattering, climateZoneProfile.scattering.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.Luminance = FloatInterpolation(settings.Luminance, climateZoneProfile.luminance.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.Exposure = FloatInterpolation(settings.Exposure, climateZoneProfile.exposure.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.RayleighColor = ColorInterpolation(settings.RayleighColor, climateZoneProfile.rayleighColor.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.MieColor = ColorInterpolation(settings.MieColor, climateZoneProfile.mieColor.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.ScatteringColor = ColorInterpolation(settings.ScatteringColor, climateZoneProfile.scatteringColor.GetValue(timeOfDay, sunElevation, moonElevation), t);
            
            // Outer space
            settings.SunTextureSize = FloatInterpolation(settings.SunTextureSize, climateZoneProfile.sunTextureSize.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.SunTextureIntensity = FloatInterpolation(settings.SunTextureIntensity, climateZoneProfile.sunTextureIntensity.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.SunTextureColor = ColorInterpolation(settings.SunTextureColor, climateZoneProfile.sunTextureColor.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.MoonTextureSize = FloatInterpolation(settings.MoonTextureSize, climateZoneProfile.moonTextureSize.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.MoonTextureIntensity = FloatInterpolation(settings.MoonTextureIntensity, climateZoneProfile.moonTextureIntensity.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.MoonTextureColor = ColorInterpolation(settings.MoonTextureColor, climateZoneProfile.moonTextureColor.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.StarsIntensity = FloatInterpolation(settings.StarsIntensity, climateZoneProfile.starsIntensity.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.MilkyWayIntensity = FloatInterpolation(settings.MilkyWayIntensity, climateZoneProfile.milkyWayIntensity.GetValue(timeOfDay, sunElevation, moonElevation), t);
            
            // Fog scattering
            settings.FogScatteringScale = FloatInterpolation(settings.FogScatteringScale, climateZoneProfile.fogScatteringScale.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.GlobalFogDistance = FloatInterpolation(settings.GlobalFogDistance, climateZoneProfile.globalFogDistance.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.GlobalFogSmooth = FloatInterpolation(settings.GlobalFogSmooth, climateZoneProfile.globalFogSmooth.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.GlobalFogDensity = FloatInterpolation(settings.GlobalFogDensity, climateZoneProfile.globalFogDensity.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.HeightFogDistance = FloatInterpolation(settings.HeightFogDistance, climateZoneProfile.heightFogDistance.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.HeightFogSmooth = FloatInterpolation(settings.HeightFogSmooth, climateZoneProfile.heightFogSmooth.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.HeightFogDensity = FloatInterpolation(settings.HeightFogDensity, climateZoneProfile.heightFogDensity.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.HeightFogStart = FloatInterpolation(settings.HeightFogStart, climateZoneProfile.heightFogStart.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.HeightFogEnd = FloatInterpolation(settings.HeightFogEnd, climateZoneProfile.heightFogEnd.GetValue(timeOfDay, sunElevation, moonElevation), t);
            
            // Clouds
            settings.StaticCloudInterpolator = t;
            staticCloudSource = currentProfile.staticCloudTexture;
            staticCloudTarget = climateZoneProfile.staticCloudTexture;
            settings.StaticCloudLayer1Speed = FloatInterpolation(settings.StaticCloudLayer1Speed, climateZoneProfile.staticCloudLayer1Speed.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.StaticCloudLayer2Speed = FloatInterpolation(settings.StaticCloudLayer2Speed, climateZoneProfile.staticCloudLayer2Speed.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.StaticCloudColor = ColorInterpolation(settings.StaticCloudColor, climateZoneProfile.staticCloudColor.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.StaticCloudScattering = FloatInterpolation(settings.StaticCloudScattering, climateZoneProfile.staticCloudScattering.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.StaticCloudExtinction = FloatInterpolation(settings.StaticCloudExtinction, climateZoneProfile.staticCloudExtinction.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.StaticCloudSaturation = FloatInterpolation(settings.StaticCloudSaturation, climateZoneProfile.staticCloudSaturation.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.StaticCloudOpacity = FloatInterpolation(settings.StaticCloudOpacity, climateZoneProfile.staticCloudOpacity.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.DynamicCloudAltitude = FloatInterpolation(settings.DynamicCloudAltitude, climateZoneProfile.dynamicCloudAltitude.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.DynamicCloudDirection = FloatInterpolation(settings.DynamicCloudDirection, climateZoneProfile.dynamicCloudDirection.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.DynamicCloudSpeed = FloatInterpolation(settings.DynamicCloudSpeed, climateZoneProfile.dynamicCloudSpeed.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.DynamicCloudDensity = FloatInterpolation(settings.DynamicCloudDensity, climateZoneProfile.dynamicCloudDensity.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.DynamicCloudColor1 = ColorInterpolation(settings.DynamicCloudColor1, climateZoneProfile.dynamicCloudColor1.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.DynamicCloudColor2 = ColorInterpolation(settings.DynamicCloudColor2, climateZoneProfile.dynamicCloudColor2.GetValue(timeOfDay, sunElevation, moonElevation), t);
            
            // Lighting
            settings.DirectionalLightIntensity = FloatInterpolation(settings.DirectionalLightIntensity, climateZoneProfile.directionalLightIntensity.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.DirectionalLightColor = ColorInterpolation(settings.DirectionalLightColor, climateZoneProfile.directionalLightColor.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.EnvironmentIntensity = FloatInterpolation(settings.EnvironmentIntensity, climateZoneProfile.environmentIntensity.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.EnvironmentAmbientColor = ColorInterpolation(settings.EnvironmentAmbientColor, climateZoneProfile.environmentAmbientColor.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.EnvironmentEquatorColor = ColorInterpolation(settings.EnvironmentEquatorColor, climateZoneProfile.environmentEquatorColor.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.EnvironmentGroundColor = ColorInterpolation(settings.EnvironmentGroundColor, climateZoneProfile.environmentGroundColor.GetValue(timeOfDay, sunElevation, moonElevation), t);
            
            // Weather
            settings.LightRainIntensity = FloatInterpolation(settings.LightRainIntensity, climateZoneProfile.lightRainIntensity.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.MediumRainIntensity = FloatInterpolation(settings.MediumRainIntensity, climateZoneProfile.mediumRainIntensity.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.HeavyRainIntensity = FloatInterpolation(settings.HeavyRainIntensity, climateZoneProfile.heavyRainIntensity.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.SnowIntensity = FloatInterpolation(settings.SnowIntensity, climateZoneProfile.snowIntensity.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.RainColor = ColorInterpolation(settings.RainColor, climateZoneProfile.rainColor.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.SnowColor = ColorInterpolation(settings.SnowColor, climateZoneProfile.snowColor.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.LightRainSoundVolume = FloatInterpolation(settings.LightRainSoundVolume, climateZoneProfile.lightRainSoundVolume.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.MediumRainSoundVolume = FloatInterpolation(settings.MediumRainSoundVolume, climateZoneProfile.mediumRainSoundVolume.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.HeavyRainSoundVolume = FloatInterpolation(settings.HeavyRainSoundVolume, climateZoneProfile.heavyRainSoundVolume.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.LightWindSoundVolume = FloatInterpolation(settings.LightWindSoundVolume, climateZoneProfile.lightWindSoundVolume.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.MediumWindSoundVolume = FloatInterpolation(settings.MediumWindSoundVolume, climateZoneProfile.mediumWindSoundVolume.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.HeavyWindSoundVolume = FloatInterpolation(settings.HeavyWindSoundVolume, climateZoneProfile.heavyWindSoundVolume.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.WindSpeed = FloatInterpolation(settings.WindSpeed, climateZoneProfile.windSpeed.GetValue(timeOfDay, sunElevation, moonElevation), t);
            settings.WindDirection = FloatInterpolation(settings.WindDirection, climateZoneProfile.windDirection.GetValue(timeOfDay, sunElevation, moonElevation), t);
            
            // Outputs
            if (!outputProfile || !climateZoneProfile.outputProfile) return;
            if (outputProfile != climateZoneProfile.outputProfile || outputProfile.outputList.Count <= 0) return;
            for (int i = 0; i < outputProfile.outputList.Count; i++)
            {
                m_outputType = outputProfile.outputList[i].type;
                if (m_outputType == AzureOutputType.Slider || m_outputType == AzureOutputType.TimelineCurve || m_outputType == AzureOutputType.SunCurve || m_outputType == AzureOutputType.MoonCurve)
                {
                    outputProfile.outputList[i].floatOutput = FloatInterpolation(outputProfile.outputList[i].floatOutput, climateZoneProfile.outputPropertyList[i].GetFloatValue(timeOfDay, sunElevation, moonElevation), t);
                }
                else
                {
                    outputProfile.outputList[i].colorOutput = ColorInterpolation(outputProfile.outputList[i].colorOutput, climateZoneProfile.outputPropertyList[i].GetColorValue(timeOfDay, sunElevation, moonElevation), t);
                }
            }
        }
        
        /// <summary>
        /// Interpolates between two values given an interpolation factor.
        /// </summary>
        private float FloatInterpolation(float from, float to, float t)
        {
            return from + (to - from) * t;
        }
        
        /// <summary>
        /// Interpolates between two vectors given an interpolation factor.
        /// </summary>
        private Vector2 Vector2Interpolation(Vector2 from, Vector2 to, float t)
        {
            Vector2 ret;
            ret.x = from.x + (to.x - from.x) * t;
            ret.y = from.y + (to.y - from.y) * t;
            return ret;
        }

        /// <summary>
        /// Interpolates between two vectors given an interpolation factor.
        /// </summary>
        private Vector3 Vector3Interpolation(Vector3 from, Vector3 to, float t)
        {
            Vector3 ret;
            ret.x = from.x + (to.x - from.x) * t;
            ret.y = from.y + (to.y - from.y) * t;
            ret.z = from.z + (to.z - from.z) * t;
            return ret;
        }
        
        /// <summary>
        /// Interpolates between two colors given an interpolation factor.
        /// </summary>
        private Color ColorInterpolation(Color from, Color to, float t)
        {
            Color ret;
            ret.r = from.r + (to.r - from.r) * t;
            ret.g = from.g + (to.g - from.g) * t;
            ret.b = from.b + (to.b - from.b) * t;
            ret.a = from.a + (to.a - from.a) * t;
            return ret;
        }
        
        /// <summary>
        /// Returns the cloud uv position based on the direction and speed.
        /// </summary>
        private Vector2 ComputeCloudPosition()
        {
            float x = m_dynamicCloudDirection.x;
            float z = m_dynamicCloudDirection.y;
            float windSpeed = settings.DynamicCloudSpeed * 0.05f * Time.deltaTime;
            
            x += windSpeed * Mathf.Sin(0.01745329f * settings.DynamicCloudDirection);
            z += windSpeed * Mathf.Cos(0.01745329f * settings.DynamicCloudDirection);

            if (x >= 1.0f) x -= 1.0f;
            if (z >= 1.0f) z -= 1.0f;

            return new Vector2(x, z);
        }
        
        /// <summary>
        /// Total rayleigh computation.
        /// </summary>
        private Vector3 ComputeRayleigh()
        {
            Vector3 rayleigh = Vector3.one;
            Vector3 lambda = settings.Wavelength * 1e-9f;
            float n = 1.0003f; // Refractive index of air
            float pn = 0.035f; // Depolarization factor for standard air.
            float n2 = n * n;
            //float N = 2.545E25f;
            float N = settings.MolecularDensity;
            float temp = (8.0f * Mathf.PI * Mathf.PI * Mathf.PI * ((n2 - 1.0f) * (n2 - 1.0f))) / (3.0f * N * 1E25f) * ((6.0f + 3.0f * pn) / (6.0f - 7.0f * pn));
			
            rayleigh.x = temp / Mathf.Pow(lambda.x, 4.0f);
            rayleigh.y = temp / Mathf.Pow(lambda.y, 4.0f);
            rayleigh.z = temp / Mathf.Pow(lambda.z, 4.0f);

            return rayleigh;
        }
        
        /// <summary>
        /// Total mie computation.
        /// </summary>
        private Vector3 ComputeMie()
        {
            Vector3 mie;
			
            //float c = (0.6544f * Turbidity - 0.6510f) * 1e-16f;
            float c = (0.6544f * 5.0f - 0.6510f) * 10f * 1e-9f;
            Vector3 k = new Vector3(686.0f, 678.0f, 682.0f);
			
            mie.x = (434.0f * c * Mathf.PI * Mathf.Pow((4.0f * Mathf.PI) / settings.Wavelength.x, 2.0f) * k.x);
            mie.y = (434.0f * c * Mathf.PI * Mathf.Pow((4.0f * Mathf.PI) / settings.Wavelength.y, 2.0f) * k.y);
            mie.z = (434.0f * c * Mathf.PI * Mathf.Pow((4.0f * Mathf.PI) / settings.Wavelength.z, 2.0f) * k.z);
			
            //float c = (6544f * 5.0f - 6510f) * 10.0f * 1.0e-9f;
            //mie.x = (0.434f * c * Pi * Mathf.Pow((2.0f * Pi) / settings.Wavelength.x, 2.0f) * settings.K.x) / 3.0f;
            //mie.y = (0.434f * c * Pi * Mathf.Pow((2.0f * Pi) / settings.Wavelength.y, 2.0f) * settings.K.y) / 3.0f;
            //mie.z = (0.434f * c * Pi * Mathf.Pow((2.0f * Pi) / settings.Wavelength.z, 2.0f) * settings.K.z) / 3.0f;
			
            return mie;
        }
    }
}