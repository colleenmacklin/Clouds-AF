using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityEngine.AzureSky
{

    public class sky : MonoBehaviour
    {

        public AzureTimeController azureTimeController;
        public AzureWeatherController weatherController;
        public int start_time = 5;
        public int end_time = 13;


        // Start is called before the first frame update
        void OnEnable()
        {
            EventManager.StartListening("sunrise", sunrise);
        }

        void OnDisable()
        {
            EventManager.StopListening("sunrise", sunrise);
        }

        void Start()
        {
            sunrise();
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
            azureTimeController.SetTimeline(start_time);
            azureTimeController.StartTimelineTransition(end_time, 0, 1.5f, AzureTimeDirection.Forward);

            /*
            Vector2 time_of_day = azureTimeController.GetTimeOfDay();

            if (time_of_day.x > 11)
            {
                weatherController.SetNewWeatherProfile(1);

            }
            */

        }

    }
}

