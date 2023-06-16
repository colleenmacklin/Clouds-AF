using UnityEngine;
using UnityEngine.UI;

namespace Crosstales.RTVoice.Demo
{
   /// <summary>Simple native TTS example.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_demo_1_1_simple_native.html")]
   public class SimpleNative : MonoBehaviour
   {
      #region Variables

      [Header("Configuration")] [Range(0f, 3f)] public float RateSpeakerA = 1.25f;
      [Range(0f, 3f)] public float RateSpeakerB = 1.75f;
      [Range(0f, 3f)] public float RateSpeakerC = 2.5f;

      public bool PlayOnStart;

      [Header("UI Objects")] public Text TextSpeakerA;
      public Text TextSpeakerB;
      public Text TextSpeakerC;

      public Text PhonemeSpeakerA;
      public Text PhonemeSpeakerB;
      public Text PhonemeSpeakerC;

      public Text VisemeSpeakerA;
      public Text VisemeSpeakerB;
      public Text VisemeSpeakerC;

      private string uidSpeakerA;
      private string uidSpeakerB;
      private string uidSpeakerC;

      private string textA = "Text A";
      private string textB = "Text B";
      private string textC = "Text C";

      private bool silent = true;

      #endregion


      #region MonoBehaviour methods

      private void Start()
      {
         Speaker.Instance.OnSpeakCurrentWord += speakCurrentWordMethod;
         Speaker.Instance.OnSpeakCurrentPhoneme += speakCurrentPhonemeMethod;
         Speaker.Instance.OnSpeakCurrentViseme += speakCurrentVisemeMethod;
         Speaker.Instance.OnSpeakStart += speakStartMethod;
         Speaker.Instance.OnSpeakComplete += speakCompleteMethod;

         if (TextSpeakerA != null)
            textA = TextSpeakerA.text;

         if (TextSpeakerB != null)
            textB = TextSpeakerB.text;

         if (TextSpeakerC != null)
            textC = TextSpeakerC.text;

         if (PlayOnStart)
            Play();
      }

      private void OnDestroy()
      {
         if (Speaker.Instance != null)
         {
            // Unsubscribe event listeners
            Speaker.Instance.OnSpeakCurrentWord -= speakCurrentWordMethod;
            Speaker.Instance.OnSpeakCurrentPhoneme -= speakCurrentPhonemeMethod;
            Speaker.Instance.OnSpeakCurrentViseme -= speakCurrentVisemeMethod;
            Speaker.Instance.OnSpeakStart -= speakStartMethod;
            Speaker.Instance.OnSpeakComplete -= speakCompleteMethod;
         }
      }

      #endregion


      #region Public methods

      public void Play()
      {
         silent = false;

         if (TextSpeakerA != null)
            TextSpeakerA.text = textA;

         if (TextSpeakerB != null)
            TextSpeakerB.text = textB;

         if (TextSpeakerC != null)
            TextSpeakerC.text = textC;

         SpeakerA(); //start with speaker A
         //SpeakerB(); //start with speaker B
         //SpeakerC(); //start with speaker C
      }

      public void SpeakerA()
      {
         uidSpeakerA = Speaker.Instance.SpeakNative(textA, Speaker.Instance.VoiceForGender(Crosstales.RTVoice.Model.Enum.Gender.MALE, "en"), RateSpeakerA);
      }

      public void SpeakerB()
      {
         uidSpeakerB = Speaker.Instance.SpeakNative(textB, Speaker.Instance.VoiceForGender(Crosstales.RTVoice.Model.Enum.Gender.FEMALE, "en"), RateSpeakerB);
      }

      public void SpeakerC()
      {
         //default voice
         uidSpeakerC = Speaker.Instance.SpeakNative(textC, Speaker.Instance.VoiceForGender(Crosstales.RTVoice.Model.Enum.Gender.MALE, "en", 1), RateSpeakerC);
      }

      public void Silence()
      {
         silent = true;
         Speaker.Instance.Silence();

         if (TextSpeakerA != null)
            TextSpeakerA.text = textA;

         if (TextSpeakerB != null)
            TextSpeakerB.text = textB;

         if (TextSpeakerC != null)
            TextSpeakerC.text = textC;

         VisemeSpeakerC.text = PhonemeSpeakerC.text = VisemeSpeakerB.text = PhonemeSpeakerB.text = VisemeSpeakerA.text = PhonemeSpeakerA.text = "-";
      }

      #endregion


      #region Callback methods

