using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CloudManager))]
public class CloudManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {

        CloudManager script = (CloudManager)target;
        if (GUILayout.Button("Generate New Cloudscape"))
        {
            script.GenerateNewClouds();
        }
        if (GUILayout.Button("Set Next Clouds"))
        {
            script.SetNextShapes();
        }


        DrawDefaultInspector();
    }
}