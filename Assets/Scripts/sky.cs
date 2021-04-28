using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityEngine.AzureSky
{

    public class sky : MonoBehaviour
    {

        public AzureTimeController azureTimeController;
        public int start_time = 5;
        public int end_time = 15;


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

        }

        private void sunrise()
        {
            azureTimeController.SetTimeline(start_time);
            azureTimeController.StartTimelineTransition(end_time, 0, 3, AzureTimeDirection.Forward);
        }

    }
}

