using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using EasyButtons;


// Potential data structure for every object's dialogues
public struct DialogueSubject
{
    public string Name { get; private set; }
    public string Prompt { get; private set; }
    public Dictionary<int, string[]> LevelDialogues { get; private set; }

    public DialogueSubject(string name, string prompt, Dictionary<int, string[]> levelDialogues)
    {
        this.Name = name;
        this.Prompt = prompt;
        this.LevelDialogues = levelDialogues;
    }

    public string[] DialogueOptionsAtLevel(int level)
    {
        return this.LevelDialogues[level];
    }
}

[CreateAssetMenu(fileName = "CloudDialogue", menuName = "Clouds-AF/CloudDialogue", order = 0)]
public class CloudDialogue : ScriptableObject
{

    [SerializeField]
    public TextAsset dialogueFile; //ingest all dialogue

    //List of dialogue options within the object
    public Dictionary<string, List<string>> dialogues = new Dictionary<string, List<string>>();

    public Dictionary<string, DialogueSubject> objectDialogues = new Dictionary<string, DialogueSubject>();

    public Dictionary<string, DialogueSubject> ReadDialogueFromCSV(TextAsset textFile = null)
    {
        //Declare new result dictionary
        var result = new Dictionary<string, DialogueSubject>();

        if (textFile == null)
        {
            textFile = dialogueFile;
        }

        // split across new lines
        string[] records = textFile.text.Split('\n');

        //skip the first line
        records = records.Skip(1).ToArray();


        //For every line
        foreach (string record in records)
        {
            List<string> spokenLines = new List<string>();

            //currently assuming this pattern
            // name|prompt|text1==text1.2==text1.3|text2|text3
            string[] fields = record.Split('|');

            string name = fields[0];
            string prompt = fields[1];
            Dictionary<int, string[]> dialogueDictionary = new Dictionary<int, string[]>();

            string[] splitOperator = { "==" };
            int level = 1; //we use level 1 as a starting point
            //then for the remaining fields add all lines broken up by == markers
            for (int i = 2; i < fields.Length; i++)
            {
                string[] linesInLevel = fields[i].Split(splitOperator, StringSplitOptions.None);//splitting by string needs the full function
                foreach (string line in linesInLevel)
                {
                    spokenLines.Add(line);
                }
                string[] spokenLinesArray = spokenLines.ToArray(); //convert lines to array
                dialogueDictionary.Add(level, spokenLinesArray); //add lines and level to dictionary

                level++; //increase level
            }

            var subject = new DialogueSubject(name, prompt, dialogueDictionary);

            result.Add(name, subject);
        }
        objectDialogues = result;
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


