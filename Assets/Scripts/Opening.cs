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


    void OnEnable()
    {
        EventManager.StartListening("Intro", Intro);

    }

    void OnDisable()
    {
        EventManager.StartListening("Intro", Intro);
    }

    // Start is called before the first frame update
    void Start()
    {
        Intro();
    }

    public void Intro()
    {
        //EventManager.TriggerEvent("sunset");
        Vector3 cloudposition = theoryPos.position;
        GameObject temp = Instantiate(cloudObjectPrefab, cloudposition, Quaternion.Euler(0f, 0f, 0f), transform);
      
        CloudShape cloud = temp.GetComponent<CloudShape>();
        cloud.SetShape(cloudTheory);
        cloud.ClarifyCloud();
        //set the amount of time to fade in TODO: make it werk!
        FadeObjectInOut fadeFunction = cloudObjectPrefab.GetComponent<FadeObjectInOut>();
        fadeFunction.fadeDelay = 0;
        fadeFunction.fadeTime = 0;
        fadeFunction.FadeIn(fadeFunction.fadeTime);

        EventManager.TriggerEvent("sunrise"); //listened to from the skyController

        //cloud.SlowDownClouds();

        //cloud.StopClouds();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
