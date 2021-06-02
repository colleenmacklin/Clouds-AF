using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using PoissonDisc;

/*

    Cloud Manager is responsible for creating the clouds into an evenly (poisson-distributed) grid
    It is also the source of the event handling, as it can route the event to the clouds. 
    The Manager, then, acts as the cloud conductor as it controls how clouds should be acting

    It handles the following events
        *   ClarifyCloud
        *   Dissipate
        *   SlowDown
        *   TurnOffCloud
        *   GenerateClouds

*/

public class Game_CloudManager : MonoBehaviour
{
    [Range(0f, 40f)]
    public float pauseBetweenText = 5f;

    [Header("Cloud Properties")]
    [SerializeField]
    [Tooltip("How many clouds to create")]

    private int numberOfCloudsToGenerate;
    [SerializeField]
    [Tooltip("How many of the clouds to turn into targets")]
    private int numberOfTargetsToGenerate;
    [SerializeField]
    [Tooltip("All the generated clouds in the sky")]
    private List<GameObject> generatedCloudObjects;
    [SerializeField]
    [Tooltip("The base cloud prefab")]
    private GameObject cloudObjectPrefab;


    [Header("Cloud Data")]//consider refactor as cloud scriptable objects
    [SerializeField]
    private Texture2D[] cloudTargetsArray; //textures stay as an array because we are not generating run time textures
    [SerializeField]
    private Texture2D[] cloudGenericsArray; //textures stay as an array because we are not generating run time textures
    [SerializeField]
    private List<string> cloudSelectedHistory;
    List<Vector2> points;//I don't think we need this


    [Header("Debug Cloud Selections")]
    public GameObject chosenCloud;
    public Texture2D chosenShape;
    public GameObject shapeCollider; //our testing box that we resuse, will be a GameObject with only a collider on it
    public Bounds shapeBounds;
    List<string> selectionHistory = new List<string>();
    public int cloudSelectionIndex = -1;

    [Header("Poisson Settings")]
    [SerializeField]
    [Tooltip("The scale range for the cloud objects, x is min, y is max")]
    private Vector2 scaleRange = new Vector2(2f, 2.75f);

    [SerializeField]
    [Tooltip("The collision distance for the poisson discs. Effectively setting grid density")]
    public float poissonRadius = 50;//default is 50

    [SerializeField]
    [Tooltip("The size of the region to create points in")] //This region is from 0,0 and must be translated 
    public Vector2 poissonRegionSize = new Vector2(150f, 120f);//default we will use 150x120

    [SerializeField]

    [Tooltip("How far to translate offset the region")]
    public Vector3 regionTranslation = new Vector3(-30f, 60f, -40f);//default is -30,-40,0

    [SerializeField]
    [Tooltip("Number of rejection samples before giving up on a sample. Default is 30 ")]
    public int poissonRejectionSamples = 30;//this can comfortably be a higher number

    ///////////////////////
    //
    // Monobehaviors
    //
    /////////////////////////

    //behaviors:
    //tell EventManager to "SpawnShape" (CloudArray[n], ShapeArray[n])
    //tell EventManager to remove "SpawnShape" (CloudArray[n])
    void OnEnable()
    {
        //  EventManager.StartListening("FoundCloud", TurnOffCloud);
        // EventManager.StartListening("ConversationEnded", SetCloudToShape);

        // EventManager.StartListening("UpdateMe",ClarifyCloud);
        // EventManager.StartListening("UpdateMe",Dissipate);
        // EventManager.StartListening("UpdateMe",SlowDown);
    }

    void OnDisable()
    {
        //EventManager.StopListening("FoundCloud", TurnOffCloud);
        // EventManager.StopListening("ConversationEnded", SetCloudToShape);
    }
    void Awake()
    {
        // points = PoissonDiscSampling.GeneratePoints(poissonRadius, poissonRegionSize, poissonRejectionSamples);
        // cloudSelectionIndex = -1;
        // cloudTargetsArray = ShuffleTexture2DArray(cloudTargetsArray);
        // CreateActiveClouds();
    }

    void Start()
    {
        GenerateClouds();
        EventManager.TriggerEvent("ConversationEnded");
        //Start Act Intro, not the cloud shape
        //        EventManager.TriggerEvent("Introduction");
        //       EventManager.TriggerEvent("SpawnShape");
    }

    //Generic Shuffler
    Texture2D[] ShuffleTexture2DArray(Texture2D[] arr)
    {
        Texture2D[] shuffledResult = arr;
        int n = arr.Length;
        while (n > 1)
        {
            n--;//simplifies the shuffledResult[n] down below

            int i = Random.Range(0, n + 1);
            var temp = shuffledResult[i];

            shuffledResult[i] = shuffledResult[n];
            shuffledResult[n] = temp;
        }
        return shuffledResult;

    }

    List<T> ShuffleList<T>(List<T> list)
    {
        List<T> shuffledResult = list;
        int n = list.Count;
        while (n > 1)
        {
            n--;//simplifies the shuffledResult[n] down below

            int i = Random.Range(0, n + 1);
            var temp = shuffledResult[i];

            shuffledResult[i] = shuffledResult[n];
            shuffledResult[n] = temp;
        }
        return shuffledResult;

    }

    ///////////////////////
    //
    // Cloud Functions
    //
    /////////////////////////

