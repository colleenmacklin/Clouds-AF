using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;

public class CloudLayerGroup : MonoBehaviour
{

    [Header("Cloud Objects")]
    [SerializeField]
    private Cloud highClouds,lowClouds;


    [Header("Cloud Distributions")]
    [SerializeField]
    [Range(3,5000)]
    private int maxCloudsPerLayer = 1000;
    [SerializeField]
    [Range(0f,1f)]
    private float cloudDistribution = .5f;
    
    const float MAX_CLOUD_DISTANCE = 40f;
    [SerializeField]
    [Range(0f,1f)]
    private float cloudsDistance = .5f;

    [Header("High Cloud Properties")]
    [SerializeField]
    private Sprite highShape;
    [SerializeField]
    private Material highCloudMaterial; // the renderer is not found?
    [SerializeField]
    [Range(0f,20f)]
    private float highCloudDuration = 10f;
    [SerializeField]
    [Range(0,5000)]
    private int highCloudEmissionRate = 35;
    
    [Header("Low Cloud Properties")]
    [SerializeField]
    private Sprite lowShape;
    [SerializeField]
    private Material lowCloudMaterial; // the renderer is not found?
    [SerializeField]
    [Range(0f,20f)]
    private float lowCloudDuration = 10f;
    [SerializeField]
    [Range(0,5000)]
    private int lowCloudEmissionRate = 35;

    [Button]
    void UpdateClouds(){
        //we have to get these at run time which is ridiculous
        var hcM = highClouds.ps.main;
        var hcE = highClouds.ps.emission;
        var hcS = highClouds.ps.shape;
        
        var lcM = lowClouds.ps.main;
        var lcE = lowClouds.ps.emission;
        var lcS = lowClouds.ps.shape;

        //set high cloud options
        hcM.maxParticles = (int)((float)maxCloudsPerLayer * (cloudDistribution));
        hcM.startLifetime = highCloudDuration;
        hcE.rateOverTime = highCloudEmissionRate;
        hcS.sprite = highShape ? highShape : hcS.sprite;

        //set low cloud options
        lcM.maxParticles = (int)((float)maxCloudsPerLayer * (1f - cloudDistribution));
        lcM.startLifetime = lowCloudDuration;
        lcE.rateOverTime = lowCloudEmissionRate;
        lcS.sprite = lowShape ? lowShape : lcS.sprite;

        //reposition clouds
        //calculate distance
        float distance = MAX_CLOUD_DISTANCE * cloudsDistance;
        //position calculated as an absolute of lower cloud position
        Vector3 upperPosition = new Vector3 (lowClouds.transform.position.x, lowClouds.transform.position.y + distance, lowClouds.transform.position.z);
        highClouds.transform.position = upperPosition;

    }


}
