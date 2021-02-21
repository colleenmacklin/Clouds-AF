using System;
using UnityEngine.UI;

namespace UnityEngine.AzureSky
{
    [ExecuteInEditMode]
    [AddComponentMenu("Azure[Sky]/Azure Time Controller")]
    [RequireComponent(typeof(AzureSkyController))]
    public class AzureTimeController : MonoBehaviour
    {
        // References
        private AzureSkyController m_skyController;

        // Options
        public AzureTimeSystem timeSystem = AzureTimeSystem.Simple;
        public AzureTimeDirection timeDirection = AzureTimeDirection.Forward;
        public AzureTimeRepeatMode repeatMode = AzureTimeRepeatMode.Off;

        // Cycle of day
        public float timeline = 6.0f;
        private float m_timeOfDay = 6.0f;
        private float m_sunElevation;
        private float m_moonElevation;
        private float m_timeProgressionStep;
        private bool m_isTimelineTransitionInProgress = false;
        private float m_timelineSourceTransitionTime;
        private float m_timelineDestinationTransitionTime;
        private float m_startTimelineTransitionStep;
        private float m_timelineTransitionStep;
        private float m_timelineTransitionSpeed;

        // Time and date
        public int hour = 6;
        public int minute = 0;
        public int day = 1;
        public int month = 1;
        public int year = 2020;
        public int selectedCalendarDay = 1;
        public float latitude = 0;
        public float longitude = 0;
        public float utc;
        public float dayLength = 24.0f;
        public bool isTimeEvaluatedByCurve;
        public AnimationCurve dayLengthCurve = AnimationCurve.Linear(0, 0, 24, 24);

        // Internal use
        private DateTime m_dateTime;
        private int m_previousHour = 6;
        private int m_previousMinute = 0;
        private int m_daysInMonth = 30;
        private int m_previousDaysInMonth = 30;
        private int m_previousMonth = 1;
        private int m_dayOfWeek = 0;
        private Vector3 m_starFieldOffset = Vector3.zero;
        private Matrix4x4 m_starFieldMatrix;
        
        // Lights controller
        private Vector3 m_sunLocalDirection, m_moonLocalDirection;
        private Vector3 m_sunRealisticRotation, m_moonRealisticRotation;
        private Quaternion m_sunSimpleRotation, m_moonSimpleRotation;
        private float m_lst, m_radians, m_radLatitude, m_sinLatitude, m_cosLatitude;
        
        private void Reset()
        {
            UpdateCalendar();
        }

        private void Start()
        {
            m_skyController = GetComponent<AzureSkyController>();
            
            // Calculates the progression step to move the timeline
            m_timeProgressionStep = GetTimeProgressionStep();
            m_previousMinute = minute;
            m_previousHour = hour;
            UpdateTimeSystem();
            UpdateCalendar();
        }

        private void Update()
        {
            // Update time of day
            m_timeOfDay = isTimeEvaluatedByCurve ? dayLengthCurve.Evaluate(timeline) : timeline;
            hour = (int) Mathf.Floor(m_timeOfDay);
            minute = (int) Mathf.Floor(m_timeOfDay * 60 % 60);
            
            // Only in gameplay
            if (Application.isPlaying)
            {
                // Moves the timeline
                if(timeDirection == AzureTimeDirection.Forward)
                {
                    timeline += m_timeProgressionStep * Time.deltaTime;
                    
                    // Timeline transition
                    if(m_isTimelineTransitionInProgress) DoTimelineTransition(m_timelineSourceTransitionTime, m_timelineDestinationTransitionTime);

                    // Next day
                    if (timeline > 24)
                    {
                        IncreaseDay();
                        m_skyController.OnDayChange();
                        timeline = 0;
                    }
                }
                else
                {
                    timeline -= m_timeProgressionStep * Time.deltaTime;
                    
                    // Timeline transition
                    if(m_isTimelineTransitionInProgress) DoTimelineTransition(m_timelineSourceTransitionTime, m_timelineDestinationTransitionTime);

                    // Previous day
                    if (timeline < 0)
                    {
                        DecreaseDay();
                        m_skyController.OnDayChange();
                        timeline = 24;
                    }
                }
                
                // On minute change event
                if (m_previousMinute != minute)
                {
                    m_skyController.onMinuteChange?.Invoke();
                    m_previousMinute = minute;
                }
                
                // On hour change event
                if (m_previousHour != hour)
                {
                    m_skyController.onHourChange?.Invoke();
                    m_previousHour = hour;
                }
                
                UpdateTimeSystem();
            }
            
            // Editor only
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                // Update the time system every frame when in edit mode
                UpdateTimeSystem();
            }
            #endif
        }

