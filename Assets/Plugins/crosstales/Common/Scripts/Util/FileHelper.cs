using System.Linq;
using UnityEngine;

namespace Crosstales.Common.Util
{
   /// <summary>Various helper functions for the file system.</summary>
#if UNITY_EDITOR
   [UnityEditor.InitializeOnLoad]
#endif
   public static class FileHelper
   {
      #region Variables

      private static string _applicationDataPath;
      private static string _applicationTempPath;
      private static string _applicationPersistentPath;
      private static char[] _invalidFilenameChars;
      private static char[] _invalidPathChars;

#if CT_RTFB && UNITY_ANDROID
      private static readonly System.Collections.Generic.List<string> _fileList = new System.Collections.Generic.List<string>();
      private static readonly System.Collections.Generic.List<string> _dirList = new System.Collections.Generic.List<string>();
#endif

      #endregion


      #region Properties

      /// <summary>Returns the path to the the "Streaming Assets".</summary>
      /// <returns>The path to the the "Streaming Assets".</returns>
      public static string StreamingAssetsPath
      {
         get
         {
            if (BaseHelper.isAndroidPlatform && !BaseHelper.isEditor)
               return $"jar:file://{ApplicationDataPath}!/assets/";

            if (BaseHelper.isIOSBasedPlatform && !BaseHelper.isEditor)
               return $"{ApplicationDataPath}/Raw/";

            return $"{ApplicationDataPath}/StreamingAssets/";
         }
      }

      /// <summary>Returns the Unity application data path.</summary>
      /// <returns>Unity application data path</returns>
      public static string ApplicationDataPath => _applicationDataPath;

      /// <summary>Returns the Unity application temporary path.</summary>
      /// <returns>Unity application temporary path</returns>
      public static string ApplicationTempPath => _applicationTempPath;

      /// <summary>Returns the Unity application persistent path.</summary>
      /// <returns>Unity application persistent path</returns>
      public static string ApplicationPersistentPath => _applicationPersistentPath;

      /// <summary>Returns a temporary file.</summary>
      /// <returns>Temporary file</returns>
      public static string TempFile => System.IO.Path.GetTempFileName();

      /// <summary>Returns the temporary directory path from the device.</summary>
      /// <returns>Temporary directory path of the device</returns>
      public static string TempPath => System.IO.Path.GetTempPath();

      #endregion


      #region Static block

#if UNITY_EDITOR
      static FileHelper()
      {
         initialize();
      }
#endif
      [RuntimeInitializeOnLoadMethod]
      private static void initialize()
      {
#if UNITY_ANDROID && !CT_RTFB && !UNITY_EDITOR
         Debug.LogWarning($"'Runtime File Browser' not found! We recommend to install it from the Unity AssetStore: {BaseConstants.ASSET_3P_RTFB}");
#endif
         //Debug.Log("initialize");
         _applicationDataPath = ValidatePath(Application.dataPath);
         _applicationTempPath = ValidatePath(Application.temporaryCachePath);
         _applicationPersistentPath = ValidatePath(Application.persistentDataPath);

         System.Collections.Generic.HashSet<char> invalidFilenameChars = new System.Collections.Generic.HashSet<char>(System.IO.Path.GetInvalidFileNameChars())
         {
            System.IO.Path.DirectorySeparatorChar,
            System.IO.Path.AltDirectorySeparatorChar,
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN || UNITY_WSA
            '<',
            '>',
            ':',
            '"',
            '/',
            '\\',
            '|',
            '?',
            '*',
#endif
         };

         _invalidFilenameChars = invalidFilenameChars.ToArray();

         System.Collections.Generic.HashSet<char> invalidPathChars = new System.Collections.Generic.HashSet<char>(System.IO.Path.GetInvalidPathChars())
         {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN || UNITY_WSA
            '<',
            '>',
            '"',
            '|',
            '?',
            '*',
#endif
         };

         _invalidPathChars = invalidPathChars.ToArray();

/*
         if (!isEditorMode)
         {
            GameObject go = new GameObject("_HelperCT");
            go.AddComponent<HelperCT>();
            GameObject.DontDestroyOnLoad(go);
         }
*/
      }

      #endregion


      #region Public methods

      /// <summary>Checks if the given path is from a Unix-device</summary>
      /// <param name="path">Path to check</param>
      /// <returns>True if the given path is from a Unix-device</returns>
      public static bool isUnixPath(string path) //NUnit
      {
         return !string.IsNullOrEmpty(path) && path.StartsWith("/");
      }

      /// <summary>Checks if the given path is from a Windows-device</summary>
      /// <param name="path">Path to check</param>
      /// <returns>True if the given path is from a Windows-device</returns>
      public static bool isWindowsPath(string path) //NUnit
      {
         return !string.IsNullOrEmpty(path) && BaseConstants.REGEX_DRIVE_LETTERS.IsMatch(path);
      }

      /// <summary>Checks if the given path is UNC</summary>
      /// <param name="path">Path to check</param>
      /// <returns>True if the given path is UNC</returns>
      public static bool isUNCPath(string path) //NUnit
      {
         return !string.IsNullOrEmpty(path) && path.StartsWith(@"\\");
      }

      /// <summary>Checks if the given path is an URL</summary>
      /// <param name="path">Path to check</param>
      /// <returns>True if the given path is an URL</returns>
      public static bool isURL(string path) //NUnit
      {
         return NetworkHelper.isURL(path);
      }

      /// <summary>Validates a given path and add missing slash.</summary>
      /// <param name="path">Path to validate</param>
      /// <param name="addEndDelimiter">Add delimiter at the end of the path (optional, default: true)</param>
      /// <param name="preserveFile">Preserves a given file in the path (optional, default: true)</param>
      /// <param name="removeInvalidChars">Removes invalid characters in the path name (optional default: true)</param>
      /// <returns>Valid path</returns>
      public static string ValidatePath(string path, bool addEndDelimiter = true, bool preserveFile = true, bool removeInvalidChars = true) //NUnit
      {
         if (string.IsNullOrEmpty(path))
            return path;

         if (isURL(path))
         {
            if (addEndDelimiter && !path.EndsWith(BaseConstants.PATH_DELIMITER_UNIX))
               path += BaseConstants.PATH_DELIMITER_UNIX;

            return path;
         }

         string pathTemp = !preserveFile && ExistsFile(path.Trim()) ? GetDirectoryName(path.Trim()) : path.Trim();

         string result = pathTemp;

         if (isWindowsPath(pathTemp) || isUNCPath(pathTemp))
         {
            //if (!isUNCPath(pathTemp))
            result = pathTemp.Replace('/', '\\');

            if (addEndDelimiter && !result.EndsWith(BaseConstants.PATH_DELIMITER_WINDOWS))
               result += BaseConstants.PATH_DELIMITER_WINDOWS;
         }
         else
         {
            result = pathTemp.Replace('\\', '/');

            if (addEndDelimiter && !result.EndsWith(BaseConstants.PATH_DELIMITER_UNIX))
               result += BaseConstants.PATH_DELIMITER_UNIX;
         }

         if (removeInvalidChars)
            return string.Join(string.Empty, result.Split(_invalidPathChars));

         return result;
      }

