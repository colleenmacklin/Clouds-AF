using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityEngine.AzureSky {

public class sky : MonoBehaviour
{
    //public AzureSkyController azureSky;
    // Start is called before the first frame update
    void OnEnable()
    {
        //EventManager.StartListening("sunrise", sunrise);
    }

    void OnDisable()
    {
        //EventManager.StopListening("sunrise", sunrise);
    }

    void Start()
    {
            //azureSky.SetTimelineSourceTransitionTime(10);
            //azureSky.StartTimelineTransition(17);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
}
