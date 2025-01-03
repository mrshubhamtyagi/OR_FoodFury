using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fusion.Photon.Realtime;
using JetBrains.Annotations;
using Newtonsoft.Json;
using OneRare.FoodFury.Multiplayer;
using UnityEngine;
using VoxelBusters.CoreLibrary.Parser;
using static FoodFury.ModelClass;

namespace FoodFury
{
    [DefaultExecutionOrder(-100)]
    public class GameData : MonoBehaviour
    {
        public static event Action OnPlayerDataUpdate, OnLevelSelected;
        public static event Action<Enums.Orientation> OnOrientationUpdated;
        [SerializeField] private GameObject gameControllerPrefab;
        [SerializeField] private TMPro.TextMeshProUGUI version;
        [SerializeField] private GameObject maintenanceObj;

        [Header("-----Launch Params")] public Enums.ServerMode serverMode;
        public Enums.ReleaseMode releaseMode;
        public Enums.GameModeType gameMode;
        public Enums.Platform Platform;
        [SerializeField] private PhotonAppSettings photonAppSettings;
        [field: SerializeField] public string ReleasedVersion { get; private set; }
        [HideInInspector] public Enums.InputType Controls = Enums.InputType.Arrows;

        [Header("-----Data")] [SerializeField] public int SelectedMapId;
        [SerializeField] public int SelectedLevelNumber;
        [field: SerializeField] public string PlayerId { get; private set; }
        [field: SerializeField] public GameSettingSO GameSettings { get; private set; }
        [field: SerializeField] public PlayerDataSO PlayerData { get; private set; }

        [field: SerializeField] public CompletedLevelsSO CompletedLevels { get; private set; }

        //[field: SerializeField] public List<ModelClass.GarageVehicleData> GarageData { get; private set; }
        [field: SerializeField] public GarageDataSO GarageData { get; private set; }
        [field: SerializeField] public MapDataSO MapData { get; private set; }
        [field: SerializeField] public LevelDataSO LevelData { get; private set; }
        [field: SerializeField] public List<int> MascotLevels { get; private set; }
        [field: SerializeField] public UserDishDataSO UserDishData { get; set; }
        [field: SerializeField] public FuelDataSO FuelData { get; private set; }

        [field: SerializeField] public MultiplayerGameConfigSO MultiplayerGameConfig { get; private set; }
        [SerializeField] public Dictionary<int, Texture2D> DishImageDictionary { get; private set; }
        [SerializeField] private int TankCapacity;
        [field: SerializeField] public int RestoreValue { get; private set; }
        [field: SerializeField] public string ReferalUrl { get; private set; }

        public int TankCapacityInSeconds => TankCapacity * 60;
        public int FuelRestoreIntervalInMinute => (int)(GameSettings.restoreFuelInterval / 60f);
        public int FuelRestoreRemainingTime { get; private set; } = 0;
        public static Texture2D ProfilePic { get; private set; }

        [Space(20)] [Header("-----Debug")] [Header("-----Debug")]
        public bool checkForMaintenance = true;

        [SerializeField] private Texture2D defaultPic;
        public string _dishNameDebug = "";
        [TextArea(2, 10)] public string JsonString;

        Vector2 _1 = new Vector2(50, 190);
        Vector2 _2 = new Vector2(-130, 190);
        Vector2 _3 = new Vector2(-130, 2);


        public static GameData Instance { get; private set; }
        public string TelegramUserName { get; private set; }


        void Awake()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.targetFrameRate = 60;
            DishImageDictionary = new();
            if (Instance != null) Destroy(gameObject);
            else
            {
                Instance = this;
                // if (!Application.isEditor) SetPlatform();
                photonAppSettings.AppSettings.AppVersion = ReleasedVersion;
                if (serverMode == Enums.ServerMode.MainNet)
                    photonAppSettings.AppSettings.AppIdFusion = Constants.PHOTON_FUSION_KEY_MAINNET;

                print($"Platform - {Platform} | Version - {ReleasedVersion} | Server Mode - {serverMode.ToString()}");
                DontDestroyOnLoad(gameObject);
            }
        }

