using System;
using System.Collections;
using System.Collections.Generic;
using FoodFury;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace OneRare.FoodFury.Multiplayer
{
    public class SettingsPopupMP : MonoBehaviour
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
        
        [Header("-----Others")]
        [SerializeField] private GameObject bottomHome;
        [SerializeField] private GameObject bottomGame;

        [SerializeField] private GameObject settingsPanel;
        //[SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Button restartButton;
        // Start is called before the first frame update

        private MobileInput mobileInput;

        void Start()
        {
            if (PlayerPrefsManager.Instance)
                InitialSetup();
            // Ensure the settings panel is hidden at the start
            settingsPanel.SetActive(false);

            // Add listeners for the buttons
            // settingsButton.onClick.AddListener(ToggleSettingsPanel);
            quitButton.onClick.AddListener(QuitGame);
            restartButton.onClick.AddListener(RestartGame);
            
            soundSlider.onValueChanged.AddListener(_value => OnChanged_SoundVolume());
            musicSlider.onValueChanged.AddListener(_value => OnChanged_MusicVolume());
            hapticsToggle.onValueChanged.AddListener(_value => OnChanged_HapticsState());
        }

        private void OnEnable()
        {
            if (PlayerPrefsManager.Instance)
                InitialSetup();
        }

        private void OnDisable()
        {
            SavePrefs();
        }
        
        private void SavePrefs()
        {
            PlayerPrefsManager.Instance.SaveMusicVol(musicVolume);
            PlayerPrefsManager.Instance.SaveSFXVol(soundVolume);
            PlayerPrefsManager.Instance.SaveGraphics();
        }

        private void InitialSetup()
        {
            hapticsToggle.isOn = PlayerPrefsManager.Instance.HapticsState == 1;

            musicVolume = PlayerPrefsManager.Instance.MusicVol;
            musicSlider.SetValueWithoutNotify(musicVolume);
            AudioManager.Instance.SetMusicVol(musicVolume);
            
            soundVolume = PlayerPrefsManager.Instance.SfxVol;
            soundSlider.SetValueWithoutNotify(soundVolume);
            AudioManager.Instance.SetSfxVol(soundVolume);

            GraphicsSetup(PlayerPrefsManager.Instance.Graphics);
        }


        #region Music
        public void OnClick_MusicPlus()
        {
            /*musicBarChilds[musicVolume].color = ColorManager.Instance.Cyan;
            AudioManager.Instance.SetMusicVol(++musicVolume);
            musicMinusBtn.interactable = musicVolume > 0;
            musicPlusBtn.interactable = musicVolume < 10;*/
        }
        public void OnClick_MusicMinus()
        {
            /*musicPlusBtn.interactable = musicVolume <= 10;
            AudioManager.Instance.SetMusicVol(--musicVolume);
            musicMinusBtn.interactable = musicVolume > 0;
            musicBarChilds[musicVolume].color = ColorManager.Instance.DarkerGrey;*/
        }
        
        private void OnChanged_SoundVolume()
        {
            soundVolume = (int)soundSlider.value;
            AudioManager.Instance.SetSfxVol(soundVolume);
        }
        #endregion
        
        #region Sound
        public void OnClick_SoundPlus()
        {
            /*soundBarChilds[soundVolume].color = ColorManager.Instance.Cyan;
            AudioManager.Instance.SetSfxVol(++soundVolume);
            soundMinusBtn.interactable = soundVolume > 0;
            soundPlusBtn.interactable = soundVolume < 10;*/
        }
        public void OnClick_SoundMinus()
        {
            /*soundPlusBtn.interactable = soundVolume <= 10;
            AudioManager.Instance.SetSfxVol(--soundVolume);
            soundMinusBtn.interactable = soundVolume > 0;
            soundBarChilds[soundVolume].color = ColorManager.Instance.DarkerGrey;*/
        }
        
        private void OnChanged_MusicVolume()
        {
            musicVolume = (int)musicSlider.value;
            AudioManager.Instance.SetMusicVol(musicVolume);
        }
        #endregion
        #region Graphics
        public void OnClick_Graphics(int _value) => GraphicsSetup((global::Enums.GraphicsMode)_value);
        private void GraphicsSetup(global::Enums.GraphicsMode _mode)
        {
            PlayerPrefsManager.Instance.SwitchGraphics(_mode);

            switch (_mode)
            {
                case global::Enums.GraphicsMode.Performant:
                    lowTMP.color = Color.black;
                    midTMP.color = highTMP.color = ColorManager.Instance.LightGrey;

                    lowImage.color = ColorManager.Instance.Cyan;
                    midImage.color = highImage.color = ColorManager.Instance.DarkestGrey;

                    QualitySettings.SetQualityLevel(0, true);
                    break;

                case global::Enums.GraphicsMode.Balanced:
                    midTMP.color = Color.black;
                    lowTMP.color = highTMP.color = ColorManager.Instance.LightGrey;

                    midImage.color = ColorManager.Instance.Cyan;
                    lowImage.color = highImage.color = ColorManager.Instance.DarkestGrey;

                    QualitySettings.SetQualityLevel(1, true);
                    break;

                case global::Enums.GraphicsMode.HighFidelity:
                    highTMP.color = Color.black;
                    lowTMP.color = midTMP.color = ColorManager.Instance.LightGrey;

                    highImage.color = ColorManager.Instance.Cyan;
                    lowImage.color = midImage.color = ColorManager.Instance.DarkestGrey;

                    QualitySettings.SetQualityLevel(2, true);
                    break;
            }
        }
        #endregion
        
        private void OnChanged_HapticsState()
        {
            hapticsSlider.value = hapticsToggle.isOn ? 1 : 0;
            hapticsStatusTMP.text = hapticsToggle.isOn ? "ON" : "OFF";
        }
        public void OpenSettingsPanel()
        {
            // Toggle the visibility of the settings panel
            settingsPanel.SetActive(true);
        }

        private GameManager gameManager;
        async void QuitGame()
        {
            if (Player.Local != null)
            {
                int _chips = Player.Local.Chips;
                int _munches = Player.Local.Munches;
                Debug.Log($"Sending player data to server: Chips:{_chips} | Munches:{_munches}");
                await PlayerAPIs.UpdatePlayerDataGameOverMultiplayer_API(_chips, _munches);
            }
            else Debug.Log("PLAYER LOCAL IS NULL---------------");

            // Code to quit the game
            mobileInput = FindObjectOfType<MobileInput>();
            mobileInput.OnDisconnect();
            Debug.Log("Quitting Game...");
            Close();
            //Application.Quit();
        }

        void RestartGame()
        {
            mobileInput = FindObjectOfType<MobileInput>();
            mobileInput.OnDisconnect();
            // Code to restart the game
            Debug.Log("Restarting Game...");
            // Assuming scene index 0 is the main game scene
            // UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }

        public void Close()
        {
            settingsPanel.SetActive(false);
            //SavePrefs();

            /*if (ScreenManager.Instance.CurrentScreen == Enums.Screen.Gameplay)
            {
                GameController.Instance.IsGamePaused = false;
                GameplayScreen.Instance.ToggleUI(true);
            }
            else MenuBar.Instance.BlockRaycasts(true);*/

            //base.Close();
        }

    }
}
