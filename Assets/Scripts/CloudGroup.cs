using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CloudGroup : MonoBehaviour
{

    [Header("Cloud Objects")]
    [SerializeField]
    private Cloud highClouds, middleClouds, lowClouds;



    [Header("Cloud Distributions")]
    [SerializeField]
    [Range(3, 5000)]
    private int maxCloudsPerLayer = 1000;
    [SerializeField]
    [Range(0f, 1f)]
    private float highCloudsDensity = .3f;
    [SerializeField]
    [Range(0f, 1f)]
    private float middleCloudsDensity = .5f;
    [SerializeField]
    [Range(0f, 1f)]
    private float lowCloudsDensity = .3f;

    [Header("High Cloud Properties")]
    [SerializeField]
    private Sprite highShape;
    [SerializeField]
    private Material highCloudMaterial; // the renderer is not found?
    [SerializeField]
    [Range(0f, 20f)]
    private float highCloudDuration = 10f;
    [SerializeField]
    [Range(0, 5000)]
    private int highCloudEmissionRate = 35;

    [Header("Middle Cloud Properties")]
    [SerializeField]
    private Sprite middleShape;
    [SerializeField]
    private Material middleCloudMaterial; // the renderer is not found?
    [SerializeField]
    [Range(0f, 20f)]
    private float middleCloudDuration = 10f;
    [SerializeField]
    [Range(0, 5000)]
    private int middleCloudEmissionRate = 35;

    [Header("Low Cloud Properties")]
    [SerializeField]
    private Sprite lowShape;
    [SerializeField]
    private Material lowCloudMaterial; // the renderer is not found?
    [SerializeField]
    [Range(0f, 20f)]
    private float lowCloudDuration = 10f;
    [SerializeField]
    [Range(0, 5000)]
    private int lowCloudEmissionRate = 35;


    void UpdateClouds()
    {
        //we have to get these at run time which is ridiculous
        var hcM = highClouds.ps.main;
        var hcE = highClouds.ps.emission;
        var hcS = highClouds.ps.shape;

        var mcM = middleClouds.ps.main;
        var mcE = middleClouds.ps.emission;
        var mcS = middleClouds.ps.shape;

        var lcM = lowClouds.ps.main;
        var lcE = lowClouds.ps.emission;
        var lcS = lowClouds.ps.shape;

        //set high cloud options
        hcM.maxParticles = (int)((float)maxCloudsPerLayer * highCloudsDensity);
        hcM.startLifetime = highCloudDuration;
        hcE.rateOverTime = highCloudEmissionRate;
        hcS.sprite = highShape;

        // //set middle cloud options
        mcM.maxParticles = (int)((float)maxCloudsPerLayer * middleCloudsDensity);
        mcM.startLifetime = middleCloudDuration;
        mcE.rateOverTime = middleCloudEmissionRate;
        mcS.sprite = middleShape;

        //set low cloud options
        lcM.maxParticles = (int)((float)maxCloudsPerLayer * lowCloudsDensity);
        lcM.startLifetime = lowCloudDuration;
        lcE.rateOverTime = lowCloudEmissionRate;
        lcS.sprite = lowShape;
    }

    void Start()
    {
    }

}
