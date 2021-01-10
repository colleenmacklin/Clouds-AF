using System;

namespace UnityEngine.AzureSky
{
    [Serializable]
    public sealed class AzureOutputProperty
    {
        public AzureOutputType type = AzureOutputType.Slider;
        public float slider = 0.0f;
        public AnimationCurve timelineCurve = AnimationCurve.Linear(0.0f, 0.0f, 24.0f, 0.0f);
        public AnimationCurve sunCurve = AnimationCurve.Linear(-1.0f, 0.0f, 1.0f, 0.0f);
        public AnimationCurve moonCurve = AnimationCurve.Linear(-1.0f, 0.0f, 1.0f, 0.0f);
        public Color color = Color.white;
        public Gradient timelineGradient = new Gradient();
        public Gradient sunGradient = new Gradient();
        public Gradient moonGradient = new Gradient();
        
        public float GetFloatValue(float time, float sunElevation, float moonElevation)
        {
            switch (type)
            {
                case AzureOutputType.Slider:
                    return slider;
                
                case AzureOutputType.TimelineCurve:
                    return timelineCurve.Evaluate(time);
                
                case AzureOutputType.SunCurve:
                    return sunCurve.Evaluate(sunElevation);
                
                case AzureOutputType.MoonCurve:
                    return moonCurve.Evaluate(moonElevation);
            }

            return slider;
        }
        
        public Color GetColorValue(float time, float sunElevation, float moonElevation)
        {
            switch (type)
            {
                case AzureOutputType.Color:
                    return color;
                
                case AzureOutputType.TimelineGradient:
                    return timelineGradient.Evaluate(time / 24.0f);
                
                case AzureOutputType.SunGradient:
                    return sunGradient.Evaluate(Mathf.InverseLerp(-1.0f, 1.0f, sunElevation));
                
                case AzureOutputType.MoonGradient:
                    return moonGradient.Evaluate(Mathf.InverseLerp(-1.0f, 1.0f, moonElevation));
            }
            
            return color;
        }
    }
}