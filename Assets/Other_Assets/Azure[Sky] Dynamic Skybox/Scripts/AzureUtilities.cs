using System;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Reflection;

namespace UnityEngine.AzureSky
{
    /// <summary>
    /// Used to set how the time system will work.
    /// </summary>
    public enum AzureTimeSystem
    {
        [Tooltip("This option sets a simple rotation for the sun and moon transforms. Best for performance.")]
        Simple,
        [Tooltip("This option sets a realistic astronomical coordinate for the sun and moon transforms.")]
        Realistic
    }

    /// <summary>
    /// Used to set the direction in which the time of day will flow.
    /// </summary>
    public enum AzureTimeDirection
    {
        Forward,
        Backward
    }

    /// <summary>
    /// Used to set the date loop.
    /// </summary>
    public enum AzureDateLoop
    {
        Off,
        ByDay,
        ByMonth,
        ByYear
    }

    public enum AzureOutputType
    {
        Float,
        Color
    }

    public enum AzureOutputFloatType
    {
        Slider,
        TimelineBasedCurve,
        SunElevationBasedCurve,
        MoonElevationBasedCurve
    }

    public enum AzureOutputColorType
    {
        ColorField,
        TimelineBasedGradient,
        SunElevationBasedGradient,
        MoonElevationBasedGradient
    }

    public enum AzureOverrideType
    {
        Field,
        Property,
        ShaderProperty
    }

    public enum AzureShaderUpdateMode
    {
        Local,
        Global
    }

    public enum AzureScatteringMode
    {
        Automatic,
        Custom
    }

    public enum AzureCloudMode
    {
        Off,
        Static,
        Dynamic
    }

    public enum AzureEventScanMode
    {
        ByMinute,
        ByHour
    }

    public enum AzureReflectionProbeState
    {
        On,
        Off
    }

    public sealed class AzureSettings
    {
        public float floatValue;
        public Color colorValue;

        public AzureSettings(float floatValue, Color colorValue)
        {
            this.floatValue = floatValue;
            this.colorValue = colorValue;
        }
    }

    [Serializable]
    public sealed class AzureGlobalWeather
    {
        public AzureWeatherProfile profile;
        public float transitionLength;
    }

    [Serializable]
    public sealed class AzureCustomEvent
    {
        public UnityEvent unityEvent;
        public int hour = 6;
        public int minute = 0;
        public int year = 2020;
        public int month = 1;
        public int day = 1;
        public int executedHour = 0;
        public bool isAlreadyExecutedOnThisHour = false;
    }

    [Serializable]
    public sealed class AzureOverrideProperty
    {
        // Not included in build
        #if UNITY_EDITOR
        public string name;
        public string description;
        #endif

        public float floatOutput = 0f;
        public Color colorOutput = Color.white;
        public List<OverridePropertySetup> overridePropertySetupList = new List<OverridePropertySetup>();

        [Serializable]
        public sealed class OverridePropertySetup
        {
            // Not included in build
            #if UNITY_EDITOR
            //public string targetComponentName;
            //public string targetPropertyName;
            #endif

            public AzureOverrideType targetType;
            public GameObject targetGameObject;
            public Material targetMaterial;
            public AzureShaderUpdateMode targetShaderUpdateMode;
            public Component targetComponent;
            public FieldInfo targetField;
            public PropertyInfo targetProperty;
            public int targetUniformID;
            public float multiplier = 1.0f;

            public string targetComponentName;
            public string targetPropertyName;
        }
    }

    /// <summary>
    /// An celestial body instance.
    /// </summary>
    [Serializable]
    public sealed class AzureCelestialBody
    {
        [Tooltip("The Transform that will receive the celestial body coordinate.")]
        public Transform transform;

        [Tooltip("The celestial body that this instance will simulate.")]
        public Type type;

        [Tooltip("How the transform direction should behaves.")]
        public Behaviour behaviour;

        [Tooltip("The distance between the celestial body and Earth's center.")]
        public float distance;

        /// <summary>
        /// A preset of celestial bodies that the system can simulate.
        /// </summary>
        public enum Type
        {
            Mercury,
            Venus,
            Mars,
            Jupiter,
            Saturn,
            Uranus,
            Neptune,
            Pluto
        }
    }

    /// <summary>
    /// Thunder settings container.
    /// </summary>
    [Serializable]
    public sealed class AzureThunderSettings
    {
        public Transform thunderPrefab;
        public AudioClip audioClip;
        public AnimationCurve lightFrequency;
        public float audioDelay;
        public Vector3 position;
    }
}