using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Glow : MonoBehaviour {

    public bool LerpOnlyStartAndEnd = true;
    public Color startColor;
    public Color middleColor;
    public Color endColor;

    public float speed = 1f;

    private float t = 0;
	// Use this for initialization
	void Start () {
        Material RuntimeMaterial = new Material(this.gameObject.GetComponent<Image>().material);
        this.gameObject.GetComponent<Image>().material = RuntimeMaterial;
//        this.gameObject.GetComponent<Image>().material.CopyPropertiesFromMaterial(RuntimeMaterial);
    }
	
	// Update is called once per frame
	void Update () {

        if (LerpOnlyStartAndEnd)
        {
            this.gameObject.GetComponent<Image>().material.color = Color.Lerp(startColor, endColor, Mathf.Sin(Time.time * speed));
        }
        else
        {
            this.gameObject.GetComponent<Image>().material.color = Lerp3(startColor, middleColor, endColor, Mathf.Sin(Time.time * speed));
        }
    }


    private Color Lerp3(Color a, Color b, Color c, float t)
    {
        if (t <= 0.5f)
        {
            return Color.Lerp(a, b, t);
        }
        else
        {
            return Color.Lerp(b, c, t);
        }
    }

}
