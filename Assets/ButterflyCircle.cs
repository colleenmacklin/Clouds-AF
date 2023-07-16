using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButterflyCircle : MonoBehaviour
{

    float timeCounter = 0;

    void Update()
    {

        //make butterfly move in a circle over time
        timeCounter += Time.deltaTime;
        float x = Mathf.Cos(timeCounter);
        float y = Mathf.Sin(timeCounter);
        float z = 100;
        transform.position = new Vector3(x, y, z);
    }
}
