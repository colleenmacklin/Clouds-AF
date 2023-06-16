﻿using UnityEngine;
using UnityEngine.UI;

namespace Crosstales.RTVoice.Demo
{
   /// <summary>Simple GUI for runtime dialogs with all available OS voices.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_demo_1_1_g_u_i_dialog.html")]
   public class GUIDialog : MonoBehaviour
   {
      #region Variables

      [Header("Dialog Script")] public Dialog DialogScript;

      [Header("UI Objects")] public Color32 SpeakerColor = new Color32(0, 255, 0, 192);

      public Image PanelPersonA;
      public Image PanelPersonB;
      public Text PersonA;
      public Text PersonB;

      private Color32 baseColorA;
      private Color32 baseColorB;

      #endregion


      #region MonoBehaviour methods

      private void Start()
      {
         if (PanelPersonA != null)
            baseColorA = PanelPersonA.color;

         if (PanelPersonB != null)
            baseColorB = PanelPersonB.color;

         //Speaker.isMaryMode = !Helper.hasBuiltInTTS;
      }

      private void Update()
      {
         if (!string.IsNullOrEmpty(DialogScript.CurrentDialogA))
         {
            PersonA.text += DialogScript.CurrentDialogA + System.Environment.NewLine + System.Environment.NewLine;

            DialogScript.CurrentDialogA = string.Empty;

            PanelPersonA.color = SpeakerColor;
            PanelPersonB.color = baseColorB;
         }

         if (!string.IsNullOrEmpty(DialogScript.CurrentDialogB))
         {
            PersonB.text += DialogScript.CurrentDialogB + System.Environment.NewLine + System.Environment.NewLine;

            DialogScript.CurrentDialogB = string.Empty;

            PanelPersonA.color = baseColorA;
            PanelPersonB.color = SpeakerColor;
         }
      }

      #endregion


      #region Public methods

      public void StartDialog()
      {
         Silence();
         if (DialogScript != null)
         {
            StartCoroutine(DialogScript.DialogSequence());
         }

         else
         {
            Debug.LogWarning("'DialogScript' is null - please assign it in the editor!", this);
         }
      }

      public void Silence()
      {
         StopAllCoroutines();
         if (DialogScript != null)
         {
            if (DialogScript.AudioPersonA != null)
               DialogScript.AudioPersonA.Stop();

            if (DialogScript.AudioPersonB != null)
               DialogScript.AudioPersonB.Stop();

            DialogScript.Running = false;
         }

         Speaker.Instance.Silence();
         if (PanelPersonA != null)
            PanelPersonA.color = baseColorA;
         if (PanelPersonB != null)
            PanelPersonB.color = baseColorB;
         if (PersonA != null)
            PersonA.text = string.Empty;
         if (PersonB != null)
            PersonB.text = string.Empty;
      }

      public void ChangeRateA(float value)
      {
         DialogScript.RateA = value;
      }

      public void ChangeRateB(float value)
      {
         DialogScript.RateB = value;
      }

      public void ChangePitchA(float value)
      {
         DialogScript.PitchA = value;
      }

      public void ChangePitchB(float value)
      {
         DialogScript.PitchB = value;
      }

      public void ChangeVolumeA(float value)
      {
         DialogScript.VolumeA = value;
      }

      public void ChangeVolumeB(float value)
      {
         DialogScript.VolumeB = value;
      }

      public void GenderAChanged(int index)
      {
         DialogScript.GenderA = (Crosstales.RTVoice.Model.Enum.Gender)index;
      }

      public void GenderBChanged(int index)
      {
         DialogScript.GenderB = (Crosstales.RTVoice.Model.Enum.Gender)index;
      }

      #endregion
   }
}
// © 2015-2022 crosstales LLC (https://www.crosstales.com)