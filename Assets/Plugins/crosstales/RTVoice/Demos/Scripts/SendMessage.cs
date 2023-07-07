using UnityEngine;
using System.Collections;

namespace Crosstales.RTVoice.Demo
{
   /// <summary>Simple "SendMessage" example.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_demo_1_1_send_message.html")]
   public class SendMessage : MonoBehaviour
   {
      #region Variables

      [TextArea] public string TextA = "RT-Voice works great with PlayMaker, SALSA, Localized Dialogs/Cutscenes, Dialogue System for Unity and THE Dialogue Engine - that's awesome!";
      [TextArea] public string TextB = "Absolutely true! RT-Voice is fantastic.";
      public float DelayTextB = 12.2f;
      public bool PlayOnStart;

      private GameObject receiver;

      #endregion


      #region MonoBehaviour methods

      private void Start()
      {
         receiver = Speaker.Instance.gameObject;

         if (PlayOnStart)
            Play();
      }

      #endregion


      #region Public methods

      public void Play()
      {
         //Speak
         SpeakerA();

         StartCoroutine(SpeakerB());
      }

      public void SpeakerA()
      {
         //example with string-array
         //receiver.SendMessage("Speak", new string[]{TextA, "en", (Helper.isWindowsPlatform ? "Microsoft David Desktop" : "Alex")}); //example with string-array

         //example with string
         receiver.SendMessage("SpeakLive", TextA + ";" + "en" + ";" + (RTVoice.Util.Helper.isWindowsPlatform ? "Microsoft David Desktop" : "Alex")); //example with string-array
      }

      public IEnumerator SpeakerB()
      {
         yield return new WaitForSeconds(DelayTextB);

         //example with string-array
         receiver.SendMessage("SpeakLive", new[] { TextB, "en", RTVoice.Util.Helper.isWindowsPlatform ? "Microsoft Zira Desktop" : "Vicki" });

         //example with string
         //receiver.SendMessage("SpeakLive", TextB + ";" + "en" + ";" + (Helper.isWindowsPlatform ? "Microsoft Zira Desktop" : "Vicki")); //example with string-array
      }

      public void Silence()
      {
         StopAllCoroutines();
         receiver.SendMessage("SilenceLive");
      }

      #endregion
   }
}
// © 2015-2022 crosstales LLC (https://www.crosstales.com)