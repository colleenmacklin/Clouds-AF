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
    [Range(0.001f, 1f)]
    float waitTime;
    bool complete;

    [SerializeField]
    AudioClip typeSound;
    AudioSource dialogueAudio;

    Coroutine typingCoroutine; //needed to stop the specific coroutine


    // Start is called before the first frame update
    void Start()
    {
        activeLine = "hello this is a line";
        dialogueAudio = GetComponent<AudioSource>();
        dialogueAudio.clip = typeSound;
        complete = false;
        textField.text = "";
        index = 0;
        typingCoroutine = StartCoroutine(TypeString());
    }



    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator TypeString()
    {
        foreach (char character in activeLine.ToCharArray())
        {
            textField.text += character;

            dialogueAudio.Play(); //play audio event

            yield return new WaitForSeconds(waitTime);
        }
    }
    //This function first checks if the line is done.
    //If not done, sets it to be done and ends the coroutine.
    //If it is, then it moves on.
    //This is the public facing function.
    [Button]
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
        }
    }

    //Take list from any source and create array of lines
    public void ReadNewLines(List<string> newLines)
    {
        linesList = newLines.ToArray();
    }


}
