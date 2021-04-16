using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

namespace UnityEngine.AzureSky
{
    [ExecuteInEditMode]
    [AddComponentMenu("Azure[Sky]/Azure Weather Controller")]
    public class AzureWeatherController : MonoBehaviour
    {
        [Tooltip("Drag the 'Azure Time Controller' here if you want the weather profiles to be avaluated using Azure time of day. Otherwise you will " +
        "need to evaluate the profiles by code calling the 'SetEvaluateTime(float evaluateTime)' on this component.")]
        [SerializeField] private AzureTimeController m_azureTimeController = null;

        [Tooltip("The transition length used to back to the default weather profile.")]
        [SerializeField] private float m_defaultWeatherTransitionLength = 10.0f;

        [Tooltip("List of default weather profiles. The scene will start using the first profile in this list and a random profile will be defined when a new day starts.")]
        [SerializeField] private List<AzureWeatherProfile> m_defaultWeatherProfilesList = new List<AzureWeatherProfile>();

        [Tooltip("List of global weather profiles. Call 'SetNewWeatherProfile(int index)' on this component to change the global weather by scripting.")]
        [SerializeField] private List<AzureGlobalWeather> m_globalWeatherProfilesList = new List<AzureGlobalWeather>();

        [Tooltip("Transform that will drive the local weather zone blending feature. By setting this field to 'null' will disable the local weather zones computation (global one will still work).")]
        [SerializeField] private Transform m_localWeatherZoneTrigger = null;

        [Tooltip("List of local weather zones. Place here all the local weather zones and arrange according to its priorities.")]
        [SerializeField] private List<AzureWeatherZone> m_localWeatherZonesList = new List<AzureWeatherZone>();

        [Tooltip("The override object with the custom property settings.")]
        public AzureOverrideObject overrideObject = null;

        [Tooltip("List of properties to override using the weather profiles.")]
        [SerializeField] private List<AzureOverrideProperty> m_overridePropertyList = new List<AzureOverrideProperty>();


        // Weather profiles
        private AzureWeatherProfile m_defaultWeatherProfile = null;
        private AzureWeatherProfile m_currentWeatherProfile = null;
        private AzureWeatherProfile m_targetWeatherProfile = null;

        // Global weather trasition
        [SerializeField]
        private float m_weatherTransitionProgress = 0.0f;
        private float m_weatherTransitionLength = 10.0f;
        private float m_weatherTransitionStart = 0.0f;
        private int   m_weatherIndex = -1;
        private bool  m_isWeatherChanging = false;

        // Profiles evaluation
        [SerializeField] private float m_timeOfDay = 6.0f;
        [SerializeField] private float m_sunElevation = 0.5f;
        [SerializeField] private float m_moonElevation = 0.5f;
        private string m_componentName = null;
        private string m_propertyName = null;
        private float m_profileMultiplier = 1f;

        // Local weather zones
        private Vector3 m_localWeatherZoneTriggerPosition = Vector3.zero;
        private Collider m_localWeatherZoneCollider;
        private float m_localWeatherZoneClosestDistanceSqr;
        private Vector3 m_localWeatherZoneClosestPoint;
        private float m_localWeatherZoneDistance;
        private float m_localWeatherZoneBlendDistanceSqr;
        private float m_localWeatherZoneInterpolationFactor;

        // Overrides
        private FieldInfo m_targetField = null;
        private PropertyInfo m_targetProperty = null;
        private Component m_targetComponent = null;
        private Material m_targetMaterial = null;
        private int m_targetUniformID = -1;

        private void Awake()
        {
            m_azureTimeController = GetComponent<AzureTimeController>();
            RefreshOverrideTargets();
            m_defaultWeatherProfile = m_defaultWeatherProfilesList[0];
            m_currentWeatherProfile = m_defaultWeatherProfile;
            m_targetWeatherProfile = m_defaultWeatherProfile;
            EvaluateWeatherProfiles();
        }

        private void OnEnable()
        {
            m_azureTimeController = GetComponent<AzureTimeController>();
        }

