using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Newtonsoft.Json;


/*
Narrator objects have stories, which are kept as dictionaries.
But we struct the story in case we want to do anything else with them, like serialize them

Consider making a custom UI element for the stories
*/

public struct Binding
{
    public string Name { get; private set; }
    public string[] Content { get; private set; }

    public Binding(string name, string[] content)
    {
        this.Name = name;
        this.Content = content;
    }
}

public struct Musing
{
    public string Name { get; private set; }
    public string[] Content { get; private set; }
    public Binding[] Bindings { get; private set; }

    public Musing(string name, string[] content, Binding[] bindings)
    {
        this.Name = name;
        this.Content = content;
        this.Bindings = bindings;
    }
}

public class BindingFormat
{
    public string name;
    public string dialog;
}

public class MusingFormat
{
    public string name; 
    public string dialog;
    public BindingFormat[] bindings;
}

[CreateAssetMenu(fileName = "CloudMusingSO", menuName = "Clouds-AF/Musing Object", order = 0)]
public class CloudMusingSO : ScriptableObject
{

    [Tooltip("File that has all the dialogue.")]
    public TextAsset storyFile;

    //public string temp;

    public List<Musing> CloudData = new List<Musing>();

    string[] dialogueOperator = { "==" };

    public bool ProcessNarratorFile()
    {

        //Debug.Log(storyData["stories"][1]);
        List<MusingFormat> data = JsonConvert.DeserializeObject<List<MusingFormat>>(storyFile.text);
        //Debug.Log(temp[3].bindings.Length);

        for (int i = 0; i < data.Count; i++)
        {
            //Dictionary<string, string[]> data = new Dictionary<string, string[]>();
            //string stripName = StripControlChars(storyData["stories"][i]["name"]);
            //string stripMuse = StripControlChars(storyData["stories"][i]["dialog"]);
            string[] dialog = data[i].dialog.Split(dialogueOperator, StringSplitOptions.None);
            List<Binding> BindingData = new List<Binding>();

            for (int j = 0; j < data[i].bindings.Length; j++)
            {
                string bindingName = data[i].bindings[j].name;
                string[] boundDialog = data[i].bindings[j].dialog.Split(dialogueOperator, StringSplitOptions.None);
                BindingData.Add(new Binding(bindingName, boundDialog));
            }

            //string jn = storyData["stories"][i];

            //string stripBindings = storyData["stories"][i]["bindings"];
            //Debug.Log(stripBindings);

            //Debug.Log(temp);

            //data.Add(stripName, dialog);
            CloudData.Add(new Musing(data[i].name, dialog, BindingData.ToArray()));
            //Debug.Log(stripName + "  " + dialog[1]);
        }

        return true;
    }


    public string StripControlChars(string s)
    {
        return Regex.Replace(s, @"[^\x20-\x7F]", "");
    }
}