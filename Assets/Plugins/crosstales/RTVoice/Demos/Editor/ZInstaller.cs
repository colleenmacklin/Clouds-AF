#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Crosstales.RTVoice.Demo
{
   /// <summary>Installs the 'UI'-package from Common.</summary>
   [InitializeOnLoad]
   public static class ZInstaller
   {
      #region Constructor

      static ZInstaller()
      {
#if !CT_UI && !CT_DEVELOP
         string pathInstaller = Application.dataPath + "/Plugins/crosstales/Common/Extras/";

         try
         {
            string package = pathInstaller + "UI.unitypackage";

            if (System.IO.File.Exists(package))
            {
               AssetDatabase.ImportPackage(package, false);

               Crosstales.Common.EditorTask.BaseCompileDefines.AddSymbolsToAllTargets("CT_UI");
            }
            else
            {
               Debug.LogWarning("Package not found: " + package);
            }
         }
         catch (System.Exception ex)
         {
            Debug.LogError("Could not import the 'UI'-package: " + ex);
         }
#endif
      }

      #endregion
   }
}
#endif
// © 2020-2022 crosstales LLC (https://www.crosstales.com)