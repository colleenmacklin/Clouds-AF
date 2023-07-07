#if UNITY_EDITOR
using UnityEditor;

namespace Crosstales.Common.Util
{
   /// <summary>Adds "CT_PROC" define symbol to PlayerSettings define symbols.</summary>
   [InitializeOnLoad]
   public class CTPCompileDefines : Crosstales.Common.EditorTask.BaseCompileDefines
   {
      private const string SYMBOL = "CT_PROC";

      static CTPCompileDefines()
      {
         addSymbolsToAllTargets(SYMBOL);
      }
   }
}
#endif
// © 2021-2023 crosstales LLC (https://www.crosstales.com)