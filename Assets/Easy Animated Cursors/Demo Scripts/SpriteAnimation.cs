using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteAnimation : MonoBehaviour {

    public Sprite[] SpriteArray;
    public float Speed = 1f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float index = Time.time * Speed * 30;
        index = index % SpriteArray.Length;
        this.GetComponent<Image>().sprite = SpriteArray[(int)index];
    }


}
