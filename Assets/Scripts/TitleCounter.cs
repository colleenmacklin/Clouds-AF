using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCounter : MonoBehaviour
{
    [SerializeField]
    private float _timeCount = 30; //seconds

    public static event Action<int> OnTitleOver;

    private bool _finishedCount = false;

    private bool _modelIsLoading = false;

    private void OnEnable()
    {
        ModelBuffer.OnStartedLoadingModel += OnLoadingModel;
    }

    private void OnLoadingModel()
    {
        _modelIsLoading = true;
    }

    private void OnDisable()
    {
        ModelBuffer.OnStartedLoadingModel -= OnLoadingModel;
    }

    private void Update()
    {
        if (_modelIsLoading)
        {
            if (!_finishedCount)
            {
                if (_timeCount > 0)
                {
                    _timeCount -= Time.deltaTime;
                }
                else
                {
                    OnTitleOver?.Invoke(1);
                    _finishedCount = true;
                }
            }
        }
    }
}
