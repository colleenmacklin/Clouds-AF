using UnityEngine;
using System.Collections;


public class ScreenshotUtility : MonoBehaviour
{
    [Tooltip("The filepath to the save location for the screenshots")]
    public string _filepath;
    [Tooltip("Scales the image by the amount specified (0 for no scaling)")]
    public int _screenshotResolutionScaleFactor = 0;
    public string current_shape;
    [Space]

    public KeyCode _toggleAutoScreenshots = KeyCode.K;
    public float _timeBetweenAutoScreenshots = 3.0f;

    [Space]

    public KeyCode _pressForSingleScreenshot = KeyCode.L;

    private int _screenshotNumber;
    private bool _autoScreenshotOn;
    void OnEnable()
    {
        EventManager.StartListening("SpawnShape", SpawnShape);
    }

    void OnDisable()
    {
        EventManager.StopListening("SpawnShape", SpawnShape);
    }

    void Start()
    {
        _screenshotNumber = 0;
        _autoScreenshotOn = false;
        StartCoroutine(AutomaticScreenshot());
        //access current texture name
        current_shape = GameObject.Find("Game_Cloud").GetComponent<Opening_Cloud_Layer>().curr_Shape.name;
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

    private void SpawnShape()
    {
        //just makes sure to find the new name for the texture
        current_shape = GameObject.Find("Game_Cloud").GetComponent<Opening_Cloud_Layer>().curr_Shape.name;
    }

    //For reference http://docs.unity3d.com/ScriptReference/Application.CaptureScreenshot.html
    private void TakeScreenshot()
    {
        ScreenCapture.CaptureScreenshot(_filepath + current_shape + _screenshotNumber++.ToString() + ".png", _screenshotResolutionScaleFactor);
    }
}