using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField]
    [Tooltip("The base cloud prefab")]
    private GameObject cloudObjectPrefab;
    public GameState gameState; //TODO: we'll need to reference GameState in each scene in the cloudManager or any other object containing an instantiated cloud. The GameCloud prefab references it. 


    void OnEnable()
    {
        EventManager.StartListening("Intro", Title);

    }

    void OnDisable()
    {
        EventManager.StopListening("Intro", Title);
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
        titleCloud.isGameLoop = false;
        titleCloud.scale = 17;
        titleCloud.SetShape(cloudTheory);

        //other clouds (models)
        OpeningCloudShape philosopherCloud = tempPhilosopher.GetComponent<OpeningCloudShape>();
        philosopherCloud.isGameLoop = false;
        philosopherCloud.scale = 10;
        philosopherCloud.SetShape(philosopher);

        //other clouds (models)
        OpeningCloudShape comedianCloud = tempComedian.GetComponent<OpeningCloudShape>();
        comedianCloud.isGameLoop = false;
        comedianCloud.scale = 8;
        comedianCloud.SetShape(comedian);
        Debug.Log("comedianCloudShape = "+comedianCloud.currentShape);

        //other clouds (models)
        OpeningCloudShape primordial_earth_Cloud = tempPrimordialEarth.GetComponent<OpeningCloudShape>();
        primordial_earth_Cloud.isGameLoop = false;
        primordial_earth_Cloud.scale = 12;
        primordial_earth_Cloud.SetShape(primordialEarth);


        EventManager.TriggerEvent("sunrise"); //listened to from the skyController
                                              //set the amount of time to fade in TODO: make it werk!
                                              //vary time to fade in for each cloud
        FadeObjectInOut titleFade = titleCloud.GetComponent<FadeObjectInOut>();
        FadeObjectInOut M1Fade = philosopherCloud.GetComponent<FadeObjectInOut>();
        FadeObjectInOut M2Fade = comedianCloud.GetComponent<FadeObjectInOut>();
        FadeObjectInOut M3Fade = primordial_earth_Cloud.GetComponent<FadeObjectInOut>();

        titleFade.fadeDelay = 5;
        titleFade.fadeTime = 5;

        M1Fade.fadeDelay = 6;
        M1Fade.fadeTime = 5;

        M2Fade.fadeDelay = 7;
        M2Fade.fadeTime = 5;

        M3Fade.fadeDelay = 8;
        M3Fade.fadeTime = 5;

        //if (cloudObjectPrefab.active) //are we loaded?
        //{
        //Debug.Log("-----------cloud_is_Active");
        titleFade.FadeIn(titleFade.fadeTime);
        M1Fade.FadeIn(M1Fade.fadeTime);
        M2Fade.FadeIn(M1Fade.fadeTime);
        M3Fade.FadeIn(M1Fade.fadeTime);

        //TODO: Danger!! Hardcoded values!! Calculate sharpening (aka particle sizes) based on cloud scale


        titleCloud.SharpenOpeningCloud(3f, 7f); 
        titleCloud.SlowDownCloud(); //has no effect

        comedianCloud.SharpenOpeningCloud(1f,3f);
        comedianCloud.SlowDownCloud();

        philosopherCloud.SharpenOpeningCloud(1f, 3f);
        philosopherCloud.SlowDownCloud();


        primordial_earth_Cloud.SharpenOpeningCloud(1f, 3f);
        primordial_earth_Cloud.SlowDownCloud();


        //}


        //cloud.SlowDownClouds();

        //cloud.StopClouds();
    }


}
