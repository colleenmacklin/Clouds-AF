using UnityEngine;
using UnityEngine.UI;

namespace Crosstales.RTVoice.Demo
{
   /// <summary>Simple GUI for runtime TTS with all available OS voices.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_demo_1_1_g_u_i_speech.html")]
   public class GUISpeech : MonoBehaviour
   {
      #region Variables

      [Header("Settings")] public bool StartAsNative;
      public GUIMultiAudioFilter AudioFilter;

      [Header("Table")] public GameObject ItemPrefab;
      public GameObject Target;
      public Scrollbar Scroll;

      public int ColumnCount = 1;
      public Vector2 SpaceWidth = new Vector2(8, 8);
      public Vector2 SpaceHeight = new Vector2(8, 8);

      [Header("UI Objects")] public InputField Input;
      public InputField Culture;
      public Text Cultures;
      public Text Voices;

      public static float Rate = 1f;
      public static float Pitch = 1f;
      public static float Volume = 1f;
      public static bool isNative;

      private string lastCulture = "unknown";
      //private System.Collections.Generic.List<SpeakWrapper> wrappers = new System.Collections.Generic.List<SpeakWrapper>();

      private System.Collections.Generic.List<Crosstales.RTVoice.Model.Voice> items = new System.Collections.Generic.List<Crosstales.RTVoice.Model.Voice>();

      private Crosstales.RTVoice.Model.Enum.Gender gender = Crosstales.RTVoice.Model.Enum.Gender.UNKNOWN;

      //private bool isCustomProvider;

      private const string forceUpdate = "RT-Voice rulez!";

      #endregion


      #region MonoBehaviour methods

      private void Start()
      {
         Rate = 1f;
         Pitch = 1f;
         Volume = 1f;
         isNative = false;

         Speaker.Instance.OnProviderChange += onProviderChange;
         Speaker.Instance.OnVoicesReady += onVoicesReady;

         //isCustomProvider = Speaker.Instance.CustomMode;

         if (Cultures != null)
            Cultures.text = string.Join(", ", Speaker.Instance.Cultures.ToArray());

         if (Speaker.Instance.isSSMLSupported)
         {
            if (Input != null)
               Input.text = "Hi there, my name is RT-Voice, your runtime speaker!" + System.Environment.NewLine + "I can speak with the complete SSML specification <prosody rate=\"-50%\">at half speed</prosody> or <prosody pitch=\"-50%\">50% lower pitched.</prosody>. " + System.Environment.NewLine + "<prosody contour=\"(0%,+20%) (40%,+40%) (60%,+60%) (80%,+80%) (100%,+100%)\">I can talk with rising intonation</prosody> <prosody contour=\"(0%,-20%) (40%,-40%) (60%,-60%) (80%,-80%) (100%,-100%)\">or with falling intonation.</prosody>" + System.Environment.NewLine + "This is <emphasis level=\"strong\">awesome</emphasis>!";

            //Debug.Log(Input.text.Length);
         }
         else
         {
            if (Input != null)
               Input.text = "Hi there, my name is RT-Voice, your runtime speaker!";
         }

         if (Culture != null)
            Culture.text = string.Empty;

         isNative = StartAsNative;

         if (Voices != null)
            Voices.text = "Voices (" + items.Count + ")";
      }

      private void Update()
      {
         if (Culture != null && !lastCulture.Equals(Culture.text) && Speaker.Instance.areVoicesReady)
         {
            buildVoicesList();

            lastCulture = Culture.text;

            Cultures.text = string.Join(", ", Speaker.Instance.Cultures.ToArray());
         }
      }

      private void OnDestroy()
      {
         if (Speaker.Instance != null)
         {
            Speaker.Instance.OnProviderChange -= onProviderChange;
            Speaker.Instance.OnVoicesReady -= onVoicesReady;
         }
      }

      #endregion


      #region Public methods

      public void Silence()
      {
         Speaker.Instance.Silence();
      }

      public void ChangeRate(float rate)
      {
         Rate = rate;
      }

      public void ChangeVolume(float volume)
      {
         Volume = volume;
      }

      public void ChangePitch(float pitch)
      {
         Pitch = pitch;
      }

      public void ChangeNative(bool native)
      {
         isNative = native;
      }

