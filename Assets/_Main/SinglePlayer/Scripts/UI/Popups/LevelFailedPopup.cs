using TMPro;
using UnityEngine;

namespace FoodFury
{
    public class LevelFailedPopup : PopupBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelTMP;
        [SerializeField] private TextMeshProUGUI ordersTMP;
        [SerializeField] private TextMeshProUGUI nftOrdersTMP;
        [SerializeField] private TextMeshProUGUI chipsTMP;
        [SerializeField] private TextMeshProUGUI munchesTMP;

        void Awake() => Init(GetComponent<CanvasGroup>());

        public override void Init()
        {
            base.Init();

            GameController.IsGamePaused = true;
            MenuBar.Instance.Show();

            levelTMP.text = $"Level {GameData.Instance.SelectedLevelNumber} Failed";
            chipsTMP.text = "+" + GameController.Instance.LevelInfo.Chips.ToString();
            munchesTMP.text = "+00";
            ordersTMP.text = GameController.Instance.LevelInfo.TotalDeliveries.ToString();
            nftOrdersTMP.text = GameController.Instance.LevelInfo.NFTDeliveries.ToString();

            GameplayScreen.Instance.ToggleUI(false);
            AudioManager.Instance.PlayLevelFailed();
        }


        public void OnClick_TryAgain()
        {
            Hide();
            MenuBar.Instance.Hide();
            GameplayScreen.Instance.ToggleUI(true);
            GameController.Instance.SetNextLevel(true);
        }


        public void OnClick_Exit()
        {
            //MenuBar.Instance.Hide();
            GameController.IsGamePaused = true;
            PopUpManager.Instance.ShowPopup(PopUpManager.Instance.ReturnToMainMenuPopup);

            //Hide();
            //ScreenManager.Instance.SwitchScreen(HomeScreen.Instance, GameplayScreen.Instance);
            //GameData.Instance.EndGame();
        }
    }
}
