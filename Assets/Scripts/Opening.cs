using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

/*
 * this class is specific to the opening, but becoming more like the CloudManager in the main game loop. Consider merging scenes?
 * TODO: 
 * --start model clouds off as generic cloud shapes, and then add their name textures
 * --when the model is selected, fade or transition the other model clouds back into generic clouds.
 * --there's a pretty long pause before the model loads, so perhaps as the model is loading, we could pull from a set of introductory texts
 */

public class Opening : MonoBehaviour
{
    public Transform theoryPos;
    public Transform M1Pos;
    public Transform M2Pos;
    public Transform M3Pos;


    public Texture2D cloudTheory;
    public Texture2D comedian;
    public Texture2D primordialEarth;
    public Texture2D philosopher;

    public GameObject chosenCloud; //used to select model
    public Texture2D chosenShape; //used to identify model text
    public ModelBuffer model_buffer; //reference to ModelBuffer


    [SerializeField]
    [Tooltip("The base cloud prefab")]
    private GameObject cloudObjectPrefab;
    public GameState gameState; //TODO: we'll need to reference GameState in each scene in the cloudManager or any other object containing an instantiated cloud. The GameCloud prefab references it. 

    public OpeningCloudShape clickedCloud;

    private IEnumerator pause;
    public List<OpeningCloudShape> cloudModels = new List<OpeningCloudShape>();    



    void OnEnable()
    {
        EventManager.StartListening("Intro", Title);
        Actions.GetClickedCloud += GetClickedCloud;

        //INDIECADE TODO: fade out non-chosen clouds
        //Actions.FadeInCloud += FadeInCloud;
        //Actions.FadeOutCloud += FadeOutCloud;

    }

    void OnDisable()
    {
        EventManager.StopListening("Intro", Title);
        Actions.GetClickedCloud -= GetClickedCloud;

    }

    // Start is called before the first frame update
    void Start()
    {
        Title();
    }


    public void Title()
    {


        //EventManager.TriggerEvent("sunset");
        Vector3 cloudposition = theoryPos.position;

        //TODO: setup other models - probably best to do this via a list!

        Vector3 model1_position = M1Pos.position;
        Vector3 model2_position = M2Pos.position;
        Vector3 model3_position = M3Pos.position;

        GameObject tempTitle = Instantiate(cloudObjectPrefab, theoryPos);
        GameObject tempPhilosopher = Instantiate(cloudObjectPrefab, M1Pos);
        GameObject tempComedian = Instantiate(cloudObjectPrefab, M2Pos);
        GameObject tempPrimordialEarth = Instantiate(cloudObjectPrefab, M3Pos);

        OpeningCloudShape titleCloud = tempTitle.GetComponent<OpeningCloudShape>();
        titleCloud.TurnOffCollider();
        titleCloud.FadeOutPS(0); //hides cloud by stopping particle system
        titleCloud.isGameLoop = false;
        titleCloud.scale = 20;
        titleCloud.SetShape(cloudTheory);

        OpeningCloudShape philosopherCloud = tempPhilosopher.GetComponent<OpeningCloudShape>();
        OpeningCloudShape comedianCloud = tempComedian.GetComponent<OpeningCloudShape>();
        OpeningCloudShape primordial_earth_Cloud = tempPrimordialEarth.GetComponent<OpeningCloudShape>();
        cloudModels.Add(philosopherCloud);
        cloudModels.Add(comedianCloud);
        cloudModels.Add(primordial_earth_Cloud);

        foreach (OpeningCloudShape tempCloud in cloudModels)
        {
            tempCloud.TurnOffCollider();
            tempCloud.isTarget = false;
            tempCloud.FadeOutPS(0); //hides cloud by stopping particle system
            tempCloud.isGameLoop = false;
        }

        philosopherCloud.scale = 10;
        philosopherCloud.SetShape(philosopher);

        comedianCloud.scale = 10;
        comedianCloud.SetShape(comedian);

        primordial_earth_Cloud.scale = 15;
        primordial_earth_Cloud.SetShape(primordialEarth);

        //foreach loop

        
        EventManager.TriggerEvent("sunrise"); //listened to from the skyController
                                              //set the amount of time to fade in TODO: make it werk!
                                              //vary time to fade in for each cloud



        
        titleCloud.FadeInPS(5);
        titleCloud.SharpenOpeningCloud(3f, 5f); //was 7f max
        titleCloud.SlowDownCloud();
        titleCloud.TurnOffCollider(); //don't detect this

        philosopherCloud.FadeInPS(7);
        philosopherCloud.SharpenOpeningCloud(1.5f, 3f);
        philosopherCloud.SlowDownCloud();

        comedianCloud.FadeInPS(9);
        comedianCloud.SharpenOpeningCloud(1.5f, 3f);
        comedianCloud.SlowDownCloud();

        primordial_earth_Cloud.FadeInPS(11);
        primordial_earth_Cloud.SharpenOpeningCloud(.7f, 2.5f);
        primordial_earth_Cloud.SlowDownCloud();

        pause = make_clickable(10.0f);
        StartCoroutine(pause);

    }

    private IEnumerator make_clickable(float waitTime)
    {

        yield return new WaitForSeconds(waitTime);

        Debug.Log("making clouds clickable");
        foreach (OpeningCloudShape tempCloud in cloudModels) //TODO: vary times for each cloud?
        {
            tempCloud.TurnOnCollider();
            tempCloud.isTarget = true;

        }

    }



    //check selected cloud, match with Model URL
    public void GetClickedCloud(GameObject c)
    {
        //write history code
        clickedCloud = c.GetComponent<OpeningCloudShape>();
        
        Debug.Log(c.name + " clicked: " + clickedCloud.CurrentShapeName);

        model_buffer.GetModel(clickedCloud.CurrentShapeName); //send model selected to ModelBuffer class
        FadeOutNonSelected();
    }

    private void FadeOutNonSelected()
    {
        //TODO INDIECADE
        foreach (OpeningCloudShape tempCloud in cloudModels)
        {
            if (tempCloud != clickedCloud)
            {
                Debug.Log("fading out non selected model clouds:" + tempCloud.CurrentShapeName);

                tempCloud.FadeOutPS(0);
                tempCloud.TurnOffCollider();
                tempCloud.isTarget = false;
            }
        }

    }
}


