using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTemplateProjects;

/*
Raycaster started as a means of controlling the clicking events and having state attached for it.
It is ballooning into a more full player class. Consider a state pattern implementation.

Just in general the movement of the camera needs to be reconsidered. It shouldn't be trailing the mouse.
*/
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
        HOVERING,
        READING
    }

    MouseState state = MouseState.EMPTY;
    Vector3 lookAtSelected = new Vector3();
    //Get the camera mover so we can turn it on and off during dialogue
    public SimpleCameraController gazeMover; //attached to the camera *it probably shouldn't be
    // Raycasting variables 
    Ray ray;
    RaycastHit hit;
    LayerMask mask;

    Quaternion initialCameraRot;
    void Start()
    {
        mask = LayerMask.GetMask("Clouds");
        initialCameraRot = Camera.main.transform.localRotation;
    }

    void OnEnable()
    {
        EventManager.StartListening("ConversationEnded", StartGazeTracking);
        EventManager.StartListening("Correct", StopGazeTracking);
    }

    void OnDisable()
    {
        EventManager.StopListening("ConversationEnded", StartGazeTracking);
        EventManager.StopListening("Correct", StopGazeTracking);
    }
    void StartGazeTracking()
    {
        //StartCoroutine(ReturnToDefaultView());

        gazeMover.enabled = true;
    }
    IEnumerator ReturnToDefaultView()
    {
        while (Quaternion.Angle(Camera.main.transform.localRotation, Quaternion.Euler(new Vector3(0, 0, 0))) > .2f)
        {
            Camera.main.transform.localRotation = Quaternion.Slerp(Camera.main.transform.localRotation, Quaternion.Euler(new Vector3(0, 0, 0)), .2f);
            yield return null;
        }

        gazeMover.enabled = true;
    }
    void StopGazeTracking()
    {
        Debug.Log("hello");
        gazeMover.enabled = false;
        //look at selected 
        StartCoroutine(LookAtSelection());
    }

    IEnumerator LookAtSelection()
    {
        Quaternion rot = Quaternion.LookRotation(Selected.transform.position, Camera.main.transform.up);

        while (Quaternion.Angle(rot, Camera.main.transform.localRotation) > .2f)
        {
            Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, rot, .2f);
            yield return null;
        }
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
                    lookAtSelected = Selected.transform.position - Camera.main.transform.position;
                    EventManager.TriggerEvent("Respond");
                }
                break;
            case MouseState.READING:

                break;
        }
    }

}
