using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
This script controls the child object that is attacked to the Cloud Object.

At present, not sure what we can use it for. But it is here in case we wanted 
add specific, child-level behaviors (unlikely but possible) without changing 
overall behavior of the rest of the object (like the collider behaviors).

*/

public class CloudSubcontroller : MonoBehaviour
{

    //Get the cloud's particle system, make available for cloudgroup
    public ParticleSystem ps;
    public ParticleSystem.ShapeModule myShape;
    public Rect Myshape_rect;
    //Get the GameObject’s mesh renderer to access the GameObject’s material and color
    public MeshRenderer m_Renderer;
    private Vector3 starting_size;
    public Vector3 myScale;

    private float srcWidth;
    private float srcHeight;
    private float minWidth = 10f; //hardcoded
    private float minHeight = 10f; //hardcoded


    Renderer rend;
    public bool visible_at_start = true;

    void OnEnable()
    {

    }

    void OnDisable()
    {

    }

    void Awake()
    {
        rend = GetComponent<ParticleSystemRenderer>();
        ps = this.GetComponent<ParticleSystem>();
        myShape = ps.shape;
        //set starting size
        starting_size = new Vector3(10.0f, 10.0f, 10.0f);
        //starting_size = new Vector3(myShape.scale.x, myShape.scale.y, myShape.scale.z);
        //Debug.Log("starting size: " + starting_size);
        myShape.scale = starting_size;

    }


}