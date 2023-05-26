using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCursor : MonoBehaviour
{
    private Vector3 _mousePos;

   
    private Camera _camera;

    private void Awake()
    {
        //_camera = Camera.main;
        foreach(Camera c in Camera.allCameras)
        {
            if (c.gameObject.name == "ButterflyCam")
            {
                _camera = c;
            }
        }
       
       Cursor.visible = false;
    }

    private void Update()
    {
        Cursor.visible = false;
        _mousePos = Input.mousePosition;
        // Debug.Log(_mousePos);


        Vector3 newPos = _camera.ScreenToWorldPoint(_mousePos);
        transform.position = _camera.ScreenToWorldPoint(new Vector3(_mousePos.x, _mousePos.y, 100)) ;

      //  Debug.Log(newPos);
    }






}
