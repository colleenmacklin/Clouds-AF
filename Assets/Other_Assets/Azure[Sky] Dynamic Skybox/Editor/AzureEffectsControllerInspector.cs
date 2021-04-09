using UnityEngine;
using UnityEditorInternal;
using UnityEngine.AzureSky;


namespace UnityEditor.AzureSky
{
    [CustomEditor(typeof(AzureEffectsController))]
    public class AzureEffectsControllerInspector : Editor
    {
        // Target
        private AzureEffectsController m_target;
        private Rect m_controlRect;
        private Rect m_headerRect;

        // Serialized properties
        private SerializedProperty m_referencesHeaderGroup;
        private SerializedProperty m_settingsHeaderGroup;
        private SerializedProperty m_thunderSeetingsHeaderGroup;
        private SerializedProperty m_windZone;
        private SerializedProperty m_lightRainAudioSource;
        private SerializedProperty m_mediumRainAudioSource;
        private SerializedProperty m_heavyRainAudioSource;
        private SerializedProperty m_lightWindAudioSource;
        private SerializedProperty m_mediumWindAudioSource;
        private SerializedProperty m_heavyWindAudioSource;
        private SerializedProperty m_defaultRainMaterial;
        private SerializedProperty m_heavyRainMaterial;
        private SerializedProperty m_snowMaterial;
        private SerializedProperty m_rippleMaterial;
        private SerializedProperty m_lightRainParticle;
        private SerializedProperty m_mediumRainParticle;
        private SerializedProperty m_heavyRainParticle;
        private SerializedProperty m_snowParticle;
        private SerializedProperty m_lightRainIntensity;
        private SerializedProperty m_mediumRainIntensity;
        private SerializedProperty m_heavyRainIntensity;
        private SerializedProperty m_snowIntensity;
        private SerializedProperty m_rainColor;
        private SerializedProperty m_snowColor;
        private SerializedProperty m_lightRainSoundFx;
        private SerializedProperty m_mediumRainSoundFx;
        private SerializedProperty m_heavyRainSoundFx;
        private SerializedProperty m_lightWindSoundFx;
        private SerializedProperty m_mediumWindSoundFx;
        private SerializedProperty m_heavyWindSoundFx;
        private SerializedProperty m_windSpeed;
        private SerializedProperty m_windDirection;

        private SerializedProperty m_thunderSettingsList;
        private ReorderableList m_reorderableThunderList;

