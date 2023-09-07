using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEngine.GraphicsBuffer;

public class FollowCursor : MonoBehaviour
{
    public Vector3 _mousePos;
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
        _mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        var mouseVector = new Vector3(_mousePos.x, _mousePos.y, 100);
        transform.position = mouseVector; //set the butterfly to the mouse cursor
    }

}







