#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Crosstales.Common.EditorTask
{
   /// <summary>Search for the "Runtime File Browser" and add or remove the compile define "CT_RTFB".</summary>
   public class RTFBCheck : AssetPostprocessor
   {
      private static readonly string DEFINE = "CT_RTFB";
      private static readonly string IDENTIFIER = "SimpleFileBrowser.aar";

      public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
      {
#if !CT_RTFB
         if (importedAssets.Any(str => str.Contains("RTFBCheck.cs")))
         {
            //Debug.Log("Search for RTFB!");
            string[] files = Crosstales.Common.Util.FileHelper.GetFilesForName(Crosstales.Common.Util.BaseConstants.APPLICATION_PATH, true, IDENTIFIER);

            if (files?.Length > 0)
            {
               //Debug.Log("RTFB found!");
               BaseCompileDefines.AddSymbolsToAllTargets(DEFINE);
            }
         }
         else
         {
            if (importedAssets.Any(str => str.Contains(IDENTIFIER)))
            {
               //Debug.Log("RTFB installed!");
               BaseCompileDefines.AddSymbolsToAllTargets(DEFINE);
            }
         }
#else
         if (deletedAssets.Any(str => str.Contains(IDENTIFIER)))
         {
            //Debug.Log("RTFB uninstalled!");
            BaseCompileDefines.RemoveSymbolsFromAllTargets(DEFINE);
         }
#endif
      }
   }
}
#endif
// © 2022-2023 crosstales LLC (https://www.crosstales.com)