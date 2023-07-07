using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class FollowCursor : MonoBehaviour
{
    private Vector3 _mousePos;

    private Vector3 _butterflyRotInitial;

    private Camera _camera;


    private Vector3 _normalizedMousePos;

    private void Awake()
    {

        _butterflyRotInitial = this.transform.eulerAngles;
        

        foreach(Camera c in Camera.allCameras)
        {
            if (c.gameObject.name == "ButterflyCam")
            {
                _camera = c;
            }
        }
       
    }

    Vector3 delta = Vector3.zero;
    Vector3 lastPos = Vector3.zero;

    float ver = 0;
    float hor = 0;
    private void Update()
    {
        _mousePos = Input.mousePosition;
        Vector3 newPos = _camera.ScreenToWorldPoint(_mousePos);
       
        transform.position = _camera.ScreenToWorldPoint(new Vector3(_mousePos.x, _mousePos.y, 100)) ;

       
    }


}
        






