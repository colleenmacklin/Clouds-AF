using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;
using UnityEngine.Events;

/*
public class GameCloudLayerGroup_Archive : MonoBehaviour
{
    

    [Header("Cloud Objects")]
    [SerializeField]
    private Game_Cloud highClouds,lowClouds;


    [Header("Cloud Distributions")]
    [SerializeField]
    [Range(3,5000)]
    private int maxCloudsPerLayer = 1000;
    [SerializeField]
    [Range(0f,1f)]
    private float cloudDistribution = .5f;
    
    const float MAX_CLOUD_DISTANCE = 40f;
    [SerializeField]
    [Range(0f,1f)]
    private float cloudsDistance = .5f;

    [Header("High Cloud Properties")]
    [SerializeField]
    private Sprite highShape;
    [SerializeField]
    private Material highCloudMaterial; // the renderer is not found?
    [SerializeField]
    [Range(0f,20f)]
    private float highCloudDuration = 10f;
    [SerializeField]
    [Range(0,5000)]
    private int highCloudEmissionRate = 35;
    
    [Header("Low Cloud Properties")]
    [SerializeField]
    private Sprite lowShape;
    [SerializeField]
    private Material lowCloudMaterial; // the renderer is not found?
    [SerializeField]
    [Range(0f,20f)]
    private float lowCloudDuration = 10f;
    [SerializeField]
    [Range(0,5000)]
    private int lowCloudEmissionRate = 35;
    

    //brought over from Game_Cloud
    private UnityAction someListener;
    private Transform camera;
    private int cloudNum;
    private GameObject target;

    public ParticleSystem.ShapeModule myShape;
    //stuff to do:
    // --add slightly different timers to shape changing
    // --destroy myself when I blow offscreen

    public bool stopSpawning = false;
    public float spawnTime;
    public float spawnDelay;

    public GameObject cloudName;
    public Sprite[] cloudShapes; //generic cloudshapes
    public bool isShape; //is this cloud turned into a shape?
    public string myName; //store the name of the underlying shape here

   
    [Button]
    void UpdateClouds(){
        //we have to get these at run time which is ridiculous
        var hcM = highClouds.ps.main;
        var hcE = highClouds.ps.emission;
        var hcS = highClouds.ps.shape;
        
        var lcM = lowClouds.ps.main;
        var lcE = lowClouds.ps.emission;
        var lcS = lowClouds.ps.shape;

        //set high cloud options
        hcM.maxParticles = (int)((float)maxCloudsPerLayer * (cloudDistribution));
        hcM.startLifetime = highCloudDuration;
        hcE.rateOverTime = highCloudEmissionRate;
        hcS.sprite = highShape ? highShape : hcS.sprite;

        //set low cloud options
        lcM.maxParticles = (int)((float)maxCloudsPerLayer * (1f - cloudDistribution));
        lcM.startLifetime = lowCloudDuration;
        lcE.rateOverTime = lowCloudEmissionRate;
        lcS.sprite = lowShape ? lowShape : lcS.sprite;

        //reposition clouds
        //calculate distance
        float distance = MAX_CLOUD_DISTANCE * cloudsDistance;
        //position calculated as an absolute of lower cloud position
        Vector3 upperPosition = new Vector3 (lowClouds.transform.position.x, lowClouds.transform.position.y + distance, lowClouds.transform.position.z);
        highClouds.transform.position = upperPosition;

    }
    
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

            var hi_ns = highClouds.ps.shape;
            var low_ns = lowClouds.ps.shape;

            hi_ns.shapeType = ParticleSystemShapeType.Sprite;
            low_ns.shapeType = ParticleSystemShapeType.Sprite;

            Sprite newShape = shape;
            hi_ns.sprite = newShape;
            low_ns.sprite = newShape;


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

        cloudNum = (Random.Range(0, cloudShapes.Length));
        var hi_s = highClouds.ps.shape;
        var low_s = lowClouds.ps.shape;

        low_s.shapeType = ParticleSystemShapeType.Sprite;
        hi_s.shapeType = ParticleSystemShapeType.Sprite;

        Sprite newSprite = cloudShapes[cloudNum];
        hi_s.sprite = newSprite;
        low_s.sprite = newSprite;

        //print("hiCloud is a : " + hi_s.sprite.name);
        //print("lowCloud is a : " + low_s.sprite.name);

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
*/