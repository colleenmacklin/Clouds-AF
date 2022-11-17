using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/*

    The CloudShape contains all the controlling behavior for all the particle system
    manipulation required for our game. We can additionally define more behavior
    as needed going forward.

    //probably should create random range timers for changing a clouds shape on the cloud, and signal the CloudManager when it is ready to be changed

    The primary function is the SetTexture method which will rescale the images into

*/
// TO DO

//REMOVE ALL EVENTS FROM INDIVIDUAL CLOUDS

public class CloudShape : MonoBehaviour
{

    [Header("Control Properties")]
    [SerializeField]
    public Texture2D currentShape;
    public Texture2D incomingShape;
    public string CurrentShapeName;
    //public string CurrentShapeName { get => currentShape.name; } //not showing...
    public bool ready;
    public float timeLeft;
    [SerializeField]
    private ParticleSystem ps;
    [SerializeField]
    private ParticleSystem.ShapeModule psShape;
    private BoxCollider cloudCollider;


    //public ParticleSystem.ShapeModule myShape;
    //stuff to do:
    // --destroy myself when I blow offscreen
    [Header("Variable Timings and Scales")]
    [SerializeField]
    [Tooltip("Min and Max for Random timings behind clouds changing")] //CM added 4/19
    public float changeTimeMin;
    public float changeTimeMax;

    [Tooltip("default was 10.0f")]
    public float minScale;
    public float maxScale;
    public float currScale;

    [SerializeField]
    [Tooltip("visible sky bounds = ")]
    public int cloudVisX = -50;

    [SerializeField]
    [Tooltip("Set to Quad renderer")]
    private Renderer shapeRenderer;


    private void OnEnable()
    {
        EventManager.StartListening("StopClouds", StopCloud);
        EventManager.StartListening("ClarifyClouds", ClarifyCloud);
        EventManager.StartListening("SlowDownClouds", SlowDownCloud);
        Actions.SharpenCloud += SharpenCloud;
        Actions.BlurCloud += BlurCloud;
    }

    private void OnDisable()
    {
        EventManager.StopListening("StopClouds", StopCloud);
        EventManager.StopListening("ClarifyClouds", ClarifyCloud);
        EventManager.StopListening("SlowDownClouds", SlowDownCloud);
        Actions.SharpenCloud -= SharpenCloud;
        Actions.BlurCloud += BlurCloud;

    }

    private void Awake()
    {
        psShape = ps.shape; // do not forget to set this first! will throw null reference exception
        cloudCollider = GetComponent<BoxCollider>();
    }

    //In start we look at the camera and
    //We set the collider reference
    private void Start()
    {
        //rotate to look at the camera 
        var camera = Camera.main.transform;
        transform.LookAt(camera, Vector3.back);

    }

    // checks the number of particles to see if this cloud is visible...//can be removed
    /*
    private void LateUpdate()
    {
        numParticles = ps.particleCount;
        if (numParticles > 1000)
        {
            ready = true;
        }
        else
            ready = false;
    }
    */

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
        //var srcWidth = shapeTexture.width;
        //var srcHeight = shapeTexture.height;

        //Calculate texture adjustment factor - no longer needed
        //Vector3 textureScaleAdjustment = CalculateSquareScaleRatio(srcWidth, srcHeight);

        //Set the object's shape reference to the shapeTexture for easy reference

        //save shapeTexture to incomingShape
        incomingShape = shapeTexture;


        //adjust the shape of the cloud and its collider based on the aspect ratio of the shape
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
        psMain.startSizeMultiplier = currScale/2;

        //add a check everytime this function is called to see if the cloud is out of visibility for the player
        //if it is, move the cloud to the far right of the screen, in its same y position (like a scroll)
        Vector3 myPosition = this.transform.position;
        if (myPosition.x < -cloudVisX)
        {
            //TODO: Make this a call to cloud manager to remomve and reinstantiate (with a nice fade effect) fadeout and move...
            //TODO: location can be tracked by thie cloud object, but fading and moving the cloud to the righ should probably be called from the cloudManager so that it can use the poisson function to re-instantiate and then avoid overlapping clouds
           
            /*
            fadeOutParticleSystem();
            transform.position = new Vector3(myPosition.x*-1, myPosition.y, myPosition.z);
            fadeInParticleSystem();
            */

            StartCloud();
        }

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
        StartCoroutine(TimeToChange());

        //add an action to trigger on Cloudmanager, with a reference to this cloud when the timer is done
    }

    IEnumerator TimeToChange()
    {
        //Start a variable timer countdown to signal when the cloud is ready to change
        //enable some variable timings for clouds to start changing shape
        float timing = Random.Range(changeTimeMin, changeTimeMax);
        for (timeLeft = timing; timeLeft > 0; timeLeft -= Time.deltaTime)
        yield return null;
        Actions.CloudIsReady(this);

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
        //adding a randomizer here for variable sizes
        float scale = UnityEngine.Random.Range(minScale, maxScale);
        currScale = scale; //just surfacing to the interface for debugging

        var ratio = Mathf.Max(scale / srcWidth, scale / srcHeight);

        var newsize = new Vector3(srcWidth * ratio, srcHeight * ratio, 1f);

        return newsize;
    }

    ////////////////
    //
    //    Mutation functions
    //       consider adding some state transitions in the future
    /////////////////
    public void ClarifyCloud()
    {
        var particleSystemSettings = ps.main;
        particleSystemSettings.simulationSpeed = 0.30f;
        //particleSystemSettings.startSize = new ParticleSystem.MinMaxCurve(1.5f, 3f);
    }

    public void SharpenCloud()
    {
        var particleSystemSettings = ps.main;
        particleSystemSettings.simulationSpeed = 0.30f;
        particleSystemSettings.startSize = new ParticleSystem.MinMaxCurve(1.5f, 3f);
    }
    public void BlurCloud()
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

    public void fadeInParticleSystem()
    {
        FadeObjectInOut refScript = GetComponent<FadeObjectInOut>();
        refScript.FadeIn(10);
        /* ultimately, change the simple fadeinout script to a script that "dissolves" the clouds by removing particles over time
      
        var particleSystemSettings = ps.main;
        particleSystemSettings.maxParticles -= particleSystemSettings.maxParticles * (int)Time.deltaTime;
        Debug.Log("particles: " + particleSystemSettings.maxParticles);
        */
    }

    public void fadeOutParticleSystem()
    {
        FadeObjectInOut refScript = GetComponent<FadeObjectInOut>();

        refScript.FadeOut(10);

        /* ultimately, change the simple fadeinout script to a script that "dissolves" the clouds by removing particles over time
      
        var particleSystemSettings = ps.main;
        particleSystemSettings.maxParticles -= particleSystemSettings.maxParticles * (int)Time.deltaTime;
        Debug.Log("particles: " + particleSystemSettings.maxParticles);
        */
    }



}
