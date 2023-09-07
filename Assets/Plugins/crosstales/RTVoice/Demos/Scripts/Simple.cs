using UnityEngine;
using UnityEngine.UI;

namespace Crosstales.RTVoice.Demo
{
   /// <summary>Simple TTS example.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_demo_1_1_simple.html")]
   public class Simple : MonoBehaviour
   {
      #region Variables

      [Header("Configuration")] public AudioSource SourceA;
      public AudioSource SourceB;

      [Range(0f, 3f)] public float RateSpeakerA = 1.25f;

      [Range(0f, 3f)] public float RateSpeakerB = 1.75f;

      public bool PlayOnStart;

      [Header("UI Objects")] public Text TextSpeakerA;
      public Text TextSpeakerB;

      public Text PhonemeSpeakerA;
      public Text PhonemeSpeakerB;

      public Text VisemeSpeakerA;
      public Text VisemeSpeakerB;

      private string uidSpeakerA;
      private string uidSpeakerB;

      private string textA = "Text A";
      private string textB = "Text B";

      private Crosstales.RTVoice.Model.Wrapper currentWrapper;

      private bool silent = true;

      #endregion


      #region MonoBehaviour methods

      public void Start()
      {
         Speaker.Instance.OnSpeakAudioGenerationStart += speakAudioGenerationStartMethod;
         Speaker.Instance.OnSpeakAudioGenerationComplete += speakAudioGenerationCompleteMethod;
         Speaker.Instance.OnSpeakCurrentWord += speakCurrentWordMethod;
         Speaker.Instance.OnSpeakCurrentPhoneme += speakCurrentPhonemeMethod;
         Speaker.Instance.OnSpeakCurrentViseme += speakCurrentVisemeMethod;
         Speaker.Instance.OnSpeakStart += speakStartMethod;
         Speaker.Instance.OnSpeakComplete += speakCompleteMethod;

         if (TextSpeakerA != null)
            textA = TextSpeakerA.text;

         if (TextSpeakerB != null)
            textB = TextSpeakerB.text;

         if (PlayOnStart)
            Play();
      }

      private void OnDestroy()
      {
         if (Speaker.Instance != null)
         {
            // Unsubscribe event listeners
            Speaker.Instance.OnSpeakAudioGenerationStart -= speakAudioGenerationStartMethod;
            Speaker.Instance.OnSpeakAudioGenerationComplete -= speakAudioGenerationCompleteMethod;
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

         //usedGuids.Clear();

         SpeakerA(); //start with speaker A
         //SpeakerB(); //start with speaker B
      }

      public void SpeakerA()
      {
         //Don't speak the text immediately
         uidSpeakerA = Speaker.Instance.Speak(textA, SourceA, Speaker.Instance.VoiceForGender(Crosstales.RTVoice.Model.Enum.Gender.MALE, "en"), false, RateSpeakerA);
      }

      public void SpeakerB()
      {
         //Don't speak the text immediately
         uidSpeakerB = Speaker.Instance.Speak(textB, SourceB, Speaker.Instance.VoiceForGender(Crosstales.RTVoice.Model.Enum.Gender.FEMALE, "en"), false, RateSpeakerB);
      }

      public void Silence()
      {
         silent = true;
         Speaker.Instance.Silence();

         if (SourceA != null)
            SourceA.Stop();

         if (SourceB != null)
            SourceB.Stop();

         if (TextSpeakerA != null)
            TextSpeakerA.text = textA;

         if (TextSpeakerB != null)
            TextSpeakerB.text = textB;

         VisemeSpeakerB.text = PhonemeSpeakerB.text = VisemeSpeakerA.text = PhonemeSpeakerA.text = "-";
      }

      #endregion


      #region Private methods

      private void speakAudio()
      {
         Speaker.Instance.SpeakMarkedWordsWithUID(currentWrapper);
      }

      #endregion


      #region Callback methods

      private static void speakAudioGenerationStartMethod(Crosstales.RTVoice.Model.Wrapper wrapper)
      {
         if (RTVoice.Util.Config.DEBUG)
            Debug.Log("speakAudioGenerationStartMethod: " + wrapper);
      }

      private void speakAudioGenerationCompleteMethod(Crosstales.RTVoice.Model.Wrapper wrapper)
      {
         if (RTVoice.Util.Config.DEBUG)
            Debug.Log("speakAudioGenerationCompleteMethod: " + wrapper);

         if (wrapper.Uid.Equals(uidSpeakerA) || wrapper.Uid.Equals(uidSpeakerB))
         {
            currentWrapper = wrapper;

            Invoke(nameof(speakAudio), 0.1f); //needs a small delay
         }
      }

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
               Invoke(nameof(SpeakerB), 0.1f);
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
               Invoke(nameof(SpeakerA), 0.1f);
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
      }

      #endregion
   }
}
// © 2015-2023 crosstales LLC (https://www.crosstales.com)