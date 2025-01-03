using TMPro;
using UnityEngine;

namespace FoodFury
{
    public class GameEndPopup : PopupBehaviour
    {
        [SerializeField] private GameObject exiBtn;
        [SerializeField] private TextMeshProUGUI reasonTMP;

        void Awake() => Init(GetComponent<CanvasGroup>());

        public void UpdateReason(string _value) => reasonTMP.text = _value;


        public void OnClick_Replay()
        {
            Hide();
            GameplayScreen.Instance.ToggleUI(true);
            GameController.IsGamePaused = false;
            GameData.Instance.EndGame(true);
        }

        public void OnClick_BuyNow()
        {
            Hide();
            GameplayScreen.Instance.ToggleUI(true);
            GameController.IsGamePaused = false;
        }

        public void OnClick_Exit()
        {
            Hide();
            GameController.IsGamePaused = false;
            GameData.Instance.EndGame();
        }

        public override async void Init()
        {
            base.Init();
            exiBtn.SetActive(false);
            GameController.IsGamePaused = true;
            GameplayScreen.Instance.ToggleUI(false);
            var _result = await PlayerAPIs.UpdatePlayerDataFuel_API();
            exiBtn.SetActive(true);
        }


    }
}