    //Generate Clouds
    //1. creates the points grid where it will put the clouds in
    //2. instantiates the clouds in those positions, based on an offset
    //3. use the Cloud's SetShape function to set targets and generics
    //4. Fires a Clouds Generated event
    private void GenerateClouds()
    {
        //generate points first using settings
        List<Vector2> poissonPositions = PoissonDiscSampling.GeneratePoints(poissonRadius, poissonRegionSize, poissonRejectionSamples);
        //poissonPositions are in clockwise order, we shuffle them
        poissonPositions = ShuffleList(poissonPositions);
        GameObject go;//temp gameObject we use

        //Points are all random, so we can just use as many of them as we need
        for (int i = 0; i < numberOfCloudsToGenerate; i++)
        {
            if (i >= poissonPositions.Count)
            {
                Debug.Log($"Out of possible positions, found {i - 1} positions out of {numberOfCloudsToGenerate}");
                break;
            }

            Vector3 candidatePosition = new Vector3(poissonPositions[i].x, 0f, poissonPositions[i].y);
            //convert Poisson Position into Cloud space by translation
            Vector3 cloudPosition = candidatePosition + regionTranslation;

            //instantiate the prefab
            go = Instantiate(cloudObjectPrefab, cloudPosition, Quaternion.Euler(0f, 0f, 0f), transform);

            float sizeScale = Random.Range(scaleRange.x, scaleRange.y);
            Vector3 scaleVector = new Vector3(sizeScale, sizeScale, sizeScale);

            //get a random scale for the transform
            go.transform.localScale = scaleVector;
            go.name = $"Cloud {i}";

            generatedCloudObjects.Add(go);
        }

    }

    void CreateActiveClouds()
    {
        //this doesn't really need to be it's own function, but leaving it for now in case we want to call anything else here.
        SpawnClouds();
    }

    //for the prototype we add this behavior of game logic here, it needs to be separate
    private void SetCloudToShape()
    {

        cloudSelectionIndex++;


        if (cloudSelectionIndex > cloudTargetsArray.Length)
        {
            //we've already seen the ending
            EventManager.TriggerEvent("AllShapesSeen");
            return;
        }

        if (cloudSelectionIndex == cloudTargetsArray.Length)
        {
            EventManager.TriggerEvent("Conclusion");
            //  Debug.Log("This is over");// the problem with this is that it controls dialogue logic in the cloud. This is confusing to maintain

            //don't set a shape anymore
            return;
        }

        Debug.Log(generatedCloudObjects.Count);

        //choose a random cloud to turn into a shape
        //This whole logic might need to be changed
        int shapeNum = cloudSelectionIndex;//(Random.Range(0, ShapeArray.Length));
        int cloudNum = (Random.Range(0, generatedCloudObjects.Count));
        chosenShape = cloudTargetsArray[shapeNum];
        chosenCloud = generatedCloudObjects[cloudNum];
        selectionHistory.Add(chosenShape.name);

        Debug.Log("chosenCloud: " + chosenCloud);

        //tells cloud that it is not a shape anymore, changes it's shape...
        TurnOffCloud();

        //This needs to be adjusted from the SpawnShape
        //Right now the chosenCloud is compared against from the clouds?
        //We should change this paradigm so that the shape is directly set
        //Because the manager, if it exists at all, should manage. Not respond.
        //i.e. clouds should not be responsible for knowing they are chosen

        EventManager.TriggerEvent("SpawnShape"); //tell a cloud to turn into a shape

        StartCoroutine(PauseBeforeTalking());
        //This is where the dialogue manager is activated.
        //maybe trigger this once the cloud has become a shape?
        //EventManager.TriggerEvent("Talk"); //Friend starts talking about the shape (on a timer)
    }

    private IEnumerator PauseBeforeTalking()
    {
        yield return new WaitForSeconds(pauseBetweenText);

        EventManager.TriggerEvent("Talk");
    }

    private void TurnOffCloud()
    {
        EventManager.TriggerEvent("TurnOffCloud"); //message received on cloud object 
    }
    //for prefab instantiation, see: https://docs.unity3d.com/Manual/InstantiatingPrefabs.html


    //Handy Gizmo draw calls for debugging cloud placement.
    /*
    void OnDrawGizmos()
    {
        var _y = 60; //based on how high clouds should spawn
        Vector3 gv3 = new Vector3(regionSize.x, 0, regionSize.y); //scale
        Vector3 gv3half = new Vector3(gv3.x / 2, _y, gv3.z / 2); //center point
        Vector3 region_shift = new Vector3(gv3.x-regionSize.x, _y, gv3.z-regionSize.y);
        Vector3 region_scale = new Vector3(0,0,0);
        Vector3 newRegion = ScaleAndShiftVector(gv3half, region_shift, region_scale); //would want to use a region_shift relative to the player
        //gv2 = (regionSize / 2);

        //Gizmos.DrawWireCube(regionSize / 2, regionSize);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(newRegion, gv3);


        if (points != null)
        {
            foreach (Vector2 point in points)
            {
                //convert vector2D to vector3d
                Vector3 v3point = new Vector3(point.x, _y, point.y); //turn vector2 point into vector3
                Vector3 _shift = new Vector3(v3point.x - gv3half.x, v3point.y, v3point.z - gv3half.z); //would want to use a region_shift relative to the player
                Vector3 _scale = new Vector3(0,0,0);
                Vector3 v3point_shift = ScaleAndShiftVector(v3point, _shift, _scale);
                Gizmos.DrawSphere(v3point_shift, displayRadius);

                //Gizmos.DrawSphere(point, displayRadius);
                Debug.Log("GIZMO location: " + v3point_shift);

            }
        }
    }
    */

}