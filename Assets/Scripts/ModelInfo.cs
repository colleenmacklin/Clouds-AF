using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelInfo
{
    // Saves selected model information for usage across scenes
    public static string ModelName { get; set; }
    public static string modelURL { get; set; }
    public static string hf_api_key { get; set; }
    public static TextAsset storyFile { get; set; } //the pre-rendered text for each model

}
