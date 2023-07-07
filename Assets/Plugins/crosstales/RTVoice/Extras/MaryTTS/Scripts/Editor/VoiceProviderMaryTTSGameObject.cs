#if UNITY_EDITOR
using UnityEditor;

namespace Crosstales.RTVoice.MaryTTS
{
   /// <summary>Editor component for for adding the prefabs from 'MaryTTS' in the "Hierarchy"-menu.</summary>
   public static class VoiceProviderMaryTTSGameObject
   {
      [MenuItem("GameObject/" + Crosstales.RTVoice.Util.Constants.ASSET_NAME + "/VoiceProviderMaryTTS", false, Crosstales.RTVoice.EditorUtil.EditorHelper.GO_ID + 10)]
      private static void AddVoiceProvider()
      {
         Crosstales.RTVoice.EditorUtil.EditorHelper.InstantiatePrefab("MaryTTS", $"{Crosstales.RTVoice.EditorUtil.EditorConfig.ASSET_PATH}Extras/MaryTTS/Resources/Prefabs/");
      }

      [MenuItem("GameObject/" + Crosstales.RTVoice.Util.Constants.ASSET_NAME + "/VoiceProviderMaryTTS", true)]
      private static bool AddVoiceProviderValidator()
      {
         return !VoiceProviderMaryTTSEditor.isPrefabInScene;
      }
   }
}
#endif
// © 2020-2023 crosstales LLC (https://www.crosstales.com)