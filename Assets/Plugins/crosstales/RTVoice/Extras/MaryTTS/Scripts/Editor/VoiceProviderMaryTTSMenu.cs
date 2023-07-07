#if UNITY_EDITOR
using UnityEditor;

namespace Crosstales.RTVoice.MaryTTS
{
   /// <summary>Editor component for for adding the prefabs from 'MaryTTS' in the "Tools"-menu.</summary>
   public static class VoiceProviderMaryTTSMenu
   {
      [MenuItem("Tools/" + Crosstales.RTVoice.Util.Constants.ASSET_NAME + "/Prefabs/VoiceProviderMaryTTS", false, Crosstales.RTVoice.EditorUtil.EditorHelper.MENU_ID + 200)]
      private static void AddVoiceProvider()
      {
         Crosstales.RTVoice.EditorUtil.EditorHelper.InstantiatePrefab("MaryTTS", $"{Crosstales.RTVoice.EditorUtil.EditorConfig.ASSET_PATH}Extras/MaryTTS/Resources/Prefabs/");
      }

      [MenuItem("Tools/" + Crosstales.RTVoice.Util.Constants.ASSET_NAME + "/Prefabs/VoiceProviderMaryTTS", true)]
      private static bool AddVoiceProviderValidator()
      {
         return !VoiceProviderMaryTTSEditor.isPrefabInScene;
      }
   }
}
#endif
// © 2020-2023 crosstales LLC (https://www.crosstales.com)