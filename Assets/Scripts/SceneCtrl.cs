using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneCtrl : MonoBehaviour
{
    private int _currentScene;

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
        SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
    }
}