        private void Update()
        {
            // Editor only
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                m_defaultWeatherProfile = m_defaultWeatherProfilesList[0];
                m_currentWeatherProfile = m_defaultWeatherProfile;
                if (overrideObject.customPropertyList.Count != m_overridePropertyList.Count)
                    UpdateOverridePropertyListSize();
                EvaluateWeatherProfiles();
            }
            #endif
        }

        private void FixedUpdate()
        {
            EvaluateWeatherProfiles();
        }

        /// <summary>
        /// Set the default weather to a random profile from the default weather profiles list. 
        /// You can use this on the 'OnDayChange' event of the 'Azure Time Controller' component.
        /// </summary>
        public void SetRandomDefaultWeather(float transitionLength)
        {
            m_defaultWeatherProfile = m_defaultWeatherProfilesList[Random.Range(0, m_defaultWeatherProfilesList.Count)];
            if (m_weatherIndex < 0 && m_isWeatherChanging == false)
            {
                m_targetWeatherProfile = m_defaultWeatherProfile;
                if (m_targetWeatherProfile != m_currentWeatherProfile)
                {
                    m_weatherTransitionLength = transitionLength;
                    m_weatherTransitionProgress = 0.0f;
                    m_weatherTransitionStart = Time.time;
                    m_isWeatherChanging = true;
                }
            }
        }

        /// <summary>
        /// Starts a global weather transition. Set the index to -1 if you want to back the global weather to the default weather profile.
        /// </summary>
        public void SetNewWeatherProfile(int index)
        {
            switch (index)
            {
                // Back to the default weather profile currently in use
                case -1:
                    if (m_defaultWeatherProfile)
                    {
                        m_targetWeatherProfile = m_defaultWeatherProfile;
                        m_weatherTransitionLength = m_defaultWeatherTransitionLength;
                        m_weatherIndex = -1;
                    }
                    break;

                // Changes the global weather to the corresponding profile index on the global weather list
                default:
                    if (m_globalWeatherProfilesList[index].profile)
                    {
                        m_targetWeatherProfile = m_globalWeatherProfilesList[index].profile;
                        m_weatherTransitionLength = m_globalWeatherProfilesList[index].transitionLength;
                        m_weatherIndex = index;
                    }
                    break;
            }

            // Starts the global weather transition progress
            m_weatherTransitionProgress = 0.0f;
            m_weatherTransitionStart = Time.time;
            m_isWeatherChanging = true;
        }

        /// <summary>
        /// Changes the current weather profile without transition.
        /// </summary>
        public void SetNewWeatherProfile(AzureWeatherProfile profile)
        {
            m_currentWeatherProfile = profile;
        }

        /// <summary>
        /// Changes the current weather profile with transition.
        /// </summary>
        public void SetNewWeatherProfile(AzureWeatherProfile profile, float transitionTime)
        {
            m_targetWeatherProfile = profile;
            m_weatherTransitionLength = transitionTime;

            // Starts the global weather transition progress
            m_weatherTransitionProgress = 0.0f;
            m_weatherTransitionStart = Time.time;
            m_isWeatherChanging = true;
        }

        /// <summary>
        /// Returns the current weather profile in use by the system.
        /// </summary>
        public AzureWeatherProfile GetCurrentWeatherProfile()
        {
            return m_currentWeatherProfile;
        }

        /// <summary>
        /// Returns the active weather profile from the Default Weather Profiles list.
        /// </summary>
        public AzureWeatherProfile GetDefaultWeatherProfile()
        {
            return m_defaultWeatherProfile;
        }

        /// <summary>
        /// Returns the target weather profile if there is a weather transition in play.
        /// </summary>
        public AzureWeatherProfile GetTargetWeatherProfile()
        {
            if (m_isWeatherChanging)
                return m_targetWeatherProfile;
            else
                return null;
        }

        /// <summary>
        /// Returns the index of the current weather profile in use from the global weather list.
        /// </summary>
        public int GetGlobalWeatherIndex()
        {
            return m_weatherIndex;
        }


        /// <summary>
        /// Returns the float output of a giving override property index.
        /// </summary>
        public float GetOverrideFloatOutput(int index, float multiplier = 1.0f)
        {
            return m_overridePropertyList[index].floatOutput * multiplier;
        }


