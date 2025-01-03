using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    public class MenuBar : MonoBehaviour
    {
        [SerializeField] private FuelMeter fuelMeter;
        [SerializeField] private FuelTimer fuelTimer;

        [Header("-----Buttons")]
        [SerializeField] private GameObject backBtn;
        [SerializeField] private Button rankIconBtn;

        [Header("-----Profile")]
        [SerializeField] private RawImage profilePic;
        [SerializeField] private TextMeshProUGUI usernameTMP, munchTMP, chipsTMP, rankTMP;

        private Button backButton;
        private CanvasGroup canvasGroup;

        public static MenuBar Instance { get; private set; }
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                canvasGroup = GetComponent<CanvasGroup>();
                backButton = backBtn.GetComponent<Button>();
            }
        }

        private void OnEnable() => GameData.OnPlayerDataUpdate += OnPlayerDataUpdate;
        private void OnDisable() => GameData.OnPlayerDataUpdate -= OnPlayerDataUpdate;

        private void OnPlayerDataUpdate()
        {
            usernameTMP.text = GameData.Instance.PlayerData.Data.username.ToString();
            munchTMP.text = GameData.Instance.PlayerData.Data.munches > 10000 ? HelperFunctions.FormatNumber(GameData.Instance.PlayerData.Data.munches) : GameData.Instance.PlayerData.Data.munches.ToString();
            chipsTMP.text = GameData.Instance.PlayerData.Data.chips > 10000 ? HelperFunctions.FormatNumber(GameData.Instance.PlayerData.Data.chips) : GameData.Instance.PlayerData.Data.chips.ToString();
            rankTMP.text = GameData.Instance.PlayerData.Data.rank > 1000 ? "Rank " + HelperFunctions.FormatNumber(GameData.Instance.PlayerData.Data.rank) : "Rank " + GameData.Instance.PlayerData.Data.rank;
            profilePic.texture = GameData.ProfilePic;

            fuelMeter.UpdateMeterData();
        }

        //public void OnClick_Settings() => PopUpManager.Instance.ShowPopup(PopUpManager.Instance.SettingsPopup);

        //public void OnClick_Leaderboard() => PopUpManager.Instance.ShowPopup(PopUpManager.Instance.LeaderboardPopup);

        //public void OnClick_Invite() => PopUpManager.Instance.ShowPopup(PopUpManager.Instance.InvitePopup);


        [ContextMenu("Back")]
        public void OnClick_Back()
        {
            BlockRaycasts(false);
            switch (ScreenManager.Instance.CurrentScreen)
            {
                case Enums.Screen.Home:
                    ScreenManager.Instance.SwitchScreen(HomeScreen.Instance, LevelScreen.Instance);
                    break;

                case Enums.Screen.Garage:
                    ScreenManager.Instance.SwitchScreen(HomeScreen.Instance, GarageScreen.Instance);
                    break;

                case Enums.Screen.MapSelection:
                    ScreenManager.Instance.SwitchScreen(HomeScreen.Instance, LevelScreen.Instance);
                    break;

                case Enums.Screen.Shop:
                    ScreenManager.Instance.SwitchScreen(HomeScreen.Instance, ShopScreen.Instance);
                    break;


                case Enums.Screen.Profile:
                    ScreenManager.Instance.SwitchScreen(HomeScreen.Instance, ProfileScreen.Instance);
                    break;

                case Enums.Screen.PreGameplay:
                case Enums.Screen.Gameplay:
                    GameController.IsGamePaused = true;
                    PopUpManager.Instance.ReturnToMainMenuPopup.ShowPopup(true, () =>
                    {
                        GameData.Instance.EndGame();
                        Hide();
                    });
                    break;
            }
        }

        public void OnClick_Quit()
        {
            Debug.Log("Unity_Quit");
            if (Application.isEditor) return;

            Application.Quit();
        }

        public void ToggleBackButton(bool _flag) => backButton.interactable = _flag;
        // False means Disable input events 
        public void BlockRaycasts(bool _flag)
        {
            if (canvasGroup.alpha > 0) canvasGroup.blocksRaycasts = _flag;
        }



        public void Init()
        {
            ToggleBackButton(true);
            OnPlayerDataUpdate();
            backBtn.SetActive(ScreenManager.Instance.CurrentScreen != Enums.Screen.Home && ScreenManager.Instance.CurrentScreen != Enums.Screen.Invite);

        }

        public void Show(Action _callback = null)
        {
            Init();
            if (ScreenManager.Instance.CurrentScreen == Enums.Screen.Home)
            {
                fuelTimer.CheckIfTimerNeedToBeStoppedOrNot();
            }
            TweenHandler.CanvasGroupAlpha(canvasGroup, 1, ScreenManager.Instance.TweenDuration, ScreenManager.Instance.TweenEase, () =>
            {
                canvasGroup.blocksRaycasts = true;
                _callback?.Invoke();

            });
        }

        public void Hide(Action _callback = null)
        {
            canvasGroup.blocksRaycasts = false;
            TweenHandler.CanvasGroupAlpha(canvasGroup, 0, ScreenManager.Instance.TweenDuration, ScreenManager.Instance.TweenEase, () =>
            {
                _callback?.Invoke();
            });
        }

    }

}