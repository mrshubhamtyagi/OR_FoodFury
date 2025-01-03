using System;
using UnityEngine;

namespace FoodFury
{
    public class PopUpManager : MonoBehaviour
    {
        // ------------------- HOME
        [field: SerializeField] public SettingsPopup SettingsPopup { get; private set; }
        [field: SerializeField] public LeaderboardPopup LeaderboardPopup { get; private set; }
        [field: SerializeField] public FuelPopup FuelPopup { get; private set; }
        [field: SerializeField] public InvitePopup InvitePopup { get; private set; }
        [field: SerializeField] public FeedbackPopup FeedbackPopup { get; private set; }
        [field: SerializeField] public ReturnToMainMenuPopup ReturnToMainMenuPopup { get; private set; }
        [field: SerializeField] public ReferalPopup ReferalPopup { get; private set; }
        [field: SerializeField] public RewardClaimedPopup RewardClaimedPopup { get; private set; }
        [field: SerializeField] public TelegramConnectPopup TelegramConnectPopup { get; private set; }


        // ------------------- GAMEPLAY
        [field: SerializeField] public GameStartPopup GameStartPopup { get; private set; }
        [field: SerializeField] public GameEndPopup GameEndPopup { get; private set; }
        [field: SerializeField] public LevelCompletePopup LevelCompletePopup { get; private set; }
        [field: SerializeField] public LevelFailedPopup LevelFailedPopup { get; private set; }
        [field: SerializeField] public ExitGamePopup ExitGamePopup { get; private set; }


        // ------------------- GARAGE
        [field: SerializeField] public GarageGetItNowPopup GetItNowPopup { get; private set; }
        [field: SerializeField] public GarageUseUnlockCodePopup UseUnlockCodePopup { get; private set; }


        // ------------------- SHOP
        [field: SerializeField] public ChestPurchasedPopup ChestPurchasedPopup { get; private set; }
        [field: SerializeField] public ChestUnlockedPopup ChestUnlockedPopup { get; private set; }
        [field: SerializeField] public ChestUnlockConfirmationPopup ChestUnlockConfirmationPopup { get; private set; }



        public static PopUpManager Instance { get; private set; }
        void Awake() => Instance = this;

        public void HideAllPopUps()
        {
            SettingsPopup.Hide();
            LeaderboardPopup.Hide();
            FuelPopup.Hide();
            InvitePopup.Hide();
            FeedbackPopup.Hide();
            ReferalPopup.Hide();
            GameStartPopup.Hide();
            GameEndPopup.Hide();
            LevelCompletePopup.Hide();
            LevelFailedPopup.Hide();
            ReturnToMainMenuPopup.Hide();
            ExitGamePopup.Hide();
            WarningPopup.Instance.Hide();
            ChestPurchasedPopup.Hide();
            ChestPurchasedPopup.Hide();
            ChestUnlockConfirmationPopup.Hide();
            OverlayWarningPopup.Instance.Hide();
            RewardClaimedPopup.Hide();
        }



        public void SwitchPopup(PopupBehaviour _toShow, PopupBehaviour _toHide, Action _onCompleted = null) => _toHide.Hide(() => _toShow.Show(_onCompleted));


        public void HidePopup(PopupBehaviour _toHide, Action _onCompleted = null)
        {
            _toHide.canvasGroup.blocksRaycasts = false;
            TweenHandler.CanvasGroupAlpha(_toHide.canvasGroup, 0, ScreenManager.Instance.TweenDuration, ScreenManager.Instance.TweenEase, _onCompleted);
        }

        public void ShowPopup(PopupBehaviour _toShow, PopupBehaviour _toHide = null, Action _onCompleted = null)
        {
            if (_toHide == null)
            {
                _toShow.Init();
                _toShow.Show(_onCompleted);
                //TweenHandler.CanvasGroupAlpha(_toShow.canvasGroup, 1, ScreenManager.Instance.TweenDuration, ScreenManager.Instance.TweenEase, () =>
                //{
                //    _toShow.canvasGroup.blocksRaycasts = true;
                //    _onCompleted?.Invoke();
                //});
            }
            else
            {
                _toHide.Hide(() =>
                {
                    _toShow.Init();
                    _toShow.Show(_onCompleted);
                });

                //_toHide.canvasGroup.blocksRaycasts = false;
                //TweenHandler.CanvasGroupAlpha(_toHide.canvasGroup, 0, ScreenManager.Instance.TweenDuration, ScreenManager.Instance.TweenEase, () =>
                //{
                //    _toShow.Init();
                //    TweenHandler.CanvasGroupAlpha(_toShow.canvasGroup, 1, ScreenManager.Instance.TweenDuration, ScreenManager.Instance.TweenEase, () =>
                //    {
                //        _toShow.canvasGroup.blocksRaycasts = true;
                //        _onCompleted?.Invoke();
                //    });
                //});
            }
        }




        public void ShowWarningPopup(string _warning)
        {
            WarningPopup.Instance.ShowWarning(_warning);
        }
    }
}
