using UnityEngine;
using System.Collections;


public class ScreenshotUtility : MonoBehaviour
{
    [Tooltip("The filepath to the save location for the screenshots")]
    public string _filepath;
    [Tooltip("Scales the image by the amount specified (0 for no scaling)")]
    public int _screenshotResolutionScaleFactor = 0;

    [Space]

    public KeyCode _toggleAutoScreenshots = KeyCode.K;
    public float _timeBetweenAutoScreenshots = 3.0f;

    [Space]

    public KeyCode _pressForSingleScreenshot = KeyCode.L;

    private int _screenshotNumber;
    private bool _autoScreenshotOn;

    void Start()
    {
        _screenshotNumber = 0;
        _autoScreenshotOn = false;
        StartCoroutine(AutomaticScreenshot());
    }

    void Update()
    {
        if (Input.GetKeyDown(_toggleAutoScreenshots))
        {
            _autoScreenshotOn = !_autoScreenshotOn;
        }
        if (Input.GetKeyDown(_pressForSingleScreenshot))
        {
            TakeScreenshot();
        }
    }

    IEnumerator AutomaticScreenshot()
    {
        while (true)
        {
            if (_autoScreenshotOn)
            {
                TakeScreenshot();
                yield return new WaitForSeconds(_timeBetweenAutoScreenshots);
            }
            yield return new WaitForFixedUpdate();
        }
    }

    //For reference http://docs.unity3d.com/ScriptReference/Application.CaptureScreenshot.html
    private void TakeScreenshot()
    {
        ScreenCapture.CaptureScreenshot(_filepath + _screenshotNumber++.ToString() + ".png", _screenshotResolutionScaleFactor);
    }
}