using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_script : MonoBehaviour
{
    public GameObject hiCloud;
    public GameObject lowCloud;
    public string hiName; //store the name of the underlying shape here
    public Texture2D newShape;
    public Texture2D currentShapeT;
    public bool changeShape;
    private ParticleSystem hps; 
    private ParticleSystem lps;
    public ParticleSystem.ShapeModule myShapeH;
    public ParticleSystem.ShapeModule myShapeL;

    // Start is called before the first frame update
    void Start()
    {
        hps = hiCloud.GetComponent<ParticleSystem>();
        lps =lowCloud.GetComponent<ParticleSystem>();
        myShapeH = hps.shape;
        myShapeL = lps.shape;
        currentShapeT = hps.shape.texture;

        hiName = hps.shape.texture.name;

        if (changeShape)
        {
            Debug.Log("changing shape");
            myShapeH.texture = newShape;
            myShapeL.texture = newShape;

            //hps.shape.texture = newShape;

        }
    

    //shape.texture = Assets.GetBuiltinExtraResource<Texture2D>("Default-Particle.psd");

}

// Update is called once per frame
void Update()
{
}
}

