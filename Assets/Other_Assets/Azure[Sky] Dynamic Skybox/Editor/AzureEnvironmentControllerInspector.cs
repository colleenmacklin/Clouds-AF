using UnityEngine;
using UnityEngine.AzureSky;
using UnityEngine.Rendering;

namespace UnityEditor.AzureSky
{
    [CustomEditor(typeof(AzureEnvironmentController))]
    public class AzureEnvironmentControllerInspector : Editor
    {
        // Editor only
        private AzureEnvironmentController m_target;

        // GUIContents
        private readonly GUIContent[] m_guiContent = new[]
        {
            new GUIContent("Reflection Probe", "The reflection probe used by the sky system."),
            new GUIContent("Refresh Mode", "The mode used to update the reflection probe."),
            new GUIContent("Update at First Frame?", "Updates the reflection probe in the first frame? If disabled, the reflection probe will be updated for the first time only after the time set in refresh interval."),
            new GUIContent("Refresh Interval", "The time interval (in seconds) to update the reflection probe."),
            new GUIContent("Follow Target", "If selected, the reflection probe will follow this transform in the scene, usually the main camera is used here."),
            new GUIContent("Time Slicing", "If enabled this probe will update over several frames, to help reduce the impact on the frame rate."),
            new GUIContent("State", "Enabled or disable the reflection probe."),
            new GUIContent("Environment Intensity", ""),
            new GUIContent("Environment Ambient Color", ""),
            new GUIContent("Environment Equator Color", ""),
            new GUIContent("Environment Ground Color", "")
        };

        // Serialized properties
        private SerializedProperty m_reflectionProbe;
        private SerializedProperty m_state;
        private SerializedProperty m_timeSlicingMode;
        private SerializedProperty m_refreshMode;
        private SerializedProperty m_updateAtFirstFrame;
        private SerializedProperty m_refreshInterval;
        private SerializedProperty m_environmentIntensity;
        private SerializedProperty m_environmentAmbientColor;
        private SerializedProperty m_environmentEquatorColor;
        private SerializedProperty m_environmentGroundColor;

        private void OnEnable()
        {
            // Get target
            m_target = (AzureEnvironmentController) target;

            // Find the serialized properties
            m_reflectionProbe = serializedObject.FindProperty("reflectionProbe");
            m_state = serializedObject.FindProperty("state");
            m_timeSlicingMode = serializedObject.FindProperty("timeSlicingMode");
            m_refreshMode = serializedObject.FindProperty("refreshMode");
            m_updateAtFirstFrame = serializedObject.FindProperty("updateAtFirstFrame");
            m_refreshInterval = serializedObject.FindProperty("refreshInterval");
            m_environmentIntensity = serializedObject.FindProperty("environmentIntensity");
            m_environmentAmbientColor = serializedObject.FindProperty("environmentAmbientColor");
            m_environmentEquatorColor = serializedObject.FindProperty("environmentEquatorColor");
            m_environmentGroundColor = serializedObject.FindProperty("environmentGroundColor");
    }

        public override void OnInspectorGUI()
        {
            // Start custom Inspector
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            // Reflection probe
            EditorGUILayout.PropertyField(m_reflectionProbe, m_guiContent[0]);

            // State
            EditorGUILayout.PropertyField(m_state, m_guiContent[6]);

            if (m_target.state == AzureReflectionProbeState.On)
            {
                // Time slicing mode
                EditorGUILayout.PropertyField(m_timeSlicingMode, m_guiContent[5]);

                // Refresh mode
                EditorGUILayout.PropertyField(m_refreshMode, m_guiContent[1]);

                if (m_target.refreshMode == ReflectionProbeRefreshMode.ViaScripting)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(m_updateAtFirstFrame, m_guiContent[2]);
                    EditorGUILayout.PropertyField(m_refreshInterval, m_guiContent[3]);
                    EditorGUI.indentLevel--;
                }
            }

            // Environment lighting
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_environmentIntensity, m_guiContent[7]);
            EditorGUILayout.PropertyField(m_environmentAmbientColor, m_guiContent[8]);
            EditorGUILayout.PropertyField(m_environmentEquatorColor, m_guiContent[9]);
            EditorGUILayout.PropertyField(m_environmentGroundColor, m_guiContent[10]);

            // End custom Inspector
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(m_target, "Undo Azure Reflection Controller");
                serializedObject.ApplyModifiedProperties();

                switch (m_target.state)
                {
                    case AzureReflectionProbeState.On:
                        m_target.reflectionProbe.gameObject.SetActive(true);
                        break;
                    
                    case AzureReflectionProbeState.Off:
                        m_target.reflectionProbe.gameObject.SetActive(false);
                        break;
                }
            }
        }
    }
}