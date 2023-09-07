using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Actions
{
    // Contains all the actions for our game!!

    public static Action SharpenCloud;
    public static Action BlurCloud;
    public static Action<GameObject> GetClickedCloud;
    public static Action<CloudShape> CloudIsReady;
    public static Action<OpeningCloudShape> OpeningCloudIsReady;
    public static Action<GameObject> FadeInCloud;
    public static Action<GameObject> FadeOutCloud;
    public static Action<String> GetModel;
    public static Action<List<string>> SetEndingClouds;
    public static Action<TextAsset> SetStory;
    public static Action MoveButterfly;
    public static Action<GameObject> Destroy;
    public static Action Shake;
}
