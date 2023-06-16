﻿using UnityEngine;
using UnityEngine.UI;

namespace Crosstales.UI.Audio
{
   /// <summary>Controller for AudioSources.</summary>
   public class AudioSourceController : MonoBehaviour
   {
      #region Variables

      /// <summary>Searches for all AudioSource in the whole scene (default: true).</summary>
      [Header("Audio Sources")] [Tooltip("Searches for all AudioSource in the whole scene (default: true).")]
      public bool FindAllAudioSourcesOnStart = true;

      /// <summary>Active controlled AudioSources.</summary>
      [Tooltip("Active controlled AudioSources.")] public AudioSource[] AudioSources;


      /// <summary>Resets all active AudioSources (default: true).</summary>
      [Header("Settings")] [Tooltip("Resets all active AudioSources (default: true).")] public bool ResetAudioSourcesOnStart = true;

      /// <summary>Mute on/off (default: false).</summary>
      [Tooltip("Mute on/off (default: false).")] public bool Mute;

      /// <summary>Loop on/off (default: false).</summary>
      [Tooltip("Loop on/off (default: false).")] public bool Loop;

      /// <summary>Volume of the audio (default: 1)</summary>
      [Tooltip("Volume of the audio (default: 1)")] public float Volume = 1f;

      /// <summary>Pitch of the audio (default: 1).</summary>
      [Tooltip("Pitch of the audio (default: 1).")] public float Pitch = 1f;

      /// <summary>Stereo pan of the audio (default: 0).</summary>
      [Tooltip("Stereo pan of the audio (default: 0).")] public float StereoPan;

      [Header("UI Objects")] public Text VolumeText;
      public Text PitchText;
      public Text StereoPanText;

      private bool initialized;

      #endregion


      #region MonoBehaviour methods

      private void Update()
      {
         if (!initialized && Time.frameCount % 30 == 0)
         {
            initialized = true;

            if (FindAllAudioSourcesOnStart)
               FindAllAudioSources();

            if (ResetAudioSourcesOnStart)
               ResetAllAudioSources();
         }
      }

      #endregion


      #region Public methods

      /// <summary>Finds all audio sources in the scene.</summary>
      public void FindAllAudioSources()
      {
         AudioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
      }

      /// <summary>Resets all audio sources.</summary>
      public void ResetAllAudioSources()
      {
         MuteEnabled(Mute);
         LoopEnabled(Loop);
         VolumeChanged(Volume);
         PitchChanged(Pitch);
         StereoPanChanged(StereoPan);
      }

      public void MuteEnabled(bool isEnabled)
      {
         foreach (AudioSource source in AudioSources)
         {
            source.mute = isEnabled;
         }
      }

      public void LoopEnabled(bool isEnabled)
      {
         foreach (AudioSource source in AudioSources)
         {
            source.mute = isEnabled;
         }
      }

      public void VolumeChanged(float value)
      {
         foreach (AudioSource source in AudioSources)
         {
            source.volume = value;
         }

         if (VolumeText != null)
            VolumeText.text = value.ToString(Crosstales.Common.Util.BaseConstants.FORMAT_TWO_DECIMAL_PLACES);
      }

      public void PitchChanged(float value)
      {
         foreach (AudioSource source in AudioSources)
         {
            source.pitch = value;
         }

         if (PitchText != null)
            PitchText.text = value.ToString(Crosstales.Common.Util.BaseConstants.FORMAT_TWO_DECIMAL_PLACES);
      }

      public void StereoPanChanged(float value)
      {
         foreach (AudioSource source in AudioSources)
         {
            source.panStereo = value;
         }

         if (StereoPanText != null)
            StereoPanText.text = value.ToString(Crosstales.Common.Util.BaseConstants.FORMAT_TWO_DECIMAL_PLACES);
      }

      #endregion
   }
}
// © 2016-2022 crosstales LLC (https://www.crosstales.com)