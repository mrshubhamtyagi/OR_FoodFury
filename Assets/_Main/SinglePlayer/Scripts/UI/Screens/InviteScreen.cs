using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    public class InviteScreen : MonoBehaviour, IScreen
    {
        [SerializeField] private InputField input;
        [SerializeField] private TextMeshProUGUI errorTMP;
        [SerializeField] private GameObject letsRideObj;
        [SerializeField] private GameObject getLicensedObj;
        [SerializeField] private GameObject setup;
        [SerializeField] private Transform vehicleModels;
        private OrbitManager orbitManager;
        private CanvasGroup canvasGroup;
        public static InviteScreen Instance { get; private set; }
        void Awake()
        {
            Instance = this;
            canvasGroup = GetComponent<CanvasGroup>();
            orbitManager = vehicleModels.GetComponent<OrbitManager>();
        }

        public void OnClick_Settings() => PopUpManager.Instance.ShowPopup(PopUpManager.Instance.SettingsPopup);

        public void OnClick_LetsRide()
        {
            if (string.IsNullOrWhiteSpace(input.text))
            {
                errorTMP.text = "Invalid Code, Please try again";
                errorTMP.gameObject.SetActive(true);
            }
            else
            {
                //GameData.Instance.ValidateInviteCode_API(input.text, _success =>
                // {
                //     if (_success)
                //     {
                //         errorTMP.gameObject.SetActive(false);
                //         letsRideObj.SetActive(false);
                //         getLicensedObj.SetActive(true);
                //     }
                //     else
                //     {
                //         errorTMP.text = "Invalid Code, Please try again";
                //         errorTMP.gameObject.SetActive(true);
                //     }
                // });

            }
        }

        public void OnClick_GetLicensed() => ScreenManager.Instance.ShowTutorials();
        public void OnClick_Feedback() => PopUpManager.Instance.ShowPopup(PopUpManager.Instance.FeedbackPopup);



        private void Init()
        {
            errorTMP.gameObject.SetActive(false);
            input.text = "";

            //letsRideObj.SetActive(string.IsNullOrWhiteSpace(GameData.Instance.PlayerData.inviteCode));
            letsRideObj.SetActive(false);
            getLicensedObj.SetActive(true);
            MenuBar.Instance.Init();

            foreach (Transform item in vehicleModels)
                item.gameObject.SetActive(false);

            vehicleModels.GetChild(GameData.Instance.PlayerData.Data.currentVehicle - 1).gameObject.SetActive(true);

            orbitManager.ResetRotation();
            orbitManager.autoRotate = true;
            setup.SetActive(true);
        }

        public void Show(Action _callback = null)
        {
            ScreenManager.Instance.CurrentScreen = Enums.Screen.Invite;
            Init();
            MenuBar.Instance.Show();
            AudioManager.Instance.PlayHomeScreenTrack();

            TweenHandler.CanvasGroupAlpha(canvasGroup, 1, ScreenManager.Instance.TweenDuration, ScreenManager.Instance.TweenEase, () =>
            {
                canvasGroup.blocksRaycasts = true;
                _callback?.Invoke();
                Loader.Instance.HideLoader();
            });
        }

        public void Hide(Action _callback = null)
        {
            Loader.Instance.ShowLoader();
            setup.SetActive(false);
            canvasGroup.blocksRaycasts = false;
            TweenHandler.CanvasGroupAlpha(canvasGroup, 0, ScreenManager.Instance.TweenDuration, ScreenManager.Instance.TweenEase, () =>
            {
                _callback?.Invoke();
            });
        }
    }

}