        public void UpdateTimeSystem()
        {
            switch (timeSystem)
            {
                case AzureTimeSystem.Simple:
                    m_sunSimpleRotation = GetSunSimpleRotation();
                    m_skyController.sunTransform.localRotation = m_sunSimpleRotation;
                    m_moonSimpleRotation = m_sunSimpleRotation * Quaternion.Euler(0, -180, 0);
                    m_skyController.moonTransform.localRotation = m_moonSimpleRotation;

                    m_starFieldMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(m_skyController.starFieldPosition), Vector3.one).inverse * m_skyController.sunTransform.transform.worldToLocalMatrix;
                    if (m_skyController.shaderUpdateMode == AzureShaderUpdateMode.ByMaterial)
                        m_skyController.skyMaterial.SetMatrix(AzureShaderUniforms.StarFieldMatrix, m_starFieldMatrix);
                    else
                        Shader.SetGlobalMatrix(AzureShaderUniforms.StarFieldMatrix, m_starFieldMatrix);
                    break;
				
                case AzureTimeSystem.Realistic:
                    m_sunRealisticRotation = GetSunRealisticRotation();
                    m_skyController.sunTransform.forward = transform.TransformDirection(m_sunRealisticRotation);
                    m_moonRealisticRotation = GetMoonRealisticRotation();
                    m_skyController.moonTransform.forward = transform.TransformDirection(m_moonRealisticRotation);
					
                    m_starFieldOffset.y = longitude;
                    m_starFieldMatrix = Matrix4x4.TRS(Vector3.zero, GetCelestialRotation() * Quaternion.Euler(m_skyController.starFieldPosition - m_starFieldOffset), Vector3.one);
                    if (m_skyController.shaderUpdateMode == AzureShaderUpdateMode.ByMaterial)
                        m_skyController.skyMaterial.SetMatrix(AzureShaderUniforms.StarFieldMatrix, m_starFieldMatrix.inverse);
                    else
                        Shader.SetGlobalMatrix(AzureShaderUniforms.StarFieldMatrix, m_starFieldMatrix.inverse);
                    break;
            }
            
            // Gets the local direction of the sun and moon
            m_sunLocalDirection = transform.InverseTransformDirection(m_skyController.sunTransform.forward);
            m_moonLocalDirection = transform.InverseTransformDirection(m_skyController.moonTransform.forward);

