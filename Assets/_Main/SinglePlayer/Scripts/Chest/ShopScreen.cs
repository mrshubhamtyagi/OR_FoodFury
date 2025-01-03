using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;
using static FoodFury.ModelClass;

namespace FoodFury
{
    public class ShopScreen : MonoBehaviour, IScreen
    {
        private CanvasGroup canvasGroup;
        public static ShopScreen Instance { get; private set; }
        [SerializeField] private ChestCard snackCard;
        [SerializeField] private ChestCard treatCard;
        [SerializeField] private ChestCard feastCard;

        // Start is called before the first frame update
        void Awake()
        {
            Instance = this;
            canvasGroup = GetComponent<CanvasGroup>();

        }

        public void Init()
        {
            GetUserChests();

        }

        public async void GetUserChests()
        {
            var _result = await ChestAPIs.GetUserChests_API();
            if (_result.error)
            {
                PopUpManager.Instance.ShowWarningPopup("ERROR");
            }
            else
            {
                Debug.Log("NUmber of chest:" + _result.result.Count);
                UpdateChestCards(_result.result);
            }
        }

        private void UpdateChestCards(List<ChestData> chestList)
        {
            bool snackCardUpdated = false;
            bool treatCardUpdated = false;
            bool feastCardUpdated = false;

            foreach (var chest in chestList)
            {
                switch (chest.type)
                {
                    case ChestType.SNACK:
                        snackCard.UpdateData(chest);
                        snackCardUpdated = true;
                        break;
                    case ChestType.TREAT:
                        treatCard.UpdateData(chest);
                        treatCardUpdated = true;
                        break;
                    case ChestType.FEAST:
                        feastCard.UpdateData(chest);
                        feastCardUpdated = true;
                        break;
                    default:
                        Debug.LogWarning("Unknown chest type: " + chest.type);
                        break;
                }
            }
            if (!snackCardUpdated)
            {
                snackCard.updateCard();
            }
            if (!treatCardUpdated)
            {
                treatCard.updateCard();
            }
            if (!feastCardUpdated)
            {
                feastCard.updateCard();
            }

        }
        public void Show(Action _callback = null)
        {
            Init();

            ScreenManager.Instance.CurrentScreen = Enums.Screen.Shop;
            MenuBar.Instance.Show();

            TweenHandler.CanvasGroupAlpha(canvasGroup, 1, ScreenManager.Instance.TweenDuration, ScreenManager.Instance.TweenEase, () =>
            {
                if (GameData.Instance.PlayerData.Data.isLicenseComplete)
                {
                    canvasGroup.blocksRaycasts = true;
                    _callback?.Invoke();
                    Loader.Instance.HideLoader();
                }
            });
        }

        public void Hide(Action _callback = null)
        {
            snackCard.StopTimer();
            treatCard.StopTimer();
            feastCard.StopTimer();
            Loader.Instance.ShowLoader();
            canvasGroup.blocksRaycasts = false;
            TweenHandler.CanvasGroupAlpha(canvasGroup, 0, ScreenManager.Instance.TweenDuration, ScreenManager.Instance.TweenEase, () =>
            {
                _callback?.Invoke();
            });
        }
    }
}
