using System;
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
public class Raycaster : MonoBehaviour
{
    public event Action<GameObject> OnHoverOverTargetCloud;
    public event Action OnHoverExit;
    public GameState gameState;
   

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
    public TextBoxController textBoxControl;
    public ButterflyController butterflyControl;

    //View FOcus settings
    [SerializeField]
    [Range(.01f, 1f)]
    private float focusInSpeed = .01f;
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
        Cursor.visible = false;
        mask = LayerMask.GetMask("Clouds");
       gazeMover.enabled = false;
       // initialCameraRot = Camera.main.transform.localRotation;
    }

    void OnEnable()
    {
        EventManager.StartListening("ConversationEnded", StartGazeTracking);
        EventManager.StartListening("Musing", StopGazeTracking);
        EventManager.StartListening("Cutscene", ReadingMode);

       
    }

    void OnDisable()
    {
        EventManager.StopListening("ConversationEnded", StartGazeTracking);
        EventManager.StopListening("Musing", StopGazeTracking);
        EventManager.StopListening("Cutscene", ReadingMode);
    }

    void ReadingMode()
    {
        state = MouseState.READING;
        //StartGazeTracking(); //shouldnt this be stop gazeTracking? //CM COMMENTED OUT 7/31
    }

    //None of the tracking should be doing as many mutations as it is now
    //gazeMover, state, and the coroutines all require some reconfiguration in the future
    void StartGazeTracking()
    {
        if (activeCoroutine != null)
        {

            StopCoroutine(activeCoroutine);

        }
       // activeCoroutine = StartCoroutine(ReturnToDefaultView());

        gazeMover.enabled = true;
        state = MouseState.EMPTY;

    }
 /*   IEnumerator ReturnToDefaultView()
    {
        while (Quaternion.Angle(Camera.main.transform.localRotation, Quaternion.identity) > 10f)
        {
           // Camera.main.transform.localRotation = Quaternion.Slerp(Camera.main.transform.localRotation, Quaternion.identity, focusOutSpeed);

            //returning to center
            yield return null;
        }
    }*/
    void StopGazeTracking()
    {
        ReadingMode();
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }
        gazeMover.enabled = false;
        activeCoroutine = StartCoroutine(LookAtSelection());
        //EventManager.TriggerEvent("closeEye");
    }

    //CM turned this on again so that the centire cloud can be seen when it is being talked about (7/30/2023)
    //Look directly at target
    IEnumerator LookAtSelection()
    {
        Quaternion rot = Quaternion.LookRotation(Selected.transform.position, Camera.main.transform.up);

        while (Quaternion.Angle(rot, Camera.main.transform.localRotation) > 1f)
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
                if (hit.transform)
                {
                   
                    state = MouseState.HOVERING;
                    //EventManager.TriggerEvent("openEye");
                    //callback to start butterfly glow - when entering cloud over hover
                    OnHoverOverTargetCloud?.Invoke(hit.transform.gameObject);
                }
                else
                {
                    OnHoverExit?.Invoke(); //DeGlow callback on Butterfly
                }
                break;

            case MouseState.HOVERING:
                //if hovering and no hit, then switch to empty
                if (!hit.transform)
                {
                    
                    state = MouseState.EMPTY;
                    Selected = null;
                    //EventManager.TriggerEvent("closeEye");

                    //if exit cloud then stop glow
                    OnHoverExit?.Invoke(); //DeGlow callback on Butterfly
                }
                else
                {
                    OnHoverOverTargetCloud?.Invoke(hit.transform.gameObject); //calbackk to butterfly startglow
                }               
                break;

            case MouseState.READING:

                if (gameState.Gameloop)
                {
                    StopGazeTracking();
                    butterflyControl._isTalking = true;
                }
                    //textBoxControl.Check();//bad mutation management.
                
                break;
        }


    }

    public void StartCloudTalking()
    {
        Selected = hit.transform.gameObject;

        GameObject c = Selected;

        Actions.GetClickedCloud(c); //lets cloudmanager know which cloud has been clicked

        EventManager.TriggerEvent("Respond");
        state = MouseState.READING;
    }

}
