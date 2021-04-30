using System;
using UnityEngine;
using UnityEditorInternal;
using UnityEngine.AzureSky;

namespace UnityEditor.AzureSky
{
    [CustomEditor(typeof(AzureTimeController))]
    public class AzureTimeControllerInspector : Editor
    {
        // Target
        private AzureTimeController m_target;

        // Logo
        public Texture2D logoTexture;
        private Rect m_controlRect;

        // Internal use
        private int m_buttonGridIndex;
        private bool m_isDaySelected;
        private bool m_showFastDateSelector;
        private int[] m_fastDateSelector = new int[3];
        private int[] m_eventDateSelector = new int[3];
        private int[] m_eventTimeSelector = new int[2];
        private GUIStyle m_headerStyle;

        private readonly GUIContent[] m_dateSelectorContent = new[]
        {
            new GUIContent("Month", ""),
            new GUIContent("Day", ""),
            new GUIContent("Year", "")
        };

        private readonly GUIContent[] m_timeSelectorContent = new[]
        {
            new GUIContent("Hour", ""),
            new GUIContent("Minute", "")
        };

        // Serialized properties
        private SerializedProperty m_sunTransform;
        private SerializedProperty m_moonTransform;
        private SerializedProperty m_directionalLight;
        private SerializedProperty m_followTarget;
        private SerializedProperty m_timeSystem;
        private SerializedProperty m_timeDirection;
        private SerializedProperty m_dateLoop;
        private SerializedProperty m_timeline;
        private SerializedProperty m_hour;
        private SerializedProperty m_minute;
        private SerializedProperty m_day;
        private SerializedProperty m_month;
        private SerializedProperty m_year;
        private SerializedProperty m_selectedCalendarDay;
        private SerializedProperty m_latitude;
        private SerializedProperty m_longitude;
        private SerializedProperty m_utc;
        private SerializedProperty m_dayLength;
        private SerializedProperty m_minLightAltitude;
        private SerializedProperty m_isTimeEvaluatedByCurve;
        private SerializedProperty m_dayLengthCurve;
        private SerializedProperty m_celestialBodiesList;
        private SerializedProperty m_onMinuteChange;
        private SerializedProperty m_onHourChange;
        private SerializedProperty m_onDayChange;
        private SerializedProperty m_customEventScanMode;
        private SerializedProperty m_customEventList;
        private SerializedProperty m_dayNumberList;

        // Reorderable lists
        private ReorderableList m_reorderableCelestialBodiesList;
        private ReorderableList m_reorderableCeustomEventList;

        private void OnEnable()
        {
            // Get target
            m_target = (AzureTimeController) target;
            m_target.UpdateCalendar();

            // Find the serialized properties
            m_sunTransform = serializedObject.FindProperty("m_sunTransform");
            m_moonTransform = serializedObject.FindProperty("m_moonTransform");
            m_directionalLight = serializedObject.FindProperty("m_directionalLight");
            m_followTarget = serializedObject.FindProperty("m_followTarget");
            m_timeSystem = serializedObject.FindProperty("m_timeSystem");
            m_timeDirection = serializedObject.FindProperty("m_timeDirection");
            m_dateLoop = serializedObject.FindProperty("m_dateLoop");
            m_timeline = serializedObject.FindProperty("m_timeline");
            m_hour = serializedObject.FindProperty("m_hour");
            m_minute = serializedObject.FindProperty("m_minute");
            m_day = serializedObject.FindProperty("m_day");
            m_month = serializedObject.FindProperty("m_month");
            m_year = serializedObject.FindProperty("m_year");
            m_selectedCalendarDay = serializedObject.FindProperty("m_selectedCalendarDay");
            m_latitude = serializedObject.FindProperty("m_latitude");
            m_longitude = serializedObject.FindProperty("m_longitude");
            m_utc = serializedObject.FindProperty("m_utc");
            m_dayLength = serializedObject.FindProperty("m_dayLength");
            m_minLightAltitude = serializedObject.FindProperty("m_minLightAltitude");
            m_isTimeEvaluatedByCurve = serializedObject.FindProperty("m_isTimeEvaluatedByCurve");
            m_dayLengthCurve = serializedObject.FindProperty("m_dayLengthCurve");
            m_celestialBodiesList = serializedObject.FindProperty("m_celestialBodiesList");
            m_onMinuteChange = serializedObject.FindProperty("m_onMinuteChange");
            m_onHourChange = serializedObject.FindProperty("m_onHourChange");
            m_onDayChange = serializedObject.FindProperty("m_onDayChange");
            m_customEventScanMode = serializedObject.FindProperty("m_customEventScanMode");
            m_customEventList = serializedObject.FindProperty("m_customEventList");
            m_dayNumberList = serializedObject.FindProperty("m_dayNumberList");

            // Create celestial body list
            m_reorderableCelestialBodiesList = new ReorderableList(serializedObject, m_celestialBodiesList, true, true, true, true)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    rect.y += 2;
                    float half = rect.width / 2;
                    Rect fieldRect = new Rect(rect.x, rect.y, half, EditorGUIUtility.singleLineHeight);

                    var element = m_celestialBodiesList.GetArrayElementAtIndex(index);
                    var transform = element.FindPropertyRelative("transform");
                    var type = element.FindPropertyRelative("type");

                    // Celestial body transform
                    EditorGUI.PropertyField(fieldRect, transform, GUIContent.none);

                    // Celestial body type
                    fieldRect = new Rect(rect.x + half + 5, rect.y, half - 5, EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(fieldRect, type, GUIContent.none);
                },

                drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "Celestial Bodies", EditorStyles.boldLabel);
                },

