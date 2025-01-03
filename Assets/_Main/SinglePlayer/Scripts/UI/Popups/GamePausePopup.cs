using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    public class GamePausePopup : PopupBehaviour
    {
        [Header("-----Graphics")]
        [SerializeField] private RectTransform highlight;
        [SerializeField] private TextMeshProUGUI lowTMP, midTMP, highTMP, logOutButtonTxt;
        [SerializeField] private Image lowImage, midImage, highImage;
        //[SerializeField] private Vector3 hightlightYPosition;


        [Header("-----Sound")]
        [SerializeField] private Transform soundBar;
        [SerializeField] private Button soundMinusBtn;
        [SerializeField] private Button soundPlusBtn;
        private List<Image> soundBarChilds = new List<Image>();
        private int soundVolume;


        [Header("-----Music")]
        [SerializeField] private Transform musicBar;
        [SerializeField] private Button musicMinusBtn;
        [SerializeField] private Button musicPlusBtn;
        private List<Image> musicBarChilds = new List<Image>();
        private int musicVolume;

        [Header("-----Others")]
        [SerializeField] private GameObject bottomHome;
        [SerializeField] private GameObject bottomGame;


        void Awake()
        {
            Init(GetComponent<CanvasGroup>());
            for (int i = 1; i < musicBar.childCount - 1; i++)
            {
                musicBarChilds.Add(musicBar.GetChild(i).GetComponent<Image>());
                soundBarChilds.Add(soundBar.GetChild(i).GetComponent<Image>());
            }
            logOutButtonTxt.text = GameData.Instance.Platform == Enums.Platform.WebGl ? "Quit" : "Logout";


        }


        private void Start()
        {
            if (PlayerPrefsManager.Instance)
                InitialSetup();
        }

        public void OnClick_Support() => Application.OpenURL("https://discord.gg/Uv6XBynzXp");

        private void InitialSetup()
        {
            musicVolume = PlayerPrefsManager.Instance.MusicVol;

            AudioManager.Instance.SetMusicVol(musicVolume);
            for (int i = 0; i < musicBarChilds.Count; i++) musicBarChilds[i].color = i < musicVolume ? ColorManager.Instance.Cyan : ColorManager.Instance.DarkerGrey;
            musicMinusBtn.interactable = musicVolume > 0;
            musicPlusBtn.interactable = musicVolume < 10;

            soundVolume = PlayerPrefsManager.Instance.SfxVol;

            for (int i = 0; i < soundBarChilds.Count; i++) soundBarChilds[i].color = i < soundVolume ? ColorManager.Instance.Red : ColorManager.Instance.DarkGrey;
            soundMinusBtn.interactable = soundVolume > 0;
            soundPlusBtn.interactable = soundVolume < 10;

            GraphicsSetup(PlayerPrefsManager.Instance.Graphics);
        }


        public void OnClick_Logout()
        {
#if !UNITY_WEBGL


            // Loader.Instance.ShowLoader();
            PlayerPrefsManager.Instance.RemovePlayerId();
            PlayerPrefsManager.Instance.RemoveVia();
            GameData.Instance.ResetPlayerData();
            AnalyticsManager.Instance.FireLogoutEvent();
            SceneManager.SwitchToLoginScene();
#else
            JavaScriptCallbacks.Quit();
#endif
        }



        #region Music
        public void OnClick_MusicPlus()
        {
            musicBarChilds[musicVolume].color = ColorManager.Instance.Cyan;
            AudioManager.Instance.SetMusicVol(++musicVolume);
            musicMinusBtn.interactable = musicVolume > 0;
            musicPlusBtn.interactable = musicVolume < 10;
        }
        public void OnClick_MusicMinus()
        {
            musicPlusBtn.interactable = musicVolume <= 10;
            AudioManager.Instance.SetMusicVol(--musicVolume);
            musicMinusBtn.interactable = musicVolume > 0;

            musicBarChilds[musicVolume].color = ColorManager.Instance.DarkGrey;
        }
        #endregion



        #region Sound
        public void OnClick_SoundPlus()
        {
            soundBarChilds[soundVolume].color = ColorManager.Instance.Cyan;
            AudioManager.Instance.SetSfxVol(++soundVolume);
            soundMinusBtn.interactable = soundVolume > 0;
            soundPlusBtn.interactable = soundVolume < 10;
        }
        public void OnClick_SoundMinus()
        {
            soundPlusBtn.interactable = soundVolume <= 10;
            AudioManager.Instance.SetSfxVol(--soundVolume);
            soundMinusBtn.interactable = soundVolume > 0;

            soundBarChilds[soundVolume].color = ColorManager.Instance.DarkGrey;
        }
        #endregion



        #region Graphics
        public void OnClick_Graphics(int _value) => GraphicsSetup((Enums.GraphicsMode)_value);
        private void GraphicsSetup(Enums.GraphicsMode _mode)
        {
            PlayerPrefsManager.Instance.SwitchGraphics(_mode);
            //changeToHigh.SetActive(_mode == Enums.GraphicsMode.Performant);
            //changeToLow.SetActive(_mode == Enums.GraphicsMode.Balanced);
            switch (_mode)
            {
                case Enums.GraphicsMode.Performant:
                    lowImage.color = ColorManager.Instance.Cyan;
                    midImage.color = highImage.color = ColorManager.Instance.DarkestGrey;
                    QualitySettings.SetQualityLevel(0, true);
                    break;
                case Enums.GraphicsMode.Balanced:
                    midImage.color = ColorManager.Instance.Cyan;
                    lowImage.color = highImage.color = ColorManager.Instance.DarkestGrey;
                    QualitySettings.SetQualityLevel(1, true);
                    break;

                case Enums.GraphicsMode.HighFidelity:
                    highImage.color = ColorManager.Instance.Cyan;
                    lowImage.color = midImage.color = ColorManager.Instance.DarkestGrey;
                    QualitySettings.SetQualityLevel(2, true);
                    break;
            }
        }
        #endregion


        public override void Init()
        {
            base.Init();

            bottomHome.SetActive(ScreenManager.Instance.CurrentScreen == Enums.Screen.Home || ScreenManager.Instance.CurrentScreen == Enums.Screen.Invite);
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
            PlayerPrefsManager.Instance.SaveGraphics();
        }
    }
}