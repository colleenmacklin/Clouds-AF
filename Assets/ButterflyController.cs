using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButterflyController : MonoBehaviour
{
    [CanBeNull] //is null in opening, needs to be refereneced in main scene
    [SerializeField]
    private Storyteller _storyTeller;

    public GameObject Butterfly;

    private GlowButterfly _glowButterfly;
    private void Awake()
    {
        _glowButterfly = GetComponent<GlowButterfly>();
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            NewButterfly();
        }
        else
        {
            _storyTeller.OnIntroComplete += NewButterfly;
            TextBoxController.OnDialogueComplete += NewButterfly;
        }

    }

    private void OnDisable()
    {
        if (_storyTeller)
        {
            _storyTeller.OnIntroComplete -= NewButterfly;
        }
    }


    private void NewButterfly()
    {
        //instantiate butterfly (offscreen)
        //TODO animate position in 



        //for now just instantiate
        if (this.transform.childCount < 1)
        {

            Instantiate(Butterfly, this.transform);
        }
        //set array 
        _glowButterfly.SetButteflyMatArray();




    }


}
