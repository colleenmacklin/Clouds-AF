using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EasyButtons;

[RequireComponent(typeof(AudioSource))]
public class TextBoxController : MonoBehaviour
{

    [SerializeField]
    TextMeshProUGUI textField;
    [SerializeField]
    string[] linesList;

    [SerializeField]
    string activeLine;

    [SerializeField]
    string currentLine = "";
    int index;


    [SerializeField]
    [Range(1f, 240f)]
    float charactersPerSecond;
    bool complete;

    [SerializeField]
    AudioClip typeSound;
    AudioSource dialogueAudio;

    Coroutine typingCoroutine; //needed to stop the specific coroutine


    // Start is called before the first frame update
    void Start()
    {
        dialogueAudio = GetComponent<AudioSource>();
        dialogueAudio.clip = typeSound;
        Reset();
        typingCoroutine = StartCoroutine(TypeString());
    }

    //Print out the string over time and play audio
    IEnumerator TypeString()
    {
        foreach (char character in activeLine.ToCharArray())
        {
            textField.text += character;

            dialogueAudio.Play(); //play audio event

            yield return new WaitForSeconds(1f / charactersPerSecond);
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
            NextLine();
            return;
        }

        if (textField.text != activeLine)
        {
            textField.text = activeLine;
            StopCoroutine(typingCoroutine);//stop the coroutine so it doesn't type on us.
            return;
        }



    }

    //Set activeline to the next line in the list
    //This is a behavior
    void NextLine()
    {
        //set complete to true if we increment to the last sentence
        if (index < linesList.Length - 1)
        {
            index++;//increment sentence
            //set the next active line
            activeLine = linesList[index];
            //clear the current text
            textField.text = "";
            //start typing
            typingCoroutine = StartCoroutine(TypeString());
        }
        else
        {
            complete = true;
            textField.text = ""; //clear text because it's the end
                                 //potentially trigger an event for ending the dialogue 
            EventManager.TriggerEvent("DoneReading");
        }
    }

    void Reset()
    {
        complete = false;
        index = 0;
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
        typingCoroutine = StartCoroutine(TypeString()); //begin typing
    }

    //Set the lines array to the lines list
    //This is a separate function in case we want to change this behavior or use it elsewhere.
    void CopyLines(string[] newLines)
    {
        linesList = newLines;
    }

}
