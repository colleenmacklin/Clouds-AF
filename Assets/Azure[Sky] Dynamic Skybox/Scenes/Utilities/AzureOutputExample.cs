using UnityEngine;
using UnityEngine.AzureSky;

public class AzureOutputExample : MonoBehaviour
{
    public AzureSkyController azureSky;
    public Light myPointLight;

    private void Update()
    {
        myPointLight.intensity = azureSky.GetOutputFloatValue(0) * 2.5f; // Get the float value from the output 0
        myPointLight.color = azureSky.GetOutputColorValue(1); // Get the color value from the output 1
    }
}