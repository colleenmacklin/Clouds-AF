using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

/*
The Dialogue Manager should take the chosen object from the Game_CloudManager
and then choose that as the active dialogue. It will then move through that dialogue list
as one presses the space bar (or calls the function). This will change the text that is inside
the dialogueText reference. 
*/
public class DialogueManager : MonoBehaviour
{
    public CloudDialogue dialogueSO;
    public GameObject cloudManager; //need to look in here and see what the current active cloud and shape spritename is
    public GameObject textBox;
    public GameObject space; //gonna remove this
    public Animator textBoxAnimator;
    public Text dialogueText; //reference to text field in Friend
    public string conversationTarget;
    public string selectedTarget;
    public float conversational_pause = 12;
    public int currentLine;


    public List<string> activeLines = new List<string>();
    public string activeSentence;
    public bool linesFinished;

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
        //This is literally just an activation.
        dialogueSO.ReadDialogueFromFile();
       // StartDialogue();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Debug.Log("-----space-----");
            EventManager.TriggerEvent("Respond");//in case something else is hooked to this event
        }
    }
    
    void OnEnable()
    {
        EventManager.StartListening("Talk", StartDialogue);
        EventManager.StartListening("Respond", Respond);
    }

    void OnDisable()
    {
        EventManager.StopListening("Talk", StartDialogue);
        EventManager.StopListening("Respond", Respond);
    }


    //////////////////////
    //
    ///   Dialogue Handling
    //
    ///////////////////

    //Handle everything about actually getting the Dialogue set up with targets and selections
    public void StartDialogue()
    {
        Debug.Log("Start dialogue starting");
        var myTarget = GameObject.FindWithTag("CloudManager").GetComponent<Game_CloudManager>().chosenShape;
        var targetName = myTarget.name;
        targetName = "No One"; //for testing
        conversationTarget = targetName;
        activeLines = dialogueSO.DialogueByKey(targetName);
        ResetCurrentLine();//set currentLine to zero
        activeSentence = activeLines[currentLine]; //set the active sentence to this. which we will send to the text
        linesFinished = false;
        StopAllCoroutines();
        StartCoroutine(UpdateTextWithSentence(conversational_pause));
    }

    public void Respond () {
        SendResponse (selectedTarget);
        StartCoroutine(UpdateTextWithSentence(1));
    }

    //Read whatever the selectedTarget is and compare against the conversationTarget
    //Advance if correct, ignore if not, or send wrong text 
    public void SendResponse ( string selection ) {
        //do not use this long term, instead we should take the selection as if it were sent from an input interface
        selectedTarget = selection;

        if ( conversationTarget == selectedTarget ){
            ActivateNextSentence();
        }
        else{
            //Handling incorrect responses
            activeSentence = "No... not that cloud";
        }
    }

    //this is its own function because this is actually handling the mutation. which we encapsulate
    private void ActivateNextSentence(){
        if ( currentLine == activeLines.Count - 1 ) {
            //we are out of lines, do nothing
            activeSentence = "We're out of ammo";
            linesFinished = true;
            Debug.Log("---ending dialogue---");
            return;
        }
        currentLine += 1;
        activeSentence = activeLines[currentLine];
    }


    /////////////////////
    //
    // Coroutines For Dialogue
    //
    //////////////////////////

    IEnumerator UpdateTextWithSentence( float time ){
        //nothing has changed so we break out
        if ( activeSentence == dialogueText.text ) yield break;

        Debug.Log("Sentence Changed!");

        //execute if sentence is different
        yield return new WaitForSeconds(time);
        textBoxAnimator.SetBool("isOpen", true);
        dialogueText.text = activeSentence;
        
    }
 
    IEnumerator EndDialogue(float time)
    {
        //Print the time of when the function is first called.
        //Debug.Log("Started Coroutine at timestamp : " + Time.time);
        Debug.Log("---ending dialogue---");

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(time);
        //Start the coroutine we define below named ExampleCoroutine.

        textBoxAnimator.SetBool("isOpen", false);
        //disableButton(continueButton);

        
        ResetCurrentLine();
        //After we have waited 5 seconds print the time again.
        //Debug.Log("Finished Coroutine at timestamp : " + Time.time);
    }

    void ResetCurrentLine(){
        currentLine = 0;
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