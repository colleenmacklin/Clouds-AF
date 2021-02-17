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
    private Text dialogueText; //reference to text field in Friend


    [SerializeField]
    public string conversationTarget;

    [SerializeField]
    public string selectedTarget;

    [SerializeField]
    [Range(3, 25)]
    public float conversational_pause = 12;

    [SerializeField]
    private int currentLine;


    [SerializeField]
    private List<string> activeLines = new List<string>();

    [SerializeField]
    private string activeSentence;
    [SerializeField]
    private bool linesFinished;

    //////////////////////////////////////
    //
    //    Monobehaviors
    ///
    ////////////////////////////////

    void Start()
    {
        dialogueSO.ReadDialogueFromFile();
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
        EventManager.StartListening("Respond", Respond); //click events do this function??? where??
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
        Debug.Log("Start dialogue beginning");
        var myTarget = GameObject.FindWithTag("CloudManager").GetComponent<Game_CloudManager>().chosenShape;
        var targetName = myTarget.name;

        conversationTarget = targetName;

        activeLines = dialogueSO.DialogueByKey(targetName);
        ResetCurrentLine(); //set currentLine to zero
        activeSentence = activeLines[currentLine]; //set the active sentence to this. which we will send to the text
        linesFinished = false;

        StartCoroutine(UpdateTextWithSentence(conversational_pause));
    }

    void ResetCurrentLine()
    {
        currentLine = 0;
    }

    //This now houses the primary logic for responses.
    // first check if the selection is valid, then update active sentence accordingly
    // then send the sentence to the text box.
    private void Respond()
    {
        ReadSelection();
        if (ValidateSelection(selectedTarget))
        {
            activeSentence = ActivateNextSentence();
        }
        else
        {
            activeSentence = WrongAnswer();
        }

        StartCoroutine(UpdateTextWithSentence(1));
    }

    void ReadSelection()
    {
        selectedTarget = GetComponent<Raycaster>()?.Selected.GetComponent<GameCloudLayerGroup>().curr_Shape.name;
    }

    bool ValidateSelection(string selection)
    {
        return conversationTarget == selection;
    }
    string WrongAnswer()
    {
        return "No... not that cloud";
    }

    //this is its own function because this is actually handling the mutation. which we encapsulate
    string ActivateNextSentence()
    {
        if (currentLine == activeLines.Count - 1)
        {
            //we are out of lines
            linesFinished = true;
            Debug.Log("---ending dialogue---");
            EndConversation();
            return "That was interesting.";
        }
        currentLine += 1;
        return activeLines[currentLine];
    }

    void EndConversation()
    {
        EventManager.TriggerEvent("ConversationEnded");
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