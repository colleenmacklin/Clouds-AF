using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using SimpleJSON;


/*
Narrator objects have stories, which are kept as dictionaries.
But we struct the story in case we want to do anything else with them, like serialize them

Consider making a custom UI element for the stories
*/

public struct Story
{
    public string Name { get; private set; }
    public Dictionary<string, string[]> Content { get; private set; }

    public Story(string name, Dictionary<string, string[]> content)
    {
        this.Name = name;
        this.Content = content;
    }
}

[CreateAssetMenu(fileName = "NarratorSO", menuName = "Clouds-AF/Narrator Object", order = 0)]
public class NarratorSO : ScriptableObject
{

    [Tooltip("File that has all the dialogue.")]
    public TextAsset narratorFile;
    public TextAsset storyFile;

    public JSONNode storyData;

    [Tooltip("Column delimiter, can be anything, default is |")]
    public string ColumnDelimiter = "|";

    [Tooltip("Dialogue delimiter, can be anything, default is ==")]
    public string DialogueDelimiter = "==";

    public List<Story> StoryData = new List<Story>();
    public List<string> keys = new List<string>();

    public bool ProcessNarratorFile(TextAsset dataFile = null)
    {

        if (dataFile == null)
        {
            dataFile = narratorFile;
            storyData = JSON.Parse(storyFile.text);
        }

        //Debug.Log(storyData["stories"]);
        for (int i = 0; i < 32; i++)
        {
            string newString = storyData["stories"][i]["name"].ToString();
            //int index = newString.Length;
            //newString = newString.Remove(0, 1);
            //newString = newString.Remove(index - 2, 1);
            //Debug.Log(newString.Length);
            keys.Add(newString);
            //Debug.Log(keys[i].Length);
        }

        string[] keyArray = keys.ToArray();

        //Debug.Log(keyArray[21]);


        //create split operators
        string[] columnOperator = { ColumnDelimiter };
        string[] dialogueOperator = { DialogueDelimiter };

        //split new lines
        string[] records = dataFile.text.Split('\n');
        //Debug.Log(records.Length);

        //first line is in format: 
        //nothing, Story 1, Story 2, Story 3
        //Get all the first titles
        string[] storyTitles = records[0].Split(columnOperator, StringSplitOptions.None);
        //keys.ToArray();
        //Debug.Log(storyTitles[5]);

        //create list of all data
        //where 0 is story 1, 1 is story 2, 2 is story 3
        //this is a vertical list of all the data, where indices are off by 1
        List < Dictionary<string, string[]>> data = new List<Dictionary<string, string[]>>();
        for (int i = 1; i < records.Length; i++)
        {
            //second line is in format:
            //key, story 1 text, story 2 text, story 3 text
            string[] keyedText = records[i].Split(columnOperator, StringSplitOptions.None);//break line on |
            //Debug.Log(keyedText[0]);

            //now add dictionary entries for every story
            //started from j = 1 because the 0 position is just the key
            for (int j = 1; j < keyedText.Length; j++)
            {
                string key = keyedText[0];
                string[] dialogue = keyedText[j].Split(dialogueOperator, StringSplitOptions.None);
                //Debug.Log(dialogue[0]);
                //add string keys per entry
                //if dictionary is not present, create dictionary
                //over time, assemble the list of data which is all those dictionaries
                if (data.Count < j)
                {
                    data.Add(new Dictionary<string, string[]>());
                }

                //j is shuffled around because of the offset of counting from 1
                data[j - 1].Add(key, dialogue);
            }

        }
        //Add all stories into the list of stories with keys
        for (int i = 1; i < storyTitles.Length; i++)
        {
            //Debug.Log(storyTitles[i]+ data[i - 1]);
            //string newString = storyTitles[i];
            //int index = newString.Length;
            //newString = newString.Remove(0, 1);
            //newString = newString.Remove(index - 2, 1);
            //Debug.Log(newString + data[i - 1]);
            StoryData.Add(new Story(storyTitles[i], data[i - 1]));
        }

        return true;
    }


    public string StripControlChars(string s)
    {
        return Regex.Replace(s, @"[^\x20-\x7F]", "");
    }
}