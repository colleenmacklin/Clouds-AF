﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Events;
using static UnityEngine.ParticleSystem;
/*

    This is a version of The CloudShape specific to the opening. It contains all the controlling behavior for all the particle system
    manipulation required for our game. We can additionally define more behavior
    as needed going forward.

    The primary function is the SetTexture method which will rescale the images into

*/

public class OpeningCloudShape : MonoBehaviour
{

    [Header("Control Properties")]
    [SerializeField]
    public Texture2D currentShape;
    public Texture2D incomingShape;
    public string CurrentShapeName;
    public bool ready;
    public bool isTarget; //cloudManager checks this to see if the cloud is a target shape, i.e. a shape of something, not just a generic cloud
    public float timeLeft;
    [SerializeField]
    private ParticleSystem ps;
    [SerializeField]
    private ParticleSystem.ShapeModule psShape;
    private BoxCollider cloudCollider;


    //public ParticleSystem.ShapeModule myShape;

    [Header("Variable Timings and Scales")]
    [SerializeField]
    [Tooltip("Min and Max for Random timings behind clouds changing")] //CM added 4/19
    public float changeTimeMin;
    public float changeTimeMax;

    [Tooltip("default was 10.0f")]
    public float minScale;
    public float maxScale;
    public float scale;
    public float currScale;

    [SerializeField]
    [Tooltip("visible sky bounds = ")]
    public int cloudVisX = -70; // TODO: Adjust

    [SerializeField]
    [Tooltip("Set to Quad renderer")]
    private Renderer shapeRenderer;

    public bool isGameLoop = true;
    private FadeObjectInOut _fadeObject;

    //bool to be set to true when cloud is nearing edge of screen and moved to the other side
    private bool _cloudIsBusyResetting = false;

    //to be set by the fadeobject in/out object (not very elegant)
    public bool IsFading;

    Transform _camTransform;

    private void OnEnable()
    {
        EventManager.StartListening("StopClouds", StopCloud);
        EventManager.StartListening("ClarifyClouds", ClarifyCloud);
        EventManager.StartListening("SlowDownClouds", SlowDownCloud);
        Actions.SharpenCloud += SharpenCloud;
        Actions.BlurCloud += BlurCloud;

        _fadeObject.ResetCloudPos += ResetCloudPos;
    }

    private void OnDisable()
    {
        EventManager.StopListening("StopClouds", StopCloud);
        EventManager.StopListening("ClarifyClouds", ClarifyCloud);
        EventManager.StopListening("SlowDownClouds", SlowDownCloud);
        Actions.SharpenCloud -= SharpenCloud;
        Actions.BlurCloud += BlurCloud;

        _fadeObject.ResetCloudPos -= ResetCloudPos; 
    }

    //Transform _camTransform;

    private void Awake()
    {
        psShape = ps.shape; // do not forget to set this first! will throw null reference exception
        cloudCollider = GetComponent<BoxCollider>();
        _fadeObject = GetComponent<FadeObjectInOut>();
        StopCloud();
        //_fadeObject.OnHasFadedInOut += 
    }

    //In start we look at the camera and
    //We set the collider reference
    private void Start()
    {
        //rotate to look at the camera 
       _camTransform = Camera.main.transform;
     //   transform.LookAt(camera, Vector3.back);

    

        //create some randomness in length delay of fading in and duration of fade

        // _fadeObject.fadeDelay = Random.Range(3, 10);
        // _fadeObject.fadeTime = Random.Range(6, 12);

    }


    private void Update()
    {

        transform.LookAt(_camTransform, Vector3.back);

        //--------------------------TESTING NEW FADE EFFECT
            if (Input.GetKeyDown(KeyCode.C))
            {
            StartCloud();
            }
            //---------------
    }


    //checks to see if cloud is close to being offscreeen, and if it is, starts fade out and reposition sequence
    //bool to prevent it from calling once sequence has started
    private void CheckCloudVis()
    {
        if (transform.position.x <= cloudVisX)
        {
            _cloudIsBusyResetting = true;
            _fadeObject.StopAllCoroutines();
            StartCoroutine(_fadeObject.FadeCloudInOut());
        }
    }

