using System;

namespace UnityEngine.AzureSky
{
    [Serializable]
    public sealed class AzureColorProperty
    {
        public enum PropertyType
        {
            Color,
            TimelineGradient,
            SunGradient,
            MoonGradient
        }

        public PropertyType type = PropertyType.Color;
        public Color color;
        public Gradient timelineGradient;
        public Gradient sunGradient;
        public Gradient moonGradient;

        public AzureColorProperty(Color color, Gradient timelineGradient, Gradient sunGradient, Gradient moonGradient)
        {
            this.color = color;
            this.timelineGradient = timelineGradient;
            this.sunGradient = sunGradient;
            this.moonGradient = moonGradient;
        }
        
        public Color GetValue(float time, float sunElevation, float moonElevation)
        {
            switch (type)
            {
                case PropertyType.Color:
                    return color;
                
                case PropertyType.TimelineGradient:
                    return timelineGradient.Evaluate(time / 24.0f);
                
                case PropertyType.SunGradient:
                    return sunGradient.Evaluate(Mathf.InverseLerp(-1.0f, 1.0f, sunElevation));
                
                case PropertyType.MoonGradient:
                    return moonGradient.Evaluate(Mathf.InverseLerp(-1.0f, 1.0f, moonElevation));
                
                default:
                    return color;
            }
        }
    }
}