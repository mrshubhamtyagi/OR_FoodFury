using System.Collections;
using System.Collections.Generic;
using FoodFury;
using TMPro;
using UnityEngine;

namespace OneRare.FoodFury.Multiplayer
{
    public class LeaderboardUIItem : MonoBehaviour
    {
        public TextMeshProUGUI rankText;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI scoreText;
        //public GameObject expandButton;
        private UIManager uiManager;


        public void SetLeaderBoardData(string name, string place, int totalPlayer, int score, bool displayTotalPlayers = true)
        {
            rankText.text = displayTotalPlayers ? place + "/" + totalPlayer : place.ToString();
            nameText.text = name;
            /*if (GameData.Instance.serverMode == global::Enums.ServerMode.TestNet)
            {
            }
            else
            {
                nameText.text = string.Empty;
            }*/
            scoreText.text = score.ToString();
        }

        public void OnExpandButtonClick()
        {
            uiManager = UIManager.Instance;
            uiManager.OnShowFullLeaderboardButtonClicked();
            //expandButton.SetActive(false);
            //Invoke("ShowExpandButton",3.1f);
        }

        /*void ShowExpandButton()
        {
            expandButton.SetActive(true);
        }

        public void HideExpandButton()
        {
            expandButton.SetActive(false);
        }*/
    }
}