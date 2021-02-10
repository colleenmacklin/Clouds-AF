using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myShape : MonoBehaviour
{
	public string myName;
    public GameObject cloud;
    public Texture2D shapeTexture;
    public Rect myRect;
    // Start is called before the first frame update

    /// <summary>
    // might want to set up some listeners to see if shape has changed, and then do the things that are currently in Update()
    /// </summary>
    void OnEnable()
    {
        EventManager.StartListening("UpdateMe", UpdateMe);
    }

    void OnDisable()
    {
        EventManager.StopListening("UpdateMe", UpdateMe);
    }


    public void UpdateMe() //called from cloudlayer any time a cloud shape changes
    {
        var myShapeIs = cloud.GetComponent<ParticleSystem>().shape.texture;
        myName = myShapeIs.name;
        shapeTexture = myShapeIs;

        myRect = new Rect(0, 0, shapeTexture.width, shapeTexture.height);
        var myPivot = new Vector2(0.5f, 0.5f);

    }
/*
    void Update()
    {

        if (Input.GetKeyDown("s"))
        {
            Color tmp = this.GetComponent<SpriteRenderer>().color;
            if (tmp.a == 0f)
            {
                tmp.a = 1f;
            }
            else { tmp.a = 0f; }

            this.GetComponent<SpriteRenderer>().color = tmp;

        }

        //print(myShapeIs.sprite.name);
    }
*/
}
