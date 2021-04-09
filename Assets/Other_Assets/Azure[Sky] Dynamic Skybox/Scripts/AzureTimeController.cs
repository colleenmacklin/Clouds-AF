using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace UnityEngine.AzureSky
{
    [ExecuteInEditMode]
    [AddComponentMenu("Azure[Sky]/Azure Time Controller")]
    public class AzureTimeController : MonoBehaviour
    {
        // Serialized Fields
        [Tooltip("The Transform used to simulate the sun position in the sky.")]
        [SerializeField] private Transform m_sunTransform = null;
        
        [Tooltip("The Transform used to simulate the moon position in the sky.")]
        [SerializeField] private Transform m_moonTransform = null;

        [Tooltip("The directional light used to apply the sun and moon lighting to the scene.")]
        [SerializeField] private Transform m_directionalLight = null;

        [Tooltip("If selected, the prefab will follow this transform in the scene, usually the main camera is used here.")]
        [SerializeField] private Transform m_followTarget = null;

        [Tooltip("The method used to compute the celestial coordinate of the sun and moon in the sky. " +
        "Any other celestial body that you may use, will always get the realistic astronomical coordinate.")]
        [SerializeField] private AzureTimeSystem m_timeSystem = AzureTimeSystem.Simple;

        [Tooltip("The direction in which the time of day will flow.")]
        [SerializeField] private AzureTimeDirection m_timeDirection = AzureTimeDirection.Forward;

        [Tooltip("The loop in which the calendar will perform the day changes.")]
        [SerializeField] private AzureDateLoop m_dateLoop = AzureDateLoop.Off;

        [Tooltip("The current 'time position' in the day-night cycle. Note that this may not represent the correct time of day.")]
        [SerializeField] private float m_timeline = 6.0f;

        [Tooltip("The hour converted from the time of day.")]
        [SerializeField] private int m_hour = 6;

        [Tooltip("The minute converted from the time of day.")]
        [SerializeField] private int m_minute = 0;

        [Tooltip("The day used by the calendar and date system.")]
        [SerializeField] private int m_day = 1;

        [Tooltip("The month used by the calendar and date system.")]
        [SerializeField] private int m_month = 1;

        [Tooltip("The year used by the calendar and date system.")]
        [SerializeField] private int m_year = 2021;

        [Tooltip("The current selected calendar day.")]
        [SerializeField] private int m_selectedCalendarDay = 1;

        [Tooltip("The north-south angle of a position on the Earth's surface.")]
        [SerializeField] private float m_latitude = 0;

        [Tooltip("The east-west angle of a position on the Earth's surface.")]
        [SerializeField] private float m_longitude = 0;

        [Tooltip("Universal Time Coordinated.")]
        [SerializeField] private float m_utc = 0;

        [Tooltip("Duration of the day-night cycle in minutes.")]
        [SerializeField] private float m_dayLength = 24.0f;

        [Tooltip("The minimum directional light altitude (0° - 90°). You can use this to avoid the shadows to get stretched when the sun is close to the horizon at sunset.")]
        [SerializeField] private float m_minLightAltitude = 0.0f;

        [Tooltip("Will the 'time of day' be evaluated based on the timeline or based on the day-night length curve?")]
        [SerializeField] private bool m_isTimeEvaluatedByCurve = false;

        [Tooltip("Duration of the day-night cycle in minutes.")]
        [SerializeField] private AnimationCurve m_dayLengthCurve = AnimationCurve.Linear(0, 0, 24, 24);

        [Tooltip("List storing all celestial bodies currently in use by the system.")]
        [SerializeField] private List<AzureCelestialBody> m_celestialBodiesList = new List<AzureCelestialBody>();

        [Tooltip("Event triggered when the minute changes.")]
        [SerializeField] private UnityEvent m_onMinuteChange = new UnityEvent();

        [Tooltip("Event triggered when the hour changes.")]
        [SerializeField] private UnityEvent m_onHourChange = new UnityEvent();

        [Tooltip("Event triggered when the day changes to the next day at midnight.")]
        [SerializeField] private UnityEvent m_onDayChange = new UnityEvent();

        [Tooltip("The correct time of day converted from the timeline.")]
        private float m_timeOfDay = 6.0f;

        [Tooltip("The time progression step used to change the time of day.")]
        private float m_timeProgressionStep = 0f;

        [Tooltip("The sun elevation in the sky.")]
        private float m_sunElevation = 0f;

        [Tooltip("The moon elevation in the sky.")]
        private float m_moonElevation = 0f;

        [Tooltip("The distance between Earth and the Sun.")]
        private float m_sunDistance = 0f;

        [Tooltip("The local direction of the Sun.")]
        private Vector3 m_sunLocalDirection;

        [Tooltip("The local direction of the Moon.")]
        private Vector3 m_moonLocalDirection;

        [Tooltip("The local direction of the Directional Light.")]
        private Vector3 m_lightLocalDirection;

        [Tooltip("The distance between Earth and the Moon.")]
        private float m_moonDistance = 0f;

        [Tooltip("The time interval the system should use for checking the custom event list.")]
        [SerializeField] private AzureEventScanMode m_customEventScanMode = AzureEventScanMode.ByMinute;

        [Tooltip("The list of custom events.")]
        [SerializeField] private List<AzureCustomEvent> m_customEventList = new List<AzureCustomEvent>();

        // Calendar stuffs
        private DateTime m_dateTime;
        private int m_daysInMonth = 30;
        private int m_previousDaysInMonth = 30;
        private int m_previousMonth = 1;
        private int m_dayOfWeek = 0;
        [Tooltip("String array that stores the name of each week day.")]
        private readonly string[] m_weekNameList = new string[]
        {
            "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"
        };

        [Tooltip("String array that stores the name of each month.")]
        private readonly string[] m_monthNameList = new string[]
        {
            "January", "February", "March", "April", "May", "June",
            "July", "August", "September", "October", "November", "December"
        };

        [Tooltip("Array with 42 numeric strings used to fill a calendar.")]
        [SerializeField]
        private string[] m_dayNumberList = new string[]
        {
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
            "11", "12", "13", "14", "15", "16", "17", "18", "19", "20",
            "21", "22", "23", "24", "25", "26", "27", "28", "29", "30",
            "31", "32", "33", "34", "35", "36", "37", "38", "39", "40", "41"
        };

        // Timeline transition stuffs
        private int m_timelineTransitionTargetHour = 0;
        private int m_timelineTransitionTargetMinute = 0;
        private float  m_timelineTransitionTargetSpeed = 1f;
        private int m_timelineTransitionTargetMonth = 0;
        private int m_timelineTransitionTargetDay = 0;
        private int m_timelineTransitionTargetYear = 0;
        private AzureTimeDirection m_timelineTransitionDirection = AzureTimeDirection.Forward;
        private bool m_isTimelineTransitionInProgress = false;

        // Event stufss
        private int m_previousHour = 6;
        private int m_previousMinute = 0;

        private void Reset()
        {
            UpdateCalendar();
        }

        private void Awake()
        {
            m_timeProgressionStep = GetTimeProgressionStep();
            m_previousMinute = m_minute;
            m_previousHour = m_hour;
            UpdateCalendar();
            EvaluateTimeOfDay();
            ComputeCelestialCoordinates();
            EvaluateSunMoonElevation();
            SetDirectionalLightRotation();
        }

        private void Update()
        {
            // Only in gameplay
            if (Application.isPlaying)
            {
                // Move the prefab always to the target position
                if (m_followTarget)
                    transform.position = m_followTarget.position;

                // Moves the timeline forward
                if (m_timeDirection == AzureTimeDirection.Forward)
                {
                    m_timeline += m_timeProgressionStep * Time.deltaTime;

                    if (m_isTimelineTransitionInProgress)
                        ApplyTimelineTransition();

                    // Change to the next day
                    if (m_timeline > 24)
                    {
                        IncreaseDay();
                        m_timeline = 0;
                        m_onDayChange?.Invoke();
                    }
                }
                // Moves the timeline backward
                else
                {
                    m_timeline -= m_timeProgressionStep * Time.deltaTime;

                    if (m_isTimelineTransitionInProgress)
                        ApplyTimelineTransition();

                    // Change to the previous day
                    if (m_timeline < 0)
                    {
                        DecreaseDay();
                        m_timeline = 24;
                        m_onDayChange?.Invoke();
                    }
                }

                EvaluateTimeOfDay();
                EvaluateSunMoonElevation();

                // On minute change event
                if (m_previousMinute != m_minute)
                {
                    m_previousMinute = m_minute;
                    m_onMinuteChange?.Invoke();
                    if (m_customEventScanMode == AzureEventScanMode.ByMinute)
                        ScanCustomEventList();
                }

                // On hour change event
                if (m_previousHour != m_hour)
                {
                    m_previousHour = m_hour;
                    m_onHourChange?.Invoke();
                    if (m_customEventScanMode == AzureEventScanMode.ByHour)
                        ScanCustomEventList();
                }
            }

            // Editor only
            // Computes the celestial coordinates and light rotation in edit mode.
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                // Evaluates the time of day
                EvaluateTimeOfDay();
                // Sets the sun, moon and planets position
                ComputeCelestialCoordinates();
                // Gets the sun and moon direction and elevation
                EvaluateSunMoonElevation();
                // Sets the directional light rotation
                SetDirectionalLightRotation();
            }
            #endif
        }

        private void FixedUpdate()
        {
            // Sets the sun, moon and planets position
            ComputeCelestialCoordinates();
            // Sets the directional light rotation
            SetDirectionalLightRotation();
        }

        /// <summary>
        /// Computes the time progression step based on the day length value.
        /// </summary>
        private float GetTimeProgressionStep()
        {
            if (m_dayLength > 0.0f)
                return (24.0f / 60.0f) / m_dayLength;
            else
                return 0.0f;
        }

        /// <summary>
        /// Starts a time transition from one time/date to another.
        /// </summary>
        public void StartTimelineTransition(int hour, int minute, float speedMultiplier, AzureTimeDirection timeDir = AzureTimeDirection.Forward, int day = int.MaxValue, int month = int.MaxValue, int year = int.MaxValue)
        {
            m_timelineTransitionTargetHour = Mathf.Min(hour, 23);
            m_timelineTransitionTargetMinute = Mathf.Min(minute, 59);
            m_timelineTransitionTargetSpeed = Mathf.Max(1.0f, m_timeProgressionStep * speedMultiplier);
            m_timelineTransitionTargetDay = day;
            m_timelineTransitionTargetMonth = month;
            m_timelineTransitionTargetYear = year;
            m_timelineTransitionDirection = timeDir;

            if (timeDir == AzureTimeDirection.Backward)
            {
                if (day == int.MaxValue) day = int.MinValue;
                if (month == int.MaxValue) month = int.MinValue;
                if (year == int.MaxValue) year = int.MinValue;

                m_timelineTransitionTargetDay = day;
                m_timelineTransitionTargetMonth = month;
                m_timelineTransitionTargetYear = year;
            }

            m_isTimelineTransitionInProgress = true;
        }

        public void CancelTimelineTransition()
        {
            m_isTimelineTransitionInProgress = false;
        }

        private void ApplyTimelineTransition()
        {
            switch (m_timelineTransitionDirection)
            {
                case AzureTimeDirection.Forward:
                    ApplyTimelineTransitionForward();
                    break;
                case AzureTimeDirection.Backward:
                    ApplyTimelineTransitionBackward();
                    break;
            }
        }

        private void ApplyTimelineTransitionForward()
        {
            m_timeline += m_timelineTransitionTargetSpeed * Time.deltaTime;

            if (m_hour >= m_timelineTransitionTargetHour)
            {
                if (m_minute >= m_timelineTransitionTargetMinute)
                {
                    if (m_timelineTransitionTargetDay == int.MaxValue && m_timelineTransitionTargetMonth == int.MaxValue && m_timelineTransitionTargetYear == int.MaxValue)
                    {
                        CancelTimelineTransition();
                        return;
                    }
                    
                    if (m_day >= m_timelineTransitionTargetDay)
                    {
                        if (m_timelineTransitionTargetMonth == int.MaxValue && m_timelineTransitionTargetYear == int.MaxValue)
                        {
                            CancelTimelineTransition();
                            return;
                        }

                        if (m_month >= m_timelineTransitionTargetMonth)
                        {
                            if (m_timelineTransitionTargetYear == int.MaxValue)
                            {
                                CancelTimelineTransition();
                                return;
                            }

                            if (m_year >= m_timelineTransitionTargetYear)
                            {
                                CancelTimelineTransition();
                                return;
                            }
                        }
                    }
                }
            }
        }

        private void ApplyTimelineTransitionBackward()
        {
            m_timeline -= m_timelineTransitionTargetSpeed * Time.deltaTime;

            if (m_hour <= m_timelineTransitionTargetHour)
            {
                if (m_minute <= m_timelineTransitionTargetMinute)
                {
                    if (m_timelineTransitionTargetDay == int.MinValue && m_timelineTransitionTargetMonth == int.MinValue && m_timelineTransitionTargetYear == int.MinValue)
                    {
                        CancelTimelineTransition();
                        return;
                    }

                    if (m_day <= m_timelineTransitionTargetDay)
                    {
                        if (m_timelineTransitionTargetMonth == int.MinValue && m_timelineTransitionTargetYear == int.MinValue)
                        {
                            CancelTimelineTransition();
                            return;
                        }

                        if (m_month <= m_timelineTransitionTargetMonth)
                        {
                            if (m_timelineTransitionTargetYear == int.MinValue)
                            {
                                CancelTimelineTransition();
                                return;
                            }

                            if (m_year <= m_timelineTransitionTargetYear)
                            {
                                CancelTimelineTransition();
                                return;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Evaluates the time of day according to the timeline.
        /// </summary>
        private void EvaluateTimeOfDay()
        {
            m_timeOfDay = m_isTimeEvaluatedByCurve ? m_dayLengthCurve.Evaluate(m_timeline) : m_timeline;
            m_hour = (int)Mathf.Floor(m_timeOfDay);
            m_minute = (int)Mathf.Floor(m_timeOfDay * 60 % 60);
        }

        /// <summary>
        /// Evaluates the sun and moon elevation in the sky.
        /// </summary>
        private void EvaluateSunMoonElevation()
        {
            m_sunLocalDirection = m_sunTransform.forward;
            m_moonLocalDirection = m_moonTransform.forward;
            m_sunElevation = Vector3.Dot(-m_sunLocalDirection, Vector3.up);
            m_moonElevation = Vector3.Dot(-m_moonLocalDirection, Vector3.up);
        }

        /// <summary>
        /// Sets the directional light transform direction.
        /// </summary>
        private void SetDirectionalLightRotation()
        {
            m_directionalLight.localRotation = Quaternion.LookRotation(m_sunElevation >= 0.0f ? m_sunLocalDirection : m_moonLocalDirection);

            // Avoid the directional light to get close to the horizon line
            if (m_minLightAltitude > 0f && m_minLightAltitude < 90f)
            {
                m_lightLocalDirection = m_directionalLight.localEulerAngles;

                if (m_lightLocalDirection.x <= m_minLightAltitude) m_lightLocalDirection.x = m_minLightAltitude;
                    m_directionalLight.localEulerAngles = m_lightLocalDirection;
            }
        }

        /// <summary>
        /// Scans the custom event list and perform the event that match with the current date and time.
        /// </summary>
        private void ScanCustomEventList()
        {
            if (m_customEventList.Count <= 0)
                return;

            for (int i = 0; i < m_customEventList.Count; i++)
            {
                if (m_customEventList[i].unityEvent == null)
                    continue;
                if (m_customEventList[i].year != m_year && m_customEventList[i].year != -1)
                    continue;
                if (m_customEventList[i].month != m_month && m_customEventList[i].month != -1)
                    continue;
                if (m_customEventList[i].day != m_day && m_customEventList[i].day != -1)
                    continue;
                if (m_customEventList[i].hour != m_hour && m_customEventList[i].hour != -1)
                    continue;
                if (m_customEventList[i].minute != m_minute && m_customEventList[i].minute != -1)
                    continue;
                m_customEventList[i].unityEvent.Invoke();
            }
        }

        /// <summary>
        /// Cumputes the celestial coordinates for all celestial bodies currently in use by the system.
        /// Based on formulas from Paul Schlyter's web page: http://www.stjarnhimlen.se/comp/ppcomp.html
        /// </summary>
        private void ComputeCelestialCoordinates()
        {
            switch (m_timeSystem)
            {
                case AzureTimeSystem.Simple:
                    m_sunTransform.localRotation = GetSunSimpleRotation();
                    m_moonTransform.localRotation = m_sunTransform.localRotation * Quaternion.Euler(0, -180, 0);
                    Shader.SetGlobalMatrix(AzureShaderUniforms.StarfieldMatrix, m_sunTransform.worldToLocalMatrix);
                    break;

                case AzureTimeSystem.Realistic:

                    // Initializations
                    float hour = m_timeOfDay - m_utc;
                    float rad = Mathf.Deg2Rad;
                    float deg = Mathf.Rad2Deg;
                    float latitude = m_latitude * rad;

                    // The time scale
                    float d = 367 * m_year - 7 * (m_year + (m_month + 9) / 12) / 4 + 275 * m_month / 9 + m_day - 730530;
                    d = d + (hour / 24.0f);

                    // Obliquity of the ecliptic: The tilt of earth's axis of rotation
                    float ecl = 23.4393f - 3.563E-7f * d;
                    ecl *= rad;

                    // Orbital elements of the sun
                    float N = 0.0f;
                    float i = 0.0f;
                    float w = 282.9404f + 4.70935E-5f * d;
                    float a = 1.000000f;
                    float e = 0.016709f - 1.151E-9f * d;
                    float M = 356.0470f + 0.9856002585f * d;

                    // Eccentric anomaly
                    M *= rad;
                    float E = M + e * Mathf.Sin(M) * (1f + e * Mathf.Cos(M));

                    // Sun's distance (r) and its true anomaly (v)
                    float xv = Mathf.Cos(E) - e;
                    float yv = Mathf.Sqrt(1.0f - (e * e)) * Mathf.Sin(E);
                    float v = Mathf.Atan2(yv, xv) * deg;
                    float r = Mathf.Sqrt((xv * xv) + (yv * yv));

                    // Sun's true longitude
                    float lonsun = (v + w) * rad;

                    // Convert lonsun,r to ecliptic rectangular geocentric coordinates xs,ys:
                    float xs = r * Mathf.Cos(lonsun);
                    float ys = r * Mathf.Sin(lonsun);
                    //    zs = 0;

                    // To convert this to equatorial, rectangular, geocentric coordinates, compute:
                    float xe = xs;
                    float ye = ys * Mathf.Cos(ecl);
                    float ze = ys * Mathf.Sin(ecl);

                    // Sun's right ascension (RA) and declination (Decl):
                    float RA = Mathf.Atan2(ye, xe);
                    float Decl = Mathf.Atan2(ze, Mathf.Sqrt((xe * xe) + (ye * ye)));

                    // The sidereal time
                    float Ls = v + w;
                    float GMST0 = Ls + 180.0f;
                    float GMST = GMST0 + (hour * 15.0f);
                    float LST = (GMST + m_longitude) * rad;

                    // Azimuthal coordinates
                    float HA = LST - RA;

                    float x = Mathf.Cos(HA) * Mathf.Cos(Decl);
                    float y = Mathf.Sin(HA) * Mathf.Cos(Decl);
                    float z = Mathf.Sin(Decl);

                    float xhor = (x * Mathf.Sin(latitude)) - (z * Mathf.Cos(latitude));
                    float yhor = y;
                    float zhor = (x * Mathf.Cos(latitude)) + (z * Mathf.Sin(latitude));

                    float azimuth = Mathf.Atan2(yhor, xhor);
                    float altitude = Mathf.Asin(zhor);

                    // Gets the celestial rotation
                    Vector3 celestialRotation;
                    celestialRotation.x = altitude * deg;
                    celestialRotation.y = azimuth * deg;
                    celestialRotation.z = 0.0f;
                    m_sunTransform.localRotation = Quaternion.Euler(celestialRotation);
                    m_sunDistance = r * 150000000f;
                    Shader.SetGlobalMatrix(AzureShaderUniforms.StarfieldMatrix,
                    Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(90.0f - m_latitude, 0.0f, 0.0f) * Quaternion.Euler(0.0f, m_longitude, 0.0f) * Quaternion.Euler(0.0f, LST * deg, 0.0f), Vector3.one).inverse);

                    // Applys position to the sun
                    //Quaternion direction = Quaternion.Euler(celestialRotation);
                    //Vector3 pos = direction * new Vector3(0, 0, -m_astronomicalUnit);
                    //m_sunTransform.transform.localPosition = pos;

                    // Orbital elements of the Moon
                    N = 125.1228f - 0.0529538083f * d;
                    i = 5.1454f;
                    w = 318.0634f + 0.1643573223f * d;
                    //a = 0.002566882112227f; (AU)
                    a = 60.2666f; // Earth radius
                    e = 0.054900f;
                    M = 115.3654f + 13.0649929509f * d;

                    // Eccentric anomaly
                    M *= rad;
                    E = M + e * Mathf.Sin(M) * (1f + e * Mathf.Cos(M));

                    // Moon's distance and true anomaly
                    xv = a * (Mathf.Cos(E) - e);
                    yv = a * (Mathf.Sqrt(1f - e * e) * Mathf.Sin(E));
                    v = Mathf.Atan2(yv, xv) * deg;
                    r = Mathf.Sqrt(xv * xv + yv * yv);

                    // Moon's true longitude
                    lonsun = (v + w) * rad;
                    float sinLongitude = Mathf.Sin(lonsun);
                    float cosLongitude = Mathf.Cos(lonsun);

                    // The position in space - for the planets
                    // Geocentric (Earth-centered) coordinates - for the moon
                    N *= rad;
                    i *= rad;

                    float xh = r * (Mathf.Cos(N) * cosLongitude - Mathf.Sin(N) * sinLongitude * Mathf.Cos(i));
                    float yh = r * (Mathf.Sin(N) * cosLongitude + Mathf.Cos(N) * sinLongitude * Mathf.Cos(i));
                    float zh = r * (sinLongitude * Mathf.Sin(i));

                    // Geocentric (Earth-centered) coordinates
                    // For the moon this is the same as the position in space, there is no need to calculate again
                    // float xg = xh;
                    // float yg = yh;
                    // float zg = zh;

                    // Equatorial coordinates
                    xe = xh;
                    ye = yh * Mathf.Cos(ecl) - zh * Mathf.Sin(ecl);
                    ze = yh * Mathf.Sin(ecl) + zh * Mathf.Cos(ecl);

                    // Moon's right ascension (RA) and declination (Decl)
                    RA = Mathf.Atan2(ye, xe);
                    Decl = Mathf.Atan2(ze, Mathf.Sqrt(xe * xe + ye * ye));

                    // The sidereal time
                    // It is already calculated for the sun, there is no need to calculate again

                    // Azimuthal coordinates
                    HA = LST - RA;

                    x = Mathf.Cos(HA) * Mathf.Cos(Decl);
                    y = Mathf.Sin(HA) * Mathf.Cos(Decl);
                    z = Mathf.Sin(Decl);

                    xhor = x * Mathf.Sin(latitude)  - z * Mathf.Cos(latitude) ;
                    yhor = y;
                    zhor = x * Mathf.Cos(latitude) + z * Mathf.Sin(latitude) ;

                    azimuth = Mathf.Atan2(yhor, xhor);
                    altitude = Mathf.Asin(zhor);

                    // Gets the celestial rotation
                    celestialRotation.x = altitude * deg;
                    celestialRotation.y = azimuth * deg;
                    celestialRotation.z = 0.0f;
                    m_moonTransform.localRotation = Quaternion.Euler(celestialRotation);
                    m_moonDistance = r * 6371f;

                    // Planets
                    if (m_celestialBodiesList.Count > 0)
                    {
                        AzureCelestialBody celestialBody;
                        for (int index = 0; index < m_celestialBodiesList.Count; index++)
                        {
                            celestialBody = m_celestialBodiesList[index];
                            if (!celestialBody.transform) continue;

                            // Orbital elements of the planets
                            switch (celestialBody.type)
                            {
                                case AzureCelestialBody.Type.Mercury:
                                    N = 48.3313f + 3.24587E-5f * d;
                                    i = 7.0047f + 5.00E-8f * d;
                                    w = 29.1241f + 1.01444E-5f * d;
                                    a = 0.387098f;
                                    e = 0.205635f + 5.59E-10f * d;
                                    M = 168.6562f + 4.0923344368f * d;
                                    break;

                                case AzureCelestialBody.Type.Venus:
                                    N = 76.6799f + 2.46590E-5f * d;
                                    i = 3.3946f + 2.75E-8f * d;
                                    w = 54.8910f + 1.38374E-5f * d;
                                    a = 0.723330f;
                                    e = 0.006773f - 1.302E-9f * d;
                                    M = 48.0052f + 1.6021302244f * d;
                                    break;

                                case AzureCelestialBody.Type.Mars:
                                    N = 49.5574f + 2.11081E-5f * d;
                                    i = 1.8497f - 1.78E-8f * d;
                                    w = 286.5016f + 2.92961E-5f * d;
                                    a = 1.523688f;
                                    e = 0.093405f + 2.516E-9f * d;
                                    M = 18.6021f + 0.5240207766f * d;
                                    break;

                                case AzureCelestialBody.Type.Jupiter:
                                    N = 100.4542f + 2.76854E-5f * d;
                                    i = 1.3030f - 1.557E-7f * d;
                                    w = 273.8777f + 1.64505E-5f * d;
                                    a = 5.20256f;
                                    e = 0.048498f + 4.469E-9f * d;
                                    M = 19.8950f + 0.0830853001f * d;
                                    break;

                                case AzureCelestialBody.Type.Saturn:
                                    N = 113.6634f + 2.38980E-5f * d;
                                    i = 2.4886f - 1.081E-7f * d;
                                    w = 339.3939f + 2.97661E-5f * d;
                                    a = 9.55475f;
                                    e = 0.055546f - 9.499E-9f * d;
                                    M = 316.9670f + 0.0334442282f * d;
                                    break;

                                case AzureCelestialBody.Type.Uranus:
                                    N = 74.0005f + 1.3978E-5f * d;
                                    i = 0.7733f + 1.9E-8f * d;
                                    w = 96.6612f + 3.0565E-5f * d;
                                    a = 19.18171f - 1.55E-8f * d;
                                    e = 0.047318f + 7.45E-9f * d;
                                    M = 142.5905f + 0.011725806f * d;
                                    break;

                                case AzureCelestialBody.Type.Neptune:
                                    N = 131.7806f + 3.0173E-5f * d;
                                    i = 1.7700f - 2.55E-7f * d;
                                    w = 272.8461f - 6.027E-6f * d;
                                    a = 30.05826f + 3.313E-8f * d;
                                    e = 0.008606f + 2.15E-9f * d;
                                    M = 260.2471f + 0.005995147f * d;
                                    break;

                                // No analytical theory has ever been constructed for the planet Pluto.
                                // Our most accurate representation of the motion of this planet is from numerical integrations.
                                case AzureCelestialBody.Type.Pluto:
                                    float S = 50.03f + 0.033459652f * d;
                                    float P = 238.95f + 0.003968789f * d;

                                    S *= rad;
                                    P *= rad;

                                    float pluto_lonecl = 238.9508f + 0.00400703f * d
                                                       - 19.799f * Mathf.Sin(P) + 19.848f *    Mathf.Cos(P)
                                                       + 0.897f * Mathf.Sin(2 * P) - 4.956f * Mathf.Cos(2 * P)
                                                       + 0.610f * Mathf.Sin(3 * P) + 1.211f * Mathf.Cos(3 * P)
                                                       - 0.341f * Mathf.Sin(4 * P) - 0.190f * Mathf.Cos(4 * P)
                                                       + 0.128f * Mathf.Sin(5 * P) - 0.034f * Mathf.Cos(5 * P)
                                                       - 0.038f * Mathf.Sin(6 * P) + 0.031f * Mathf.Cos(6 * P)
                                                       + 0.020f * Mathf.Sin(S - P) - 0.010f * Mathf.Cos(S - P);

                                    float pluto_latecl = -3.9082f
                                                       - 5.453f * Mathf.Sin(P)     -14.975f * Mathf.Cos(P)
                                                       + 3.527f * Mathf.Sin(2 * P) + 1.673f * Mathf.Cos(2 * P)
                                                       - 1.051f * Mathf.Sin(3 * P) + 0.328f * Mathf.Cos(3 * P)
                                                       + 0.179f * Mathf.Sin(4 * P) - 0.292f * Mathf.Cos(4 * P)
                                                       + 0.019f * Mathf.Sin(5 * P) + 0.100f * Mathf.Cos(5 * P)
                                                       - 0.031f * Mathf.Sin(6 * P) - 0.026f * Mathf.Cos(6 * P)
                                                                                   + 0.011f * Mathf.Cos(S - P);

                                    float pluto_r = 40.72f
                                                  + 6.68f * Mathf.Sin(P) +     6.90f * Mathf.Cos(P)
                                                  - 1.18f * Mathf.Sin(2 * P) - 0.03f * Mathf.Cos(2 * P)
                                                  + 0.15f * Mathf.Sin(3 * P) - 0.14f * Mathf.Cos(3 * P);

                                    // Geocentric (Earth-centered) coordinates
                                    pluto_lonecl *= rad;
                                    pluto_latecl *= rad;
                                    float pluto_cosLatecl = Mathf.Cos(pluto_latecl);
                                    xh = pluto_r * Mathf.Cos(pluto_lonecl) * pluto_cosLatecl;
                                    yh = pluto_r * Mathf.Sin(pluto_lonecl) * pluto_cosLatecl;
                                    zh = pluto_r * Mathf.Sin(pluto_latecl);

                                    // From the sun computation
                                    // xs = r * cosLongitude;
                                    // ys = r * sinLongitude;

                                    float pluto_xg = xh + xs;
                                    float pluto_yg = yh + ys;
                                    float pluto_zg = zh;

                                    // Equatorial coordinates
                                    xe = pluto_xg;
                                    ye = pluto_yg * Mathf.Cos(ecl) - pluto_zg * Mathf.Sin(ecl);
                                    ze = pluto_yg * Mathf.Sin(ecl) + pluto_zg * Mathf.Cos(ecl);

                                    // Moon's right ascension (RA) and declination (Decl)
                                    RA = Mathf.Atan2(ye, xe);
                                    Decl = Mathf.Atan2(ze, Mathf.Sqrt(xe * xe + ye * ye));

                                    // The sidereal time
                                    // It is already calculated for the sun, there is no need to calculate again

                                    // Azimuthal coordinates
                                    HA = LST - RA;

                                    x = Mathf.Cos(HA) * Mathf.Cos(Decl);
                                    y = Mathf.Sin(HA) * Mathf.Cos(Decl);
                                    z = Mathf.Sin(Decl);

                                    xhor = x * Mathf.Sin(latitude) - z * Mathf.Cos(latitude);
                                    yhor = y;
                                    zhor = x * Mathf.Cos(latitude) + z * Mathf.Sin(latitude);

                                    azimuth = Mathf.Atan2(yhor, xhor);
                                    altitude = Mathf.Asin(zhor);

                                    // Gets the celestial rotation
                                    celestialRotation.x = altitude * deg;
                                    celestialRotation.y = azimuth * deg;
                                    celestialRotation.z = 0.0f;
                                    celestialBody.distance = pluto_r * 150000000f;

                                    celestialBody.transform.localRotation = Quaternion.Euler(celestialRotation);
                                    continue;
                            }

                            // Eccentric anomaly
                            M *= rad;
                            E = M + e * Mathf.Sin(M) * (1f + e * Mathf.Cos(M));

                            // Planet's distance and true anomaly
                            xv = a * (Mathf.Cos(E) - e);
                            yv = a * (Mathf.Sqrt(1f - e * e) * Mathf.Sin(E));
                            v = Mathf.Atan2(yv, xv) * deg;
                            r = Mathf.Sqrt(xv * xv + yv * yv);

                            // Planet's true longitude
                            lonsun = (v + w) * rad;
                            cosLongitude = Mathf.Cos(lonsun);
                            sinLongitude = Mathf.Sin(lonsun);

                            // The position in space - heliocentric (Sun-centered) position
                            N *= rad;
                            i *= rad;

                            xh = r * (Mathf.Cos(N) * cosLongitude - Mathf.Sin(N) * sinLongitude * Mathf.Cos(i));
                            yh = r * (Mathf.Sin(N) * cosLongitude + Mathf.Cos(N) * sinLongitude * Mathf.Cos(i));
                            zh = r * (sinLongitude * Mathf.Sin(i));

                            float lonecl = Mathf.Atan2(yh, xh);
                            float latecl = Mathf.Atan2(zh, Mathf.Sqrt(xh * xh + yh * yh));

                            // Geocentric (Earth-centered) coordinates
                            float cosLatecl = Mathf.Cos(latecl);
                            xh = r * Mathf.Cos(lonecl) * cosLatecl;
                            yh = r * Mathf.Sin(lonecl) * cosLatecl;
                            zh = r * Mathf.Sin(latecl);

                            // From the sun computation
                            // xs = r * cosLongitude;
                            // ys = r * sinLongitude;

                            float xg = xh + xs;
                            float yg = yh + ys;
                            float zg = zh;

                            // Equatorial coordinates
                            xe = xg;
                            ye = yg * Mathf.Cos(ecl) - zg * Mathf.Sin(ecl);
                            ze = yg * Mathf.Sin(ecl) + zg * Mathf.Cos(ecl);

                            // Moon's right ascension (RA) and declination (Decl)
                            RA = Mathf.Atan2(ye, xe);
                            Decl = Mathf.Atan2(ze, Mathf.Sqrt(xe * xe + ye * ye));

                            // The sidereal time
                            // It is already calculated for the sun, there is no need to calculate again

                            // Azimuthal coordinates
                            HA = LST - RA;

                            x = Mathf.Cos(HA) * Mathf.Cos(Decl);
                            y = Mathf.Sin(HA) * Mathf.Cos(Decl);
                            z = Mathf.Sin(Decl);

                            xhor = x * Mathf.Sin(latitude) - z * Mathf.Cos(latitude);
                            yhor = y;
                            zhor = x * Mathf.Cos(latitude) + z * Mathf.Sin(latitude);

                            azimuth = Mathf.Atan2(yhor, xhor);
                            altitude = Mathf.Asin(zhor);

                            // Gets the celestial rotation
                            celestialRotation.x = altitude * deg;
                            celestialRotation.y = azimuth * deg;
                            celestialRotation.z = 0.0f;
                            celestialBody.distance = r * 150000000f;

                            celestialBody.transform.localRotation = Quaternion.Euler(celestialRotation);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Gets the current day of the week and return an integer between 0 and 6.
        /// </summary>
        public int GetDayOfWeek()
        {
            m_dateTime = new DateTime(m_year, m_month, m_day);
            return (int)m_dateTime.DayOfWeek;
        }

        /// <summary>
        /// Gets the day of the week from a custom date and return an integer between 0 and 6.
        /// </summary>
        public int GetDayOfWeek(int year, int month, int day)
        {
            m_dateTime = new DateTime(year, month, day);
            return (int)m_dateTime.DayOfWeek;
        }

        /// <summary>
        /// Gets the current day of the week and return as string.
        /// </summary>
        public string GetDayOfWeekString()
        {
            m_dateTime = new DateTime(m_year, m_month, m_day);
            return m_weekNameList[(int)m_dateTime.DayOfWeek];
        }

        /// <summary>
        /// Gets the day of the week from a custom date and return as string.
        /// </summary>
        public string GetDayOfWeekString(int year, int month, int day)
        {
            m_dateTime = new DateTime(year, month, day);
            return m_weekNameList[(int)m_dateTime.DayOfWeek];
        }

        /// <summary>
        /// Adjust the calendar when there is a change in the date.
        /// </summary>
        public void UpdateCalendar()
        {
            // Get the number of days in the current month
            m_daysInMonth = DateTime.DaysInMonth(m_year, m_month);

            // Avoid selecting a date that does not exist
            m_day = Mathf.Clamp(m_day, 1, m_daysInMonth);
            m_month = Mathf.Clamp(m_month, 1, 12);
            m_year = Mathf.Clamp(m_year, 0, 9999);

            // Creates a custom DateTime at the first day of the current month
            m_dateTime = new DateTime(m_year, m_month, 1);

            // Gets the day of week corresponding to this custom DateTime
            m_dayOfWeek = (int) m_dateTime.DayOfWeek;

            // Keeps the same day selected in the calendar even when the date is changed externally.
            m_selectedCalendarDay = m_day - 1 + m_dayOfWeek;
            
            for (int i = 0; i < m_dayNumberList.Length; i++)
            {
                // Make null all the calendar buttons
                if (i < m_dayOfWeek || i >= (m_dayOfWeek + m_daysInMonth))
                {
                    m_dayNumberList[i] = "";
                    continue;
                }

                // Sets the day number only on the valid buttons of the current month in use by the calendar.
                m_dateTime = new DateTime(m_year, m_month, (i - m_dayOfWeek) + 1);
                m_dayNumberList[i] = m_dateTime.Day.ToString();
            }
        }

        /// <summary>
        /// Sets the timeline using a float as parameter.
        /// </summary>
        public void SetTimeline(float value)
        {
            m_timeline = value;
        }

        /// <summary>
        /// Returns the timeline float value.
        /// </summary>
        public float GetTimeline()
        {
            return m_timeline;
        }

        /// <summary>
        /// Returns the current time of day as a Vector2(hours, minutes) converted from the timeline.
        /// </summary>
        public Vector2 GetTimeOfDay()
        {
            return new Vector2(m_hour, m_minute);
        }

        /// <summary>
        /// Pause the time progression.
        /// </summary>
        public void PauseTime()
        {
            m_timeProgressionStep = 0.0f;
        }

        /// <summary>
        /// Starts the time progression again.
        /// </summary>
        public void PlayTimeAgain()
        {
            m_timeProgressionStep = GetTimeProgressionStep();
        }

        /// <summary>
        /// Set a new duration of the day cycle in minutes.
        /// </summary>
        public void SetNewDayLength(float value)
        {
            m_dayLength = value;
            m_timeProgressionStep = GetTimeProgressionStep();
        }

        /// <summary>
        /// Returns the correct time to evaluate curves and gradients on weather profiles.
        /// </summary>
        public float GetEvaluateTime() { return m_timeOfDay; }

        /// <summary>
        /// Returns the sun elevation in the sky.
        /// </summary>
        public float GetSunElevation() { return m_sunElevation; }

        /// <summary>
        /// Returns the moon elevation in the sky.
        /// </summary>
        public float GetMoonElevation() { return m_moonElevation; }

        /// <summary>
        /// Sets a new custom date.
        /// </summary>
        public void SetDate(int year, int month, int day)
        {
            this.m_year = year;
            this.m_month = month;
            this.m_day = day;
            UpdateCalendar();
        }

        /// <summary>
        /// Returns the current date as a Vector3Int(year, month, day).
        /// </summary>
        public Vector3Int GetDate()
        {
            return new Vector3Int(m_year, m_month, m_day);
        }

        /// <summary>
        /// Returns the current date converted to string using the default format used by Azure.
        /// </summary>
        public string GetDateString()
        {
            // Format: "MMMM dd, yyyy"
            return m_monthNameList[m_month - 1] + " " + m_day.ToString("00") + ", " + m_year.ToString("0000");
        }

        /// <summary>
        /// Returns the current date converted to string using a custom format.
        /// </summary>
        public string GetDateString(string format)
        {
            m_dateTime = new DateTime(m_year, m_month, m_day);
            return m_dateTime.ToString(format);
        }

        /// <summary>
        /// Sets a new custom year.
        /// </summary>
        public void SetYear(int value)
        {
            m_year = value;
            UpdateCalendar();
        }

        /// <summary>
        /// Returns the current year number.
        /// </summary>
        public int GetYear()
        {
            return m_year;
        }

        /// <summary>
        /// Sets a new custom month.
        /// </summary>
        public void SetMonth(int value)
        {
            m_month = value;
            UpdateCalendar();
        }

        /// <summary>
        /// Returns the current month number.
        /// </summary>
        public int GetMonth()
        {
            return m_month;
        }

        /// <summary>
        /// Sets a new custom day.
        /// </summary>
        public void SetDay(int value)
        {
            this.m_day = value;
            UpdateCalendar();
        }

        /// <summary>
        /// Returns the current day number.
        /// </summary>
        public int GetDay()
        {
            return m_day;
        }

        /// <summary>
        /// Increases the year number.
        /// </summary>
        public void IncreaseYear()
        {
            if (m_dateLoop != AzureDateLoop.ByYear)
            {
                m_year++;
                if (m_year > 9999) m_year = 0;
            }

            UpdateCalendar();
        }

        /// <summary>
        /// Decreases the year number.
        /// </summary>
        public void DecreaseYear()
        {
            if (m_dateLoop != AzureDateLoop.ByYear)
            {
                m_year--;
                if (m_year < 0) m_year = 9999;
            }

            UpdateCalendar();
        }

        /// <summary>
        /// Increases the month number.
        /// </summary>
        public void IncreaseMonth()
        {
            if (m_dateLoop != AzureDateLoop.ByMonth)
            {
                m_month++;

                if (m_month > 12)
                {
                    m_month = 1;
                    IncreaseYear();
                }
            }
            
            UpdateCalendar();
        }
        
        /// <summary>
        /// Decreases the month number.
        /// </summary>
        public void DecreaseMonth()
        {
            if (m_dateLoop != AzureDateLoop.ByMonth)
            {
                m_month--;

                if (m_month < 1)
                {
                    m_month = 12;
                    DecreaseYear();
                }
            }
            
            UpdateCalendar();
        }

        /// <summary>
        /// Increases the day number.
        /// </summary>
        public void IncreaseDay()
        {
            if (m_dateLoop != AzureDateLoop.ByDay)
            {
                m_day++;

                if (m_day > m_daysInMonth)
                {
                    m_day = 1;
                    IncreaseMonth();
                }
            }

            UpdateCalendar();
        }

        /// <summary>
        /// Decreases the day number.
        /// </summary>
        public void DecreaseDay()
        {
            if (m_dateLoop != AzureDateLoop.ByDay)
            {
                m_day--;
                m_previousMonth = m_dateLoop == AzureDateLoop.ByMonth ? m_month : m_month - 1;
                if (m_previousMonth < 1) m_previousMonth = 12;
                m_previousDaysInMonth = DateTime.DaysInMonth(m_year, m_previousMonth);

                if (m_day < 1)
                {
                    m_day = m_previousDaysInMonth;
                    DecreaseMonth();
                }
            }

            UpdateCalendar();
        }

        /// <summary>
        /// Computes the sun simple rotation
        /// </summary>
        private Quaternion GetSunSimpleRotation()
        {
            return Quaternion.Euler(0.0f, m_longitude, -m_latitude) * Quaternion.Euler(((m_timeOfDay + m_utc) * 360.0f / 24.0f) - 90.0f, 180.0f, 0.0f);
        }

        #if UNITY_EDITOR
        public void ForceEditorUpdate()
        {
            Awake();
        }
        #endif
    }
}