      /// <summary>Validates a given file.</summary>
      /// <param name="path">File to validate</param>
      /// <param name="removeInvalidChars">Removes invalid characters in the file name (optional, default: true)</param>
      /// <returns>Valid file path</returns>
      public static string ValidateFile(string path, bool removeInvalidChars = true) //NUnit
      {
         if (string.IsNullOrEmpty(path))
            return path;

         if (isURL(path))
            return path;

         bool isWin = isWindowsPath(path);
         bool isUNC = isUNCPath(path);

         string result = ValidatePath(path, false, removeInvalidChars);

         if (result.EndsWith(BaseConstants.PATH_DELIMITER_WINDOWS) ||
             result.EndsWith(BaseConstants.PATH_DELIMITER_UNIX))
            result = result.Substring(0, result.Length - 1);

         string fileName;
         if (isWin || isUNC)
         {
            fileName = result.Substring(result.CTLastIndexOf(BaseConstants.PATH_DELIMITER_WINDOWS) + 1);
         }
         else
         {
            fileName = result.Substring(result.CTLastIndexOf(BaseConstants.PATH_DELIMITER_UNIX) + 1);
         }

         string newName = string.Empty;

         if (removeInvalidChars)
         {
            newName = string.Join(string.Empty, fileName.Split(_invalidFilenameChars)); //.Replace(BaseConstants.PATH_DELIMITER_WINDOWS, string.Empty).Replace(BaseConstants.PATH_DELIMITER_UNIX, string.Empty);

            if ((isWin || isUNC) && newName.EndsWith(".")) //file under Windows/UNC can not end with .
               newName = newName.Substring(0, fileName.Length - 1);
         }

         return result.Substring(0, result.Length - fileName.Length) + newName; //this is correct!
      }

      /// <summary>
      /// Checks a given path for invalid characters
      /// </summary>
      /// <param name="path">Path to check for invalid characters</param>
      /// <param name="ignoreNullOrEmpty">If set to true, return false for null or empty paths (optional, default: true)</param>
      /// <returns>Returns true if the path contains invalid chars, otherwise it's false.</returns>
      public static bool HasPathInvalidChars(string path, bool ignoreNullOrEmpty = true) //NUnit
      {
         if (string.IsNullOrEmpty(path))
            return !ignoreNullOrEmpty;

         return path.IndexOfAny(_invalidPathChars) >= 0;
      }

      /// <summary>
      /// Checks a given file for invalid characters
      /// </summary>
      /// <param name="file">File to check for invalid characters</param>
      /// <param name="ignoreNullOrEmpty">If set to true, return false for null or empty paths (optional, default: true)</param>
      /// <returns>Returns true if the file contains invalid chars, otherwise it's false.</returns>
      public static bool HasFileInvalidChars(string file, bool ignoreNullOrEmpty = true) //NUnit
      {
         if (string.IsNullOrEmpty(file))
            return !ignoreNullOrEmpty;

#if UNITY_STANDALONE_WIN
         return GetFileName(file, false).IndexOfAny(_invalidFilenameChars) >= 0 || file.EndsWith(".");
#else
         return GetFileName(file, false).IndexOfAny(_invalidFilenameChars) >= 0;
#endif
      }

      /// <summary>
      /// Find files inside a path.
      /// </summary>
      /// <param name="path">Path to find the files</param>
      /// <param name="isRecursive">Recursive search (optional, default: false)</param>
      /// <param name="filenames">Array of file names for the file search, e.g. "Image.png" (optional)</param>
      /// <returns>Returns array of the found files inside the path (alphabetically ordered). Zero length array when an error occured.</returns>
      public static string[] GetFilesForName(string path, bool isRecursive = false, params string[] filenames) //NUnit
      {
         if (BaseHelper.isWebPlatform && !BaseHelper.isEditor)
         {
            Debug.LogWarning("'GetFilesForName' is not supported for the current platform!");
         }
         else
         {
            if (!string.IsNullOrEmpty(path))
            {
               if (BaseHelper.isWSABasedPlatform && !BaseHelper.isEditor)
               {
#if CT_FB
#if (UNITY_WSA || UNITY_XBOXONE) && !UNITY_EDITOR && ENABLE_WINMD_SUPPORT
                  Crosstales.FB.FileBrowserWSAImpl fbWsa = new Crosstales.FB.FileBrowserWSAImpl();
                  fbWsa.isBusy = true;
                  UnityEngine.WSA.Application.InvokeOnUIThread(() => { fbWsa.GetFilesForName(path, isRecursive, filenames); }, false);

                  do
                  {
                    //wait
                  } while (fbWsa.isBusy);

                  return fbWsa.Selection.ToArray();
#endif
#else
                  Debug.LogWarning($"'GetFilesForName' under UWP (WSA) is supported in combination with 'File Browser PRO'. For more, please see: {BaseConstants.ASSET_FB}");
#endif
               }
               else
               {
                  try
                  {
                     string _path = ValidatePath(path);
#if CT_RTFB && UNITY_ANDROID
                     _fileList.Clear();
                     getFilesRTFB(_path, isRecursive, filenames);
                     return _fileList.ToArray();
#else
                     if (filenames == null || filenames.Length == 0 || filenames.Any(extension => extension.Equals("*") || extension.Equals("*.*")))
                     {
                        return System.IO.Directory.EnumerateFiles(_path, "*", isRecursive
                           ? System.IO.SearchOption.AllDirectories
                           : System.IO.SearchOption.TopDirectoryOnly).ToArray();
                     }

                     System.Collections.Generic.List<string> files = new System.Collections.Generic.List<string>();

                     foreach (string filename in filenames)
                     {
                        files.AddRange(System.IO.Directory.EnumerateFiles(_path, filename.StartsWith("*.") ? filename : $"*{filename}*", isRecursive
                           ? System.IO.SearchOption.AllDirectories
                           : System.IO.SearchOption.TopDirectoryOnly));
                     }

                     return files.OrderBy(q => q).ToArray();
#endif
                  }
                  catch (System.Exception ex)
                  {
                     Debug.LogWarning($"Could not scan the path '{path}' for files: {ex}");
                  }
               }
            }
         }

         return System.Array.Empty<string>();
      }

      /// <summary>
      /// Find files inside a path.
      /// </summary>
      /// <param name="path">Path to find the files</param>
      /// <param name="isRecursive">Recursive search (optional, default: false)</param>
      /// <param name="extensions">Extensions for the file search, e.g. "png" (optional)</param>
      /// <returns>Returns array of the found files inside the path (alphabetically ordered). Zero length array when an error occured.</returns>
      public static string[] GetFiles(string path, bool isRecursive = false, params string[] extensions) //NUnit
      {
         if (BaseHelper.isWSABasedPlatform && !BaseHelper.isEditor)
         {
#if CT_FB
#if (UNITY_WSA || UNITY_XBOXONE) && !UNITY_EDITOR && ENABLE_WINMD_SUPPORT
            Crosstales.FB.FileBrowserWSAImpl fbWsa = new Crosstales.FB.FileBrowserWSAImpl();
            fbWsa.isBusy = true;
            UnityEngine.WSA.Application.InvokeOnUIThread(() => { fbWsa.GetFiles(path, isRecursive, extensions); }, false);

            do
            {
              //wait
            } while (fbWsa.isBusy);

            return fbWsa.Selection.ToArray();
#endif
#else
            Debug.LogWarning($"'GetFiles' under UWP (WSA) is supported in combination with 'File Browser PRO'. For more, please see: {BaseConstants.ASSET_FB}");
            return System.Array.Empty<string>();
#endif
         }

         if (extensions?.Length > 0)
         {
            string[] wildcardExt = new string[extensions.Length];

            for (int ii = 0; ii < extensions.Length; ii++)
            {
               wildcardExt[ii] = $"*.{extensions[ii]}";
            }

            return GetFilesForName(path, isRecursive, wildcardExt);
         }

         return GetFilesForName(path, isRecursive, extensions);
      }

