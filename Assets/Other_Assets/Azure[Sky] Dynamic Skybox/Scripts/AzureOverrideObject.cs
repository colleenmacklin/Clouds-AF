using System;
using System.Collections.Generic;

namespace UnityEngine.AzureSky
{
    [CreateAssetMenu(fileName = "Override Object", menuName = "Azure[Sky] Dynamic Skybox/New Override Object", order = 1)]
    public class AzureOverrideObject : ScriptableObject
    {
        public List<AzureCustomProperty> customPropertyList = new List<AzureCustomProperty>();
    }

    [Serializable]
    public sealed class AzureCustomProperty
    {
        // Not included in build
        #if UNITY_EDITOR
        public string name;
        public string description;
        #endif

        public AzureOutputType outputType;
        public AzureOutputFloatType floatPropertyType;
        public AzureOutputColorType colorPropertyType;
    }
}