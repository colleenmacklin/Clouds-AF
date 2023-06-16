using UnityEngine;

namespace Crosstales.UI
{
   /// <summary>Static Button Manager.</summary>
   public class StaticManager : MonoBehaviour
   {
      #region Public methods

      ///<summary>Quit the application (stop playing inside the Editor).</summary>
      public void Quit()
      {
#if UNITY_EDITOR
         UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
      }

      ///<summary>Open the crosstales homepage.</summary>
      public void OpenCrosstales()
      {
         Crosstales.Common.Util.NetworkHelper.OpenURL(Crosstales.Common.Util.BaseConstants.ASSET_AUTHOR_URL);
      }

      ///<summary>Open the Unity AssetStore homepage.</summary>
      public void OpenAssetstore()
      {
         Crosstales.Common.Util.NetworkHelper.OpenURL(Crosstales.Common.Util.BaseConstants.ASSET_CT_URL);
      }

      #endregion
   }
}
// © 2017-2022 crosstales LLC (https://www.crosstales.com)