using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    public class ScrollToIndex : MonoBehaviour
    {
        [SerializeField] ScrollRect scrollRect;
        [SerializeField] RectTransform contentRect;
        [SerializeField] VerticalLayoutGroup verticalLayoutGroup;


        public void ScrollToLevelIndex(int index)
        {
            int totalItemCount = contentRect.childCount;

            if (index < 0 || index >= totalItemCount)
            {
                Debug.LogWarning("Level index out of range.");
                return;
            }
            float itemHeight = ((RectTransform)contentRect.GetChild(0)).rect.height;
            float spacing = verticalLayoutGroup.spacing;
            RectOffset padding = verticalLayoutGroup.padding;
            float itemPosY = padding.top + index * (itemHeight + spacing);
            float contentHeight = padding.top + padding.bottom + totalItemCount * itemHeight + (totalItemCount - 1) * spacing;
            float viewportHeight = scrollRect.viewport.rect.height;
            float scrollableHeight = contentHeight - viewportHeight;
            if (scrollableHeight <= 0)
            {
                scrollRect.verticalNormalizedPosition = 1;
                return;
            }
            float targetPosY = itemPosY + itemHeight / 2 - viewportHeight / 2;
            float normalizedScrollPos = Mathf.Clamp01(targetPosY / scrollableHeight);
            scrollRect.verticalNormalizedPosition = 1 - normalizedScrollPos;
        }
    }
}
