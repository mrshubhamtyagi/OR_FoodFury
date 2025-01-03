using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    public class ScrollToLevel : MonoBehaviour
    {
        [SerializeField] ScrollRect scrollRect;
        [SerializeField] RectTransform contentRect;
        [SerializeField] GridLayoutGroup gridLayoutGroup;

        private int index;


        public void ScrollToSelectedLevel()
        {
            index = GameData.Instance.PlayerData.Data.playerLevel[GameData.Instance.SelectedMapId - 1].levelNumber - 1;

            Vector2 cellSize = gridLayoutGroup.cellSize;
            Vector2 spacing = gridLayoutGroup.spacing;
            RectOffset padding = gridLayoutGroup.padding;


            int totalItemCount = contentRect.childCount;


            if (index < 0 || index >= totalItemCount)
            {
                Debug.LogWarning("Level index out of range.");
                index = GameData.Instance.LevelData.levels.Count - 1;
            }


            float contentWidth = contentRect.rect.width - padding.left - padding.right;


            int columnCount = Mathf.FloorToInt((contentWidth + spacing.x + 0.01f) / (cellSize.x + spacing.x));
            if (columnCount < 1) columnCount = 1;


            int row = index / columnCount;
            int column = index % columnCount;


            int totalRowCount = Mathf.CeilToInt((float)totalItemCount / columnCount);

            float contentHeight = padding.top + padding.bottom + (cellSize.y * totalRowCount) + (spacing.y * (totalRowCount - 1));


            float viewportHeight = scrollRect.viewport.rect.height;


            float itemPosY = padding.top + row * (cellSize.y + spacing.y);


            float scrollableHeight = contentHeight - viewportHeight;
            if (scrollableHeight <= 0)
            {

                scrollRect.verticalNormalizedPosition = 1;
                return;
            }


            float targetPosY = itemPosY + cellSize.y / 2 - viewportHeight / 2;


            float normalizedScrollPos = Mathf.Clamp01(targetPosY / scrollableHeight);


            scrollRect.verticalNormalizedPosition = 1 - normalizedScrollPos;
        }
    }
}
