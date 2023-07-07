using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Crosstales.RTVoice.Demo
{
   /// <summary>Main GUI component for all demo scenes.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_demo_1_1_g_u_i_main.html")]
   public class GUIMain : MonoBehaviour
   {
      #region Variables

      [Header("UI Objects")] public Text Name;
      public Text Version;
      public Text Scene;
      public GameObject NoVoices;
      public Text Errors;

      #endregion


      #region MonoBehaviour methods

      private void Start()
      {
         Speaker.Instance.OnVoicesReady += onVoicesReady;
         Speaker.Instance.OnErrorInfo += onErrorInfo;
         Speaker.Instance.OnSpeakStart += onSpeakStart;

         if (Name != null)
            Name.text = RTVoice.Util.Constants.ASSET_NAME;

         if (Version != null)
            Version.text = RTVoice.Util.Constants.ASSET_VERSION;

         if (Scene != null)
            Scene.text = SceneManager.GetActiveScene().name;

         if (NoVoices != null)
            NoVoices.SetActive(Speaker.Instance.Voices.Count <= 0);

         if (Errors != null)
            Errors.text = string.Empty;
      }

      private void Update()
      {
         Cursor.visible = true;
      }

      private void OnDestroy()
      {
         if (Speaker.Instance != null)
         {
            Speaker.Instance.OnVoicesReady -= onVoicesReady;
            Speaker.Instance.OnErrorInfo -= onErrorInfo;
            Speaker.Instance.OnSpeakStart -= onSpeakStart;
         }
      }

      #endregion


      #region Public methods

      public void OpenAssetURL()
      {
         Crosstales.Common.Util.NetworkHelper.OpenURL(RTVoice.Util.Constants.ASSET_CT_URL);
      }

      public void OpenCTURL()
      {
         Crosstales.Common.Util.NetworkHelper.OpenURL(RTVoice.Util.Constants.ASSET_AUTHOR_URL);
      }

      public void Silence()
      {
         Speaker.Instance.Silence();
      }

      public void Quit()
      {
         if (Application.isEditor)
         {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
         }
         else
         {
            Application.Quit();
         }
      }

      #endregion


      #region Callbacks

      private void onVoicesReady()
      {
         if (NoVoices != null)
            NoVoices.SetActive(Speaker.Instance.Voices.Count <= 0);

         if (Errors != null)
            Errors.text = string.Empty;
      }

      private void onErrorInfo(Crosstales.RTVoice.Model.Wrapper wrapper, string errorInfo)
      {
         if (Errors != null)
            Errors.text = errorInfo;
      }

      private void onSpeakStart(Crosstales.RTVoice.Model.Wrapper wrapper)
      {
         if (Errors != null)
            Errors.text = string.Empty;
      }

      #endregion
   }
}
// © 2015-2022 crosstales LLC (https://www.crosstales.com)