            // Update shader directions
            if (m_skyController.shaderUpdateMode == AzureShaderUpdateMode.ByMaterial)
            {
                m_skyController.skyMaterial.SetVector(AzureShaderUniforms.SunDirection, -m_sunLocalDirection);
                m_skyController.skyMaterial.SetVector(AzureShaderUniforms.MoonDirection, -m_moonLocalDirection);
                m_skyController.skyMaterial.SetMatrix(AzureShaderUniforms.SunMatrix, m_skyController.sunTransform.worldToLocalMatrix);
                m_skyController.skyMaterial.SetMatrix(AzureShaderUniforms.MoonMatrix, m_skyController.moonTransform.worldToLocalMatrix);
                m_skyController.skyMaterial.SetMatrix(AzureShaderUniforms.UpDirectionMatrix, transform.worldToLocalMatrix);
                m_skyController.fogMaterial.SetVector(AzureShaderUniforms.SunDirection, -m_sunLocalDirection);
                m_skyController.fogMaterial.SetVector(AzureShaderUniforms.MoonDirection, -m_moonLocalDirection);
                m_skyController.fogMaterial.SetMatrix(AzureShaderUniforms.SunMatrix, m_skyController.sunTransform.worldToLocalMatrix);
                m_skyController.fogMaterial.SetMatrix(AzureShaderUniforms.MoonMatrix, m_skyController.moonTransform.worldToLocalMatrix);
                m_skyController.fogMaterial.SetMatrix(AzureShaderUniforms.UpDirectionMatrix, transform.worldToLocalMatrix);
            }
            else
            {
                Shader.SetGlobalVector(AzureShaderUniforms.SunDirection, -m_sunLocalDirection);
                Shader.SetGlobalVector(AzureShaderUniforms.MoonDirection, -m_moonLocalDirection);
                Shader.SetGlobalMatrix(AzureShaderUniforms.SunMatrix, m_skyController.sunTransform.worldToLocalMatrix);
                Shader.SetGlobalMatrix(AzureShaderUniforms.MoonMatrix, m_skyController.moonTransform.worldToLocalMatrix);
                Shader.SetGlobalMatrix(AzureShaderUniforms.UpDirectionMatrix, transform.worldToLocalMatrix);
            }

