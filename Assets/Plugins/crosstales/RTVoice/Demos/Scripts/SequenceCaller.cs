﻿using UnityEngine;

namespace Crosstales.RTVoice.Demo
{
   /// <summary>Simple Sequence caller example.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_demo_1_1_sequence_caller.html")]
   public class SequenceCaller : MonoBehaviour
   {
      #region Variables

      public GameObject receiver;
      public int NumberOfSequences;
      public float SequenceDelay = 1f;

      #endregion


      #region MonoBehaviour methods

      private void Start()
      {
         for (int ii = 0; ii < NumberOfSequences; ii++)
         {
            Invoke(nameof(playNextSequence), ii * SequenceDelay);
         }
      }

      #endregion


      #region Public methods

      private void playNextSequence()
      {
         receiver.SendMessage("PlayNextSequence");
      }

      #endregion
   }
}
// © 2016-2022 crosstales LLC (https://www.crosstales.com)