    //moves cloud to other side of screen 
    //sets flag to false so it starts checking for being offscreen again
    private void ResetCloudPos()
    {
        Vector3 cloudPos = transform.position;
        //todo: should probs think abt wind changing exponentially 
        //like stronger when on sides of screen etc
        //also to stop while u r watching cloud
        cloudPos.x = 210;
        transform.position = cloudPos;
        _cloudIsBusyResetting = false;
    }


    public void TurnOnCollider()
    {
        //Debug.Log("turning on Collider..............");
        cloudCollider.enabled = true;
    }

    public void TurnOffCollider()
    {
        //Debug.Log("...........turning off Collider..............");

        cloudCollider.enabled = false;
    }

    //cm added coroutine to this 4/15

    //SetShape takes a texture (and sets it after a rescale)
    //this also sets the collider size to update with it
    public void SetShape(Texture2D shapeTexture)
    {

        //save shapeTexture to incomingShape
        //Debug.Log("My Shape is: " + shapeTexture);
        incomingShape = shapeTexture;


        //adjust the shape of the cloud and its collider based on the aspect ratio of the shape
        //TODO: we might need to move this into cloudManager for greater control, and also for different contexts like the Opening
        //for now, I have a check to see if we're in the Gameloop, and if not, the scale is set elsewhere.
        var srcWidth = incomingShape.width;
        var srcHeight = incomingShape.height;
        Vector3 textureScaleAdjustment = CalculateSquareScaleRatio(srcWidth, srcHeight);


        //Set the object's shape reference to the shapeTexture for easy reference
        currentShape = incomingShape;

        //Set the scale and texture value in the particle system shape module
        psShape.scale = textureScaleAdjustment;
        psShape.texture = incomingShape;

        //modify the StartSize of the particle system so that it scales with the size of the cloud (ensures there's no gaps between parrticles)
        //StartSize is a range, and the base setting is 3 to 7
        var psMain = ps.main;
        //psMain.startSizeMultiplier = currScale/2;
        psMain.startSizeMultiplier = scale / 2;


        //Set the scale *of the collider* that represents the shape
        //Collider is rotated, so the values are x and y.
        //And the 7f arbbitrarily for "best fit"
        Vector3 colliderSize = new Vector3(
            5f * textureScaleAdjustment.x / 7f,
            5f * textureScaleAdjustment.y / 7f,
            2f
        );
        cloudCollider.size = colliderSize;

        CurrentShapeName = currentShape.name;

        //commenting out for opening cloud
        //StartCoroutine(TimeToChange());

    }

    IEnumerator TimeToChange()
    {
        //Start a variable timer countdown to signal when the cloud is ready to change
        //enable some variable timings for clouds to start changing shape
        float timing = Random.Range(changeTimeMin, changeTimeMax);
        for (timeLeft = timing; timeLeft > 0; timeLeft -= Time.deltaTime)
        yield return null;
        Actions.OpeningCloudIsReady(this);

        //yield return new WaitForSeconds(timing);
    }

    ////////////////
    //
    //    Utility Function
    //
    ///////////////////


    //Calculates a SQUARE aspect ratio ***SCALE*** for the image texture in particle system
    //This makes it look even if the base image is something like 1024x512
    //It does require that the long edge of an image be 1024. Otherwise we might run into problems
    //Does not actually change the texture, we should probably move to 1024x1024 in the long run.
    //The 10f and 10f are our chosen  minimums.
    //private Vector3 CalculateSquareScaleRatio(float srcWidth, float srcHeight, float minWidth = 10f, float minHeight = 10f)
    private Vector3 CalculateSquareScaleRatio(float srcWidth, float srcHeight)

