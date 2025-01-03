using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

namespace FoodFury
{
    public class TutorialScreen : MonoBehaviour
    {
        public int step = 0;
        public int fuel = 1800;

        [SerializeField] private Rider rider;
        [SerializeField] private DrivingTestOrderCampass campass;
        [SerializeField] private GameObject booster;
        [SerializeField] private GameObject boosterObj;
        [SerializeField] private GameObject congratsObj;
        [SerializeField] private MenuBarTutorials menuBar;
        [SerializeField] private Button startContent;
        [SerializeField] private Button lessonCompleted;
        [SerializeField] private GameObject mainBG;
        [SerializeField] private DrivingLicence learnerLicence;
        [SerializeField] private DrivingLicence drivingLicence;

        [Header("-----Player")]
        [SerializeField] private RawImage playerPic;
        [SerializeField] private TextMeshProUGUI usernameTMP;

        [Header("-----Lesson Start")]
        [SerializeField] private Button lessonStart;
        [SerializeField] private TextMeshProUGUI startTitleTMP;
        [SerializeField] private TextMeshProUGUI startInfoTMP;

        [Header("-----Controls")]
        [SerializeField] private GameObject controls;
        [SerializeField] private GameObject controlsBG, topPanel, nameTagsAndInfo;
        //[SerializeField] private Button forwardArrow, reverseArrow, rightArrow, leftArrow;
        //private OnScreenButton forwardArrowScreenBtn, reverseArrowScreenBtn, rightArrowScreenBtn, leftArrowScreenBtn;

        [Header("-----Order")]
        [SerializeField] private RawImage orderThumb;
        [SerializeField] private Image orderFillImage;
        [SerializeField] private TextMeshProUGUI dishNameTMP;
        [SerializeField] private TextMeshProUGUI dishTimeTMP;

        [Header("-----ControlsTags")]
        [SerializeField] private CanvasGroup forwardTag;
        [SerializeField] private CanvasGroup breakTag;
        [SerializeField] private CanvasGroup leftAndrightTag;

        [Header("-----Others")]
        [SerializeField] private CanvasGroup passedCG;
        [SerializeField] private CanvasGroup failedCG;
        [SerializeField] private Image fuelFillImage;
        [SerializeField] private TextMeshProUGUI fuelTMP;
        [SerializeField] private Transform speedNeedle;
        [SerializeField] private GameObject skipChallenge;


        public static TutorialScreen Instance;
        private void Awake()
        {
            Instance = this;
            //forwardArrowScreenBtn = forwardArrow.GetComponent<OnScreenButton>();
            //reverseArrowScreenBtn = reverseArrow.GetComponent<OnScreenButton>();
            //rightArrowScreenBtn = rightArrow.GetComponent<OnScreenButton>();
            //leftArrowScreenBtn = leftArrow.GetComponent<OnScreenButton>();
        }

        private void Start()
        {
            InputManager.Instance.enabled = false;
            menuBar.gameObject.SetActive(true);
            passedCG.gameObject.SetActive(true);
            failedCG.gameObject.SetActive(true);
            DisableTags();
            campass.gameObject.SetActive(false);
            learnerLicence.transform.parent.gameObject.SetActive(false);
            //forwardTag.alpha = breakTag.alpha = leftAndrightTag.alpha = 0;

            step = !GameData.Instance.PlayerData.Data.isLicenseComplete ? 1 : 0;

            OnClick_TapToContinue();
        }

        private Vector3 _needleRotation;
        private void Update()
        {
            if (InputManager.Instance.enabled)
            {
                _needleRotation.z = Mathf.Lerp(90, -90, rider.Vehicle.CurrentSpeed01);
                speedNeedle.localRotation = Quaternion.Slerp(speedNeedle.rotation, Quaternion.Euler(_needleRotation), Time.deltaTime * 3);
            }
        }