      private void speakStartMethod(Crosstales.RTVoice.Model.Wrapper wrapper)
      {
         if (wrapper.Uid.Equals(uidSpeakerA))
         {
            if (RTVoice.Util.Config.DEBUG)
               Debug.Log("Speaker A - Speech start: " + wrapper, this);
         }
         else if (wrapper.Uid.Equals(uidSpeakerB))
         {
            if (RTVoice.Util.Config.DEBUG)
               Debug.Log("Speaker B - Speech start: " + wrapper, this);
         }
         else if (wrapper.Uid.Equals(uidSpeakerC))
         {
            if (RTVoice.Util.Config.DEBUG)
               Debug.Log("Speaker C - Speech start: " + wrapper, this);
         }
      }

      private void speakCompleteMethod(Crosstales.RTVoice.Model.Wrapper wrapper)
      {
         if (wrapper.Uid.Equals(uidSpeakerA))
         {
            if (RTVoice.Util.Config.DEBUG)
               Debug.Log("Speaker A - Speech complete: " + wrapper, this);

            if (TextSpeakerA != null)
               TextSpeakerA.text = wrapper.Text;

            if (VisemeSpeakerA != null)
               VisemeSpeakerA.text = PhonemeSpeakerA.text = "-";

            if (!silent)
               SpeakerB();
         }
         else if (wrapper.Uid.Equals(uidSpeakerB))
         {
            if (RTVoice.Util.Config.DEBUG)
               Debug.Log("Speaker B - Speech complete: " + wrapper, this);

            if (TextSpeakerB != null)
               TextSpeakerB.text = wrapper.Text;

            if (VisemeSpeakerB != null)
               VisemeSpeakerB.text = PhonemeSpeakerB.text = "-";

            if (!silent)
               SpeakerC();
         }
         else if (wrapper.Uid.Equals(uidSpeakerC))
         {
            if (RTVoice.Util.Config.DEBUG)
               Debug.Log("Speaker C - Speech complete: " + wrapper, this);

            if (TextSpeakerC != null)
               TextSpeakerC.text = wrapper.Text;

            if (VisemeSpeakerC != null)
               VisemeSpeakerC.text = PhonemeSpeakerC.text = "-";

            if (!silent)
               SpeakerA();
         }
      }

      private void speakCurrentWordMethod(Crosstales.RTVoice.Model.Wrapper wrapper, string[] speechTextArray, int wordIndex)
      {
         if (wrapper.Uid.Equals(uidSpeakerA))
         {
            if (TextSpeakerA != null)
               TextSpeakerA.text = RTVoice.Util.Helper.MarkSpokenText(speechTextArray, wordIndex);
         }
         else if (wrapper.Uid.Equals(uidSpeakerB))
         {
            if (TextSpeakerB != null)
               TextSpeakerB.text = RTVoice.Util.Helper.MarkSpokenText(speechTextArray, wordIndex);
         }
         else if (wrapper.Uid.Equals(uidSpeakerC))
         {
            if (TextSpeakerC != null)
               TextSpeakerC.text = RTVoice.Util.Helper.MarkSpokenText(speechTextArray, wordIndex);
         }
      }

      private void speakCurrentPhonemeMethod(Crosstales.RTVoice.Model.Wrapper wrapper, string phoneme)
      {
         if (wrapper.Uid.Equals(uidSpeakerA))
         {
            if (PhonemeSpeakerA != null)
               PhonemeSpeakerA.text = phoneme;
         }
         else if (wrapper.Uid.Equals(uidSpeakerB))
         {
            if (PhonemeSpeakerB != null)
               PhonemeSpeakerB.text = phoneme;
         }
         else if (wrapper.Uid.Equals(uidSpeakerC))
         {
            if (PhonemeSpeakerC != null)
               PhonemeSpeakerC.text = phoneme;
         }
      }

      private void speakCurrentVisemeMethod(Crosstales.RTVoice.Model.Wrapper wrapper, string viseme)
      {
         if (wrapper.Uid.Equals(uidSpeakerA))
         {
            if (VisemeSpeakerA != null)
               VisemeSpeakerA.text = viseme;
         }
         else if (wrapper.Uid.Equals(uidSpeakerB))
         {
            if (VisemeSpeakerB != null)
               VisemeSpeakerB.text = viseme;
         }
         else if (wrapper.Uid.Equals(uidSpeakerC))
         {
            if (VisemeSpeakerC != null)
               VisemeSpeakerC.text = viseme;
         }
      }

      #endregion
   }
}
// © 2015-2022 crosstales LLC (https://www.crosstales.com)