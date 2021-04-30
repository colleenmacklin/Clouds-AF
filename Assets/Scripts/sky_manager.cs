using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityEngine.AzureSky

{
    //behaviors:
 
    //the time increments each time a new shape is found
    //sunset happens at the end of the game

    public class sky_manager : MonoBehaviour

    {
        public AzureTimeController azureTimeController;
        public AzureWeatherController weatherController;

        void OnEnable()
        {
            EventManager.StartListening("timePasses", timePasses);
        }

        void OnDisable()
        {
            EventManager.StopListening("timePasses", timePasses);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void timePasses()
        {
            var currentHour = azureTimeController.GetTimeOfDay().x;
            var currentMinute = azureTimeController.GetTimeOfDay().y;

            azureTimeController.StartTimelineTransition(Mathf.RoundToInt(currentHour)+1, Mathf.RoundToInt(currentMinute), 2, AzureTimeDirection.Forward);
        }
    }
}
