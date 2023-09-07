#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Crosstales.RTVoice.Demo
{
   /// <summary>Installs the packages from Common.</summary>
   [InitializeOnLoad]
   public abstract class ZInstaller : Crosstales.Common.EditorTask.BaseInstaller
   {
      #region Constructor

      static ZInstaller()
      {
         string path = $"{Application.dataPath}{Crosstales.RTVoice.EditorUtil.EditorConfig.ASSET_PATH}";

#if !CT_UI && !CT_DEVELOP
         InstallUI(path);
#endif

#if !CT_RTV_DEMO && !CT_DEVELOP
         Crosstales.Common.EditorTask.BaseCompileDefines.AddSymbolsToAllTargets("CT_RTV_DEMO");
#endif
      }

      #endregion
   }
}
#endif
// © 2020-2023 crosstales LLC (https://www.crosstales.com)