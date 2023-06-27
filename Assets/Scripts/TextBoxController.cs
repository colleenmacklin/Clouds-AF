using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Crosstales.RTVoice;
using System;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class TextBoxController : MonoBehaviour
{

    [SerializeField]
    TextMeshProUGUI textField;
    [SerializeField]
    TextMeshProUGUI textFieldBKGD;
    [SerializeField]
    String BKGDColor;

    //black
    //"<mark=#00000055 padding=\"30, 30, 10, 10\">"
    //PINK
    //"<mark = #FF00FF22 padding=30, 30, 10, 10>";

    //"<mark=#DD00DD padding=\"30, 100, 10, 20\">";


    [SerializeField]
    string[] linesList;

    [SerializeField]
    string activeLine;
    public Speaker speaker;
    public Crosstales.RTVoice.Model.Voice voice;
    public bool isPC = false;

    [SerializeField]
    string currentLine = "";
    int textLineIndex;

    [SerializeField]
    bool typeLines;

    [SerializeField]
    [Range(.5f, 5f)]
    float fadePause;

    [SerializeField]
    [Range(1f, 240f)]
    float charactersPerSecond;
    bool complete;
    int voiceState;
    public Storyteller teller;
    [SerializeField] List<bool> lineBools = new List<bool>();

    [SerializeField]
    AudioClip typeSound;
    AudioSource dialogueAudio;

    Coroutine typingCoroutine; //needed to stop the specific coroutine
    Coroutine fadeCoroutine; //needed to stop the specific coroutine

    float fadeSpeed = 1;
    public GameObject textCanvas;

    public bool PlayingEnding = false;

    public static event Action OnEndingDialogueComplete;
    public static event Action OnDialogueComplete;

    // Start is called before the first frame update
    void Start()
    {
        //TODO: INdieCade Change voices for each model

        switch (ModelInfo.ModelName)
        {
            case ("philosopher"):
                voice.Name = "Grandma";
                break;
            case ("comedian"):
                voice.Name = "Moira";
                //or 
                break;
            case ("primordial_earth"):
                voice.Name = "Tingting";
                //or Samantha
                break;
            default:
                voice.Name = "Grandma";
                break;
        }

        dialogueAudio = GetComponent<AudioSource>();
        dialogueAudio.clip = typeSound;
        for (int i = 0; i < teller.numberOfSentences; i++)
        {
            lineBools.Add(false);
        }
    }

    //Print out the string over time and play audio
    IEnumerator TypeString()
    {
        foreach (char character in activeLine.ToCharArray())
        {
            textField.text += character;

            dialogueAudio?.Play(); //play audio event

            yield return new WaitForSeconds(1f / charactersPerSecond);
            textFieldBKGD.text = BKGDColor + textField.text + "</mark>";

        }

    }
    IEnumerator fadeLineInOut()
    {
        //var material = textCanvas.GetComponent<Renderer>().material;
        var text = textCanvas.GetComponent<CanvasGroup>();
        textField.text = activeLine;
        textFieldBKGD.text = BKGDColor + textField.text + "</mark>";
        //set alpha to 0 first
        text.alpha = 0;

        //forever
        while (true)
        {
            //            dialogueAudio?.Play(); //play audio event


            // fade in
            yield return Fade(text, 1);
            // wait
            yield return new WaitForSeconds(fadePause);
            // fade out
            //yield return Fade(text, 0);
            // wait
            //yield return new WaitForSeconds(fadePause);
        }


    }

    IEnumerator Fade(CanvasGroup text, float targetAlpha)
    {
        while (text.alpha != targetAlpha)
        {
            var newAlpha = Mathf.MoveTowards(text.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
            text.alpha = newAlpha;
            //        mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, newAlpha);


            yield return null;
        }
    }

    public void VoiceComplete()
    {
        voiceState++;
        Debug.Log("voice complete");
    }

    //This function first checks if the line is done.
    //If not done, sets it to be done and ends the coroutine.
    //If it is, then it moves on.
    //This is the public function.
    public void Check()
    {

        if (complete) return;

        if (textField.text == activeLine && voiceState == 0)
        {
            Debug.Log("nextline called");
            NextLine();
            return;
        }

        //if (textField.text != activeLine)
        //{
        //    textField.text = activeLine;
        //    StopCoroutine(typingCoroutine);//stop the coroutine so it doesn't type on us.
        //    //StopAllCoroutines();
        //    return;
        //}

    }

    //Set activeline to the next line in the list
    //This is a behavior
    void NextLine()
    {
        //set complete to true if we increment to the last sentence
        if (textLineIndex < linesList.Length - 1)
        {
            textLineIndex++;//increment sentence
            //set the next active line
            activeLine = linesList[textLineIndex];

            if (isPC) {
                speaker.Speak(activeLine, null);
            }
            else
            {
                speaker.Speak(activeLine, null, voice);
            }

            
            //textField.text = "";
            //show line
            if (typeLines)
            {
                //clear the current text

                textField.text = "";

                //PINK Background
                textFieldBKGD.text = BKGDColor + textField.text + "</mark>";

                typingCoroutine = StartCoroutine(TypeString()); //begin typing
            }
            else
            {
                //Debug.Log("fading");

                fadeCoroutine = StartCoroutine(fadeLineInOut());//display the whole line (fadein)
            }

        }
        else
        {
            complete = true;
            Debug.Log("TEXTBoxCOntroller: voice complete is true");
            textField.text = ""; //clear text because it's the end
                                 //potentially trigger an event for ending the dialogue
            textFieldBKGD.text = BKGDColor + textField.text + "</mark>";

            EventManager.TriggerEvent("DoneReading");

            

            if (PlayingEnding)
            {
                OnEndingDialogueComplete?.Invoke();
            }
            else
            {
                OnDialogueComplete?.Invoke();
            }
        }
    }

    void Reset()
    {
        complete = false;
        textLineIndex = 0;
        textField.text = "";
        textFieldBKGD.text = BKGDColor + textField.text + "</mark>";
        activeLine = "";
        voiceState = -1;
        for (int i = 0; i < lineBools.Count; i++)
        {
            lineBools[i] = false;
        }
    }

    //Take list from any source and create array of lines
    //And read it out.
    public void ReadNewLines(string[] newLines)
    {
        Reset(); //reset first and then ingest lines
        CopyLines(newLines);
        activeLine = linesList[0]; //set active to first line
        Debug.Log("ReadNEwLines: "+ activeLine);
        if (isPC)
        {
            speaker.Speak(activeLine, null);
        }
        else
        {
            speaker.Speak(activeLine, null, voice);
        }


        if (typeLines)
        {
            typingCoroutine = StartCoroutine(TypeString()); //begin typing

        }
        else
        {
            //Debug.Log("fading");

            fadeCoroutine = StartCoroutine(fadeLineInOut());//display the whole line (fadein)
        }
    }

    //Set the lines array to the lines list
    //This is a separate function in case we want to change this behavior or use it elsewhere.
    void CopyLines(string[] newLines)
    {
        linesList = newLines;
    }

    void Update()
    {
        for (int i = 0; i < lineBools.Count; i++)
        {
            if(voiceState == i && !lineBools[i])
            {
                NextLine();
                lineBools[i] = true;
            }
        }
    }

}