using UnityEngine;

namespace Crosstales.UI
{
   /// <summary>Crosstales social media links.</summary>
   public class Social : MonoBehaviour
   {
      #region Public methods

      public void Facebook()
      {
         Crosstales.Common.Util.NetworkHelper.OpenURL(Crosstales.Common.Util.BaseConstants.ASSET_SOCIAL_FACEBOOK);
      }

      public void Twitter()
      {
         Crosstales.Common.Util.NetworkHelper.OpenURL(Crosstales.Common.Util.BaseConstants.ASSET_SOCIAL_TWITTER);
      }

      public void LinkedIn()
      {
         Crosstales.Common.Util.NetworkHelper.OpenURL(Crosstales.Common.Util.BaseConstants.ASSET_SOCIAL_LINKEDIN);
      }

      public void Youtube()
      {
         Crosstales.Common.Util.NetworkHelper.OpenURL(Crosstales.Common.Util.BaseConstants.ASSET_SOCIAL_YOUTUBE);
      }

      public void Discord()
      {
         Crosstales.Common.Util.NetworkHelper.OpenURL(Crosstales.Common.Util.BaseConstants.ASSET_SOCIAL_DISCORD);
      }

      #endregion
   }
}
// © 2017-2022 crosstales LLC (https://www.crosstales.com)