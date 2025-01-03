using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    public class GameplayScreen : MonoBehaviour, IScreen
    {
        public static event Action OnGameReady;
        public static Action<Weapon> OnWeaponButtonActivated;

        [Header("-----Anim")]
        [SerializeField] private GameObject startAnimGo;
        [SerializeField] private Image gameStartSequenceImage;
        [SerializeField] private GameObject confettiAnimGo;
        [SerializeField] private Image confettiSequenceImage;

        [Header("-----Fuel & Health")]
        [SerializeField] private Image fuelFillImage;
        [SerializeField] private TextMeshProUGUI fuelTMP;
        [SerializeField] private Image healthFillImage;
        [SerializeField] private TextMeshProUGUI healthTMP;
        [SerializeField] private Image shieldFillImage;
        [SerializeField] private Image armourIconBG;
        [SerializeField] private Image armourFillImage;

        [Header("-----Objectives & Conditions")]
        [SerializeField] private GameObject orderComplete;
        [SerializeField] private GameObject emojiComplete;
        [SerializeField] private TextMeshProUGUI objectiveTitleTMP;
        [SerializeField] private TextMeshProUGUI deliverTMP;
        [SerializeField] private GameObject objectiveSeparator;
        [SerializeField] private TextMeshProUGUI emojiTMP;
        [SerializeField] private Image emojiIcon;
        private GameObject objectiveDeliverParent;
        private GameObject objectiveEmojiParent;

        [Space(20)]
        [SerializeField] private GameObject conditionSeparator;
        [SerializeField] private TextMeshProUGUI conditionHealthTMP;
        [SerializeField] private TextMeshProUGUI conditionTimeTMP;
        [SerializeField] private GameObject conditionObj;
        private RectTransform conditionRect;
        private GameObject conditionHealthParent;
        private GameObject conditionTimeParent;
        [SerializeField] private LevelDataSO.Level Test_level;


        [Header("-----Points")]
        [SerializeField] private TextMeshProUGUI pointsTMP;
        [SerializeField] private PlayerStatAnim playerStatAnimPrefab;


        [Header("-----Order")]
        [SerializeField] public OrderStatus orderStatus;
        [SerializeField] private TextMeshProUGUI dishNameTMP;
        [SerializeField] private TextMeshProUGUI dishLevelTMP;
        [SerializeField] private TextMeshProUGUI orderTimeoutTMP;
        [SerializeField] private RawImage orderImage;
        [SerializeField] private Image timeoutFillImage;
        [SerializeField] private Image dishNameImage;
        [SerializeField] private Transform nftBag;
        private GameObject orderTimeout;
        private Image dishLevel;


        [Header("-----Booster")]
        [SerializeField] private GameObject boosterParent;
        [SerializeField] private Image boosterIcon;
        [SerializeField] private TextMeshProUGUI boosterTMP;
        [SerializeField] private int activeBoosterTimeout;


        [Header("-----Weapon")]
        [SerializeField] private AudioButton weaponButton;
        [SerializeField] private Image weaponButtonIcon;
        [SerializeField] private Image weaponButtonCircle;
        [SerializeField] private SlotManager slotManager;
        [SerializeField] private GameObject ketchupStains;
        [SerializeField] private Sprite ChestWeaponSpriteCircle;


        [Header("-----Other")]
        [SerializeField] private UIPlayerControls playerControls;
        [SerializeField] private Transform speedNeedle;
        [SerializeField] private Image zoomInOutImage;



        private CanvasGroup canvasGroup;

        public static GameplayScreen Instance { get; private set; }
        void Awake()
        {
            Instance = this;
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;

            conditionRect = conditionObj.GetComponent<RectTransform>();
            objectiveDeliverParent = deliverTMP.transform.parent.gameObject;
            objectiveEmojiParent = emojiTMP.transform.parent.gameObject;

            conditionHealthParent = conditionHealthTMP.transform.parent.gameObject;
            conditionTimeParent = conditionTimeTMP.transform.parent.gameObject;
            orderTimeout = orderTimeoutTMP.transform.parent.gameObject;
            dishLevel = dishLevelTMP.GetComponentInParent<Image>();
        }

        private void OnEnable()
        {
            GameController.OnOrderComplete += OnOrderComplete;
            OrderManager.OnNewOrderGenerated += OnNewOrderGenerated;
            Order.OnOrderFailed += OnOrderFailed;
            Weapon.OnWeaponTriggered += OnWeaponTriggered;
            WeaponItem.OnWeaponCollected += OnWeaponCollected;
            Ketchup.OnKetchupTouchedPlayer += OnKetchupTouchedPlayer;
            GameController.OnLevelStart += OnLevelStartSetup;
            weaponButton.onClick.AddListener(OnWeaponButtonClicked);
        }


        private void OnDisable()
        {
            GameController.OnOrderComplete -= OnOrderComplete;
            OrderManager.OnNewOrderGenerated -= OnNewOrderGenerated;
            Order.OnOrderFailed -= OnOrderFailed;
            Weapon.OnWeaponTriggered -= OnWeaponTriggered;
            WeaponItem.OnWeaponCollected -= OnWeaponCollected;
            Ketchup.OnKetchupTouchedPlayer -= OnKetchupTouchedPlayer;
            GameController.OnLevelStart -= OnLevelStartSetup;
        }


        private Vector3 _needleRotation;
        private void Update()
        {
            if (GameController.Instance == null || GameController.Instance.Rider == null) return;

            _needleRotation.z = Mathf.Lerp(90, -90, GameController.Instance.Rider.Vehicle.CurrentSpeed01);
            speedNeedle.localRotation = Quaternion.Slerp(speedNeedle.localRotation, Quaternion.Euler(_needleRotation), Time.deltaTime * 3);
        }

        public void SetupRiderEvents(Rider _rider, bool _flag)
        {
            if (_flag)
            {
                _rider.Vehicle.VehicleHealth.OnHealthChanged += OnHealthChanged;
                _rider.Vehicle.VehicleHealth.OnArmourChanged += OnArmourChanged;
                _rider.Vehicle.VehicleFuel.OnFuelChanged += OnFuelChanged;
                _rider.OnBoosterCollected += OnBoosterCollected;
                _rider.OnBoosterEnd += OnBoosterEnd;
            }
            else
            {
                if (_rider == null) return;
                _rider.Vehicle.VehicleHealth.OnHealthChanged -= OnHealthChanged;
                _rider.Vehicle.VehicleHealth.OnArmourChanged += OnArmourChanged;
                _rider.Vehicle.VehicleFuel.OnFuelChanged -= OnFuelChanged;
                _rider.OnBoosterCollected -= OnBoosterCollected;
                _rider.OnBoosterEnd -= OnBoosterEnd;
            }
        }



        #region Events
        public void OnKetchupTouchedPlayer() => ketchupStains.SetActive(true);

        private void OnWeaponTriggered(WeaponType obj, Enums.RiderType _riderType)
        {
            if (_riderType == Enums.RiderType.Player)
                weaponButton.gameObject.SetActive(false);
        }

        public async void OnWeaponCollected(Weapon weapon, bool isChest)
        {
            weaponButton.gameObject.SetActive(true);
            if (isChest)
            {
                slotManager.gameObject.SetActive(true);
                weaponButtonIcon.gameObject.SetActive(false);
                weaponButtonCircle.sprite = ChestWeaponSpriteCircle;
                await slotManager.SpinAsync(weapon.WeaponType, () =>
                {
                    AnalyticsManager.Instance.FireWeaponCollectedEvent(weapon.WeaponType.ToString());
                    OnWeaponButtonActivated?.Invoke(weapon);
                });
            }
            else
            {
                slotManager.gameObject.SetActive(false);
                weaponButtonIcon.gameObject.SetActive(true);
                weaponButtonIcon.sprite = weapon.WeaponIcon;
                weaponButtonCircle.sprite = weapon.WeaponCircle;
                AnalyticsManager.Instance.FireWeaponCollectedEvent(weapon.WeaponType.ToString());
                OnWeaponButtonActivated?.Invoke(weapon);
            }
        }


        public void OnHealthChanged(int _current, int _initial)
        {
            healthFillImage.fillAmount = _current / (GameData.Instance.GameSettings.defaultEngineHealth * 1f);
            healthFillImage.color = healthFillImage.fillAmount < .30f ? ColorManager.Instance.Red : healthFillImage.fillAmount < .75f ? ColorManager.Instance.Yellow : ColorManager.Instance.LightGreen;
            healthTMP.text = $"{_current}";

            float _shieldValue = GameData.Instance.GameSettings.defaultEngineHealth - _initial;
            shieldFillImage.fillAmount = _current > 100 ? (100 - _current) / _shieldValue : 0;
        }

        public void OnArmourChanged(int _current, int _initial)
        {
            armourIconBG.color = armourFillImage.color = _current > 0 ? ColorManager.Instance.Orange : ColorManager.Instance.LightGrey;
            armourFillImage.fillAmount = _current / (_initial * 1f);
        }

        public void OnFuelChanged(float _current, float _initial)
        {
            GameData.Instance.PlayerData.Data.fuel = (int)_current;
            fuelFillImage.fillAmount = _current / (_initial * 1f);
            fuelFillImage.color = fuelFillImage.fillAmount < .30f ? ColorManager.Instance.Red : fuelFillImage.fillAmount < .75f ? ColorManager.Instance.Yellow : ColorManager.Instance.LightGreen;
            fuelTMP.text = HelperFunctions.ToTimerString((int)_current);
        }


        private void OnNewOrderGenerated(Order _order)
        {
            if (_order.IsNFTOrder)
            {
                dishLevelTMP.text = "L" + GameData.Instance.GetUserDishByToken(_order.NFTOrderTokenId).level;
                dishLevel.color = dishNameImage.color = ColorManager.Instance.DarkGreen;
            }
            else
            {
                dishLevelTMP.text = string.Empty;
                dishLevel.color = ColorManager.Instance.Red;
                dishNameImage.color = Color.white;
            }

            orderImage.texture = _order.Thumbnail;
            dishNameTMP.text = _order.Dish.name;
            timeoutFillImage.fillAmount = 1;
            timeoutFillImage.color = ColorManager.Instance.DarkGreen;
            orderTimeoutTMP.text = _order.RemainingTime.ToString();
            orderTimeout.SetActive(true);

            _order.OnOrderTimeUpdate += OnOrderTimeUpdate;
            _order.OnOrderDeliveringTimeUpdate += orderStatus.ShowDeliveringStatus;

            orderStatus.ShowOrderStatus(Enums.OrderStatus.NewOrder, _order.OrderNFTDetails, (Texture2D)orderImage.texture, _initialDelay: 2, _autoHideDelay: 3);
        }

        private void OnOrderTimeUpdate(int _time)
        {
            orderTimeoutTMP.text = _time.ToString();
            timeoutFillImage.fillAmount = _time / (GameController.Instance.LevelInfo.CurrentLevel.orderTime * 1f);
            timeoutFillImage.color = timeoutFillImage.fillAmount < 0.33f ? ColorManager.Instance.Red : timeoutFillImage.fillAmount < 0.66 ? ColorManager.Instance.Yellow : ColorManager.Instance.DarkGreen;
        }

        private async void OnOrderComplete(Order _order, bool _isLevelComplete)
        {
            orderTimeout.SetActive(false);
            var _success = await PlayerAPIs.UpdatePlayerDataOrder_API(_order);

            Debug.Log($"Success - {_isLevelComplete}");
            orderStatus.ShowOrderStatus(Enums.OrderStatus.Delivered, _initialDelay: 0, _autoHideDelay: 1);

            StartCoroutine(Co_OrderChipsAndEmojiAnim(_order));
            if (_order.IsNFTOrder) StartCoroutine("Co_PlayConfettiAnim");
            UpdateObjective();

            _order.OnOrderTimeUpdate -= OnOrderTimeUpdate;
            _order.OnOrderDeliveringTimeUpdate -= orderStatus.ShowDeliveringStatus;

            if (!_isLevelComplete) OrderManager.Instance.SpawnOrder();

        }

        private void OnOrderFailed(Order _order)
        {
            orderTimeout.SetActive(false);
            _order.OnOrderTimeUpdate -= OnOrderTimeUpdate;
            _order.OnOrderDeliveringTimeUpdate -= orderStatus.ShowDeliveringStatus;

            orderStatus.ShowOrderStatus(Enums.OrderStatus.MissedIt, _initialDelay: 0, _autoHideDelay: 1);
            OrderManager.Instance.SpawnOrder();

        }


        private void OnBoosterCollected(ModelClass.BoosterData _booster)
        {
            if (_booster.type == Enums.BoosterType.Speed)
            {
                boosterIcon.sprite = SpriteManager.Instance.speedBooster;
                boosterParent.SetActive(true);
                //StartBoosterTimer(_booster.duration, () => { SetInitialBooster(GameController.Instance.InitialBooster); });
            }
        }

        private void OnBoosterEnd(Enums.BoosterType obj) => SetInitialBooster(GameController.Instance.LevelInfo.LevelBooster);
        #endregion


        private void OnWeaponButtonClicked()
        {
            InputManager.OnShootButtonPressed?.Invoke();
            weaponButton.gameObject.SetActive(false);
        }

        private void OnLevelStartSetup() => weaponButton.gameObject.SetActive(false);

        private void SetChips(float _value) => pointsTMP.text = _value.ToString();

        #region ------------------------------------------------------------------------------------------ Objective & Conditions
        [ContextMenu("SetObjective")]
        private void SetObjective_Debug() => SetObjectivesAndConditions(Test_level);
        private void SetObjectivesAndConditions(LevelDataSO.Level _level)
        {
            // Objective
            objectiveSeparator.SetActive(_level.objectives.Count > 1);
            objectiveTitleTMP.gameObject.SetActive(_level.objectives.Count == 1);
            if (_level.TryGetObjective(Enums.Objective.DeliverOrders, out int _value))
            {
                deliverTMP.text = $"{0}/{_value}";
                objectiveTitleTMP.text = "Deliver";
                objectiveDeliverParent.SetActive(true);
            }
            else objectiveDeliverParent.SetActive(false);

            if (_level.TryGetObjective(Enums.Objective.CollectGreenEmoji, out _value))
            {
                emojiTMP.text = $"{0}/{_value}";
                emojiIcon.sprite = SpriteManager.Instance.greenEmoji;
                objectiveTitleTMP.text = "Collect";
                objectiveEmojiParent.SetActive(true);
            }
            else if (_level.TryGetObjective(Enums.Objective.CollectYellowEmoji, out _value))
            {
                emojiTMP.text = $"{0}/{_value}";
                emojiIcon.sprite = SpriteManager.Instance.yellowEmoji;
                objectiveTitleTMP.text = "Collect";
                objectiveEmojiParent.SetActive(true);
            }
            else if (_level.TryGetObjective(Enums.Objective.CollectRedEmoji, out _value))
            {
                emojiTMP.text = $"{0}/{_value}";
                emojiIcon.sprite = SpriteManager.Instance.redEmoji;
                objectiveTitleTMP.text = "Collect";
                objectiveEmojiParent.SetActive(true);
            }
            else objectiveEmojiParent.SetActive(false);


            // Condition
            conditionObj.SetActive(_level.conditions.Count > 0);
            conditionSeparator.SetActive(_level.conditions.Count > 1);
            conditionRect.sizeDelta = _level.conditions.Count > 1 ? new Vector2(330, 50) : new Vector2(160, 50);
            if (_level.TryGetCondition(Enums.ObjectiveCondition.MinHealth, out _value))
            {
                conditionHealthTMP.text = $">  {_value}";
                conditionHealthParent.SetActive(true);
            }
            else conditionHealthParent.SetActive(false);

            if (_level.TryGetCondition(Enums.ObjectiveCondition.InTime, out _value))
            {
                conditionTimeTMP.text = $"IN {HelperFunctions.ToTimerString(_value)}";
                conditionTimeParent.SetActive(true);
            }
            else conditionTimeParent.SetActive(false);


            orderComplete.SetActive(false);
            emojiComplete.SetActive(false);
        }

        private void UpdateObjective()
        {
            if (GameController.Instance.LevelInfo.CurrentLevel.TryGetObjective(Enums.Objective.DeliverOrders, out int _value))
            {
                orderComplete.SetActive(GameController.Instance.LevelInfo.TotalDeliveries >= _value);
                deliverTMP.text = $"{GameController.Instance.LevelInfo.TotalDeliveries}/{_value}";
            }


            if (GameController.Instance.LevelInfo.CurrentLevel.TryGetObjective(Enums.Objective.CollectGreenEmoji, out _value))
            {
                emojiTMP.text = $"{GameController.Instance.LevelInfo.GreenEmojis}/{_value}";
                emojiComplete.SetActive(GameController.Instance.LevelInfo.GreenEmojis >= _value);
            }
            else if (GameController.Instance.LevelInfo.CurrentLevel.TryGetObjective(Enums.Objective.CollectYellowEmoji, out _value))
            {
                int _collected = GameController.Instance.LevelInfo.GreenEmojis + GameController.Instance.LevelInfo.YellowEmojis;
                emojiTMP.text = $"{_collected}/{_value}";
                emojiComplete.SetActive(_collected >= _value);
            }
            else if (GameController.Instance.LevelInfo.CurrentLevel.TryGetObjective(Enums.Objective.CollectRedEmoji, out _value))
            {
                int _collected = GameController.Instance.LevelInfo.GreenEmojis + GameController.Instance.LevelInfo.YellowEmojis + GameController.Instance.LevelInfo.RedEmojis;
                emojiTMP.text = $"{_collected}/{_value}";
                emojiComplete.SetActive(_collected >= _value);
            }
        }

        public void UpdateObjectiveConditionTimer(int _value) => conditionTimeTMP.text = $"IN {HelperFunctions.ToTimerString(_value)}";
        #endregion


        #region ------------------------------------------------------------------------------------------ Booster
        Action OnBoosterEndCallback;
        public void SetInitialBooster(Enums.LevelBoosterType _type)
        {
            boosterTMP.text = _type.ToString();
            switch (_type)
            {
                case Enums.LevelBoosterType.Speed:
                    boosterIcon.sprite = SpriteManager.Instance.speedBooster;
                    boosterParent.SetActive(true);
                    break;

                case Enums.LevelBoosterType.Shield:
                    boosterIcon.sprite = SpriteManager.Instance.shieldBooster;
                    boosterParent.SetActive(true);
                    break;

                case Enums.LevelBoosterType.NoRival:
                    boosterIcon.sprite = SpriteManager.Instance.noBotBooster;
                    boosterParent.SetActive(true);
                    break;

                case Enums.LevelBoosterType.None:
                    boosterIcon.sprite = null;
                    boosterParent.SetActive(false);
                    break;
            }
        }

        public void StartBoosterTimer(int _duration, Action _onEnd)
        {
            boosterTMP.text = HelperFunctions.ToTimerString(_duration);
            activeBoosterTimeout = _duration;
            OnBoosterEndCallback = _onEnd;
            StartCoroutine("Co_StartBoosterTimer");
        }

        private WaitForSeconds waitFor1Sec = new WaitForSeconds(1f);
        private IEnumerator Co_StartBoosterTimer()
        {
            while (activeBoosterTimeout > 0)
            {
                yield return waitFor1Sec;
                if (!GameController.IsGamePaused) boosterTMP.text = HelperFunctions.ToTimerString(--activeBoosterTimeout);
            }

            OnBoosterEndCallback?.Invoke();

            yield return waitFor01Sec;
            OnBoosterEndCallback = null;
        }

        private void StopBoosterTimer()
        {
            activeBoosterTimeout = 0;
            OnBoosterEndCallback?.Invoke();
            OnBoosterEndCallback = null;
            StopCoroutine("Co_StartBoosterTimer");
        }
        #endregion



        #region ------------------------------------------------------------------------------------------ Animations
        //private WaitForSeconds waitFor04Sec = new WaitForSeconds(0.04f);
        IEnumerator Co_StartCountdownAnim()
        {
            startAnimGo.SetActive(true);
            gameStartSequenceImage.sprite = SpriteManager.Instance.startAnimSeq[0];

            int _length = SpriteManager.Instance.startAnimSeq.Length;
            for (int i = 0; i < _length; i++)
            {
                gameStartSequenceImage.sprite = SpriteManager.Instance.startAnimSeq[i];
                yield return waitFor03Sec;
            }
            startAnimGo.SetActive(false);

            GameController.Instance.StartGame();
            StopBoosterTimer();
        }

        private WaitForSeconds waitFor03Sec = new WaitForSeconds(0.03f);
        IEnumerator Co_PlayConfettiAnim()
        {
            confettiAnimGo.SetActive(true);
            confettiSequenceImage.sprite = SpriteManager.Instance.confettiAnimSeq[0];

            int _length = SpriteManager.Instance.confettiAnimSeq.Length;
            for (int i = 0; i < _length; i++)
            {
                confettiSequenceImage.sprite = SpriteManager.Instance.confettiAnimSeq[i];
                yield return waitFor03Sec;
            }
            confettiAnimGo.SetActive(false);
        }

        private WaitForSeconds waitFor01Sec = new WaitForSeconds(0.1f);
        private WaitForSeconds waitFor015Sec = new WaitForSeconds(0.015f);
        private IEnumerator Co_OrderChipsAndEmojiAnim(Order _order)
        {
            // Chip Animation
            for (int i = 0; i < 3; i++)
            {
                PlayerStatAnim _p = Instantiate(playerStatAnimPrefab, transform);
                _p.Init(SpriteManager.Instance.chips, ScreenManager.Instance.TweenDuration, pointsTMP.transform.parent);

                if (_order.IsNFTOrder)
                {
                    // NFT Chip
                    PlayerStatAnim _p2 = Instantiate(playerStatAnimPrefab, nftBag);
                    _p2.Init(SpriteManager.Instance.chips, ScreenManager.Instance.TweenDuration, pointsTMP.transform.parent);
                }

                yield return waitFor01Sec;
            }


            // Emoji Animation
            float _interval = _order.RemainingTime / (GameController.Instance.LevelInfo.CurrentLevel.orderTime * 1f);
            Sprite _emoji = _interval < 0.33f ? SpriteManager.Instance.redEmoji : _interval < 0.66 ? SpriteManager.Instance.yellowEmoji : SpriteManager.Instance.greenEmoji;
            Transform _parent = transform;
            PlayerStatAnim _p3 = Instantiate(playerStatAnimPrefab, transform);
            if (GameController.Instance.LevelInfo.CurrentLevel.TryGetObjective(Enums.Objective.CollectGreenEmoji, out int _value))
            {
                if (_interval > 0.66) _parent = emojiIcon.transform;
            }
            else if (GameController.Instance.LevelInfo.CurrentLevel.TryGetObjective(Enums.Objective.CollectYellowEmoji, out _value))
            {
                if (_interval >= 0.33) _parent = emojiIcon.transform;
            }
            else if (GameController.Instance.LevelInfo.CurrentLevel.TryGetObjective(Enums.Objective.CollectRedEmoji, out _value))
            {
                if (_interval > 0) _parent = emojiIcon.transform;
            }
            _p3.Init(_emoji, ScreenManager.Instance.TweenDuration * 2, _parent);
            // _p3.transform.localPosition += Vector3.right * -100f;

            // Amount Animation
            yield return new WaitForSeconds(0.5f);
            int _earnedChips = GameController.Instance.CalculateChips(_order);
            int _points = GameController.Instance.LevelInfo.Chips - _earnedChips;
            for (int i = 0; i < _earnedChips; i++)
            {
                yield return waitFor015Sec;
                SetChips(++_points);
            }
        }
        #endregion



        public void OnClick_Pause(PopupBehaviour _popup) => PopUpManager.Instance.ShowPopup(_popup);
        public void OnClick_ZoomInOut()
        {
            Camera _main = Camera.main;

            if (_main.orthographicSize > 15)
            {
                zoomInOutImage.GetComponentInParent<Button>().interactable = false;
                //while (_main.orthographicSize > 15) _main.orthographicSize -= 0.01f;
                _main.orthographicSize = 15;
                zoomInOutImage.sprite = SpriteManager.Instance.minus;
                zoomInOutImage.GetComponentInParent<Button>().interactable = true;
            }
            else
            {
                zoomInOutImage.GetComponentInParent<Button>().interactable = false;
                //while (_main.orthographicSize < 20) _main.orthographicSize += 0.01f;
                _main.orthographicSize = 20;
                zoomInOutImage.sprite = SpriteManager.Instance.plus;
                zoomInOutImage.GetComponentInParent<Button>().interactable = true;
            }
        }


        public void ToggleUI(bool _show)
        {
            canvasGroup.alpha = _show ? 1 : 0;
            canvasGroup.blocksRaycasts = _show ? true : false;
        }



        private void InputSetup()
        {
            playerControls.SwitchInput(GameData.Instance.Platform);
            //playerControls.SwitchControl(GameData.Instance.Controls);
        }

        private void Init()
        {
            InputSetup();
            MenuBar.Instance.Hide();
            startAnimGo.SetActive(false);
            weaponButton.gameObject.SetActive(false);
            SetChips(GameController.Instance.LevelInfo.Chips);
            SetObjectivesAndConditions(GameController.Instance.LevelInfo.CurrentLevel);

            //ScreenManager.Instance.LoadingBG.SetActive(false);

            OrderManager.Instance.SpawnOrder();
            //WeaponManager.Instance.SpawnStartingWeapons();
            //ChompSpawnManager.Instance.CheckAndSpawnChomp();

            if (GameController.Instance.LevelInfo.CurrentLevel.boosterDetails.Count > 0) BoosterManager.Instance.SpawnBooster();
            //if (GameController.Instance.LevelInfo.CurrentLevel.obstacleDetails.varients.Count > 0) ObstacleManager.Instance.SpawnObstacle();

            OnGameReady?.Invoke();
            StartCoroutine("Co_StartCountdownAnim");
        }


        public void Show(Action _callback = null)
        {
            Init();
            TweenHandler.CanvasGroupAlpha(canvasGroup, 1, ScreenManager.Instance.TweenDuration, ScreenManager.Instance.TweenEase, () =>
            {
                canvasGroup.blocksRaycasts = true;
                ScreenManager.Instance.CurrentScreen = Enums.Screen.Gameplay;
                Loader.Instance.HideLoader();
                _callback?.Invoke();

            });
        }

        public void Hide(Action _callback = null)
        {
            StopBoosterTimer();
            Loader.Instance.ShowLoader();
            startAnimGo.SetActive(false);
            canvasGroup.blocksRaycasts = false;
            TweenHandler.CanvasGroupAlpha(canvasGroup, 0, ScreenManager.Instance.TweenDuration, ScreenManager.Instance.TweenEase, () =>
            {
                _callback?.Invoke();
            });
        }


        private void OnApplicationQuit()
        {
            StopCoroutine("Co_StartBoosterTimer");
            StopCoroutine("Co_StartCountdownAnim");
            StopCoroutine("Co_PlayConfettiAnim");
            StopAllCoroutines();
        }
    }
}
