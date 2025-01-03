using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FoodFury
{
    public class MunchWithCards : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI munchText;
        [SerializeField] private List<DishCard> cards;
        public void SetData(List<ModelClass.Dish> dishes, int munches)
        {
            munchText.text = munches.ToString();
            foreach (DishCard card in cards)
            {
                card.gameObject.SetActive(false);
            }
            for (int i = 0; i < dishes.Count; i++)
            {
                if (i < cards.Count)
                {
                    cards[i].gameObject.SetActive(true);
                    cards[i].FillData(dishes[i]);
                }
                else
                {
                    OverlayWarningPopup.Instance.ShowWarning("No more cards to show");
                }
            }
        }
    }
}
