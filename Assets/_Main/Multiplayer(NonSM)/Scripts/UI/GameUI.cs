using System;
using System.Collections;
using DG.Tweening;
using FoodFury;
using OneRare.FoodFury.Multiplayer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OneRare.FoodFury.Multiplayer
{
    public class InGameUIHUD : MonoBehaviour
    {
        [SerializeField] private CanvasGroup fader;
        [SerializeField] private Animator introAnimator;
        [SerializeField] private Animator countdownAnimator;
        [SerializeField] private Text coinCount;
        [SerializeField] private Text raceTimeText;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private Image healthFillerImage;
        [SerializeField] private Image fuelFillImage;
        [SerializeField] private TextMeshProUGUI fuelTMP;
        [SerializeField] private Text playerNameText;
        [SerializeField] private RawImage playerIcon;
        [SerializeField] private TextMeshProUGUI boostTimeText;
        [SerializeField] private Transform speedNeedle;
        [SerializeField] private GameObject odometerPanel;
        [SerializeField] private GameObject boosterUIPanel;
        [SerializeField] private SplashUIItem splashUIItemPrefab;
        [SerializeField] private EndRaceUI endRaceUIPrefab;
        [SerializeField] private Image ketchupHitImage;
        [SerializeField] private Image submissileHitImage;
        [SerializeField] private Image oilspillHitImage;
        //[SerializeField] private Image timeFillerImage;
        private bool _startedCountdown;

        private Player player;
        private string playerName;
        private Texture playerProfilePic;
        private SplashUIItem splashUIItem;
        private EndRaceUI endRaceUI;
        public void Init(Player player)
        {

            player.OnOrderCountChanged += count =>
            {
                //AudioManager.Play("coinSFX", AudioManager.MixerTarget.SFX);
                coinCount.text = $"{count:00}";
            };
            player.OnBoosterTimeChanged += time =>
            {
                var timeSpan = TimeSpan.FromSeconds(time);
                var outPut = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
                boostTimeText.text = outPut;

            };

            this.player = player;
        }

        private Vector3 _needleRotation;
        private void Update()
        {
            if (player == null) return;

            _needleRotation.z = Mathf.Lerp(90, -90, player.playerMovementHandler.odometerSpeed);
            speedNeedle.localRotation = Quaternion.Slerp(speedNeedle.localRotation, Quaternion.Euler(_needleRotation), Time.deltaTime * 3);
        }

        public void ShowOrHideOdometer(bool showPanel)
        {
            odometerPanel.SetActive(showPanel);
        }

        public void ShowOrHideBoostPanel(bool showPanel)
        {
            boosterUIPanel.SetActive(showPanel);
        }
        public void HideIntro()
        {
            introAnimator.SetTrigger("Exit");
        }

        private void FadeIn()
        {
            StartCoroutine(FadeInRoutine());
        }

        private IEnumerator FadeInRoutine()
        {
            float t = 1;
            while (t > 0)
            {
                fader.alpha = 1 - t;
                t -= Time.deltaTime;
                yield return null;
            }
        }

        public void UpdateTime(string time, float fillAmount)
        {
            if (!raceTimeText)
                return;
            raceTimeText.text = time;

            UIManager.Instance.UpdateTime(time, fillAmount);
            /*if (timeFillerImage != null)
			{
				timeFillerImage.fillAmount = fillAmount;
			}*/
        }

        public void UpdateHealthText(int health)
        {
            healthText.text = health.ToString();
            healthFillerImage.fillAmount = health / 100f;
        }

        public void UpdateFuelBar(float current, float initial)
        {
            fuelFillImage.fillAmount = current / (initial * 1f);
            fuelFillImage.color = fuelFillImage.fillAmount < .30f ? ColorManager.Instance.Red : fuelFillImage.fillAmount < .75f ? ColorManager.Instance.Yellow : ColorManager.Instance.LightGreen;
            fuelTMP.text = HelperFunctions.ToTimerString((int)current);
        }

        public void UpdatePlayerNameOnHud(string playerName, Texture2D profilePic)
        {
            this.playerName = playerName;
            playerNameText.text = playerName;
            //playerIcon.
            playerProfilePic = profilePic;
            playerIcon.texture = profilePic;
            //Sprite sprite = Sprite.Create(profilePic, new Rect(0, 0, profilePic.width, profilePic.height), new Vector2(profilePic.width / 2, profilePic.height / 2));
            //playerIcon.sprite = sprite;
        }
        public void ShowSplashPopUpForVictory()
        {
            splashUIItem = Instantiate(splashUIItemPrefab);
            splashUIItem.ShowVictory();
        }
        public void ShowSplashPopUpForEliminated()
        {
            splashUIItem = Instantiate(splashUIItemPrefab);
            splashUIItem.ShowEliminated();
        }
        public void ShowResultScreen(Player player)
        {
            endRaceUI = Instantiate(endRaceUIPrefab);
            endRaceUI.Init();
            endRaceUI.UpdateResultScreen(player, false);
        }

        public void OnWeaponHit(WeaponManager.WeaponInstallationType weaponInstallationType)
        {
            if (weaponInstallationType == WeaponManager.WeaponInstallationType.KETCHUP)
            {
                ShowPopUp(ketchupHitImage.gameObject);
            }
            else if (weaponInstallationType == WeaponManager.WeaponInstallationType.SUBMISSILE)
            {
                ShowPopUp(submissileHitImage.gameObject);
            }
            else if (weaponInstallationType == WeaponManager.WeaponInstallationType.OILSPILL)
            {
                ShowPopUp(oilspillHitImage.gameObject);
            }
            else
            {
             Debug.Log("Weapon Not Installed");   
            }
        }
        
        private Vector3 startScale = new Vector3(0.01f, 0.01f, 0.01f);
        [SerializeField] private float animationDuration = 0.5f;
        private void ShowPopUp(GameObject go)
        {
            go.transform.localScale = startScale;

            // Use DOTween to animate the scale to normal size
            go.transform.DOScale(Vector3.one, animationDuration)
                .SetEase(Ease.OutBack)  // Choose your desired easing function
                .OnComplete(() => 
                {
                    // This is called after the first scale animation completes
                    // Now return to the default scale
                    go.transform.DOScale(startScale, animationDuration)
                        .SetEase(Ease.InBack); // Choose an easing function for the reverse animation
                    go.SetActive(false);
                });
        }
        
        

    }
}