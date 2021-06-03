using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

/*
The Dialogue Manager should take the chosen object from the CloudManager
and then choose that as the active dialogue. It will then move through that dialogue list
as one presses the space bar (or calls the function). This will change the text that is inside
the dialogueText reference. 

4/27/21 This is now responsible for three things: handling choices, sending dialogue, and advancing choices
Perhaps this should be reconsidered.
*/

[RequireComponent(typeof(Raycaster))]
public class DialogueManager : MonoBehaviour
{
    [SerializeField]
    private CloudDialogue dialogueSO;

    [SerializeField]
    public GameObject cloudManager; //need to look in here and see what the current active cloud and shape spritename is

    [SerializeField]
    public GameObject textBox;

    [SerializeField]
    public GameObject space; //gonna remove this

    //[SerializeField]
    //public Animator textBoxAnimator;

    [SerializeField]
    private TextMeshProUGUI dialogueText; //reference to text field in Friend

    [SerializeField]
    public string conversationTarget;

    [SerializeField]
    public string selectedTarget;

    [SerializeField]
    [Range(0, 25)]
    public float conversational_pause = 12;

    [SerializeField]
    private int currentLine;

    [SerializeField]
    private DialogueSubject subjectMatter;

    [SerializeField]
    private TextBoxController textBoxController;

    [SerializeField]
    private string[] activeLines;

    [SerializeField]
    private string activeSentence;
    [SerializeField]
    private bool linesFinished;

    //////////////////////////////////////
    //
    //    Monobehaviors
    ///
    ////////////////////////////////

    void Awake()
    {
        dialogueSO.ReadDialogueFromCSV();//look into a way to make this happen absolutely first
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
        EventManager.StartListening("Introduction", ShowOpening);
        EventManager.StartListening("Conclusion", ShowConclusion);
        EventManager.StartListening("Respond", Respond); //click events do this function??? where??
        EventManager.StartListening("DoneReading", ConversationalPauseTransition);
    }

    void OnDisable()
    {
        EventManager.StopListening("Talk", StartDialogue);
        EventManager.StopListening("Introduction", ShowOpening);
        EventManager.StopListening("Conclusion", ShowConclusion);
        EventManager.StopListening("Respond", Respond);
        EventManager.StopListening("DoneReading", ConversationalPauseTransition);
    }


    //////////////////////
    //
    ///   Dialogue Handling
    //
    ///////////////////

    private void CutscenePlay(string cutsceneName)
    {
        var startingDialogue = dialogueSO.DialogueSubjectByKey(cutsceneName); //startingdialogue is the wrong name for this
        textBoxController.ReadNewLines(startingDialogue.Prompt);
        EventManager.TriggerEvent("Cutscene");
    }
    public void ShowOpening()
    {
        string cutsceneName = "act_start";
        CutscenePlay(cutsceneName);
        //we might need to use timelines or something for better timing of events in the opening and closing but for now this works
        EventManager.TriggerEvent("SlowDown"); //tells the cloud particle system to slow down to normal speed(speeds up in the beginning to enable fully-formed look at start

    }

    public void ShowConclusion()
    {
        string cutsceneName = "act_end";
        CutscenePlay(cutsceneName);
        EventManager.TriggerEvent("StopClouds"); //dissolves clouds
        EventManager.TriggerEvent("sunset"); //
    }

    //Handle everything about actually getting the Dialogue set up with targets and selections
    public void StartDialogue()
    {
        Debug.Log("Start dialogue beginning");
        var myTarget = GameObject.FindWithTag("CloudManager").GetComponent<CloudManager>().chosenShape;
        var targetName = myTarget.name;

        conversationTarget = targetName;

        subjectMatter = dialogueSO.DialogueSubjectByKey(targetName);
        activeLines = subjectMatter.DialogueOptionsAtLevel(1);
        ResetCurrentLine(); //set currentLine to -1 to account for prompt
        //set the active line to the prompt
        //activeSentence = subjectMatter.Prompt;
        linesFinished = false;

        //textBoxController.ReadNewLines(new[] { activeSentence });
        textBoxController.ReadNewLines(subjectMatter.Prompt);

    }

    void ResetCurrentLine()
    {
        currentLine = -1;
    }

    //This now houses the primary logic for responses.
    // first check if the selection is valid, then update active sentence accordingly
    // then send the sentence to the text box.
    private void Respond()
    {
        if (linesFinished) return;

        ReadSelection();
        if (SelectionIsValid(selectedTarget))
        {
            //activeSentence = ActivateNextSentence();
            textBoxController.ReadNewLines(activeLines);
            EventManager.TriggerEvent("Correct");
            //EventManager.TriggerEvent("FadeInSpace"); //fades in the space indicator

        }
        else
        {
            activeSentence = WrongAnswer();
        }

        //StartCoroutine(UpdateTextWithSentence(1));

        //if we are at the last option, we should move on without a click
        // if (currentLine == activeLines.Length - 1)
        // {
        //     linesFinished = true;
        //     StartCoroutine(TransitionToNextCloud()); //transition the lines
        //     // EventManager.TriggerEvent("FadeOutSpace"); //fades out the space indicator
        // }
    }


    /*
        Woof, ok. This is a lot of external logic being implemented
        So we checked for the ending in the response (because we check if we're at the 
        last line).
        If we are, then we say the linesFinished is true;
        then we start this transition. 

        It waits for 3 seconds before saying I wonder what other clouds we find.
        Then it updates with that,
        then waits another 3 before triggering the EndConversation.
        ->which immediately sets the StartDialogue.

        The issue is that the EndConversation event in the cloud is *immediately* tied
        to the StartDialogue event. This is not something we should trivially move around
        So it is something we should have a bigger conversation about.

        That is - we need to talk about what our event structure and our game sequence is

    */

    void ConversationalPauseTransition()
    {
        Debug.Log("---ending dialogue, conversational pause---");

        StartCoroutine(TransitionToNextCloud());
    }
    IEnumerator TransitionToNextCloud()
    {
        activeSentence = "";
        yield return new WaitForSeconds(conversational_pause);

        EndConversation(); //activate the event for ending the convo
    }

    void ReadSelection()
    {
        //be careful about script component: it has to match the cloud.
        selectedTarget = GetComponent<Raycaster>()?.Selected.GetComponent<CloudShape>().CurrentShapeName;
    }

    bool SelectionIsValid(string selection)
    {
        return conversationTarget == selection;
    }
    string WrongAnswer()
    {
        return ""; //used to be: "No... not that cloud"
    }

    //this is its own function because this is actually handling the mutation. which we encapsulate
    string ActivateNextSentence()
    {

        currentLine += 1;
        return activeLines[currentLine];
    }

    void EndConversation()
    {
        EventManager.TriggerEvent("ConversationEnded"); //this is moved to the text box controller
    }

    /////////////////////
    //
    // Coroutine For Updating TextBox
    //
    //////////////////////////

    IEnumerator UpdateTextWithSentence(float time)
    {
        //nothing has changed so we break out
        if (activeSentence == dialogueText.text) yield break;

        Debug.Log("Sentence Changed!");

        //execute if sentence is different
        yield return new WaitForSeconds(time);

        dialogueText.text = activeSentence;

    }

}