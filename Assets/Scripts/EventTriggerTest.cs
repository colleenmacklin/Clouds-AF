using UnityEngine;
using System.Collections;

public class EventTriggerTest : MonoBehaviour
{


    void Update()
    {
        if (Input.GetKeyDown("q"))
        {
            EventManager.TriggerEvent("test");
        }
        /*
        if (Input.GetKeyDown("o"))
        {
            EventManager.TriggerEvent("SpawnShape");
        }
        */
        if (Input.GetKeyDown("p"))
        {
            EventManager.TriggerEvent("Destroy");
        }

        if (Input.GetKeyDown("x"))
        {
            EventManager.TriggerEvent("ChangeCloudShape");
        }
    }
}