    {
        //TODO: we might need to move this into cloudManager for greater control, and also for different contexts like the Opening
        //for now, I have a check to see if we're in the Gameloop, and if not, the scale is set by the manager (such as in the opening.)
        if (isGameLoop)
        {
            //adding a randomizer here for variable sizes
            //Debug.Log("gameState = GameLoop");
            scale = UnityEngine.Random.Range(minScale, maxScale);
            currScale = scale; //just surfacing to the interface for debugging
        }

        if (!isGameLoop)
        {
            currScale = scale; //just surfacing to the interface for debugging
        }
        var ratio = Mathf.Max(scale / srcWidth, scale / srcHeight);

        var newsize = new Vector3(srcWidth * ratio, srcHeight * ratio, 1f);

        return newsize;
    }

    ////////////////
    //
    //    Mutation functions
    //       consider adding some state transitions in the future
    /////////////////
    public void ClarifyCloud() //speeds up the cloud so it takes shape faster
    {
        var particleSystemSettings = ps.main;
        particleSystemSettings.simulationSpeed = 0.30f;
        //particleSystemSettings.startSize = new ParticleSystem.MinMaxCurve(1.5f, 3f);
    }

    public void SharpenCloud() //makes the particles smaller, sharper around the underlying shape
    {
        var particleSystemSettings = ps.main;
        //particleSystemSettings.simulationSpeed = 0.30f;
        particleSystemSettings.startSize = new ParticleSystem.MinMaxCurve(1.5f, 3f);
    }

    public void SharpenOpeningCloud(float min, float max) //TODO: this is a hack for the opening...change all cloud functions to allow passed in values
    {
        var particleSystemSettings = ps.main;
        particleSystemSettings.simulationSpeed = 0.30f;
        //particleSystemSettings.startSize = new ParticleSystem.MinMaxCurve(3f, 7f);
        particleSystemSettings.startSize = new ParticleSystem.MinMaxCurve(min, max);

    }

    public void BlurCloud() //makes the particles more diffuse around the underlying shape
    {
        var particleSystemSettings = ps.main;
        particleSystemSettings.simulationSpeed = 0.30f;
        particleSystemSettings.startSize = new ParticleSystem.MinMaxCurve(3f, 10f);
    }

    public void StopCloud()
    {
        var particleSystem = ps;
        particleSystem.Stop();
    }

    public void StartCloud()
    {
        var particleSystem = ps;
        particleSystem.Play();
        //Actions.CloudIsReady(this);
    }

    public void SlowDownCloud()
    {
        var particleSystemSettings = ps.main;
        particleSystemSettings.simulationSpeed = .08f;
    }

    public void FadeInPS(int _delay)
    {
        StartCoroutine("fadeInParticleSystem", _delay);
    }

    public void FadeOutPS(int _delay)
    {
        StartCoroutine("fadeOutParticleSystem", _delay);
    }


    IEnumerator fadeInParticleSystem(int _delay)
    {
        yield return new WaitForSeconds(_delay);
        var particleSystem = ps;
        particleSystem.Play();


        //_fadeObject.FadeIn(_fadeObject.fadeTime);
        /* ultimately, change the simple fadeinout script to a script that "dissolves" the clouds by removing particles over time
      
        var particleSystemSettings = ps.main;
        particleSystemSettings.maxParticles -= particleSystemSettings.maxParticles * (int)Time.deltaTime;
        Debug.Log("particles: " + particleSystemSettings.maxParticles);
        */
    }

    IEnumerator fadeOutParticleSystem(int _delay)
    {
        yield return new WaitForSeconds(_delay);
        var particleSystem = ps;
        particleSystem.Stop();

       // _fadeObject.FadeOut(_fadeObject.fadeTime);

        /* ultimately, change the simple fadeinout script to a script that "dissolves" the clouds by removing particles over time
      
        var particleSystemSettings = ps.main;
        particleSystemSettings.maxParticles -= particleSystemSettings.maxParticles * (int)Time.deltaTime;
        Debug.Log("particles: " + particleSystemSettings.maxParticles);
        */
    }



}