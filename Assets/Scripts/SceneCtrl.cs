using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneCtrl : MonoBehaviour
{
    private int _currentScene;
    public Animator transition;
    public float transitionTime = 1f;

    private void Awake()
    {
        _currentScene = SceneManager.GetActiveScene().buildIndex;
    }



    private void Start()
    {
        switch (_currentScene)
        {
            case 0:
                TitleCounter.OnTitleOver += LoadNextScene;
                break;
            case 1:
                ScrollCredits.OnCreditsOver += LoadNextScene;
                break;
            default:
                Debug.LogWarning("There is no (valid) active scene");
                break;
        }
    }
    private void OnDisable()
    {
        TitleCounter.OnTitleOver -= LoadNextScene;  
        ScrollCredits.OnCreditsOver -= LoadNextScene;
    }
    private void LoadNextScene(int scene)
    {
        if (_currentScene == 0)
        {
            StartCoroutine(Transition(1));
        }
        else
        {
            SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
        }
        
       
    }

    IEnumerator Transition(int scene) {
        //Play Crossfade Animation
        transition.SetTrigger("Start");
        //Wait
        yield return new WaitForSeconds(transitionTime);
        //Load Scene
        SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single); //should still be async?

    }

}
