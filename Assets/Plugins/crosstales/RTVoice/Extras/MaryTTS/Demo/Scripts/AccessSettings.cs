using UnityEngine;
using UnityEngine.UI;

namespace Crosstales.RTVoice.MaryTTS
{
   /// <summary>Set the access settings for MaryTTS.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_mary_t_t_s_1_1_access_settings.html")]
   public class AccessSettings : MonoBehaviour
   {
      #region Variables

      public VoiceProviderMaryTTS Provider;

      public GameObject SettingsPanel;

      public InputField URL;

      public InputField Port;

      public InputField Username;

      public InputField Password;

      public Button OkButton;

      private string enteredUrl;
      private int enteredPort;
      private string enteredUsername;
      private string enteredPassword;

      private static string lastUrl;
      private static int lastPort;
      private static string lastUsername;
      private static string lastPassword;

      private Color okColor;

      #endregion


      #region MonoBehaviour methods

      private void Start()
      {
         okColor = OkButton.image.color;

         if (!string.IsNullOrEmpty(lastUrl))
            Provider.URL = lastUrl;

         if (lastPort != 0)
            Provider.Port = lastPort;

         if (!string.IsNullOrEmpty(lastUsername))
            Provider.Username = lastUsername;

         if (!string.IsNullOrEmpty(lastPassword))
            Provider.Password = lastPassword;

         if (!string.IsNullOrEmpty(Provider.URL))
            enteredUrl = lastUrl = URL.text = Provider.URL;

         if (Provider.Port != 0)
         {
            enteredPort = lastPort = Provider.Port;
            Port.text = Provider.Port.ToString();
         }

         if (!string.IsNullOrEmpty(Provider.Username))
            enteredUsername = lastUsername = Username.text = Provider.Username;

         if (!string.IsNullOrEmpty(Provider.Password))
            enteredPassword = lastPassword = Password.text = Provider.Password;


         if (string.IsNullOrEmpty(Provider.URL))
         {
            ShowSettings();
         }
         else
         {
            HideSettings();
         }

         SetOkButton();
      }

      #endregion


      #region Public methods

      public void OnURLEntered(string url)
      {
         enteredUrl = url ?? string.Empty;
         SetOkButton();
      }

      public void OnPortEntered(string port)
      {
         enteredPort = int.TryParse(port, out int parsedPort) ? parsedPort : 59125;
         SetOkButton();
      }

      public void OnUserEntered(string user)
      {
         enteredUsername = user ?? string.Empty;
         SetOkButton();
      }

      public void OnPasswordEntered(string password)
      {
         enteredPassword = password ?? string.Empty;
         SetOkButton();
      }

      public void HideSettings()
      {
         SettingsPanel.SetActive(false);

         if (!string.IsNullOrEmpty(enteredUrl) && (!enteredUrl.Equals(lastUrl) || enteredPort != lastPort || (!string.IsNullOrEmpty(enteredPassword) && !enteredPassword.Equals(lastPassword)) || (!string.IsNullOrEmpty(lastUsername) && !enteredUsername.Equals(lastUsername))))
         {
            lastUrl = Provider.URL = enteredUrl;
            lastPort = Provider.Port = enteredPort;
            lastUsername = Provider.Username = enteredUsername;
            lastPassword = Provider.Password = enteredPassword;

            Crosstales.RTVoice.Speaker.Instance.ReloadProvider();
         }
      }

      public void ShowSettings()
      {
         SettingsPanel.SetActive(true);
      }

      public void SetOkButton()
      {
         if (Crosstales.Common.Util.NetworkHelper.isURL(enteredUrl))
         {
            OkButton.interactable = true;
            OkButton.image.color = okColor;
         }
         else
         {
            OkButton.interactable = false;
            OkButton.image.color = Color.gray;
         }
      }

      #endregion
   }
}
// © 2020-2023 crosstales LLC (https://www.crosstales.com)