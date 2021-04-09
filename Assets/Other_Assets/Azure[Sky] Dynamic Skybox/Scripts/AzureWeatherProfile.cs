using System;
using System.Collections.Generic;

namespace UnityEngine.AzureSky
{
    [CreateAssetMenu(fileName = "Weather Profile", menuName = "Azure[Sky] Dynamic Skybox/New Weather Profile", order = 1)]
    public class AzureWeatherProfile : ScriptableObject
    {
        [Tooltip("The override object with the custom properties list.")]
        public AzureOverrideObject overrideObject;

        [Tooltip("List with the profile properties created from the override object to be stored and edited in this profile.")]
        public List<AzureProfileProperty> profilePropertyList = new List<AzureProfileProperty>();

        /// <summary>
        /// Resize a list.
        /// </summary>
        public void ResizeList(int newCount)
        {
            if (newCount == profilePropertyList.Count) return;

            if (newCount <= 0)
            {
                profilePropertyList.Clear();
            }
            else
            {
                while (profilePropertyList.Count > newCount) profilePropertyList.RemoveAt(profilePropertyList.Count - 1);
                while (profilePropertyList.Count < newCount) profilePropertyList.Add(new AzureProfileProperty());
            }
        }
    }

    /// <summary>
    /// A custom property used by this scriptable object.
    /// </summary>
    [Serializable]
    public sealed class AzureProfileProperty
    {
        // Not included in build
        #if UNITY_EDITOR
        public string name;
        public string description;
        #endif

        public AzureOutputType outputType;
        public AzureOutputFloatType floatPropertyType;
        public AzureOutputColorType colorPropertyType;

        public float slider = 0.5f;
        public AnimationCurve timelineBasedCurve = AnimationCurve.Linear(0f, 0.5f, 24f, 0.5f);
        public AnimationCurve sunElevationBasedCurve = AnimationCurve.Linear(-1f, 0.5f, 1f, 0.5f);
        public AnimationCurve moonElevationBasedCurve = AnimationCurve.Linear(-1f, 0.5f, 1f, 0.5f);
        public Color colorField = Color.white;
        public Gradient timelineBasedGradient = new Gradient();
        public Gradient sunElevationBasedGradient = new Gradient();
        public Gradient moonElevationBasedGradient = new Gradient();

        /// <summary>
        /// Returns the evaluated float value based on the property type.
        /// </summary>
        public float GetFloatOutput(float timeOfDay, float sunElevation, float moonElevation)
        {
            switch (floatPropertyType)
            {
                case AzureOutputFloatType.Slider:
                    return slider;
                case AzureOutputFloatType.TimelineBasedCurve:
                    return timelineBasedCurve.Evaluate(timeOfDay);
                case AzureOutputFloatType.SunElevationBasedCurve:
                    return sunElevationBasedCurve.Evaluate(sunElevation);
                case AzureOutputFloatType.MoonElevationBasedCurve:
                    return moonElevationBasedCurve.Evaluate(moonElevation);
            }

            return 0f;
        }

        /// <summary>
        /// Returns the evaluated color based on the property type.
        /// </summary>
        public Color GetColorOutput(float timeOfDay, float sunElevation, float moonElevation)
        {
            switch (colorPropertyType)
            {
                case AzureOutputColorType.ColorField:
                    return colorField;
                case AzureOutputColorType.TimelineBasedGradient:
                    return timelineBasedGradient.Evaluate(timeOfDay / 24f);
                case AzureOutputColorType.SunElevationBasedGradient:
                    return sunElevationBasedGradient.Evaluate(sunElevation);
                case AzureOutputColorType.MoonElevationBasedGradient:
                    return moonElevationBasedGradient.Evaluate(moonElevation);
            }

            return colorField;
        }
    }
}