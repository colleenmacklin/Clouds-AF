#if UNITY_EDITOR
using UnityEditor;

namespace Crosstales.UI
{
   /// <summary>Adds the given define symbols to PlayerSettings define symbols.</summary>
   [InitializeOnLoad]
   public class CompileDefines : Common.EditorTask.BaseCompileDefines
   {
      private const string symbol = "CT_UI";

      static CompileDefines()
      {
         addSymbolsToAllTargets(symbol);
      }
   }
}
#endif
// © 2020-2022 crosstales LLC (https://www.crosstales.com)