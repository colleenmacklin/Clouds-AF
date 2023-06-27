using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using Crosstales.RTVoice;
using SimpleJSON;
using MiniJSON;
//using static UnityEditor.Rendering.CameraUI;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using UnityEngine.Windows;
using Crosstales.RTVoice.Model;
using System;
using Random = UnityEngine.Random;
using static UnityEngine.UIElements.UxmlAttributeDescription;

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
* 
* current model/name dict:
*     Dictionary<String, String> model_urls = new Dictionary<String, String>()
    {
        {"philosopher", "https://api-inference.huggingface.co/models/Triangles/fantastic_philosopher_124_4000"},
        {"comedian", "https://api-inference.huggingface.co/models/Triangles/comedian_4000_gpt_only"},
        {"primordial_earth", "https://api-inference.huggingface.co/models/Triangles/primordial_earth_gpt_content_only"}
    };


*/

[RequireComponent((typeof(DialogueManager)))]
public class Storyteller : MonoBehaviour
{
    //[SerializeField]
    //NarratorSO narrator;
    //public Speaker speaker;
    public GameState GameState; //3 states: Intro, GameLoop, Ending
    public event Action OnIntroComplete;
    [SerializeField]
    CloudMusingSO muse;

    [SerializeField]
    public TextAsset storyFile;

    [SerializeField]
    public RectTransform credits;
    private bool gameover = false;
    public List<string> names = new List<string>();
    [Tooltip("Chooses a random story if on, otherwise picks first one")]
    [SerializeField] bool ChooseRandomStory = false;

    [SerializeField] int numberOfMusings = 3; //total musings for our story
    [SerializeField] int musingsGiven = 0; //how many musings we've done
    public int numberOfSentences = 3;
    [SerializeField] List<string> prompts = new List<string>();
    [SerializeField] float bindingProbablility = 0.3f;
    [SerializeField] List<string> bindingPrompts = new List<string>();
    [SerializeField] TextBoxController textBoxController;
    [SerializeField] List<string> viewedShapes = new List<string>();//the shapes we've viewed so far

    [Header("Model Name")]
    public string model_name;

    [Header("HuggingFace Model URL")]
    public string model_url;

    [Header("HuggingFace Key API")]
    public string hf_api_key;

    [SerializeField]
    private GlowButterfly _butterfly;

    [Header("model storyfile")]
    public TextAsset story_file;

    int timer = 20;


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
        model_url = ModelInfo.modelURL;
        hf_api_key = ModelInfo.hf_api_key;
        model_name = ModelInfo.ModelName;


        /*
        switch (model_name)
        {
            case "philosopher":

                muse.storyFile = storyFile;
                break;

            case "comedian":
                muse.storyFile = null;
                break;

            case "sentient_earth":
                muse.storyFile = null;
                break;
        }
        story_file = ModelInfo.storyFile;
        */
        //check to see that the modelURL was passed on from the opening, and if so, assign public vars
        if (string.IsNullOrEmpty(ModelInfo.modelURL))
        {
            Debug.Log("No model URL, defaulting to Philosopher");
            model_name = "philosopher";
            model_url = "https://api-inference.huggingface.co/models/Triangles/fantastic_philosopher_124_4000"; //default to philosopher
            //hf_api_key = "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";
            hf_api_key = "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";
        }

        string filename = "Text/" + model_name + "_storiesbound";
        Debug.Log("filename: " + filename);

        //Load texture from disk
        //TextAsset model_storyFile = Resources.Load(filename) as TextAsset;

        TextAsset model_storyFile = Resources.Load<TextAsset>(filename);
        Debug.Log("storyFile: " + model_storyFile.name);
        storyFile = model_storyFile;

        muse.storyFile = model_storyFile;

        Debug.Log("muse.CloudData Count: " + muse.CloudData.Count);

