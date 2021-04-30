using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AzureSky;

public class AzureSliderToTime : MonoBehaviour
{
    public AzureTimeController azureTimeController;

    public void SetAzureTimeline(Slider timeSlider)
    {
        azureTimeController.SetTimeline(timeSlider.value);
    }
}