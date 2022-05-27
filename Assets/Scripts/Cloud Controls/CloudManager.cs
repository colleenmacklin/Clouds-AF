﻿using System.IO;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using PoissonDisc;
using System.Linq;

/*

    Cloud Manager is responsible for creating the clouds into an evenly (poisson-distributed) grid
    It is also the source of the event handling, as it can route the event to the clouds. 
    The Manager, then, acts as the cloud conductor as it controls how clouds should be acting

    Events have been mostly shifted over to the clouds

    But generation events and transitions are still on the manager
    * Begin -> GenerateNew
    * StoryEnded -> NewTargets

    //TODO:: BRING THE CLOUD EVENTS INTO THIS MANAGER SO WE CAN CONTROL HOW THEY ALL BEHAVE
    IN TANDEM AND ALSO FIRE EVENTS WHEN THEY FINISH ACTING

    Developmental Note** because poisson positions actually come in an ordered fashion, we 
    can actually use the structure to select for the "middle" if we start there and fan out.
    The fan out naturally allow us to center our target shapes without additional search.
    This is left as a task for a future developer.
*/

public class CloudManager : MonoBehaviour
{
    [Range(0f, 40f)]
    public float pauseBetweenText = 5f;

    public CloudShape clickedCloud;

    [Header("Cloud Properties")]
    [SerializeField]
    [Tooltip("How many clouds to create")]
    private int numberOfCloudsToGenerate;
    [SerializeField]
    [Tooltip("How many of the clouds to turn into targets")]
    private int numberOfTargetsToGenerate;
    //[SerializeField]
    //private int numberOfPeopleToGenerate;
    //[SerializeField]
    //private int numberOfAnimalsToGenerate;
    //[SerializeField]
    //private int numberOfObjectsToGenerate;
    [SerializeField]
    [Tooltip("All the generated clouds in the sky")]
    private List<GameObject> generatedCloudObjects;
    [SerializeField]
    [Tooltip("The base cloud prefab")]
    private GameObject cloudObjectPrefab;
    [SerializeField]
    [Tooltip("delay for making cloud clickable")]
    private int transitionTime;


    [Header("Cloud Data")]//consider refactor as cloud scriptable objects
    [SerializeField]
    private Texture2D[] cloudTargetsArray; //textures stay as an array because we are not generating run time textures
    [SerializeField]
    private List<Texture2D> cloudTargetsList; //should probably try to convert this so we can remove items from the list
    //[SerializeField]
    //private Texture2D[] cloudPersonTargetsArray;
    //[SerializeField]
    //private Texture2D[] cloudAnimalTargetsArray;
    //[SerializeField]
    //private Texture2D[] cloudObjectTargetsArray;
    [SerializeField]
    private Texture2D[] cloudGenericsArray; //textures stay as an array because we are not generating run time textures
    [SerializeField]
    private List<string> cloudsSelectedHistory;
    [SerializeField]
    private List<string> cloudsActiveHistory;


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
    public float poissonRadius = 20;//default is 20

    [SerializeField]
    [Tooltip("The size of the region to create points in")] //This region is from 0,0 and must be translated 
    public Vector2 poissonRegionSize = new Vector2(150f, 100f);//default we will use 150x120

    [SerializeField]

    [Tooltip("How far to translate offset the region")]
    public Vector3 regionTranslation = new Vector3(-75, 60f, -40f);//default is -30,-40,0

    [SerializeField]
    [Tooltip("Number of rejection samples before giving up on a sample. Default is 30 ")]
    public int poissonRejectionSamples = 30;//this can comfortably be a higher number

    ///////////////////////
    //
    // Monobehaviors
    //
    /////////////////////////

    void OnEnable()
    {
        //EventManager.StartListening("Setup", GenerateNewClouds); //from Storyteller Start()
        EventManager.StartListening("Setup", GenerateClouds); //from Storyteller Start()

        EventManager.StartListening("DoneReading", SetNextShapes); //should only apply to the cloud that was being remarked upon - use an Action
        Actions.GetClickedCloud += GetClickedCloud;
        //Actions.CloudIsReady += ReadyCloud;
    }

