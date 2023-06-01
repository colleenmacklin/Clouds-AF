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

    public GameObject chosenCloud; //used to select model
    public Texture2D chosenShape; //used to identify model text


    [SerializeField]
    [Tooltip("The base cloud prefab")]
    private GameObject cloudObjectPrefab;
    public GameState gameState; //TODO: we'll need to reference GameState in each scene in the cloudManager or any other object containing an instantiated cloud. The GameCloud prefab references it. 

    public OpeningCloudShape clickedCloud;


    void OnEnable()
    {
        EventManager.StartListening("Intro", Title);
        Actions.GetClickedCloud += GetClickedCloud;

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

        //other clouds (models)
        OpeningCloudShape philosopherCloud = tempPhilosopher.GetComponent<OpeningCloudShape>();
        philosopherCloud.TurnOffCollider();
        philosopherCloud.FadeOutPS(0); //hides cloud by stopping particle system
        philosopherCloud.isGameLoop = false;
        philosopherCloud.scale = 10;
        philosopherCloud.SetShape(philosopher);

        //other clouds (models)
        OpeningCloudShape comedianCloud = tempComedian.GetComponent<OpeningCloudShape>();
        comedianCloud.TurnOffCollider();
        comedianCloud.FadeOutPS(0); //hides cloud by stopping particle system
        comedianCloud.isGameLoop = false;
        comedianCloud.scale = 10;
        comedianCloud.SetShape(comedian);

        //other clouds (models)
        OpeningCloudShape primordial_earth_Cloud = tempPrimordialEarth.GetComponent<OpeningCloudShape>();
        primordial_earth_Cloud.TurnOffCollider();
        primordial_earth_Cloud.FadeOutPS(0); //hides cloud by stopping particle system
        primordial_earth_Cloud.isGameLoop = false;
        primordial_earth_Cloud.scale = 12;
        primordial_earth_Cloud.SetShape(primordialEarth);


        EventManager.TriggerEvent("sunrise"); //listened to from the skyController
                                              //set the amount of time to fade in TODO: make it werk!
                                              //vary time to fade in for each cloud



        /*
        FadeObjectInOut titleFade = titleCloud.GetComponent<FadeObjectInOut>();
        FadeObjectInOut M1Fade = philosopherCloud.GetComponent<FadeObjectInOut>();
        FadeObjectInOut M2Fade = comedianCloud.GetComponent<FadeObjectInOut>();
        FadeObjectInOut M3Fade = primordial_earth_Cloud.GetComponent<FadeObjectInOut>();

        titleFade.fadeDelay = 5;
        titleFade.fadeTime = 5;

        M1Fade.fadeDelay = 7;
        M1Fade.fadeTime = 5;

        M2Fade.fadeDelay = 9;
        M2Fade.fadeTime = 5;

        M3Fade.fadeDelay = 11;
        M3Fade.fadeTime = 5;

        titleFade.FadeIn(titleFade.fadeTime);
        M1Fade.FadeIn(M1Fade.fadeTime);
        M2Fade.FadeIn(M1Fade.fadeTime);
        M3Fade.FadeIn(M1Fade.fadeTime);
        */
        //TODO: Danger!! Hardcoded values!! Calculate sharpening (aka particle sizes) based on cloud scale
       
        titleCloud.FadeInPS(5);
        titleCloud.SharpenOpeningCloud(3f, 5f); //was 7f max
        titleCloud.SlowDownCloud();
        titleCloud.TurnOffCollider(); //don't detect this

        philosopherCloud.FadeInPS(7);
        philosopherCloud.SharpenOpeningCloud(1.5f, 3f);
        philosopherCloud.SlowDownCloud();
        philosopherCloud.isTarget = true; //mark this cloud as a target shape
        philosopherCloud.TurnOnCollider(); //don't detect this

        comedianCloud.FadeInPS(9);
        comedianCloud.SharpenOpeningCloud(1.5f, 3f);
        comedianCloud.SlowDownCloud();
        comedianCloud.isTarget = true; //mark this cloud as a target shape
        comedianCloud.TurnOnCollider(); //don't detect this

        primordial_earth_Cloud.FadeInPS(11);
        primordial_earth_Cloud.SharpenOpeningCloud(.7f, 2.5f);
        primordial_earth_Cloud.SlowDownCloud();
        primordial_earth_Cloud.isTarget = true; //mark this cloud as a target shape
        primordial_earth_Cloud.TurnOnCollider(); //don't detect this

    }

    //check selected cloud, match with Model URL
    public void GetClickedCloud(GameObject c)
    {
        //write history code
        clickedCloud = c.GetComponent<OpeningCloudShape>();
        
        Debug.Log(c.name + " clicked: " + clickedCloud.CurrentShapeName);

        switch (clickedCloud.CurrentShapeName)
        {
            case "philosopher":
                print("Why hello there good sir! Let me teach you about Trigonometry!");

                break;
            case "primordial_earth":
                print("Hello and good day!");
                break;
            case "comedian":
                print("Whadya want?");
                break;
            default:
                print("Incorrect intelligence level.");
                break;
        }
    }
}


