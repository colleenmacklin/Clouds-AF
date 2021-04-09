using UnityEngine;
using UnityEngine.AzureSky;

public class AzureOverridePropertiesManually : MonoBehaviour
{
    [Header("Target Components")]
    public AzureWeatherController weatherController;
    public AzureSkyRenderController skyRenderController;
    public AzureEnvironmentController environmentController;
    public AzureEffectsController effectsController;
    public Light directionalLight;


    // Take the output value from the override properties list and manually send it to the targets
    private void LateUpdate()
    {
        skyRenderController.molecularDensity = weatherController.GetOverrideFloatOutput(0, 5.09f);
        skyRenderController.rayleigh = weatherController.GetOverrideFloatOutput(1, 10f);
        skyRenderController.mie = weatherController.GetOverrideFloatOutput(2, 10f);
        skyRenderController.rayleighColor = weatherController.GetOverrideColorOutput(3);
        skyRenderController.mieColor = weatherController.GetOverrideColorOutput(4);
        skyRenderController.starsIntensity = weatherController.GetOverrideFloatOutput(5);
        skyRenderController.milkyWayIntensity = weatherController.GetOverrideFloatOutput(6);
        skyRenderController.globalFogDistance = weatherController.GetOverrideFloatOutput(7, 5500f);
        skyRenderController.globalFogSmoothStep = weatherController.GetOverrideFloatOutput(8);
        skyRenderController.globalFogDensity = weatherController.GetOverrideFloatOutput(9);
        skyRenderController.heightFogDistance = weatherController.GetOverrideFloatOutput(10, 200f);
        skyRenderController.heightFogSmoothStep = weatherController.GetOverrideFloatOutput(11);
        skyRenderController.heightFogDensity = weatherController.GetOverrideFloatOutput(12);
        skyRenderController.dynamicCloudsAltitude = weatherController.GetOverrideFloatOutput(13, 15f);
        skyRenderController.dynamicCloudsDirection = weatherController.GetOverrideFloatOutput(14);
        skyRenderController.dynamicCloudsSpeed = weatherController.GetOverrideFloatOutput(15);
        skyRenderController.dynamicCloudsDensity = weatherController.GetOverrideFloatOutput(16);
        skyRenderController.dynamicCloudsColor1 = weatherController.GetOverrideColorOutput(17);
        skyRenderController.dynamicCloudsColor2 = weatherController.GetOverrideColorOutput(18);
        directionalLight.intensity = weatherController.GetOverrideFloatOutput(19);
        directionalLight.color = weatherController.GetOverrideColorOutput(20);
        environmentController.environmentIntensity = weatherController.GetOverrideFloatOutput(21);
        environmentController.environmentAmbientColor = weatherController.GetOverrideColorOutput(22);
        environmentController.environmentEquatorColor = weatherController.GetOverrideColorOutput(23);
        environmentController.environmentGroundColor = weatherController.GetOverrideColorOutput(24);
        effectsController.lightRainIntensity = weatherController.GetOverrideFloatOutput(25);
        effectsController.mediumRainIntensity = weatherController.GetOverrideFloatOutput(26);
        effectsController.heavyRainIntensity = weatherController.GetOverrideFloatOutput(27);
        effectsController.snowIntensity = weatherController.GetOverrideFloatOutput(28);
        effectsController.rainColor = weatherController.GetOverrideColorOutput(29);
        effectsController.snowColor = weatherController.GetOverrideColorOutput(30);
        effectsController.lightRainSoundFx = weatherController.GetOverrideFloatOutput(31);
        effectsController.mediumRainSoundFx = weatherController.GetOverrideFloatOutput(32);
        effectsController.heavyRainSoundFx = weatherController.GetOverrideFloatOutput(33);
        effectsController.lightWindSoundFx = weatherController.GetOverrideFloatOutput(34);
        effectsController.mediumWindSoundFx = weatherController.GetOverrideFloatOutput(35);
        effectsController.heavyWindSoundFx = weatherController.GetOverrideFloatOutput(36);
        effectsController.windSpeed = weatherController.GetOverrideFloatOutput(37);
        effectsController.windDirection = weatherController.GetOverrideFloatOutput(38);
        skyRenderController.sunTextureIntensity = weatherController.GetOverrideFloatOutput(39);
        skyRenderController.moonTextureIntensity = weatherController.GetOverrideFloatOutput(40);
        directionalLight.shadowStrength = weatherController.GetOverrideFloatOutput(41);
        skyRenderController.staticCloudScattering = weatherController.GetOverrideFloatOutput(42, 2f);
        skyRenderController.staticCloudColor = weatherController.GetOverrideColorOutput(43);
    }
}