        /// <summary>
        /// Returns the color output of a giving override property index.
        /// </summary>
        public Color GetOverrideColorOutput(int index, float multiplier = 1.0f)
        {
            return m_overridePropertyList[index].colorOutput * multiplier;
        }

        /// <summary>
        /// Evaluates the time of day used to evaluate the weather profiles.
        /// </summary>
        public void EvaluateTimeOfDay(float evaluateTime)
        {
            m_timeOfDay = evaluateTime;
        }

        /// <summary>
        /// Evaluates the sun elevation used to evaluate the weather profiles.
        /// </summary>
        public void EvaluateSunElevation(float evaluateTime)
        {
            m_sunElevation = evaluateTime;
        }

        /// <summary>
        /// Evaluates the moon elevation used to evaluate the weather profiles.
        /// </summary>
        public void EvaluateMoonElevation(float evaluateTime)
        {
            m_moonElevation = evaluateTime;
        }

        private void EvaluateWeatherProfiles()
        {
            if (!overrideObject) return;

            if (m_azureTimeController)
            {
                m_timeOfDay = m_azureTimeController.GetEvaluateTime();
                m_sunElevation = m_azureTimeController.GetSunElevation();
                m_moonElevation = m_azureTimeController.GetMoonElevation();
            }

            if (!m_isWeatherChanging)
            {
                // Evaluates the current weather profile when there is no global weather transition or local weather zone influence
                EvaluateCurrentWeatherProfile();
            }
            else
            {
                // Computes the global weather transition progress
                m_weatherTransitionProgress = Mathf.Clamp01((Time.time - m_weatherTransitionStart) / m_weatherTransitionLength);

                // Performs the global weather transition
                ApplyGlobalWeatherTransition(m_currentWeatherProfile, m_targetWeatherProfile, m_weatherTransitionProgress);

                // Ends the global weather transition
                if (Mathf.Abs(m_weatherTransitionProgress - 1.0f) <= 0.0f)
                {
                    m_isWeatherChanging = false;
                    m_weatherTransitionProgress = 0.0f;
                    m_weatherTransitionStart = 0.0f;
                    m_currentWeatherProfile = m_targetWeatherProfile;
                }
            }

            // Computes weather zones influence
            // Based on Unity's Post Processing v2
            if (!m_localWeatherZoneTrigger)
                return;

            m_localWeatherZoneTriggerPosition = m_localWeatherZoneTrigger.position;

            // Traverse all weather zones in the weather zone list
            foreach (var weatherZone in m_localWeatherZonesList)
            {
                // Skip if the list index is null
                if (weatherZone == null)
                    continue;

                // If weather zone has no collider, skip it as it's useless
                m_localWeatherZoneCollider = weatherZone.GetComponent<Collider>();
                if (!m_localWeatherZoneCollider)
                    continue;

                if (!m_localWeatherZoneCollider.enabled || !m_localWeatherZoneCollider.gameObject.activeSelf)
                    continue;

                // Find closest distance to weather zone, 0 means it's inside it
                m_localWeatherZoneClosestDistanceSqr = float.PositiveInfinity;
                
                m_localWeatherZoneClosestPoint = m_localWeatherZoneCollider.ClosestPoint(m_localWeatherZoneTriggerPosition); // 5.6-only API
                m_localWeatherZoneDistance = ((m_localWeatherZoneClosestPoint - m_localWeatherZoneTriggerPosition) / 2f).sqrMagnitude;

                if (m_localWeatherZoneDistance < m_localWeatherZoneClosestDistanceSqr)
                    m_localWeatherZoneClosestDistanceSqr = m_localWeatherZoneDistance;

                m_localWeatherZoneCollider = null;
                m_localWeatherZoneBlendDistanceSqr = weatherZone.blendDistance * weatherZone.blendDistance;

                // Weather zone has no influence, ignore it
                // Note: Weather zone doesn't do anything when `closestDistanceSqr = blendDistSqr` but
                //       we can't use a >= comparison as blendDistSqr could be set to 0 in which
                //       case weather zone would have total influence
                if (m_localWeatherZoneClosestDistanceSqr > m_localWeatherZoneBlendDistanceSqr)
                    continue;

                // Weather zone has influence
                m_localWeatherZoneInterpolationFactor = 1f;

                if (m_localWeatherZoneBlendDistanceSqr > 0f)
                    m_localWeatherZoneInterpolationFactor = 1f - (m_localWeatherZoneClosestDistanceSqr / m_localWeatherZoneBlendDistanceSqr);

                // No need to clamp01 the interpolation factor as it'll always be in [0;1[ range
                ApplyWeatherZonesInfluence(weatherZone.profile, m_localWeatherZoneInterpolationFactor);
            }
        }

