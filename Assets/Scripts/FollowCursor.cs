using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCursor : MonoBehaviour
{
    private Vector3 _mousePos;

    [SerializeField]
    private Camera _camera;

    private void Awake()
    {
       Cursor.visible = false;
    }

    private void Update()
    {

        _mousePos = Input.mousePosition;
        // Debug.Log(_mousePos);


        Vector3 newPos = _camera.ScreenToWorldPoint(_mousePos);
        transform.position = _camera.ScreenToWorldPoint(new Vector3(_mousePos.x, _mousePos.y, 10)) ;

        Debug.Log(newPos);
    }






}