        //speaker.SpeakNative("RT Voice is speaking");
        prompts.Add("That cloud reminds me of __");
        prompts.Add("Clouds often take the form of __. I think this is because");
        prompts.Add("I see __, which makes me wonder why");
        prompts.Add("That cloud reminds me of __, a");
        prompts.Add("A cloud shaped like __ symbolizes");

        //TODO INDIECADE: check the formatting (__ and --)
        bindingPrompts.Add("I was thinking about __ and --’s relationship, and");
        bindingPrompts.Add("Do you know why __ and --");
        bindingPrompts.Add("Do you think we saw __-shaped cloud and ---shaped cloud because");
        bindingPrompts.Add("Did you know that __ and -- in the same day predicts");
        bindingPrompts.Add("I’ve never seen __ with --, but now I see they are connected by");

        //.ProcessNarratorFile();
        muse.ProcessNarratorFile();
        //Debug.Log(muse.CloudData[5].Bindings[0].Content[1]);
        EventManager.TriggerEvent("Cutscene");
        //EventManager.TriggerEvent("Intro");
        SetupStory();
        EventManager.TriggerEvent("Setup"); //tell clouds to get ready
                                            //access pattern for the narrator story content
                                            //internally the data is kept in a Story structure that has a Name and Content (dictionary of string to string[])
                                            //Debug.Log(narrator.StoryData[0].Name);
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

        //TODO INDIECADE: make openings random.
        SendMusing(muse.CloudData[0].Content);
        GameState.Intro = true;

