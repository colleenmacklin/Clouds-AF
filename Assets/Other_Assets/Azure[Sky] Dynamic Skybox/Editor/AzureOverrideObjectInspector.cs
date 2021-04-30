using UnityEngine;
using UnityEngine.AzureSky;
using UnityEditorInternal;
using System.Collections.Generic;

namespace UnityEditor.AzureSky
{
    [CustomEditor(typeof(AzureOverrideObject))]
    public class AzureOverrideObjectInspector : Editor
    {
        // Editor only
        private AzureOverrideObject m_target;
        private SerializedProperty m_customPropertyList;
        private ReorderableList m_reorderableList;

        // Logo
        public Texture2D logoTexture;
        private Rect m_controlRect;

        // GUIContents
        private readonly GUIContent[] m_guiContent = new[]
        {
            new GUIContent("Custom Properties", "The custom properties you want to use with the Override Controller component.")
        };

        private void OnEnable()
        {
            m_customPropertyList = serializedObject.FindProperty("customPropertyList");

            m_reorderableList = new ReorderableList(serializedObject, m_customPropertyList, false, true, true, true)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    rect.y += 2;
                    float height = EditorGUIUtility.singleLineHeight;
                    float half = rect.width / 2;

                    var element = m_customPropertyList.GetArrayElementAtIndex(index);
                    var name = element.FindPropertyRelative("name");
                    var description = element.FindPropertyRelative("description");
                    var outputType = element.FindPropertyRelative("outputType");
                    var floatPropertyType = element.FindPropertyRelative("floatPropertyType");
                    var colorPropertyType = element.FindPropertyRelative("colorPropertyType");

                    Rect fieldRect = new Rect(rect.x + 15, rect.y, rect.width - 18, height);
                    element.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(fieldRect, element.isExpanded, name.stringValue);
                    if (element.isExpanded)
                    {
                        // Output type
                        fieldRect = new Rect(rect.x, rect.y + height + 2, rect.width, height);
                        EditorGUI.PropertyField(fieldRect, outputType);

                        // Property type
                        fieldRect = new Rect(rect.x, rect.y + height * 2 + 4, rect.width, height);
                        switch (outputType.enumValueIndex)
                        {
                            case 0:
                                EditorGUI.PropertyField(fieldRect, floatPropertyType, new GUIContent("Customization Mode", ""));
                                break;
                            case 1:
                                EditorGUI.PropertyField(fieldRect, colorPropertyType, new GUIContent("Customization Mode", ""));
                                break;
                        }

                        // Name
                        fieldRect = new Rect(rect.x, rect.y + height * 3 + 6, rect.width, height);
                        if (name.stringValue == "") name.stringValue = "Custom Name";
                        name.stringValue = EditorGUI.DelayedTextField(fieldRect, "Property Name", name.stringValue);

                        // Description
                        fieldRect = new Rect(rect.x, rect.y + height * 4 + 8, rect.width, height * 2.7f);
                        if (description.stringValue == "") description.stringValue = "Description:";
                        description.stringValue = EditorGUI.TextArea(fieldRect, description.stringValue);
                    }
                    EditorGUILayout.EndFoldoutHeaderGroup();
                },

                onAddCallback = (ReorderableList l) =>
                {
                    ReorderableList.defaultBehaviours.DoAddButton(l);
                    var element = m_customPropertyList.GetArrayElementAtIndex(l.index);
                    element.FindPropertyRelative("outputType").enumValueIndex = 0;
                    element.FindPropertyRelative("floatPropertyType").enumValueIndex = 0;
                    element.FindPropertyRelative("colorPropertyType").enumValueIndex = 0;
                    element.FindPropertyRelative("name").stringValue = "My Name...";
                    element.FindPropertyRelative("description").stringValue = "Description:";
                    element.isExpanded = true;

                    // Find all weather profiles in the project and resize their property list
                    List<AzureWeatherProfile> weathers = FindAssetsByType<AzureWeatherProfile>();
                    foreach (var weather in weathers)
                    {
                        if (weather.overrideObject == m_target)
                        {
                            weather.ResizeList(l.serializedProperty.arraySize);
                            EditorUtility.SetDirty(weather);
                        }
                    }
                },

