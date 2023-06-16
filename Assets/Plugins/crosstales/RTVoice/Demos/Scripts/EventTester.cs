using UnityEngine;

namespace Crosstales.RTVoice.Demo
{
   /// <summary>Simple test script for all UnityEvent-callbacks.</summary>
   [ExecuteInEditMode]
   [HelpURL("https://crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_demo_1_1_event_tester.html")]
   public class EventTester : MonoBehaviour
   {
      public void OnReady()
      {
         Debug.Log("OnReady");
      }

      public void OnSpeakStarted(string uid)
      {
         Debug.Log("OnSpeakStarted: " + uid);
      }

      public void OnSpeakCompleted(string uid)
      {
         Debug.Log("OnSpeakCompleted: " + uid);
      }

      public void OnProviderChanged(string provider)
      {
         Debug.Log("OnProviderChanged: " + provider);
      }


      public void OnError(string uid, string info)
      {
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
   }
}
// © 2021-2022 crosstales LLC (https://www.crosstales.com)