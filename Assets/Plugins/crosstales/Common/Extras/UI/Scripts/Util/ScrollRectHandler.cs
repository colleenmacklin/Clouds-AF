using UnityEngine;
using UnityEngine.UI;

namespace Crosstales.UI.Util
{
   /// <summary>Changes the sensitivity of ScrollRects under various platforms.</summary>
   public class ScrollRectHandler : MonoBehaviour
   {
      #region Variables

      public ScrollRect Scroll;
      public float WindowsSensitivity = 35f;
      public float MacSensitivity = 25f;

      #endregion


      #region MonoBehaviour methods

      private void Start()
      {
         if (Common.Util.BaseHelper.isWindowsPlatform)
         {
            Scroll.scrollSensitivity = WindowsSensitivity;
         }
         else if (Common.Util.BaseHelper.isMacOSPlatform)
         {
            Scroll.scrollSensitivity = MacSensitivity;
         }
      }

      #endregion
   }
}
// © 2016-2022 crosstales LLC (https://www.crosstales.com)