                drawElementBackgroundCallback = (rect, index, active, focused) =>
                {
                    if (active)
                        GUI.Box(new Rect(rect.x + 2, rect.y, rect.width - 4, rect.height + 1), "", "selectionRect");
                }
            };

            // Create custom event list
            m_reorderableCeustomEventList = new ReorderableList(serializedObject, m_customEventList, true, true, true, true)
            {
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    rect.y += 2;
                    var height = EditorGUIUtility.singleLineHeight;
                    var element = m_customEventList.GetArrayElementAtIndex(index);

                    element.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(new Rect(rect.x + 13, rect.y, rect.width - 17, height), element.isExpanded, "Event " + index.ToString());
                    if (element.isExpanded)
                    {
                        // Date settings
                        rect.y += height + 6;
                        var month = element.FindPropertyRelative("month");
                        var day = element.FindPropertyRelative("day");
                        var year = element.FindPropertyRelative("year");
                        var hour = element.FindPropertyRelative("hour");
                        var minute = element.FindPropertyRelative("minute");

                        m_eventDateSelector[0] = month.intValue;
                        m_eventDateSelector[1] = day.intValue;
                        m_eventDateSelector[2] = year.intValue;
                        EditorGUI.MultiIntField(new Rect(rect.x, rect.y, rect.width, height), m_dateSelectorContent, m_eventDateSelector);
                        month.intValue = m_eventDateSelector[0];
                        day.intValue = m_eventDateSelector[1];
                        year.intValue = m_eventDateSelector[2];

                        // Time settings
                        rect.y += height + 4;
                        m_eventTimeSelector[0] = hour.intValue;
                        m_eventTimeSelector[1] = minute.intValue;
                        EditorGUI.MultiIntField(new Rect(rect.x, rect.y, rect.width, height), m_timeSelectorContent, m_eventTimeSelector);
                        hour.intValue = m_eventTimeSelector[0];
                        minute.intValue = m_eventTimeSelector[1];

                        // UnityEvent list
                        rect.y += height + 4;
                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, height), element.FindPropertyRelative("unityEvent"), new GUIContent("Unity Event", ""));
                    }
                    EditorGUI.EndFoldoutHeaderGroup();
                },

                drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, "Custom Events", EditorStyles.boldLabel);
                },

                elementHeightCallback = (int index) =>
                {
                    var element = m_customEventList.GetArrayElementAtIndex(index);
                    var elementHeight = EditorGUI.GetPropertyHeight(element);
                    var margin = EditorGUIUtility.standardVerticalSpacing + 2;
                    if (element.isExpanded) return elementHeight - 40; else return elementHeight + margin;
                },

                drawElementBackgroundCallback = (rect, index, active, focused) =>
                {
                    if (active)
                        GUI.Box(new Rect(rect.x + 2, rect.y, rect.width - 4, rect.height + 1), "", "selectionRect");
                }
            };
        }

        public override void OnInspectorGUI()
        {
            // Initializations
            m_buttonGridIndex = 0;

            // Start custom inspector
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            // Logo
            m_controlRect = EditorGUILayout.GetControlRect(GUILayout.Height(65));
            EditorGUI.LabelField(m_controlRect, "", "", "selectionRect");
            if (logoTexture) GUI.DrawTexture(new Rect(m_controlRect.x, m_controlRect.y, 261, 56), logoTexture);
            EditorGUILayout.Space(-18);
            GUILayout.Label(" Version 7.0.2", EditorStyles.miniLabel);

            // Calendar header buttons
            EditorGUILayout.BeginHorizontal("box");
            if (GUILayout.Button("<<", EditorStyles.miniButtonLeft, GUILayout.Width(25))) DecreaseYear();
            if (GUILayout.Button("<", EditorStyles.miniButtonMid, GUILayout.Width(25))) DecreaseMonth();
            if (GUILayout.Button(m_target.GetDateString(), EditorStyles.miniButtonMid))
            {
                m_showFastDateSelector = !m_showFastDateSelector;
                m_fastDateSelector[0] = m_month.intValue;
                m_fastDateSelector[1] = m_day.intValue;
                m_fastDateSelector[2] = m_year.intValue;
            }
            if (GUILayout.Button(">", EditorStyles.miniButtonMid, GUILayout.Width(25))) IncreaseMonth();
            if (GUILayout.Button(">>", EditorStyles.miniButtonRight, GUILayout.Width(25))) IncreaseYear();
            EditorGUILayout.EndHorizontal();

            if (m_showFastDateSelector)
            {
                EditorGUILayout.Space(-4);
                EditorGUILayout.BeginVertical("box");
                EditorGUI.MultiIntField(EditorGUILayout.GetControlRect(), m_dateSelectorContent, m_fastDateSelector);
                m_fastDateSelector[0] = Mathf.Clamp(m_fastDateSelector[0], 1, 12);
                m_fastDateSelector[2] = Mathf.Clamp(m_fastDateSelector[2], 0, 9999);
                m_fastDateSelector[1] = Mathf.Min(m_fastDateSelector[1], DateTime.DaysInMonth(m_fastDateSelector[2], m_fastDateSelector[0]));
                if (GUILayout.Button("Go To", EditorStyles.miniButtonMid))
                {
                    Undo.RecordObject(m_target, "Undo Azure Time Controller");
                    m_target.SetDate(m_fastDateSelector[2], m_fastDateSelector[0], m_fastDateSelector[1]);
                    m_target.ForceEditorUpdate();
                    m_showFastDateSelector = false;
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(3);
            }

            // Draws the days of the week strings above the selectable grid
            EditorGUILayout.Space(-5);
            EditorGUILayout.BeginHorizontal("box");
            GUILayout.Label("Sun");
            GUILayout.Label("Mon");
            GUILayout.Label("Tue");
            GUILayout.Label("Wed");
            GUILayout.Label("Thu");
            GUILayout.Label("Fri");
            GUILayout.Label("Sat");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(-5);

            // Creates the calendar selectable grid
            EditorGUILayout.BeginVertical("Box");
            for (int i = 0; i < 6; i++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < 7; j++)
                {
                    m_isDaySelected = m_selectedCalendarDay.intValue == m_buttonGridIndex ? true : false;
                    if (GUILayout.Toggle(m_isDaySelected, m_dayNumberList.GetArrayElementAtIndex(m_buttonGridIndex).stringValue, GUI.skin.button, GUILayout.MinWidth(30)))
                    {
                        if (m_dayNumberList.GetArrayElementAtIndex(m_buttonGridIndex).stringValue != "")
                            m_selectedCalendarDay.intValue = m_buttonGridIndex;
                    }

                    m_buttonGridIndex++;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            // References
            EditorGUILayout.PropertyField(m_sunTransform);
            EditorGUILayout.PropertyField(m_moonTransform);
            EditorGUILayout.PropertyField(m_directionalLight);
            EditorGUILayout.PropertyField(m_followTarget);
            //EditorGUILayout.Space();

            // Options
            EditorGUILayout.PropertyField(m_timeSystem);
            EditorGUILayout.PropertyField(m_timeDirection);
            EditorGUILayout.PropertyField(m_dateLoop);

            // Sliders
            EditorGUILayout.Slider(m_timeline, 0.0f, 24.0f);
            EditorGUILayout.Slider(m_latitude, -90.0f, 90.0f);
            EditorGUILayout.Slider(m_longitude, -180.0f, 180.0f);
            EditorGUILayout.Slider(m_utc, -12.0f, 12.0f);
            EditorGUILayout.PropertyField(m_dayLength);
            EditorGUILayout.PropertyField(m_minLightAltitude);

            // Day-Night length curve
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.Space(-5);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            GUILayout.Label("Day and Night Length", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            // Toggle
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Evaluate Time of Day by Curve?", "Will the 'time of day' be evaluated based on the timeline or based on the day-night length curve?"));
            EditorGUILayout.PropertyField(m_isTimeEvaluatedByCurve, GUIContent.none, GUILayout.Width(15));
            EditorGUILayout.EndHorizontal();

            // Reset Button
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("R", GUILayout.Width(25), GUILayout.Height(25)))
            {
                Undo.RecordObject(m_target, "Undo Azure Time Controller");
                m_dayLengthCurve.animationCurveValue = AnimationCurve.Linear(0, 0, 24, 24);
            }

            // Curve field
            EditorGUILayout.CurveField(m_dayLengthCurve, Color.yellow, new Rect(0, 0, 24, 24), GUIContent.none, GUILayout.Height(25));
            EditorGUILayout.EndHorizontal();

            // Time of day display
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Current Time of Day:", "Displays the current 'time of day' based on the 'time position' of the day-night cycle"));
            GUILayout.Label(m_target.GetDayOfWeekString() + " " + m_hour.intValue.ToString("00") + ":" + m_minute.intValue.ToString("00"),GUILayout.ExpandWidth(false));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            // Draw the celestial bodies reorderable lists
            if (m_timeSystem.enumValueIndex == 1)
                m_reorderableCelestialBodiesList.DoLayoutList();

            // Events
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_onMinuteChange);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_onHourChange);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_onDayChange);
            EditorGUILayout.Space(10);

            // Custom event list header
            EditorGUILayout.BeginHorizontal("RL Header");
            m_controlRect = EditorGUILayout.GetControlRect();
            EditorGUI.LabelField(new Rect(m_controlRect.x + 2, m_controlRect.y, m_controlRect.width, m_controlRect.height), "Custom Event List", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
            m_controlRect = EditorGUILayout.GetControlRect(GUILayout.Height(25));

            // Draw the custom event list
            EditorGUILayout.Space(-25);
            m_reorderableCeustomEventList.DoLayoutList();
            EditorGUILayout.Space(25);

            // Custom event scan mode
            EditorGUI.LabelField(new Rect(m_controlRect.x, m_controlRect.y - 2, m_controlRect.width, m_controlRect.height), "", "", "RL Background");
            EditorGUI.LabelField(new Rect(m_controlRect.x + 6, m_controlRect.y + 8, 10, m_controlRect.height), "", "", "RL DragHandle");
            //EditorGUI.LabelField(new Rect(m_controlRect.x + 19, m_controlRect.y + 1, m_controlRect.width - 30, EditorGUIUtility.singleLineHeight), "Scan Mode");
            EditorGUI.PropertyField(new Rect(m_controlRect.x + 5, m_controlRect.y + 1, m_controlRect.width - 10, m_controlRect.height), m_customEventScanMode, new GUIContent("Scan Mode", ""));

            // Update the inspector when there is a change
            if (EditorGUI.EndChangeCheck())
            {
                m_day.intValue = m_selectedCalendarDay.intValue + 1 - m_target.GetDayOfWeek(m_year.intValue, m_month.intValue, 1);
                serializedObject.ApplyModifiedProperties();
                m_target.UpdateCalendar();
                //m_target.ComputeCelestialCoordinates();
            }

            // Updates the calendar if the undo command is performed
            if (Event.current.commandName == "UndoRedoPerformed")
            {
                m_target.UpdateCalendar();
                m_target.ForceEditorUpdate();
            }
        }

        private void DecreaseMonth()
        {
            Undo.RecordObject(m_target, "Undo Azure Time Controller");
            m_target.DecreaseMonth();
            m_target.ForceEditorUpdate();
        }
        
        private void IncreaseMonth()
        {
            Undo.RecordObject(m_target, "Undo Azure Time Controller");
            m_target.IncreaseMonth();
            m_target.ForceEditorUpdate();
        }
        
        private void DecreaseYear()
        {
            Undo.RecordObject(m_target, "Undo Azure Time Controller");
            m_target.DecreaseYear();
            m_target.ForceEditorUpdate();
        }
        
        private void IncreaseYear()
        {
            Undo.RecordObject(m_target, "Undo Azure Time Controller");
            m_target.IncreaseYear();
            m_target.ForceEditorUpdate();
        }
    }
}