        /// <summary>
        /// Evaluates the current weather profile when there is no global weather transition or local weather zone influence.
        /// </summary>
        private void EvaluateCurrentWeatherProfile()
        {
            //Debug.Log("Profile: " + m_currentWeatherProfile.profilePropertyList.Count);
            //Debug.Log("Controller: " + m_overridePropertyList.Count);
            //Debug.Log("Object: " + overrideObject.customPropertyList.Count);

            // Loop through the override property list
            for (int index = 0; index < overrideObject.customPropertyList.Count; index++)
            {
                switch (overrideObject.customPropertyList[index].outputType)
                {
                    case AzureOutputType.Float:
                        m_overridePropertyList[index].floatOutput = m_currentWeatherProfile.profilePropertyList[index].GetFloatOutput(m_timeOfDay, m_sunElevation, m_moonElevation);
                        // Loop through the setup property list within the override property list
                        for (int innerIndex = 0; innerIndex < m_overridePropertyList[index].overridePropertySetupList.Count; innerIndex++)
                        {
                            m_profileMultiplier = m_overridePropertyList[index].overridePropertySetupList[innerIndex].multiplier;
                            switch (m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetType)
                            {
                                case AzureOverrideType.Field:
                                    m_targetField = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetField;
                                    m_targetComponent = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetComponent;
                                    if (m_targetField != null)
                                        m_targetField.SetValue(m_targetComponent, m_overridePropertyList[index].floatOutput * m_profileMultiplier);
                                    break;
                                case AzureOverrideType.Property:
                                    m_targetProperty = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetProperty;
                                    m_targetComponent = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetComponent;
                                    if (m_targetProperty != null)
                                        m_targetProperty.SetValue(m_targetComponent, m_overridePropertyList[index].floatOutput * m_profileMultiplier);
                                    break;
                                case AzureOverrideType.ShaderProperty:
                                    m_targetMaterial = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetMaterial;
                                    m_targetUniformID = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetUniformID;
                                    if (m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetShaderUpdateMode == AzureShaderUpdateMode.Local)
                                    {
                                        if (m_targetMaterial)
                                            m_targetMaterial.SetFloat(m_targetUniformID, m_overridePropertyList[index].floatOutput * m_profileMultiplier);
                                    }
                                    else Shader.SetGlobalFloat(m_targetUniformID, m_overridePropertyList[index].floatOutput * m_profileMultiplier);
                                    break;
                            }
                        }
                        break;

                    case AzureOutputType.Color:
                        m_overridePropertyList[index].colorOutput = m_currentWeatherProfile.profilePropertyList[index].GetColorOutput(m_timeOfDay, m_sunElevation, m_moonElevation);
                        // Loop through the setup property list within the override property list
                        for (int innerIndex = 0; innerIndex < m_overridePropertyList[index].overridePropertySetupList.Count; innerIndex++)
                        {
                            m_profileMultiplier = m_overridePropertyList[index].overridePropertySetupList[innerIndex].multiplier;
                            switch (m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetType)
                            {
                                case AzureOverrideType.Field:
                                    m_targetField = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetField;
                                    m_targetComponent = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetComponent;
                                    if (m_targetField != null)
                                        m_targetField.SetValue(m_targetComponent, m_overridePropertyList[index].colorOutput * m_profileMultiplier);
                                    break;
                                case AzureOverrideType.Property:
                                    m_targetProperty = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetProperty;
                                    m_targetComponent = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetComponent;
                                    if (m_targetProperty != null)
                                        m_targetProperty.SetValue(m_targetComponent, m_overridePropertyList[index].colorOutput * m_profileMultiplier);
                                    break;
                                case AzureOverrideType.ShaderProperty:
                                    m_targetMaterial = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetMaterial;
                                    m_targetUniformID = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetUniformID;
                                    if (m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetShaderUpdateMode == AzureShaderUpdateMode.Local)
                                    {
                                        if (m_targetMaterial)
                                            m_targetMaterial.SetColor(m_targetUniformID, m_overridePropertyList[index].colorOutput * m_profileMultiplier);
                                    }
                                    else Shader.SetGlobalColor(m_targetUniformID, m_overridePropertyList[index].colorOutput * m_profileMultiplier);
                                    break;
                            }
                        }
                        break;
                }
            }
        }