      /// <summary>
      /// Find directories inside.
      /// </summary>
      /// <param name="path">Path to find the directories</param>
      /// <param name="isRecursive">Recursive search (optional, default: false)</param>
      /// <returns>Returns array of the found directories inside the path. Zero length array when an error occured.</returns>
      public static string[] GetDirectories(string path, bool isRecursive = false) //NUnit
      {
         if (BaseHelper.isWebPlatform && !BaseHelper.isEditor)
         {
            Debug.LogWarning("'GetDirectories' is not supported for the current platform!");
         }
         else if (BaseHelper.isWSABasedPlatform && !BaseHelper.isEditor)
         {
#if CT_FB
#if (UNITY_WSA || UNITY_XBOXONE) && !UNITY_EDITOR && ENABLE_WINMD_SUPPORT
            Crosstales.FB.FileBrowserWSAImpl fbWsa = new Crosstales.FB.FileBrowserWSAImpl();
            fbWsa.isBusy = true;
            UnityEngine.WSA.Application.InvokeOnUIThread(() => { fbWsa.GetDirectories(path, isRecursive); }, false);

            do
            {
              //wait
            } while (fbWsa.isBusy);

            return fbWsa.Selection.ToArray();
#endif
#else
            Debug.LogWarning($"'GetDirectories' under UWP (WSA) is supported in combination with 'File Browser PRO'. For more, please see: {BaseConstants.ASSET_FB}");
#endif
         }
         else
         {
            if (!string.IsNullOrEmpty(path))
            {
               try
               {
                  string _path = ValidatePath(path);
#if CT_RTFB && UNITY_ANDROID
                  _dirList.Clear();
                  getDirectoriesRTFB(_path, isRecursive);
                  return _dirList.ToArray();
#else
#if NET_4_6 || NET_STANDARD_2_0
                  return System.IO.Directory.EnumerateDirectories(_path, "*", isRecursive
                     ? System.IO.SearchOption.AllDirectories
                     : System.IO.SearchOption.TopDirectoryOnly).ToArray();
#else
                  return System.IO.Directory.GetDirectories(_path, "*",
                     isRecursive
                        ? System.IO.SearchOption.AllDirectories
                        : System.IO.SearchOption.TopDirectoryOnly);
#endif
#endif
               }
               catch (System.Exception ex)
               {
                  Debug.LogWarning($"Could not scan the path '{path}' for directories: {ex}");
               }
            }
         }

         return System.Array.Empty<string>();
      }

      /// <summary>
      /// Find all logical drives.
      /// </summary>
      /// <returns>Returns array of the found drives. Zero length array when an error occured.</returns>
      public static string[] GetDrives() //NUnit
      {
         if (BaseHelper.isWebPlatform && !BaseHelper.isEditor)
         {
            Debug.LogWarning("'GetDrives' is not supported for the current platform!");
         }
         else if (BaseHelper.isWSABasedPlatform && !BaseHelper.isEditor)
         {
#if CT_FB
#if (UNITY_WSA || UNITY_XBOXONE) && !UNITY_EDITOR && ENABLE_WINMD_SUPPORT
            Crosstales.FB.FileBrowserWSAImpl fbWsa = new Crosstales.FB.FileBrowserWSAImpl();
            fbWsa.isBusy = true;
            UnityEngine.WSA.Application.InvokeOnUIThread(() => { fbWsa.GetDrives(); }, false);

            do
            {
              //wait
            } while (fbWsa.isBusy);

            return fbWsa.Selection.ToArray();
#endif
#else
            Debug.LogWarning($"'GetDrives' under UWP (WSA) is supported in combination with 'File Browser PRO'. For more, please see: {BaseConstants.ASSET_FB}");
#endif
         }
         else
         {
#if (!UNITY_WSA && !UNITY_XBOXONE) || UNITY_EDITOR
            try
            {
               return System.IO.Directory.GetLogicalDrives();
            }
            catch (System.Exception ex)
            {
               Debug.LogWarning($"Could not scan for drives: {ex}");
            }
#endif
         }

         return System.Array.Empty<string>();
      }

      /// <summary>Copy or move a directory.</summary>
      /// <param name="sourceDir">Source directory path</param>
      /// <param name="destDir">Destination directory path</param>
      /// <param name="move">Move directory instead of copy (optional, default: false)</param>
      /// <returns>True if the operation was successful</returns>
      public static bool CopyDirectory(string sourceDir, string destDir, bool move = false) //NUnit
      {
         if (string.IsNullOrEmpty(destDir))
            return false;

         bool success = false;

         if ((BaseHelper.isWSABasedPlatform || BaseHelper.isWebPlatform) && !BaseHelper.isEditor)
         {
            Debug.LogWarning("'CopyDirectory' is not supported for the current platform!");
         }
         else
         {
            try
            {
               string src = ValidatePath(sourceDir);
               string dest = ValidatePath(destDir);

               if (!ExistsDirectory(src))
               {
                  Debug.LogWarning($"Source directory does not exists: {src}");
               }
               else
               {
                  if (ExistsDirectory(dest))
                  {
                     if (BaseConstants.DEV_DEBUG)
                        Debug.LogWarning($"Overwrite destination directory: {dest}");

                     DeleteDirectory(dest);
                  }
#if CT_RTFB && UNITY_ANDROID
                  if (move)
                  {
                     SimpleFileBrowser.FileBrowserHelpers.MoveDirectory(src, dest);
                  }
                  else
                  {
                     SimpleFileBrowser.FileBrowserHelpers.CopyDirectory(src, dest);
                  }

                  success = true;
#else
                  if (move)
                  {
                     System.IO.Directory.Move(src, dest);
                  }
                  else
                  {
                     copyAll(new System.IO.DirectoryInfo(src), new System.IO.DirectoryInfo(dest));
                  }

                  success = true;
#endif
               }
            }
            catch (System.Exception ex)
            {
               Debug.LogError($"Could not {(move ? "move" : "copy")} directory '{sourceDir}' to '{destDir}': {ex}");
               throw;
            }
         }

         return success;
      }

