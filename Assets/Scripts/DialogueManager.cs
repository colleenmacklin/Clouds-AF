using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/*
The Dialogue Manager should take the chosen object from the CloudManager
and then choose that as the active dialogue. It will then move through that dialogue list
as one presses the space bar (or calls the function). This will change the text that is inside
the dialogueText reference. 

4/27/21 This is now responsible for three things: handling choices, sending dialogue, and advancing choices
Perhaps this should be reconsidered.
*/

[RequireComponent(typeof(Raycaster), typeof(Storyteller))]
public class DialogueManager : MonoBehaviour
{

    [SerializeField]
    [Tooltip("The storyteller we will use")]
    private Storyteller Teller;

    [SerializeField]
    [Tooltip("Pause between sending new lines")]
    [Range(0, 25)]
    public float conversational_pause = 12;

    [SerializeField]
    [Tooltip("The raycaster's target")]
    string selectedTarget;
    [SerializeField]
    [Tooltip("UI text Controller")]
    private TextBoxController textBoxController;

    [SerializeField]
    [Tooltip("If Lines are completed. This is handled in script")]
    private bool linesFinished;

    //////////////////////////////////////
    //
    //    Monobehaviors
    //
    ////////////////////////////////

    private void Update() { }

    void OnEnable()
    {
        EventManager.StartListening("sunset", EndGame);
        EventManager.StartListening("Respond", Respond);
        EventManager.StartListening("DoneReading", ConversationalPauseTransition);
    }

    void OnDisable()
    {
        EventManager.StopListening("sunset", EndGame);
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
        EventManager.TriggerEvent("sunset");
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
            return;
        }
        ReadSelection();
        //Debug.Log(selectedTarget);
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

        //YIKES LINE IS IN HERE
        if (!linesFinished)
            EndConversation(); //activate the event for ending the convo
        else
        {
            EventManager.TriggerEvent("EndingConclusion");
            EventManager.TriggerEvent("");
        }
    }

    void EndConversation()
    {
        EventManager.TriggerEvent("ConversationEnded"); //this is moved to the text box controller
    }

}