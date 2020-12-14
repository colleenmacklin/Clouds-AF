using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Game_Cloud : MonoBehaviour
{

    //Get the cloud's particle system, make available for cloudgroup
    public ParticleSystem ps;
    //Get the GameObject’s mesh renderer to access the GameObject’s material and color
    public MeshRenderer m_Renderer;
    Renderer rend;



    void Awake()
    {
        rend = GetComponent<ParticleSystemRenderer>();
    }



    void Start()
    {
       ps = this.GetComponent<ParticleSystem>();

        //Fetch the mesh renderer component from the GameObject
        m_Renderer = GetComponent<MeshRenderer>();

    }

}