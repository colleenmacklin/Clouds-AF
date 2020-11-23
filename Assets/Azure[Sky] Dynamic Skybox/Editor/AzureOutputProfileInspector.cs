using UnityEngine;
using UnityEngine.AzureSky;
using UnityEditorInternal;

namespace UnityEditor.AzureSky
{
    [CustomEditor(typeof(AzureOutputProfile))]
    public class AzureOutputProfileInspector : Editor
    {
        // GUIContents
        private readonly GUIContent[] m_guiContent = new[]
        {
            new GUIContent("Output List", "Create and set the type of each extra property you want to include to the sky system.")
        };
        private SerializedProperty m_outputList;
        private ReorderableList m_reorderableOutputList;

        private void OnEnable()
        {
            m_outputList = serializedObject.FindProperty("outputList");
            
            // Create the output list
            m_reorderableOutputList = new ReorderableList(serializedObject, m_outputList, true, true, true, true)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    rect.y += 2;
                    float height = EditorGUIUtility.singleLineHeight;
                    Rect fieldRect = new Rect(rect.x, rect.y, rect.width, height);
                    var element = m_reorderableOutputList.serializedProperty.GetArrayElementAtIndex(index);
                    var type = element.FindPropertyRelative("type");
                    var description = element.FindPropertyRelative("description");
                    
                    // Output index
                    EditorGUI.LabelField(fieldRect, "Output " + index.ToString());
                    
                    // Type popup
                    fieldRect = new Rect(rect.x + 65, rect.y, rect.width - 65, height);
                    EditorGUI.PropertyField(fieldRect, type, GUIContent.none);
                    
                    // Description
                    fieldRect = new Rect(rect.x, rect.y + height + 2, rect.width, height * 3.0f);
                    if (description.stringValue == "") description.stringValue = "Description:";
                    description.stringValue = EditorGUI.TextArea(fieldRect, description.stringValue);
                },
                
                onAddCallback = (ReorderableList l) =>
                {
                    var index = l.serializedProperty.arraySize;
                    l.serializedProperty.arraySize++;
                    l.index = index;
                },
                
                drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, m_guiContent[0], EditorStyles.boldLabel);
                },
                
                elementHeightCallback = (int index) => EditorGUIUtility.singleLineHeight * 4.5f,
                
                drawElementBackgroundCallback = (rect, index, active, focused) =>
                {
                    if (active)
                        GUI.Box(new Rect(rect.x +2, rect.y, rect.width -4, rect.height +1), "","selectionRect");
                }
            };
        }

        public override void OnInspectorGUI()
        {
            // Start custom inspector
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            
            m_reorderableOutputList.DoLayoutList();
            
            // Update the inspector when there is a change
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}