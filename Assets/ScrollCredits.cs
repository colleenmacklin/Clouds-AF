using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollCredits : MonoBehaviour
{
    [SerializeField]
    private RectTransform _rt;

    private Vector2 _pos;
   
    private float _startY;

    [SerializeField]
    private float _scrollSpeed;

    [SerializeField]
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

        _pos = _rt.anchoredPosition;
        _startY = -_rt.sizeDelta.y;
    }

    private void Start()
    {
        _pos = new Vector2(_pos.x, _startY);
        _rt.anchoredPosition = _pos;
    }


    private void Update()
    {
        //just for now for testing
        if (Input.GetKeyDown(KeyCode.G))
        {
            StartCredits();
        }

        //add a delay??

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
    }
}
