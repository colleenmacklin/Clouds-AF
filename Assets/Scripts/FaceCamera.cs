using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    private Transform camera;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main.transform;
        transform.LookAt(camera);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
