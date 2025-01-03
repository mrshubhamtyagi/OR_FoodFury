using System;
using System.Threading.Tasks;
using OneRare.FoodFury.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    public class WarningPopup : PopupBehaviour
    {
        public TMPro.TextMeshProUGUI warningTMP;

        [SerializeField] private Button logoutButton, retryButton, goHomeButton;

        public static WarningPopup Instance;
        void Awake()
        {
            if (Instance == null)
            {
                Init(GetComponent<CanvasGroup>());
                Instance = this;
                logoutButton.onClick.AddListener(OnClick_Logout);
                goHomeButton.onClick.AddListener(OnClick_GoToMainMenu);
                retryButton.onClick.AddListener(OnClick_Retry);
            }
        }

        Action OnRetryCallback;


        public void SetWarning(string _warning)
        {
            print($"[WARNING] {_warning}");
            warningTMP.text = _warning;
        }
        public void ShowWarning(string _warning)
        {
            print($"[WARNING] {_warning}");
            ShowHomeButton();
            warningTMP.text = _warning;
            Show();

        }
        public void ShowWarningWithRetryAndLogout(string _warning, Action _onRetry)
        {
            print($"[WARNING] {_warning}");
            warningTMP.text = _warning;
            OnRetryCallback = _onRetry;
            ShowRetryAndLogoutButton();
            Show();
        }
        public void ShowWarningWithRetry(string _warning, Action _onRetry)
        {
            print($"[WARNING] {_warning}");
            warningTMP.text = _warning;
            OnRetryCallback = _onRetry;

            ShowRetryButton();
            Show();
        }
        public void ShowWarningWithLogout(string _warning)
        {
            print($"[WARNING] {_warning}");
            warningTMP.text = _warning;
            ShowLogoutButton();
            Show();
        }
        public void ToggleButton(Button btn, bool value) => btn.gameObject.SetActive(value);
        public void ShowRetryAndLogoutButton()
        {
            ToggleButton(goHomeButton, false);
            ToggleButton(logoutButton, true);
            ToggleButton(retryButton, true);
        }
        public void ShowHomeButton()
        {
            ToggleButton(goHomeButton, true);
            ToggleButton(logoutButton, false);
            ToggleButton(retryButton, false);
        }
        public void ShowRetryButton()
        {
            ToggleButton(goHomeButton, false);
            ToggleButton(logoutButton, false);
            ToggleButton(retryButton, true);
        }
        public void ShowLogoutButton()
        {
            ToggleButton(goHomeButton, false);
            ToggleButton(logoutButton, true);
            ToggleButton(retryButton, false);
        }
        public override void Init()
        {
            base.Init();
            AudioManager.Instance.PlayWarningSound();
        }
        private MobileInput mobileInput;
        private GameManager gameManager;
        public void OnClick_GoToMainMenu()
        {
            PopUpManager.Instance.HideAllPopUps();
            if (GameData.Instance.gameMode == Enums.GameModeType.SinglePlayer)
            {
                if (GameController.Instance) GameData.Instance.EndGame();
                else ScreenManager.Instance.HideAllScreensExcept(HomeScreen.Instance);
            }
            else
            {
                gameManager = FindObjectOfType<GameManager>();
                gameManager.ShutdownOnInput();
                /*mobileInput = FindObjectOfType<MobileInput>();
                mobileInput.OnDisconnect();*/
            }
        }



        public async void OnClick_Retry()
        {
            Loader.Instance.ShowLoader();
            Hide();
            await Task.Delay(1000);
            Loader.Instance.HideLoader();
            OnRetryCallback?.Invoke();
            OnRetryCallback = null;
        }
        public void OnClick_Logout()
        {
            if (GameData.Instance.Platform != Enums.Platform.WebGl)
            {
                Loader.Instance.ShowLoader();
                PlayerPrefsManager.Instance.RemovePlayerId();
                PlayerPrefsManager.Instance.RemoveVia();
                GameData.Instance.ResetPlayerData();
                AnalyticsManager.Instance.FireLogoutEvent();
                Hide();
                SceneManager.SwitchToLoginScene();
            }
            else
            {
#if UNITY_WEBGL
   Hide();
                JavaScriptCallbacks.Quit();
#endif
            }
        }
        private void OnDestroy()
        {
            logoutButton.onClick.RemoveAllListeners();
            goHomeButton.onClick.RemoveAllListeners();
            retryButton.onClick.RemoveAllListeners();
        }
    }

}