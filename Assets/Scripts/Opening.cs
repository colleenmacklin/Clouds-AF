using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opening : MonoBehaviour
{
    public Transform theoryPos;
    [SerializeField]
    private Texture2D cloudTheory;
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

        GameObject temp = Instantiate(cloudObjectPrefab, cloudposition, Quaternion.Euler(0f, 0f, 0f), transform);
      
        CloudShape cloud = temp.GetComponent<CloudShape>();
        cloud.isGameLoop = false;
        cloud.scale = 7;
        cloud.SetShape(cloudTheory);
        cloud.SharpenOpeningCloud(); //TODO: we will need to modify all of the cloud functions to be able to pass values relevant to different scenes --CM

        EventManager.TriggerEvent("sunrise"); //listened to from the skyController
                                              //set the amount of time to fade in TODO: make it werk!
        FadeObjectInOut fadeFunction = cloudObjectPrefab.GetComponent<FadeObjectInOut>();
        fadeFunction.fadeDelay = 5;
        fadeFunction.fadeTime = 5;
        //if (cloudObjectPrefab.active) //are we loaded?
        //{
            //Debug.Log("-----------cloud_is_Active");
            fadeFunction.FadeIn(fadeFunction.fadeTime);
            //cloud.SharpenOpeningCloud(); //TODO: we will need to modify all of the cloud functions to be able to pass values relevant to different scenes --CM
            cloud.SlowDownCloud(); //has no effect
        //}


        //cloud.SlowDownClouds();

        //cloud.StopClouds();
    }


}
