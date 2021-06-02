using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/*

    The CloudShape contains all the controlling behavior for all the particle system
    manipulation required for our game. We can additionally define more behavior
    as needed going forward.

    The primary function is the SetTexture method which will rescale the images into

*/
public class CloudShape : MonoBehaviour
{

    [Header("Control Properties")]
    [SerializeField]
    private Texture2D currentShape;
    public string CurrentShapeName { get => currentShape.name; }
    [SerializeField]
    private ParticleSystem ps;
    [SerializeField]
    private ParticleSystem.ShapeModule psShape;
    private BoxCollider cloudCollider;


    //public ParticleSystem.ShapeModule myShape;
    //stuff to do:
    // --add slightly different timers to shape changing
    // --destroy myself when I blow offscreen
    [Header("Under Construction")]
    [Tooltip("Not in use")]
    public float spawnTime;
    [Tooltip("Not in use")]
    public float spawnDelay;
    [SerializeField]
    [Tooltip("Set to Quad renderer")]
    private Renderer shapeRenderer;

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

    //SetShape takes a texture (and sets it after a rescale)
    //this also sets the collider size to update with it
    public void SetShape(Texture2D shapeTexture)
    {
        var srcWidth = shapeTexture.width;
        var srcHeight = shapeTexture.height;

        //Calculate texture adjustment factor
        Vector3 textureScaleAdjustment = CalculateSquareScaleRatio(srcWidth, srcHeight);

        //Set the object's shape reference to the shapeTexture for easy reference
        currentShape = shapeTexture;

        //Set the scale and texture value in the shape module directly
        psShape.scale = textureScaleAdjustment;
        psShape.texture = shapeTexture;

        //Set the scale *of the collider* that represents the shape
        Vector3 colliderSize = new Vector3(
            5f * textureScaleAdjustment.x / 7.5f,
            5f * textureScaleAdjustment.y / 7.5f,
            2f
        );
        cloudCollider.size = colliderSize;
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
    private Vector3 CalculateSquareScaleRatio(float srcWidth, float srcHeight, float minWidth = 10f, float minHeight = 10f)
    {
        var ratio = Mathf.Max(minWidth / srcWidth, minHeight / srcHeight);

        var newsize = new Vector3(srcWidth * ratio, srcHeight * ratio, 1f);

        return newsize;
    }

    ////////////////
    //
    //    Mutation functions
    //       consider adding some state transitions in the future
    /////////////////
    public void ClarifyClouds()
    {
        var particleSystemSettings = ps.main;
        particleSystemSettings.simulationSpeed = 0.30f;
        particleSystemSettings.startSize = new ParticleSystem.MinMaxCurve(1.5f, 3f);
    }

    public void StopClouds()
    {
        var particleSystem = ps;
        particleSystem.Stop();
    }

    public void SlowDownClouds()
    {
        var particleSystemSettings = ps.main;
        particleSystemSettings.simulationSpeed = .08f;
    }

}
