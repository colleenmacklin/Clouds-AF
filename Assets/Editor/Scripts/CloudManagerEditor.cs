using UnityEngine;
using System.Collections;
//#if UNITY_EDITOR
using UnityEditor;
//#endif


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