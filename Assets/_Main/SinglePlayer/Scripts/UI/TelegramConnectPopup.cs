using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    public class TelegramConnectPopup : PopupBehaviour
    {
        [SerializeField] private Button connectBtn;
        [SerializeField] private GameObject connectedBtn;
        [SerializeField] private TMP_InputField codeInputField;

        void Awake()
        {
            Init(GetComponent<CanvasGroup>());
            connectBtn.onClick.AddListener(OnClickConnectBtn);
        }

        public override void Init()
        {
            base.Init();
            MenuBar.Instance.BlockRaycasts(false);
            HandleInputs();
        }

        private async void OnClickConnectBtn()
        {
            print("OnClickConnectBtn");
            var isFieldEmpty = string.IsNullOrEmpty(codeInputField.text);
            if (isFieldEmpty)
            {
                OverlayWarningPopup.Instance.ShowWarning("No Code Entered");
                return;
            }

            Loader.Instance.ShowLoader();
            bool isConnected = await PlayerAPIs.VerifyCode_API(codeInputField.text);
            if (isConnected)
            {
                HandleInputs();
                AnalyticsManager.Instance.FireConnectedToTelegramEvent();
                GameData.Instance.AddMapsToPlayerLevels();
                GameData.Instance.HandleFuelRestorationAndLevelCompletion();
                Loader.Instance.HideLoader();

            }
            else
            {
                OverlayWarningPopup.Instance.ShowWarning("Could not connect! Please check your ID.");
                Loader.Instance.HideLoader();
            }

        }

        private void HandleInputs()
        {
            bool _isTGConnected = GameData.Instance.IsTelegramConnected();
            codeInputField.interactable = !_isTGConnected;
            connectBtn.gameObject.SetActive(!_isTGConnected);
            connectedBtn.gameObject.SetActive(_isTGConnected);
        }


        public override void Close()
        {
            MenuBar.Instance.BlockRaycasts(true);
            base.Close();
        }
    }
}
