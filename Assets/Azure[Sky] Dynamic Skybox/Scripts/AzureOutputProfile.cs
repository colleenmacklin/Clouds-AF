using System.Collections.Generic;

namespace UnityEngine.AzureSky
{
    [CreateAssetMenu(fileName = "Output Profile", menuName = "Azure[Sky] Dynamic Skybox/New Output Profile", order = 1)]
    public sealed class AzureOutputProfile : ScriptableObject
    {
        public List<AzureOutput> outputList = new List<AzureOutput>();
    }
}