#if UNITY_EDITOR && UNITY_STANDALONE_OSX || CT_DEVELOP
using UnityEditor;
using UnityEngine;
using UnityEditor.Callbacks;

namespace Crosstales.Common.Util
{
   /// <summary>Post processor for macOS.</summary>
   public static class CTPMacOSPostProcessor
   {
      private const string ID = "com.crosstales.procstart";

      private const bool REWRITE_BUNDLE = true; //change it to false if the bundle should not be changed

      [PostProcessBuildAttribute(1)]
      public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
      {
         if (BaseHelper.isMacOSPlatform)
         {
            //remove all meta-files
            string[] files = FileHelper.GetFiles(pathToBuiltProject, true, "meta");

            try
            {
               foreach (string file in files)
               {
                  //Debug.Log(file);
                  FileHelper.DeleteFile(file);
               }
            }
            catch (System.Exception ex)
            {
               Debug.Log($"Could not delete files: {ex}");
            }

            if (REWRITE_BUNDLE)
            {
               //rewrite Info.plist
               files = FileHelper.GetFiles(pathToBuiltProject, true, "plist");

               try
               {
                  foreach (string file in files)
                  {
                     string content = FileHelper.ReadAllText(file);

                     if (content.Contains(ID))
                     {
                        content = content.Replace(ID, $"{ID}.{System.DateTime.Now:yyyyMMddHHmmss}");
                        FileHelper.WriteAllText(file, content);
                     }
                  }
               }
               catch (System.Exception ex)
               {
                  Debug.Log($"Could not rewrite 'Info.plist' files: {ex}");
               }
               //UnityEditor.OSXStandalone.MacOSCodeSigning.CodeSignAppBundle("/path/to/bundle.bundle"); //TODO add for Unity > 2018?
            }
         }
      }
   }
}
#endif
// © 2021-2023 crosstales LLC (https://www.crosstales.com)