using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    public AudioSource dialogueAudio;


    // Start is called before the first frame update
    void Start()
    {
        activeLine = "hello this is a line";
        dialogueAudio = GetComponent<AudioSource>();
        textField.text = "";
        StartCoroutine(TypeString());
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
    //Set activeline to the next line in the list
    //This is a behavior
    void NextLine()
    {
        //fallout early if we are complete
        if (complete)
        {
            return;
        }

        index++;//increment sentence
        //set complete to true if we increment to the last sentence
        if (index == linesList.Length - 1)
        {
            complete = true;
        }
        //set the next active line
        activeLine = linesList[index];
        //clear the current text
        textField.text = "";
        //start typing
        StartCoroutine(TypeString());
    }

    //Take list from any source and create array of lines
    public void ReadNewLines(List<string> newLines)
    {
        linesList = newLines.ToArray();
    }


}
