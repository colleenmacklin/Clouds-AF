using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycaster : MonoBehaviour
{

    //maybe this needs to be more available than what it is.
    public GameObject Selected
    {
        get;
        private set;
    }

    private enum MouseState
    {
        EMPTY,
        HOVERING
    }

    MouseState state = MouseState.EMPTY;
    // Raycasting variables 
    Ray ray;
    RaycastHit hit;
    LayerMask mask;

    void Start()
    {
        mask = LayerMask.GetMask("Clouds");
    }
    void Update()
    {
        //always cast the ray
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit, Mathf.Infinity, mask);

        switch (state)
        {
            case MouseState.EMPTY:
                //if empty and hit, then switch to hovering
                if (hit.transform)
                {
                    state = MouseState.HOVERING;
                    Debug.Log("hitting");
                    EventManager.TriggerEvent("openEye");
                }
                break;
            case MouseState.HOVERING:
                //if hovering and no hit, then switch to empty
                if (!hit.transform)
                {
                    state = MouseState.EMPTY;
                    Selected = null;
                    EventManager.TriggerEvent("closeEye");
                }
                if (Input.GetMouseButtonDown(0))
                {
                    Selected = hit.transform.gameObject;
                    EventManager.TriggerEvent("Respond");
                }
                break;
        }
    }

}
