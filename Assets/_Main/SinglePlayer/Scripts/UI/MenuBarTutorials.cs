using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FoodFury
{
    public class MenuBarTutorials : MonoBehaviour
    {
        [SerializeField] private SettingsPopup settingsPopup;
        [SerializeField] private ExitGamePopup exitPopup;

        [SerializeField] private GameObject exitBtn, detailsObj, challengeObj;
        [SerializeField] private TMPro.TextMeshProUGUI titleTMP, challengeTMP;

        private Image menuBarImage;


        private CanvasGroup canvasGroup;
        void Awake()
        {
            menuBarImage = GetComponent<Image>();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            ToggleMenuBarBG(false);
            ToggleDetailsPage(false);
        }

        public void ToggleDetailsPage(bool _flag) => detailsObj.SetActive(_flag);
        public void ToggleMenuBarBG(bool _flag) => menuBarImage.enabled = _flag;
        public void ToggleChallenge(bool _flag) => challengeObj.SetActive(_flag);

        public void SetDetails(string _title, string _challenge)
        {
            titleTMP.text = _title;
            challengeTMP.text = _challenge;
        }



        public void OnClick_Settings()
        {
            settingsPopup.Init();
            settingsPopup.Show();
        }

        public void OnClick_Exit(bool _showConfirmationPopup)
        {
            canvasGroup.alpha = 0;
            if (!_showConfirmationPopup)
            {
                TweenHandler.StopTween();
                Loader.Instance.ShowLoader();
                SceneManager.SwitchToHomeScene(_callback: (op) =>
                 {
                     ScreenManager.Instance.SwitchScreen(HomeScreen.Instance, null);
                 });
            }
            else
            {
                exitPopup.Show(_result =>
                {
                    if (_result)
                    {
                        Loader.Instance.ShowLoader();
                        SceneManager.SwitchToHomeScene(_callback: (op) =>
                        {
                            ScreenManager.Instance.SwitchScreen(HomeScreen.Instance, null);
                        });
                    }
                    else canvasGroup.alpha = 1;
                });
            }
        }




        public void Show(Action _callback = null)
        {
            TweenHandler.CanvasGroupAlpha(canvasGroup, 1, 0.5f, DG.Tweening.Ease.OutExpo, () =>
            {
                canvasGroup.blocksRaycasts = true;
                _callback?.Invoke();
            });
        }

        public void Hide(Action _callback = null)
        {
            canvasGroup.blocksRaycasts = false;
            TweenHandler.CanvasGroupAlpha(canvasGroup, 0, 0.5f, DG.Tweening.Ease.OutExpo, () =>
            {
                _callback?.Invoke();
            });
        }


    }

}