using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

//script is for demo purposes only. Shouldn't be used in a production setting as all the UI work should be handled elsewhere.

public class QT_InteractContainer : MonoBehaviour
{
    public GameObject ContainerTop;
    public GameObject[] raycastObjects;
    public string OpenText = "Press E to Open.";
    public string CloseText = "Press E to Close.";
    public AnimationClip OpenClip, CloseClip;
    private bool isOpen = false;
    private Animator Anim;
    private bool isTriggered = false;

   
    public Text uiCanvasText;
    private Canvas uiCanvas;
    private List<Collider> colliders = new List<Collider>();
    

    // Use this for initialization
    void Start()
    {        
        uiCanvasText.gameObject.SetActive(false);       
        Anim = ContainerTop.GetComponent<Animator>();
    
        if(raycastObjects.Length==0)
            Debug.LogError("Raycast Objects length is 0 in Gameobject: "+this.gameObject);
        else
        for (int x = 0; x < raycastObjects.Length; x++)
        {
            colliders.Add(raycastObjects[x].GetComponent<Collider>());
            if (colliders[x]==null)            
                Debug.LogError("Gameobject: " + this.gameObject + " at "+this.gameObject.transform.position+" has invalid entry for Raycast Objects array. Add a collider to it or replace the gameobject.\nParent: "+this.transform.parent.name);
            
        }

        uiCanvas = uiCanvasText.canvas;
    }

   void Update()
    {
        if(isTriggered)
        {
            
            if (Input.GetButtonUp("Fire1"))
            {                
                if (!isOpen)
                {
                    Anim.SetBool(CloseClip.name, false);
                    Anim.SetBool(OpenClip.name, true);
                    isOpen = true;              
                }
                else
                {
                    Anim.SetBool(CloseClip.name, true);                                     
                    Anim.SetBool(OpenClip.name, false);
                    isOpen = false;
                }
                
            }
        }
    }

    private bool HitValidObject(Collider hit)
    {
        bool isValid = false;
        foreach(Collider c in colliders)
        {
            if (hit == c)
            {
                isValid = true;
                break;
            }
        }
        return isValid;
    }
    void OnTriggerStay()
    {
       
        //transform.rotation = Quaternion.LookRotation(transform.position - target.position);
        uiCanvas.transform.rotation = Quaternion.LookRotation(uiCanvas.transform.position - Camera.main.transform.position);
        Vector3 rayStart = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
        Vector3 rayDir = Camera.main.transform.forward;
        RaycastHit rayHit;

        if (Physics.Raycast(rayStart, rayDir, out rayHit, 5.0f))
        {
            
            if (HitValidObject(rayHit.collider) && !isOpen)
            {
                uiCanvasText.gameObject.SetActive(true);
                uiCanvasText.text = OpenText;
                isTriggered = true;

            }
            else if (HitValidObject(rayHit.collider) && isOpen)
            {
                uiCanvasText.gameObject.SetActive(true);
                uiCanvasText.text = CloseText;
                isTriggered = true;
            }

            else
            {
                uiCanvasText.gameObject.SetActive(false);
                isTriggered = false;
            }

        }
    }

    void OnTriggerExit()
    {
        uiCanvasText.gameObject.SetActive(false);
        isTriggered = false;
    }

}
