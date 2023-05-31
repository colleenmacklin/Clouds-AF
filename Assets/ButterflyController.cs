using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButterflyController : MonoBehaviour
{
    [SerializeField]
    private Storyteller _storyTeller;

    public GameObject Butterfly;

    private GlowButterfly _glowButterfly;
    private void Awake()
    {
        _storyTeller.OnIntroComplete += NewButterfly;
        TextBoxController.OnDialogueComplete += NewButterfly;
        _glowButterfly = GetComponent<GlowButterfly>();
    }




    private void NewButterfly()
    {
        //instantiate butterfly (offscreen)
        //animate position in 
        
        Debug.Log("terry butterfly fly in");


        //for now just instantiate
        if (this.transform.childCount <1)
        {

        Instantiate(Butterfly, this.transform);
        }
        //set array 
        _glowButterfly.SetButteflyMatArray();

        

        
    }

 
}
