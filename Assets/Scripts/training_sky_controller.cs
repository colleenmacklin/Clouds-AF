using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityEngine.AzureSky
{

    public class training_sky_controller : MonoBehaviour
    {
        public AzureTimeController azureTimeController;

        // Start is called before the first frame update
        void Start()
        {
            azureTimeController.PauseTime();

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
