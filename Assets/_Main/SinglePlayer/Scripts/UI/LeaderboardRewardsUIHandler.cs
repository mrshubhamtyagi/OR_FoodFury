using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FoodFury
{
    public class LeaderboardRewardsUIHandler : MonoBehaviour
    {
        [SerializeField] private GameObject chipsGameObject;
        [SerializeField] private TextMeshProUGUI chipsText;
        [SerializeField] private TextMeshProUGUI cardsText;
        [SerializeField] private TextMeshProUGUI separatorText;
        [SerializeField] private GameObject cardsGameObject;
        [SerializeField] private GameObject separatorGameObject;
        public void SetData(int chips, bool isPlayer, int cards = 0)
        {
            chipsGameObject.SetActive(true);
            cardsGameObject.SetActive(cards != 0);
            separatorGameObject.SetActive(cards != 0);

            cardsText.color = isPlayer ? Color.black : Color.white;
            chipsText.color = isPlayer ? Color.black : Color.white;
            separatorText.color = isPlayer ? Color.black : Color.white;




            cardsText.text = cards != 0 ? cards.ToString() + " CARDS" : "0";
            chipsText.text = chips.ToString();
        }
    }
}
