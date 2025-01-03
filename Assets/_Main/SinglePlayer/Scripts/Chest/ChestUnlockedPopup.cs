using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace FoodFury
{
    public class ChestUnlockedPopup : PopupBehaviour
    {
        // Start is called before the first frame update
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private string snackText;
        [SerializeField] private string treatText;
        [SerializeField] private string feastText;

        [SerializeField] private GameObject innerStars;
        [SerializeField] private GameObject chestImageHolder;
        [SerializeField] private GameObject snackImage;
        [SerializeField] private GameObject treatImage;
        [SerializeField] private GameObject feastImage;
        [SerializeField] private MunchOnlyPrize munchOnly;
        [SerializeField] private MunchWithCards munchWithDishes;
        [SerializeField] private GameObject closeBtn;
        private ModelClass.ChestBuyResponse.Result chestResult;
        private bool hasShownReward;

        private void Awake()
        {
            Init(GetComponent<CanvasGroup>());
        }
        public void Init(Enums.ChestType chestType, ModelClass.ChestBuyResponse.Result result)
        {
            base.Init();
            chestResult = result;
            hasShownReward = false;
            innerStars.gameObject.SetActive(true);
            snackImage.gameObject.SetActive(false);
            treatImage.gameObject.SetActive(false);
            feastImage.gameObject.SetActive(false);
            munchOnly.gameObject.SetActive(false);
            munchWithDishes.gameObject.SetActive(false);
            closeBtn.SetActive(false);
            chestImageHolder.gameObject.SetActive(true);
            switch (chestType)
            {
                case Enums.ChestType.SNACK:
                    snackImage.gameObject.SetActive(true);
                    titleText.text = snackText;
                    break;
                case Enums.ChestType.TREAT:
                    treatImage.gameObject.SetActive(true);
                    titleText.text = treatText;
                    break;
                case Enums.ChestType.FEAST:
                    feastImage.gameObject.SetActive(true);
                    titleText.text = feastText;
                    break;

            }
            if (MenuBar.Instance) MenuBar.Instance.BlockRaycasts(false);
            Invoke("ShowPrize", 2f);

        }


        private void ShowMunchOnlyPrizes()
        {
            GameData.Instance.PlayerData.Data.munches += chestResult.munch;
            GameData.Invoke_OnPlayerDataUpdate();
            munchWithDishes.gameObject.SetActive(false);
            munchOnly.gameObject.SetActive(true);
            munchOnly.SetData(chestResult.munch);
        }
        private void ShowDishWithMunchPrize()
        {
            GameData.Instance.PlayerData.Data.munches += chestResult.munch;
            GameData.Invoke_OnPlayerDataUpdate();
            munchWithDishes.gameObject.SetActive(true);
            munchOnly.gameObject.SetActive(false);
            munchWithDishes.SetData(chestResult.dishes.ToList(), chestResult.munch);
        }
        public void ShowPrize()
        {
            innerStars.gameObject.SetActive(false);
            chestImageHolder.SetActive(false);
            closeBtn.SetActive(true);
            Debug.Log($"Chest length {chestResult.dishes.Length}");
            if (chestResult.dishes != null && chestResult.dishes.Length > 0)
            {
                ShowDishWithMunchPrize();
            }
            else
            {
                ShowMunchOnlyPrizes();
            }
        }
        public override void Close()
        {

            ShopScreen.Instance.GetUserChests();
            if (MenuBar.Instance) MenuBar.Instance.BlockRaycasts(true);
            base.Close();


        }
    }
}
