
using UnityEngine;
using System.Collections;

public class FadeObjectInOut : MonoBehaviour
{

    // publically editable speed
    public float fadeDelay = 0.0f;
    public float fadeTime = 0.5f;
    public bool fadeInOnStart = false;
    public bool fadeOutOnStart = false;
    private bool logInitialFadeSequence = false;



    // store colours
    private Color[] colors;
    private Renderer[] rendererObjects;


    public bool IsFading;

    void OnEnable()
    {
        EventManager.StartListening("FadeIn", FadeIn);
        EventManager.StartListening("FadeOut", FadeOut);
        
    }

    void OnDisable()
    { 
        EventManager.StopListening("FadeIn", FadeIn);
        EventManager.StopListening("FadeOut", FadeOut);
    }
   
    // allow automatic fading on the start of the scene
    private void Start()
    {
        //yield return null;
        // grab all child objects
        rendererObjects = GetComponentsInChildren<Renderer>();
        if (colors == null)
        {
            //create a cache of colors if necessary
            colors = new Color[rendererObjects.Length];
            //Debug.Log("colors" + colors);

            // store the original colours for all child objects
            for (int i = 0; i < rendererObjects.Length; i++)
            {
                colors[i] = rendererObjects[i].material.color;
            }
        }

        if (fadeInOnStart)
        {
            logInitialFadeSequence = true;
            //rendererObjects = GetComponentsInChildren<Renderer>();
            for (int i = 0; i < rendererObjects.Length; i++)
            {
                Color tmp = rendererObjects[i].material.color;

                if (tmp.a > 0.0f)
                {
                    tmp.a = 0f;
                    rendererObjects[i].material.color = tmp;
                }

                //Debug.Log("renderer " + rendererObjects[i].material.color.a);
            }

            FadeIn();
        }

        if (fadeOutOnStart)
        {
            FadeOut(fadeTime);
        }
    }




    // check the alpha value of most opaque object
    float MaxAlpha()
    {
        float maxAlpha = 0.0f;
        Renderer[] rendererObjects = GetComponentsInChildren<Renderer>();
        foreach (Renderer item in rendererObjects)
        {
            maxAlpha = Mathf.Max(maxAlpha, item.material.color.a);
            //maxAlpha = Mathf.Max(maxAlpha, 1.0f);

        }
        return maxAlpha;
    }

    // fade sequence
    IEnumerator FadeSequence(float fadingOutTime)
    {
        IsFading = true;

        yield return new WaitForSeconds(fadeDelay);

        // log fading direction, then precalculate fading speed as a multiplier
        bool fadingOut = (fadingOutTime < 0.0f);
        float fadingOutSpeed = 1.0f / fadingOutTime;

        // grab all child objects
        /*
        Renderer[] rendererObjects = GetComponentsInChildren<Renderer>();
        if (colors == null)
        {
            //create a cache of colors if necessary
            colors = new Color[rendererObjects.Length];
            Debug.Log("colors" + colors);

            // store the original colours for all child objects
            for (int i = 0; i < rendererObjects.Length; i++)
            {
                colors[i] = rendererObjects[i].material.color;
            }
        }
        */
        // make all objects visible
        for (int i = 0; i < rendererObjects.Length; i++)
        {
            rendererObjects[i].enabled = true;
        }


        // get current max alpha
        float alphaValue = MaxAlpha();


        // This is a special case for objects that are set to fade in on start. 
        // it will treat them as alpha 0, despite them not being so. 
        if (logInitialFadeSequence && !fadingOut)
        {
            alphaValue = 0.0f;

            logInitialFadeSequence = false;
        }

        // iterate to change alpha value 
        while ((alphaValue >= 0.0f && fadingOut) || (alphaValue <= 1.0f && !fadingOut))
        {
            alphaValue += Time.deltaTime * fadingOutSpeed;

            for (int i = 0; i < rendererObjects.Length; i++)
            {
                Color newColor = (colors != null ? colors[i] : rendererObjects[i].material.color);
                newColor.a = Mathf.Min(newColor.a, alphaValue);
                newColor.a = Mathf.Clamp(newColor.a, 0.0f, 1.0f);
                rendererObjects[i].material.SetColor("_Color", newColor);
            }

            yield return null;
        }

        // turn objects off after fading out
        if (fadingOut)
        {
            for (int i = 0; i < rendererObjects.Length; i++)
            {
                rendererObjects[i].enabled = false;
            }
        }

        IsFading = false;
    }


    void FadeIn()
    {
        Debug.Log("fading in, time: "+ fadeTime + " delay: " + fadeDelay);
        FadeIn(fadeTime);
    }

    void FadeOut()
    {
        Debug.Log("fading out");
        FadeOut(fadeTime);
    }

    public void FadeIn(float newFadeTime)
    {
        StopAllCoroutines();
        StartCoroutine("FadeSequence", newFadeTime);
    }

    public void FadeOut(float newFadeTime)
    {
        StopAllCoroutines();
        StartCoroutine("FadeSequence", -newFadeTime);
    }

}