        private void SetPlatform()
        {
#if !UNITY_WEBGL
            if (Application.platform == RuntimePlatform.Android) Platform = Enums.Platform.Android;
            else if (Application.platform == RuntimePlatform.IPhonePlayer) Platform = Enums.Platform.iOS;
            else Platform = Enums.Platform.WebGl;
#elif UNITY_WEBGL
            //Navid bhaiya ka code 
#endif
        }

        public bool IsGamePaused => gameMode == Enums.GameModeType.SinglePlayer
            ? GameController.IsGamePaused
            : GameManager.IsGamePaused;

        public static void Invoke_OnPlayerDataUpdate() => OnPlayerDataUpdate?.Invoke();

        public static void Invoke_OnOrientationUpdate(Enums.Orientation orientation) =>
            OnOrientationUpdated?.Invoke(orientation);

        public void RestoreFuel()
        {
            int totalFuel = PlayerData.Data.fuel + CalculateFuelToBeGivenAfterEachInterval();
            FuelRestore_API(totalFuel, completed => { Debug.Log("Fuel Updated"); });
        }

        [ContextMenu("Null Flags")]
        public async void GetNullTextures()
        {
            var countries = new List<string>
            {
                "India",
                "Venezuela",
                "Argentina",
                "Brazil",
                "Chile",
                "Colombia",
                "Peru",
                "Paraguay",
                "Ecuador",
                "Ethiopia",
                "Kenya",
                "Nigeria",
                "South Africa",
                "Egypt",
                "Ghana",
                "Morocco",
                "Japan",
                "Germany",
                "Austria",
                "Czech Republic",
                "Georgia",
                "Hungary",
                "Poland",
                "Russia",
                "Ukraine",
                "Belgium",
                "France",
                "Italy",
                "Spain",
                "Switzerland",
                "Netherlands",
                "Iran",
                "Saudi Arabia",
                "Israel",
                "Jordan",
                "Lebanon",
                "Syria",
                "Turkey",
                "Mexico",
                "USA",
                "Britain",
                "Bangladesh",
                "Cambodia",
                "Singapore",
                "Indonesia",
                "Malaysia",
                "Thailand",
                "Sri Lanka",
                "South Korea",
                "Taiwan",
                "Vietnam",
                "Philippines",
                "North Korea",
                "China",
                "Denmark",
                "Finland",
                "Norway",
                "Sweden",
                "Canada",
                "Australia",
                "New Zealand",
                "Greece"
            };
            foreach (string country in countries)
            {
                Texture texture = await APIManager.GetTextureAsync(RemoveSpacesAndLowerCaseFlagName(country),
                    APIManager.AWSUrl.Flag);
                if (texture == null)
                {
                    Debug.Log(country);
                }
            }
        }

        //dish dasboioard, gameplay screen, order, order prefab
        private string RemoveSpacesAndLowerCaseFlagName(string country)
        {
            country = country.Replace(" ", string.Empty).ToLower();
            return country;
        }

        private void Start()
        {
            if (!Application.isEditor) checkForMaintenance = releaseMode == Enums.ReleaseMode.Release;
            Loader.Instance.ShowLoader();

            version.text = $"Version_{ReleasedVersion}";
            _dishNameDebug = string.Empty;
            //UserDishData = new();
            MascotLevels = new();

            GetDataAndInit();
        }

