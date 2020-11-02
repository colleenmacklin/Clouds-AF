using UnityEngine;
using System.Collections;
using UnityEditor;

public class LuxParticles_FadeDistancesDrawer : MaterialPropertyDrawer {

	override public void OnGUI (Rect position, MaterialProperty prop, string label, MaterialEditor editor) {
		
	//	Needed by Unity 2019
		EditorGUIUtility.labelWidth = 0;

		Vector4 vec4value = prop.vectorValue;

	//	In order to not break old settings we remap the the values here.
		Vector2 nearFade = Vector2.zero;
		nearFade.x = vec4value.w;
		nearFade.y = vec4value.x;
		Vector2 farFade = Vector2.zero;
		farFade.x = vec4value.y;
		farFade.y = vec4value.z;


		GUILayout.Space(-16);
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.BeginVertical();
			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("Near:  X (Start) Y (Range)");
				GUILayout.Space(-8);
				nearFade = EditorGUILayout.Vector2Field ("", nearFade);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("Far:    X (End)   Y (Range)");
				GUILayout.Space(-8);
				farFade = EditorGUILayout.Vector2Field ("", farFade);
			EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.EndVertical();
	
	//	Remapping.
		if (EditorGUI.EndChangeCheck ()) {
			vec4value.x = nearFade.y;
			vec4value.w = Mathf.Max(0.0f, nearFade.x);
			vec4value.y = farFade.x;
			vec4value.z = farFade.y;
			prop.vectorValue = vec4value;
		}
	}
}