using UnityEngine;
using UnityEngine.AzureSky;
using UnityEditorInternal;

namespace UnityEditor.AzureSky
{
    [CustomEditor(typeof(AzureSkyController))]
    public class AzureSkyControllerInspector : Editor
    {
        // Editor only
        public Texture2D logoTexture;
        private AzureSkyController m_target;
        private Rect m_controlRect;
        private readonly Color m_greenColor = new Color(0.85f, 1.0f, 0.85f);
        private readonly Color m_redColor = new Color(1.0f, 0.75f, 0.75f);
        private GUIStyle m_textBarStyle;

        // GUIContents
        private readonly GUIContent[] m_guiContent = new[]
        {
            new GUIContent("Sun Transform", "The Transform used to simulate the sun position in the sky."),
            new GUIContent("Moon Transform", "The Transform used to simulate the moon position in the sky."),
            new GUIContent("Directional Light", "The directional light used to apply the lighting of the sun and moon to the scene."),
            new GUIContent("Sky Material", "The material used to render the sky."),
            new GUIContent("Reflection Probe", "The reflection probe used to compute the sky reflection."),
            new GUIContent("Default Day Profiles", "Stores the default day profiles. A random profile from this list will be used by sky system every time the next day starts."),
            new GUIContent("Go", "Changes the global weather to this specific profile in the list."),
            new GUIContent("Global Weather Profiles", "Stores the profiles used to control the global climate."),
            new GUIContent("Local Weather Zones", "Place here all the local weather zones and arrange according to its priorities."),
            new GUIContent("Trigger", "Transform that will drive the local weather zone blending feature. Setting this field to 'null' will disable local weather zones (global one will still work)."),
            new GUIContent("Mie Depth", "Sets the Mie distance range."),
            new GUIContent("Fog Material", "The material used to render the fog scattering."),
            new GUIContent("Empty Sky Shader", "The shader used to render the empty sky."),
            new GUIContent("Static Cloud Shader", "The shader used to render the static clouds."),
            new GUIContent("Dynamic Cloud Shader", "The shader used to render the dynamic clouds."),
            new GUIContent("Scattering Mode", "Sets how the scattering color will be performed. You can set a custom scattering color by editing the 'Scattering Color' property from the 'Scattering' tab of each day profile."),
            new GUIContent("Cloud Mode", "Sets the cloud mode."),
            new GUIContent("Day Transition Time", "Sets the duration in seconds of the default day profiles transition when the time changes to the next calendar day at 24 o'clock." +
                                                  " If a global weather profile is in use, the transition will not be performed."),
            new GUIContent("Output Profile", "Place here the output profile that stores the extra properties you want to include in this sky controller. Note that the day profiles must be using this same output profile."),
            new GUIContent("Shader Update Mode", "How should shader uniforms be updated? Select 'By Material' if you want to use multiple views showing different sky settings.")
            
        };
        
        private Vector3 m_starFieldPosition = Vector3.zero;
        private Vector3 m_starFieldColor = Vector3.one;
        
        // Serialized properties
        private SerializedProperty m_showReferencesHeaderGroup;
        private SerializedProperty m_showProfilesHeaderGroup;
        private SerializedProperty m_showEventsHeaderGroup;
        private SerializedProperty m_showOptionsHeaderGroup;
        private SerializedProperty m_showOutputsHeaderGroup;
        private SerializedProperty m_sunTransform;
        private SerializedProperty m_moonTransform;
        private SerializedProperty m_directionalLight;
        private SerializedProperty m_skyMaterial;
        private SerializedProperty m_fogMaterial;
        private SerializedProperty m_emptySkyShader;
        private SerializedProperty m_staticCloudShader;
        private SerializedProperty m_dynamicCloudShader;
        private SerializedProperty m_defaultProfileList;
        private SerializedProperty m_globalWeatherList;
        private SerializedProperty m_weatherZoneList;
        private SerializedProperty m_defaultWeatherTransitionTime;
        private SerializedProperty m_weatherZoneTrigger;
        private SerializedProperty m_onMinuteChange;
        private SerializedProperty m_onHourChange;
        private SerializedProperty m_onDayChange;
        private SerializedProperty m_scatteringMode;
        private SerializedProperty m_cloudMode;
        private SerializedProperty m_shaderUpdateMode;
        private SerializedProperty m_dayTransitionTime;
        private SerializedProperty m_mieDepth;
        private SerializedProperty m_outputProfile;
        