        private async void GetDataAndInit()
        {
            string errorString = "Could not Fetch Data!!! \n Make sure you are connected to internet";
            (Enums.QueryResult result, bool maintenanceMode) maintenanceResult = await IsInMaintenenceMode();
            if (maintenanceResult.result == Enums.QueryResult.Error)
            {
                WarningPopup.Instance.ShowWarningWithRetry(errorString, GetDataAndInit);
                return;
            }

            var _fuel = await FuelAPIs.GetFuelData_API();
            if (!_fuel.error)
            {
                FuelData.initialValues = _fuel.result.initialValues;
                FuelData.tankUpgrades = _fuel.result.tankUpgrades;
                FuelData.restoreUpgrades = _fuel.result.restoreUpgrades;
            }
            else
            {
                WarningPopup.Instance.ShowWarningWithRetry(errorString, GetDataAndInit);
                Debug.Log("Could not fetch Data - Fuel");
                return;
            }

            var _garage = await GarageAndVehiclesAPIs.GetGarageData_API();
            if (!_garage.error) GarageData.Data = _garage.result;
            else
            {
                WarningPopup.Instance.ShowWarningWithRetry(errorString, GetDataAndInit);
                Debug.Log("Could not fetch Data - Garage");
                return;
            }

            var _maps = await LevelsAndMapAPIs.GetMapData_API();
            if (!_maps.error) MapData.Data = _maps.result.OrderBy(x => x.isActive ? 0 : 1).ToList();
            //MapData.Data = _maps.result.OrderBy(x => x.isActive ? 0 : 1).ToList();
            else
            {
                WarningPopup.Instance.ShowWarningWithRetry(errorString, GetDataAndInit);
                Debug.Log("Could not fetch Data - Maps");
                return;
            }

            var _levels = await LevelsAndMapAPIs.GetLevels_API();
            if (!_levels.error)
            {
                LevelData.version = _levels.result.version;
                LevelData.levels = _levels.result.levels;
            }
            else
            {
                WarningPopup.Instance.ShowWarningWithRetry(errorString, GetDataAndInit);
                Debug.Log("Could not fetch Data - Levels");
                return;
            }

            var _settings = await OtherAPIs.GetGeneralSettings_API();
            if (!_settings.error) GameSettings = _settings.result;
            else
            {
                WarningPopup.Instance.ShowWarningWithRetry(errorString, GetDataAndInit);
                Debug.Log("Could not fetch Data - Settings");
                return;
            }

            if (LoginManager.Instance != null) LoginManager.Instance.Init();
            else Init_Debug();
        }


        public void SetReferalUrl(string url)
        {
            ReferalUrl = url;
        }

        public void SetTelegramUserName(string userName)
        {
            TelegramUserName = userName;
        }

        [ContextMenu("Init")]
        private void Init_Debug() => Init(PlayerId);

        public async void Init(string _playerId)
        {
            PlayerId = _playerId;
            Loader.Instance.ShowLoader();

            bool _result = await PlayerAPIs.GetPlayerData_API(_playerId);
            if (_result)
            {
                // FetchUserDishes();
                if (serverMode == Enums.ServerMode.TestNet) version.text += $"\n{_playerId}";
                AnalyticsManager.Instance.FireGameStartedEvent(PlayerData.Data.signupvia.handle.ToString(),
                    PlayerData.Data.signupvia.via, PlayerData.Data.rank.ToString(), PlayerData.Data.munches,
                    ((int)PlayerData.Data.chips));
                AddMapsToPlayerLevels();
                HandleFuelRestorationAndLevelCompletion();
            }
            else
            {
                WarningPopup.Instance.ShowWarningWithRetryAndLogout(
                    "Could not fetch Player Data! \n Make sure you are connected to internet",
                    () => { Init(PlayerId); });

                // Show Logout and Retry option
            }
        }

        public void HandleFuelRestorationAndLevelCompletion()
        {
            if (PlayerData.Data.fuelRestoreTimestamp != 0)
            {
                int fuelToBeAdded =
                    CalculateFuelToBeGiven(
                        HelperFunctions.UnixTimeStampToDateTime(PlayerData.Data.fuelRestoreTimestamp));
                Debug.Log("Fuel to be added " + fuelToBeAdded);
                if (fuelToBeAdded > 0)
                {
                    FuelRestore_API(PlayerData.Data.fuel + fuelToBeAdded, completed =>
                    {
                        ProceedToNextStep();
                        Debug.Log("Tank:" + TankCapacity);
                    });
                    return;
                }
            }

            ProceedToNextStep();
        }

        async void ProceedToNextStep()
        {
            var _result = await LevelsAndMapAPIs.GetCompletedLevels_API();
            if (!_result.error) CompletedLevels.Data = _result.result;
            ScreenManager.Instance.SwitchScreen(HomeScreen.Instance, null);
        }

        public void SetRemaingFuelTimer(int value)
        {
            FuelRestoreRemainingTime = value;
        }

        [ContextMenu("FetchUserDishes")]
        private async void FetchUserDishes() => await DishAPIs.GetUserDishes_API();


        #region ------------------------------------------------------------------------------ GETTERS

        public bool IsTelegramConnected() =>
            PlayerData.Data.isTelegramConnected && !string.IsNullOrEmpty(PlayerData.Data.gmailUserId);

        public bool IsTelegramDisconnected() =>
            PlayerData.Data.signupvia.via.Equals("TELEGRAM") && !PlayerData.Data.isTelegramConnected;