                onRemoveCallback = (ReorderableList l) =>
                {
                    ReorderableList.defaultBehaviours.DoRemoveButton(l);

                    // Find all weather profiles in the project and resize their property list
                    List<AzureWeatherProfile> weathers = FindAssetsByType<AzureWeatherProfile>();
                    foreach (var weather in weathers)
                    {
                        if (weather.overrideObject == m_target)
                        {
                            weather.ResizeList(l.serializedProperty.arraySize);
                            EditorUtility.SetDirty(weather);
                        }
                    }
                },

                drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, m_guiContent[0], EditorStyles.boldLabel);
                },

                elementHeightCallback = (int index) =>
                {
                    var element = m_customPropertyList.GetArrayElementAtIndex(index);
                    var elementHeight = EditorGUI.GetPropertyHeight(element);
                    var margin = EditorGUIUtility.standardVerticalSpacing + 4f;
                    if (element.isExpanded) return elementHeight + 16f; else return elementHeight + margin;
                },

                drawElementBackgroundCallback = (rect, index, active, focused) =>
                {
                    if (active)
                        GUI.Box(new Rect(rect.x + 2, rect.y - 1, rect.width - 4, rect.height), "", "selectionRect");
                }
            };
        }

        public override void OnInspectorGUI()
        {
            // Get target
            m_target = (AzureOverrideObject)target;

            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            // Logo
            m_controlRect = EditorGUILayout.GetControlRect(GUILayout.Height(65));
            EditorGUI.LabelField(m_controlRect, "", "", "selectionRect");
            if (logoTexture) GUI.DrawTexture(new Rect(m_controlRect.x, m_controlRect.y, 261, 56), logoTexture);

            m_reorderableList.DoLayoutList();

            // Update the inspector when there is a change
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();

                // Find all weather profiles in the project and update the property type
                //List<AzureWeatherProfile> weathers = FindAssetsByType<AzureWeatherProfile>();
                //foreach (var weather in weathers)
                //{
                //    //if (weather.profilePropertyList.Count != m_customPropertyList.arraySize)
                //    //{
                //    //    ResizeList(weather.profilePropertyList, m_customPropertyList.arraySize);
                //    //}
                //
                //    //Debug.Log(m_customPropertyList.arraySize);
                //    for (int i = 0; i < weather.profilePropertyList.Count; i++)
                //    {
                //        //Debug.Log(weather.name+ " " + weather.profilePropertyList[i].floatPropertyType);
                //        //weather.profilePropertyList[i].floatPropertyType = 0;
                //        //weather.profilePropertyList[i].colorPropertyType = 0;
                //        //Debug.Log(m_customPropertyList.arraySize);
                //        //Debug.Log(weather.profilePropertyList.Count);
                //        weather.profilePropertyList[i].floatPropertyType = (AzureOutputFloatType)m_customPropertyList.GetArrayElementAtIndex(i).FindPropertyRelative("floatPropertyType").enumValueIndex;
                //        weather.profilePropertyList[i].colorPropertyType = (AzureOutputColorType)m_customPropertyList.GetArrayElementAtIndex(i).FindPropertyRelative("colorPropertyType").enumValueIndex;
                //    }
                //}
            }

            // If the undo command is performed
            if (Event.current.commandName == "UndoRedoPerformed")
            {
                // Find all weather profiles in the project and resize their property list
                List<AzureWeatherProfile> weathers = FindAssetsByType<AzureWeatherProfile>();
                foreach (var weather in weathers)
                {
                    if (weather.overrideObject == m_target)
                    {
                        weather.ResizeList(m_reorderableList.serializedProperty.arraySize);
                        EditorUtility.SetDirty(weather);
                    }
                }
            }
        }

        /// <summary>
        /// Find any asset type in the Editor.
        /// </summary>
        public List<T> FindAssetsByType<T>() where T : UnityEngine.Object
        {
            List<T> assets = new List<T>();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }
            return assets;
        }

        /// <summary>
        /// Resize a list.
        /// </summary>
        //public void ResizeList<T>(List<T> list, int newCount)
        //{
        //    if (newCount <= 0)
        //    {
        //        list.Clear();
        //    }
        //    else
        //    {
        //        while (list.Count > newCount) list.RemoveAt(list.Count - 1);
        //        while (list.Count < newCount) list.Add(default(T));
        //    }
        //}
    }
}