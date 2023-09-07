using UnityEngine;

namespace Crosstales.RTVoice.Demo.Util
{
   /// <summary>Enables or disable game objects for a given platform.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_demo_1_1_util_1_1_platform_controller.html")]
   public class PlatformController : Crosstales.Common.Util.PlatformController
   {
      #region MonoBehaviour methods

      private void Start()
      {
         Speaker.Instance.OnProviderChange += onProviderChange;

         onProviderChange(string.Empty);
      }

      private void OnDestroy()
      {
         if (Speaker.Instance != null)
            Speaker.Instance.OnProviderChange -= onProviderChange;
      }

      #endregion


      #region Private methods

      private void onProviderChange(string provider)
      {
         selectPlatform();

         //Debug.Log (currentPlatform, this);
      }

      #endregion
   }
}
// © 2016-2023 crosstales LLC (https://www.crosstales.com)