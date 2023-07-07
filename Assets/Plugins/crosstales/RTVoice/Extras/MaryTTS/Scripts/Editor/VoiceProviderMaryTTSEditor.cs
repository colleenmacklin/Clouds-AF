#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Crosstales.RTVoice.MaryTTS.Model.Enum;

namespace Crosstales.RTVoice.MaryTTS
{
   /// <summary>Custom editor for the 'VoiceProviderMaryTTS'-class.</summary>
   [CustomEditor(typeof(VoiceProviderMaryTTS))]
   public class VoiceProviderMaryTTSEditor : Editor
   {
      #region Variables

      private VoiceProviderMaryTTS script;

      private string url;
      private int port;
      private string username;
      private string password;
      private MaryTTSType type;

      #endregion


      #region Properties

      public static bool isPrefabInScene => GameObject.Find("MaryTTS") != null;

      #endregion


      #region Editor methods

      private void OnEnable()
      {
         script = (VoiceProviderMaryTTS)target;
      }

      public override void OnInspectorGUI()
      {
         Crosstales.RTVoice.EditorUtil.EditorHelper.BannerOC();

         //DrawDefaultInspector();

         if (GUILayout.Button(new GUIContent(" Learn more", Crosstales.RTVoice.EditorUtil.EditorHelper.Icon_Manual, "Learn more about MaryTTS.")))
            Crosstales.Common.Util.NetworkHelper.OpenURL("http://mary.dfki.de/");

         GUILayout.Label("MaryTTS Connection", EditorStyles.boldLabel);

         url = EditorGUILayout.TextField(new GUIContent("URL", "Server URL for MaryTTS."), script.URL);
         if (!url.Equals(script.URL))
         {
            serializedObject.FindProperty("url").stringValue = url;
            serializedObject.ApplyModifiedProperties();

            Speaker.Instance.ReloadProvider();
         }

         port = EditorGUILayout.IntSlider("Port", script.Port, 0, 65535);
         if (port != script.Port)
         {
            serializedObject.FindProperty("port").intValue = port;
            serializedObject.ApplyModifiedProperties();

            //script.ReloadProvider();
         }

         username = EditorGUILayout.TextField(new GUIContent("Username", "Username for MaryTTS (default: empty)."), script.Username);
         if (!username.Equals(script.Username))
         {
            serializedObject.FindProperty("username").stringValue = username;
            serializedObject.ApplyModifiedProperties();

            Speaker.Instance.ReloadProvider();
         }

         password = EditorGUILayout.PasswordField(new GUIContent("Password", "User password for MaryTTS (default: empty)."), script.Password);
         if (!password.Equals(script.Password))
         {
            serializedObject.FindProperty("password").stringValue = password;
            serializedObject.ApplyModifiedProperties();

            Speaker.Instance.ReloadProvider();
         }

         type = (MaryTTSType)EditorGUILayout.EnumPopup(new GUIContent("Type", "Input type for MaryTTS (default: MaryTTSType.RAWMARYXML)."), script.Type);
         if (type != script.Type)
         {
            serializedObject.FindProperty("type").enumValueIndex = (int)type;
            serializedObject.ApplyModifiedProperties();

            Speaker.Instance.ReloadProvider();
         }

         if (script.isActiveAndEnabled)
         {
            if (string.IsNullOrEmpty(url))
            {
               EditorGUILayout.HelpBox("'URL' is null or empty! Please add a valid MaryTTS-server.", MessageType.Warning);
            }
            else
            {
               if (url.Contains("mary.dfki.de") || url.Contains("crosstales.com") || url.Contains("46.101.111.65"))
               {
                  EditorGUILayout.HelpBox("You are using the test server of MaryTTS. Please setup your own server from 'http://mary.dfki.de'.", MessageType.Warning);
               }
            }
         }
         else
         {
            Crosstales.RTVoice.EditorUtil.EditorHelper.SeparatorUI();
            EditorGUILayout.HelpBox("Script is disabled!", MessageType.Info);
         }
      }

      #endregion
   }
}
#endif
// © 2020-2023 crosstales LLC (https://www.crosstales.com)