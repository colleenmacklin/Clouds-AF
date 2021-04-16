using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Game_CloudManager : MonoBehaviour
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
    private Vector2 scaleRange;

    [SerializeField]
    public float radius = 1;

    [SerializeField]
    public Vector2 regionSize = Vector2.one;

    [SerializeField]
    public int rejectionSamples = 30;

    [SerializeField]
    public float displayRadius = 1;

    List<Vector2> points;


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
        //this doesn't really need to be it's own function, but leaving it for now in case we want to call anything else here.
        SpawnClouds();
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


    //using poisson disc method for cloud distribution


    void OnValidate()
    {
        points = PoissonDiscSampling.GeneratePoints(radius, regionSize, rejectionSamples);
    }
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
    //shifts vector3
    Vector3 ScaleAndShiftVector(Vector3 v, Vector3 shift, Vector3 scale)
    {
        return Vector3.Scale(v, scale) + shift;
    }

    private void SpawnClouds()
    {
        var _y = 60; //based on how high clouds should spawn
        Vector3 gv3 = new Vector3(regionSize.x, 0, regionSize.y); //scale
        Vector3 gv3half = new Vector3(gv3.x / 2, _y, gv3.z / 2); //center point
        Vector3 region_shift = new Vector3(gv3.x - regionSize.x, _y, gv3.z - regionSize.y);
        Vector3 region_scale = new Vector3(0, 0, 0);
        Vector3 newRegion = ScaleAndShiftVector(gv3half, region_shift, region_scale); //would want to use a region_shift relative to the player

        Vector3 v3point_shift = new Vector3();
        Vector3 scaleChange = new Vector3();
        ActiveClouds = new List<GameObject>();
        GameObject go;
        int i = 0;

        if (points != null)
        {
            foreach (Vector2 point in points)
            {
                //convert vector2D to vector3d
                Vector3 v3point = new Vector3(point.x, _y, point.y); //turn vector2 point into vector3
                Vector3 _shift = new Vector3(v3point.x - gv3half.x, v3point.y, v3point.z - gv3half.z); //would want to use a region_shift relative to the player
                Vector3 _scale = new Vector3(0, 0, 0);
                v3point_shift = ScaleAndShiftVector(v3point, _shift, _scale);

                float scaleNum = Random.Range(scaleRange.x, scaleRange.y);
                scaleChange = new Vector3(scaleNum, scaleNum, scaleNum);
                Debug.Log("points: " + v3point_shift);
                //var go = SpawnClouds(ActiveClouds);
                i++;
                go = (GameObject)Instantiate(cloudGroup, v3point_shift, Quaternion.Euler(0, 0, 0), transform);
                go.transform.localScale = scaleChange;

                go.name = $"cloud {i}";
                ActiveClouds.Add(go);

            }

        }


    }

}