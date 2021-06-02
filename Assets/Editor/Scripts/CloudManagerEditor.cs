using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Game_CloudManager))]
public class CloudManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {

        Game_CloudManager script = (Game_CloudManager)target;
        if (GUILayout.Button("Generate New Cloudscape"))
        {
            script.GenerateNewClouds();
        }

        DrawDefaultInspector();
    }
}