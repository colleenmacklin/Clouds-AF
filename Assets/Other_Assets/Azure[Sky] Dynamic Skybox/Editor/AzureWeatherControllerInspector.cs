using System;
using UnityEngine;
using UnityEngine.AzureSky;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Reflection;

namespace UnityEditor.AzureSky
{
    [CustomEditor(typeof(AzureWeatherController))]
    public class AzureWeatherControllerInspector : Editor
    {
        // Editor only
        private AzureWeatherController m_target;
        private static AzureWeatherControllerInspector instance;
        private Rect m_controlRect;
        private Rect m_headerRect;
        private GUIStyle m_textBarStyle;
        private readonly Color m_greenColor = new Color(0.4f, 1.0f, 0.4f);
        private readonly Color m_redColor = new Color(1.0f, 0.4f, 0.4f);

        // Serialized properties
        private SerializedProperty m_azureTimeController;
        private SerializedProperty m_defaultWeatherTransitionLength;
        private SerializedProperty m_defaultWeatherProfilesList;
        private SerializedProperty m_weatherTransitionProgress;
        private SerializedProperty m_globalWeatherProfilesList;
        private SerializedProperty m_localWeatherZoneTrigger;
        private SerializedProperty m_localWeatherZonesList;
        private SerializedProperty m_overrideObject;
        private SerializedProperty m_overridePropertyList;
        private SerializedProperty m_timeOfDay;
        private SerializedProperty m_sunElevation;
        private SerializedProperty m_moonElevation;
        private ReorderableList m_defaultWeatherProfilesReorderableList;
        private ReorderableList m_globalWeatherProfilesReorderableList;
        private ReorderableList m_localWeatherZonesReorderableList;
        private ReorderableList m_overridePropertyReorderableList;
        private Dictionary<string, ReorderableList> m_innerListDict = new Dictionary<string, ReorderableList>();

        private void OnEnable()
        {
            // Get target
            m_target = (AzureWeatherController)target;
            m_target.RefreshOverrideTargets();
            instance = this;

            // Get serialized properties
            m_azureTimeController = serializedObject.FindProperty("m_azureTimeController");
            m_defaultWeatherTransitionLength = serializedObject.FindProperty("m_defaultWeatherTransitionLength");
            m_defaultWeatherProfilesList = serializedObject.FindProperty("m_defaultWeatherProfilesList");
            m_weatherTransitionProgress = serializedObject.FindProperty("m_weatherTransitionProgress");
            m_globalWeatherProfilesList = serializedObject.FindProperty("m_globalWeatherProfilesList");
            m_localWeatherZoneTrigger = serializedObject.FindProperty("m_localWeatherZoneTrigger");
            m_localWeatherZonesList = serializedObject.FindProperty("m_localWeatherZonesList");
            m_overrideObject = serializedObject.FindProperty("overrideObject");
            m_overridePropertyList = serializedObject.FindProperty("m_overridePropertyList");
            m_timeOfDay = serializedObject.FindProperty("m_timeOfDay");
            m_sunElevation = serializedObject.FindProperty("m_sunElevation");
            m_moonElevation = serializedObject.FindProperty("m_moonElevation");

            // Create the default weather reorderable lists
            m_defaultWeatherProfilesReorderableList = new ReorderableList(serializedObject, m_defaultWeatherProfilesList, true, true, true, true)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    rect.y += 2;
                    Rect fieldRect = new Rect(rect.x + 65, rect.y, rect.width - 65, EditorGUIUtility.singleLineHeight);

                    // Profile index
                    EditorGUI.LabelField(rect, "Profile " + index.ToString());

                    // Object field
                    var element = m_defaultWeatherProfilesList.GetArrayElementAtIndex(index);
                    EditorGUI.PropertyField(fieldRect, element, GUIContent.none);
                },

                drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "Default Weather Profiles", EditorStyles.boldLabel);
                },

                drawElementBackgroundCallback = (rect, index, active, focused) =>
                {
                    if (active)
                        GUI.Box(new Rect(rect.x + 2, rect.y, rect.width - 4, rect.height + 1), "", "selectionRect");
                }
            };

            // Create the global weather reorderable lists
            m_globalWeatherProfilesReorderableList = new ReorderableList(serializedObject, m_globalWeatherProfilesList, true, true, true, true)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    rect.y += 2;
                    Rect fieldRect = new Rect(rect.x + 65, rect.y, rect.width - 131, EditorGUIUtility.singleLineHeight);
                    var element = m_globalWeatherProfilesList.GetArrayElementAtIndex(index);
                    var profile = element.FindPropertyRelative("profile");
                    var transition = element.FindPropertyRelative("transitionLength");

                    // Profile index
                    EditorGUI.LabelField(rect, "Profile  " + index.ToString());

                    // Object field
                    EditorGUI.PropertyField(fieldRect, profile, GUIContent.none);

                    // Transition time field
                    fieldRect = new Rect(rect.width - 25, rect.y, 32, EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(fieldRect, transition, GUIContent.none);

                    // Go button
                    fieldRect = new Rect(rect.x + rect.width - 30, rect.y, 30, EditorGUIUtility.singleLineHeight);
                    if (GUI.Button(fieldRect, "Go"))
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
                    ReorderableList.defaultBehaviours.DoAddButton(l);

                    var element = l.serializedProperty.GetArrayElementAtIndex(l.index);
                    element.FindPropertyRelative("transitionLength").floatValue = 10.0f;
                },

                drawElementBackgroundCallback = (rect, index, active, focused) =>
                {
                    if (active)
                        GUI.Box(new Rect(rect.x + 2, rect.y, rect.width - 4, rect.height + 1), "", "selectionRect");
                }
            };

            // Create the local weather zones list
            m_localWeatherZonesReorderableList = new ReorderableList(serializedObject, m_localWeatherZonesList, true, true, true, true)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    rect.y += 2;
                    Rect fieldRect = new Rect(rect.x + 65, rect.y, rect.width - 65, EditorGUIUtility.singleLineHeight);

                    // Profile index
                    EditorGUI.LabelField(rect, "Priority " + index.ToString());

                    // Object field
                    EditorGUI.PropertyField(fieldRect, m_localWeatherZonesList.GetArrayElementAtIndex(index), GUIContent.none);
                },

                drawElementBackgroundCallback = (rect, index, active, focused) =>
                {
                    if (active)
                        GUI.Box(new Rect(rect.x + 2, rect.y, rect.width - 4, rect.height + 1), "", "selectionRect");
                }
            };

            // Create the override properties reorderable lists
            m_overridePropertyReorderableList = new ReorderableList(serializedObject, m_overridePropertyList, false, true, false, false)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    float height = EditorGUIUtility.singleLineHeight;
                    Rect fieldRect = new Rect(rect.x + 15, rect.y, rect.width - 18, height);
                    var element = m_overridePropertyList.GetArrayElementAtIndex(index);

                    var name = element.FindPropertyRelative("name");
                    var description = element.FindPropertyRelative("description");
                    name.stringValue = m_target.overrideObject.customPropertyList[index].name;
                    description.stringValue = m_target.overrideObject.customPropertyList[index].description;

                    // Create inner reorderable list
                    var innerList = element.FindPropertyRelative("overridePropertySetupList");
                    string listKey = element.propertyPath;
                    ReorderableList innerReorderableList;

                    if (m_innerListDict.ContainsKey(listKey))
                    {
                        innerReorderableList = m_innerListDict[listKey];
                    }
                    else
                    {
                        innerReorderableList = new ReorderableList(element.serializedObject, innerList, true, true, true, true)
                        {
                            drawElementCallback = (Rect innerRect, int innerIndex, bool innerIsActive, bool innerIsFocused) =>
                            {
                                if (element.isExpanded)
                                {
                                    float half = innerRect.width / 2;
                                    var innerElement = innerList.GetArrayElementAtIndex(innerIndex);

                                    // Override type
                                    var targetType = innerElement.FindPropertyRelative("targetType");
                                    fieldRect = new Rect(innerRect.x, innerRect.y, innerRect.width, height);
                                    EditorGUI.LabelField(fieldRect, "Target Type");
                                    fieldRect = new Rect(innerRect.x + half, innerRect.y, innerRect.width - half, height);
                                    EditorGUI.PropertyField(fieldRect, targetType, GUIContent.none);

                                    if (targetType.enumValueIndex == 0 || targetType.enumValueIndex == 1)
                                    {
                                        // Object field
                                        var gameObject = innerElement.FindPropertyRelative("targetGameObject");
                                        fieldRect = new Rect(innerRect.x, innerRect.y + height + 2f, innerRect.width, height);
                                        EditorGUI.LabelField(fieldRect, "Game Object");
                                        GUI.color = gameObject.objectReferenceValue ? m_greenColor : m_redColor;
                                        fieldRect = new Rect(innerRect.x + half, innerRect.y + height + 2f, innerRect.width - half, height);
                                        EditorGUI.PropertyField(fieldRect, gameObject, GUIContent.none);
                                        GUI.color = Color.white;

                                        // Component name field
                                        var targetComponent = innerElement.FindPropertyRelative("targetComponent");
                                        var componentName = innerElement.FindPropertyRelative("targetComponentName");
                                        fieldRect = new Rect(innerRect.x, innerRect.y + height * 2f + 4f, innerRect.width, height);
                                        EditorGUI.LabelField(fieldRect, "Component Name");
                                        GUI.color = gameObject.objectReferenceValue && targetComponent.objectReferenceValue ? m_greenColor : m_redColor;
                                        fieldRect = new Rect(innerRect.x + half, innerRect.y + height * 2f + 4f, innerRect.width - half, height);
                                        componentName.stringValue = EditorGUI.DelayedTextField(fieldRect, GUIContent.none, componentName.stringValue);
                                        GUI.color = Color.white;

                                        // Property name field
                                        var propertyName = innerElement.FindPropertyRelative("targetPropertyName");
                                        var targetField = innerElement.FindPropertyRelative("targetField");
                                        var targetProperty = innerElement.FindPropertyRelative("targetProperty");
                                        fieldRect = new Rect(innerRect.x, innerRect.y + height * 3f + 6f, innerRect.width, height);
                                        EditorGUI.LabelField(fieldRect, "Property Name");
                                        GUI.color = m_greenColor;
                                        if (targetType.enumValueIndex == 0 && m_target.TargetHasField(index, innerIndex))
                                            GUI.color = m_redColor;
                                        if (targetType.enumValueIndex == 1 && m_target.TargetHasProperty(index, innerIndex))
                                            GUI.color = m_redColor;
                                        fieldRect = new Rect(innerRect.x + half, innerRect.y + height * 3f + 6f, innerRect.width - half, height);
                                        propertyName.stringValue = EditorGUI.DelayedTextField(fieldRect, GUIContent.none, propertyName.stringValue);
                                        GUI.color = Color.white;
                                    }

                                    if (targetType.enumValueIndex == 2)
                                    {
                                        // Shader update mode
                                        var targetShaderUpdateMode = innerElement.FindPropertyRelative("targetShaderUpdateMode");
                                        fieldRect = new Rect(innerRect.x, innerRect.y + height + 2f, innerRect.width, height);
                                        EditorGUI.LabelField(fieldRect, "Shader Update Mode");
                                        fieldRect = new Rect(innerRect.x + half, innerRect.y + height + 2f, innerRect.width - half, height);
                                        EditorGUI.PropertyField(fieldRect, targetShaderUpdateMode, GUIContent.none);

                                        if (targetShaderUpdateMode.enumValueIndex == 0)
                                        {
                                            // Material field
                                            var material = innerElement.FindPropertyRelative("targetMaterial");
                                            fieldRect = new Rect(innerRect.x, innerRect.y + height * 2f + 4f, innerRect.width, height);
                                            EditorGUI.LabelField(fieldRect, "Material");
                                            GUI.color = material.objectReferenceValue ? m_greenColor : m_redColor;
                                            fieldRect = new Rect(innerRect.x + half, innerRect.y + height * 2f + 4f, innerRect.width - half, height);
                                            EditorGUI.PropertyField(fieldRect, material, GUIContent.none);
                                            GUI.color = Color.white;
                                        }
                                        else
                                        {
                                            fieldRect = new Rect(innerRect.x, innerRect.y + height * 2f + 4f, innerRect.width, height);
                                            EditorGUI.LabelField(fieldRect, "Setting a global value to all shaders using this property!", EditorStyles.helpBox);
                                        }

                                        // Shader uniform name field
                                        var propertyName = innerElement.FindPropertyRelative("targetPropertyName");
                                        fieldRect = new Rect(innerRect.x, innerRect.y + height * 3f + 6f, innerRect.width, height);
                                        EditorGUI.LabelField(fieldRect, "Property Name");
                                        fieldRect = new Rect(innerRect.x + half, innerRect.y + height * 3f + 6f, innerRect.width - half, height);
                                        propertyName.stringValue = EditorGUI.DelayedTextField(fieldRect, GUIContent.none, propertyName.stringValue);

                                        var targetUniformID = innerElement.FindPropertyRelative("targetUniformID");
                                        targetUniformID.intValue = Shader.PropertyToID(propertyName.stringValue);

                                    }

                                    // Multiplier field
                                    var multiplier = innerElement.FindPropertyRelative("multiplier");
                                    fieldRect = new Rect(innerRect.x, innerRect.y + height * 4f + 8f, innerRect.width, height);
                                    EditorGUI.LabelField(fieldRect, "Multiplier");
                                    fieldRect = new Rect(innerRect.x + half, innerRect.y + height * 4f + 8f, innerRect.width - half, height);
                                    EditorGUI.PropertyField(fieldRect, multiplier, GUIContent.none);
                                }
                            },

                            onAddCallback = (ReorderableList l) =>
                            {
                                ReorderableList.defaultBehaviours.DoAddButton(l);
                                var innerElement = l.serializedProperty.GetArrayElementAtIndex(l.index);
                                innerElement.FindPropertyRelative("multiplier").floatValue = 1.0f;
                            },

                            drawHeaderCallback = (Rect innerRect) =>
                            {
                                Rect r = new Rect(innerRect.x + 8, innerRect.y - 1, innerRect.width - 6, innerRect.height + 2);
                                element.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(r, element.isExpanded, GUIContent.none, "RL Header");
                                EditorGUI.EndFoldoutHeaderGroup();
                                EditorGUI.Foldout(r, element.isExpanded, GUIContent.none);
                                EditorGUI.LabelField(r, new GUIContent(" " + index + " - " + name.stringValue, description.stringValue));
                            },

                            elementHeightCallback = (int innerIndex) => { return EditorGUIUtility.singleLineHeight * 6; },

                            drawElementBackgroundCallback = (innerRect, innerIndex, innerIsActive, innerIsFocused) =>
                            {
                                if (innerIsActive && element.isExpanded)
                                    GUI.Box(new Rect(innerRect.x + 2, innerRect.y - 3, innerRect.width - 4, innerRect.height), "", "selectionRect");
                            }
                        };
                    }

                    m_innerListDict[listKey] = innerReorderableList;

                    if (element.isExpanded)
                        innerReorderableList.DoList(rect);
                    else
                    {
                        Rect r = new Rect(rect.x + 14, rect.y, rect.width - 18, rect.height);
                        element.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(r, element.isExpanded, GUIContent.none, "RL Header");
                        EditorGUI.EndFoldoutHeaderGroup();
                        r.y -= 2;
                        EditorGUI.Foldout(r, element.isExpanded, GUIContent.none);
                        EditorGUI.LabelField(r, new GUIContent(" " + index + " - " + name.stringValue, description.stringValue));
                    }
                },

                elementHeightCallback = (int index) =>
                {
                    var element = m_overridePropertyReorderableList.serializedProperty.GetArrayElementAtIndex(index);
                    var margin = EditorGUIUtility.standardVerticalSpacing + 3f;
                    var innerList = element.FindPropertyRelative("overridePropertySetupList");
                    float h = EditorGUIUtility.singleLineHeight;
                    if (element.isExpanded) return Mathf.Max(100, (h * 6) * (innerList.arraySize + 1) - 50); else return h + margin;
                },

                drawElementBackgroundCallback = (rect, index, active, focused) => { }
            };
        }

        public override void OnInspectorGUI()
        {
            m_textBarStyle = new GUIStyle("WhiteMiniLabel")
            {
                alignment = TextAnchor.MiddleCenter,
                contentOffset = new Vector2(0, -3)
            };

            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            // Azure Time Controller
            //EditorGUILayout.PropertyField(m_azureTimeController);
            if (!m_azureTimeController.objectReferenceValue)
            {
                EditorGUILayout.Slider(m_timeOfDay, 0f, 24f);
                EditorGUILayout.Slider(m_sunElevation, -1f, 1f);
                EditorGUILayout.Slider(m_moonElevation, -1f, 1f);
                EditorGUILayout.HelpBox("Azure Time Controller component was not found, add this component to the game object or the evaluation of the above sliders should be done by scripting.", MessageType.Info);
            }

            // Draw the default weather list
            m_defaultWeatherProfilesReorderableList.DoLayoutList();
            EditorGUILayout.Space(20);

            // Progress bar
            m_controlRect = EditorGUILayout.GetControlRect();
            EditorGUI.ProgressBar(new Rect(m_controlRect.x + 1, m_controlRect.y + 5, m_controlRect.width - 2, m_controlRect.height - 5), m_weatherTransitionProgress.floatValue, "");
            EditorGUI.LabelField(new Rect(m_controlRect.x, m_controlRect.y + 4, m_controlRect.width, m_controlRect.height), "Transition Progress", m_textBarStyle);

            // Custom local weather zones list header
            EditorGUILayout.BeginHorizontal("RL Header");
            m_controlRect = EditorGUILayout.GetControlRect();
            EditorGUI.LabelField(new Rect(m_controlRect.x + 2, m_controlRect.y, m_controlRect.width, m_controlRect.height), "Global Weather Profiles", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
            m_controlRect = EditorGUILayout.GetControlRect(GUILayout.Height(25));

            // Draw the global weather list
            EditorGUILayout.Space(-25);
            m_globalWeatherProfilesReorderableList.DoLayoutList();
            EditorGUILayout.Space(25);

            // Default profile field
            EditorGUI.LabelField(new Rect(m_controlRect.x, m_controlRect.y - 2, m_controlRect.width, m_controlRect.height), "", "", "RL Background");
            EditorGUI.LabelField(new Rect(m_controlRect.x + 6, m_controlRect.y + 8, 10, m_controlRect.height), "", "", "RL DragHandle");
            EditorGUI.LabelField(new Rect(m_controlRect.x + 19, m_controlRect.y + 1, m_controlRect.width - 30, EditorGUIUtility.singleLineHeight), "Profile -1");
            EditorGUI.LabelField(new Rect(m_controlRect.x + 85, m_controlRect.y + 1, m_controlRect.width - 158, EditorGUIUtility.singleLineHeight), "@Default", EditorStyles.objectField);
            EditorGUI.PropertyField(new Rect(m_controlRect.width - 53, m_controlRect.y + 1, 32, EditorGUIUtility.singleLineHeight), m_defaultWeatherTransitionLength, GUIContent.none);
            // Go button
            if (GUI.Button(new Rect(m_controlRect.width - 19, m_controlRect.y + 1, 30, EditorGUIUtility.singleLineHeight), "Go"))
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

            // Custom local weather zones list header
            EditorGUILayout.BeginHorizontal("RL Header");
            m_controlRect = EditorGUILayout.GetControlRect();
            EditorGUI.LabelField(new Rect(m_controlRect.x + 2, m_controlRect.y, m_controlRect.width, m_controlRect.height), "Local Weather Zones", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
            m_controlRect = EditorGUILayout.GetControlRect(GUILayout.Height(25));

            // Draw the local weather zones list
            EditorGUILayout.Space(-25);
            m_localWeatherZonesReorderableList.DoLayoutList();
            EditorGUILayout.Space(25);

            // Custom trigger object field
            EditorGUI.LabelField(new Rect(m_controlRect.x, m_controlRect.y - 2, m_controlRect.width, m_controlRect.height), "", "", "RL Background");
            EditorGUI.LabelField(new Rect(m_controlRect.x + 6, m_controlRect.y + 8, 10, m_controlRect.height), "", "", "RL DragHandle");
            EditorGUI.LabelField(new Rect(m_controlRect.x + 19, m_controlRect.y + 1, m_controlRect.width - 30, EditorGUIUtility.singleLineHeight), "Trigger");
            EditorGUI.PropertyField(new Rect(m_controlRect.x + 85, m_controlRect.y + 1, m_controlRect.width - 93, EditorGUIUtility.singleLineHeight), m_localWeatherZoneTrigger, GUIContent.none);

            // Custom override properties list header
            m_controlRect = EditorGUILayout.GetControlRect();
            if (GUI.Button(new Rect(m_controlRect.x, m_controlRect.y - 2, m_controlRect.width, m_controlRect.height), GUIContent.none, "RL Header"))
                m_overridePropertyList.isExpanded = !m_overridePropertyList.isExpanded;
            m_overridePropertyList.isExpanded = EditorGUI.Foldout(new Rect(m_controlRect.x + 18, m_controlRect.y, m_controlRect.width, m_controlRect.height), m_overridePropertyList.isExpanded, "");
            EditorGUI.LabelField(new Rect(m_controlRect.x + 20, m_controlRect.y, m_controlRect.width, m_controlRect.height), "Override Properties", EditorStyles.boldLabel);
            
            // Draw the override properties list
            if (m_overridePropertyList.isExpanded)
            {
                m_controlRect = EditorGUILayout.GetControlRect(GUILayout.Height(25));
                if (m_target.overrideObject)
                {
                    EditorGUILayout.Space(-25);
                    m_overridePropertyReorderableList.serializedProperty.arraySize = m_target.overrideObject.customPropertyList.Count;
                    m_overridePropertyReorderableList.DoLayoutList();
                }
                else
                {
                    EditorGUILayout.HelpBox("There is no Override Object attached, please add an Override Object to customize the override properties.", MessageType.Error);
                }

                // Custom override object field
                EditorGUI.LabelField(new Rect(m_controlRect.x, m_controlRect.y - 2, m_controlRect.width, m_controlRect.height), "", "", "RL Background");
                EditorGUI.PropertyField(new Rect(m_controlRect.x + 5, m_controlRect.y + 1, m_controlRect.width - 12, EditorGUIUtility.singleLineHeight), m_overrideObject);
            }

            // Update the inspector when there is a change
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                m_target.RefreshOverrideTargets();
            }

            // Refresh the override targets if the undo command is performed
            if (Event.current.commandName == "UndoRedoPerformed")
            {
                m_target.RefreshOverrideTargets();
            }
        }

        /// <summary>
        /// FieldInfo and PropertyInfo data doesn't survive the script reload, so we need to reference it again right after the scripts finish recompiling.
        /// </summary>
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            if (instance)
            {
                instance.m_target.RefreshOverrideTargets();
            }
            else
            {
                var foundTargets = FindObjectsOfType<AzureWeatherController>();
                for (int i = 0; i < foundTargets.Length; i++)
                {
                    foundTargets[i].RefreshOverrideTargets();
                }
            }
        }
    }
}