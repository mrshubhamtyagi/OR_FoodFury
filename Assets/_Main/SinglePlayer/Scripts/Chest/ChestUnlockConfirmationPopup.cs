using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Enums;
using static FoodFury.ModelClass;

namespace FoodFury
{
    public class ChestUnlockConfirmationPopup : PopupBehaviour
    {
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private TextMeshProUGUI confirmationText;
        [SerializeField] private string snackConfirmationText;
        [SerializeField] private string treatConfirmationText;
        [SerializeField] private string feastConfirmationText;
        private int costToUnlock;
        private string chestId;
        private Enums.ChestType chestType;
        private GameTimer gameTimer;
        // Start is called before the first frame update

        private void Awake() => Init(GetComponent<CanvasGroup>());

        public void Init(ChestType chestType, int cost, string chestId, GameTimer timer)
        {
            base.Init();
            costToUnlock = cost;
            this.chestId = chestId;
            this.chestType = chestType;
            this.gameTimer = timer;
            costText.text = costToUnlock.ToString();
            switch (chestType)
            {
                case Enums.ChestType.SNACK:
                    confirmationText.text = snackConfirmationText;
                    break;
                case Enums.ChestType.TREAT:
                    confirmationText.text = treatConfirmationText;
                    break;
                case Enums.ChestType.FEAST:
                    confirmationText.text = feastConfirmationText;
                    break;
            }
            if (MenuBar.Instance) MenuBar.Instance.BlockRaycasts(false);
        }

        public async void OnConfirmButtonClicked()
        {
            var _result = await ChestAPIs.UnlockChest_API(chestId, costToUnlock);
            if (!_result.error)
            {
                //if (_result.result.dishes != null && _result.result.dishes.Length > 0)
                //{
                //    var succesful = await DishAPIs.GetUserDishes_API();
                //    if (succesful)
                //    {
                //        List<Dish> newDishes = new();
                //        foreach (Dish dish in _result.result.dishes)
                //        {
                //            newDishes.Add(GameData.Instance.GetUserDishByToken(dish.tokenId));
                //        }
                //        _result.result.dishes = newDishes.ToArray();
                //    }
                //}
                PopUpManager.Instance.ShowPopup(PopUpManager.Instance.ChestUnlockedPopup, this);
                PopUpManager.Instance.ChestUnlockedPopup.Init(chestType, _result.result);
                gameTimer.StopTimer();
                GameData.Instance.DeductCost(Enums.CostType.POINTS, costToUnlock);
                GameData.Invoke_OnPlayerDataUpdate();
            }
            else
            {
                OverlayWarningPopup.Instance.ShowWarning("Something went wrong!");
            }
        }

        public override void Close()
        {
            if (MenuBar.Instance) MenuBar.Instance.BlockRaycasts(true);
            base.Close();
        }
    }
}
