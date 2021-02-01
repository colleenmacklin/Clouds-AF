using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameCloudLayerGroup : MonoBehaviour
{
    
    //brought over from Game_Cloud
    private UnityAction someListener;
    private Transform camera;
    public int cloudNum;
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

    //for debugging
    public Rect hiRect;
    public GameObject UnderlyingShape;

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
        //transform.LookAt(camera, Vector3.up);
        curr_Shape = h_ps.shape.texture;
        hiRect = UnderlyingShape.GetComponent<myShape>().myRect;

        //cloudNum = (Random.Range(0, cloudShapes.Length)); //set random cloudshape on instantiate
        cloudNum = 0;
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

            EventManager.TriggerEvent("UpdateMe"); //tell components to update -- there may be a more efficient way to do this so it doesn't call every cloud object

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
            cloudNum = 0;
            //cloudNum = (Random.Range(0, cloudShapes.Length));
            ShapeH = h_ps.shape;
            ShapeL = l_ps.shape;


            Texture2D cloudShape = cloudShapes[cloudNum];
            ShapeH.texture = cloudShape;
            ShapeL.texture = cloudShape;
            EventManager.TriggerEvent("UpdateMe"); //tell components to update, again, might be a more performant/efficient way to do this


        }
    }

    //for testing
    void ChangeCloudShape_Sequence() //this should be on some kind of timer. Should Cloudmanager call this?
    {
        cloudNum++;
        Debug.Log("cloudnnum: " + cloudNum);

        if (cloudNum > cloudShapes.Length - 1) { 
            cloudNum = 0;
        }

        ShapeH = h_ps.shape;
        ShapeL = l_ps.shape;
        Texture2D cloudShape = cloudShapes[cloudNum];
        ShapeH.texture = cloudShape;
        ShapeL.texture = cloudShape;
        EventManager.TriggerEvent("UpdateMe"); //tell components to update, again, might be a more performant/efficient way to do this
        Debug.Log("cloud changing to a: " + cloudShapes[cloudNum].name);
    }


    //added for testing
    private void Update()
    {
        if (Input.GetKeyDown("a"))
        {
            ChangeCloudShape_Sequence();
        }

    }

        void OnMouseDown()
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
