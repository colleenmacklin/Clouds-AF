using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollCredits : MonoBehaviour
{
    [SerializeField]
    private RectTransform _rt;

    private float _startY;

    [SerializeField]
    private float _scrollSpeed;

    private float _endY;

    private bool _creditsAreScrolling = false;

    public static event Action<int> OnCreditsOver;

    private void OnEnable()
    {
        TextBoxController.OnEndingDialogueComplete += StartCredits;
    }

    private void OnDisable()
    {
        TextBoxController.OnEndingDialogueComplete -= StartCredits;
    }

    private void Awake()
    {
        _startY = -Screen.currentResolution.height;
        _endY = Screen.currentResolution.height;
    }

    private void Start()
    {
        _rt.anchoredPosition =new Vector2(_rt.anchoredPosition.x, _startY);
    }


    private void Update()
    {

        if (_creditsAreScrolling)
        {
            if (_rt.anchoredPosition.y < _endY)
            {
                _rt.anchoredPosition += new Vector2(0, _scrollSpeed);
            }
            else
            {
                OnCreditsOver?.Invoke(0);
                _creditsAreScrolling = false;
            }
        }
    }

    private void StartCredits()
    {
        _creditsAreScrolling = true;
        EventManager.TriggerEvent("sunset");
    }
}
