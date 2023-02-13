﻿using JetBrains.Annotations;
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
        private void Awake()
        {
            _startTime = azureTimeController.GetTimeline();
        }
        void Start()
        {
            //azureTimeController.PauseTime;
            //sunrise();

            if (_storyteller != null)
            {
                CaculateTimeLineTimes();
            }
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

            if (Input.GetKeyDown(KeyCode.G))
            {
                sunset();
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
        private void  CaculateTimeLineTimes()
        {
            //consider making the array one size bigger and adding end goal point, i.e. taking it out of the 'sunset' method
            int arrayLength = _storyteller.GetNumberOfMusings();

            
            _timeLineTimes = new float[arrayLength];

            //set the last two to be goldenish colors 
            //hardcoding these values for now but could consider exposing them
            _timeLineTimes[arrayLength-1] = 17.5f;
            _timeLineTimes[arrayLength - 2] = 16.7f;

            //also hardcoding start time, again, could expose 
            // second last time - start time is the time frame we're working with 

           
            //caluate the difference between the remaining values
            float diff = (16.7f - _startTime) / (arrayLength - 1);

            for (int i = 1; i < arrayLength - 1 ; i++)
            {
                _timeLineTimes[i-1] = _startTime + i * diff;
            }
        }



        ///logic to change light during game, without ending the day before the game is over 
        ///

        //number of musings
        //sunset end time// target time
        //different light times to make sure we hit those 
        //start at 11

        

        /*
         5 musings

        11 12 13 14 15 16 


        total number of musings 


        16.7 - 11 / musings = 2

        //array 
        each next timeline point 
        size is total of musings
        last two values we know 

        1


        */

        // second last at 16:45?
        //last musing at 17.5


        //end at 19


        //

    }

}