    void OnDisable()
    {
        //EventManager.StopListening("Setup", GenerateNewClouds);
        EventManager.StopListening("Setup", GenerateClouds);

        EventManager.StopListening("DoneReading", SetNextShapes); //should only apply to the cloud that was being remarked upon - use an Action
        Actions.GetClickedCloud -= GetClickedCloud;
        //Actions.CloudIsReady -= ReadyCloud;

    }
    void Awake()
    {
        SetTextureArrays(); //more intensive(?), so we do this in awake
    }

    void Start()
    {
        //Start Act Intro, not the cloud shape
        //        EventManager.TriggerEvent("Introduction");
        //       EventManager.TriggerEvent("SpawnShape");
    }

    void SetTextureArrays()
    {//from unity docs
     // would be how to do it as a List
        
        cloudTargetsArray = Resources.LoadAll("Cloud_Targets", typeof(Texture2D)).Cast<Texture2D>().ToArray();
        cloudTargetsList = new List<Texture2D>(cloudTargetsArray);
        cloudGenericsArray = Resources.LoadAll("Cloud_Natural", typeof(Texture2D)).Cast<Texture2D>().ToArray();
    }

    //Basically make sure we always have updated images in our array
    void OnValidate()
    {
        SetTextureArrays();
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

            float sizeScale = UnityEngine.Random.Range(scaleRange.x, scaleRange.y);
            Vector3 scaleVector = new Vector3(sizeScale, sizeScale, sizeScale);

            //get a random scale for the transform
            go.transform.localScale = scaleVector; //CM: Need to change to scale the underlying shape - which will unify the fluffiness of the clouds, but differentiate their shapes.
            go.name = $"Cloud {i}";

            generatedCloudObjects.Add(go);
        }

