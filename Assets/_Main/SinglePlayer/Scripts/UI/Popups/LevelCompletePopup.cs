using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    public class LevelCompletePopup : PopupBehaviour
    {
        [SerializeField] private Image starFill, timeFill, healthFill;
        [SerializeField] private TextMeshProUGUI levelTMP;
        [SerializeField] private TextMeshProUGUI ordersTMP;
        [SerializeField] private TextMeshProUGUI nftOrdersTMP;
        [SerializeField] private TextMeshProUGUI chipsTMP;
        [SerializeField] private TextMeshProUGUI munchesTMP;
        [SerializeField] private GameObject nextBtn, replayBtn;
        [SerializeField] private GameObject bonusObj;


        void Awake() => Init(GetComponent<CanvasGroup>());

        public override void Init()
        {
            base.Init();

            nextBtn.SetActive(false);
            replayBtn.SetActive(false);
            MenuBar.Instance.Show();

            GameController.IsGamePaused = true;

            levelTMP.text = $"Level {GameData.Instance.SelectedLevelNumber} Complete";
            chipsTMP.text = /*"+" +*/ GameController.Instance.LevelInfo.Chips.ToString();
            munchesTMP.text = /*"+" +*/ GameController.Instance.LevelInfo.Munches.ToString();
            ordersTMP.text = GameController.Instance.LevelInfo.TotalDeliveries.ToString();
            nftOrdersTMP.text = GameController.Instance.LevelInfo.NFTDeliveries.ToString();
            starFill.fillAmount = GameController.Instance.LevelInfo.StarValue;
            timeFill.fillAmount = GameController.Instance.LevelInfo.TimeBonus;
            healthFill.fillAmount = GameController.Instance.LevelInfo.HealthBonus;

            //gotNftObj.SetActive(GameController.Instance.LevelInfo.NFTDeliveries > 0);
            //noNftObj.SetActive(GameController.Instance.LevelInfo.NFTDeliveries == 0);

            TweenHandler.FloatValue(0, GameController.Instance.LevelInfo.StarValue, 1, ScreenManager.Instance.TweenEase,
            _value => starFill.fillAmount = _value, () =>
            {
                nextBtn.SetActive(GameData.Instance.SelectedLevelNumber < GameData.Instance.LevelData.levels.Count);
                replayBtn.SetActive(true);
            });

            GameplayScreen.Instance.ToggleUI(false);
            AudioManager.Instance.PlayLevelSuccess();
        }

        [ContextMenu("Debug_PlayStarAnim")]
        private void Debug_PlayStarAnim()
        {
            TweenHandler.FloatValue(0, GameController.Instance.LevelInfo.StarValue, 1 + GameController.Instance.LevelInfo.StarValue, DG.Tweening.Ease.InOutSine,
            _value => starFill.fillAmount = _value, () =>
            {
                nextBtn.SetActive(true);
                replayBtn.SetActive(true);
            });
        }

        //public void OnClick_BuyNFT() => Application.OpenURL(buyNftUrl);

        public void OnClick_Next()
        {
            AudioManager.Instance.StopSFX();
            //foreach (var animator in animators) animator.Stop();

            Hide();
            GameController.Instance.SetNextLevel(false);
        }

        public void OnClick_PlayAgain()
        {
            AudioManager.Instance.StopSFX();
            //foreach (var animator in animators) animator.Stop();

            Hide();
            MenuBar.Instance.Hide();
            GameplayScreen.Instance.ToggleUI(true);
            GameController.Instance.SetNextLevel(true);
        }


        public void OnClick_Exit()
        {
            Hide();
            GameController.IsGamePaused = false;
            GameData.Instance.EndGame();
        }
    }
}