        // Returns Current map id based on Platform
        public int GetCurrentMapId()
        {
            if (Platform == Enums.Platform.Android) return GameSettings.currentMapIdAndroid;
            else if (Platform == Enums.Platform.iOS) return GameSettings.currentMapIdiOS;
            else return GameSettings.currentMapIdWebGL;
        }


        // Returns Current Playing MapData
        public ModelClass.MapDetail GetSelectedMapData() => MapData.GetMapDetails(SelectedMapId);


        // Returns Map All Levels - for OnBoarding or Other Maps
        public List<LevelDataSO.Level> GetLevels() => LevelData.levels;

        // Returns Level Data from Map by Index 
        public LevelDataSO.Level GetLevelDataByIndex(int _index) => LevelData.levels[_index - 1];


        // Returns Completed Levels for Selected Map
        public ModelClass.CompletedLevels GetCompletedLevels() => CompletedLevels.GetCompletedLevels(SelectedMapId);


        // Returns Level number for Selected Map from PLAYER DATA
        public ModelClass.PlayerData.PlayerLevel GetPlayerLevelNumberData() =>
            PlayerData.Data.playerLevel.FirstOrDefault(m => m.mapId == SelectedMapId);


        // Returns Player Stats Data
        public ModelClass.PlayerLevelStats GetCompletedLevelStats(int _level)
        {
            if (CompletedLevels == null) CompletedLevels = new CompletedLevelsSO();
            if (GetCompletedLevels() == null)
            {
                CompletedLevels.Data.Add(new ModelClass.CompletedLevels()
                {
                    mapId = SelectedMapId,
                    levels = new List<ModelClass.PlayerLevelStats>()
                });
            }

            return CompletedLevels.GetCompletedLevels(SelectedMapId).levels[_level];
        }

        public void AddCompletedLevelStats(ModelClass.PlayerLevelStats _completedLevel, bool _retry = false)
        {
            if (CompletedLevels == null) CompletedLevels = new CompletedLevelsSO();
            ModelClass.CompletedLevels _completedLevels = GetCompletedLevels();

            // Map not added to the Completed Levels - First Level completed on New Map
            if (_completedLevels == null)
            {
                CompletedLevels.Data.Add(new ModelClass.CompletedLevels()
                {
                    mapId = SelectedMapId,
                    levels = new List<ModelClass.PlayerLevelStats>() { _completedLevel }
                });
                return;
            }
            else if (_retry)
            {
                ModelClass.PlayerLevelStats _playerLevel =
                    _completedLevels.levels.Find(l => l.level == _completedLevel.level);
                _playerLevel.munches += _completedLevel.munches;
                _playerLevel.chips += _completedLevel.chips;
                _playerLevel.stars += _completedLevel.stars;
            }
            else
            {
                if (_completedLevels.levels.Find(l => l.level == _completedLevel.level) == null)
                    _completedLevels.levels.Add(_completedLevel);
            }
        }

        public ModelClass.Dish GetUserDishByToken(int _tokenId) => UserDishData.GetDishByTokenId(_tokenId);

        public bool CheckUserHasDish(int _baseId, out int _tokenId)
        {
            var _dish = UserDishData.GetDishByBaseId(_baseId);
            _tokenId = _dish == null ? _baseId : _dish.tokenId;
            return _dish != null;
        }

        #endregion


        public void GetMapMusic_API(Action _callback)
        {
            StartCoroutine(APIManager.DownloadTrack(GetSelectedMapData().mapTrack, (GameplayMusicClip) =>
            {
                //Debug.Log("Gameplay:" + GameplayMusicClip?.name);
                AudioManager.Instance.SetGamePlayMusicAudio(GameplayMusicClip);
                _callback?.Invoke();
            }));
        }


        public List<ModelClass.Dish> GetFilteredDishes() => MapData.GetMapDetails(SelectedMapId).dishDetails;


        #region ---------------------------------------------------------------------------------------------- Fuel

