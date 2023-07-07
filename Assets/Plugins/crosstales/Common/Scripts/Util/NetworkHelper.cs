using System.Linq;
using UnityEngine;

namespace Crosstales.Common.Util
{
   /// <summary>Base for various helper functions for networking.</summary>
   public abstract class NetworkHelper
   {
      #region Variables

      protected const string FILE_PREFIX = "file://";
#if UNITY_ANDROID
      protected const string CONTENT_PREFIX = "content://";
#endif

      #endregion


      #region Properties

      /// <summary>Checks if an Internet connection is available.</summary>
      /// <returns>True if an Internet connection is available.</returns>
      public static bool isInternetAvailable
      {
         get
         {
#if CT_OC
            if (OnlineCheck.OnlineCheck.Instance == null)
            {
               return Application.internetReachability != NetworkReachability.NotReachable;
            }
            else
            {
               return OnlineCheck.OnlineCheck.Instance.isInternetAvailable;
            }
#else
            return Application.internetReachability != NetworkReachability.NotReachable;
#endif
         }
      }

      #endregion


      #region Public methods

      /// <summary>Opens the given URL with the file explorer or browser.</summary>
      /// <param name="url">URL to open</param>
      /// <returns>True if the operation was successful</returns>
      public static bool OpenURL(string url)
      {
         if (isURL(url))
         {
            openURL(url);

            return true;
         }

         Debug.LogWarning($"URL was invalid: {url}");
         return false;
      }

#if (!UNITY_WSA && !UNITY_XBOXONE) || UNITY_EDITOR
      /// <summary>HTTPS-certification callback.</summary>
      public static bool RemoteCertificateValidationCallback(object sender,
         System.Security.Cryptography.X509Certificates.X509Certificate certificate,
         System.Security.Cryptography.X509Certificates.X509Chain chain,
         System.Net.Security.SslPolicyErrors sslPolicyErrors)
      {
         bool isOk = true;

         // If there are errors in the certificate chain, look at each error to determine the cause.
         if (sslPolicyErrors != System.Net.Security.SslPolicyErrors.None)
         {
            foreach (System.Security.Cryptography.X509Certificates.X509ChainStatus t in chain.ChainStatus.Where(t =>
                        t.Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags
                           .RevocationStatusUnknown))
            {
               chain.ChainPolicy.RevocationFlag = System.Security.Cryptography.X509Certificates.X509RevocationFlag.EntireChain;
               chain.ChainPolicy.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.Online;
               chain.ChainPolicy.UrlRetrievalTimeout = new System.TimeSpan(0, 1, 0);
               chain.ChainPolicy.VerificationFlags = System.Security.Cryptography.X509Certificates.X509VerificationFlags.AllFlags;

               isOk = chain.Build((System.Security.Cryptography.X509Certificates.X509Certificate2)certificate);
            }
         }

         return isOk;
      }
#endif
      /// <summary>Returns the URL of a given file.</summary>
      /// <param name="path">File path</param>
      /// <returns>URL of the file path</returns>
      public static string GetURLFromFile(string path) //NUnit
      {
         if (!string.IsNullOrEmpty(path))
         {
            if (!isURL(path))
               return Crosstales.Common.Util.BaseConstants.PREFIX_FILE + System.Uri.EscapeUriString(Crosstales.Common.Util.FileHelper.ValidateFile(path).Replace('\\', '/'));

            return System.Uri.EscapeUriString(Crosstales.Common.Util.FileHelper.ValidateFile(path).Replace('\\', '/'));
         }

         return path;
      }

      /// <summary>Validates a given URL.</summary>
      /// <param name="url">URL to validate</param>
      /// <param name="removeProtocol">Remove the protocol, e.g. http:// (optional, default: false)</param>
      /// <param name="removeWWW">Remove www (optional, default: true)</param>
      /// <param name="removeSlash">Remove slash at the end (optional, default: true)</param>
      /// <returns>Clean URL</returns>
      public static string ValidateURL(string url, bool removeProtocol = false, bool removeWWW = true, bool removeSlash = true) //NUnit
      {
         if (isURL(url))
         {
            string result = url?.Trim().Replace('\\', '/');

            if (removeProtocol)
               result = result.Substring(result.CTIndexOf("//") + 2);

            if (removeWWW)
               result = result.CTReplace("www.", string.Empty);

            if (removeSlash && result.CTEndsWith(Crosstales.Common.Util.BaseConstants.PATH_DELIMITER_UNIX))
               result = result.Substring(0, result.Length - 1);

            return System.Uri.EscapeUriString(result);
         }

         return url;
      }

