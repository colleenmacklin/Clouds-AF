using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Crosstales.RTVoice;


[RequireComponent(typeof(AudioSource))]
public class TextBoxController : MonoBehaviour
{

    [SerializeField]
    TextMeshProUGUI textField;
    [SerializeField]
    string[] linesList;

    [SerializeField]
    string activeLine;
    public Speaker speaker;

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

    [SerializeField]
    AudioClip typeSound;
    AudioSource dialogueAudio;

    Coroutine typingCoroutine; //needed to stop the specific coroutine
    Coroutine fadeCoroutine; //needed to stop the specific coroutine

    float fadeSpeed = 1;
    public GameObject textCanvas;


    // Start is called before the first frame update
    void Start()
    {
        dialogueAudio = GetComponent<AudioSource>();
        dialogueAudio.clip = typeSound;
    }

    //Print out the string over time and play audio
    IEnumerator TypeString()
    {
        foreach (char character in activeLine.ToCharArray())
        {
            textField.text += character;

            dialogueAudio?.Play(); //play audio event

            yield return new WaitForSeconds(1f / charactersPerSecond);
        }

    }
    IEnumerator fadeLineInOut()
    {
        //var material = textCanvas.GetComponent<Renderer>().material;
        var text = textCanvas.GetComponent<CanvasGroup>();
        textField.text = activeLine;
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



    //This function first checks if the line is done.
    //If not done, sets it to be done and ends the coroutine.
    //If it is, then it moves on.
    //This is the public function.
    public void Check()
    {
        if (complete) return;

        if (textField.text == activeLine)
        {
            Debug.Log("nextline called");
            NextLine();
            return;
        }

        if (textField.text != activeLine)
        {
            textField.text = activeLine;
            StopCoroutine(typingCoroutine);//stop the coroutine so it doesn't type on us.
            //StopAllCoroutines();
            return;
        }

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
            speaker.Speak(activeLine);
            //textField.text = "";
            //show line
            if (typeLines)
            {
                //clear the current text

                textField.text = "";

                typingCoroutine = StartCoroutine(TypeString()); //begin typing
            }
            else
            {
                Debug.Log("fading");

                fadeCoroutine = StartCoroutine(fadeLineInOut());//display the whole line (fadein)
            }

        }
        else
        {
            complete = true;
            Debug.Log("complete is true");
            textField.text = ""; //clear text because it's the end
                                 //potentially trigger an event for ending the dialogue 
            EventManager.TriggerEvent("DoneReading");
        }
    }

    void Reset()
    {
        complete = false;
        textLineIndex = 0;
        textField.text = "";
        activeLine = "";
    }

    //Take list from any source and create array of lines
    //And read it out.
    public void ReadNewLines(string[] newLines)
    {
        Reset(); //reset first and then ingest lines
        CopyLines(newLines);
        activeLine = linesList[0]; //set active to first line
        speaker.Speak(activeLine);
        if (typeLines) {
            typingCoroutine = StartCoroutine(TypeString()); //begin typing
        }
        else
        {
            Debug.Log("fading");

            fadeCoroutine = StartCoroutine(fadeLineInOut());//display the whole line (fadein)
        }
    }

    //Set the lines array to the lines list
    //This is a separate function in case we want to change this behavior or use it elsewhere.
    void CopyLines(string[] newLines)
    {
        linesList = newLines;
    }

}
