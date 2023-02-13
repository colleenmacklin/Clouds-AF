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
    private GameObject _cloud;

    void OnEnable()
    {
        EventManager.StartListening("Intro", Title);

    }

    void OnDisable()
    {
        EventManager.StartListening("Intro", Title);
    }

    private void Awake()
    {

        Vector3 cloudposition = theoryPos.position;
        _cloud = Instantiate(cloudObjectPrefab, cloudposition, Quaternion.Euler(0f, 0f, 0f), transform);
        _cloud.SetActive(true);
        if (!_cloud.activeInHierarchy)
        {
            

        }

    }

    // Start is called before the first frame update
    void Start()
    {
        Title();
    }

    public void Title()
    {

        CloudShape cloudShape = _cloud.GetComponent<CloudShape>();
        cloudShape.isGameLoop = false;
        cloudShape.scale = 20;
        cloudShape.SetShape(cloudTheory);
        cloudShape.ClarifyCloud();

        ConstantForce wind = _cloud.GetComponent<ConstantForce>();
        Vector3 f = Vector3.zero;

        wind.force = f;
        

        FadeObjectInOut fadeFunction = cloudObjectPrefab.GetComponent<FadeObjectInOut>();
        fadeFunction.fadeInOnStart = false;
        fadeFunction.fadeDelay = 3;
        fadeFunction.fadeTime = 10;

        fadeFunction.FadeIn(fadeFunction.fadeTime);


        EventManager.TriggerEvent("sunrise"); //listened to from the skyController


    }


}