      public void GenderChanged(int index)
      {
         gender = (Crosstales.RTVoice.Model.Enum.Gender)index;

         Invoke(nameof(buildVoicesList), 0.2f);
      }

      #endregion


      #region Private methods

      private void onProviderChange(string provider)
      {
         lastCulture = forceUpdate; //force update
      }

      private void onVoicesReady()
      {
         //Debug.Log("+++ Voices ready... +++");
         lastCulture = forceUpdate; //force update
      }

      private void clearVoicesList()
      {
         if (AudioFilter != null)
            AudioFilter.ClearFilters();

         if (Target != null)
         {
            for (int ii = Target.transform.childCount - 1; ii >= 0; ii--)
            {
               Transform child = Target.transform.GetChild(ii);
               child.SetParent(null);
               Destroy(child.gameObject);
            }
         }
      }

      private void buildVoicesList()
      {
         clearVoicesList(); //make sure everything is deleted

         if (Target != null)
         {
            RectTransform containerRectTransform = Target.GetComponent<RectTransform>();
            items = Speaker.Instance.VoicesForGender(gender, Culture.text);

            if (items.Count > 0)
            {
               //calculate the width and height of each child item.
               float width = containerRectTransform.rect.width / ColumnCount - SpaceWidth.x;
               float height = SpaceHeight.x + SpaceHeight.y;

               int rowCount = items.Count / ColumnCount;

               if (rowCount > 0 && items.Count % rowCount > 0)
                  rowCount++;

               //adjust the height of the container so that it will just barely fit all its children
               float scrollHeight = height * rowCount;
               containerRectTransform.offsetMin = new Vector2(containerRectTransform.offsetMin.x, -scrollHeight / 2);
               containerRectTransform.offsetMax = new Vector2(containerRectTransform.offsetMax.x, scrollHeight / 2);

               int jj = 0;
               for (int ii = 0; ii < items.Count; ii++)
               {
                  //this is used instead of a double for loop because itemCount may not fit perfectly into the rows/columns
                  if (ii % ColumnCount == 0)
                     jj++;

                  //create a new item, name it, and set the parent
                  GameObject newItem = Instantiate(ItemPrefab, Target.transform, true);
                  newItem.name = Target.name + " item at (" + ii + "," + jj + ")";
                  newItem.transform.localScale = Vector3.one;

                  if (AudioFilter != null)
                  {
                     AudioFilter.Sources.Add(newItem.GetComponent<AudioSource>());
                     AudioFilter.ReverbFilters.Add(newItem.GetComponent<AudioReverbFilter>());
                     AudioFilter.ChorusFilters.Add(newItem.GetComponent<AudioChorusFilter>());
                     AudioFilter.EchoFilters.Add(newItem.GetComponent<AudioEchoFilter>());
                     AudioFilter.DistortionFilters.Add(newItem.GetComponent<AudioDistortionFilter>());
                     AudioFilter.LowPassFilters.Add(newItem.GetComponent<AudioLowPassFilter>());
                     AudioFilter.HighPassFilters.Add(newItem.GetComponent<AudioHighPassFilter>());
                  }

                  SpeakWrapper wrapper = newItem.GetComponent<SpeakWrapper>();
                  wrapper.SpeakerVoice = items[ii];
                  wrapper.Input = Input;
                  wrapper.Label.text = items[ii].Name;
                  //wrappers.Add(wrapper);

                  //move and size the new item
                  RectTransform rectTransform = newItem.GetComponent<RectTransform>();

                  float x = (width + SpaceWidth.x) * (ii % ColumnCount) + SpaceWidth.x;
                  float y = -height * jj;

                  Vector2 offsetMin = new Vector2(x, y);
                  rectTransform.offsetMin = offsetMin;

                  x = offsetMin.x + width;
                  y = offsetMin.y + SpaceHeight.x;

                  rectTransform.offsetMax = new Vector2(x, y);
               }

               if (AudioFilter != null)
                  AudioFilter.ResetFilters();
            }

            if (Scroll != null)
               Scroll.value = 1f;
         }

         if (Voices != null)
            Voices.text = "Voices (" + items.Count + ")";
      }

      #endregion
   }
}
// © 2015-2022 crosstales LLC (https://www.crosstales.com)