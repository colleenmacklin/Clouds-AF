using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using EasyButtons;

[CreateAssetMenu(fileName = "CloudDialogue", menuName = "Clouds-AF/CloudDialogue", order = 0)]
public class CloudDialogue : ScriptableObject
{

    [SerializeField]
    public TextAsset dialogueFile; //ingest all dialogue

    //List of dialogue options within the object
    public Dictionary<string, List<string>> dialogues = new Dictionary<string, List<string>>();

    public Dictionary<string, List<string>> ReadDialogueFromCSV(TextAsset textFile = null)
    {
        var result = NewDialogueDictionary();

        if (textFile == null)
        {
            textFile = dialogueFile;
        }

        string[] records = textFile.text.Split('\n');
        //skip the first line
        records = records.Skip(1).ToArray();
        foreach (string record in records)
        {
            List<string> spokenLines = new List<string>();

            string[] fields = record.Split(',');

            //the second field is the prompt line -- need to adjust this
            spokenLines.Add(fields[1]);

            //the third field, index 2, is the possible lines
            string[] lines = fields[2].Split('|');

            foreach (string line in lines)
            {
                spokenLines.Add(line);
            }

            var key = fields[0];
            var dialogue = spokenLines;
            result.Add(key, dialogue);
        }
        dialogues = result;
        return result;
    }

    //Reads through a text file that follows the #Target dialogue form
    public Dictionary<string, List<string>> ReadDialogueFromFile(TextAsset textFile = null)
    {
        dialogues = NewDialogueDictionary(); //mutation

        if (textFile == null)
        {
            textFile = dialogueFile; //use local file if undefined
        }

        //go through file, breaking it up by # first and then by \n
        string[] allLines = textFile.text.Split('#');

        string[] targetLines;
        foreach (string lineGroup in allLines)
        {
            //first line is the target, the rest are the lines
            targetLines = lineGroup.Split('\n');

            //get the key (target)
            string key = StripControlChars(targetLines[0]);

            //skip if key is empty or null
            if (key == "" || key == null) continue;

            //container for the lines
            List<string> targetDialogues = new List<string>();
            int numOfLines = targetLines.Length - 1;
            for (int i = 1; i < numOfLines; i++)
            {//adds lines in order of writing!
                var line = targetLines[i];
                //skip empty lines
                if (line == null || line == "") continue;

                targetDialogues.Add(line);
            }
            //if we're missing lines
            if (targetDialogues.Count == 0)
            {
                targetDialogues.Add("No Lines Detected");
            }

            //add key and dialogue pair to the dialogues dictionary
            Debug.Log($"adding {key} with {targetDialogues.Count} lines.");
            dialogues.Add(key, targetDialogues);
        }


        return dialogues;
    }

    private Dictionary<string, List<string>> NewDialogueDictionary()
    {
        return new Dictionary<string, List<string>>();
    }

    //This function takes any target and returns a list of dialogue
    //The manager can then take the rest of the dialogue to process
    //Nulls will have to be handled by the manager, not the dialogue.
    public List<string> DialogueByKey(string key)
    {

        if (dialogues.ContainsKey(key))
        {
            return dialogues[key];
        }

        return null; // get nothing back if the key is not there 

    }

    //replaces pesky control characters from the beginning of strings.
    //from https://stackoverflow.com/questions/5303272/remove-all-invisible-chars-from-a-string
    static public string StripControlChars(string s)
    {
        return Regex.Replace(s, @"[^\x20-\x7F]", "");
    }
}