        // Reorderable lists
        private ReorderableList m_reorderableDefaultProfileList;
        private ReorderableList m_reorderableGlobalWeatherList;
        private ReorderableList m_reorderableWeatherZoneList;

        private void OnEnable()
        {
            // Get target
            m_target = (AzureSkyController) target;
            
            // Find the serialized properties
            m_showReferencesHeaderGroup = serializedObject.FindProperty("showReferencesHeaderGroup");
            m_showProfilesHeaderGroup = serializedObject.FindProperty("showProfilesHeaderGroup");
            m_showEventsHeaderGroup = serializedObject.FindProperty("showEventsHeaderGroup");
            m_showOptionsHeaderGroup = serializedObject.FindProperty("showOptionsHeaderGroup");
            m_showOutputsHeaderGroup = serializedObject.FindProperty("showOutputsHeaderGroup");
            m_sunTransform = serializedObject.FindProperty("sunTransform");
            m_moonTransform = serializedObject.FindProperty("moonTransform");
            m_directionalLight = serializedObject.FindProperty("directionalLight");
            m_skyMaterial = serializedObject.FindProperty("skyMaterial");
            m_fogMaterial = serializedObject.FindProperty("fogMaterial");
            m_emptySkyShader = serializedObject.FindProperty("emptySkyShader");
            m_staticCloudShader = serializedObject.FindProperty("staticCloudShader");
            m_dynamicCloudShader = serializedObject.FindProperty("dynamicCloudShader");
            m_defaultProfileList = serializedObject.FindProperty("defaultProfileList");
            m_globalWeatherList = serializedObject.FindProperty("globalWeatherList");
            m_weatherZoneList = serializedObject.FindProperty("weatherZoneList");
            m_defaultWeatherTransitionTime = serializedObject.FindProperty("defaultWeatherTransitionTime");
            m_weatherZoneTrigger = serializedObject.FindProperty("weatherZoneTrigger");
            m_onMinuteChange = serializedObject.FindProperty("onMinuteChange");
            m_onHourChange = serializedObject.FindProperty("onHourChange");
            m_onDayChange = serializedObject.FindProperty("onDayChange");
            m_scatteringMode = serializedObject.FindProperty("scatteringMode");
            m_cloudMode = serializedObject.FindProperty("cloudMode");
            m_shaderUpdateMode = serializedObject.FindProperty("shaderUpdateMode");
            m_dayTransitionTime = serializedObject.FindProperty("dayTransitionTime");
            m_mieDepth = serializedObject.FindProperty("mieDepth");
            m_outputProfile = serializedObject.FindProperty("outputProfile");
            
            // Create default profile list
            m_reorderableDefaultProfileList = new ReorderableList(serializedObject, m_defaultProfileList, true, true, true, true)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
	                rect.y += 2;
                    Rect fieldRect = new Rect(rect.x + 65, rect.y, rect.width - 65, EditorGUIUtility.singleLineHeight);
                    
                    // Profile index
	                EditorGUI.LabelField(rect, "Profile " + index.ToString());

                    // Object field
                    GUI.color = m_greenColor;
	                if (!m_target.defaultProfileList[index]) GUI.color = m_redColor;
	                EditorGUI.PropertyField(fieldRect, m_defaultProfileList.GetArrayElementAtIndex(index), GUIContent.none);
	                GUI.color = Color.white;
                },
                
                onAddCallback = (ReorderableList l) =>
                {
                    var index = l.serializedProperty.arraySize;
                    l.serializedProperty.arraySize++;
                    l.index = index;
                },
                
                drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, m_guiContent[5], EditorStyles.boldLabel);
                },
                
                drawElementBackgroundCallback = (rect, index, active, focused) =>
                {
                    if (active)
	                    GUI.Box(new Rect(rect.x +2, rect.y, rect.width -4, rect.height +1), "","selectionRect");
                }
        	};
            
            // Create global weather list
            m_reorderableGlobalWeatherList = new ReorderableList(serializedObject, m_globalWeatherList, true, false, true, true)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    rect.y += 2;
                    Rect fieldRect = new Rect(rect.x + 65, rect.y, rect.width - 100 - 28, EditorGUIUtility.singleLineHeight);
                    var element = m_reorderableGlobalWeatherList.serializedProperty.GetArrayElementAtIndex(index);
                    var profile = element.FindPropertyRelative("profile");
                    var transition = element.FindPropertyRelative("transitionTime");

                    // Profile index
                    EditorGUI.LabelField(rect, "Profile  " + index.ToString());
                    
                    // Object field
                    GUI.color = m_greenColor;
                    if (!m_target.globalWeatherList[index].profile) GUI.color = m_redColor;
                    EditorGUI.PropertyField(fieldRect, profile, GUIContent.none);
                    GUI.color = Color.white;
                    
                    // Transition time field
                    fieldRect = new Rect(rect.x + rect.width - 61, rect.y, 28, EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(fieldRect, transition, GUIContent.none);
                    
                    // Go button
                    fieldRect = new Rect(rect.x + rect.width - 30, rect.y, 30, EditorGUIUtility.singleLineHeight);
                    if (GUI.Button(fieldRect, m_guiContent[6]))
                    {
                        if (Application.isPlaying)
                        {
	                        m_target.SetNewWeatherProfile(index);
                        }
                        else
                        {
                            Debug.Log("To perform a weather transition, the application must be playing.");
                        }
                    }
                },
                
                onAddCallback = (ReorderableList l) =>
                {
                    var index = l.serializedProperty.arraySize;
                    l.serializedProperty.arraySize++;
                    l.index = index;

                    var element = l.serializedProperty.GetArrayElementAtIndex(index);
                    element.FindPropertyRelative("transitionTime").floatValue = 10.0f;
                },
                
                drawElementBackgroundCallback = (rect, index, active, focused) =>
                {
                    if (active)
	                    GUI.Box(new Rect(rect.x +2, rect.y, rect.width -4, rect.height +1), "","selectionRect");
                }
        	};
            
            // Create weather zone list
            m_reorderableWeatherZoneList = new ReorderableList(serializedObject, m_weatherZoneList, true, true, true, true)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
	                rect.y += 2;
	                Rect fieldRect = new Rect(rect.x + 65, rect.y, rect.width - 65, EditorGUIUtility.singleLineHeight);
                    
	                // Profile index
	                EditorGUI.LabelField(rect, "Priority " + index.ToString());

	                // Object field
	                GUI.color = m_greenColor;
	                if (!m_target.weatherZoneList[index]) GUI.color = m_redColor;
	                EditorGUI.PropertyField(fieldRect, m_weatherZoneList.GetArrayElementAtIndex(index), GUIContent.none);
	                GUI.color = Color.white;
                },
                
                onAddCallback = (ReorderableList l) =>
                {
                    var index = l.serializedProperty.arraySize;
                    l.serializedProperty.arraySize++;
                    l.index = index;
                },
                
                drawElementBackgroundCallback = (rect, index, active, focused) =>
                {
                    if (active)
	                    GUI.Box(new Rect(rect.x +2, rect.y, rect.width -4, rect.height +1), "","selectionRect");
                }
        	};
        }

        public override void OnInspectorGUI()
        {
	        m_textBarStyle = new GUIStyle("WhiteMiniLabel")
	        {
		        alignment = TextAnchor.MiddleCenter,
		        contentOffset = new Vector2(0, -3)
	        };
	        
            // Start custom Inspector
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            // Logo
            m_controlRect = EditorGUILayout.GetControlRect();
            EditorGUI.LabelField(new Rect(m_controlRect.x - 3, m_controlRect.y, m_controlRect.width + 3, 65), "", "", "selectionRect");
            if (logoTexture)
                GUI.DrawTexture(new Rect(m_controlRect.x, m_controlRect.y, 261, 56), logoTexture);
            GUILayout.Space(32);
            GUILayout.Label("Version 6.0.2", EditorStyles.miniLabel);

            // References header group
            GUILayout.Space(2);
            m_showReferencesHeaderGroup.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(m_showReferencesHeaderGroup.isExpanded, "References");
            if (m_showReferencesHeaderGroup.isExpanded)
            {
                EditorGUI.indentLevel++;
                
                // Sun transform
                GUI.color = m_greenColor;
                if (!m_target.sunTransform) GUI.color = m_redColor;
                EditorGUILayout.PropertyField(m_sunTransform, m_guiContent[0]);

                // Moon transform
                GUI.color = m_greenColor;
                if (!m_target.moonTransform) GUI.color = m_redColor;
                EditorGUILayout.PropertyField(m_moonTransform, m_guiContent[1]);

                // Light transform
                GUI.color = m_greenColor;
                if (!m_target.directionalLight) GUI.color = m_redColor;
                EditorGUILayout.PropertyField(m_directionalLight, m_guiContent[2]);

                // Sky material
                GUI.color = m_greenColor;
                if (!m_target.skyMaterial) GUI.color = m_redColor;
                EditorGUILayout.PropertyField(m_skyMaterial, m_guiContent[3]);
                
                // Fog material
                GUI.color = m_greenColor;
                if (!m_target.fogMaterial) GUI.color = m_redColor;
                EditorGUILayout.PropertyField(m_fogMaterial, m_guiContent[11]);
                
                // Empty sky shader
                GUI.color = m_greenColor;
                if (!m_target.emptySkyShader) GUI.color = m_redColor;
                EditorGUILayout.PropertyField(m_emptySkyShader, m_guiContent[12]);
                
                // Static clouds shader
                GUI.color = m_greenColor;
                if (!m_target.staticCloudShader) GUI.color = m_redColor;
                EditorGUILayout.PropertyField(m_staticCloudShader, m_guiContent[13]);
                
                // Dynamic clouds shader
                GUI.color = m_greenColor;
                if (!m_target.dynamicCloudShader) GUI.color = m_redColor;
                EditorGUILayout.PropertyField(m_dynamicCloudShader, m_guiContent[14]);
                
                EditorGUI.indentLevel--;
                GUI.color = Color.white;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            // Profiles header group
            GUILayout.Space(2);
            m_showProfilesHeaderGroup.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(m_showProfilesHeaderGroup.isExpanded , "Profiles");
            if (m_showProfilesHeaderGroup.isExpanded)
            {
                // Draw the default reorderable lists
	            m_reorderableDefaultProfileList.DoLayoutList();
	            EditorGUILayout.Space();
	            EditorGUILayout.Space();
	            EditorGUILayout.Space();
	            
	            // Progress bar
				GUILayout.Space(-5);
				m_controlRect = EditorGUILayout.GetControlRect();
				EditorGUI.ProgressBar (new Rect(m_controlRect.x +1, m_controlRect.y -2, m_controlRect.width -2, m_controlRect.height -4), m_target.globalWeatherTransitionProgress, "");
				EditorGUI.LabelField(new Rect(m_controlRect.x, m_controlRect.y -2, m_controlRect.width, m_controlRect.height), "Transition Progress", m_textBarStyle);
				
				// Draw custom header for the global climate reorderable list
				GUILayout.Space(-6);
				EditorGUILayout.BeginHorizontal("RL Header");
				m_controlRect = EditorGUILayout.GetControlRect();
				EditorGUI.LabelField(new Rect(m_controlRect.x +2,m_controlRect.y,m_controlRect.width,m_controlRect.height), m_guiContent[7], EditorStyles.boldLabel);
				EditorGUILayout.EndHorizontal();
				
				// Draw the global climate reorderable list
				GUILayout.Space(2);
				m_reorderableGlobalWeatherList.DoLayoutList();
				
				// Add the default profile options above the reorderable list elements
				EditorGUI.LabelField(new Rect(m_controlRect.x -3, m_controlRect.y +17, m_controlRect.width +6, m_controlRect.height +6), "", "", "RL Background");
				EditorGUI.LabelField(new Rect(m_controlRect.x +4, m_controlRect.y +25, 10, m_controlRect.height), "", "", "RL DragHandle");
				EditorGUI.LabelField(new Rect(m_controlRect.x +18, m_controlRect.y +20, 65, m_controlRect.height), "Profile -1");
				EditorGUI.LabelField(new Rect(m_controlRect.x +82, m_controlRect.y +20, m_controlRect.width -149, m_controlRect.height), "@Default", EditorStyles.helpBox);
				EditorGUI.PropertyField(new Rect(m_controlRect.x + m_controlRect.width - 65, m_controlRect.y +20, 28, m_controlRect.height), m_defaultWeatherTransitionTime, GUIContent.none);
				
				// Go button
				if (GUI.Button(new Rect(m_controlRect.x + m_controlRect.width - 34, m_controlRect.y + 20, 30, m_controlRect.height), m_guiContent[6]))
				{
					if (Application.isPlaying)
					{
						m_target.SetNewWeatherProfile(-1);
					}
					else
					{
						Debug.Log("To perform a weather transition, the application must be playing.");
					}
				}
				
				EditorGUILayout.Space();
				EditorGUILayout.Space();
				EditorGUILayout.Space();
				
				// Draw custom header for the weather zones reorderable list
				GUILayout.Space(-6);
				EditorGUILayout.BeginHorizontal("RL Header");
				m_controlRect = EditorGUILayout.GetControlRect();
				EditorGUI.LabelField(new Rect(m_controlRect.x +2,m_controlRect.y,m_controlRect.width,m_controlRect.height), m_guiContent[8], EditorStyles.boldLabel);
				EditorGUILayout.EndHorizontal();
				
				// Draw the weather zone list
				GUILayout.Space(2);
				m_reorderableWeatherZoneList.DoLayoutList();
				
				// Add the trigger object field above the reorderable list elements
				EditorGUI.LabelField(new Rect(m_controlRect.x -3, m_controlRect.y +17, m_controlRect.width +6, m_controlRect.height +6), "", "", "RL Background");
				EditorGUI.LabelField(new Rect(m_controlRect.x +4, m_controlRect.y +25, 10, m_controlRect.height), "", "", "RL DragHandle");
				EditorGUI.LabelField(new Rect(m_controlRect.x +18, m_controlRect.y +20, 65, m_controlRect.height), m_guiContent[9]);
				GUI.color = m_greenColor;
				if (!m_target.weatherZoneTrigger) GUI.color = m_redColor;
				EditorGUI.PropertyField(new Rect(m_controlRect.x +82, m_controlRect.y +20, m_controlRect.width -86, m_controlRect.height), m_weatherZoneTrigger, GUIContent.none);
				GUI.color = Color.white;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            // Events header group
            GUILayout.Space(2);
            m_showEventsHeaderGroup.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(m_showEventsHeaderGroup.isExpanded, "Events");
            if (m_showEventsHeaderGroup.isExpanded)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_onMinuteChange);
                EditorGUILayout.PropertyField(m_onHourChange);
                EditorGUILayout.PropertyField(m_onDayChange);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            // Options header group
            GUILayout.Space(2);
            m_showOptionsHeaderGroup.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(m_showOptionsHeaderGroup.isExpanded, "Options");
            if (m_showOptionsHeaderGroup.isExpanded)
            {
	            EditorGUI.indentLevel++;
	            EditorGUILayout.PropertyField(m_scatteringMode, m_guiContent[15]);
	            EditorGUILayout.PropertyField(m_cloudMode, m_guiContent[16]);
	            EditorGUILayout.PropertyField(m_shaderUpdateMode, m_guiContent[19]);
	            EditorGUILayout.Slider(m_dayTransitionTime, 0.0f, 30.0f, m_guiContent[17]);
	            EditorGUILayout.Slider(m_mieDepth, 0.0f, 1.0f, m_guiContent[10]);
	            EditorGUILayout.Space();
	            
	            // Star field position
	            m_starFieldPosition.x = EditorGUILayout.Slider("Starfield Position X", m_target.starFieldPosition.x, -180.0f, 180.0f);
	            m_starFieldPosition.y = EditorGUILayout.Slider("Starfield Position Y", m_target.starFieldPosition.y, -180.0f, 180.0f);
	            m_starFieldPosition.z = EditorGUILayout.Slider("Starfield Position Z", m_target.starFieldPosition.z, -180.0f, 180.0f);
	            EditorGUILayout.Space();
	            
	            // Star field color
	            m_starFieldColor.x = EditorGUILayout.Slider("Starfield Color R", m_target.starFieldColor.x, 1.0f, 2.0f);
	            m_starFieldColor.y = EditorGUILayout.Slider("Starfield Color G", m_target.starFieldColor.y, 1.0f, 2.0f);
	            m_starFieldColor.z = EditorGUILayout.Slider("Starfield Color B", m_target.starFieldColor.z, 1.0f, 2.0f);
	            EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            // Outputs header group
            GUILayout.Space(2);
            m_showOutputsHeaderGroup.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(m_showOutputsHeaderGroup.isExpanded, "Outputs");
            if (m_showOutputsHeaderGroup.isExpanded)
            {
	            EditorGUI.indentLevel++;
	            GUI.color = m_greenColor;
	            if (!m_target.outputProfile) GUI.color = m_redColor;
	            EditorGUILayout.PropertyField(m_outputProfile, m_guiContent[18]);
	            GUI.color = Color.white;
	            EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            
            // End custom Inspector
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(m_target, "Undo Azure Sky Controller");
                serializedObject.ApplyModifiedProperties();
                m_target.starFieldPosition = m_starFieldPosition;
                m_target.starFieldColor = m_starFieldColor;
                m_target.UpdateMaterialSettings();
            }
        }
    }
}