        private void ApplyGlobalWeatherTransition(AzureWeatherProfile from, AzureWeatherProfile to, float t)
        {
            // Loop through the override property list
            for (int index = 0; index < overrideObject.customPropertyList.Count; index++)
            {
                switch (overrideObject.customPropertyList[index].outputType)
                {
                    case AzureOutputType.Float:
                        m_overridePropertyList[index].floatOutput = FloatInterpolation(from.profilePropertyList[index].GetFloatOutput(m_timeOfDay, m_sunElevation, m_moonElevation),
                                                                   to.profilePropertyList[index].GetFloatOutput(m_timeOfDay, m_sunElevation, m_moonElevation), t);
                        // Loop through the setup property list within the override property list
                        for (int innerIndex = 0; innerIndex < m_overridePropertyList[index].overridePropertySetupList.Count; innerIndex++)
                        {
                            m_profileMultiplier = m_overridePropertyList[index].overridePropertySetupList[innerIndex].multiplier;
                            switch (m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetType)
                            {
                                case AzureOverrideType.Field:
                                    m_targetField = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetField;
                                    m_targetComponent = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetComponent;
                                    if (m_targetField != null)
                                        m_targetField.SetValue(m_targetComponent, m_overridePropertyList[index].floatOutput * m_profileMultiplier);
                                    break;
                                case AzureOverrideType.Property:
                                    m_targetProperty = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetProperty;
                                    m_targetComponent = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetComponent;
                                    if (m_targetProperty != null)
                                        m_targetProperty.SetValue(m_targetComponent, m_overridePropertyList[index].floatOutput * m_profileMultiplier);
                                    break;
                                case AzureOverrideType.ShaderProperty:
                                    m_targetMaterial = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetMaterial;
                                    m_targetUniformID = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetUniformID;
                                    if (m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetShaderUpdateMode == AzureShaderUpdateMode.Local)
                                    {
                                        if (m_targetMaterial)
                                            m_targetMaterial.SetFloat(m_targetUniformID, m_overridePropertyList[index].floatOutput * m_profileMultiplier);
                                    }
                                    else Shader.SetGlobalFloat(m_targetUniformID, m_overridePropertyList[index].floatOutput * m_profileMultiplier);
                                    break;
                            }
                        }
                        break;

                    case AzureOutputType.Color:
                        m_overridePropertyList[index].colorOutput = ColorInterpolation(from.profilePropertyList[index].GetColorOutput(m_timeOfDay, m_sunElevation, m_moonElevation),
                                                                   to.profilePropertyList[index].GetColorOutput(m_timeOfDay, m_sunElevation, m_moonElevation), t);
                        // Loop through the setup property list within the override property list
                        for (int innerIndex = 0; innerIndex < m_overridePropertyList[index].overridePropertySetupList.Count; innerIndex++)
                        {
                            m_profileMultiplier = m_overridePropertyList[index].overridePropertySetupList[innerIndex].multiplier;
                            switch (m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetType)
                            {
                                case AzureOverrideType.Field:
                                    m_targetField = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetField;
                                    m_targetComponent = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetComponent;
                                    if (m_targetField != null)
                                        m_targetField.SetValue(m_targetComponent, m_overridePropertyList[index].colorOutput * m_profileMultiplier);
                                    break;
                                case AzureOverrideType.Property:
                                    m_targetProperty = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetProperty;
                                    m_targetComponent = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetComponent;
                                    if (m_targetProperty != null)
                                        m_targetProperty.SetValue(m_targetComponent, m_overridePropertyList[index].colorOutput * m_profileMultiplier);
                                    break;
                                case AzureOverrideType.ShaderProperty:
                                    m_targetMaterial = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetMaterial;
                                    m_targetUniformID = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetUniformID;
                                    if (m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetShaderUpdateMode == AzureShaderUpdateMode.Local)
                                    {
                                        if (m_targetMaterial)
                                            m_targetMaterial.SetColor(m_targetUniformID, m_overridePropertyList[index].colorOutput * m_profileMultiplier);
                                    }
                                    else Shader.SetGlobalColor(m_targetUniformID, m_overridePropertyList[index].colorOutput * m_profileMultiplier);
                                    break;
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Computes local weather zones influence.
        /// </summary>
        private void ApplyWeatherZonesInfluence(AzureWeatherProfile localZoneProfile, float t)
        {
            // Loop through the override property list
            for (int index = 0; index < overrideObject.customPropertyList.Count; index++)
            {
                switch (overrideObject.customPropertyList[index].outputType)
                {
                    case AzureOutputType.Float:
                        m_overridePropertyList[index].floatOutput = FloatInterpolation(m_overridePropertyList[index].floatOutput, localZoneProfile.profilePropertyList[index].GetFloatOutput(m_timeOfDay, m_sunElevation, m_moonElevation), t);
                        // Loop through the setup property list within the override property list
                        for (int innerIndex = 0; innerIndex < m_overridePropertyList[index].overridePropertySetupList.Count; innerIndex++)
                        {
                            m_profileMultiplier = m_overridePropertyList[index].overridePropertySetupList[innerIndex].multiplier;
                            switch (m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetType)
                            {
                                case AzureOverrideType.Field:
                                    m_targetField = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetField;
                                    m_targetComponent = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetComponent;
                                    if (m_targetField != null)
                                        m_targetField.SetValue(m_targetComponent, m_overridePropertyList[index].floatOutput * m_profileMultiplier);
                                    break;
                                case AzureOverrideType.Property:
                                    m_targetProperty = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetProperty;
                                    m_targetComponent = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetComponent;
                                    if (m_targetProperty != null)
                                        m_targetProperty.SetValue(m_targetComponent, m_overridePropertyList[index].floatOutput * m_profileMultiplier);
                                    break;
                                case AzureOverrideType.ShaderProperty:
                                    m_targetMaterial = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetMaterial;
                                    m_targetUniformID = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetUniformID;
                                    if (m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetShaderUpdateMode == AzureShaderUpdateMode.Local)
                                    {
                                        if (m_targetMaterial)
                                            m_targetMaterial.SetFloat(m_targetUniformID, m_overridePropertyList[index].floatOutput * m_profileMultiplier);
                                    }
                                    else Shader.SetGlobalFloat(m_targetUniformID, m_overridePropertyList[index].floatOutput * m_profileMultiplier);
                                    break;
                            }
                        }
                        break;

                    case AzureOutputType.Color:
                        m_overridePropertyList[index].colorOutput = ColorInterpolation(m_overridePropertyList[index].colorOutput, localZoneProfile.profilePropertyList[index].GetColorOutput(m_timeOfDay, m_sunElevation, m_moonElevation), t);
                        // Loop through the setup property list within the override property list
                        for (int innerIndex = 0; innerIndex < m_overridePropertyList[index].overridePropertySetupList.Count; innerIndex++)
                        {
                            m_profileMultiplier = m_overridePropertyList[index].overridePropertySetupList[innerIndex].multiplier;
                            switch (m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetType)
                            {
                                case AzureOverrideType.Field:
                                    m_targetField = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetField;
                                    m_targetComponent = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetComponent;
                                    if (m_targetField != null)
                                        m_targetField.SetValue(m_targetComponent, m_overridePropertyList[index].colorOutput * m_profileMultiplier);
                                    break;
                                case AzureOverrideType.Property:
                                    m_targetProperty = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetProperty;
                                    m_targetComponent = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetComponent;
                                    if (m_targetProperty != null)
                                        m_targetProperty.SetValue(m_targetComponent, m_overridePropertyList[index].colorOutput * m_profileMultiplier);
                                    break;
                                case AzureOverrideType.ShaderProperty:
                                    m_targetMaterial = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetMaterial;
                                    m_targetUniformID = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetUniformID;
                                    if (m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetShaderUpdateMode == AzureShaderUpdateMode.Local)
                                    {
                                        if (m_targetMaterial)
                                            m_targetMaterial.SetColor(m_targetUniformID, m_overridePropertyList[index].colorOutput * m_profileMultiplier);
                                    }
                                    else Shader.SetGlobalColor(m_targetUniformID, m_overridePropertyList[index].colorOutput * m_profileMultiplier);
                                    break;
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// FieldInfo and PropertyInfo data doesn't survive the assembly reload, so we need to reference it again at the starting.
        /// </summary>
        public void RefreshOverrideTargets()
        {
            if (!overrideObject) return;

            // Resize the override property list if needed
            if (overrideObject.customPropertyList.Count != m_overridePropertyList.Count)
                UpdateOverridePropertyListSize();

            // Loop through the override property list
            for (int index = 0; index < overrideObject.customPropertyList.Count; index++)
            {
                // Initialize the settings

                // Loop through the setup property list within the override property list
                for (int innerIndex = 0; innerIndex < m_overridePropertyList[index].overridePropertySetupList.Count; innerIndex++)
                {
                    if (m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetGameObject)
                    {
                        m_componentName = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetComponentName;
                        m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetComponent =
                        m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetGameObject.GetComponent(m_componentName);

                        if (m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetComponent)
                        {
                            m_propertyName = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetPropertyName;
                            m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetField =
                            m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetComponent.GetType().GetField(m_propertyName);
                            m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetProperty =
                            m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetComponent.GetType().GetProperty(m_propertyName);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Resize the override property list to the same size of the override object custom property list.
        /// </summary>
        private void UpdateOverridePropertyListSize()
        {
            while (m_overridePropertyList.Count > overrideObject.customPropertyList.Count) m_overridePropertyList.RemoveAt(m_overridePropertyList.Count - 1);
            while (m_overridePropertyList.Count < overrideObject.customPropertyList.Count) m_overridePropertyList.Add(new AzureOverrideProperty());
        }

        /// <summary>
        /// Returns the current weather transition progress.
        /// </summary>
        public float GetWeatherTransitionProgress()
        {
            return m_weatherTransitionProgress;
        }

        /// <summary>
        /// Interpolates between two values given an interpolation factor.
        /// </summary>
        private float FloatInterpolation(float from, float to, float t)
        {
            return from + (to - from) * t;
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

        // Editor only
        #if UNITY_EDITOR
        /// <summary>
        /// Checks if the target 'field' of a given override property exists.
        /// </summary>
        public bool TargetHasField(int index, int innerIndex)
        {
            return m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetField == null;
        }

        /// <summary>
        /// Checks if the target 'property' of a given override property exists.
        /// </summary>
        public bool TargetHasProperty(int index, int innerIndex)
        {
            return m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetProperty == null;
        }

        /// <summary>
        /// Checks if the target material of a given override property has the target property.
        /// </summary>
        public bool MaterialHasProperty(int index, int innerIndex, string name)
        {
            bool ret = false;
            Material mat = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetMaterial;
            if (mat.HasProperty(name)) ret = true;
            return ret;
        }

        /// <summary>
        /// Checks if the target shader of a given override property has the target property.
        /// </summary>
        public bool ShaderHasProperty(int index, int innerIndex, string name)
        {
            bool ret = false;
            int id = m_overridePropertyList[index].overridePropertySetupList[innerIndex].targetUniformID;
            Debug.Log(id);
            if (id != 0) ret = true;
            return ret;
        }
        #endif
    }
}