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
    public GameObject continueButton; //gonna remove this
    public Text dialogueText; //reference to text field in Friend
    public string myName;
    public string currentShape;
    public Text theText;
    public int myIndex;

    public TextAsset textFile; // this file has a hashtag for the name of the sprite, and text after... maybe need to create a multidimensional array?
    public string[] textLines; //all the text seperated by lines marked by a return (\n)
    public int currentLine;
    public int endAtLine;

    public Animator animator;

    void Awake()
    {
        //maybe put textfile in here?
        StartCoroutine(EndDialogue(0)); //hide the dialogue
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
        //return index + 1;
        return index;

    }


    public void StartDialogue()
    {
        var myTarget = GameObject.FindWithTag("CloudManager").GetComponent<CloudManager>().chosenShape;
        var targetName = myTarget.name;
        myName = targetName;

        myIndex = searchForName(targetName);
        currentLine = myIndex;
        //enableButton(continueButton);

        //DisplayNextSentence();
        StartCoroutine(DisplayQuestion(12)); //hide the dialogue

    }

    IEnumerator DisplayQuestion(float time)
    {
        yield return new WaitForSeconds(time);
        animator.SetBool("isOpen", true);
        currentLine += 1;
        disableButton(continueButton);
        StopAllCoroutines();
        StartCoroutine(ShowSentence(textLines[currentLine]));
        //StartCoroutine(TypeSentence(textLines[currentLine]));
    }

    public void DisplayNextSentence()
    {

        if (currentLine <= textLines.Length) { 
        currentLine += 1;
            //enableButton(continueButton);
        }
        //Debug.Log("currentLine: " + currentLine);


        if (textLines[currentLine + 1].Contains("#"))
        {
            Debug.Log("disabling continue button " + textLines[currentLine]);
            //enableButton(continueButton);
            disableButton(continueButton);
            StopAllCoroutines();
            //type it out
            //StartCoroutine(TypeSentence(textLines[currentLine]));
            StartCoroutine(ShowSentence(textLines[currentLine]));
            currentLine += 1;
           
            //return;
        }
        //else
        //{
            //enableButton(continueButton);
        //}


        if (currentLine == endAtLine || textLines[currentLine].Contains("#"))
        {
            //Debug.Log("there are NO more lines of text left, lastLine: "+currentLine);
            disableButton(continueButton);
            StartCoroutine(EndDialogue(7));

            //EndDialogue();
            return;
        }
        //else {
        //enableButton(continueButton);
        //}

        enableButton(continueButton);


        //Debug.Log("there ARE more lines of text left:" + +currentLine +" " + textLines[currentLine]);


        //animation to type out sentences
        //StopAllCoroutines();
        StartCoroutine(ShowSentence(textLines[currentLine]));

        //StartCoroutine(TypeSentence(textLines[currentLine]));

        //currentLine += 1;
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
    public void enableButton(GameObject button)
    {
        continueButton.SetActive(true);
    }

    // function to disable continueButton
    public void disableButton(GameObject button)
    {
        continueButton.SetActive(false);
    }


    private void Update()
    {
        
        if (Input.GetButtonDown("Continue"))
        {
            //Debug.Log("-----continue-----" + continueButton.activeSelf);

            if (continueButton.activeSelf == true)
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