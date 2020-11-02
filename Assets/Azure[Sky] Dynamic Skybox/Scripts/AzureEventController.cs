using System;
using System.Collections.Generic;

namespace UnityEngine.AzureSky
{
    [AddComponentMenu("Azure[Sky]/Azure Event Controller")]
    public class AzureEventController : MonoBehaviour
    {
        // References
        private AzureSkyController m_skyController;
        private AzureTimeController m_timeController;

        public AzureEventScanMode scanMode = AzureEventScanMode.ByMinute;
        public List<AzureEventAction> eventList = new List<AzureEventAction>();
        
        private void OnEnable()
        {
            m_skyController = GetComponent<AzureSkyController>();
            m_timeController = GetComponent<AzureTimeController>();
            m_skyController.onMinuteChange.AddListener(OnMinuteChange);
            m_skyController.onHourChange.AddListener(OnHourChange);
        }

        private void OnDisable()
        {
            m_skyController.onMinuteChange.RemoveListener(OnMinuteChange);
            m_skyController.onHourChange.RemoveListener(OnHourChange);
        }


        private void Start()
        {
            m_skyController = GetComponent<AzureSkyController>();
            m_timeController = GetComponent<AzureTimeController>();
        }

        private void OnMinuteChange()
        {
            if (scanMode == AzureEventScanMode.ByMinute)
                ScanEventList();
        }
        
        private void OnHourChange()
        {
            if (scanMode == AzureEventScanMode.ByHour)
                ScanEventList();
        }

        /// <summary>
        /// Scans the event system list and perform the event actions that match with the current date and time.
        /// </summary>
        private void ScanEventList()
        {
            if (eventList.Count <= 0)
                return;
            
            for (int i = 0; i < eventList.Count; i++)
            {
                if (eventList[i].eventAction == null)
                    continue;
                if (eventList[i].year != m_timeController.year && eventList[i].year != -1)
                    continue;
                if (eventList[i].month != m_timeController.month && eventList[i].month != -1)
                    continue;
                if (eventList[i].day != m_timeController.day && eventList[i].day != -1)
                    continue;
                if (eventList[i].hour != m_timeController.hour && eventList[i].hour != -1)
                    continue;
                if (eventList[i].minute != m_timeController.minute && eventList[i].minute != -1)
                    continue;
                eventList[i].eventAction.Invoke();
            }
        }
    }
}