      /// <summary>Checks if the input is an URL.</summary>
      /// <param name="url">Input as possible URL</param>
      /// <returns>True if the given path is an URL</returns>
      public static bool isURL(string url) //NUnit
      {
         return !string.IsNullOrEmpty(url) &&
                (url.StartsWith(FILE_PREFIX, System.StringComparison.OrdinalIgnoreCase) ||
#if UNITY_ANDROID
                 url.StartsWith(CONTENT_PREFIX, System.StringComparison.OrdinalIgnoreCase) ||
#endif
                 url.StartsWith(Crosstales.Common.Util.BaseConstants.PREFIX_HTTP, System.StringComparison.OrdinalIgnoreCase) ||
                 url.StartsWith(Crosstales.Common.Util.BaseConstants.PREFIX_HTTPS, System.StringComparison.OrdinalIgnoreCase));
      }

      /// <summary>Checks if the input is an IPv4 address.</summary>
      /// <param name="url">Input as possible IPv4</param>
      /// <returns>True if the given path is an IPv4 address</returns>
      public static bool isIPv4(string ip) //NUnit
      {
         if (!string.IsNullOrEmpty(ip) && BaseConstants.REGEX_IP_ADDRESS.IsMatch(ip))
         {
            string[] ipBytes = ip.Split('.');

            foreach (string ipByte in ipBytes)
            {
               if (int.TryParse(ipByte, out int val) && val > 255 || val < 0)
                  return false;
            }

            return true;
         }

         return false;
      }

      /// <summary>Returns the IP of a given host name.</summary>
      /// <param name="host">Host name</param>
      /// <returns>IP of a given host name.</returns>
      public static string GetIP(string host) //NUnit
      {
         string validHost = ValidateURL(host, isURL(host));

         if (!string.IsNullOrEmpty(validHost))
         {
#if (!UNITY_WSA && !UNITY_WEBGL && !UNITY_XBOXONE) || UNITY_EDITOR
            try
            {
               string ip = System.Net.Dns.GetHostAddresses(validHost)[0].ToString();
               return ip == "::1" ? "127.0.0.1" : ip;
            }
            catch (System.Exception ex)
            {
               Debug.LogWarning($"Could not resolve host '{host}': {ex}");
            }
#else
            Debug.LogWarning("'GetIP' doesn't work in WebGL or WSA! Returning original string.");
#endif
         }
         else
         {
            Debug.LogWarning("Host name is null or empty - can't resolve to IP!");
         }

         return host;
      }

      #region Legacy

      /// <summary>Returns the URL of a given file.</summary>
      /// <param name="path">File path</param>
      /// <returns>URL of the file path</returns>
      [System.Obsolete("Please use 'GetURLFromFile' instead.")]
      public static string ValidURLFromFilePath(string path)
      {
         return GetURLFromFile(path);
      }

      /// <summary>Cleans a given URL.</summary>
      /// <param name="url">URL to clean</param>
      /// <param name="removeProtocol">Remove the protocol, e.g. http:// (optional, default: true)</param>
      /// <param name="removeWWW">Remove www (optional, default: true)</param>
      /// <param name="removeSlash">Remove slash at the end (optional, default: true)</param>
      /// <returns>Clean URL</returns>
      [System.Obsolete("Please use 'ValidateURL' instead.")]
      public static string CleanUrl(string url, bool removeProtocol = true, bool removeWWW = true, bool removeSlash = true)
      {
         return ValidateURL(url, removeProtocol, removeWWW, removeSlash);
      }

      /// <summary>Checks if the URL is valid.</summary>
      /// <param name="url">URL to check</param>
      /// <returns>True if the URL is valid.</returns>
      [System.Obsolete("Please use 'isURL' instead.")]
      public static bool isValidURL(string url)
      {
         return isURL(url);
      }

      #endregion

      #endregion

      #region Private methods

      private static void openURL(string url)
      {
#if !UNITY_EDITOR && UNITY_WEBGL && CT_OPENWINDOW
         openURLPlugin(url);
#else
         Application.OpenURL(url);
#endif
      }
/*
      private static void openURLJS(string url)
      {
         Application.ExternalEval("window.open('" + url + "');");
      }
*/
#if !UNITY_EDITOR && UNITY_WEBGL && CT_OPENWINDOW
      private static void openURLPlugin(string url)
      {
		   ctOpenWindow(url);
      }

      [System.Runtime.InteropServices.DllImportAttribute("__Internal")]
      private static extern void ctOpenWindow(string url);
#endif

      #endregion
   }
}
// © 2015-2023 crosstales LLC (https://www.crosstales.com)