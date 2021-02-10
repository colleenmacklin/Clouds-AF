using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour
{
    [Header("Cloud Properties")]
    [SerializeField]
    private int numberOfClouds;
    [SerializeField]
    private List<GameObject> ActiveClouds;
    [SerializeField]
    private GameObject cloudGroup;
    [SerializeField]
    private Texture2D[] ShapeArray; //textures stay as an array because we are not generating run time textures

    [Header("Spawning Attributes")]

    [SerializeField]
    [Range(1, 1000)]
    private int instantiationAttempts = 50; //how many instantiation attempts we should make before failing
    // Ranges for positioning clouds when they spawn (left/right, far/near, up/down)
    [SerializeField]
    private Vector2 xRange, yRange, zRange;
    [SerializeField]
    private Vector2 scaleRange;

    [Header("Debug Cloud Selections")]
    public GameObject chosenCloud;
    public Texture2D chosenShape;
    public GameObject shapeCollider; //our testing box that we resuse, will be a GameObject with only a collider on it
    public Bounds shapeBounds;


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
        EventManager.StartListening("FoundCloud", TurnOffCloud);
        EventManager.StartListening("ConversationEnded", SetCloudToShape);
    }

    void OnDisable()
    {
        EventManager.StopListening("FoundCloud", TurnOffCloud);
        EventManager.StopListening("ConversationEnded", SetCloudToShape);
    }

    void Start()
    {
        CreateActiveClouds();
        SetCloudToShape();
    }


    // Update is called once per frame
    void Update()
    {

        //call this on a timer, or set of co-routines
        if (Input.GetKeyDown("o"))
        {
            SetCloudToShape();
        }


    }

    ///////////////////////
    //
    // Cloud Functions
    //
    /////////////////////////


    void CreateActiveClouds()
    {
        ActiveClouds = new List<GameObject>();
        for (int i = 0; i < numberOfClouds; i++)
        {
            var go = SpawnRandomNonIntersectingCloud(ActiveClouds);
            go.name = $"cloud {i}";
            ActiveClouds.Add(go);
        }
    }

    private void SetCloudToShape()
    {
        //choose a random cloud to turn into a shape
        //This whole logic might need to be changed
        int shapeNum = (Random.Range(0, ShapeArray.Length));
        int cloudNum = (Random.Range(0, ActiveClouds.Count));
        chosenShape = ShapeArray[shapeNum];
        chosenCloud = ActiveClouds[cloudNum];
        Debug.Log("chosenCloud: " + chosenCloud);

        //tells cloud that it is not a shape anymore, changes it's shape...
        TurnOffCloud();

        //This needs to be adjusted from the SpawnShape
        //Right now the chosenCloud is compared against from the clouds?
        //We should change this paradigm so that the shape is directly set
        //Because the manager, if it exists at all, should manage. Not respond.
        //i.e. clouds should not be responsible for knowing they are chosen

        EventManager.TriggerEvent("SpawnShape"); //tell a cloud to turn into a shape

        //This is where the dialogue manager is activated.
        //maybe trigger this once the cloud has become a shape?
        EventManager.TriggerEvent("Talk"); //Friend starts talking about the shape (on a timer)
    }

    private void TurnOffCloud()
    {
        EventManager.TriggerEvent("TurnOffCloud"); //message received on cloud object 
    }
    //for prefab instantiation, see: https://docs.unity3d.com/Manual/InstantiatingPrefabs.html

    private GameObject SpawnRandomNonIntersectingCloud(List<GameObject> currentClouds) //need to check to make sure these clouds are not too close / intersecting
    {
        //check for collisions first

        Vector3 _randomPosition = new Vector3();
        Vector3 scaleChange = new Vector3();
        //set random locations
        for (int n = 0; n < instantiationAttempts; n++)
        {

            Debug.Log("Instantiation Attempts: " + n);

            //set up random location
            float _xAxis = Random.Range(xRange.x, xRange.y);
            float _yAxis = Random.Range(yRange.x, yRange.y);
            float _zAxis = Random.Range(zRange.x, zRange.y);
            _randomPosition = new Vector3(_xAxis, _yAxis, _zAxis);

            float scaleNum = Random.Range(scaleRange.x, scaleRange.y);
            scaleChange = new Vector3(scaleNum, scaleNum, scaleNum);

            //first check for possible collisions
            //Step 1: random position
            shapeCollider.transform.localScale = scaleChange;
            shapeCollider.transform.position = _randomPosition; //move collider to position
            shapeBounds = shapeCollider.GetComponent<Collider>().bounds;//get the collider

            //Step 2: compare position against active positions
            var valid = true; //how we will know if we have a valid shape
                              //compare our testing box to every other active cloud shape's
            foreach (var cloud in currentClouds)
            {
                //if intersection found, then not valid
                if (shapeBounds.Intersects(cloud.GetComponent<Collider>().bounds))
                {
                    Debug.Log("INTERSECTION");
                    valid = false;
                    break;
                }
            }
            if (valid) break; //if it is valid, then we leave 

        }

        //step 3: create cloud
        GameObject go = (GameObject)Instantiate(cloudGroup, _randomPosition, Quaternion.Euler(-90, 0, 0), transform);
        go.transform.localScale = scaleChange;

        return go;


    }

}
