using System;

namespace UnityEngine.AzureSky
{
    [Serializable]
    public sealed class AzureFloatProperty
    {
        public enum PropertyType
        {
            Slider,
            TimelineCurve,
            SunCurve,
            MoonCurve
        }

        public PropertyType type = PropertyType.Slider;
        public float slider;
        public AnimationCurve timelineCurve;
        public AnimationCurve sunCurve;
        public AnimationCurve moonCurve;

        public AzureFloatProperty(float slider, AnimationCurve timelineCurve, AnimationCurve sunCurve, AnimationCurve moonCurve)
        {
            this.slider = slider;
            this.timelineCurve = timelineCurve;
            this.sunCurve = sunCurve;
            this.moonCurve = moonCurve;
        }
        
        public float GetValue(float time, float sunElevation, float moonElevation)
        {
            switch (type)
            {
                case PropertyType.Slider:
                    return slider;
                
                case PropertyType.TimelineCurve:
                    return timelineCurve.Evaluate(time);
                
                case PropertyType.SunCurve:
                    return sunCurve.Evaluate(sunElevation);
                
                case PropertyType.MoonCurve:
                    return moonCurve.Evaluate(moonElevation);
                
                default:
                    return slider;
            }
        }
    }
}