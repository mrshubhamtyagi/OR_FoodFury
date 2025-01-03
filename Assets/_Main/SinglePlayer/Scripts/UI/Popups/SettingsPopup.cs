using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_IOS
using Assets.SimpleSignIn.Telegram.Scripts;
#endif

namespace FoodFury
{
    public class SettingsPopup : PopupBehaviour
    {
        [Header("-----Graphics")] [SerializeField]
        private RectTransform highlight;

        [SerializeField] private TextMeshProUGUI lowTMP, midTMP, highTMP, logOutButtonTxt, hapticsStatusTMP;
        [SerializeField] private Image lowImage, midImage, highImage;
        [SerializeField] private Slider soundSlider, musicSlider, hapticsSlider;
        [SerializeField] private Toggle hapticsToggle;
        //[SerializeField] private Vector3 hightlightYPosition;


        [Header("-----Sound")] [SerializeField]
        private int soundVolume;
        // private Transform soundBar;
        // [SerializeField] private Button soundMinusBtn;
        // [SerializeField] private Button soundPlusBtn;
        // private List<Image> soundBarChilds = new List<Image>();


        [Header("-----Music")] [SerializeField]
        private int musicVolume;
        // private Transform musicBar;
        // [SerializeField] private Button musicMinusBtn;
        // [SerializeField] private Button musicPlusBtn;
        // private List<Image> musicBarChilds = new List<Image>();

        [Header("-----Others")] [SerializeField]
        private GameObject bottomHome;

        [SerializeField] private GameObject bottomGame;


        void Awake()
        {
            Init(GetComponent<CanvasGroup>());
            soundSlider.onValueChanged.AddListener(_value => OnChanged_SoundVolume());
            musicSlider.onValueChanged.AddListener(_value => OnChanged_MusicVolume());
            hapticsToggle.onValueChanged.AddListener(_value => OnChanged_HapticsState());

            // for (int i = 1; i < musicBar.childCount - 1; i++)
            // {
            //     musicBarChilds.Add(musicBar.GetChild(i).GetComponent<Image>());
            //     soundBarChilds.Add(soundBar.GetChild(i).GetComponent<Image>());
            // }

            logOutButtonTxt.text = GameData.Instance.Platform == Enums.Platform.WebGl ? "EXIT GAME" : "Logout";
            logOutButtonTxt.color = GameData.Instance.Platform == Enums.Platform.WebGl ? Color.black : Color.white;
        }


        private void Start()
        {
            if (PlayerPrefsManager.Instance)
                InitialSetup();
        }

        public void OnClick_Support()
        {
#if UNITY_ANDROID || UNITY_EDITOR
            Application.OpenURL("https://forms.gle/g6D8nmbfjtPnf2pr7");
#elif UNITY_IOS
            SafariViewController.OpenURL("https://forms.gle/g6D8nmbfjtPnf2pr7");
#endif
        }

        public void OnClick_AccountDelete()
        {
#if UNITY_ANDROID || UNITY_EDITOR
            Application.OpenURL("https://forms.gle/tdUpWdSycagp6dBb8");
#elif UNITY_IOS
            SafariViewController.OpenURL("https://forms.gle/tdUpWdSycagp6dBb8");
#endif
        }

        private void InitialSetup()
        {
            hapticsToggle.isOn = PlayerPrefsManager.Instance.HapticsState == 1;

            musicVolume = PlayerPrefsManager.Instance.MusicVol;
            musicSlider.SetValueWithoutNotify(musicVolume);
            AudioManager.Instance.SetMusicVol(musicVolume);
            // for (int i = 0; i < musicBarChilds.Count; i++)
            //     musicBarChilds[i].color =
            //         i < musicVolume ? ColorManager.Instance.Cyan : ColorManager.Instance.DarkerGrey;
            // musicMinusBtn.interactable = musicVolume > 0;
            // musicPlusBtn.interactable = musicVolume < 10;

            soundVolume = PlayerPrefsManager.Instance.SfxVol;
            soundSlider.SetValueWithoutNotify(soundVolume);
            AudioManager.Instance.SetSfxVol(soundVolume);
            // for (int i = 0; i < soundBarChilds.Count; i++)
            //     soundBarChilds[i].color =
            //         i < soundVolume ? ColorManager.Instance.Cyan : ColorManager.Instance.DarkerGrey;
            // soundMinusBtn.interactable = soundVolume > 0;
            // soundPlusBtn.interactable = soundVolume < 10;

            GraphicsSetup(PlayerPrefsManager.Instance.Graphics);
        }


        public void OnClick_Logout()
        {
            if (GameData.Instance.Platform != Enums.Platform.WebGl)
            {
                // Loader.Instance.ShowLoader();
                PlayerPrefsManager.Instance.RemovePlayerId();
                PlayerPrefsManager.Instance.RemoveVia();
                GameData.Instance.ResetPlayerData();
                AnalyticsManager.Instance.FireLogoutEvent();
                SceneManager.SwitchToLoginScene();
            }
            else
            {
#if UNITY_WEBGL
                JavaScriptCallbacks.Quit();
#endif
            }
        }


        #region Music

