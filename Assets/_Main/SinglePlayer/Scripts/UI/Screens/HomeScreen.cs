using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    public class HomeScreen : MonoBehaviour, IScreen
    {
        [SerializeField] private Button singlePlayerBtn;
        [SerializeField] private FuelMeter fuelMeter;
        [SerializeField] private GameObject setup;
        [SerializeField] private Transform vehicleModels;
        [SerializeField] private GameObject referalBtn;
        [SerializeField] private GameObject profileBtn;
        [SerializeField] private GameObject telegramConnectBtn;

        private OrbitManager orbitManager;
        private CanvasGroup canvasGroup;

        public static HomeScreen Instance { get; private set; }
        void Awake()
        {
            if (Instance != null) return;
            Instance = this;
            canvasGroup = GetComponent<CanvasGroup>();
            orbitManager = vehicleModels.GetComponent<OrbitManager>();
        }

        public void OnClick_Garage()
        {
            AudioManager.Instance.PlayGarageButtonSound();
            ScreenManager.Instance.SwitchScreen(GarageScreen.Instance, this);
        }

        public void OnClick_Help() => ScreenManager.Instance.ShowTutorials(); //PopUpManager.Instance.ShowPopup(PopUpManager.Instance.HelpPopup);
        public void OnClick_Settings() => PopUpManager.Instance.ShowPopup(PopUpManager.Instance.SettingsPopup);
        public void OnClick_Leaderboard() => PopUpManager.Instance.ShowPopup(PopUpManager.Instance.LeaderboardPopup);
        public void OnClick_Feedback() => PopUpManager.Instance.ShowPopup(PopUpManager.Instance.FeedbackPopup);


        public void OnClick_SinglePlayer() => ClickSinglePlayerButton();
        public void OnClick_MultiPlayer() => ClickMultiPlayerButton();
        public void OnClick_TeamRumble() => ScreenManager.Instance.SwitchScreen(LobbyScreen.Instance, this);


        public void OnClick_Shop() => ScreenManager.Instance.SwitchScreen(ShopScreen.Instance, this);
        public void OnClick_Profile() => ScreenManager.Instance.SwitchScreen(ProfileScreen.Instance, this);
        public void OnClick_Referral() => PopUpManager.Instance.ShowPopup(PopUpManager.Instance.ReferalPopup);
        public void OnClick_Airdrop() => PopUpManager.Instance.ShowPopup(PopUpManager.Instance.TelegramConnectPopup);


        public async void Init()
        {
            AudioManager.Instance.PlayHomeScreenTrack();
            var _response = await PlayerAPIs.GetPlayerData_API(GameData.Instance.PlayerId);

            // Logout if Manually Disconnect from TELEGRAM - Show warning and RETURN
            //WarningPopup.Instance.SHow("asdasdasdas", () => ScreenManager.Instance.SwitchScreen(Instance, null));

            if (_response)
            {
                fuelMeter.UpdateMeterData();
                if (GameData.Instance.IsTelegramDisconnected())
                {
                    WarningPopup.Instance.ShowWarningWithLogout("Telegram Disconnected!!!.please Logout and Login Again");
                }
                telegramConnectBtn.SetActive(!GameData.Instance.IsTelegramConnected());
                //  profileBtn.SetActive(GameData.Instance.UserDishData.Data.Count > 0);
                if (!GameData.Instance.PlayerData.Data.isLicenseComplete)
                    ScreenManager.Instance.SwitchScreen(InviteScreen.Instance, this);
                else
                {
                    foreach (Transform item in vehicleModels) item.gameObject.SetActive(false);

                    vehicleModels.GetChild(GameData.Instance.PlayerData.Data.currentVehicle - 1).gameObject.SetActive(true);
                    orbitManager.ResetRotation();
                    orbitManager.autoRotate = true;
                    setup.SetActive(true);
                }
            }
            else
            {
                WarningPopup.Instance.ShowWarningWithRetryAndLogout("Could not Fetch Player Data", () => ScreenManager.Instance.SwitchScreen(HomeScreen.Instance, null));
                return;
                // Show warning and RETURN
                //ScreenManager.Instance.SwitchScreen(HomeScreen.Instance, null);
            }
            //  referalBtn.SetActive(GameData.Instance.Platform == Enums.Platform.WebGl);
        }

        public async void Show(Action _callback = null)
        {
            (Enums.QueryResult result, bool maintenanceMode) maintenanceResult = await GameData.Instance.IsInMaintenenceMode();
            if (maintenanceResult.result == Enums.QueryResult.Error)
            {
                WarningPopup.Instance.ShowWarningWithRetry("Could not fetch Data", () => { ScreenManager.Instance.SwitchScreen(HomeScreen.Instance, null); });
                return;
            }
            else if (maintenanceResult.result == Enums.QueryResult.Success && maintenanceResult.maintenanceMode == true) return;
            Init();

            ScreenManager.Instance.CurrentScreen = Enums.Screen.Home;
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
            Loader.Instance.ShowLoader();
            setup.SetActive(false);
            canvasGroup.blocksRaycasts = false;
            TweenHandler.CanvasGroupAlpha(canvasGroup, 0, ScreenManager.Instance.TweenDuration, ScreenManager.Instance.TweenEase, () =>
            {
                _callback?.Invoke();
            });
        }

        public void ClickMultiPlayerButton()
        {
            ScreenManager.Instance.LoadMultiplayerScene();//ScreenManager.Instance.SwitchScreen(LobbyScreen.Instance, this);
            GameData.Instance.gameMode = Enums.GameModeType.Multiplayer;
        }

        void ClickSinglePlayerButton()
        {
            ScreenManager.Instance.SwitchScreen(LevelScreen.Instance, this);
            GameData.Instance.gameMode = Enums.GameModeType.SinglePlayer;
        }
    }
}
