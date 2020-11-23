﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Game_Cloud : MonoBehaviour
{


    //stuff to do:
    // --add slightly different timers to shape changing
    // --destroy myself when I blow offscreen

    public bool stopSpawning = false;
    public float spawnTime;
    public float spawnDelay;
    public ParticleSystem ps;
    public ParticleSystem.ShapeModule myShape;
    public GameObject cloudName;
    public Sprite[] cloudShapes; //generic cloudshapes
    public bool isShape; //is this cloud turned into a shape?

    public string myName; //store the name of the underlying shape here

    //Get the GameObject’s mesh renderer to access the GameObject’s material and color
    public MeshRenderer m_Renderer;
    Renderer rend;


    private int cloudNum;

    private GameObject target;

    private UnityAction someListener;
    private Transform camera;


    void Awake()
    {
        rend = GetComponent<ParticleSystemRenderer>();
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



    void Start()
    {
        ps = this.GetComponent<ParticleSystem>();

        //rotate to look at the camera
        camera = Camera.main.transform;
        transform.LookAt(camera, Vector3.back);

        cloudNum = (Random.Range(0, cloudShapes.Length));

        InvokeRepeating("ChangeCloudShape", 0, 0);


        //Fetch the mesh renderer component from the GameObject
        m_Renderer = GetComponent<MeshRenderer>();

    }
    void SpawnShape() //called from CloudManager
    {

        target = GameObject.FindWithTag("CloudManager").GetComponent<Game_CloudManager>().chosenCloud;
        Debug.Log("spawnshape  called, target is: " + target.name);


        if (target == cloudName) //if that's me
        {
            isShape = true;

            //do stuff
            var shape = GameObject.FindWithTag("CloudManager").GetComponent<Game_CloudManager>().chosenShape;

            var ns = ps.shape;
            ns.shapeType = ParticleSystemShapeType.Sprite;

            Sprite newShape = shape;
            ns.sprite = newShape;

            Debug.Log("hi, I am a shape: " + ns.sprite.name + "and my name is: " + this.name);
            CancelInvoke("ChangeCloudShape");

        } else { isShape = false; }
        //stop rotating through regular clouds, now I am a shape - but need to restart invoke after I am discovered.
    }


    void ChangeCloudShape() //this should be on some kind of timer. Should Cloudmanager call this?
    {
        if (isShape) //if this is a special shape cloud, we ignore the constant changing of underlying cloud shapes
        {
            CancelInvoke("ChangeCloudShape");
            return;
        }

        cloudNum = (Random.Range(0, cloudShapes.Length));
        var s = ps.shape;
        s.shapeType = ParticleSystemShapeType.Sprite;
        Sprite newSprite = cloudShapes[cloudNum];
        s.sprite = newSprite;
        //print("hi, " + this.gameObject.name + " is a : " + s.sprite.name);
    }

    private void OnMouseDown()
    {
        Debug.Log("clicked on: " + this.gameObject.name);

        if (isShape) {
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


    public void turnOff()
    {
        //stop the cloud from constantly shifting cloud shapes
        Debug.Log("turning off cloud");
        isShape = false;
        InvokeRepeating("ChangeCloudShape", spawnTime, spawnDelay);
    }


}