        public void FuelPurchased_API(Enums.FuelPurchaseType _fuelType, Action<bool> _callback)
        {
            int _level = (_fuelType == Enums.FuelPurchaseType.Tank
                ? PlayerData.Data.fuelTankLevel
                : PlayerData.Data.fuelRestoreLevel) + 1;
            bool _canUpgrade = _fuelType == Enums.FuelPurchaseType.Tank ? CanBuyTankUpgrade() : CanBuyRestoreUpgrade();
            int _totalUpgradeLevels = _fuelType == Enums.FuelPurchaseType.Tank
                ? FuelData.TotalTankUpgrades()
                : FuelData.TotalRestoreUpgrades();
            Debug.Log($"{_level} {_totalUpgradeLevels} {_level >= _totalUpgradeLevels}");
            if ((_level > _totalUpgradeLevels) || !_canUpgrade)
            {
                if (!_canUpgrade)
                {
                    OverlayWarningPopup.Instance.ShowWarning($"Could not purchase Fuel - Insufficient Chips");
                }

                _callback?.Invoke(false);
                return;
            }

            if (releaseMode == Enums.ReleaseMode.Debug)
            {
                if (_fuelType == Enums.FuelPurchaseType.Tank)
                {
                    PlayerData.Data.chips -= GetTankUpgrade(PlayerData.Data.fuelTankLevel + 1).cost;
                    PlayerData.Data.fuelTankLevel++;
                    TankCapacity = GetCurrentFuelTankCapacity();
                }

                if (_fuelType == Enums.FuelPurchaseType.Restore)
                {
                    PlayerData.Data.chips -= GetRestoreUpgrade(PlayerData.Data.fuelRestoreLevel + 1).cost;
                    PlayerData.Data.fuelRestoreLevel++;
                    RestoreValue = GetFuelTankRestore(PlayerData.Data.fuelRestoreLevel).value;
                }

                OnPlayerDataUpdate?.Invoke();
                //AnalyticsManager.Instance.FireFuelPurchasedSuccessfulEvent(_fuelType.id, PlayerData.fuel - _fuelType.fuel, PlayerData.fuel, _fuelType.costType.ToString());
                _callback?.Invoke(true);
                return;
            }


            Dictionary<string, object> _params = new() { { "userId", PlayerId } };
            _params.Add("type", _fuelType.ToString());

            //Debug.Log($"FuelPurchased Params - {JsonConvert.SerializeObject(_params)}");

            StartCoroutine(APIManager.PurchaseFuel(_params.ToJSONObject(), (_status, _response) =>
            {
                //Debug.Log($"FuelPurchased - Status {_status} | Response {_response}");
                if (_status)
                {
                    ModelClass.ErrorAndResultResponse<string> _data =
                        JsonConvert.DeserializeObject<ModelClass.ErrorAndResultResponse<string>>(_response);
                    if (_data.error)
                    {
                        OverlayWarningPopup.Instance.ShowWarning(_data.result);
                        Debug.Log(_data.result);
                        if (GetTankUpgrade(PlayerData.Data.fuelTankLevel + 1).costType == Enums.CostType.POINTS)
                            PlayerData.Data.chips = int.Parse(_data.result.Split('-')[1].Trim());
                        else PlayerData.Data.gameBalance = float.Parse(_data.result.Split('-')[1].Trim());
                        OnPlayerDataUpdate?.Invoke();
                        _callback?.Invoke(false);
                    }
                    else
                    {
                        if (_fuelType == Enums.FuelPurchaseType.Tank)
                        {
                            PlayerData.Data.chips -= GetTankUpgrade(PlayerData.Data.fuelTankLevel + 1).cost;
                            PlayerData.Data.fuelTankLevel++;
                            TankCapacity = GetCurrentFuelTankCapacity();
                        }

                        if (_fuelType == Enums.FuelPurchaseType.Restore)
                        {
                            PlayerData.Data.chips -= GetRestoreUpgrade(PlayerData.Data.fuelRestoreLevel + 1).cost;
                            PlayerData.Data.fuelRestoreLevel++;
                            RestoreValue = GetFuelTankRestore(PlayerData.Data.fuelRestoreLevel).value;
                        }

                        OnPlayerDataUpdate?.Invoke();
                        //AnalyticsManager.Instance.FireFuelPurchasedSuccessfulEvent(_fuelType.id, PlayerData.fuel - _fuelType.fuel, PlayerData.fuel, _fuelType.costType.ToString());
                        _callback?.Invoke(true);
                    }
                }
                else
                {
                    OverlayWarningPopup.Instance.ShowWarning($"Could not purchase Fuel - {_response}");
                    _callback?.Invoke(false);
                }
            }));
        }