      /// <summary>Copy or move a file.</summary>
      /// <param name="sourceFile">Source file path</param>
      /// <param name="destFile">Destination file path</param>
      /// <param name="move">Move file instead of copy (optional, default: false)</param>
      /// <returns>True if the operation was successful</returns>
      public static bool CopyFile(string sourceFile, string destFile, bool move = false) //NUnit
      {
         if (string.IsNullOrEmpty(destFile))
            return false;

         bool success = false;

         if ((BaseHelper.isWSABasedPlatform || BaseHelper.isWebPlatform) && !BaseHelper.isEditor)
         {
            Debug.LogWarning("'CopyFile' is not supported for the current platform!");
         }
         else
         {
            try
            {
               if (!ExistsFile(sourceFile))
               {
                  Debug.LogWarning($"Source file does not exists: {sourceFile}");
               }
               else
               {
                  string dest = ValidateFile(destFile);

                  CreateDirectory(GetDirectoryName(dest));

                  if (ExistsFile(dest))
                  {
                     if (BaseConstants.DEV_DEBUG)
                        Debug.LogWarning($"Overwrite destination file: {dest}");

                     DeleteFile(dest);
                  }
#if CT_RTFB && UNITY_ANDROID
                  if (move)
                  {
                     SimpleFileBrowser.FileBrowserHelpers.MoveFile(sourceFile, dest);
                  }
                  else
                  {
                     SimpleFileBrowser.FileBrowserHelpers.CopyFile(sourceFile, dest);
                  }

                  success = true;
#else
                  if (move)
                  {
#if UNITY_STANDALONE || UNITY_EDITOR
                     System.IO.File.Move(sourceFile, dest);
#else
                     System.IO.File.Copy(sourceFile, dest);
                     System.IO.File.Delete(sourceFile);
#endif
                  }
                  else
                  {
                     System.IO.File.Copy(sourceFile, dest);
                  }

                  success = true;
#endif
               }
            }
            catch (System.Exception ex)
            {
               Debug.LogError($"Could not {(move ? "move" : "copy")} file '{sourceFile}' to '{destFile}': {ex}");
               throw;
            }
         }

         return success;
      }

      /// <summary>Move a directory.</summary>
      /// <param name="sourceDir">Source directory path</param>
      /// <param name="destDir">Destination directory path</param>
      /// <returns>True if the operation was successful</returns>
      public static bool MoveDirectory(string sourceDir, string destDir)
      {
         return CopyDirectory(sourceDir, destDir, true);
      }

      /// <summary>Move a file.</summary>
      /// <param name="sourceFile">Source file path</param>
      /// <param name="destFile">Destination file path</param>
      /// <returns>True if the operation was successful</returns>
      public static bool MoveFile(string sourceFile, string destFile)
      {
         return CopyFile(sourceFile, destFile, true);
      }

      /// <summary>Renames a directory in a path.</summary>
      /// <param name="path">Path to the directory</param>
      /// <param name="newName">New name for the directory</param>
      /// <returns>New path of the directory</returns>
      public static string RenameDirectory(string path, string newName) //NUnit
      {
         if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(newName))
            return path;

         if ((BaseHelper.isWSABasedPlatform || BaseHelper.isWebPlatform) && !BaseHelper.isEditor)
         {
            Debug.LogWarning("'RenameDirectory' is not supported for the current platform!");
         }
         else
         {
            try
            {
#if CT_RTFB && UNITY_ANDROID
               return SimpleFileBrowser.FileBrowserHelpers.RenameDirectory(path, newName);
#else
               string newPath = System.IO.Path.Combine(new System.IO.DirectoryInfo(path).Parent.FullName, newName);
               System.IO.Directory.Move(path, newPath);

               return newPath;
#endif
            }
            catch (System.Exception ex)
            {
               Debug.LogError($"Could not rename directory '{path}' to '{newName}': {ex}");
               throw;
            }
         }

         return null;
      }

      /// <summary>Renames a file in a path.</summary>
      /// <param name="path">Path to the file</param>
      /// <param name="newName">New name for the file</param>
      /// <returns>New path of the file</returns>
      public static string RenameFile(string path, string newName) //NUnit
      {
         if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(newName))
            return path;