        private void OnEnable()
        {
            // Get target
            m_target = (AzureEffectsController)target;

            // Find the serialized properties
            m_referencesHeaderGroup = serializedObject.FindProperty("m_referencesHeaderGroup");
            m_settingsHeaderGroup = serializedObject.FindProperty("m_settingsHeaderGroup");
            m_thunderSeetingsHeaderGroup = serializedObject.FindProperty("m_thunderSettingsHeaderGroup");
            m_windZone = serializedObject.FindProperty("windZone");
            m_lightRainAudioSource = serializedObject.FindProperty("lightRainAudioSource");
            m_mediumRainAudioSource = serializedObject.FindProperty("mediumRainAudioSource");
            m_heavyRainAudioSource = serializedObject.FindProperty("heavyRainAudioSource");
            m_lightWindAudioSource = serializedObject.FindProperty("lightWindAudioSource");
            m_mediumWindAudioSource = serializedObject.FindProperty("mediumWindAudioSource");
            m_heavyWindAudioSource = serializedObject.FindProperty("heavyWindAudioSource");
            m_defaultRainMaterial = serializedObject.FindProperty("defaultRainMaterial");
            m_heavyRainMaterial = serializedObject.FindProperty("heavyRainMaterial");
            m_snowMaterial = serializedObject.FindProperty("snowMaterial");
            m_rippleMaterial = serializedObject.FindProperty("rippleMaterial");
            m_lightRainParticle = serializedObject.FindProperty("lightRainParticle");
            m_mediumRainParticle = serializedObject.FindProperty("mediumRainParticle");
            m_heavyRainParticle = serializedObject.FindProperty("heavyRainParticle");
            m_snowParticle = serializedObject.FindProperty("snowParticle");
            m_lightRainIntensity = serializedObject.FindProperty("lightRainIntensity");
            m_mediumRainIntensity = serializedObject.FindProperty("mediumRainIntensity");
            m_heavyRainIntensity = serializedObject.FindProperty("heavyRainIntensity");
            m_snowIntensity = serializedObject.FindProperty("snowIntensity");
            m_rainColor = serializedObject.FindProperty("rainColor");
            m_snowColor = serializedObject.FindProperty("snowColor");
            m_lightRainSoundFx = serializedObject.FindProperty("lightRainSoundFx");
            m_mediumRainSoundFx = serializedObject.FindProperty("mediumRainSoundFx");
            m_heavyRainSoundFx = serializedObject.FindProperty("heavyRainSoundFx");
            m_lightWindSoundFx = serializedObject.FindProperty("lightWindSoundFx");
            m_mediumWindSoundFx = serializedObject.FindProperty("mediumWindSoundFx");
            m_heavyWindSoundFx = serializedObject.FindProperty("heavyWindSoundFx");
            m_windSpeed = serializedObject.FindProperty("windSpeed");
            m_windDirection = serializedObject.FindProperty("windDirection");

            // Create thunder list
            m_thunderSettingsList = serializedObject.FindProperty("thunderSettingsList");
            m_reorderableThunderList = new ReorderableList(serializedObject, m_thunderSettingsList, true, true, true, true)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    rect.y += 2;
                    var element = m_reorderableThunderList.serializedProperty.GetArrayElementAtIndex(index);
                    var height = EditorGUIUtility.singleLineHeight;

                    element.isExpanded = EditorGUI.Toggle(new Rect(rect.x - 5, rect.y, rect.width + 5, height), GUIContent.none, element.isExpanded, EditorStyles.foldoutHeader);
                    EditorGUI.LabelField(new Rect(rect.x + 5, rect.y, rect.width - 5, height), "Thunder " + index.ToString(), EditorStyles.miniLabel);

                    if (element.isExpanded)
                    {
                        rect.y += height + 1;
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, height), element.FindPropertyRelative("thunderPrefab"), new GUIContent("Thunder Prefab", ""));