        public void FuelRestore_API(int totalFuel, Action<bool> _callback)
        {
            if (totalFuel > TankCapacityInSeconds)
            {
                totalFuel = TankCapacityInSeconds;
            }

            if (releaseMode == Enums.ReleaseMode.Debug)
            {
                PlayerData.Data.fuel = totalFuel;
                OnPlayerDataUpdate?.Invoke();
                //AnalyticsManager.Instance.FireFuelPurchasedSuccessfulEvent(_fuelType.id, PlayerData.fuel - _fuelType.fuel, PlayerData.fuel, _fuelType.costType.ToString());
                _callback?.Invoke(true);
                return;
            }


            Dictionary<string, object> _params = new() { { "userId", PlayerId } };
            _params.Add("fuel", totalFuel);

            //Debug.Log($"FuelPurchased Params - {JsonConvert.SerializeObject(_params)}");

            StartCoroutine(APIManager.RestoreFuel(_params.ToJSONObject(), (_status, _response) =>
            {
                //Debug.Log($"FuelPurchased - Status {_status} | Response {_response}");
                if (_status)
                {
                    ModelClass.ErrorAndResultResponse<long> _data =
                        JsonConvert.DeserializeObject<ModelClass.ErrorAndResultResponse<long>>(_response);
                    if (_data.error)
                    {
                        OverlayWarningPopup.Instance.ShowWarning("Could not restore fuel!");
                        _callback?.Invoke(false);
                    }
                    else
                    {
                        PlayerData.Data.fuel = totalFuel;
                        PlayerData.Data.fuelRestoreTimestamp = _data.result;
                        OnPlayerDataUpdate?.Invoke();
                        _callback?.Invoke(true);
                    }
                }
                else
                {
                    OverlayWarningPopup.Instance.ShowWarning($"Could not restore fuel! - {_response}");
                    _callback?.Invoke(false);
                }
            }));
        }


        public int GetCurrentFuelTankCapacity() => PlayerData.Data.fuelTankLevel > 0
            ? FuelData.initialValues.tank +
              (PlayerData.Data.fuelTankLevel * GetFuelTankUpgradeValue(PlayerData.Data.fuelTankLevel))
            : FuelData.initialValues.tank;

        private int GetFuelTankUpgradeValue(int level) => FuelData.tankUpgrades[level - 1].value;

        public int CalculateInterval(DateTime lastCalculationTime)
        {
            DateTime currentTime = DateTime.Now;
            TimeSpan timeSpan = currentTime - lastCalculationTime;
            double intervalsPassed = (timeSpan.TotalMinutes / (double)FuelRestoreIntervalInMinute);
            int interpassedInInt = (int)intervalsPassed;
            double intervalAlreadyPassed = (intervalsPassed - interpassedInInt);
            FuelRestoreRemainingTime = (int)(GameSettings.restoreFuelInterval * (1 - intervalAlreadyPassed));
            return interpassedInInt;
        }

        public int CalculateFuelToBeGiven(DateTime lastFuelTime)
        {
            if (FuelData.restoreUpgrades.Count == 0)
            {
                OverlayWarningPopup.Instance.ShowWarning("Could not Fetch Fuel");
                return 0;
            }

            int restoreValue = PlayerData.Data.fuelRestoreLevel > 0
                ? FuelData.restoreUpgrades[PlayerData.Data.fuelRestoreLevel - 1].value
                : FuelData.initialValues.restore;
            int intervalspassed = CalculateInterval(lastFuelTime);
            int difference = (GetCurrentFuelTankCapacity() * 60) - PlayerData.Data.fuel;
            int fuelToBeAdded = restoreValue * intervalspassed;
            Debug.Log(
                $"restore value: {restoreValue} interval passed : {intervalspassed} difference: {difference} fuelTo be added: {fuelToBeAdded}");
            if (difference > 0)
            {
                if (fuelToBeAdded > difference)
                {
                    Debug.Log("Fuel is Full");
                    // FuelRestoreRemainingTime = 0;
                    return difference;
                }
                else
                {
                    return fuelToBeAdded;
                }
            }
            else
            {
                return 0;
            }
        }

