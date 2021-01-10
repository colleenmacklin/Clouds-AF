using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameCloudLayerGroup : MonoBehaviour
{
    
    //brought over from Game_Cloud
    private UnityAction someListener;
    private Transform camera;
    private int cloudNum;
    private GameObject target;
    //private ParticleSystem ps;

    //public Game_Cloud highClouds;
    //public Game_Cloud lowClouds;

    public GameObject highClouds;
    public GameObject lowClouds;

    //public ParticleSystem.ShapeModule myShape;
    //stuff to do:
    // --add slightly different timers to shape changing
    // --destroy myself when I blow offscreen

    public bool stopSpawning = false;
    public float spawnTime;
    public float spawnDelay;

    public GameObject cloudName;
    public Texture2D[] cloudShapes; //generic cloudshapes
    public bool isShape; //is this cloud turned into a shape?
    public string myName; //store the name of the underlying shape here
    public Texture2D new_Shape;
    public Texture2D curr_Shape;

    public ParticleSystem h_ps;
    public ParticleSystem l_ps;
    private ParticleSystem.ShapeModule ShapeH;
    private ParticleSystem.ShapeModule ShapeL;

    void OnEnable()
    {
        EventManager.StartListening("SpawnShape", SpawnShape);
        EventManager.StartListening("TurnOffCloud", turnOff);
    }

    void OnDisable()
    {
        EventManager.StopListening("SpawnShape", SpawnShape);
        EventManager.StopListening("TurnOffCloud", turnOff);
    }

    private void Start()
    {

        //rotate to look at the camera
        camera = Camera.main.transform;
        transform.LookAt(camera, Vector3.back);
        //transform.LookAt(camera, Vector3.forward);
        curr_Shape = h_ps.shape.texture;

        cloudNum = (Random.Range(0, cloudShapes.Length));
        InvokeRepeating("ChangeCloudShape", 0, 0);
    }

    void SpawnShape() //called from CloudManager
    {

        target = GameObject.FindWithTag("CloudManager").GetComponent<Game_CloudManager>().chosenCloud;


        if (target == cloudName) //if that's me
        {

            isShape = true;

            //do stuff
            var shape = GameObject.FindWithTag("CloudManager").GetComponent<Game_CloudManager>().chosenShape;
            Debug.Log("spawnshape  called, target is: " + target.name + " shape is: " + shape.name);


            ShapeH = h_ps.shape;
            ShapeL = l_ps.shape;
            
            ShapeH.texture = shape;
            ShapeL.texture = shape;

            //Debug.Log("hi, I am a shape: " + low_ns.sprite.name);
            CancelInvoke("ChangeCloudShape");

        }
        else { isShape = false; }
        //stop rotating through regular clouds, now I am a shape - but need to restart invoke after I am discovered.
    }

    public void turnOff()
    {
        //stop the cloud from constantly shifting cloud shapes
        //Debug.Log("turning off cloud");
        isShape = false;
        InvokeRepeating("ChangeCloudShape", spawnTime, spawnDelay);
    }

    void ChangeCloudShape() //this should be on some kind of timer. Should Cloudmanager call this?
    {
        if (isShape) //if this is a special shape cloud, we ignore the constant changing of underlying cloud shapes
        {
            CancelInvoke("ChangeCloudShape");
            return;
        }
        else if (cloudShapes.Length>0)
        {

            cloudNum = (Random.Range(0, cloudShapes.Length));
            ShapeH = h_ps.shape;
            ShapeL = l_ps.shape;


            Texture2D cloudShape = cloudShapes[cloudNum];
            ShapeH.texture = cloudShape;
            ShapeL.texture = cloudShape;
        }
    }


    private void OnMouseDown()
    {
        Debug.Log("clicked on: " + this.gameObject.name);

        if (isShape)
        {
            EventManager.TriggerEvent("Respond");
            EventManager.TriggerEvent("shapeEye");
        }
        EventManager.TriggerEvent("glowEye");

    }
    void OnMouseOver()
    {
        EventManager.TriggerEvent("openEye");
    }

    void OnMouseExit()
    {
        EventManager.TriggerEvent("closeEye");
    }

}
