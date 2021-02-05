using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Game_Cloud : MonoBehaviour
{

    //Get the cloud's particle system, make available for cloudgroup
    public ParticleSystem ps;
    public ParticleSystem.ShapeModule myShape;
    public Rect Myshape_rect;
    //Get the GameObject’s mesh renderer to access the GameObject’s material and color
    public MeshRenderer m_Renderer;
    public GameObject underlyingShape;
    private Vector3 starting_size;
    public Vector3 myScale;

    Renderer rend;

    void OnEnable()
    {
        EventManager.StartListening("UpdateMe", UpdateMe);
    }

    void OnDisable()
    {
        EventManager.StopListening("UpdateMe", UpdateMe);
    }



    void Awake()
    {
        rend = GetComponent<ParticleSystemRenderer>();
        ps = this.GetComponent<ParticleSystem>();
        myShape = ps.shape;
        //set starting size
        starting_size = new Vector3(5.0f, 5.0f, 5.0f);
        myShape.scale = starting_size;

    }


    void Start()
    {
        //Fetch the mesh renderer component from the GameObject
        m_Renderer = GetComponent<MeshRenderer>();

    }

    public void UpdateMe() //called from cloudlayer
    {
        //this function primarily makes sure that the underlying texture remains true to aspect ratio. Otherwise it squares everything off!
        myShape = ps.shape;
        myScale = new Vector3(myShape.scale.x, myShape.scale.y, myShape.scale.z);


        if (myScale != starting_size)
        {
            myShape.scale = starting_size;
        }

        Myshape_rect = new Rect(0, 0, myShape.texture.width, myShape.texture.height); //size of underlying texture
        float aspect = Myshape_rect.width / Myshape_rect.height; //get aspect ratio


        if (aspect < 1)
        {
           myScale = new Vector3(myShape.scale.x * 1, myShape.scale.y * (1 + aspect), myShape.scale.z * 1);
           myShape.scale = myScale;

        }

        if (aspect > 1)
        {
           myScale = new Vector3(myShape.scale.x * aspect, myShape.scale.y * 1, myShape.scale.z * 1);
           myShape.scale = myScale;

        }

      /* Debugging
                Debug.Log(myShape.texture.name + "Texture Size: " + Myshape_rect);
                Debug.Log(myShape.texture.name + "Aspect: " + aspect);
                Debug.Log(myShape.texture.name + " Scaled: " + myShape.scale);
         */

        //var myPivot = new Vector2(0.5f, 0.5f);
        //spriteRenderer.sprite = Sprite.Create(shapeTexture, myRect, myPivot);


    }


}