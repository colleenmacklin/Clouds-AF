using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCursor : MonoBehaviour
{
    private Vector3 _mousePos;

   
    private Camera _camera;

    private void Awake()
    {
        
        foreach(Camera c in Camera.allCameras)
        {
            if (c.gameObject.name == "ButterflyCam")
            {
                _camera = c;
            }
        }
       
    }

    private void Update()
    {
        _mousePos = Input.mousePosition;
      

        Vector3 newPos = _camera.ScreenToWorldPoint(_mousePos);
        transform.position = _camera.ScreenToWorldPoint(new Vector3(_mousePos.x, _mousePos.y, 100)) ;

    }






}
