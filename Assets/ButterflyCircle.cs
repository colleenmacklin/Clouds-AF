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
        float x = Mathf.Cos(timeCounter*2);
        float y = Mathf.Sin(timeCounter*2);
        float z = 100;
        transform.position = new Vector3((x * 4 / 5)+1, y * 4 / 5, z);
    }
}
