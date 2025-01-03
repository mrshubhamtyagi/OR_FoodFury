using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    public class ChestCard : MonoBehaviour
    {
        [SerializeField] private Enums.ChestType chestType;
        [SerializeField] private Button buyNowButton;
        [SerializeField] private Button unlockNowButton;
        [SerializeField] private Image insufficientCostImage;
        [SerializeField] private GameObject unlockTimer;
        [SerializeField] private GameObject costGameobject;
        [SerializeField] private TextMeshProUGUI unlockTimeText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private GameTimer gameTimer;
        [SerializeField] private int cost;
        public ModelClass.ChestData chestData;
        private void Awake()
        {
            chestData = null;
        }
        private void OnEnable()
        {
            GameData.OnPlayerDataUpdate += updateCard;
            gameTimer.onTimerUpdate += UpdateTime;
            gameTimer.onTimerComplete += OnTimerComplete;
        }

        private void OnTimerComplete()
        {
            unlockTimer.SetActive(false);
            unlockNowButton.interactable = true;
        }

        private void UpdateTime(TimeSpan obj)
        {
            unlockTimeText.text = $"{obj.Hours}H {obj.Minutes}M";
            CheckTimeDifferenceAndUpdateCost(obj);
        }

        private void OnDisable()
        {
            GameData.OnPlayerDataUpdate -= updateCard;
            gameTimer.onTimerUpdate -= UpdateTime;
            gameTimer.onTimerComplete -= OnTimerComplete;
        }
        public void CardHidden()
        {
            gameTimer.StopTimer();
        }

        public async void OnUnlockButtonClicked()
        {
            if (cost <= 0)
            {
                var _result = await ChestAPIs.UnlockChest_API(chestData._id);
                if (!_result.error)
                {
                    PopUpManager.Instance.ShowPopup(PopUpManager.Instance.ChestUnlockedPopup);
                    PopUpManager.Instance.ChestUnlockedPopup.Init(chestType, _result.result);
                    gameTimer.StopTimer();
                    GameData.Invoke_OnPlayerDataUpdate();
                }
                else
                {
                    OverlayWarningPopup.Instance.ShowWarning("Something Went Wrong!");
                }
            }
            else
            {
                PopUpManager.Instance.ShowPopup(PopUpManager.Instance.ChestUnlockConfirmationPopup);
                PopUpManager.Instance.ChestUnlockConfirmationPopup.Init(chestType, cost, chestData._id, gameTimer);
            }
        }
        public async void OnBuyNowButtonClicked()
        {
            buyNowButton.interactable = false;

            var succesful = await ChestAPIs.BuyChest_API(chestType);
            if (succesful)
            {
                GameData.Instance.DeductCost(Enums.CostType.POINTS, cost);
                ShopScreen.Instance.GetUserChests();
                PopUpManager.Instance.ShowPopup(PopUpManager.Instance.ChestPurchasedPopup);
                GameData.Invoke_OnPlayerDataUpdate();
            }
            else
            {
                //show warning
                OverlayWarningPopup.Instance.ShowWarning("Something went wrong!");
                buyNowButton.interactable = true;
            }
        }
        public void UpdateData(ModelClass.ChestData chestData)
        {
            SetChestData(chestData);
            UserHasChest();

        }
        public void SetChestData(ModelClass.ChestData chestData)
        {
            this.chestData = chestData;
        }

        public void updateCard()
        {
            chestData = null;
            GetCardCost(chestType);
            //Debug.Log($"{chestType} cost {cost}");
            if (chestData == null)
            {
                CheckUserCanBuy();
            }
        }
        public void CheckUserCanBuy()
        {

            if (cost <= GameData.Instance.PlayerData.Data.chips)
            {
                UserCanBuyTheChest();
            }
            else
            {
                UserCannotBuyTheChest();
            }
        }
        public void UserCanBuyTheChest()
        {
            unlockNowButton.gameObject.SetActive(false);
            insufficientCostImage.gameObject.SetActive(false);
            buyNowButton.gameObject.SetActive(true);
            buyNowButton.interactable = true;
            unlockTimer.SetActive(false);
            costGameobject.SetActive(true);
        }
        public void UserCannotBuyTheChest()
        {
            unlockNowButton.gameObject.SetActive(false);
            buyNowButton.gameObject.SetActive(true);
            buyNowButton.interactable = false;
            insufficientCostImage.gameObject.SetActive(true);
            unlockTimer.SetActive(false);
            costGameobject.SetActive(true);
        }
        public void UserHasChest()
        {
            unlockNowButton.gameObject.SetActive(true);
            unlockNowButton.interactable = true;
            costGameobject.SetActive(true);
            CheckAndUpdateTime();
            buyNowButton.gameObject.SetActive(false);
            insufficientCostImage.gameObject.SetActive(false);
        }
        public void CheckAndUpdateTime()
        {
            TimeSpan timeDiff = HelperFunctions.UnixTimeStampToDateTime(chestData.unlockTime) - DateTime.Now;
            Debug.Log(HelperFunctions.UnixTimeStampToDateTime(chestData.unlockTime) + " " + DateTime.Now + " " + timeDiff.ToString());
            if (timeDiff.TotalSeconds <= 0)
            {
                unlockTimer.SetActive(false);
                cost = 0;
                costText.text = cost.ToString();
            }
            else
            {
                CheckTimeDifferenceAndUpdateCost(timeDiff);
                unlockTimer.SetActive(true);
                gameTimer.StartTimerInMinutes((int)timeDiff.TotalMinutes);
                unlockTimeText.text = $"{timeDiff.Hours}H {timeDiff.Minutes}M";
            }
        }

        private void CheckTimeDifferenceAndUpdateCost(TimeSpan timeDiff)
        {
            if (timeDiff.TotalMinutes <= 30)
            {
                unlockNowButton.interactable = false;
                costGameobject.SetActive(false);
                cost = 0;

            }
            else
            {
                cost = (int)(timeDiff.TotalMinutes * 0.9f);
                if (cost > GameData.Instance.PlayerData.Data.chips)
                {
                    unlockNowButton.interactable = false;
                }
                else
                {
                    unlockNowButton.interactable = true;
                }
                costText.text = cost.ToString();
            }
        }
        public void GetCardCost(Enums.ChestType chestType)
        {
            switch (chestType)
            {
                case Enums.ChestType.SNACK:
                    cost = GameData.Instance.GameSettings.chestSnackCost;
                    costText.text = cost.ToString();
                    break;
                case Enums.ChestType.TREAT:
                    cost = GameData.Instance.GameSettings.chestTreatCost;
                    costText.text = cost.ToString();
                    break;
                case Enums.ChestType.FEAST:
                    cost = GameData.Instance.GameSettings.chestFeastCost;
                    costText.text = cost.ToString();
                    break;
            }
        }
        public void StopTimer()
        {
            gameTimer.StopTimer();
        }
    }
}
