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
    private Storyteller Teller;

    [SerializeField]
    [Tooltip("Pause between sending new lines")]
    [Range(0, 25)]
    public float conversational_pause = 12;

    [SerializeField] string selectedTarget;
    [SerializeField] private TextBoxController textBoxController;

    [SerializeField] private bool linesFinished;

    //////////////////////////////////////
    //
    //    Monobehaviors
    ///
    ////////////////////////////////

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
        // EventManager.StartListening("Talk", StartDialogue);
        // EventManager.StartListening("Introduction", ShowOpening);
        // EventManager.StartListening("Conclusion", ShowConclusion);
        EventManager.StartListening("Respond", Respond); //click events do this function??? where??
        EventManager.StartListening("DoneReading", ConversationalPauseTransition);

    }

    void OnDisable()
    {
        EventManager.StopListening("Respond", Respond);
        EventManager.StopListening("DoneReading", ConversationalPauseTransition);

    }


    //////////////////////
    //
    ///   Dialogue Handling
    //
    ///////////////////

    void EndGame()
    {
        linesFinished = true;
    }
    public void ShowConclusion()
    {
        EventManager.TriggerEvent("StopClouds"); //dissolves clouds
        EventManager.TriggerEvent("sunset"); //
    }

    void ReadSelection()
    {
        selectedTarget = GetComponent<Raycaster>()?.Selected.GetComponent<CloudShape>().CurrentShapeName;
    }
    //This now houses the primary logic for responses.
    // first check if the selection is valid, then update active sentence accordingly
    // then send the sentence to the text box.
    private void Respond()
    {
        if (linesFinished)
        {
            Debug.Log("Lines Finished, no response");
            EndGame();
        }
        ReadSelection();
        Teller.RespondToShape(selectedTarget);
    }

    void ConversationalPauseTransition()
    {
        Debug.Log("---ending dialogue, conversational pause---");

        StartCoroutine(TransitionToNextCloud());
    }
    IEnumerator TransitionToNextCloud()
    {
        yield return new WaitForSeconds(conversational_pause);

        EndConversation(); //activate the event for ending the convo
    }

    void EndConversation()
    {
        EventManager.TriggerEvent("ConversationEnded"); //this is moved to the text box controller
    }



}