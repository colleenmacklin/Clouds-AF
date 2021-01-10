using UnityEngine;
using System.Collections;
using UnityEditor;

public class LuxParticles_HelpDrawer : MaterialPropertyDrawer
{
    override public void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
    {
        float brightness = 1.45f;
        if (!EditorGUIUtility.isProSkin) {
            brightness = 1.0f;
        }
        Color HelpCol = new Color(0.32f * brightness, 0.50f * brightness, 1.0f * brightness, 1.0f * brightness);
        GUIStyle hStyle = GUI.skin.GetStyle("HelpBox");
        hStyle.normal.textColor = HelpCol;

        hStyle.padding = new RectOffset(18, 0, 1, 3);

        Color col = GUI.contentColor;
        Color colbg = GUI.backgroundColor;

        GUI.contentColor = HelpCol;
        GUI.backgroundColor = Color.clear;

        GUILayout.Space(-4);
        EditorGUILayout.TextArea(label, hStyle);

        GUI.contentColor = col;
        GUI.backgroundColor = colbg;
    }

    public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
    {
        return base.GetPropertyHeight(prop, label, editor) * 0.0f;
    }
}