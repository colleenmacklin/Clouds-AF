using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_CloudManager : MonoBehaviour
{
	public GameObject cloudGroup;
	public Sprite[] ShapeArray;
    public bool isShape;
    public bool timerReset;
    public GameObject chosenCloud;
    public int cloudNum;
    public Sprite chosenShape;
    public GameObject[] CloudArray;
    private GameObject[] chosenCloudArray; //keep track of the clouds that have been chosen

    public int shapeCount;// how many shapes to show before ending game?
                          //timer
    public float radius = 5f;

    // Ranges for positioning clouds when they spawn (left/right, far/near, up/down)
    private Vector3 Min;
    private Vector3 Max;
    private float _xAxis;
    private float _yAxis;
    private float _zAxis; //If you need this, use it
    private Vector3 _randomPosition;
    public bool _canInstantiate;

    private float sMin;
    private float sMax;
    private float scaleNum;
    private Vector3 scaleChange;


    //behaviors:
    //tell EventManager to "SpawnShape" (CloudArray[n], ShapeArray[n])
    //tell EventManager to remove "SpawnShape" (CloudArray[n])
    void OnEnable()
    {
        EventManager.StartListening("FoundCloud", TurnOffCloud);
    }

    void OnDisable()
    {
        EventManager.StopListening("FoundCloud", TurnOffCloud);
    }



    // Start is called before the first frame update
    void Start()
	{
        //start some kind of timer?
        //set the possible locations for clouds
        SetRanges();
        InstantiateRandomObjects();
        shapeCloud();


    }
    //possible locations for clouds /scales
    private void SetRanges()
    {
        Min = new Vector3(-30, 25, -30); //Random location value not behind trees.
        Max = new Vector3(30, 40, 30); //Another random value, not behind trees.
        sMin = 1;
        sMax = 2;
    }

    private void shapeCloud()
    {
        //choose a random cloud to turn into a shape
        int shapeNum = (Random.Range(0, ShapeArray.Length));
        chosenShape = ShapeArray[shapeNum];
        cloudNum = (Random.Range(0, CloudArray.Length));
        chosenCloud = CloudArray[cloudNum];
        Debug.Log("chosenCloud: " + chosenCloud);

        TurnOffCloud(); //tells cloud that it is not a shape anymore, changes it's shape...

        EventManager.TriggerEvent("SpawnShape"); //tell a cloud to turn into a shape

        //maybe trigger this once the cloud has become a shape?
        EventManager.TriggerEvent("Talk"); //Friend starts talking about the shape (on a timer)
    }

    private void TurnOffCloud()
    {
        EventManager.TriggerEvent("TurnOffCloud"); //message received on cloud object 
    }

    // Update is called once per frame
    void Update()
    {

        //call this on a timer, or set of co-routines
        if (Input.GetKeyDown("o"))
        {
            shapeCloud();
        }


    }
    //for prefab instantiation, see: https://docs.unity3d.com/Manual/InstantiatingPrefabs.html

    private void InstantiateRandomObjects() //need to check to make sure these clouds are not too close / intersecting
    {
        if (_canInstantiate)
        {

            //set random locations
            //Debug.Log("_xAxis: " + _yAxis);
            for (int i = 0; i < CloudArray.Length; i++)
                {
                //set up scales
                scaleNum = Random.Range(sMin, sMax);
                scaleChange = new Vector3(scaleNum, scaleNum, scaleNum);
                //set up locations
                _xAxis = Random.Range(Min.x, Max.x);
                _yAxis = Random.Range(Min.y, Max.y);
                _zAxis = Random.Range(Min.z, Max.z);
                _randomPosition = new Vector3(_xAxis, _yAxis, _zAxis);
    
                CloudArray[i] = (GameObject)Instantiate(cloudGroup, _randomPosition, Quaternion.EulerRotation(-90, 0, 0)); ; //clouds are always rotated to show their face to the person lying down (x = -90)
                CloudArray[i].name = "Cloud" + i;
                CloudArray[i].transform.localScale = scaleChange;
                Debug.Log("cloud instantiated: " + CloudArray[i].name);
            }
        }

    }

}
