using System;
using UnityEngine;
using SceneManagerUnity =   UnityEngine.SceneManagement.SceneManager;

namespace FoodFury
{
    public class ScreenManager : MonoBehaviour
    {
        [SerializeField] private LevelProgress levelProgress;
        public Enums.Screen CurrentScreen;

        [field: SerializeField] public float TweenDuration { get; private set; }
        [field: SerializeField] public DG.Tweening.Ease TweenEase { get; private set; }

        public static ScreenManager Instance { get; private set; }
        void Awake() => Instance = this;

        private void Start() => TweenEase = DG.Tweening.Ease.InOutExpo;

        public void SwitchScreen(IScreen _toShow, IScreen _toHide, Action _onCompleted = null)
        {
            if (_toHide == null)
            {
                AnalyticsManager.Instance.FireScreenChangeEvent("", _toShow.GetType().Name);
                _toShow.Show(_onCompleted);
            }
            else
            {
                AnalyticsManager.Instance.FireScreenChangeEvent(_toHide.GetType().Name, _toShow.GetType().Name);
                _toHide.Hide(() => _toShow.Show(_onCompleted));
            }
        }

        public void HideAllScreensExcept(IScreen _toshow)
        {
            HomeScreen.Instance.Hide();
            LevelScreen.Instance.Hide();
            LobbyScreen.Instance.Hide();
            GameplayScreen.Instance.Hide();
            ShopScreen.Instance.Hide();
            _toshow.Show();

        }


        public void ShowTutorials()
        {
            Loader.Instance.ShowLoader();
            SceneManager.SwitchToDrivingScene();
        }

        public void ShowLevelProgress() => levelProgress.PlayLevelAnimation();
        
        public void LoadMultiplayerScene()
        {
            //Loader.Instance.ShowLoader();
            SceneManagerUnity.LoadScene(2);
        }
    }
}