using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    public GameObject cloudManager; //need to look in here and see what the current active cloud and shape spritename is
    public GameObject textBox;
    public GameObject space; //gonna remove this
    public Text dialogueText; //reference to text field in Friend
    public string myName;
    public string currentShape;
    public Text theText;
    public int myIndex; //line number of the cloudshape in the text file (indicated by "#")
    public float conversational_pause = 12;
    public TextAsset textFile; // this file has a hashtag for the name of the sprite, and text after... maybe need to create a multidimensional array?
    public string[] textLines; //all the text seperated by lines marked by a return (\n)
    public int currentLine;
    public int endAtLine;

    public Animator animator;

    void Awake()
    {
        //maybe put textfile in here?
        StartCoroutine(EndDialogue(0)); //hide the dialogue
        disableSpaceButton(space);
    }

    void OnEnable()
    {
        EventManager.StartListening("Talk", StartDialogue);
        EventManager.StartListening("Respond", Respond);
        EventManager.StartListening("Continue", DisplayNextSentence);
    }

    void OnDisable()
    {
        EventManager.StopListening("Talk", StartDialogue);
        EventManager.StopListening("Respond", Respond);
        EventManager.StopListening("Continue", DisplayNextSentence);
    }

    void Start()
    {
        if (textFile != null)
        {
            textLines = (textFile.text.Split('\n'));
        }
            endAtLine = textLines.Length - 1;
    }

    public int searchForName(string n) //find the dialogue associated with the cloud shape called from event manager "talk"
    {

    int index = 0;
    for (; index<textLines.Length; index++) {
      if (textLines[index].Contains("#" + n))
      {
         var tempString = textLines[index].Remove(0, 1);
          currentShape = tempString;
          break;
      }
  }
    Debug.Log("here is the droid you were looking for: "+index.ToString ());
        return index;
    }


    public void StartDialogue()
    {
        var myTarget = GameObject.FindWithTag("CloudManager").GetComponent<Game_CloudManager>().chosenShape;
        var targetName = myTarget.name;
        myName = targetName;
        myIndex = searchForName(targetName);
        currentLine = myIndex;
        StartCoroutine(DisplayQuestion(conversational_pause)); //hide the dialogue
    }

    IEnumerator DisplayQuestion(float time)
    {
        yield return new WaitForSeconds(time);
        animator.SetBool("isOpen", true);
        currentLine += 1;
        disableSpaceButton(space);
        StopAllCoroutines();
        StartCoroutine(ShowQuestion(textLines[currentLine]));

        //StartCoroutine(TypeSentence(textLines[currentLine]));
    }

    public void DisplayNextSentence()
    {

        if (currentLine <= textLines.Length) { 
        currentLine += 1;
        }

        if (textLines[currentLine + 1].Contains("#") || currentLine + 1 == endAtLine) //if there's no more lines of dialogue left
        {
            StopAllCoroutines();
            //type it out
            //StartCoroutine(TypeSentence(textLines[currentLine]));
            StartCoroutine(ShowFinalSentence(textLines[currentLine]));
            currentLine += 1;
        }
        else
        {
            //enableSpaceButton(space);
        }


        if (currentLine == endAtLine || textLines[currentLine].Contains("#"))
        {
            //Debug.Log("there are NO more lines of text left, lastLine: "+currentLine);
            disableSpaceButton(space);
            StartCoroutine(EndDialogue(7));

            //EndDialogue();
            return;
        }


        //animation to type out sentences
        //StopAllCoroutines();
        StartCoroutine(ShowSentence(textLines[currentLine]));

        //StartCoroutine(TypeSentence(textLines[currentLine]));

        //currentLine += 1;
        //enableSpaceButton(space);

    }

    public void Respond()
    {
        //Debug.Log("currentShape: " + currentShape + "  " + "myName: " + myName);
        animator.SetBool("isOpen", true);

        if (myName == currentShape)
        {
            DisplayNextSentence();
            EventManager.TriggerEvent("FoundCloud");
            return;
        }

    }

    //-------------------------------------------------maybe get rid of this
    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        //foreach (char letter in sentence.ToCharArray())
        foreach (char letter in textLines[currentLine].ToCharArray())

        {
            dialogueText.text += letter;
            yield return null;
        }
        //currentLine += 1;
    }

    IEnumerator ShowSentence(string sentence)
    {
        yield return new WaitForSeconds(1);
        dialogueText.text = sentence;
        animator.SetBool("isOpen", true);
        enableSpaceButton(space);
    }

    IEnumerator ShowFinalSentence(string sentence)
    {
        yield return new WaitForSeconds(1);
        dialogueText.text = sentence;
        animator.SetBool("isOpen", true);
        disableSpaceButton(space);
    }


    IEnumerator ShowQuestion(string sentence)
    {
        yield return new WaitForSeconds(1);
        dialogueText.text = sentence;
        animator.SetBool("isOpen", true);
    }

    /*
    void EndDialogue()
    {

        Debug.Log("---ending dialogue---");
        //Start the coroutine we define below named ExampleCoroutine.

        animator.SetBool("isOpen", false);
        disableButton(continueButton);

    }
    */

    // function to enable continueButton
    public void enableSpaceButton(GameObject space)
    {
        EventManager.TriggerEvent("FadeIn");
        space.SetActive(true);
    }

    // function to disable continueButton
    public void disableSpaceButton(GameObject space)
    {
        EventManager.TriggerEvent("FadeOut");
        space.SetActive(false);
    }
    

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            //Debug.Log("-----space-----" + space.activeSelf);
            if (space.activeSelf == true) //turn this into a graphic that shows up 
            {
                EventManager.TriggerEvent("Continue");
            }
        }
        
        /*
        if (currentLine < endAtLine+1)
        {
            theText.text = textLines[currentLine];

        }
        else
        {
            textBox.SetActive(false);

        }
        */
    }

    IEnumerator EndDialogue(float time)
    {
        //Print the time of when the function is first called.
        //Debug.Log("Started Coroutine at timestamp : " + Time.time);
        Debug.Log("---ending dialogue---");

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(time);
        //Start the coroutine we define below named ExampleCoroutine.

        animator.SetBool("isOpen", false);
        //disableButton(continueButton);

        //After we have waited 5 seconds print the time again.
        //Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }


}