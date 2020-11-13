using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myShape : MonoBehaviour
{
	public string myName;
    //private Sprite mySprite;
    public GameObject cloud;
    public SpriteRenderer spriteRenderer;
    // Start is called before the first frame update

    private void Awake()
    {
        //var myShapeIs = this.GetComponent<ParticleSystem>().shape.sprite;
       // myName = myShapeIs.name;
        //mySprite = myShapeIs;

    }
    void Start()
    {
        // var myShapeIs = this.GetComponent<ParticleSystem>().shape;
        // myName = myShapeIs.sprite.name;
    }

    // Update is called once per frame
    void Update()
    {
        var myShapeIs = cloud.GetComponent<ParticleSystem>().shape.sprite;
		myName = myShapeIs.name;
        spriteRenderer.sprite = myShapeIs;
        if (Input.GetKeyDown("s"))
        {
            Color tmp = this.GetComponent<SpriteRenderer>().color;
            if (tmp.a > 0)
            {
                tmp.a = 0f;
            }
            else { tmp.a = 1f; }

            this.GetComponent<SpriteRenderer>().color = tmp;

        }


        //print(myShapeIs.sprite.name);
    }
}
