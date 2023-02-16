using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
namespace UnityEngine.AzureSky
{

    public class sky : MonoBehaviour
    {

        public AzureTimeController azureTimeController;
        public AzureWeatherController weatherController;
        public int sunrise_start_time = 5;
        public int sunrise_end_time_hour = 12;
        public int sunrise_end_time_minute = 0;
        public int sunset_start_time;
        public int sunset_end_time_hour = 24;
        public int sunset_end_time_minute = 0;
        public float timeScale = 0.005f;
        //TODO: tweak the AzureSky materials stuff to make better colaration for sunrise and sunset

        private float[] _timeLineTimes;

        //hacky way to say that this is going to be null in the opening scene 
        [CanBeNull]
        [SerializeField]
        private Storyteller _storyteller;

        private float _startTime;

        private bool _timeLineIsPaused;

        // Start is called before the first frame update
        void OnEnable()
        {
            EventManager.StartListening("sunrise", sunrise);
            EventManager.StartListening("sunset", sunset);
           // EventManager.StartListening("Musing", CatchUpToTimeAtMusing);

        }

        void OnDisable()
        {
            EventManager.StopListening("sunrise", sunrise);
            EventManager.StopListening("sunset", sunset);
         //   EventManager.StopListening("Musing", CatchUpToTimeAtMusing);

        }
        private void Awake()
        {
            _startTime = azureTimeController.GetTimeline();
        }
        void Start()
        {

            if (_storyteller != null)
            {
                CaculateTimeLineTimes();
            }
        }


        void Update()
        {
            //check if timeline has reached each value, if not pause; 
            //after musing, unpause or catch up
            if (_storyteller != null)
            {
                CheckTimeLineAgainstTimes();
            }
          


        }

        private void sunrise() //consider converting to an action with the timeScale set by gameManager/cloudManager
        {
            //TODO: Slow the sunrise speed to conform to loading the GPT model
            Debug.Log("Starting Sunrise");
            azureTimeController.SetTimeline(sunrise_start_time);
            azureTimeController.StartTimelineTransition(sunrise_end_time_hour, sunrise_end_time_minute, timeScale, AzureTimeDirection.Forward);

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
            //  azureTimeController.SetTimeline(current_hour);
            azureTimeController.StartTimelineTransition(sunset_end_time_hour, sunset_end_time_minute, timeScale, AzureTimeDirection.Forward);
        }

        /// <summary>
        ///  setting up the array for each point in the timeline to hold at until each musing is reached
        /// </summary>
        private void CaculateTimeLineTimes()
        {
            //consider making the array one size bigger and adding end goal point, i.e. taking it out of the 'sunset' method
            int arrayLength = _storyteller.GetNumberOfMusings();


            _timeLineTimes = new float[arrayLength];

            //set the last two to be goldenish colors 
            //hardcoding these values for now but could consider exposing them
            _timeLineTimes[arrayLength - 1] = 17.5f;
            _timeLineTimes[arrayLength - 2] = 16.7f;

            //also hardcoding start time, again, could expose 
            // second last time - start time is the time frame we're working with 


            //caluate the difference between the remaining values
            float diff = (16.7f - _startTime) / (arrayLength - 1);

            for (int i = 1; i < arrayLength - 1; i++)
            {
                _timeLineTimes[i - 1] = _startTime + i * diff;
            }
        }

       
        /// <summary>
        /// checks if the timeline has gone past the value of the array set for that particular musing, and pauses time if so
        /// </summary>

        private void CheckTimeLineAgainstTimes()
        {

            if (!_timeLineIsPaused)
            {
                if (azureTimeController.GetTimeline() >= _timeLineTimes[_storyteller.GetMusingsGiven()])
                {
                    azureTimeController.PauseTime();
                    _timeLineIsPaused = true;
                }
            }
           
        }

        private void CatchUpToTimeAtMusing()
        {
            if (_timeLineIsPaused)
            {
                azureTimeController.PlayTimeAgain();
                _timeLineIsPaused = false;
            }

            //this value is gona be wrong possibly - check musings given value at these points
            //also check that the conversion in the transition can use this number

            

            if (azureTimeController.GetTimeline() <= _timeLineTimes[_storyteller.GetMusingsGiven()] )
            {

                string timelineStringTime = _timeLineTimes[_storyteller.GetMusingsGiven()].ToString();

                string[] times = timelineStringTime.Split(".");
                int hour = int.Parse((times[0]));
                int minute = int.Parse((times[1])) / 100 * 60;

                azureTimeController.StartTimelineTransition(hour, minute, 1f, AzureTimeDirection.Forward); 
            }

        }

    }

}