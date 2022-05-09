using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Crosstales.RTVoice;

/*

The setup for this Storyteller is the following.

Pick a story out of the options, that becomes the active story.
Send the opening to the dialogue manager, which handles how to display the dialogue
And it will fire off the event for Opening ("setup").

Once the opening is done, then this will fire off the event for starting the gameplay
This will generate the clouds shapes with the MusingEnd general purpose event ("MusingEnded")

During that time, the storyteller will await input from the Raycaster, which sends the chosen 
element to the storyteller which fires the "Correct" event if correct (it always will be in this version)
Only some shapes are not.

The dialogue should be sent over to the manager in the same manner as the opening.
Then release the event with "MusingEnded" **note that here we should have scriptable object'd our events

After some number of loops of this happening, trigger the ending. *needs to be ported*

Major changes from Dialogue Manager:
* No selection validation
* More control over flow sequence

*/

[RequireComponent((typeof(DialogueManager)))]
public class Storyteller : MonoBehaviour
{
    //[SerializeField]
    //NarratorSO narrator;
    public Speaker speaker;
    [SerializeField]
    CloudMusingSO muse;
    [SerializeField]
    public RectTransform credits;
    private bool gameover = false;
    public List<string> names = new List<string>();
    [Tooltip("Chooses a random story if on, otherwise picks first one")]
    [SerializeField] bool ChooseRandomStory = false;

    [SerializeField] int numberOfMusings = 3; //total musings for our story
    [SerializeField] int musingsGiven = 0; //how many musings we've done
    [SerializeField] TextBoxController textBoxController;
    [SerializeField] List<string> viewedShapes = new List<string>();//the shapes we've viewed so far


    //Story chosenStory;//the active story we will use
    private void OnEnable()
    {
        EventManager.StartListening("ConversationEnded", CheckForCompletion);
        //    EventManager.StopListening("Conclusion", ShowConclusion);        
    }
    private void OnDisable()
    {

        EventManager.StopListening("ConversationEnded", CheckForCompletion);
        //    EventManager.StartListening("Conclusion", ShowConclusion);    
    }



    // Start is called before the first frame update
    void Start()
    {
        //speaker.SpeakNative("RT Voice is speaking");

        //.ProcessNarratorFile();
        muse.ProcessNarratorFile();
        //Debug.Log(muse.CloudData[5].Bindings[0].Content[1]);
        EventManager.TriggerEvent("Cutscene");
        //EventManager.TriggerEvent("Intro");
        SetupStory();
        EventManager.TriggerEvent("Setup"); //tell clouds to get ready
                                            //access pattern for the narrator story content
                                            //internally the data is kept in a Story structure that has a Name and Content (dictionary of string to string[])
                                            //        Debug.Log(narrator.StoryData[0].Name);
                                            //Debug.Log(narrator.StoryData[0].Content["entertainer"][0]);

    }

    //Pick one of the narrator's random stories
    void SetupStory()
    {
        //int storyIndex = 0;
        //if (ChooseRandomStory)
        //{
        //    storyIndex = Random.Range(0, narrator.StoryData.Count);
        //}

        //chosenStory = narrator.StoryData[storyIndex]; //picked story

        //Send the story opening to the dialogue manager
        SendMusing(muse.CloudData[0].Content);

        //string speakText = "Hi how are you";
        //speaker.Speak(speakText);
        Debug.Log(speaker.VoiceForCulture("en"));
    }

    //Send a musing to the text controller
    void SendMusing(string[] musing)
    {
        textBoxController.ReadNewLines(musing);
        //for (int i = 0; i < musing.Length; i++)
        //{
        //    speaker.Speak(musing[i]);
        //}
    }

    void NextMusing(string key)
    {
        //store the shape
        //Debug.Log(key);
        viewedShapes.Add(key);
        //send the musing
        for (int i = 0; i < muse.CloudData.Count; i++)
        {

            if (muse.CloudData[i].Name == key)
            {
                string[] currentMusing = muse.CloudData[i].Content;
                for (int j = 0; j < viewedShapes.Count; j++)
                {
                    //Debug.Log(viewedShapes[j]);
                    for (int k = 0; k < muse.CloudData[i].Bindings.Length; k++)
                    {
                        Debug.Log(muse.CloudData[i].Bindings[k].Name);
                        if(viewedShapes[j] == muse.CloudData[i].Bindings[k].Name)
                        {
                            currentMusing = muse.CloudData[i].Bindings[k].Content;
                        }
                    }
                }

                SendMusing(currentMusing);
            }
        }
        //increase musings
        musingsGiven += 1;
        EventManager.TriggerEvent("Musing");

        EventManager.TriggerEvent("Correct");

    }

    //Respond to a selection from the raycaster, send string[] if found, nothing if not.
    public void RespondToShape(string shapeKey)
    {
        Debug.Log($"Responding to {shapeKey}");

        //if (chosenStory.Content.ContainsKey(shapeKey))
        //{
        NextMusing(shapeKey);
        //    return;
        //}

        //if not found then send... nothing?
        //SendMusing(new string[] { $"I don't think I have anything for that." });//graceful failure
    }

    //Ending Event
    void CheckForCompletion()
    {
        if (musingsGiven == numberOfMusings)
        {
            EventManager.TriggerEvent("Musing");
            EventManager.TriggerEvent("Cutscene");
            EventManager.TriggerEvent("sunset");
            EndStory();
            gameover = true;
            //Credits();
        }
    }

    //create the ending story
    void EndStory()
    {
        //first create the list
        //"A, B, and C"
        //We would likely replace this with a different, combination key in the future.
        string chosenList = "";
        for (int i = 0; i < viewedShapes.Count; i++)
        {
            string shapeName = viewedShapes[i];
            shapeName = shapeName.Replace("_", " ");
            if (i == 0)
            {
                chosenList = shapeName;
            }
            else if (i == viewedShapes.Count - 1)
            {
                chosenList += $", and {shapeName}";
            }
            else
            {
                chosenList += $", {shapeName}";
            }
        }

        //Adjust the lines in the original by replacing <CLOUD_LIST> with our list.
        List<string> adjustedLines = new List<string>();
        foreach (string line in muse.CloudData[31].Content)
        {
            string adjustedLine = line.Replace("<CLOUD_LIST>", chosenList);

            adjustedLines.Add(adjustedLine);
        }

        //create the list of chosen items.
        SendMusing(adjustedLines.ToArray());
    }

    void Credits()
    {
        if(credits.position.y < 1000.0f)
        {
            credits.position += new Vector3(0, 2.0f, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if(gameover)
        //{
        //    Credits();
        //}
    }
}
