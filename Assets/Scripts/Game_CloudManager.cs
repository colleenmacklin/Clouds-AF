using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Game_CloudManager : MonoBehaviour
{
	public GameObject cloudGroup;
	public Texture2D[] ShapeArray;
    public bool isShape;
    public bool timerReset;
    public GameObject chosenCloud;
    public int cloudNum;
    public Texture2D chosenShape;
    public GameObject[] CloudArray;
    private GameObject[] chosenCloudArray; //keep track of the clouds that have been chosen
    public GameObject shapeCollider; //our testing box that we resuse, will be a GameObject with only a collider on it
    public Bounds shapeBounds;
    public int instantiationAttempts = 50; //how many instantiation attempts we should make before failing

    public int shapeCount;// how many shapes to show before ending game?
                          //timer

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
        for (int i = 0; i < CloudArray.Length; i++)
        {
            CloudArray[i] = InstantiateRandomObjects(); // should return a gameObject
            CloudArray[i].name = "cloud" + i;
        }
        shapeCloud();


    }
    //possible locations for clouds /scales
    private void SetRanges()
    {
        Min = new Vector3(-30, 25, -40); //Random location value not behind trees.
        Max = new Vector3(40, 35, 70); //Another random value, not behind trees. - Y used to be 40
        sMin = 1;
        sMax = 2.0f; //changed from 2
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

    private GameObject InstantiateRandomObjects() //need to check to make sure these clouds are not too close / intersecting
    {
            //check for collisions first


            //set random locations
            //Debug.Log("_xAxis: " + _yAxis);
            for (int n = 0; n < instantiationAttempts; n++)
            {

            Debug.Log("Instantiation Attempts: " + n);

            //set up random location
            _xAxis = Random.Range(Min.x, Max.x);
                _yAxis = Random.Range(Min.y, Max.y);
                _zAxis = Random.Range(Min.z, Max.z);
                _randomPosition = new Vector3(_xAxis, _yAxis, _zAxis);

                scaleNum = Random.Range(sMin, sMax);
                scaleChange = new Vector3(scaleNum, scaleNum, scaleNum);

                //first check for possible collisions
                //Step 1: random position
                shapeCollider.transform.localScale = scaleChange;
                shapeCollider.transform.position = _randomPosition; //move collider to position
                shapeBounds = shapeCollider.GetComponent<Collider>().bounds;//get the collider

                //Step 2: compare position against active positions
                var valid = true; //how we will know if we have a valid shape
                                  //compare our testing box to every other active cloud shape's
                foreach (var cloud in CloudArray)
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
        var go = (GameObject)Instantiate(cloudGroup, _randomPosition, Quaternion.EulerRotation(-90, 0, 0));
            //var go = Instantiate(prefab); //create prefab at position
            go.transform.localScale = scaleChange;

            return go;
        /*

                    for (int i = 0; i < CloudArray.Length; i++)
                        {
                            //set up scales


                            CloudArray[i] = (GameObject)Instantiate(cloudGroup, _randomPosition, Quaternion.EulerRotation(-90, 0, 0)); ; //clouds are always rotated to show their face to the person lying down (x = -90)
                            CloudArray[i].name = "Cloud" + i;
                            CloudArray[i].transform.localScale = scaleChange;
                            //Debug.Log("cloud instantiated: " + CloudArray[i].name);
                        }
        */

    }
}

