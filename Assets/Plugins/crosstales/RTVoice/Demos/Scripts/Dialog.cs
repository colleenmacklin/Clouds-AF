using UnityEngine;
using System.Collections;

namespace Crosstales.RTVoice.Demo
{
   /// <summary>Simple dialog system with TTS voices.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_demo_1_1_dialog.html")]
   public class Dialog : MonoBehaviour
   {
      #region Variables

      [Header("Configuration")] public string CultureA = "en";
      public string CultureB = "en";
      [Range(0f, 3f)] public float RateA = 1f;
      [Range(0f, 3f)] public float RateB = 1f;

      [Range(0f, 2f)] public float PitchA = 1f;
      [Range(0f, 2f)] public float PitchB = 1f;

      [Range(0f, 1f)] public float VolumeA = 1f;
      [Range(0f, 1f)] public float VolumeB = 1f;

      public Crosstales.RTVoice.Model.Enum.Gender GenderA = Crosstales.RTVoice.Model.Enum.Gender.UNKNOWN;
      public Crosstales.RTVoice.Model.Enum.Gender GenderB = Crosstales.RTVoice.Model.Enum.Gender.UNKNOWN;

      public AudioSource AudioPersonA;
      public AudioSource AudioPersonB;

      public Crosstales.RTVoice.Model.Enum.SpeakMode ModeA = Crosstales.RTVoice.Model.Enum.SpeakMode.Speak;
      public Crosstales.RTVoice.Model.Enum.SpeakMode ModeB = Crosstales.RTVoice.Model.Enum.SpeakMode.Speak;

      [Header("Dialogues")] public string[] DialogPersonA;
      public string[] DialogPersonB;
      public string CurrentDialogA = string.Empty;
      public string CurrentDialogB = string.Empty;

      public bool Running;

      private string uidSpeakerA;
      private string uidSpeakerB;

      private bool playingA;
      private bool playingB;

      #endregion


      #region MonoBehaviour methods

      private void Start()
      {
         Speaker.Instance.OnSpeakStart += speakStartMethod;
         Speaker.Instance.OnSpeakComplete += speakCompleteMethod;
      }

      private void OnDestroy()
      {
         if (Speaker.Instance != null)
         {
            Speaker.Instance.OnSpeakStart -= speakStartMethod;
            Speaker.Instance.OnSpeakComplete -= speakCompleteMethod;
         }
      }

      #endregion


      #region Public methods

      public IEnumerator DialogSequence()
      {
         if (!Running)
         {
            Running = true;

            playingA = false;
            playingB = false;

            int index = 0;

            while (Running && index < DialogPersonA?.Length || index < DialogPersonB?.Length)
            {
               yield return null;

               //Person A
               if (index < DialogPersonA?.Length)
                  CurrentDialogA = DialogPersonA[index];

               uidSpeakerA = ModeA == Crosstales.RTVoice.Model.Enum.SpeakMode.Speak ? Speaker.Instance.Speak(CurrentDialogA, AudioPersonA, Speaker.Instance.VoiceForGender(GenderA, CultureA), true, RateA, PitchA, VolumeA) : Speaker.Instance.SpeakNative(CurrentDialogA, Speaker.Instance.VoiceForGender(GenderA, CultureA), RateA, PitchA, VolumeA);

               //wait until ready
               do
               {
                  yield return null;
               } while (!playingA && Running);

               //wait until played
               do
               {
                  yield return null;
               } while (playingA && Running);

               CurrentDialogA = string.Empty;

               yield return null;

               if (Running)
               {
                  //ensure it's still running

                  // Person B
                  if (index < DialogPersonB?.Length)
                     CurrentDialogB = DialogPersonB[index];

                  uidSpeakerB = ModeB == Crosstales.RTVoice.Model.Enum.SpeakMode.Speak ? Speaker.Instance.Speak(CurrentDialogB, AudioPersonB, Speaker.Instance.VoiceForGender(GenderB, CultureB, 1), true, RateB, PitchB, VolumeB) : Speaker.Instance.SpeakNative(CurrentDialogB, Speaker.Instance.VoiceForGender(GenderB, CultureB, 1), RateB, PitchB, VolumeB);

                  //wait until ready
                  do
                  {
                     yield return null;
                  } while (!playingB && Running);

                  //wait until played
                  do
                  {
                     yield return null;
                  } while (playingB && Running);

                  CurrentDialogB = string.Empty;
               }

               index++;
            }

            Running = false;
         }
      }

      #endregion


      #region Callback methods

      private void speakStartMethod(Crosstales.RTVoice.Model.Wrapper wrapper)
      {
         if (wrapper.Uid.Equals(uidSpeakerA))
         {
            if (RTVoice.Util.Config.DEBUG)
               Debug.Log("speakStartMethod - Speaker A: " + wrapper, this);

            playingA = true;
         }
         else if (wrapper.Uid.Equals(uidSpeakerB))
         {
            if (RTVoice.Util.Config.DEBUG)
               Debug.Log("speakStartMethod - Speaker B: " + wrapper, this);

            playingB = true;
         }
      }

      private void speakCompleteMethod(Crosstales.RTVoice.Model.Wrapper wrapper)
      {
         if (wrapper.Uid.Equals(uidSpeakerA))
         {
            if (RTVoice.Util.Config.DEBUG)
               Debug.Log("speakCompleteMethod - Speaker A: " + wrapper, this);

            playingA = false;
         }
         else if (wrapper.Uid.Equals(uidSpeakerB))
         {
            if (RTVoice.Util.Config.DEBUG)
               Debug.Log("speakCompleteMethod - Speaker B: " + wrapper, this);

            playingB = false;
         }
      }

      #endregion
   }
}
// © 2015-2022 crosstales LLC (https://www.crosstales.com)