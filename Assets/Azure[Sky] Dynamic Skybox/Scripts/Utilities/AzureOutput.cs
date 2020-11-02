using System;

namespace UnityEngine.AzureSky
{
    [Serializable]
    public class AzureOutput
    {
        // Not included in build
        #if UNITY_EDITOR
        public string description;
        #endif
        
        public AzureOutputType type = AzureOutputType.Slider;
        public float floatOutput;
        public Color colorOutput;
    }
}