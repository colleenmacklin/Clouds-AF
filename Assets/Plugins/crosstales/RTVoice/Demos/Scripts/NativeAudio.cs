using UnityEngine;

namespace Crosstales.RTVoice.Demo
{
   /// <summary>Simple example with native audio for exact timing.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_demo_1_1_native_audio.html")]
   public class NativeAudio : MonoBehaviour
   {
      #region Variables

      public string SpeechText = "This is an example with native audio for exact timing (e.g. animations).";
      public bool PlayOnStart;
      public float Delay = 1f;

      #endregion


      #region MonoBehaviour methods

      private void Start()
      {
         Speaker.Instance.OnSpeakStart += play;
         Speaker.Instance.OnSpeakComplete += stop;

         if (PlayOnStart)
            Invoke(nameof(StartTTS), Delay); //Invoke the TTS-system after x seconds
      }

      private void OnDestroy()
      {
         if (Speaker.Instance != null)
         {
            Speaker.Instance.OnSpeakStart -= play;
            Speaker.Instance.OnSpeakComplete -= stop;
         }
      }

      #endregion


      #region Public methods

      public void StartTTS()
      {
         Speaker.Instance.SpeakNative(SpeechText, Speaker.Instance.VoiceForCulture("en", 1));
      }

      public void Silence()
      {
         Speaker.Instance.Silence();
      }

      #endregion


      #region Callback methods

      private void play(Crosstales.RTVoice.Model.Wrapper wrapper)
      {
         Debug.Log("Play your animations to the event: " + wrapper, this);

         //Here belongs your stuff, like animations
      }

      private void stop(Crosstales.RTVoice.Model.Wrapper wrapper)
      {
         Debug.Log("Stop your animations from the event: " + wrapper, this);

         //Here belongs your stuff, like animations
      }

      #endregion
   }
}
// © 2015-2023 crosstales LLC (https://www.crosstales.com)