        public int CalculateFuelToBeGivenAfterEachInterval()
        {
            int restoreValue = PlayerData.Data.fuelRestoreLevel > 0
                ? FuelData.restoreUpgrades[PlayerData.Data.fuelRestoreLevel - 1].value
                : FuelData.initialValues.restore;
            int difference = GetCurrentFuelTankCapacity() * 60 - PlayerData.Data.fuel;
            int fuelToBeAdded = restoreValue;
            Debug.Log($"restore value: {restoreValue} difference: {difference} fuelTo be added: {fuelToBeAdded}");
            if (difference > 0)
            {
                return fuelToBeAdded >= difference ? difference : fuelToBeAdded;
            }
            else
            {
                return 0;
            }
        }

        public bool CanBuyRestoreUpgrade() =>
            PlayerData.Data.chips >= GetRestoreUpgrade(PlayerData.Data.fuelRestoreLevel + 1).cost;

        public bool CanBuyTankUpgrade() =>
            PlayerData.Data.chips >= GetTankUpgrade(PlayerData.Data.fuelTankLevel + 1).cost;

        public FuelDataSO.FuelUpgrade GetTankUpgrade(int level) => FuelData.tankUpgrades[level - 1];

        public FuelDataSO.FuelUpgrade GetRestoreUpgrade(int level) => FuelData.restoreUpgrades[level - 1];

        public FuelDataSO.FuelUpgrade GetFuelTankRestore(int level) => FuelData.restoreUpgrades[level - 1];

        #endregion


        public void UpdatePlayerDataLocal(PlayerData _data)
        {
            PlayerId = _data._id;
            PlayerData.Data = _data;
            if (Platform == Enums.Platform.WebGl)
            {
                PlayerData.Data.username = String.IsNullOrEmpty(TelegramUserName) == false
                    ? TelegramUserName
                    : PlayerData.Data.username;
            }

            PlayerData.Data.driverDetails.name = _data.username;

            TankCapacity = GetCurrentFuelTankCapacity();
            RestoreValue = _data.fuelRestoreLevel > 0
                ? GetFuelTankRestore(_data.fuelRestoreLevel).value
                : FuelData.initialValues.restore;
            OnPlayerDataUpdate?.Invoke();

            StartCoroutine(APIManager.GetTexture(PlayerData.Data.profilePic, _result =>
            {
                if (_result == null)
                {
                    ProfilePic = defaultPic;
                    Debug.Log("Hey profile pic is not updating please check !!!!!");
                    // OverlayWarningPopup.Instance.ShowWarning("Could not get profile pic!");
                }
                else ProfilePic = _result;

                OnPlayerDataUpdate?.Invoke();
            }));
        }


        // Add maps to Player Completed Levels with Level set to 1
        public void AddMapsToPlayerLevels()
        {
            foreach (var item in MapData.Data)
            {
                if (PlayerData.Data.playerLevel.Find(l => l.mapId == item.mapId) == null)
                    PlayerData.Data.playerLevel.Add(
                        new PlayerData.PlayerLevel() { mapId = item.mapId, levelNumber = 1 });
            }
        }


        public bool DeductCost(Enums.CostType _type, int _cost)
        {
            //Debug.Log($"DeductCost -> Type [{_type}] | Cost [{_cost}] | PlayerChips [{PlayerData.chips}]");
            if (_type == Enums.CostType.POINTS)
            {
                if (PlayerData.Data.chips - _cost < 0) return false;
                else PlayerData.Data.chips -= _cost;
            }
            else
            {
                if (PlayerData.Data.gameBalance - _cost < 0) return false;
                else PlayerData.Data.gameBalance -= _cost;
            }

            return true;
        }


        public async void LoadGame()
        {
            OnLevelSelected?.Invoke();
            if (ScreenManager.Instance.CurrentScreen != Enums.Screen.Gameplay)
            {
                ScreenManager.Instance.CurrentScreen = Enums.Screen.PreGameplay;
                var _success = await AddressableLoader.LoadSceneAsync($"{GetSelectedMapData().sceneName}.unity",
                    UnityEngine.SceneManagement.LoadSceneMode.Additive,
                    _progress => Loader.Instance.UpdateLoadingPercentage(_progress));

                if ((int)_success == (int)Enums.QueryResult.Error)
                {
                    PopUpManager.Instance.ShowWarningPopup("Something went wrong! [SceneError]");
                    Loader.Instance.HideLoader();
                    return;
                }

                if ((int)_success == (int)Enums.QueryResult.Cancelled)
                    return;

                GetMapMusic_API(() =>
                {
                    AddressableLoader.SpawnObject<GameController>(gameControllerPrefab).StartSetup();
                });
            }
            else
            {
                GameController.Instance.SetNextLevel(false);
                Loader.Instance.HideLoader();
            }
        }