            // Gets the sun and moon elevation and sets the directional light rotation
            m_sunElevation = Vector3.Dot(-m_sunLocalDirection, Vector3.up);
            m_moonElevation = Vector3.Dot(-m_moonLocalDirection, Vector3.up);
            m_skyController.directionalLight.transform.localRotation = Quaternion.LookRotation(m_sunElevation >= 0.0f ? m_sunLocalDirection : m_moonLocalDirection);
            m_skyController.timeOfDay = m_timeOfDay;
            m_skyController.sunElevation = m_sunElevation;
            m_skyController.moonElevation = m_moonElevation;
        }
        
        /// <summary>
        /// Adjust the calendar when there is a change in the date.
        /// </summary>
        public void UpdateCalendar()
        {
            // Get the number of days in the current month
            m_daysInMonth = DateTime.DaysInMonth(year, month);

            // Avoid selecting a date that does not exist
            day = Mathf.Clamp(day, 1, m_daysInMonth);
            month = Mathf.Clamp(month, 1, 12);
            year = Mathf.Clamp(year, 0, 9999);

            // Creates a custom DateTime at the first day of the current month
            m_dateTime = new DateTime(year, month, 1);
            // Gets the day of week corresponding to this custom DateTime
            m_dayOfWeek = (int) m_dateTime.DayOfWeek;
            // Keeps the same day selected in the calendar even when the date is changed externally.
            selectedCalendarDay = day - 1 + m_dayOfWeek;
            
            for (int i = 0; i < DayList.Length; i++)
            {
                // Make null all the calendar buttons
                if (i < m_dayOfWeek || i >= (m_dayOfWeek + m_daysInMonth))
                {
                    DayList[i] = "";
                    continue;
                }

                // Sets the day number only on the valid buttons of the current month in use by the calendar.
                m_dateTime = new DateTime(year, month, (i - m_dayOfWeek) + 1);
                DayList[i] = m_dateTime.Day.ToString();
            }
        }
        
        /// <summary>
        /// String array that stores the name of each day of the week.
        /// </summary>
        public readonly string[] WeekList = new string[]
        {
            "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"
        };
        
        /// <summary>
        /// String array that stores the name of each month.
        /// </summary>
        public readonly string[] MonthList = new string[]
        {
            "January", "February", "March", "April", "May", "June",
            "July", "August", "September", "October", "November", "December"
        };
        
        /// <summary>
        /// Array with 42 numeric strings used to fill a calendar.
        /// </summary>
        public readonly string[] DayList = new string[]
        {
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
            "11", "12", "13", "14", "15", "16", "17", "18", "19", "20",
            "21", "22", "23", "24", "25", "26", "27", "28", "29", "30",
            "31", "32", "33", "34", "35", "36", "37", "38", "39", "40", "41"
        };
        
        /// <summary>
        /// Computes the time progression step based on the day length value.
        /// </summary>
        private float GetTimeProgressionStep()
        {
            if (dayLength > 0.0f)
                return (24.0f / 60.0f) / dayLength;
            else
                return 0.0f;
        }
        
        /// <summary>
        /// Gets the current day of the week and return an integer between 0 and 6.
        /// </summary>
        public int GetDayOfWeek()
        {
            m_dateTime = new DateTime(year, month, day);
            return (int) m_dateTime.DayOfWeek;
        }
        
        /// <summary>
        /// Gets the day of the week from a custom date and return an integer between 0 and 6.
        /// </summary>
        public int GetDayOfWeek(int year, int month, int day)
        {
            m_dateTime = new DateTime(year, month, day);
            return (int) m_dateTime.DayOfWeek;
        }
        
        /// <summary>
        /// Gets the current day of the week and return as string.
        /// </summary>
        public string GetDayOfWeekString()
        {
            m_dateTime = new DateTime(year, month, day);
            return WeekList[(int) m_dateTime.DayOfWeek];
        }
        
        /// <summary>
        /// Gets the day of the week from a custom date and return as string.
        /// </summary>
        public string GetDayOfWeekString(int year, int month, int day)
        {
            m_dateTime = new DateTime(year, month, day);
            return WeekList[(int) m_dateTime.DayOfWeek];
        }
        
        /// <summary>
        /// Set the timeline using a float as parameter.
        /// </summary>
        public void SetTimeline(float value)
        {
            timeline = value;
        }
        
        /// <summary>
        /// Set the timeline using a slider as parameter.
        /// </summary>
        public void SetTimeline(Slider slider)
        {
            timeline = slider.value;
        }
        
        public void SetTimelineTransitionTime(float transitionTime)
		{
			m_timelineTransitionSpeed = transitionTime;
		}

		public void SetTimelineSourceTransitionTime(float source)
		{
			m_timelineSourceTransitionTime = source;
		}
		
		public void SetTimelineDestinationTransitionTime(float destination)
		{
			m_timelineDestinationTransitionTime = destination;
		}
		
		/// <summary>
		/// Transition the timeline from source to destination in a period of time (in seconds).
		/// </summary>
		public void StartTimelineTransition(float source, float destination, float transitionTime)
		{
			m_timelineTransitionSpeed = transitionTime;
			m_startTimelineTransitionStep = Time.time;
			m_timelineSourceTransitionTime = source;
			m_timelineDestinationTransitionTime = destination;
			m_isTimelineTransitionInProgress = true;
		}
		
		/// <summary>
		/// Transition the timeline from current time of day to destination in a period of time (in seconds).
		/// </summary>
		public void StartTimelineTransition(float destination, float transitionTime)
		{
			m_timelineTransitionSpeed = transitionTime;
			m_startTimelineTransitionStep = Time.time;
			m_timelineSourceTransitionTime = timeline;
			m_timelineDestinationTransitionTime = destination;
			m_isTimelineTransitionInProgress = true;
		}
		
		/// <summary>
		/// Transition the timeline from current time of day to destination in a period of time (in seconds).
		/// </summary>
		public void StartTimelineTransition(float destination)
		{
			m_startTimelineTransitionStep = Time.time;
			m_timelineSourceTransitionTime = timeline;
			m_timelineDestinationTransitionTime = destination;
			m_isTimelineTransitionInProgress = true;
		}
		
		/// <summary>
		/// Transition the timeline from current time of day to destination in a period of time (in seconds).
		/// </summary>
		public void StartTimelineTransition()
		{
			m_startTimelineTransitionStep = Time.time;
			m_isTimelineTransitionInProgress = true;
		}
		
		private void DoTimelineTransition(float source, float destination)
		{
			// Computes the lerp t=time
			m_timelineTransitionStep = Mathf.Clamp01((Time.time - m_startTimelineTransitionStep) / m_timelineTransitionSpeed);
         
			// Apply the lerp transition to Azure[Sky] timeline
			timeline = Mathf.Lerp(source, destination, m_timelineTransitionStep);

			// Ends the lerp transition
			if (Mathf.Abs(m_timelineTransitionStep - 1.0f) <= 0.0f)
			{
				m_isTimelineTransitionInProgress = false;
			}
		}
        
        /// <summary>
        /// Sets a new custom date.
        /// </summary>
        public void SetNewDate(int year, int month, int day)
        {
            this.year = year;
            this.month = month;
            this.day = day;
            UpdateCalendar();
        }
        
        /// <summary>
        /// Sets a new custom day.
        /// </summary>
        public void SetNewDay(int day)
        {
            this.day = day;
            UpdateCalendar();
        }
        
        /// <summary>
        /// Increase the day number.
        /// </summary>
        public void IncreaseDay()
        {
            if (repeatMode != AzureTimeRepeatMode.ByDay)
            {
                day++;

                if (day > m_daysInMonth)
                {
                    day = 1;
                    IncreaseMonth();
                }
            }
            
            UpdateCalendar();
        }
        
        /// <summary>
        /// Decrease the day number.
        /// </summary>
        public void DecreaseDay()
        {
            if (repeatMode != AzureTimeRepeatMode.ByDay)
            {
                day--;
                m_previousMonth = repeatMode == AzureTimeRepeatMode.ByMonth ? month : month - 1;
                if (m_previousMonth < 1) m_previousMonth = 12;
                m_previousDaysInMonth = DateTime.DaysInMonth(year, m_previousMonth);

                if (day < 1)
                {
                    day = m_previousDaysInMonth;
                    DecreaseMonth();
                }
            }
            
            UpdateCalendar();
        }
        
        /// <summary>
        /// Sets a new custom month.
        /// </summary>
        public void SetNewMonth(int month)
        {
            this.month = month;
            UpdateCalendar();
        }
        
        /// <summary>
        /// Increase the month number.
        /// </summary>
        public void IncreaseMonth()
        {
            if (repeatMode != AzureTimeRepeatMode.ByMonth)
            {
                month++;

                if (month > 12)
                {
                    month = 1;
                    IncreaseYear();
                }
            }
            
            UpdateCalendar();
        }
        
        /// <summary>
        /// Decrease the month number.
        /// </summary>
        public void DecreaseMonth()
        {
            if (repeatMode != AzureTimeRepeatMode.ByMonth)
            {
                month--;

                if (month < 1)
                {
                    month = 12;
                    DecreaseYear();
                }
            }
            
            UpdateCalendar();
        }
        
        /// <summary>
        /// Sets a new custom year.
        /// </summary>
        public void SetNewYear(int year)
        {
            this.year = year;
            UpdateCalendar();
        }
        
        /// <summary>
        /// Increase the year number.
        /// </summary>
        public void IncreaseYear()
        {
            if (repeatMode != AzureTimeRepeatMode.ByYear)
            {
                year++;
                if (year > 9999) year = 0;
            }

            UpdateCalendar();
        }
        
        /// <summary>
        /// Decrease the year number.
        /// </summary>
        public void DecreaseYear()
        {
            if (repeatMode != AzureTimeRepeatMode.ByYear)
            {
                year--;
                if (year < 0) year = 9999;
            }

            UpdateCalendar();
        }
        
        /// <summary>
        /// Returns the current date converted to string using the default format used by Azure.
        /// </summary>
        public string GetDateString()
        {
            // Format: "MMMM dd, yyyy"
            return MonthList[month - 1] + " " + day.ToString("00") + ", " + year.ToString("0000");
        }
        
        /// <summary>
        /// Returns the current date converted to string using a custom format.
        /// </summary>
        public string GetDateString(string format)
        {
            m_dateTime = new DateTime(year, month, day);
            return m_dateTime.ToString(format);
        }
        
        /// <summary>
        /// Computes the sun simple rotation
        /// </summary>
        private Quaternion GetSunSimpleRotation()
        {
            return Quaternion.Euler(0.0f, longitude, -latitude) * Quaternion.Euler(((m_timeOfDay + utc) * 360.0f / 24.0f) - 90.0f, 180.0f, 0.0f);
        }
        
        /// <summary>
        /// Computes celestial rotation.
        /// </summary>
        public Quaternion GetCelestialRotation()
        {
            return Quaternion.Euler(90.0f - latitude, 0.0f, 0.0f) * Quaternion.Euler(0.0f, longitude, 0.0f) * Quaternion.Euler(0.0f, m_lst * Mathf.Rad2Deg, 0.0f);
        }
        
        /// <summary>
        /// Computes the sun realistic rotation based on time, date and location.
        /// </summary>
        //  Based on formulas from Paul Schlyter's web page: http://www.stjarnhimlen.se/comp/ppcomp.html
        public Vector3 GetSunRealisticRotation()
        {
            m_radians = (Mathf.PI * 2.0f) / 360.0f; // Used to convert degrees to radians
            m_radLatitude = m_radians * latitude;
            m_sinLatitude = Mathf.Sin(m_radLatitude);
            m_cosLatitude = Mathf.Cos(m_radLatitude);

            float hour = m_timeOfDay - utc;
            
            // Time Scale
            //--------------------------------------------------
            // d = 367 * y - 7 * (y + (m + 9) / 12) / 4 + 275 * m / 9 + D - 730530
            // d = d + UT / 24.0
            float d = 367 * year - 7 * (year + (month + 9) / 12) / 4 + 275 * month / 9 + day - 730530;
            d = d + hour / 24.0f;

            // Tilt of earth's axis
            //--------------------------------------------------
            // Obliquity of the ecliptic
            float ecliptic = 23.4393f - 3.563E-7f * d;
            // Need convert to radians before apply sine and cosine
            float radEcliptic = m_radians * ecliptic;
            float sinEcliptic = Mathf.Sin(radEcliptic);
            float cosEcliptic = Mathf.Cos(radEcliptic);

            // Orbital elements of the sun
            //--------------------------------------------------
            //float N = 0.0;
            //float i = 0.0;
            float w = 282.9404f + 4.70935E-5f * d;
            //float a = 1.000000f;
            float e = 0.016709f - 1.151E-9f * d;
            float M = 356.0470f + 0.9856002585f * d;

            // Eccentric anomaly
            //--------------------------------------------------
            // E = M + e*(180/pi) * sin(M) * ( 1.0 + e * cos(M)) in degrees
            // E = M + e * sin(M) * (1.0 + e * cos(M)) in radians
            // Need convert to radians before apply sine and cosine
            float radM = m_radians * M;
            float sinM = Mathf.Sin(radM);
            float cosM = Mathf.Cos(radM);

            // Need convert to radians before apply sine and cosine
            float radE = radM + e * sinM * (1.0f + e * cosM);
            float sinE = Mathf.Sin(radE);
            float cosE = Mathf.Cos(radE);

            // Sun's distance (r) and its true anomaly (v)
            //--------------------------------------------------
            // Xv = r * cos(v) = cos(E) - e
            // Yv = r * sen(v) = sqrt(1,0 - e * e) * sen(E)
            float xv = cosE - e;
            float yv = Mathf.Sqrt(1.0f - e * e) * sinE;

            // V = atan2(yv, xv)
            // R = sqrt(xv * xv + yv * yv)
            float v = Mathf.Rad2Deg * Mathf.Atan2(yv, xv);
            float r = Mathf.Sqrt(xv * xv + yv * yv);

            // Sun's true longitude
            //--------------------------------------------------
            float radLongitude = m_radians * (v + w);
            float sinLongitude = Mathf.Sin(radLongitude);
            float cosLongitude = Mathf.Cos(radLongitude);

            float xs = r * cosLongitude;
            float ys = r * sinLongitude;

            // Equatorial coordinates
            //--------------------------------------------------
            float xe = xs;
            float ye = ys * cosEcliptic;
            float ze = ys * sinEcliptic;

            // Sun's Right Ascension(RA) and Declination(Dec)
            //--------------------------------------------------
            float RA = Mathf.Atan2(ye, xe);
            float Dec = Mathf.Atan2(ze, Mathf.Sqrt(xe * xe + ye * ye));
            float sinDec = Mathf.Sin(Dec);
            float cosDec = Mathf.Cos(Dec);

            // The Sidereal Time
            //--------------------------------------------------
            float Ls = v + w;
            float GMST0 = Ls + 180.0f;
            float UT = 15.0f * hour;//Universal Time.
            float GMST = GMST0 + UT;
            float LST = m_radians * (GMST + longitude);

            // Store local sideral time
            m_lst = LST;

            // Azimuthal coordinates
            //--------------------------------------------------
            float HA = LST - RA;
            float sinHA = Mathf.Sin(HA);
            float cosHA = Mathf.Cos(HA);

            float x = cosHA * cosDec;
            float y = sinHA * cosDec;
            float z = sinDec;

            float xhor = x * m_sinLatitude - z * m_cosLatitude;
            float yhor = y;
            float zhor = x * m_cosLatitude + z * m_sinLatitude;

            // az  = atan2(yhor, xhor) + 180_degrees
            // alt = asin(zhor) = atan2(zhor, sqrt(xhor*xhor+yhor*yhor))
            float azimuth = Mathf.Atan2(yhor, xhor) + m_radians * 180.0f;
            float altitude = Mathf.Asin(zhor);

            // Zenith angle
            // Zenith=90°−α  Where α is the elevation angle
            float zenith = 90.0f * m_radians - altitude;

            // Converts from Spherical(radius r, zenith-inclination θ, azimuth φ) to Cartesian(x,y,z) coordinates
            // https://en.wikipedia.org/wiki/Spherical_coordinate_system
            //--------------------------------------------------
            // x = r sin(θ)cos(φ)​​
            // y = r sin(θ)sin(φ)
            // z = r cos(θ)
            Vector3 ret;

            // radius = 1
            ret.z = Mathf.Sin(zenith) * Mathf.Cos(azimuth);
            ret.x = Mathf.Sin(zenith) * Mathf.Sin(azimuth);
            ret.y = Mathf.Cos(zenith);

            return ret * -1.0f;
        }
        
        /// <summary>
        /// Computes the moon realistic rotation based on time, date and location.
        /// </summary>
        //  Based on formulas from Paul Schlyter's web page: http://www.stjarnhimlen.se/comp/ppcomp.html
        public Vector3 GetMoonRealisticRotation()
        {
            float hour = m_timeOfDay - utc;

            // Time Scale
            //--------------------------------------------------
            // d = 367 * y - 7 * (y + (m + 9) / 12) / 4 + 275 * m / 9 + D - 730530
            // d = d + UT / 24.0
            float d = 367 * year - 7 * (year + (month + 9) / 12) / 4 + 275 * month / 9 + day - 730530;
            d = d + hour / 24.0f;

            // Tilt of earth's axis
            //--------------------------------------------------
            // obliquity of the ecliptic
            float ecliptic = 23.4393f - 3.563E-7f * d;
            // Need convert to radians before apply sine and cosine
            float radEcliptic = m_radians * ecliptic;
            float sinEcliptic = Mathf.Sin(radEcliptic);
            float cosEcliptic = Mathf.Cos(radEcliptic);

            // Orbital elements of the Moon
            //--------------------------------------------------
            float N = 125.1228f - 0.0529538083f * d;
            float i = 5.1454f;
            float w = 318.0634f + 0.1643573223f * d;
            float a = 60.2666f;
            float e = 0.054900f;
            float M = 115.3654f + 13.0649929509f * d;

            // Eccentric anomaly
            //--------------------------------------------------
            // E = M + e*(180/pi) * sin(M) * (1.0 + e * cos(M))
            float radM = m_radians * M;
            float E = radM + e * Mathf.Sin(radM) * (1f + e * Mathf.Cos(radM));

            // Planet's distance and true anomaly
            //--------------------------------------------------
            // xv = r * cos(v) = a * (cos(E) - e)
            // yv = r * sin(v) = a * (sqrt(1.0 - e*e) * sin(E))
            float xv = a * (Mathf.Cos(E) - e);
            float yv = a * (Mathf.Sqrt(1f - e * e) * Mathf.Sin(E));
            // V = atan2 (yv, xv)
            // R = sqrt (xv * xv + yv * yv)
            float v = Mathf.Rad2Deg * Mathf.Atan2(yv, xv);
            float r = Mathf.Sqrt(xv * xv + yv * yv);

            // Moon position in 3D space
            //--------------------------------------------------
            float radLongitude = m_radians * (v + w);
            float sinLongitude = Mathf.Sin(radLongitude);
            float cosLongitude = Mathf.Cos(radLongitude);

            // Geocentric (Earth-centered) coordinates
            //--------------------------------------------------
            // xh = r * (cos(N) * cos(v+w) - sin(N) * sin(v+w) * cos(i))
            // yh = r * (sin(N) * cos(v+w) + cos(N) * sin(v+w) * cos(i))
            // zh = r * (sin(v+w) * sin(i))
            float radN = m_radians * N;
            float radI = m_radians * i;

            float xh = r * (Mathf.Cos(radN) * cosLongitude - Mathf.Sin(radN) * sinLongitude * Mathf.Cos(radI));
            float yh = r * (Mathf.Sin(radN) * cosLongitude + Mathf.Cos(radN) * sinLongitude * Mathf.Cos(radI));
            float zh = r * (sinLongitude * Mathf.Sin(radI));

            // No needed to the moon
            // float xg = xh;
            // float yg = yh;
            // float zg = zh;

            // Equatorial coordinates
            //--------------------------------------------------
            float xe = xh;
            float ye = yh * cosEcliptic - zh * sinEcliptic;
            float ze = yh * sinEcliptic + zh * cosEcliptic;

            // Planet's Right Ascension (RA) and Declination (Dec)
            //--------------------------------------------------
            float RA = Mathf.Atan2(ye, xe);
            float Dec = Mathf.Atan2(ze, Mathf.Sqrt(xe * xe + ye * ye));

            // The Sidereal Time
            //--------------------------------------------------
            // It is already calculated for the sun and stored in the lst, it is not necessary to calculate again for the moon
            
            //float Ls = v + w;
            //float GMST0 = Ls + 180.0f;
            //float UT    = 15.0f * hour;
            //float GMST  = GMST0 + UT;
            //float LST   = radians * (GMST + Azure_Longitude);

            // Azimuthal coordinates
            //--------------------------------------------------
            float HA = m_lst - RA;

            float x = Mathf.Cos(HA) * Mathf.Cos(Dec);
            float y = Mathf.Sin(HA) * Mathf.Cos(Dec);
            float z = Mathf.Sin(Dec);

            float xhor = x * m_sinLatitude - z * m_cosLatitude;
            float yhor = y;
            float zhor = x * m_cosLatitude + z * m_sinLatitude;

            // az  = atan2(yhor, xhor) + 180_degrees
            // alt = asin(zhor) = atan2(zhor, sqrt(xhor*xhor+yhor*yhor))
            float azimuth = Mathf.Atan2(yhor, xhor) + m_radians * 180.0f;
            float altitude = Mathf.Asin(zhor);

            // Zenith angle
            // Zenith = 90°−α  where α is the elevation angle
            float zenith = 90.0f * m_radians - altitude;

            // Converts from Spherical(radius r, zenith-inclination θ, azimuth φ) to Cartesian(x,y,z) coordinates
            // https://en.wikipedia.org/wiki/Spherical_coordinate_system
            //--------------------------------------------------
            // x = r sin(θ)cos(φ)​​
            // y = r sin(θ)sin(φ)
            // z = r cos(θ)
            Vector3 ret;

            //radius = 1
            ret.z = Mathf.Sin(zenith) * Mathf.Cos(azimuth);
            ret.x = Mathf.Sin(zenith) * Mathf.Sin(azimuth);
            ret.y = Mathf.Cos(zenith);

            return ret * -1.0f;
        }
    }
}