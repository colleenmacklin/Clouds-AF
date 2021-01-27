using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    public CloudDialogue dialogueSO;
    public GameObject cloudManager; //need to look in here and see what the current active cloud and shape spritename is
    public GameObject textBox;
    public GameObject space; //gonna remove this
    public Text dialogueText; //reference to text field in Friend
    public string myName;
    public string currentShape;
    public float conversational_pause = 12;
    public TextAsset textFile; // this file has a hashtag for the name of the sprite, and text after... maybe need to create a multidimensional array?
    public string[] textLines; //all the text seperated by lines marked by a return (\n)
    public int myIndex; //line number of the cloudshape in the text file (indicated by "#")
    public int currentLine;
    public int endAtLine;


    public Animator animator;

    public List<string> activeLines = new List<string>();

    //////////////////////////////////////
    //
    //    Monobehaviors
    ///
    ////////////////////////////////

    void Awake()
    {
        StartCoroutine(EndDialogue(0)); //hide the dialogue
        disableSpaceButton(space);
    }

    void Start()
    {
        //Tell the SO to read the text and create its lists.
        dialogueSO.ReadDialogueFromFile();
       // StartDialogue();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            //Debug.Log("-----space-----" + space.activeSelf);
            //if (space.activeSelf == true) //turn this into a graphic that shows up 
            
                EventManager.TriggerEvent("Continue");
            
        }
        
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


    //////////////////////
    //
    ///   Dialogue Handling
    //
    ///////////////////

    public void StartDialogue()
    {
        Debug.Log("Start dialogue starting");
        //this is an interesting problem.
        //we start dialogue with shape selection..?
        var myTarget = GameObject.FindWithTag("CloudManager").GetComponent<Game_CloudManager>().chosenShape;
        var targetName = myTarget.name;
        myName = targetName;
        activeLines = dialogueSO.DialogueByKey(targetName);
        myIndex = 0; // reset dialogue index
        currentLine = myIndex;
        StartCoroutine(DisplayQuestion(conversational_pause)); //hide the dialogue
    }

    public void DisplayNextSentence()
    {

        if (currentLine < activeLines.Count) { 
        currentLine += 1;
        }
        
        if (currentLine + 1 == activeLines.Count-1){
            StopAllCoroutines();
            StartCoroutine(ShowFinalSentence(activeLines[currentLine]));
            currentLine+=1;
        }

        if (currentLine == activeLines.Count-1){
            disableSpaceButton(space);
            StartCoroutine(EndDialogue(7));
            return;
        }

        StartCoroutine(ShowSentence(activeLines[currentLine]));

    }

    public void Respond()
    {
        
        animator.SetBool("isOpen", true);

        if (myName == currentShape)
        {
            DisplayNextSentence();
            EventManager.TriggerEvent("FoundCloud");
            return;
        }

    }

    /////////////////////
    //
    // Coroutines For Dialogue
    //
    //////////////////////////
    IEnumerator DisplayQuestion(float time)
    {
        yield return new WaitForSeconds(time);
        animator.SetBool("isOpen", true);
        disableSpaceButton(space);
        StopAllCoroutines();
        StartCoroutine(ShowQuestion(activeLines[currentLine]));
        currentLine += 1;

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
        disableSpaceButton(space);//only difference from Show Sentence
    }
    IEnumerator ShowQuestion(string sentence)
    {
        yield return new WaitForSeconds(1);
        dialogueText.text = sentence;
        animator.SetBool("isOpen", true);
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

    ////////////////////////
    //
    // Space Button functions... not used?
    //
    /////////////////////////////////

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


}