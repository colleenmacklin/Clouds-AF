using UnityEngine;
//using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Crosstales.UI
{
   /// <summary>Resize a UI element.</summary>
   [DisallowMultipleComponent]
   public class UIResize : MonoBehaviour, IPointerDownHandler, IDragHandler
   {
      #region Variables

      /// <summary>Minimum size of the UI element.</summary>
      [Tooltip("Minimum size of the UI element.")] public Vector2 MinSize = new Vector2(300, 160);

      /// <summary>Maximum size of the UI element.</summary>
      [Tooltip("Maximum size of the UI element.")] public Vector2 MaxSize = new Vector2(800, 600);

      /// <summary>Ignore maximum size of the UI element (default: false).</summary>
      [Tooltip("Ignore maximum size of the UI element (default: false).")] public bool IgnoreMaxSize = false;

      /// <summary>Resize speed (default: 2).</summary>
      [Tooltip("Resize speed (default: 2).")] public float SpeedFactor = 2;

      private RectTransform panelRectTransform;
      private Vector2 originalLocalPointerPosition;
      private Vector2 originalSizeDelta;
      private Vector2 originalSize;

      #endregion


      #region MonoBehaviour methods

      private void Awake()
      {
         panelRectTransform = transform.parent.GetComponent<RectTransform>();
         Rect rect = panelRectTransform.rect;
         originalSize = new Vector2(rect.width, rect.height);
      }

      #endregion


      #region Implemented methods

      public void OnPointerDown(PointerEventData data)
      {
         originalSizeDelta = panelRectTransform.sizeDelta;

         RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRectTransform, data.position, data.pressEventCamera, out originalLocalPointerPosition);
      }

      public void OnDrag(PointerEventData data)
      {
         if (panelRectTransform == null)
            return;

         RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRectTransform, data.position, data.pressEventCamera, out Vector2 localPointerPosition);
         Vector3 offsetToOriginal = localPointerPosition - originalLocalPointerPosition;

         Vector2 sizeDelta = originalSizeDelta + new Vector2(offsetToOriginal.x * SpeedFactor, -offsetToOriginal.y * SpeedFactor);

         if (originalSize.x + sizeDelta.x < MinSize.x)
         {
            sizeDelta.x = -(originalSize.x - MinSize.x);
         }
         else if (!IgnoreMaxSize && originalSize.x + sizeDelta.x > MaxSize.x)
         {
            sizeDelta.x = MaxSize.x - originalSize.x;
         }

         if (originalSize.y + sizeDelta.y < MinSize.y)
         {
            sizeDelta.y = -(originalSize.y - MinSize.y);
         }
         else if (!IgnoreMaxSize && originalSize.y + sizeDelta.y > MaxSize.y)
         {
            sizeDelta.y = MaxSize.y - originalSize.y;
         }

         panelRectTransform.sizeDelta = sizeDelta;
      }

      #endregion
   }
}
// © 2018-2022 crosstales LLC (https://www.crosstales.com)