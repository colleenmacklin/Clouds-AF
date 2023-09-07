using UnityEngine;

namespace Crosstales.RTVoice.Demo
{
   /// <summary>Simple test script for all UnityEvent/C# callbacks.</summary>
   [ExecuteInEditMode]
   [HelpURL("https://crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_demo_1_1_event_tester.html")]
   public class EventTester : MonoBehaviour
   {
      #region Variables

      public bool ShowUnityEvents = true;
      public bool ShowCSharpEvents = false;

      #endregion


      #region MonoBehaviour methods

      private void Start()
      {
         Speaker.Instance.OnVoicesReady += onVoicesReady;
         Speaker.Instance.OnSpeakStart += onSpeakStart;
         Speaker.Instance.OnSpeakComplete += onSpeakComplete;
         Speaker.Instance.OnProviderChange += onProviderChange;
         Speaker.Instance.OnErrorInfo += onErrorInfo;
      }

      private void OnDestroy()
      {
         Speaker.Instance.OnVoicesReady -= onVoicesReady;
         Speaker.Instance.OnSpeakStart -= onSpeakStart;
         Speaker.Instance.OnSpeakComplete -= onSpeakComplete;
         Speaker.Instance.OnProviderChange -= onProviderChange;
         Speaker.Instance.OnErrorInfo -= onErrorInfo;
      }

      #endregion


      #region Public methods

      public void OnReady()
      {
         if (ShowUnityEvents)
            Debug.Log("OnReady");
      }

      public void OnSpeakStarted(string uid)
      {
         if (ShowUnityEvents)
            Debug.Log("OnSpeakStarted: " + uid);
      }

      public void OnSpeakCompleted(string uid)
      {
         if (ShowUnityEvents)
            Debug.Log("OnSpeakCompleted: " + uid);
      }

      public void OnProviderChanged(string provider)
      {
         if (ShowUnityEvents)
            Debug.Log("OnProviderChanged: " + provider);
      }

      public void OnError(string uid, string info)
      {
         if (ShowUnityEvents)
            Debug.LogWarning("OnError: " + uid + " - " + info);
      }

      public void AudioFileGeneratorStarted()
      {
         Debug.Log("AudioFileGeneratorStarted");
      }

      public void AudioFileGeneratorCompleted()
      {
         Debug.Log("AudioFileGeneratorCompleted");
      }

      public void ParalanguageStarted()
      {
         Debug.Log("ParalanguageStarted");
      }

      public void ParalanguageCompleted()
      {
         Debug.Log("ParalanguageCompleted");
      }

      public void SpeechTextStarted()
      {
         Debug.Log("SpeechTextStarted");
      }

      public void SpeechTextCompleted()
      {
         Debug.Log("SpeechTextCompleted");
      }

      public void TextFileSpeakerStarted()
      {
         Debug.Log("TextFileSpeakerStarted");
      }

      public void TextFileSpeakerCompleted()
      {
         Debug.Log("TextFileSpeakerCompleted");
      }

      #endregion


      #region Callbacks

      private void onVoicesReady()
      {
         if (ShowCSharpEvents)
            Debug.Log("C# - OnVoicesReady");
      }

      private void onSpeakStart(Crosstales.RTVoice.Model.Wrapper wrapper)
      {
         if (ShowCSharpEvents)
            Debug.Log("C# - OnSpeakStart: " + wrapper);
      }

      private void onSpeakComplete(Crosstales.RTVoice.Model.Wrapper wrapper)
      {
         if (ShowCSharpEvents)
            Debug.Log("C# - onSpeakComplete: " + wrapper);
      }

      public void onProviderChange(string provider)
      {
         if (ShowCSharpEvents)
            Debug.Log("C# - OnProviderChange: " + provider);
      }

      private void onErrorInfo(Crosstales.RTVoice.Model.Wrapper wrapper, string info)
      {
         if (ShowCSharpEvents)
            Debug.LogWarning("C# - OnErrorInfo: " + wrapper + " - " + info);
      }

      #endregion
   }
}
// © 2021-2023 crosstales LLC (https://www.crosstales.com)