        //string speakText = "Hi how are you";
        //speaker.Speak(speakText);
        //Debug.Log(speaker.VoiceForCulture("en"));
    }

    //Send a musing to the text controller
    void SendMusing(string[] musing)
    {
        Debug.Log("sendMusing: "+musing[0]);
//TODO: INdieCADE Build - fix problem when musing is empty - check to see if it is null, and if so, say something else and release camera lock
//
        textBoxController.ReadNewLines(musing);
        //for (int i = 0; i < musing.Length; i++)
        //{
        //    speaker.Speak(musing[i]);
        //}
    }

    void NextMusing(string key)
    {

        string keyString = key.Replace("_", " ");
        string fullPrompt = " ";

        if (viewedShapes.Count < 1)
        {
            int promptIndex = Random.Range(0, prompts.Count);
            //string prompt = "That cloud reminds me of a ";
            string prompt = prompts[promptIndex];

            fullPrompt = prompt.Replace("__", keyString);
        }
        else
        {
            float bindingChance = Random.Range(0, 1f);
            if (bindingChance > bindingProbablility)
            {
                int promptIndex = Random.Range(0, prompts.Count);
                //string prompt = "That cloud reminds me of a ";
                string prompt = prompts[promptIndex];

                fullPrompt = prompt.Replace("__", keyString);
            }
            else
            {

                int promptIndex = Random.Range(0, bindingPrompts.Count);
                int viewedShapeIndex = Random.Range(0, viewedShapes.Count - 1);
                string viewedShapeString = viewedShapes[viewedShapeIndex].Replace("_", " ");
                string prompt = bindingPrompts[promptIndex];

                fullPrompt = prompt.Replace("__", keyString);
                fullPrompt = fullPrompt.Replace("--", viewedShapeString);
            }
        }

        //store the shape
        //Debug.Log(key);
        viewedShapes.Add(key);
        

        timer = 20;
        StartCoroutine(LiveMusing(fullPrompt, key));

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
            // EventManager.TriggerEvent("sunset"); moved this to credits
            EndStory();
            gameover = true;
            GameState.Ending = true;
            GameState.Gameloop = false;

            //Credits();
        }
        if (GameState.Intro)
        {
            Debug.Log("introDone");
            EventManager.TriggerEvent("IntroDone");
            GameState.Intro = false;
            GameState.Gameloop = true;

            //TODO TERRY ENTER BUTTERFLY
            OnIntroComplete?.Invoke();
        }

    }

    //create the ending story
    void EndStory()
    {
        Debug.Log("EndStory() called on storyteller");
        //first create the list
        //"A, B, and C"
        //We would likely replace this with a different, combination key in the future.
        string chosenList = "";
        for (int i = 0; i < viewedShapes.Count; i++)
        {
            string shapeName = viewedShapes[i];
            names.Add(viewedShapes[i]);

            Debug.Log("ending shapes (from storyteller viewedShapes[];" + shapeName);

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
        Actions.SetEndingClouds(names); //listened to by CloudManager
        //Adjust the lines in the original by replacing <CLOUD_LIST> with our list.
        List<string> adjustedLines = new List<string>();

        //TODO IndieCade: fix integer in CloudData to conform to correct one for model - this is hardcoded to philosopher.

        Debug.Log("muse.CloudData: " + muse.CloudData);
        //int index = muse.CloudData.IndexOf(Musing.Name);

        //WARNING: HACK
        int dataIndex;

        switch (model_name)
        {
            case "philosopher":
                dataIndex = 31;
                break;
            case "comedian":
                dataIndex = 21;
                break;
            case "primordial_earth":
                dataIndex = 20;
                break;
            default:
                dataIndex = 31; //philosopher
                break;
        }



        foreach (string line in muse.CloudData[dataIndex].Content)
        {
            string adjustedLine = line.Replace("<CLOUD_LIST>", chosenList);
            adjustedLines.Add(adjustedLine);
        }

        //create the list of chosen items.
        SendMusing(adjustedLines.ToArray());
        textBoxController.PlayingEnding = true;

        _butterfly.DestroyButterfly();
    }

    //This is where the webrequests are made - but it's buggy. Sometimes the connection fails, and when that happens, the game gets stuck.
    //need to release the camera when this gets stuck. --Colleen
    //
    public IEnumerator LiveMusing(string prompt, string key)
    {
        // Form the JSON
        var form = new Dictionary<string, object>();
        //var attributes = new Dictionary<string, object>();
        //attributes["source_sentence"] = prompt;
        //attributes["sentences"] = sentences;
        form["n"] = 1; //the number of generated texts
        form["inputs"] = prompt;
        form["num_return_sequences"] = 3; //set the number of sentences to be returned - I don;t think this actually works!
        form["wait_for_model"] = false; //(Default: false) Boolean. If the model is not ready, wait for it instead of receiving 503. It limits the number of requests required to get your inference done. It is advised to only set this flag to true after receiving a 503 error as it will limit hanging in your application to known places.
        form["temperature"] = 0.7;
        form["top_k"] = 50;
        form["top_p"] = 0.95;
        form["do_sample"] = true;
        form["min_length"] = 20;
        form["max_length"] = 100; //TODO: check to see if all of these parameters actually do anything!


        var json = Json.Serialize(form);
        Debug.Log("JSON" + json);
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);

        // Make the web request
        UnityWebRequest request = UnityWebRequest.Put(model_url, bytes);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + hf_api_key);
        request.method = "POST"; // Hack to send POST to server instead of PUT

        yield return request.SendWebRequest();

        Debug.Log("live musing attempt result: "+request.result);
        // If the request return an error set the error on console. - we should set parameters on the api to request again  TODO INDIECADE: Debug this -- freezes when thrown


        //if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        if (request.result != UnityWebRequest.Result.Success) //CM added to check for all errors and "inProgress" hangs
        {
            Debug.Log("failed request error: " + request.error);
            Debug.Log("failed downloadHandler.data: " + request.downloadHandler.text);
            //TODO: when a 503 error is thrown (model is not ready) set "wait_for_model" = true; remake the request
            JSONNode data = request.downloadHandler.text;

            //on error code here
            //pull a pre-rendered musing from the text file
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
                            Debug.Log("Binding: "+muse.CloudData[i].Bindings[k].Name);
                            if (viewedShapes[j] == muse.CloudData[i].Bindings[k].Name)
                            {
                                currentMusing = muse.CloudData[i].Bindings[k].Content;
                            }
                        }
                    }

                    SendMusing(currentMusing);
                }
            }

        }
        else
        {
            Debug.Log("Success!");
            JSONNode data = request.downloadHandler.text;

         
            // Process the result
            //Debug.Log(ProcessResult(data, prompt));

            string[] currentMusing = ProcessResult(data, prompt);
            SendMusing(currentMusing);
            Debug.Log("CurrentMusing: "+ currentMusing[0]);

            //generativeStory.Add(ProcessResult(data)); //may want 

        }

        request.Dispose(); //Colleen added to manage a memory leak. See documentation here: https://answers.unity.com/questions/1904005/a-native-collection-has-not-been-disposed-resultin-1.html

    }


    string[] ProcessResult(string result, string prompt)
    {
        // The data is a score for each possible sentence candidate
        // But, it looks something like this "[0.7777, 0.19, 0.01]"
        // First, we need to remove [ and ]
        string cleanedResult = result.Replace("{", "");
        cleanedResult = cleanedResult.Replace(":", "");
        cleanedResult = cleanedResult.Replace("generated_text", "");
        cleanedResult = cleanedResult.Replace("}", "");
        cleanedResult = cleanedResult.Replace("[", "");
        cleanedResult = cleanedResult.Replace("]", "");
        cleanedResult = cleanedResult.Replace("\"", "");
        cleanedResult = cleanedResult.Replace("\' ", " ");
        string paragraph = cleanedResult;

        Debug.Log(paragraph);

        // Split the paragraph into sentences
        string[] sentences = paragraph.Split("\\n");

        //Debug.Log(sentences.Length);

        // Create a new list to store the sentences
        List<string> sentenceList = new List<string>();

        // Iterate through the sentences and add each one to the list
        //foreach (string sentence in sentences)
        //{
        //    sentenceList.Add(sentence);
        //}
        for (int i = 0; i < numberOfSentences; i++) //TODO: Returns an out of index error --cm
        {
            sentenceList.Add(sentences[i]);
        }

        return ChooseSentences(sentenceList, prompt).ToArray();

        //sentenceList.RemoveAt(sentenceList.Count - 1);
        //Debug.Log("sentence 1: " + sentenceList[0] + " sentence2: " + sentenceList[1]);
        //int s_num = num_sentences;
        //while (s_num)
        //return sentenceList.ToArray();
    }

    List<string> ChooseSentences(List<string> sentenceList, string prompt)
    {
        if (sentenceList.Count <= 1) return sentenceList;

        List<string> tempSentenceList = new List<string>();

        if (sentenceList.Count > 1)
        {
            if (sentenceList[0].Length > prompt.Length + 50)
            {
                if (sentenceList[0].Length > prompt.Length + 150)
                {
                    tempSentenceList.Add(sentenceList[1]);
                    tempSentenceList.Add(sentenceList[2]);
                }
                else
                {
                    tempSentenceList.Add(sentenceList[0]);
                }
                
            }
            else
            {
                if (sentenceList[1].Length > 100)
                {
                    tempSentenceList.Add(sentenceList[0]);
                    tempSentenceList.Add(sentenceList[1]);
                }
                else
                {
                    if (sentenceList[2].Length < 100)
                    {
                        tempSentenceList.Add(sentenceList[0]);
                        tempSentenceList.Add(sentenceList[1]);
                        tempSentenceList.Add(sentenceList[2]);
                    }
                    else
                    {
                        tempSentenceList.Add(sentenceList[0]);
                        tempSentenceList.Add(sentenceList[1]);
                    }
                }
            }
        }

        //lookAhead
        string[] lookAhead = { "teapot" }; //put words we don't want to show up from old dialogue here

        for (int i = 0; i < tempSentenceList.Count; i++)
        {
            foreach (string look in lookAhead)
            {
                if (tempSentenceList[i].Contains(look))
                {
                    tempSentenceList.RemoveAt(i);
                }
            }
        }

        return tempSentenceList;
    }


    public int GetNumberOfMusings()
    {
        return numberOfMusings;
    }

    public int GetMusingsGiven()
    {
        return musingsGiven;
    }
   
}