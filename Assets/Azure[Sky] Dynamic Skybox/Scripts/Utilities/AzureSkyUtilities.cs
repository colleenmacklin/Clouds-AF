using System;
using UnityEngine.Events;

namespace UnityEngine.AzureSky
{
    [Serializable]
    public struct AzureGlobalWeather
    {
        public AzureSkyProfile profile;
        public float transitionTime;
    }
    
    public enum AzureTimeSystem
    {
        Simple,
        Realistic
    }
    
    public enum AzureTimeRepeatMode
    {
        Off,
        ByDay,
        ByMonth,
        ByYear
    }
    
    public enum AzureScatteringMode
    {
        Automatic,
        CustomColor
    }

    public enum AzureCloudMode
    {
        EmptySky,
        StaticClouds,
        DynamicClouds
    }

    public enum AzureTimeDirection
    {
        Forward,
        Back
    }

    public enum AzureEventScanMode
    {
        ByMinute,
        ByHour
    }
    
    public enum AzureOutputType
    {
        Slider,
        TimelineCurve,
        SunCurve,
        MoonCurve,
        Color,
        TimelineGradient,
        SunGradient,
        MoonGradient
    }

    public enum AzureReflectionProbeState
    {
        On,
        Off
    }

    public enum AzureShaderUpdateMode
    {
        Global,
        ByMaterial
    }

    [Serializable]
    public sealed class AzureEventAction
    {
        // Not included in build
        #if UNITY_EDITOR
        public bool isExpanded = true;
        #endif
        
        public UnityEvent eventAction;
        public int hour = 6;
        public int minute = 0;
        public int year = 2020;
        public int month = 1;
        public int day = 1;
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