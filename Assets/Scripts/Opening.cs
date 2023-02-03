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
        EventManager.StartListening("Intro", Title);
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
        cloud.scale = 20;
        cloud.SetShape(cloudTheory);
        cloud.ClarifyCloud();

        //set the amount of time to fade in TODO: make it werk!
        FadeObjectInOut fadeFunction = cloudObjectPrefab.GetComponent<FadeObjectInOut>();
        fadeFunction.fadeDelay = 2;
        fadeFunction.fadeTime = 2;
        if (cloudObjectPrefab.active) //are we loaded?
        {
            fadeFunction.FadeIn(fadeFunction.fadeTime);

        }
        EventManager.TriggerEvent("sunrise"); //listened to from the skyController

        //cloud.SlowDownClouds();

        //cloud.StopClouds();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