        EventManager.TriggerEvent("CloudsGenerated"); //heard by nothing?
        EventManager.TriggerEvent("SlowDownClouds"); //heard by CloudShape
        //should add some generic shapes here...
        SetCloudsToGenericShapes();
    }

    //Set all clouds to some generic cloud (non target) Shapes
    //Note this is simpler because we do not care about repeats here
    void SetCloudsToGenericShapes()
    {
        //Shuffle clouds shape array
        cloudGenericsArray = ShuffleArray(cloudGenericsArray);

        //loop through all clouds

        for (int i = 0; i < generatedCloudObjects.Count; i++)
        {
            //Get the shape component
            CloudShape cloud = generatedCloudObjects[i].GetComponent<CloudShape>();
            //Tell the cloud to handle the texture
            cloud.SetShape(cloudGenericsArray[i % cloudGenericsArray.Length]);
            cloud.TurnOffCollider();
        }
    }

    //---------For future implementation, call from Narrator after a shaped cloud has been selected
    void SetSingleCloudToGenericShape(CloudShape c) //for a future action on a single cloud called from narrator
    {
        //Shuffle clouds shape array
        cloudGenericsArray = ShuffleArray(cloudGenericsArray);

        //loop through all clouds
          CloudShape cloud = c;
        //pick a random cloudGeneric from the cloudGenericsArray
        int index = UnityEngine.Random.Range(0, cloudGenericsArray.Length);

        //Tell the cloud to handle the texture
        cloud.SetShape(cloudGenericsArray[index]);
            cloud.TurnOffCollider();
    }


    //Set specified number of clouds to the target shapes
    //Is uncontrolled to an extent, entirely random -- we might want to change that so that we can vary where these appear 
    //Create a comparison list as we go along so we do not repeat shapes set to set
    void SetCloudsToTargetShapes()
    {
        //shuffle possible targets
        cloudTargetsList = ShuffleList(cloudTargetsList);
        //cloudTargetsArray = ShuffleArray(cloudTargetsArray);
        //cloudPersonTargetsArray = ShuffleArray(cloudPersonTargetsArray);
        //cloudAnimalTargetsArray = ShuffleArray(cloudAnimalTargetsArray);
        //cloudObjectTargetsArray = ShuffleArray(cloudObjectTargetsArray);
        //int tempPeople = numberOfPeopleToGenerate;
        //int tempAnimals = numberOfAnimalsToGenerate;
        //int tempObjects = numberOfObjectsToGenerate;

        //shuffle the generated clouds
        generatedCloudObjects = ShuffleList(generatedCloudObjects);

        //Store the next set of active targets so we compare them later
        List<string> incomingActiveTargets = new List<string>();

        for (int i = 0; i < numberOfTargetsToGenerate; i++)
        {

            int indexOffset = 0;
            if (i >= generatedCloudObjects.Count)
            {
                Debug.Log($"Out of potential clouds to convert to target, reached {i} of {numberOfTargetsToGenerate}.");
                break;
            }
            //Get the shape component
            CloudShape cloud = generatedCloudObjects[i].GetComponent<CloudShape>();

            Texture2D nextShape;

            //if (tempPeople != 0)
            //{
            //    nextShape = cloudPersonTargetsArray[(i + indexOffset) % cloudPersonTargetsArray.Length];
            //    tempPeople -= 1;
            //} else if (tempAnimals != 0)
            //{
            //    nextShape = cloudAnimalTargetsArray[(i + indexOffset) % cloudAnimalTargetsArray.Length];
            //    tempAnimals -= 1;
            //} else if (tempObjects != 0)
            //{
            //    nextShape = cloudObjectTargetsArray[(i + indexOffset) % cloudObjectTargetsArray.Length];
            //    tempObjects -= 1;
            //} else
            //{
                nextShape = cloudTargetsList[(i + indexOffset) % cloudTargetsList.Count];
            //}

            //compare current texture with existing selections
            //if it's already been used, then we skip forward in the deck
            //=================================================================NOTE: this doesn't work, check logic (CM, 5/10)
            Debug.Log("Cloud CurrentShapeName: " + cloud.CurrentShapeName);


            //while (cloudsActiveHistory.Contains(cloud.CurrentShapeName) ||
                    //cloudsSelectedHistory.Contains(cloud.CurrentShapeName) ||
                    //nextShape.name == cloud.CurrentShapeName)

                while (cloudsActiveHistory.Contains(nextShape.name) ||
                    cloudsSelectedHistory.Contains(nextShape.name))
                {
                 Debug.Log("shape was previously seen");
                 //cloudTargetsList.Remove(nextShape);
                

                //we should remove the object from the array - but this would be easier if it was a list!
                indexOffset++;

                nextShape = cloudTargetsList[(i + indexOffset) % cloudTargetsList.Count];

                //if we've gone through all the options, break out.
                if (indexOffset >= cloudTargetsList.Count)
                {
                    Debug.Log("Looped through all possible targets but could not find non-duplicate, breaking");
                    break;
                }
            }

            //stop particles
            //StopCloud(cloud);

            //Tell the cloud to handle the texture
            cloud.SetShape(nextShape);

            StartCoroutine(waitToMakeClickable(cloud));//this gets applied to all clouds - but should only apply to clouds that are "transitioning"


            incomingActiveTargets.Add(nextShape.name);
        }

        //replace active history with new set of targets
        cloudsActiveHistory = incomingActiveTargets;
    }

    //inactive
    IEnumerator waitToMakeClickable(CloudShape c)
    {

        //we need to wait until the cloud has stopped transitioning - which is based on a guess!
        yield return new WaitForSeconds(transitionTime);


        //Start particle system
        //StartCloud(c);
        //c.TurnOnCollider();
        ReadyCloud(c);//probably don't need a seperate function

    }

    //Set a specific cloud to a shape
    //cm 4/15 commented out
    //void SetCloudToShape(int index, Texture2D shape)
    //{
    //TO DO
    //}
    void ReadyCloud(CloudShape c)
    {
        //if (c.ready)
        //{
            //Debug.Log("Cloud: " + c + "Is ready");

            c.TurnOnCollider();

        //}
        //else c.TurnOffCollider();
    }


    public void GenerateNewClouds() //called from "setup" event, via storyteller, Start() 
    {
        //GenerateClouds();
        SetNextShapes();
    }

    public void SetNextShapes() //called from Narrator -> "DoneReading" event 
    {
        SetCloudsToGenericShapes(); //should be applied to only the cloud remarked upon from narrator
        SetCloudsToTargetShapes(); //should be removed, and placed on a random timer
    }



    //////////////////
    //
    //   Event Behavior
    //
    //////////////////////

    //Two versions of the Cloud Actions from the manager level
    //But because events can't take furhter data right now, we would need to add yet another function
    //To wrap around this.


    // TODO:: Set up these manager level functions because they let us fire events when
    // all the cloud actions are **DONE** 
    // That is actually pretty important!!!
    IEnumerable PerformCloudActionOverTime(Action<CloudShape> cloudAction, float totalExecutionTIme)
    {
        float interval = totalExecutionTIme / generatedCloudObjects.Count;

        foreach (GameObject go in generatedCloudObjects)
        {
            CloudShape c = go.GetComponent<CloudShape>();
            cloudAction(c);
            yield return new WaitForSeconds(interval);
        }
    }
    void PerformCloudAction(Action<CloudShape> cloudAction)
    {
        foreach (GameObject go in generatedCloudObjects)
        {
            CloudShape c = go.GetComponent<CloudShape>();
            cloudAction(c);
        }
    }

    //These are added at the manager level in case we need to do any high order scope comparisons
    //Otherwise, the event driven form is fine.


    void SlowDownCloud(CloudShape c)
    {
        c.SlowDownCloud();
    }
    void ClarifyCloud(CloudShape c)
    {
        c.ClarifyCloud();
    }

    void SharpenCloud(CloudShape c)
    {
        c.SharpenCloud();
        Debug.Log("SharpenCloud: " + c);

    }

    void BlurCloud(CloudShape c)
    {
        c.BlurCloud();
        Debug.Log("Blur: " + c);

    }


    void StopCloud(CloudShape c)
    {
        c.StopCloud();
    }

    void StartCloud(CloudShape c)
    {
        c.StartCloud();
    }


    ///////////////////////
    //
    //  Utilities
    //
    ///////////////////////////////

    //Keep Track of Clicked Clouds

    public void GetClickedCloud(GameObject c) //from storyteller
    {
        //write history code
        clickedCloud = c.GetComponent<CloudShape>();
        Debug.Log("----cloud clicked: " + clickedCloud.CurrentShapeName);
        cloudsSelectedHistory.Add(clickedCloud.CurrentShapeName);
        cloudTargetsList.Remove(clickedCloud.currentShape);
    }

    //Generic Shuffler
    List<T> ShuffleList<T>(List<T> list)
    {
        List<T> shuffledResult = list;
        int n = list.Count;
        while (n > 1)
        {
            n--;//simplifies the shuffledResult[n] down below

            int i = UnityEngine.Random.Range(0, n + 1);
            var temp = shuffledResult[i];

            shuffledResult[i] = shuffledResult[n];
            shuffledResult[n] = temp;
        }
        return shuffledResult;

    }


    T[] ShuffleArray<T>(T[] array)
    {
        T[] shuffledResult = array;
        int n = array.Length;
        while (n > 1)
        {
            n--;//simplifies the shuffledResult[n] down below

            int i = UnityEngine.Random.Range(0, n + 1);
            var temp = shuffledResult[i];

            shuffledResult[i] = shuffledResult[n];
            shuffledResult[n] = temp;
        }
        return shuffledResult;

    }
    //for the prototype we add this behavior of game logic here, it needs to be separate
    private void SetCloudToShape()
    {
        print("setCloudtoShape called");

        cloudSelectionIndex++;


        if (cloudSelectionIndex > cloudTargetsList.Count)
        {
            //we've already seen the ending
            EventManager.TriggerEvent("AllShapesSeen");
            return;
        }

        if (cloudSelectionIndex == cloudTargetsList.Count)
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
        int cloudNum = (UnityEngine.Random.Range(0, generatedCloudObjects.Count));
        chosenShape = cloudTargetsList[shapeNum];
        chosenCloud = generatedCloudObjects[cloudNum];
        selectionHistory.Add(chosenShape.name);
        //cloudsSelectedHistory.Add(chosenShape.name); //cm added 4/15


        Debug.Log("chosenCloud: " + chosenCloud);


        //tells cloud that it is not a shape anymore, changes it's shape...
        TurnOffCloud();

        //This needs to be adjusted from the SpawnShape
        //Right now the chosenCloud is compared against from the clouds?
        //We should change this paradigm so that the shape is directly set
        //Because the manager, if it exists at all, should manage. Not respond.
        //i.e. clouds should not be responsible for knowing they are chosen

        EventManager.TriggerEvent("SpawnShape"); //tell a cloud to turn into a shape -- this should not all happen at the same time

        StartCoroutine(PauseBeforeTalking());
        //This is where the dialogue manager is activated.
        //maybe trigger this once the cloud has become a shape?
        //EventManager.TriggerEvent("Talk"); //Friend starts talking about the shape (on a timer)
    }

    private IEnumerator PauseBeforeTalking()
    {

        EventManager.TriggerEvent("Talk");
        yield return new WaitForSeconds(pauseBetweenText);

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
    }*/
    

}
