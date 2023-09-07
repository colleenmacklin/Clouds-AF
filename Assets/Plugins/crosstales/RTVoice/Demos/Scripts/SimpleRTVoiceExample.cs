using UnityEngine;
using Crosstales.RTVoice;
using Crosstales.RTVoice.Model;

/// <summary>
/// Simple example to demonstrate the basic usage of RT-Voice.
/// </summary>
public class SimpleRTVoiceExample : MonoBehaviour
{
   public string Text = "Hello world, I am RT-Voice!";
   public string Culture = "en";
   public bool SpeakWhenReady;
   public AudioSource Audio;
   public bool UseNative;

   private string uid; //Unique id of the speech

   private void OnEnable()
   {
      // Subscribe event listeners
      Speaker.Instance.OnVoicesReady += voicesReady;
      Speaker.Instance.OnSpeakStart += speakStart;
      Speaker.Instance.OnSpeakComplete += speakComplete;
   }

   private void OnDisable()
   {
      if (Speaker.Instance != null)
      {
         // Unsubscribe event listeners
         Speaker.Instance.OnVoicesReady -= voicesReady;
         Speaker.Instance.OnSpeakStart -= speakStart;
         Speaker.Instance.OnSpeakComplete -= speakComplete;
      }
   }

   public void Speak()
   {
      if (UseNative)
      {
         uid = Speaker.Instance.SpeakNative(Text, Speaker.Instance.VoiceForCulture(Culture)); //Speak (native TTS) with the first voice matching the given culture
      }
      else
      {
         uid = Speaker.Instance.Speak(Text, Audio, Speaker.Instance.VoiceForCulture(Culture)); //Speak (audio file) with the first voice matching the given culture
      }
   }

   private void voicesReady()
   {
      Debug.Log($"RT-Voice: {Speaker.Instance.Voices.Count} voices are ready to use!");

      if (SpeakWhenReady) //Speak after the voices are ready
         Speak();
   }

   private void speakStart(Wrapper wrapper)
   {
      if (wrapper.Uid == uid) //Only write the log message if it's "our" speech
         Debug.Log($"RT-Voice: speak started: {wrapper}");
   }

   private void speakComplete(Wrapper wrapper)
   {
      if (wrapper.Uid == uid) //Only write the log message if it's "our" speech
         Debug.Log($"RT-Voice: speak completed: {wrapper}");
   }
}
// © 2022-2023 crosstales LLC (https://www.crosstales.com)