         if ((BaseHelper.isWSABasedPlatform || BaseHelper.isWebPlatform) && !BaseHelper.isEditor)
         {
            Debug.LogWarning("'RenameFile' is not supported for the current platform!");
         }
         else
         {
            try
            {
#if CT_RTFB && UNITY_ANDROID
               return SimpleFileBrowser.FileBrowserHelpers.RenameFile(path, newName);
#else
               string newPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(path), newName);
               System.IO.File.Move(path, newPath);

               return newPath;
#endif
            }
            catch (System.Exception ex)
            {
               Debug.LogError($"Could not rename file '{path}' to '{newName}': {ex}");
               throw;
            }
         }

         return null;
      }

      /// <summary>Delete a file.</summary>
      /// <param name="file">File to delete</param>
      /// <returns>True if the operation was successful</returns>
      public static bool DeleteFile(string file) //NUnit
      {
         bool success = false;

         if ((BaseHelper.isWSABasedPlatform || BaseHelper.isWebPlatform) && !BaseHelper.isEditor)
         {
            Debug.LogWarning("'DeleteFile' is not supported for the current platform!");
         }
         else
         {
            try
            {
               if (!ExistsFile(file))
               {
                  Debug.LogWarning($"File does not exists: {file}");
               }
               else
               {
#if CT_RTFB && UNITY_ANDROID
                  SimpleFileBrowser.FileBrowserHelpers.DeleteFile(file);
                  success = true;
#else
                  System.IO.File.Delete(file);
                  success = true;
#endif
               }
            }
            catch (System.Exception ex)
            {
               Debug.LogError($"Could not delete file '{file}': {ex}");
               throw;
            }
         }

         return success;
      }

      /// <summary>Delete a directory.</summary>
      /// <param name="dir">Directory to delete</param>
      /// <returns>True if the operation was successful</returns>
      public static bool DeleteDirectory(string dir) //NUnit
      {
         bool success = false;

         if ((BaseHelper.isWSABasedPlatform || BaseHelper.isWebPlatform) && !BaseHelper.isEditor)
         {
            Debug.LogWarning("'DeleteDirectory' is not supported for the current platform!");
         }
         else
         {
            try
            {
               if (!ExistsDirectory(dir))
               {
                  Debug.LogWarning($"Source directory does not exists: {dir}");
               }
               else
               {
#if CT_RTFB && UNITY_ANDROID
                  SimpleFileBrowser.FileBrowserHelpers.DeleteDirectory(dir);
                  success = true;
#else
                  System.IO.Directory.Delete(dir, true);
                  success = true;
#endif
               }
            }
            catch (System.Exception ex)
            {
               Debug.LogError($"Could not delete directory '{dir}': {ex}");
               throw;
            }
         }

         return success;
      }

      /// <summary>Checks if the directory exists.</summary>
      /// <returns>True if the directory exists</returns>
      public static bool ExistsFile(string file) //NUnit
      {
         if ((BaseHelper.isWSABasedPlatform || BaseHelper.isWebPlatform) && !BaseHelper.isEditor)
         {
            Debug.LogWarning("'ExistsFile' is not supported for the current platform!");
         }
         else
         {
#if CT_RTFB && UNITY_ANDROID
            return SimpleFileBrowser.FileBrowserHelpers.FileExists(file);
#else
            return System.IO.File.Exists(file);
#endif
         }

         return false;
      }

      /// <summary>Checks if the directory exists.</summary>
      /// <returns>True if the directory exists</returns>
      public static bool ExistsDirectory(string path) //NUnit
      {
         if ((BaseHelper.isWSABasedPlatform || BaseHelper.isWebPlatform) && !BaseHelper.isEditor)
         {
            Debug.LogWarning("'ExistsPath' is not supported for the current platform!");
         }
         else
         {
#if CT_RTFB && UNITY_ANDROID
            return SimpleFileBrowser.FileBrowserHelpers.DirectoryExists(path);
#else
            return System.IO.Directory.Exists(path);
#endif
         }

         return false;
      }

      /// <summary>Creates a directory in a given path.</summary>
      /// <param name="path">Path for the directory</param>
      /// <param name="folderName">New folder</param>
      public static string CreateDirectory(string path, string folderName) //NUnit
      {
         if (string.IsNullOrEmpty(folderName))
            return path;

         if ((BaseHelper.isWSABasedPlatform || BaseHelper.isWebPlatform) && !BaseHelper.isEditor)
         {
            Debug.LogWarning("'CreateDirectory' is not supported for the current platform!");
         }
         else
         {
            try
            {
               if (!ExistsDirectory(path))
               {
                  Debug.LogWarning($"Path directory does not exists: {path}");
               }
               else
               {
#if CT_RTFB && UNITY_ANDROID
                  return SimpleFileBrowser.FileBrowserHelpers.CreateFolderInDirectory(path, folderName);
#else
                  string newPath = System.IO.Path.Combine(path, folderName);
                  System.IO.Directory.CreateDirectory(newPath);
                  return newPath;
#endif
               }
            }
            catch (System.Exception ex)
            {
               Debug.LogError($"Could not create directory at '{path}' with name '{folderName}': {ex}");
               throw;
            }
         }

         return null;
      }

      /// <summary>Creates a directory.</summary>
      /// <param name="path">Path to the directory to create</param>
      /// <returns>True if the operation was successful</returns>
      public static bool CreateDirectory(string path) //NUnit
      {
         if (string.IsNullOrEmpty(path))
            return false;

         bool success = false;

         if ((BaseHelper.isWSABasedPlatform || BaseHelper.isWebPlatform) && !BaseHelper.isEditor)
         {
            Debug.LogWarning("'CreateDirectory' is not supported for the current platform!");
         }
         else
         {
            try
            {
#if CT_RTFB && UNITY_ANDROID
               SimpleFileBrowser.FileBrowserHelpers.CreateFolderInDirectory(GetDirectoryName(path), GetCurrentDirectoryName(path));
               success = true;
#else
               System.IO.Directory.CreateDirectory(path);
               success = true;
#endif
            }
            catch (System.Exception ex)
            {
               Debug.LogError($"Could not create directory '{path}': {ex}");
               throw;
            }
         }

         return success;
      }

      /// <summary>Creates a file in a given path.</summary>
      /// <param name="path">Path for the file</param>
      /// <param name="fileName">New file</param>
      public static string CreateFile(string path, string fileName) //NUnit
      {
         if (string.IsNullOrEmpty(fileName))
            return path;

         if ((BaseHelper.isWSABasedPlatform || BaseHelper.isWebPlatform) && !BaseHelper.isEditor)
         {
            Debug.LogWarning("'CreateFile' is not supported for the current platform!");
         }
         else
         {
            try
            {
               if (!ExistsDirectory(path))
               {
                  Debug.LogWarning($"Path directory does not exists: {path}");
               }
               else
               {
#if CT_RTFB && UNITY_ANDROID
                  return SimpleFileBrowser.FileBrowserHelpers.CreateFileInDirectory(path, fileName);
#else
                  string newPath = System.IO.Path.Combine(path, fileName);
                  using (System.IO.File.Create(newPath))
                  {
                  }

                  return newPath;
#endif
               }
            }
            catch (System.Exception ex)
            {
               Debug.LogError($"Could not create file at '{path}' with name '{fileName}': {ex}");
               throw;
            }
         }

         return null;
      }

      /// <summary>Creates a file.</summary>
      /// <param name="path">Path to the file to create</param>
      /// <returns>True if the operation was successful</returns>
      public static bool CreateFile(string path) //NUnit
      {
         if (string.IsNullOrEmpty(path))
            return false;

         bool success = false;

         if ((BaseHelper.isWSABasedPlatform || BaseHelper.isWebPlatform) && !BaseHelper.isEditor)
         {
            Debug.LogWarning("'CreateFile' is not supported for the current platform!");
         }
         else
         {
            try
            {
#if CT_RTFB && UNITY_ANDROID
               SimpleFileBrowser.FileBrowserHelpers.CreateFileInDirectory(GetDirectoryName(path), GetFileName(path));

               success = true;
#else
               using (System.IO.File.Create(path))
               {
               }

               success = true;
#endif
            }
            catch (System.Exception ex)
            {
               Debug.LogError($"Could not create file '{path}': {ex}");
               throw;
            }
         }

         return success;
      }

      /// <summary>Checks if the path is a directory.</summary>
      /// <param name="path">Path to the directory</param>
      /// <param name="checkForExtensions">Check for extensions (optional, default: true)</param>
      /// <returns>True if the path is a directory</returns>
      public static bool isDirectory(string path, bool checkForExtensions = true) //NUnit
      {
         if (string.IsNullOrEmpty(path))
            return false;

         if ((BaseHelper.isWSABasedPlatform || BaseHelper.isWebPlatform) && !BaseHelper.isEditor)
         {
            Debug.LogWarning("'isDirectory' is not supported for the current platform!");
         }
         else
         {
#if CT_RTFB && UNITY_ANDROID
            return SimpleFileBrowser.FileBrowserHelpers.IsDirectory(path);
#else
            if (ExistsDirectory(path))
               return true;
            if (ExistsFile(path))
               return false;

            if (checkForExtensions)
            {
               string extension = GetExtension(path);
               return extension == null || extension.Length <= 1; // extension includes '.'
            }
#endif
         }

         return false;
      }

      /// <summary>Checks if the path is a file.</summary>
      /// <param name="path">Path to the file</param>
      /// <param name="checkForExtensions">Check for extensions (optional, default: true)</param>
      /// <returns>True if the path is a file</returns>
      public static bool isFile(string path, bool checkForExtensions = true) //NUnit
      {
         return !string.IsNullOrEmpty(path) && !isDirectory(path, checkForExtensions);
      }

      /// <summary>Returns the file name for the path.</summary>
      /// <param name="path">Path to the file</param>
      /// <param name="removeInvalidChars">Removes invalid characters in the file name (optional, default: true)</param>
      /// <returns>File name for the path</returns>
      public static string GetFileName(string path, bool removeInvalidChars = true) //NUnit
      {
         string _path = ValidatePath(path, false, removeInvalidChars);
         string fname = _path;

         if (!string.IsNullOrEmpty(_path))
         {
            try
            {
#if CT_RTFB && UNITY_ANDROID
               fname = SimpleFileBrowser.FileBrowserHelpers.GetFilename(_path);
#else
               fname = System.IO.Path.GetFileName(_path);
#endif
            }
            catch (System.Exception)
            {
               //do nothing
            }

            if (string.IsNullOrEmpty(fname) || fname == _path)
            {
               if (isWindowsPath(_path))
               {
                  fname = _path.Substring(_path.CTLastIndexOf(BaseConstants.PATH_DELIMITER_WINDOWS) + 1);
               }
               else
               {
                  fname = _path.Substring(_path.CTLastIndexOf(BaseConstants.PATH_DELIMITER_UNIX) + 1);
               }
            }

            if (removeInvalidChars)
               fname = string.Join(string.Empty, fname.Split(_invalidFilenameChars)); //.Replace(BaseConstants.PATH_DELIMITER_WINDOWS, string.Empty).Replace(BaseConstants.PATH_DELIMITER_UNIX, string.Empty);
         }

         return fname;
      }

      /// <summary>Returns the current directory name for the path.</summary>
      /// <param name="path">Path to the directory</param>
      /// <returns>Current directory name for the path</returns>
      public static string GetCurrentDirectoryName(string path) //NUnit
      {
         string _path = ValidatePath(path, false);
         string dname = _path;

         if (!string.IsNullOrEmpty(_path))
         {
            try
            {
               dname = new System.IO.DirectoryInfo(_path).Name;
            }
            catch (System.Exception)
            {
               //do nothing
            }

            if (string.IsNullOrEmpty(dname) || dname == _path)
            {
               if (isWindowsPath(_path))
               {
                  dname = _path.Substring(_path.CTLastIndexOf(BaseConstants.PATH_DELIMITER_WINDOWS) + 1);
               }
               else
               {
                  dname = _path.Substring(_path.CTLastIndexOf(BaseConstants.PATH_DELIMITER_UNIX) + 1);
               }

               dname = string.Join(string.Empty, dname.Split(_invalidPathChars));
            }
         }

         return dname;
      }

      /// <summary>Returns the directory name for the path.</summary>
      /// <param name="path">Path to the directory</param>
      /// <returns>Directory name for the path</returns>
      public static string GetDirectoryName(string path) //NUnit
      {
         string dname = path;

         if (!string.IsNullOrEmpty(path))
         {
            bool isUNC = isUNCPath(path);
            bool isWin = isWindowsPath(path);

            try
            {
               bool hadEndDelimiter = !string.IsNullOrEmpty(path) && path.EndsWith(BaseConstants.PATH_DELIMITER_WINDOWS) ||
                                      path.EndsWith(BaseConstants.PATH_DELIMITER_UNIX);

               string _path = ValidatePath(isWin ? "/" + path : path, !BaseConstants.REGEX_FILE.IsMatch(path));

               if (!isURL(_path))
               {
#if CT_RTFB && UNITY_ANDROID
                  dname = SimpleFileBrowser.FileBrowserHelpers.GetDirectoryName(_path);
#else
                  dname = System.IO.Path.GetDirectoryName(_path);
#endif
               }

               if (string.IsNullOrEmpty(dname))
                  dname = _path;

               if (isWin)
               {
                  dname = dname.Substring(1);
               }

               dname = isWin || isUNC ? dname.Replace('/', '\\') : dname.Replace('\\', '/');

               bool hasEndDelimiter = !string.IsNullOrEmpty(dname) && dname.EndsWith(BaseConstants.PATH_DELIMITER_WINDOWS) ||
                                      dname.EndsWith(BaseConstants.PATH_DELIMITER_UNIX);

               if (hadEndDelimiter && !hasEndDelimiter)
               {
                  dname = ValidatePath(dname, true);
               }
               else if (!hadEndDelimiter && hasEndDelimiter)
               {
                  dname = dname.Substring(0, dname.Length - 1);
               }
            }
            catch (System.Exception ex)
            {
               Debug.LogError($"Could not get directory name for '{path}': {ex}");
               throw;
            }
         }

         return dname;
      }

      /// <summary>Returns the size of a file.</summary>
      /// <param name="path">Path of the file</param>
      /// <returns>Size for the file</returns>
      public static long GetFilesize(string path) //NUnit
      {
         if (ExistsFile(path))
         {
            if ((BaseHelper.isWSABasedPlatform || BaseHelper.isWebPlatform) && !BaseHelper.isEditor)
            {
               Debug.LogWarning("'GetFilesize' is not supported for the current platform!");
            }
            else
            {
               try
               {
#if CT_RTFB && UNITY_ANDROID
                  return SimpleFileBrowser.FileBrowserHelpers.GetFilesize(path);
#else
                  return new System.IO.FileInfo(path).Length;
#endif
               }
               catch (System.Exception ex)
               {
                  Debug.LogError($"Could not get file size for '{path}': {ex}");
                  throw;
               }
            }
         }

         Debug.LogWarning($"Path is not a file: {path}");

         return -1;
      }

      /// <summary>Returns the extension of a file.</summary>
      /// <param name="path">Path to the file</param>
      /// <returns>Extension of the file</returns>
      public static string GetExtension(string path) //NUnit
      {
         if (isFile(path, false))
         {
            if ((BaseHelper.isWSABasedPlatform || BaseHelper.isWebPlatform) && !BaseHelper.isEditor)
               return path.Substring(path.LastIndexOf("."));

            try
            {
               string ext = System.IO.Path.GetExtension(path);

               return !string.IsNullOrEmpty(ext) ? ext.Substring(1) : null;
            }
            catch (System.Exception ex)
            {
               Debug.LogError($"Could not get extension for file '{path}': {ex}");
               throw;
            }
         }

         Debug.LogWarning($"File does not exists: {path}");

         return null;
      }

      /// <summary>Returns the size of a file.</summary>
      /// <param name="path">Path to the file</param>
      /// <returns>Size for the file</returns>
      public static System.DateTime GetLastModifiedDate(string path) //NUnit
      {
         if (ExistsFile(path))
         {
            if ((BaseHelper.isWSABasedPlatform || BaseHelper.isWebPlatform) && !BaseHelper.isEditor)
            {
               Debug.LogWarning("'GetLastModifiedDate' is not supported for the current platform!");
            }
            else
            {
               try
               {
#if CT_RTFB && UNITY_ANDROID
                  return SimpleFileBrowser.FileBrowserHelpers.GetLastModifiedDate(path);
#else
                  return new System.IO.FileInfo(path).LastWriteTime;
#endif
               }
               catch (System.Exception ex)
               {
                  Debug.LogError($"Could not get last modify date for '{path}': {ex}");
                  throw;
               }
            }
         }

         Debug.LogWarning($"File does not exists: {path}");

         return System.DateTime.MinValue;
      }

      /// <summary>Reads the text of a file.</summary>
      /// <param name="sourceFile">Source file path</param>
      /// <param name="encoding">Encoding of the text (optional, default: UTF8)</param>
      /// <returns>Text-content of the file</returns>
      public static string ReadAllText(string sourceFile, System.Text.Encoding encoding = null) //NUnit
      {
         if ((BaseHelper.isWSABasedPlatform || BaseHelper.isWebPlatform) && !BaseHelper.isEditor)
         {
            Debug.LogWarning("'ReadAllText' is not supported for the current platform!");
         }
         else
         {
            try
            {
               if (!ExistsFile(sourceFile))
               {
                  Debug.LogWarning($"Source file does not exists: {sourceFile}");
               }
               else
               {
#if CT_RTFB && UNITY_ANDROID
                  return SimpleFileBrowser.FileBrowserHelpers.ReadTextFromFile(sourceFile);
#else
                  return System.IO.File.ReadAllText(sourceFile, encoding ?? System.Text.Encoding.UTF8);
#endif
               }
            }
            catch (System.Exception ex)
            {
               Debug.LogError($"Could not read file '{sourceFile}': {ex}");
               throw;
            }
         }

         return null;
      }

      /// <summary>Reads all lines of text from a file.</summary>
      /// <param name="sourceFile">Source file path</param>
      /// <param name="encoding">Encoding of the text (optional, default: UTF8)</param>
      /// <returns>Array of text lines from the file</returns>
      public static string[] ReadAllLines(string sourceFile, System.Text.Encoding encoding = null) //NUnit
      {
         if ((BaseHelper.isWSABasedPlatform || BaseHelper.isWebPlatform) && !BaseHelper.isEditor)
         {
            Debug.LogWarning("'ReadAllLines' is not supported for the current platform!");
         }
         else
         {
            try
            {
               if (!ExistsFile(sourceFile))
               {
                  Debug.LogWarning($"Source file does not exists: {sourceFile}");
               }
               else
               {
                  return System.IO.File.ReadAllLines(sourceFile, encoding ?? System.Text.Encoding.UTF8);
               }
            }
            catch (System.Exception ex)
            {
               Debug.LogError($"Could not read file '{sourceFile}': {ex}");
               throw;
            }
         }

         return null;
      }

      /// <summary>Reads the bytes of a file.</summary>
      /// <param name="sourceFile">Source file path</param>
      /// <returns>Byte-content of the file</returns>
      public static byte[] ReadAllBytes(string sourceFile) //NUnit
      {
         if ((BaseHelper.isWSABasedPlatform || BaseHelper.isWebPlatform) && !BaseHelper.isEditor)
         {
            Debug.LogWarning("'ReadAllBytes' is not supported for the current platform!");
         }
         else
         {
            try
            {
               if (!ExistsFile(sourceFile))
               {
                  Debug.LogWarning($"Source file does not exists: {sourceFile}");
               }
               else
               {
#if CT_RTFB && UNITY_ANDROID
                  return SimpleFileBrowser.FileBrowserHelpers.ReadBytesFromFile(sourceFile);
#else
                  return System.IO.File.ReadAllBytes(sourceFile);
#endif
               }
            }
            catch (System.Exception ex)
            {
               Debug.LogError($"Could not read file '{sourceFile}': {ex}");
               throw;
            }
         }

         return null;
      }

      /// <summary>Writes text to a file.</summary>
      /// <param name="destFile">Destination file path</param>
      /// <param name="text">Text-content to write</param>
      /// <param name="encoding">Encoding of the text (optional, default: UTF8)</param>
      /// <returns>True if the operation was successful</returns>
      public static bool WriteAllText(string destFile, string text, System.Text.Encoding encoding = null) //NUnit
      {
         if (string.IsNullOrEmpty(destFile))
            return false;

         bool success = false;

         if ((BaseHelper.isWSABasedPlatform || BaseHelper.isWebPlatform) && !BaseHelper.isEditor)
         {
            Debug.LogWarning("'WriteAllText' is not supported for the current platform!");
         }
         else
         {
            try
            {
#if CT_RTFB && UNITY_ANDROID
               SimpleFileBrowser.FileBrowserHelpers.WriteTextToFile(destFile, text);
               success = true;
#else
               System.IO.File.WriteAllText(destFile, text, encoding ?? System.Text.Encoding.UTF8);
               success = true;
#endif
            }
            catch (System.Exception ex)
            {
               Debug.LogError($"Could not write file '{destFile}': {ex}");
               throw;
            }
         }

         return success;
      }

      /// <summary>Writes all lines of text to a file.</summary>
      /// <param name="destFile">Destination file path</param>
      /// <param name="lines">Array of text lines to write</param>
      /// <param name="encoding">Encoding of the text (optional, default: UTF8)</param>
      /// <returns>True if the operation was successful</returns>
      public static bool WriteAllLines(string destFile, string[] lines, System.Text.Encoding encoding = null) //NUnit
      {
         if (string.IsNullOrEmpty(destFile) || lines == null)
            return false;

         bool success = false;

         if ((BaseHelper.isWSABasedPlatform || BaseHelper.isWebPlatform) && !BaseHelper.isEditor)
         {
            Debug.LogWarning("'WriteAllLines' is not supported for the current platform!");
         }
         else
         {
            try
            {
               System.IO.File.WriteAllLines(destFile, lines, encoding ?? System.Text.Encoding.UTF8);
               success = true;
            }
            catch (System.Exception ex)
            {
               Debug.LogError($"Could not write file '{destFile}': {ex}");
               throw;
            }
         }

         return success;
      }

      /// <summary>Writes bytes to a file.</summary>
      /// <param name="destFile">Destination file path</param>
      /// <param name="data">Byte-content to write</param>
      /// <returns>True if the operation was successful</returns>
      public static bool WriteAllBytes(string destFile, byte[] data) //NUnit
      {
         if (string.IsNullOrEmpty(destFile) || data == null)
            return false;

         bool success = false;

         if ((BaseHelper.isWSABasedPlatform || BaseHelper.isWebPlatform) && !BaseHelper.isEditor)
         {
            Debug.LogWarning("'WriteAllBytes' is not supported for the current platform!");
         }
         else
         {
            try
            {
#if CT_RTFB && UNITY_ANDROID
               SimpleFileBrowser.FileBrowserHelpers.WriteBytesToFile(destFile, data);
               success = true;
#else
               System.IO.File.WriteAllBytes(destFile, data);
               success = true;
#endif
            }
            catch (System.Exception ex)
            {
               Debug.LogError($"Could not write file '{destFile}': {ex}");
               throw;
            }
         }

         return success;
      }

      /// <summary>
      /// Shows the location of a path (or file) in OS file explorer.
      /// NOTE: only works on standalone platforms
      /// </summary>
      /// <returns>True if the operation was successful</returns>
      public static bool ShowPath(string path)
      {
         return ShowFile(path);
      }

      /// <summary>
      /// Shows the location of a file (or path) in OS file explorer.
      /// NOTE: only works on standalone platforms
      /// </summary>
      /// <returns>True if the operation was successful</returns>
      public static bool ShowFile(string file)
      {
         bool success = false;

         if (BaseHelper.isStandalonePlatform || BaseHelper.isEditor)
         {
#if UNITY_STANDALONE || UNITY_EDITOR
            string path;

            if (string.IsNullOrEmpty(file) || file.Equals("."))
            {
               path = ".";
            }
            else if ((BaseHelper.isWindowsPlatform || BaseHelper.isWindowsEditor) && file.Length < 4)
            {
               path = file; //root directory
            }
            else
            {
               path = ValidatePath(GetDirectoryName(file));
            }

            try
            {
               if (ExistsDirectory(path))
               {
#if (ENABLE_IL2CPP && CT_PROC) || (CT_DEVELOP && CT_PROC)
                  using (CTProcess process = new CTProcess())
#else
                  using (System.Diagnostics.Process process = new System.Diagnostics.Process())
                  //using (CTProcess process = new CTProcess())
#endif
                  {
                     process.StartInfo.Arguments = $"\"{path}\"";

                     if (BaseHelper.isWindowsPlatform || BaseHelper.isWindowsEditor)
                     {
                        process.StartInfo.FileName = "explorer.exe";
#if (ENABLE_IL2CPP && CT_PROC) || (CT_DEVELOP && CT_PROC)
                        process.StartInfo.UseCmdExecute = true;
#endif
                        process.StartInfo.CreateNoWindow = true;
                     }
                     else if (BaseHelper.isMacOSPlatform || BaseHelper.isMacOSEditor)
                     {
                        process.StartInfo.FileName = "open";
                     }
                     else
                     {
                        process.StartInfo.FileName = "xdg-open";
                     }

                     process.Start();
                     success = true;
                  }
               }
               else
               {
                  Debug.LogWarning($"Path to file doesn't exist: {path}");
               }
            }
            catch (System.Exception ex)
            {
               Debug.LogError($"Could not show file location '{file}': {ex}");
            }
#endif
         }
         else
         {
            Debug.LogWarning("'ShowFileLocation' is not supported on the current platform!");
         }

         return success;
      }

      /// <summary>
      /// Opens a file with the OS default application.
      /// NOTE: only works for standalone platforms
      /// </summary>
      /// <param name="file">File path</param>
      /// <returns>True if the operation was successful</returns>
      public static bool OpenFile(string file)
      {
         bool success = false;

         if (BaseHelper.isStandalonePlatform || BaseHelper.isEditor)
         {
            try
            {
#if UNITY_STANDALONE || UNITY_EDITOR
               if (ExistsFile(file))
               {
#if ENABLE_IL2CPP && CT_PROC
                  using (CTProcess process = new CTProcess())
                  {
                     process.StartInfo.Arguments = $"\"{file}\"";

                     if (BaseHelper.isWindowsPlatform || BaseHelper.isWindowsEditor)
                     {
                        process.StartInfo.FileName = "explorer.exe";
                        process.StartInfo.UseCmdExecute = true;
                        process.StartInfo.CreateNoWindow = true;
                     }
                     else if (BaseHelper.isMacOSPlatform || BaseHelper.isMacOSEditor)
                     {
                        process.StartInfo.FileName = "open";
                     }
                     else
                     {
                        process.StartInfo.FileName = "xdg-open";
                     }

                     process.Start();
                  }
#else
                  using (System.Diagnostics.Process process = new System.Diagnostics.Process())
                  {
                     if (BaseHelper.isMacOSPlatform || BaseHelper.isMacOSEditor)
                     {
                        process.StartInfo.FileName = "open";
                        process.StartInfo.WorkingDirectory = GetDirectoryName(file) + BaseConstants.PATH_DELIMITER_UNIX;
                        process.StartInfo.Arguments = $"-t \"{GetFileName(file)}\"";
                     }
                     else if (BaseHelper.isLinuxPlatform || BaseHelper.isLinuxEditor)
                     {
                        process.StartInfo.FileName = "xdg-open";
                        process.StartInfo.WorkingDirectory = GetDirectoryName(file) + BaseConstants.PATH_DELIMITER_UNIX;
                        process.StartInfo.Arguments = GetFileName(file);
                     }
                     else
                     {
                        process.StartInfo.FileName = file;
                     }

                     process.Start();
                  }
#endif
                  success = true;
               }
               else
               {
                  Debug.LogWarning($"File doesn't exist: {file}");
               }
#endif
            }
            catch (System.Exception ex)
            {
               Debug.LogError($"Could not open file '{file}': {ex}");
            }
         }
         else
         {
            Debug.LogWarning("'OpenFile' is not supported on the current platform!");
         }

         return success;
      }

      #region Legacy

      /// <summary>
      /// Checks a given path for invalid characters
      /// </summary>
      /// <param name="path">Path to check for invalid characters</param>
      /// <returns>Returns true if the path contains invalid chars, otherwise it's false.</returns>
      [System.Obsolete("Please use 'HasPathInvalidChars' instead.")]
      public static bool PathHasInvalidChars(string path)
      {
         return HasPathInvalidChars(path);
      }

      /// <summary>
      /// Checks a given file for invalid characters
      /// </summary>
      /// <param name="file">File to check for invalid characters</param>
      /// <returns>Returns true if the file contains invalid chars, otherwise it's false.</returns>
      [System.Obsolete("Please use 'HasFileInvalidChars' instead.")]
      public static bool FileHasInvalidChars(string file)
      {
         return HasFileInvalidChars(file);
      }

      /// <summary>Copy or move a directory.</summary>
      /// <param name="sourceDir">Source directory path</param>
      /// <param name="destDir">Destination directory path</param>
      /// <param name="move">Move directory instead of copy (optional, default: false)</param>
      /// <returns>True if the operation was successful</returns>
      [System.Obsolete("Please use 'CopyDirectory' instead.")]
      public static bool CopyPath(string sourceDir, string destDir, bool move = false)
      {
         return CopyDirectory(sourceDir, destDir, move);
      }

      /// <summary>Move a directory.</summary>
      /// <param name="sourceDir">Source directory path</param>
      /// <param name="destDir">Destination directory path</param>
      /// <returns>True if the operation was successful</returns>
      [System.Obsolete("Please use 'MoveDirectory' instead.")]
      public static bool MovePath(string sourceDir, string destDir)
      {
         return MoveDirectory(sourceDir, destDir);
      }

      #endregion

      #endregion


      #region Private methods

      private static void copyAll(System.IO.DirectoryInfo source, System.IO.DirectoryInfo target)
      {
         CreateDirectory(target.FullName);

         foreach (System.IO.FileInfo fi in source.GetFiles())
         {
            fi.CopyTo(System.IO.Path.Combine(target.FullName, fi.Name), true);
         }

         // Copy each subdirectory using recursion.
         foreach (System.IO.DirectoryInfo sourceSubDir in source.GetDirectories())
         {
            System.IO.DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(sourceSubDir.Name);
            copyAll(sourceSubDir, nextTargetSubDir);
         }
      }
