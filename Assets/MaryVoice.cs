using Crosstales.RTVoice;
using Crosstales.RTVoice.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaryVoice : MonoBehaviour
{
    public Speaker speaker;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log(speaker.Voices.Count);
            Voice voice = speaker.Voices[5];
            speaker.Speak("this is me waaa wooo woo bladeee rap lyricccs", null, voice);
        }
    }
}
