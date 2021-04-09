using UnityEngine;
using UnityEngine.AzureSky;
using UnityEditorInternal;

namespace UnityEditor.AzureSky
{
    [CustomEditor(typeof(AzureWeatherProfile))]
    public class AzureWeatherProfileInspector : Editor
    {
        // Editor only
        private AzureWeatherProfile m_target;

        // Logo
        public Texture2D logoTexture;
        private Rect m_controlRect;

        private SerializedProperty m_overrideObject;
        private SerializedProperty m_profilePropertyList;
        private ReorderableList m_reorderableList;

        private void OnEnable()
        {
            // Get target
            m_target = (AzureWeatherProfile)target;

            m_overrideObject = serializedObject.FindProperty("overrideObject");
            m_profilePropertyList = serializedObject.FindProperty("profilePropertyList");
            m_reorderableList = new ReorderableList(serializedObject, m_profilePropertyList, false, true, false, false)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    rect.y += 2;
                    float height = EditorGUIUtility.singleLineHeight;
                    float half = rect.width / 2;

                    var element = m_reorderableList.serializedProperty.GetArrayElementAtIndex(index);

                    // Get name and description/tooltip
                    var name = element.FindPropertyRelative("name");
                    var description = element.FindPropertyRelative("description");
                    name.stringValue = m_target.overrideObject.customPropertyList[index].name;
                    description.stringValue = m_target.overrideObject.customPropertyList[index].description;

                    // Get types
                    var outputType = element.FindPropertyRelative("outputType");
                    outputType.enumValueIndex = (int)m_target.overrideObject.customPropertyList[index].outputType;
                    var floatPropertyType = element.FindPropertyRelative("floatPropertyType");
                    floatPropertyType.enumValueIndex = (int)m_target.overrideObject.customPropertyList[index].floatPropertyType;
                    var colorPropertyType = element.FindPropertyRelative("colorPropertyType");
                    colorPropertyType.enumValueIndex = (int)m_target.overrideObject.customPropertyList[index].colorPropertyType;

                    // Draw name using description as tooltip
                    Rect fieldRect = new Rect(rect.x, rect.y, half - 5, height);
                    EditorGUI.LabelField(fieldRect, new GUIContent(index + " - " + name.stringValue, description.stringValue));

                    // Draw the correct property for customization
                    fieldRect = new Rect(rect.x + half + 5, rect.y, rect.width - half - 5, height);
                    switch (outputType.enumValueIndex)
                    {
                        case 0: // Float type
                            switch (floatPropertyType.enumValueIndex)
                            {
                                case 0: // Slider
                                    EditorGUI.Slider(fieldRect, element.FindPropertyRelative("slider"), 0.0f, 1.0f, GUIContent.none);
                                    break;
                                case 1: // Timeline based curve
                                    EditorGUI.CurveField(fieldRect, element.FindPropertyRelative("timelineBasedCurve"), Color.green, new Rect(0, 0.0f, 24.0f, 1.0f), GUIContent.none);
                                    break;
                                case 2: // Sun based curve
                                    EditorGUI.CurveField(fieldRect, element.FindPropertyRelative("sunElevationBasedCurve"), Color.yellow, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
                                    break;
                                case 3: // Moon based curve
                                    EditorGUI.CurveField(fieldRect, element.FindPropertyRelative("moonElevationBasedCurve"), Color.cyan, new Rect(-1.0f, 0.0f, 2.0f, 1.0f), GUIContent.none);
                                    break;
                            }
                            break;

                        case 1: // Color type
                            switch (colorPropertyType.enumValueIndex)
                            {
                                case 0: // Color
                                    EditorGUI.PropertyField(fieldRect, element.FindPropertyRelative("colorField"), GUIContent.none);
                                    break;
                                case 1: // Timeline based gradient
                                    EditorGUI.PropertyField(fieldRect, element.FindPropertyRelative("timelineBasedGradient"), GUIContent.none);
                                    break;
                                case 2: // Sun based gradient
                                    EditorGUI.PropertyField(fieldRect, element.FindPropertyRelative("sunElevationBasedGradient"), GUIContent.none);
                                    break;
                                case 3: // Moon based gradient
                                    EditorGUI.PropertyField(fieldRect, element.FindPropertyRelative("moonElevationBasedGradient"), GUIContent.none);
                                    break;
                            }
                            break;
                    }
                },

                drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "Property List", EditorStyles.boldLabel);
                },

                drawElementBackgroundCallback = (rect, index, active, focused) => { }
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            // Logo
            m_controlRect = EditorGUILayout.GetControlRect(GUILayout.Height(65));
            EditorGUI.LabelField(m_controlRect, "", "", "selectionRect");
            if (logoTexture) GUI.DrawTexture(new Rect(m_controlRect.x, m_controlRect.y, 261, 56), logoTexture);

            EditorGUILayout.PropertyField(m_overrideObject);

            if (m_target.overrideObject)
            {
                m_reorderableList.serializedProperty.arraySize = m_target.overrideObject.customPropertyList.Count;
                m_reorderableList.DoLayoutList();
            }
            else
            {
                EditorGUILayout.HelpBox("There is no Override Object attached, please add an Override Object to customize the override properties.", MessageType.Error);
            }

            // Update the inspector when there is a change
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}