        public void OnClick_TapToContinue()
        {
            skipChallenge.SetActive(false);
            TutorialManager.Instance.result = TutorialManager.ChallengeReseult.Pending;
            switch (step)
            {
                case 0:
                    InputManager.Instance.enabled = false;
                    menuBar.Show();
                    mainBG.SetActive(true);
                    menuBar.gameObject.SetActive(true);
                    lessonStart.gameObject.SetActive(false);
                    lessonCompleted.gameObject.SetActive(false);
                    controls.SetActive(false);
                    startContent.gameObject.SetActive(true);
                    break;

                case 1: // Licence
                    startContent.gameObject.SetActive(false);
                    if (GameData.Instance.PlayerData.Data.isLicenseComplete)
                    {
                        step++;
                        OnClick_TapToContinue();
                        return;
                    }
                    else
                        learnerLicence.transform.parent.gameObject.SetActive(true);
                    break;

                case 2:// Lesson 1
                    learnerLicence.transform.parent.gameObject.SetActive(false);
                    startContent.gameObject.SetActive(false);
                    startTitleTMP.text = "Lesson 1: Driving";
                    startInfoTMP.text = "Complete 5 Challenges to Pass your Driving Lesson!";
                    lessonStart.gameObject.SetActive(true);
                    break;

                case 3: // Controls
                    mainBG.SetActive(false);
                    lessonStart.gameObject.SetActive(false);

                    topPanel.SetActive(false);
                    nameTagsAndInfo.SetActive(true);

                    //forwardArrow.interactable = reverseArrow.interactable = rightArrow.interactable = leftArrow.interactable = false;
                    //forwardArrowScreenBtn.enabled = reverseArrowScreenBtn.enabled = rightArrowScreenBtn.enabled = leftArrowScreenBtn.enabled = false;
                    controlsBG.SetActive(true);
                    controls.SetActive(true);
                    break;

                case 4:// Challenge 1
                    Invoke("EnableSkipChallenge", 1);
                    nameTagsAndInfo.SetActive(false);
                    controlsBG.SetActive(false);
                    //forwardArrow.interactable = true;
                    //forwardArrowScreenBtn.enabled = true;
                    forwardTag.gameObject.SetActive(true);
                    Invoke("DisableTags", 2f);
                    Lession1Setup(step - 4);
                    break;

                case 5: // Challenge 2
                    Invoke("EnableSkipChallenge", 1);
                    //reverseArrow.interactable = true;
                    //reverseArrowScreenBtn.enabled = true;
                    breakTag.gameObject.SetActive(true);
                    Invoke("DisableTags", 2f);
                    Lession1Setup(step - 4);
                    break;

                case 6: // Challenge 3
                    Invoke("EnableSkipChallenge", 1);
                    //rightArrow.interactable = leftArrow.interactable = true;
                    //rightArrowScreenBtn.enabled = leftArrowScreenBtn.enabled = true;
                    leftAndrightTag.gameObject.SetActive(true);
                    Invoke("DisableTags", 2f);
                    Lession1Setup(step - 4);
                    break;

                case 7: // Challenge 4
                    Invoke("EnableSkipChallenge", 1);
                    Lession1Setup(step - 4);
                    break;

                case 8: // Challenge 5
                    Invoke("EnableSkipChallenge", 1);
                    Lession1Setup(step - 4);
                    break;

                case 9: // Lesson Complete
                    menuBar.ToggleMenuBarBG(false);
                    menuBar.ToggleDetailsPage(false);
                    controls.SetActive(false);

                    mainBG.SetActive(true);
                    lessonCompleted.gameObject.SetActive(true);
                    TogglePassedScreen(false);
                    break;

                case 10: // Lesson 2
                    lessonCompleted.gameObject.SetActive(false);
                    startTitleTMP.text = "Lesson 2: Delivery";
                    startInfoTMP.text = "Deliver 3 dishes, to sucessfully finish your lesson and gain license";
                    lessonStart.gameObject.SetActive(true);
                    break;

                case 11: // Challenge 6
                    Invoke("EnableSkipChallenge", 1);
                    lessonStart.gameObject.SetActive(false);
                    controls.SetActive(true);
                    controlsBG.SetActive(false);
                    mainBG.SetActive(false);
                    topPanel.SetActive(true);
                    Lession2Setup(step - 6);
                    break;

                case 12: // Challenge 7
                    Invoke("EnableSkipChallenge", 1);
                    campass.SetTarget(TutorialManager.Instance.steps[step - 6].finishingPoint.transform);
                    campass.gameObject.SetActive(true);
                    Lession2Setup(step - 6);
                    break;

                case 13: // Challenge 8
                    Invoke("EnableSkipChallenge", 1);
                    booster.SetActive(true);
                    campass.SetTarget(TutorialManager.Instance.steps[step - 6].finishingPoint.transform);
                    Lession2Setup(step - 6);
                    break;

                case 14: // Congo
                    menuBar.Hide();
                    controls.SetActive(false);
                    mainBG.SetActive(true);
                    TogglePassedScreen(false);
                    return;

                default:
                    Debug.Log("Default Case");
                    break;
            }
            step++;
        }

        private void EnableSkipChallenge() => skipChallenge.SetActive(true);
        private void DisableTags()
        {
            forwardTag.gameObject.SetActive(false);
            breakTag.gameObject.SetActive(false);
            leftAndrightTag.gameObject.SetActive(false);
        }

        private void Lession1Setup(int _index)
        {
            //Debug.Log("Index - " + _index);
            StartCoroutine("Co_FuelTimeout");

            menuBar.SetDetails($"Challenge {_index + 1}", TutorialManager.Instance.steps[_index].stepInfo);
            menuBar.ToggleMenuBarBG(true);
            menuBar.ToggleDetailsPage(true);

            ToggleBooster(false);
            TogglePassedScreen(false);
            ToggleFailedScreen(false);
            InputManager.Instance.enabled = true;
            rider.Vehicle.VehicleSound.SetupSound();
        }

