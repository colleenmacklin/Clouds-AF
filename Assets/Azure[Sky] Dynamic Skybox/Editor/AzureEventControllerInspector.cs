using UnityEngine;
using UnityEngine.AzureSky;
using UnityEditorInternal;

namespace UnityEditor.AzureSky
{
    [CustomEditor(typeof(AzureEventController))]
    public class AzureEventControllerInspector : Editor
    {
        // Editor only
        private AzureEventController m_target;
        private readonly Color m_alphaColor = new Color(1.0f, 1.0f, 1.0f, 0.35f);
        private Rect m_controlRect;
        private GUIStyle m_headerStyle;
        
        // GUIContents
        private readonly GUIContent[] m_guiContent = new[]
        {
            new GUIContent("Scan Mode", "Sets the time interval to scan the event list to check if it needs to invoke some event action."),
            new GUIContent("Event Actions", ""),
            new GUIContent("Unity Event", "")
        };
        
        private readonly GUIContent[] m_dateSelectorContent = new[]
        {
	        new GUIContent("Day", ""),
	        new GUIContent("Month", ""),
	        new GUIContent("Year", "")
        };
        
        private readonly GUIContent[] m_timeSelectorContent = new[]
        {
	        new GUIContent("Hour", ""),
	        new GUIContent("Minute", "")
        };
        
        // Serialized properties
        private SerializedProperty m_scanMode;
        private SerializedProperty m_eventList;

        private ReorderableList m_reorderableEventList;

        private void OnEnable()
        {
            // Get target
            m_target = (AzureEventController) target;
            
            // Find the serialized properties
            m_scanMode = serializedObject.FindProperty("scanMode");
            m_eventList = serializedObject.FindProperty("eventList");
            
             // Create event system list
             m_reorderableEventList = new ReorderableList(serializedObject, m_eventList, true, true, true, true)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
	                rect.y += 2;
	                var element = m_reorderableEventList.serializedProperty.GetArrayElementAtIndex(index);
	                var height = EditorGUIUtility.singleLineHeight;
	                
	                element.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(new Rect(rect.x, rect.y, rect.width, height), element.isExpanded, "Event Action " + index.ToString());
	                if (element.isExpanded)
	                {
		                // Date settings
		                rect.y += height +6;
		                int[] dateSelector = new int[3];
		                dateSelector[0] = element.FindPropertyRelative("day").intValue;
		                dateSelector[1] = element.FindPropertyRelative("month").intValue;
		                dateSelector[2] = element.FindPropertyRelative("year").intValue;
		                GUI.color = m_alphaColor;
		                EditorGUI.LabelField( new Rect(rect.x -3, rect.y -3, rect.width +8, height + 5), "", m_headerStyle);
		                GUI.color = Color.white;
		                EditorGUI.MultiIntField(new Rect(rect.x, rect.y, rect.width, height), m_dateSelectorContent, dateSelector);
		                element.FindPropertyRelative("day").intValue = dateSelector[0];
		                element.FindPropertyRelative("month").intValue = dateSelector[1];
		                element.FindPropertyRelative("year").intValue = dateSelector[2];
		                
		                // Time settings
		                rect.y += height +4;
		                int[] timeSelector = new int[2];
		                timeSelector[0] = element.FindPropertyRelative("hour").intValue;
		                timeSelector[1] = element.FindPropertyRelative("minute").intValue;
		                GUI.color = m_alphaColor;
		                EditorGUI.LabelField( new Rect(rect.x -3, rect.y -3, rect.width +8, height + 5), "", m_headerStyle);
		                GUI.color = Color.white;
		                EditorGUI.MultiIntField(new Rect(rect.x, rect.y, rect.width, height), m_timeSelectorContent, timeSelector);
		                element.FindPropertyRelative("hour").intValue = timeSelector[0];
		                element.FindPropertyRelative("minute").intValue = timeSelector[1];
		                
		                // UnityEvent list
		                rect.y += height +4;
		                EditorGUI.PropertyField( new Rect(rect.x -2, rect.y, rect.width +6, height), element.FindPropertyRelative("eventAction"), m_guiContent[2]);
	                }
	                EditorGUI.EndFoldoutHeaderGroup();
                },

                onAddCallback = (ReorderableList l) =>
                {
                    var index = l.serializedProperty.arraySize;
                    l.serializedProperty.arraySize++;
                    l.index = index;
                },

                drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, m_guiContent[1], EditorStyles.boldLabel);
                },
                
                elementHeightCallback = (int index) =>
                {
	                var element = m_reorderableEventList.serializedProperty.GetArrayElementAtIndex(index);
	                var elementHeight = EditorGUI.GetPropertyHeight(element);
	                var margin = EditorGUIUtility.standardVerticalSpacing;
	                if (element.isExpanded) return elementHeight - 50; else return elementHeight + margin;
                },

                drawElementBackgroundCallback = (rect, index, active, focused) =>
                {
                    if (active)
	                    GUI.Box(new Rect(rect.x +2, rect.y, rect.width -4, rect.height +2), "","selectionRect");
                }
        	};
        }

        public override void OnInspectorGUI()
        {
	        // Initializing the custom styles
	        m_headerStyle = new GUIStyle("selectionRect")
	        {
		        fontStyle = FontStyle.Bold,
		        contentOffset = new Vector2(0f, -2f)
	        };
	        
            // Start custom inspector
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            
            EditorGUILayout.PropertyField(m_scanMode, m_guiContent[0]);
            m_reorderableEventList.DoLayoutList();
            
            // Update the inspector when there is a change
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(m_target, "Undo Azure Event Controller");
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}