#if CT_RTFB && UNITY_ANDROID
      private static void getFilesRTFB(string path, bool isRecursive = false, params string[] filenames)
      {
         SimpleFileBrowser.FileSystemEntry[] result = SimpleFileBrowser.FileBrowserHelpers.GetEntriesInDirectory(path, false);

         foreach (SimpleFileBrowser.FileSystemEntry entry in result)
         {
            string filePath = entry.Path;

            if (isFile(filePath))
            {
               if (filenames == null || filenames.Length == 0)
               {
                  _fileList.Add(filePath);
               }
               else
               {
                  foreach (string part in filenames)
                  {
                     if (part == "*.*")
                     {
                        _fileList.Add(filePath);
                        break;
                     }

                     if (part.StartsWith("*."))
                     {
                        if (filePath.CTEndsWith(part.Replace("*", string.Empty)))
                        {
                           _fileList.Add(filePath);
                           break;
                        }
                     }
                     else
                     {
                        if (GetFileName(filePath).CTContains(part.Replace("*", string.Empty)))
                        {
                           _fileList.Add(filePath);
                           break;
                        }
                     }
                  }
               }
            }
            else
            {
               if (isRecursive)
                  getFilesRTFB(filePath, isRecursive, filenames);
            }
         }
      }

      private static void getDirectoriesRTFB(string path, bool isRecursive = false)
      {
         SimpleFileBrowser.FileSystemEntry[] result = SimpleFileBrowser.FileBrowserHelpers.GetEntriesInDirectory(path, false);

         foreach (SimpleFileBrowser.FileSystemEntry entry in result)
         {
            string dirPath = entry.Path;

            if (isDirectory(dirPath))
            {
               _dirList.Add(dirPath);

               if (isRecursive)
                  getDirectoriesRTFB(dirPath, isRecursive);
            }
         }
      }
#endif

      #endregion
   }
}
// © 2015-2023 crosstales LLC (https://www.crosstales.com)