        private void Lession2Setup(int _index)
        {
            //Debug.Log("Index - " + _index);
            StopCoroutine("Co_FuelTimeout");
            StopCoroutine("Co_OrderTimeout");
            orderFillImage.fillAmount = 1;
            orderTimeout = TutorialManager.Instance.steps[_index].orderTimeout;
            dishNameTMP.text = TutorialManager.Instance.steps[_index].dishName;
            dishTimeTMP.text = TutorialManager.Instance.steps[_index].orderTimeout.ToString();
            orderThumb.texture = TutorialManager.Instance.steps[_index].nft.material.mainTexture = TutorialManager.Instance.steps[_index].orderThumb;

            if (TutorialManager.Instance.steps[_index].tag)
            {
                TutorialManager.Instance.steps[_index].tag.SetActive(true);
                StartCoroutine(InvokeCustom(_index, 3));
            }
            StartCoroutine("Co_FuelTimeout");
            StartCoroutine("Co_OrderTimeout");

            menuBar.SetDetails($"Delivery {_index - 4}", TutorialManager.Instance.steps[_index].stepInfo);
            menuBar.ToggleMenuBarBG(false);
            menuBar.ToggleDetailsPage(true);
            menuBar.ToggleChallenge(false);

            ToggleBooster(false);
            TogglePassedScreen(false);
            ToggleFailedScreen(false);
            InputManager.Instance.enabled = true;
            rider.Vehicle.VehicleSound.SetupSound();

        }
        IEnumerator InvokeCustom(int _index, float _time)
        {
            yield return new WaitForSeconds(_time);
            TutorialManager.Instance.steps[_index].tag.SetActive(false);
        }


        private WaitForSeconds sec = new WaitForSeconds(1);
        [SerializeField] private float orderCurrentTime;
        private int orderTimeout;
        public IEnumerator Co_OrderTimeout()
        {
            orderCurrentTime = orderTimeout;
            while (orderCurrentTime > 0)
            {
                yield return sec;
                dishTimeTMP.text = (--orderCurrentTime).ToString();
                if (orderCurrentTime < 0) orderFillImage.fillAmount = 1;
                else orderFillImage.fillAmount = 1 / (orderTimeout / orderCurrentTime);
            }

            TutorialManager.Instance.OnStepFailed();
        }

        public IEnumerator Co_FuelTimeout()
        {
            while (fuel > 0)
            {
                yield return sec;
                if (InputManager.Instance.enabled)
                {
                    fuelTMP.text = HelperFunctions.ToTimerString(--fuel);
                    fuelFillImage.fillAmount = fuel / 1800f;
                }
            }
        }

        public void OnChallengeCompleted()
        {
            InputManager.Instance.enabled = false;
            skipChallenge.SetActive(false);
            StopCoroutine("Co_FuelTimeout");
            StopCoroutine("Co_OrderTimeout");
        }

        public void OnChallengeFailed()
        {
            --step;
            InputManager.Instance.enabled = false;
            StopCoroutine("Co_FuelTimeout");
            StopCoroutine("Co_OrderTimeout");
        }

        [ContextMenu("ShowCongratulations")]
        public async void ShowCongratulations()
        {
            var _response = await OtherAPIs.SetLicenseCompleted_API();
            if (!_response) Debug.Log("Could not get Referral Codes");

            drivingLicence.SetDetails(GameData.Instance.PlayerData.Data.driverDetails, "-");
            TogglePassedScreen(false);
            ToggleFailedScreen(false);
            controls.SetActive(false);
            mainBG.SetActive(true);
            menuBar.Hide();
            congratsObj.SetActive(true);
        }

        public void InvokeTapToContinue(float _time) => Invoke("OnClick_TapToContinue", _time);

        public void ToggleBooster(bool _flag) => boosterObj.SetActive(_flag);


        public void TogglePassedScreen(bool _flag, Action _callback = null)
        {
            if (_flag) menuBar.Hide();
            else menuBar.Show();
            StartCoroutine(ToggleCanvasGroup(passedCG, _flag, 0.5f, 0, _callback));
        }

        public void ToggleFailedScreen(bool _flag, Action _callback = null)
        {
            if (_flag) menuBar.Hide();
            else menuBar.Show();
            StartCoroutine(ToggleCanvasGroup(failedCG, _flag, 0.5f, 0, _callback));
        }



        private IEnumerator ToggleCanvasGroup(CanvasGroup _canvasGroup, bool _flag, float _duration, float _delay, Action _callback = null)
        {
            yield return new WaitForSeconds(_delay);

            TweenHandler.CanvasGroupAlpha(_canvasGroup, Convert.ToInt16(_flag), _duration, DG.Tweening.Ease.OutExpo, () =>
            {
                _callback?.Invoke();
            });
        }







    }

}