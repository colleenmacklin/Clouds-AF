using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityEngine.AzureSky
{

    public class sky : MonoBehaviour
    {

        public AzureTimeController azureTimeController;
        public AzureWeatherController weatherController;
        public int sunrise_start_time = 5;
        public int sunrise_end_time = 13;
        public int sunset_start_time;
        public int sunset_end_time = 24;


        // Start is called before the first frame update
        void OnEnable()
        {
            EventManager.StartListening("sunrise", sunrise);
            EventManager.StartListening("sunset", sunset);

        }

        void OnDisable()
        {
            EventManager.StopListening("sunrise", sunrise);
            EventManager.StopListening("sunset", sunset);

        }

        void Start()
        {
            //azureTimeController.PauseTime;
            //sunrise();
        }

        // Update is called once per frame
        void Update()
        {
            /*
            if (azureTimeController.GetTimeOfDay().x > 13)
            {
                weatherController.SetNewWeatherProfile(1);
                Debug.Log("weather changed to: " + weatherController.GetCurrentWeatherProfile().name);
                return;
            }
            */
        }

        private void sunrise()
        {
            Debug.Log("Starting Sunrise");
            azureTimeController.SetTimeline(sunrise_start_time);
            azureTimeController.StartTimelineTransition(sunrise_end_time, 0, 1.5f, AzureTimeDirection.Forward);

            /*
            Vector2 time_of_day = azureTimeController.GetTimeOfDay();

            if (time_of_day.x > 11)
            {
                weatherController.SetNewWeatherProfile(1);

            }
            */

        }

        private void sunset()
        {
            Debug.Log("Starting Sunset");
            var current_hour = azureTimeController.GetTimeOfDay().x;
            var current_minute = azureTimeController.GetTimeOfDay().y;
            azureTimeController.SetTimeline(current_hour);
            azureTimeController.StartTimelineTransition(sunset_end_time, 0, 1.5f, AzureTimeDirection.Forward);


        }

    }

}