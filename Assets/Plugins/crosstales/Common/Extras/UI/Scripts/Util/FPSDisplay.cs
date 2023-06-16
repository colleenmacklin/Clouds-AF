using UnityEngine;
using UnityEngine.UI;

namespace Crosstales.UI.Util
{
   /// <summary>Simple FPS-Counter.</summary>
   [DisallowMultipleComponent]
   public class FPSDisplay : MonoBehaviour
   {
      #region Variables

      /// <summary>Text component to display the FPS.</summary>
      [Tooltip("Text component to display the FPS.")] public Text FPS;

      /// <summary>Update every set frame (default: 5).</summary>
      [Tooltip("Update every set frame (default: 5)."), Range(1, 300)] public int FrameUpdate = 5;

      [Tooltip("Key to activate the FPS counter (default: none).")] public KeyCode Key = KeyCode.None;

      private float deltaTime;
      private float elapsedTime;

      private float msec;
      private float fps;

      private const string wait = "<i>...calculating <b>FPS</b>...</i>";
      private const string red = "<color=#E57373><b>FPS: {0:0.}</b> ({1:0.0} ms)</color>";
      private const string orange = "<color=#FFB74D><b>FPS: {0:0.}</b> ({1:0.0} ms)</color>";
      private const string green = "<color=#81C784><b>FPS: {0:0.}</b> ({1:0.0} ms)</color>";

      #endregion


      #region MonoBehaviour methods

      private void Update()
      {
         if (Key == KeyCode.None || Input.GetKey(Key))
         {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            elapsedTime += Time.deltaTime;

            if (elapsedTime > 1f)
            {
               if (Time.frameCount % FrameUpdate == 0)
               {
                  FPS.enabled = true;

                  msec = deltaTime * 1000f;
                  fps = 1f / deltaTime;

                  if (fps < 15f)
                  {
                     FPS.text = string.Format(red, fps, msec);
                  }
                  else if (fps < 29f)
                  {
                     FPS.text = string.Format(orange, fps, msec);
                  }
                  else
                  {
                     FPS.text = string.Format(green, fps, msec);
                  }
               }
            }
            else
            {
               FPS.text = wait;
            }
         }
         else
         {
            //elapsedTime = 0;
            FPS.enabled = false;
         }
      }

      #endregion
   }
}
// © 2017-2022 crosstales LLC (https://www.crosstales.com)