                        rect.y += height + 1;
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, height), element.FindPropertyRelative("audioClip"), new GUIContent("Audio Clip", ""));

                        rect.y += height + 1;
                        EditorGUI.CurveField(new Rect(rect.x, rect.y, rect.width, height), element.FindPropertyRelative("lightFrequency"), Color.yellow, new Rect(0.0f, 0.0f, 1.0f, 1.0f), new GUIContent("Light Frequency", ""));

                        rect.y += height + 1;
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, height), element.FindPropertyRelative("audioDelay"), new GUIContent("Audio Delay", ""));

                        rect.y += height + 1;
                        EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, height), "Position");

                        rect.y += height + 1;
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, height), element.FindPropertyRelative("position"), GUIContent.none);

                        rect.y += height + 1;
                        if (GUI.Button(new Rect(rect.x + 15, rect.y, rect.width - 15, height), "Test"))
                        {
                            if (Application.isPlaying)
                                m_target.InstantiateThunderEffect(index);
                            else
                                Debug.Log("The application must be playing to perform a thunder effect.");
                        }
                    }
                },

                elementHeightCallback = (int index) =>
                {
                    var element = m_reorderableThunderList.serializedProperty.GetArrayElementAtIndex(index);
                    var elementHeight = EditorGUI.GetPropertyHeight(element);
                    var margin = EditorGUIUtility.standardVerticalSpacing + 4;
                    if (element.isExpanded) return elementHeight + 40; else return elementHeight + margin;
                },

                drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, new GUIContent("Thunder List", ""), EditorStyles.boldLabel);
                },

                drawElementBackgroundCallback = (rect, index, active, focused) =>
                {
                    if (active)
                        GUI.Box(new Rect(rect.x + 2, rect.y - 1, rect.width - 4, rect.height + 1), "", "selectionRect");
                },

                onAddCallback = (ReorderableList l) =>
                {
                    var index = l.serializedProperty.arraySize;
                    l.serializedProperty.arraySize++;
                    l.index = index;
                    var element = l.serializedProperty.GetArrayElementAtIndex(index);
                    element.FindPropertyRelative("lightFrequency").animationCurveValue = AnimationCurve.Constant(0.0f, 1.0f, 0.0f);
                }
            };
        }

        public override void OnInspectorGUI()
        {
            // Start custom inspector
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            // References header group
            m_controlRect = EditorGUILayout.GetControlRect();
            m_headerRect = new Rect(m_controlRect.x + 15, m_controlRect.y, m_controlRect.width - 20, EditorGUIUtility.singleLineHeight);
            m_referencesHeaderGroup.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(m_headerRect, m_referencesHeaderGroup.isExpanded, GUIContent.none, "RL Header");
            EditorGUI.Foldout(m_headerRect, m_referencesHeaderGroup.isExpanded, GUIContent.none);
            EditorGUI.LabelField(m_headerRect, new GUIContent(" References", ""));
            if (m_referencesHeaderGroup.isExpanded)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_windZone);
                EditorGUILayout.PropertyField(m_lightRainAudioSource);
                EditorGUILayout.PropertyField(m_mediumRainAudioSource);
                EditorGUILayout.PropertyField(m_heavyRainAudioSource);
                EditorGUILayout.PropertyField(m_lightWindAudioSource);
                EditorGUILayout.PropertyField(m_mediumWindAudioSource);
                EditorGUILayout.PropertyField(m_heavyWindAudioSource);
                EditorGUILayout.PropertyField(m_defaultRainMaterial);
                EditorGUILayout.PropertyField(m_heavyRainMaterial);
                EditorGUILayout.PropertyField(m_snowMaterial);
                EditorGUILayout.PropertyField(m_rippleMaterial);
                EditorGUILayout.PropertyField(m_lightRainParticle);
                EditorGUILayout.PropertyField(m_mediumRainParticle);
                EditorGUILayout.PropertyField(m_heavyRainParticle);
                EditorGUILayout.PropertyField(m_snowParticle);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space(2);

            // Settings header group
            m_controlRect = EditorGUILayout.GetControlRect();
            m_headerRect = new Rect(m_controlRect.x + 15, m_controlRect.y, m_controlRect.width - 20, EditorGUIUtility.singleLineHeight);
            m_settingsHeaderGroup.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(m_headerRect, m_settingsHeaderGroup.isExpanded, GUIContent.none, "RL Header");
            EditorGUI.Foldout(m_headerRect, m_settingsHeaderGroup.isExpanded, GUIContent.none);
            EditorGUI.LabelField(m_headerRect, new GUIContent(" Settings", ""));
            if (m_settingsHeaderGroup.isExpanded)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.Slider(m_lightRainIntensity, 0f, 1f);
                EditorGUILayout.Slider(m_mediumRainIntensity, 0f, 1f);
                EditorGUILayout.Slider(m_heavyRainIntensity, 0f, 1f);
                EditorGUILayout.Slider(m_snowIntensity, 0f, 1f);
                EditorGUILayout.PropertyField(m_rainColor);
                EditorGUILayout.PropertyField(m_snowColor);
                EditorGUILayout.Slider(m_lightRainSoundFx, 0f, 1f);
                EditorGUILayout.Slider(m_mediumRainSoundFx, 0f, 1f);
                EditorGUILayout.Slider(m_heavyRainSoundFx, 0f, 1f);
                EditorGUILayout.Slider(m_lightWindSoundFx, 0f, 1f);
                EditorGUILayout.Slider(m_mediumWindSoundFx, 0f, 1f);
                EditorGUILayout.Slider(m_heavyWindSoundFx, 0f, 1f);
                EditorGUILayout.Slider(m_windSpeed, 0f, 1f);
                EditorGUILayout.Slider(m_windDirection, 0f, 1f);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space(2);

            // Settings header group
            m_controlRect = EditorGUILayout.GetControlRect();
            m_headerRect = new Rect(m_controlRect.x + 15, m_controlRect.y, m_controlRect.width - 20, EditorGUIUtility.singleLineHeight);
            m_thunderSeetingsHeaderGroup.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(m_headerRect, m_thunderSeetingsHeaderGroup.isExpanded, GUIContent.none, "RL Header");
            EditorGUI.Foldout(m_headerRect, m_thunderSeetingsHeaderGroup.isExpanded, GUIContent.none);
            EditorGUI.LabelField(m_headerRect, new GUIContent(" Thunder Settings", ""));
            if (m_thunderSeetingsHeaderGroup.isExpanded)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.Space();
                m_reorderableThunderList.DoLayoutList();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space(2);

            // Update the inspector when there is a change
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}