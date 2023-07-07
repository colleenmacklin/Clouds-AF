#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Crosstales.Common.EditorTask
{
   /// <summary>Base-class for moving all resources to 'Editor Default Resources'.</summary>
   public abstract class BaseSetupResources
   {
      protected static void setupResources(string source, string sourceFolder, string target, string targetFolder, string metafile)
      {
         bool exists = false;

         try
         {
            if (Crosstales.Common.Util.FileHelper.ExistsDirectory(sourceFolder))
            {
               exists = true;

               if (!Crosstales.Common.Util.FileHelper.ExistsDirectory(targetFolder))
                  Crosstales.Common.Util.FileHelper.CreateDirectory(targetFolder);

               System.IO.DirectoryInfo dirSource = new System.IO.DirectoryInfo(sourceFolder);

               foreach (System.IO.FileInfo file in dirSource.GetFiles("*"))
               {
                  if (Crosstales.Common.Util.FileHelper.ExistsFile(targetFolder + file.Name))
                  {
                     if (Crosstales.Common.Util.BaseConstants.DEV_DEBUG)
                        Debug.Log($"File exists: {file}");
                  }
                  else
                  {
                     AssetDatabase.MoveAsset(source + file.Name, target + file.Name);

                     if (Crosstales.Common.Util.BaseConstants.DEV_DEBUG)
                        Debug.Log($"File moved: {file}");
                  }
               }

               //dirSource.Delete(true);
               dirSource.Delete();

               if (Crosstales.Common.Util.FileHelper.ExistsFile(metafile))
                  Crosstales.Common.Util.FileHelper.DeleteFile(metafile);
            }
         }
         catch (System.Exception ex)
         {
            if (Crosstales.Common.Util.BaseConstants.DEV_DEBUG)
               Debug.LogError($"Could not move all files: {ex}");
         }
         finally
         {
            if (exists)
               Crosstales.Common.EditorUtil.BaseEditorHelper.RefreshAssetDatabase();
         }
      }
   }
}
#endif
// © 2018-2023 crosstales LLC (https://www.crosstales.com)