        public void OnClick_MusicPlus()
        {
            // musicBarChilds[musicVolume].color = ColorManager.Instance.Cyan;
            // AudioManager.Instance.SetMusicVol(++musicVolume);
            // musicMinusBtn.interactable = musicVolume > 0;
            // musicPlusBtn.interactable = musicVolume < 10;
        }

        public void OnClick_MusicMinus()
        {
            // musicPlusBtn.interactable = musicVolume <= 10;
            // AudioManager.Instance.SetMusicVol(--musicVolume);
            // musicMinusBtn.interactable = musicVolume > 0;
            // musicBarChilds[musicVolume].color = ColorManager.Instance.DarkerGrey;
        }

        private void OnChanged_SoundVolume()
        {
            soundVolume = (int)soundSlider.value;
            AudioManager.Instance.SetMusicVol(soundVolume);
        }

        #endregion

        private void OnChanged_HapticsState()
        {
            hapticsSlider.value = hapticsToggle.isOn ? 1 : 0;
            hapticsStatusTMP.text = hapticsToggle.isOn ? "ON" : "OFF";
        }

        #region Sound

        public void OnClick_SoundPlus()
        {
            // soundBarChilds[soundVolume].color = ColorManager.Instance.Cyan;
            // AudioManager.Instance.SetSfxVol(++soundVolume);
            // soundMinusBtn.interactable = soundVolume > 0;
            // soundPlusBtn.interactable = soundVolume < 10;
        }

        public void OnClick_SoundMinus()
        {
            // soundPlusBtn.interactable = soundVolume <= 10;
            // AudioManager.Instance.SetSfxVol(--soundVolume);
            // soundMinusBtn.interactable = soundVolume > 0;
            // soundBarChilds[soundVolume].color = ColorManager.Instance.DarkerGrey;
        }

        private void OnChanged_MusicVolume()
        {
            musicVolume = (int)musicSlider.value;
            AudioManager.Instance.SetMusicVol(musicVolume);
        }

        #endregion


        #region Graphics

        public void OnClick_Graphics(int _value) => GraphicsSetup((Enums.GraphicsMode)_value);

        private void GraphicsSetup(Enums.GraphicsMode _mode)
        {
            PlayerPrefsManager.Instance.SwitchGraphics(_mode);
            Color _color = ColorManager.Instance.DarkerGrey;
            _color.a = 0.3f;

            switch (_mode)
            {
                case Enums.GraphicsMode.Performant:
                    lowTMP.color = Color.black;
                    midTMP.color = highTMP.color = ColorManager.Instance.LightGrey;

                    lowImage.color = ColorManager.Instance.Cyan;
                    midImage.color = highImage.color = _color;

                    QualitySettings.SetQualityLevel(0, true);
                    break;

                case Enums.GraphicsMode.Balanced:
                    midTMP.color = Color.black;
                    lowTMP.color = highTMP.color = ColorManager.Instance.LightGrey;

                    midImage.color = ColorManager.Instance.Cyan;
                    lowImage.color = highImage.color = _color;

                    QualitySettings.SetQualityLevel(1, true);
                    break;

                case Enums.GraphicsMode.HighFidelity:
                    highTMP.color = Color.black;
                    lowTMP.color = midTMP.color = ColorManager.Instance.LightGrey;

                    highImage.color = ColorManager.Instance.Cyan;
                    lowImage.color = midImage.color = _color;

                    QualitySettings.SetQualityLevel(2, true);
                    break;
            }
        }

        #endregion


        public override void Init()
        {
            base.Init();

            bottomHome.SetActive(ScreenManager.Instance.CurrentScreen == Enums.Screen.Home ||
                                 ScreenManager.Instance.CurrentScreen == Enums.Screen.Invite);
            bottomGame.SetActive(ScreenManager.Instance.CurrentScreen == Enums.Screen.Gameplay);

            if (ScreenManager.Instance.CurrentScreen == Enums.Screen.Gameplay)
            {
                GameController.IsGamePaused = true;
                GameplayScreen.Instance.ToggleUI(false);
            }
            else MenuBar.Instance.BlockRaycasts(false);

            InitialSetup();
        }


        public override void Close()
        {
            SavePrefs();

            if (ScreenManager.Instance.CurrentScreen == Enums.Screen.Gameplay)
            {
                GameController.IsGamePaused = false;
                GameplayScreen.Instance.ToggleUI(true);
            }
            else MenuBar.Instance.BlockRaycasts(true);

            base.Close();
        }


        public void OnClick_Restart()
        {
            SavePrefs();
            Hide();
            MenuBar.Instance.Hide();
            GameplayScreen.Instance.ToggleUI(true);
            GameController.Instance.SetNextLevel(true);
        }

        public void OnClick_Exit()
        {
            SavePrefs();
            Hide();
            GameData.Instance.EndGame();
            GameController.IsGamePaused = false;
        }

        private void SavePrefs()
        {
            PlayerPrefsManager.Instance.SaveMusicVol(musicVolume);
            PlayerPrefsManager.Instance.SaveSFXVol(soundVolume);
            PlayerPrefsManager.Instance.SaveHaptics(hapticsToggle.isOn ? 1 : 0);
            PlayerPrefsManager.Instance.SaveGraphics();
        }
    }
}