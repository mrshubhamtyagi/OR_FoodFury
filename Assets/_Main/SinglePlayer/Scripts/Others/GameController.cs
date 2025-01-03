using System;
using System.Collections;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace FoodFury
{
    public class GameController : MonoBehaviour
    {
        public static event Action OnLevelFailed;
        public static event Action OnLevelComplete;
        public static event Action<Order, bool> OnOrderComplete;
        public static event Action OnLevelStart;

        public static bool IsGamePaused;
        [field: SerializeField] public ModelClass.LevelInfo LevelInfo { get; private set; }

        [Header("-----Managers & Controllers")]
        [SerializeField] private GameObject inputManagerPrefab;
        [SerializeField] private GameObject orderManagerPrefab;
        [SerializeField] private GameObject boosterManagerPrefab;
        [SerializeField] private GameObject obstacleManagerPrefab;
        [SerializeField] private GameObject weaponManagerPrefab;
        [SerializeField] private GameObject mascotSpawnManagerPrefab;

        [Header("-----Riders")]
        [SerializeField] private GameObject playerCameraPrefab;
        [SerializeField] private GameObject playerLocalPrefab;
        [SerializeField] private GameObject rivalLocalPrefab;
        public Rider Rider { get; private set; }
        public RiderAIRival RiderRival { get; private set; }
        public SpawnPositions SpawnPositions { get; private set; }
        public CameraController cameraController;



        private Camera uiCamera;

        private WaitForSeconds waitFor1Sec = new WaitForSeconds(1);

        [field: SerializeField] public ModelClass.CalculationData CalculationData { get; private set; }
        private int totalChipsEarnedByOrderDelivering;

        public static GameController Instance;
        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
            uiCamera = Camera.main;
            CalculationData = new();
            totalChipsEarnedByOrderDelivering = 0;
            //print(JsonUtility.ToJson(calculationData));
        }


        private async void Start()
        {
            var _response = await APIManager.GetCalculationDataAsync();
            if (_response.error)
            {
                PopUpManager.Instance.ShowWarningPopup($"Something went wrong! [{_response}]");
                return;
            }

            ModelClass.ErrorAndResultResponse<ModelClass.CalculationData> _data = JsonConvert.DeserializeObject<ModelClass.ErrorAndResultResponse<ModelClass.CalculationData>>(_response.result);
            if (_data.error) PopUpManager.Instance.ShowWarningPopup(_data.result.ToString());
            else CalculationData = _data.result;
        }

        private void OnEnable()
        {
            OnLevelFailed += FireLevelFailedAnalytics;
            OnLevelComplete += FireLevelCompleteAnalytics;
            GameData.OnOrientationUpdated += CheckOrientation;
        }



        private void OnDisable()
        {
            OnLevelFailed -= FireLevelFailedAnalytics;
            OnLevelComplete -= FireLevelCompleteAnalytics;
            GameData.OnOrientationUpdated -= CheckOrientation;
        }
        private void CheckOrientation(Enums.Orientation obj)
        {
            if (obj == Enums.Orientation.Portrait && ScreenManager.Instance.CurrentScreen == Enums.Screen.Gameplay)
            {
                PopUpManager.Instance.ShowPopup(PopUpManager.Instance.SettingsPopup);
            }
        }
        private void FireLevelFailedAnalytics()
        {
            Debug.Log("Analitics level failed fired");
            AnalyticsManager.Instance.FireLevelFailedEvent(GameData.Instance.SelectedLevelNumber, GameData.Instance.GetSelectedMapData().mapName, LevelInfo.TotalDeliveries, LevelInfo.Munches, LevelInfo.Chips, LevelInfo.NFTDeliveries, Rider.Vehicle.VehicleHealth.CurrentHealth, DateTimeOffset.UtcNow.ToUnixTimeSeconds() - LevelInfo.StartTime, LevelInfo.GreenEmojis, LevelInfo.YellowEmojis, LevelInfo.RedEmojis, GameData.Instance.SelectedLevelNumber < GameData.Instance.GetPlayerLevelNumberData().levelNumber);
        }

        private void FireLevelCompleteAnalytics()
        {
            Debug.Log("Analitics level complete fired");
            AnalyticsManager.Instance.FireLevelCompleteEvent(GameData.Instance.SelectedLevelNumber, GameData.Instance.GetSelectedMapData().mapName, LevelInfo.TotalDeliveries, LevelInfo.Munches, LevelInfo.Chips, LevelInfo.NFTDeliveries, Rider.Vehicle.VehicleHealth.CurrentHealth, DateTimeOffset.UtcNow.ToUnixTimeSeconds() - LevelInfo.StartTime, LevelInfo.GreenEmojis, LevelInfo.YellowEmojis, LevelInfo.RedEmojis, GameData.Instance.SelectedLevelNumber < GameData.Instance.GetPlayerLevelNumberData().levelNumber);
            AnalyticsManager.Instance.FireMunchesEarnedEvent(Enums.SourceType.LevelComplete, LevelInfo.Munches);
            if (LevelInfo.Chips > totalChipsEarnedByOrderDelivering)
            {
                AnalyticsManager.Instance.FireChipsEarnedEvent(Enums.SourceType.LevelComplete, LevelInfo.Chips - totalChipsEarnedByOrderDelivering);
            }
            else
            {
                Debug.Log("Both the total chips and level complete have same chips");
            }
        }

        //private float GetObjectivePercentage() => LevelInfo.RemainingObjectiveValue / (float)LevelInfo.CurrentLevel.objectiveValue;


        #region Events
        private void OnHealthChanged(int _currenthealth, int _defaultHealth)
        {
            if (LevelInfo.CurrentLevel.TryGetCondition(Enums.ObjectiveCondition.MinHealth, out int _value) && _currenthealth < _value)
            {
                OnLevelFailed?.Invoke();
                if (ScreenManager.Instance.CurrentScreen == Enums.Screen.Gameplay)
                    PopUpManager.Instance.ShowPopup(PopUpManager.Instance.LevelFailedPopup);
            }
        }

        private void OnHealthFinished() => PopUpManager.Instance.ShowPopup(PopUpManager.Instance.LevelFailedPopup);
        private void OnFuelFinished() => PopUpManager.Instance.ShowPopup(PopUpManager.Instance.GameEndPopup);

        private void OnOrderCollected(Order _order)
        {
            UpdateLevelStats(_order);
            int chipsEarned = CalculateChips(_order);
            totalChipsEarnedByOrderDelivering += chipsEarned;
            AnalyticsManager.Instance.FireChipsEarnedEvent(Enums.SourceType.OrderDelivered, chipsEarned);
            AnalyticsManager.Instance.FireOrderDeliveredEvent(LevelInfo.CurrentLevel.level, GameData.Instance.GetSelectedMapData().mapName, _order.IsNFTOrder, _order.name, _order.Dish.tokenId, _order.Dish.level, chipsEarned);
            bool _primaryObjective = LevelInfo.IsPrimaryObjectiveCompleted();
            bool _secondaryObjective = LevelInfo.IsSecondaryObjectiveCompleted();
            OnOrderComplete?.Invoke(_order, _primaryObjective && _secondaryObjective);


            if (_primaryObjective && !_secondaryObjective) // Level Failed
            {
                OnLevelFailed?.Invoke();
                PopUpManager.Instance.ShowPopup(PopUpManager.Instance.LevelFailedPopup);
            }
            else if (_primaryObjective && _secondaryObjective) // Level Complete
            {
                StopCoroutine("Co_StartObjectiveTime");

                UpdateHealthMultiplier(Rider.Vehicle.VehicleHealth.CurrentHealth);
                CalculateStarValue();

                LevelInfo.Munches = GetTotalMunches();


                OnLevelCompleteSetup();
            }
            else if (_primaryObjective)
            {
                OnLevelFailed?.Invoke();
                PopUpManager.Instance.ShowPopup(PopUpManager.Instance.LevelFailedPopup);
            }


            void UpdateLevelStats(Order _order)
            {
                // Chips and Munches
                LevelInfo.Chips += CalculateChips(_order);


                // Deliveries
                if (_order.IsNFTOrder) LevelInfo.NFTDeliveries++;
                LevelInfo.TotalDeliveries++;
                LevelInfo.TotalOrdersList.Add(_order.Dish.tokenId);

                // Emojis
                float _interval = LevelInfo.CurrentLevel.orderTime / 3f;
                if (_order.RemainingTime >= _interval * 2) LevelInfo.GreenEmojis++;
                else if (_order.RemainingTime >= _interval) LevelInfo.YellowEmojis++;
                else LevelInfo.RedEmojis++;

                // Score
                UpdateTimeMultiplier(LevelInfo.CurrentLevel.orderTime - _order.RemainingTime);
                UpdateNFTAsWellAsNonNFTScore(_order);
            }
        }


        private async void OnLevelCompleteSetup()
        {
            OnLevelComplete?.Invoke();
            ModelClass.Analytics _analytics = new()
            {
                level = GameData.Instance.SelectedLevelNumber,
                chips = LevelInfo.Chips,
                munches = LevelInfo.Munches,
                stars = LevelInfo.StarValue,
                time = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - LevelInfo.StartTime,
                totalDeliveries = LevelInfo.TotalDeliveries,
                nftOrders = LevelInfo.NFTDeliveries,
                orders = LevelInfo.TotalOrdersList,
                greenEmojis = LevelInfo.GreenEmojis,
                yellowEmojis = LevelInfo.YellowEmojis,
                redEmojis = LevelInfo.RedEmojis
            };


            int _munches = 0;
            bool _retry = GameData.Instance.SelectedLevelNumber < GameData.Instance.GetPlayerLevelNumberData().levelNumber;
            if (_retry) // Retry
            {
                ModelClass.PlayerLevelStats _level = GameData.Instance.GetCompletedLevelStats(GameData.Instance.SelectedLevelNumber - 1);
                _munches = _analytics.munches > _level.munches ? _analytics.munches - _level.munches : 0;
            }
            else // First time Level
            {
                _munches = _analytics.munches;
            }

            //print($"CurrentLevel - {GameData.Instance.SelectedLevelNumber} | PlayerLevel - {GameData.Instance.SelectedLevelNumber}");
            var _success = await PlayerAPIs.UpdatePlayerDataLevel_API(_munches, GameData.Instance.SelectedMapId, _analytics, _retry);
            if (_success)
            {
                var _result = await OtherAPIs.SaveAnalytics_API(_analytics, _retry);
                PopUpManager.Instance.ShowPopup(PopUpManager.Instance.LevelCompletePopup);
            }
        }
        #endregion



        private IEnumerator Co_StartObjectiveTime()
        {
            while (LevelInfo.LevelTimer > 0)
            {
                yield return waitFor1Sec;
                if (!IsGamePaused)
                {
                    LevelInfo.LevelTimer--;
                    GameplayScreen.Instance.UpdateObjectiveConditionTimer(LevelInfo.LevelTimer);
                }
            }

            OnLevelFailed?.Invoke();
            print("Co_StartObjectiveTime");
            PopUpManager.Instance.ShowPopup(PopUpManager.Instance.LevelFailedPopup);
        }


        // Called after start countdown (3...2...1 countdown)
        public void StartGame()
        {
            // Start Level Timer
            if (LevelInfo.CurrentLevel.TryGetCondition(Enums.ObjectiveCondition.InTime, out LevelInfo.LevelTimer))
                StartCoroutine("Co_StartObjectiveTime");

            LevelInfo.StartTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            IsGamePaused = false;

            Debug.Log("------------------- StartGame");
        }




        public async void StartSetup()
        {
            IsGamePaused = true;
            LevelInfo.ResetStats();

            LevelInfo.CurrentLevel = GameData.Instance.GetLevelDataByIndex(GameData.Instance.SelectedLevelNumber);
            var _success = await DishAPIs.GetUserDishes_API();

            if (_success)
            {
                LevelInfo.FilteredDishData = GameData.Instance.GetFilteredDishes();

                if (LevelInfo.FilteredDishData.Count > 0)
                {
                    SpawnPositions = FindObjectOfType<SpawnPositions>();

                    SpawnManagers();
                    SpawnPlayer(SpawnPositions.transform);
                    SpawnRival(SpawnPositions.transform);

                    uiCamera.gameObject.SetActive(false);

                    TileManager.Instance.rider = Rider;
                    //TileManager.Instance.FindNeighbours();

                    //ScreenManager.Instance.LoadingBG.SetActive(false);
                    Loader.Instance.HideLoader();


                    AudioManager.Instance.PlayGameplayTrack();
                    OnLevelStartSetup();
                }
                else
                {
                    PopUpManager.Instance.ShowWarningPopup("Could not fetch filtered dish data!");
                }
            }


            Debug.Log("------------------- LoadGame");


            void SpawnManagers()
            {
                AddressableLoader.SpawnObject<InputManager>(inputManagerPrefab);
                AddressableLoader.SpawnObject<OrderManager>(orderManagerPrefab);
                AddressableLoader.SpawnObject<BoosterManager>(boosterManagerPrefab);
                AddressableLoader.SpawnObject<WeaponManager>(weaponManagerPrefab);

                //if (GameData.Instance.GameSettings.showMascot)
                //    AddressableLoader.SpawnObject<MascotSpawnManager>(mascotSpawnManagerPrefab);
                //AddressableLoader.SpawnObject<ObstacleManager>(obstacleManagerPrefab);
            }


            void SpawnPlayer(Transform _parent)
            {
                Rider = AddressableLoader.SpawnObject<Rider>(playerLocalPrefab, _parent);
                SetRiderPosition();


                Rider.OnOrderCollected += OnOrderCollected;
                Rider.Vehicle.VehicleHealth.OnHealthChanged += OnHealthChanged;
                Rider.Vehicle.VehicleHealth.OnHealthFinished += OnHealthFinished;
                Rider.Vehicle.VehicleFuel.OnFuelFinished += OnFuelFinished;

                Rider.SetBoosterCampass(false);// CurrentLevel.boosterDetails.Count > 0);
                GameplayScreen.Instance.SetupRiderEvents(Rider, true);

                // Camera
                cameraController = AddressableLoader.SpawnObject<CameraController>(playerCameraPrefab, _parent);
                cameraController.SetRider(Rider);
                cameraController.SetCameraSettings(GameData.Instance.GetSelectedMapData().cameraSettings);

            }

            void SpawnRival(Transform _parent)
            {
                RiderRival = AddressableLoader.SpawnObject<RiderAIRival>(rivalLocalPrefab, _parent);
                SetRivalPosition();
            }
        }

        private void SetRiderPosition()
        {
            int _index = UnityEngine.Random.Range(0, SpawnPositions.Player.Length);
            Rider.transform.SetPositionAndRotation(SpawnPositions.Player[_index].position, Quaternion.Euler(SpawnPositions.Player[_index].rotation));
            Rider.Vehicle.rotationAngleY = SpawnPositions.Player[_index].rotation.y;
            if (cameraController) cameraController.GetRiderPosition();
            SwapElement(_index);


            void SwapElement(int _index)
            {
                var _taken = SpawnPositions.Player[_index];
                SpawnPositions.Player[_index] = SpawnPositions.Player[0];
                SpawnPositions.Player[0] = _taken;
            }
        }

        private void SetRivalPosition()
        {
            int _index = UnityEngine.Random.Range(1, SpawnPositions.Player.Length);
            RiderRival.transform.SetPositionAndRotation(SpawnPositions.Player[_index].position, Quaternion.Euler(SpawnPositions.Player[_index].rotation));
            RiderRival.Vehicle.rotationAngleY = SpawnPositions.Player[_index].rotation.y;
        }


        private void OnLevelStartSetup()
        {
            totalChipsEarnedByOrderDelivering = 0;
            StopCoroutine("Co_StartObjectiveTime");
            OrderManager.Instance.ClearOrders();
            BoosterManager.Instance.ClearBoosters();
            WeaponManager.Instance.ClearWeapons();


            SetRiderPosition();
            SetRivalPosition();

            if (RiderRival != null) RiderRival.gameObject.SetActive(true);

            PopUpManager.Instance.ShowPopup(PopUpManager.Instance.GameStartPopup);
            OnLevelStart?.Invoke();
        }



        public async void SetNextLevel(bool _replay)
        {
            LevelInfo.ResetStats();
            var _success = await DishAPIs.GetUserDishes_API();
            if (_success)
            {
                if (!_replay) // Setup for Next Level 
                {
                    GameData.Instance.SelectedLevelNumber++;
                    LevelInfo.CurrentLevel = GameData.Instance.GetLevelDataByIndex(GameData.Instance.SelectedLevelNumber);
                    LevelInfo.FilteredDishData = GameData.Instance.GetFilteredDishes();

                    if (LevelInfo.FilteredDishData.Count > 0) OnLevelStartSetup();
                    else PopUpManager.Instance.ShowWarningPopup("Could not fetch filtered dish data!");
                }
                else OnLevelStartSetup();
            }
        }


        public void EndSetup()
        {
            Rider.OnOrderCollected -= OnOrderCollected;
            Rider.Vehicle.VehicleHealth.OnHealthChanged -= OnHealthChanged;
            Rider.Vehicle.VehicleHealth.OnHealthFinished -= OnHealthFinished;
            Rider.Vehicle.VehicleFuel.OnFuelFinished -= OnFuelFinished;

            GameplayScreen.Instance.SetupRiderEvents(Rider, false);
            uiCamera.gameObject.SetActive(true);

            InputManager.Instance.DestroyInstance();
            OrderManager.Instance.DestroyInstance();
            BoosterManager.Instance.DestroyInstance();
            WeaponManager.Instance.DestroyInstance();


            //if (GameData.Instance.GameSettings.showMascot)
            //    MascotSpawnManager.Instance.DestroyInstance();
            //ObstacleManager.Instance.DestroyInstance();

            Destroy(gameObject);
        }

        public ModelClass.Dish GetRandomDishData()
        {
            if (LevelInfo.FilteredDishData.Count == 0) return null;
            return LevelInfo.FilteredDishData[UnityEngine.Random.Range(0, LevelInfo.FilteredDishData.Count)];
        }



        #region ------------------------------------------------------------------------------------------ Calculation
        // Called on Order Delivered
        public int CalculateChips(Order _order) => _order.IsNFTOrder ? CalculationData.chipsPoints[GameData.Instance.GetUserDishByToken(_order.NFTOrderTokenId).level] : CalculationData.chipsPoints[0];
        private void UpdateNFTAsWellAsNonNFTScore(Order _order) => LevelInfo.NFTScore += _order.IsNFTOrder ? GameData.Instance.GetUserDishByToken(_order.NFTOrderTokenId).points : CalculationData.NonNFTMunchValue;

        // Called on Level Complete
        private int GetTotalMunches() => Mathf.FloorToInt(LevelInfo.NFTScore * GetLevelMultiplier() * timerMultiplier * healthMultiplier);
        private float GetLevelMultiplier() => 1 + MathF.Pow(GameData.Instance.SelectedLevelNumber / CalculationData.LevelXValue, CalculationData.LevelYValue);



        float healthMultiplier = 0;
        private void UpdateHealthMultiplier(int currentHealth)
        {
            float _startingHealth = LevelInfo.LevelBooster == Enums.LevelBoosterType.Shield ? Rider.Vehicle.VehicleHealth.InitialHealth + 25 : Rider.Vehicle.VehicleHealth.InitialHealth;
            float healthPercentage = (currentHealth / _startingHealth) * 100;
            if (healthPercentage <= 0)
            {
                healthMultiplier = 0;
            }
            else
            {
                healthPercentage = Mathf.Clamp(Mathf.FloorToInt(healthPercentage), 0, 100);
                LevelInfo.HealthBonus = Mathf.Clamp(healthPercentage / 100f, 0, 1);
                ModelClass.CalculationData.PercentageWithMultiplierData percentageWithMultiplierData = CalculationData.HealthMultiplier.FirstOrDefault(x => healthPercentage >= x.MinPercentage);
                if (percentageWithMultiplierData != null) healthMultiplier = percentageWithMultiplierData.Multiplier;
            }
        }

        float totalTimeTakenToDeliverOrder;
        float totalOrderTime;
        float timerMultiplier;
        private void UpdateTimeMultiplier(int deliveryTime)
        {
            float percentage = 0;
            if (LevelInfo.CurrentLevel.TryGetCondition(Enums.ObjectiveCondition.InTime, out int _value))
            {
                int totalTimeUsed = _value - LevelInfo.LevelTimer;
                percentage = ((totalTimeUsed / (float)_value) * 100);
            }
            else
            {
                totalTimeTakenToDeliverOrder += deliveryTime;
                totalOrderTime += LevelInfo.CurrentLevel.orderTime;
                percentage = (totalTimeTakenToDeliverOrder / totalOrderTime) * 100;
            }

            percentage = Mathf.Clamp(Mathf.FloorToInt(percentage), 0, 100);
            LevelInfo.TimeBonus = Mathf.Clamp((100 - percentage) / 100f, 0, 1);
            timerMultiplier = CalculationData.TimeMultiplier.FirstOrDefault(x => percentage >= x.MinPercentage && percentage <= x.MaxPercentage).Multiplier;
        }

        private void CalculateStarValue()
        {
            LevelInfo.StarValue = 0;
            LevelInfo.StarValue += CalculationData.HealthMultiplier.Count - CalculationData.HealthMultiplier.IndexOf(CalculationData.HealthMultiplier.FirstOrDefault(x => x.Multiplier == healthMultiplier));
            LevelInfo.StarValue += CalculationData.TimeMultiplier.Count - CalculationData.TimeMultiplier.IndexOf(CalculationData.TimeMultiplier.FirstOrDefault(x => x.Multiplier == timerMultiplier));
            LevelInfo.StarValue = Mathf.Clamp01(LevelInfo.StarValue / 6f);
        }
        #endregion


        public ModelClass.Dish GetNextOrder()
        {
            return null;
        }



        private void OnApplicationQuit()
        {
            StopCoroutine("Co_StartObjectiveTime");
            StopAllCoroutines();
        }



    }

}