        private async void UpdatePlayerDataFuel_API(Action<bool> _callback)
        {
            var _result = await PlayerAPIs.UpdatePlayerDataFuel_API();
            _callback?.Invoke(_result);
        }

        public async void EndGame(bool _restart = false)
        {
            Loader.Instance.ShowLoader();
            var _success = await AddressableLoader.UnloadSceneAsync();
            if (_success)
            {
                GameController.Instance.EndSetup();

                UpdatePlayerDataFuel_API(_success =>
                {
                    if (_success)
                    {
                        if (_restart)
                        {
                            LoadGame();
                        }
                        else
                        {
                            AudioManager.Instance.StopSFX();
                            AudioManager.Instance.PlayHomeScreenTrack();
                            ScreenManager.Instance.HideAllScreensExcept(HomeScreen.Instance);
                            PopUpManager.Instance.HideAllPopUps();
                        }
                    }
                    else
                    {
                        OverlayWarningPopup.Instance.ShowWarning("Could not update fuel!!!");
                        if (_restart)
                        {
                            LoadGame();
                        }
                        else
                        {
                            AudioManager.Instance.StopSFX();
                            AudioManager.Instance.PlayHomeScreenTrack();
                            ScreenManager.Instance.HideAllScreensExcept(HomeScreen.Instance);
                            PopUpManager.Instance.HideAllPopUps();
                        }
                    }
                });
            }
        }


        public void CalculateMascotLevels()
        {
            MascotLevels = new();
            UnityEngine.Random.InitState(DateTime.Now.Second);
            int numberOfLevels = GetLevels().Count;
            for (int i = 1; i <= numberOfLevels; i++)
            {
                if (UnityEngine.Random.Range(0, 4) == 0)
                    MascotLevels.Add(i);
            }
        }


        public bool CanMascotSpawn(int selectedLevel) => MascotLevels.FindIndex(x => x == selectedLevel) != -1;


        public async Task<(Enums.QueryResult, bool)> IsInMaintenenceMode()
        {
            maintenanceObj.SetActive(false);
            if (!checkForMaintenance)
                return (Enums.QueryResult.Success, false);


            var _response = await APIManager.GetMaintenanceDataAsync();
            if (!_response.error)
            {
                MaintenanceResponse _data = JsonConvert.DeserializeObject<MaintenanceResponse>(_response.result);
                if (_data.error)
                {
                    Debug.Log($"Could not load Maintenance [{_response}]");
                    // maintenanceObj.SetActive(true);
                    return (Enums.QueryResult.Error, true);
                }
                else
                {
                    bool _isInMaintenance = false;
                    if (Platform == Enums.Platform.Android) _isInMaintenance = _data.result.android;
                    if (Platform == Enums.Platform.iOS) _isInMaintenance = _data.result.ios;
                    if (Platform == Enums.Platform.WebGl) _isInMaintenance = _data.result.webgl;
                    maintenanceObj.SetActive(_isInMaintenance);
                    return (Enums.QueryResult.Success, _isInMaintenance);
                }
            }
            else
            {
                Debug.Log($"Could not load Maintenance [{_response}]");
                // maintenanceObj.SetActive(true);
                return (Enums.QueryResult.Error, true);
            }
        }


        [ContextMenu("Json -> AES")]
        private void JsonToAES() => Debug.Log(AESEncryptionECB.Encrypt(JsonString));

        [ContextMenu("AES -> Json")]
        private void AESToJson() => Debug.Log(AESEncryptionECB.Decrypt(JsonString));


        private void OnApplicationQuit()
        {
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
            StopAllCoroutines();
        }

        public void ResetPlayerData()
        {
            PlayerData.Data = new();
            CompletedLevels.Data.Clear();
            UserDishData.Data.Clear();
        }

        [ContextMenu("Map Sort")]
        private void SortMap()
        {
            MapData.Data = MapData.Data.OrderBy(x => x.isActive ? 0 : 1).ToList();
        }
    }
}