using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameCloudGroup : MonoBehaviour
{

    //brought over from Game_Cloud
    private UnityAction someListener;
    private Transform camera;
    public int cloudNum;
    private GameObject target;
    //private ParticleSystem ps;

    //public Game_Cloud cloud;
    //public Game_Cloud lowClouds;

    public GameObject cloud;

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

    public ParticleSystem ps;
    private ParticleSystem.ShapeModule Shape;

    //for debugging
    public Rect rect;
    //public GameObject UnderlyingShape;

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

        curr_Shape = ps.shape.texture;

        //cloudNum = (Random.Range(0, cloudShapes.Length)); //set random cloudshape on instantiate
        cloudNum = 0;

    }

    void SpawnShape() //called from CloudManager
    {

        target = GameObject.FindWithTag("CloudManager").GetComponent<Game_CloudManager>().chosenCloud;

        if (target == gameObject) //if that's me
        {

            isShape = true;

            //do stuff
            var shape = GameObject.FindWithTag("CloudManager").GetComponent<Game_CloudManager>().chosenShape;
            Debug.Log("spawnshape  called, target is: " + target.name + " shape is: " + shape.name);

            Shape = ps.shape;

            Shape.texture = shape;
            curr_Shape = shape;
            //Debug.Log("hi, I am a shape: " + low_ns.sprite.name);
            CancelInvoke("ChangeCloudShape");


        }
        else { isShape = false; }
        //stop rotating through regular clouds, now I am a shape - but need to restart invoke after I am discovered.
    }

    public void turnOff()
    {
        //stop the cloud from constantly shifting cloud shapes
        Debug.Log("turning off cloud: " + this.name);
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
        else if (cloudShapes.Length > 0)
        {
            cloudNum = (Random.Range(0, cloudShapes.Length));
            Shape = ps.shape;


            Texture2D cloudShape = cloudShapes[cloudNum];
            Shape.texture = cloudShape;
            curr_Shape = cloudShape;
        }
    }

    //for testing
    void ChangeCloudShape_Sequence() //this should be on some kind of timer. Should Cloudmanager call this?
    {
        cloudNum++;
        Debug.Log("cloudnnum: " + cloudNum);

        if (cloudNum > cloudShapes.Length - 1)
        {
            cloudNum = 0;
        }

        Shape = ps.shape;
        Texture2D cloudShape = cloudShapes[cloudNum];
        Shape.texture = cloudShape;
        curr_Shape = cloudShape;

        EventManager.TriggerEvent("UpdateMe"); //tell components to update, again, might be a more performant/efficient way to do this
        Debug.Log("cloud changing to a: " + cloudShapes[cloudNum].name);

    }


    //added for testing
    private void Update()
    {
        // if (Input.GetKeyDown("a"))
        // {
        //     ChangeCloudShape_Sequence();
        // }

    }


}
