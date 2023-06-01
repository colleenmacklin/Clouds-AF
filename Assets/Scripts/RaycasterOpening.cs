using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTemplateProjects;

/*
Raycaster started as a means of controlling the clicking events and having state attached for it.
It is ballooning into a more full player class. Consider a state pattern implementation.

Just in general the movement of the camera needs to be reconsidered. It shouldn't be trailing the mouse.
Movement of the mouse should be made relative to the screen rect bounds.
*/
public class RaycasterOpening : MonoBehaviour
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
    //public TextBoxController textBoxControl;


    //View FOcus settings
    [SerializeField]
    [Range(.01f, 1f)]
    private float focusInSpeed = .2f;
    [SerializeField]
    [Range(.01f, 1f)]
    private float focusOutSpeed = .01f;

    // Raycasting variables 
    Ray ray;
    RaycastHit hit;
    LayerMask mask;

    Quaternion initialCameraRot;
    Coroutine activeCoroutine;
    void Start()
    {
        mask = LayerMask.GetMask("Clouds");
        gazeMover.enabled = false;
        initialCameraRot = Camera.main.transform.localRotation;
        StartGazeTracking();
    }

    void OnEnable()
    {
        //EventManager.StartListening("ConversationEnded", StartGazeTracking);
        //EventManager.StartListening("Musing", StopGazeTracking);
        //EventManager.StartListening("Cutscene", ReadingMode);
    }

    void OnDisable()
    {
        //EventManager.StopListening("ConversationEnded", StartGazeTracking);
        //EventManager.StopListening("Musing", StopGazeTracking);
        //EventManager.StopListening("Cutscene", ReadingMode);
    }
    
    void ReadingMode()
    {
        state = MouseState.READING;
    }

    //None of the tracking should be doing as many mutations as it is now
    //gazeMover, state, and the coroutines all require some reconfiguration in the future
    void StartGazeTracking()
    {
        if (activeCoroutine != null)
        {

            StopCoroutine(activeCoroutine);

        }
        activeCoroutine = StartCoroutine(ReturnToDefaultView());

        gazeMover.enabled = true;
        state = MouseState.EMPTY;

    }
    IEnumerator ReturnToDefaultView()
    {
        while (Quaternion.Angle(Camera.main.transform.localRotation, Quaternion.identity) > 10f)
        {
            Camera.main.transform.localRotation = Quaternion.Slerp(Camera.main.transform.localRotation, Quaternion.identity, focusOutSpeed);

            //returning to center
            yield return null;
        }
    }
    void StopGazeTracking()
    {
        ReadingMode();
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }
        gazeMover.enabled = false;
        activeCoroutine = StartCoroutine(LookAtSelection());
        EventManager.TriggerEvent("closeEye");
    }

    //Look directly at target
    //Does not end because the Quaternion Angle is in the wrong relative space (local to world angle comparison gives high values like 60 degrees).
    IEnumerator LookAtSelection()
    {
        Quaternion rot = Quaternion.LookRotation(Selected.transform.position, Camera.main.transform.up);

        while (Quaternion.Angle(rot, Camera.main.transform.localRotation) > 1f)// these values are just wrong
        {
            Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, rot, focusInSpeed);

            //Debug.Log($"looking at target, {rot},{Camera.main.transform.localRotation}");
            yield return null;
        }

        Debug.Log("TargetFound");
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
                //if (hit.transform)
               
                if (hit.transform)

                {
                    Selected = hit.transform.gameObject;

                    GameObject c = Selected;
                    OpeningCloudShape select_c = c.GetComponent<OpeningCloudShape>();

                    if (select_c.isTarget)
                    {
                        state = MouseState.HOVERING;
                        EventManager.TriggerEvent("openEye");
                    }

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

                    GameObject c = Selected;
                    
                    Actions.GetClickedCloud(c); //lets cloudmanager know which cloud has been clicked

                    lookAtSelected = Selected.transform.position - Camera.main.transform.position;
                    //EventManager.TriggerEvent("Respond");
                }
                break;
            case MouseState.READING:

                if (Input.GetMouseButtonDown(0))
                {
                    //in the future this should be some sort of reading state input
                    //textBoxControl.Check();//bad mutation management.
                }
                break;
        }
    }

}
