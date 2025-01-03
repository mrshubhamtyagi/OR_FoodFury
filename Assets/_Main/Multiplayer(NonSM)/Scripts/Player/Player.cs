using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FoodFury;
using Fusion;
using Fusion.Addons.SimpleKCC;
using FusionHelpers;
using UnityEngine;

namespace OneRare.FoodFury.Multiplayer
{
    public class Player : FusionPlayer, ICollidable
    {
        private const int MaxLives = 3;
        private int MaxHealth => IsBot ? 200 : 100;

        [Header("---General Settings")] [SerializeField]
        public PlayerMovementHandler playerMovementHandler;

        [SerializeField] private SimpleKCC kcc;

        [SerializeField] private InGameUIHUD hudPrefab;

        //[SerializeField] private Transform visualParent;
        [SerializeField] private Texture2D defaultPic;
        [SerializeField] private Material[] playerMaterials;
        [SerializeField] private WeaponManager weaponManager;
        [SerializeField] private PlayerAIRivalMP _playerAIRivalMp;
        [SerializeField] private TMPro.TextMeshPro playernameTMP;
        //[SerializeField] private MeshRenderer part;

        [Header("---Vehicle Model")] [SerializeField]
        private Transform handle;

        [SerializeField] private Transform body;
        [SerializeField] private bool isPlayer;
        [SerializeField] private VehicleModel[] vehicleModels;
        [SerializeField] private GameObject[] commonVehicleComponents;
        [SerializeField] private GameObject vehicleDamageSmoke;
        [SerializeField] private VehicleFuelMP vehicleFuelMp;

        //[SerializeField] private Rigidbody rigidbody;
        [Header("---Order")] [SerializeField] private TMPro.TextMeshPro orderDistanceTMP;

        //[SerializeField] private TMPro.TextMeshPro livesTMP;
        private Vector3 _targetOrderTransorm;
        private float _orderRange = 10;
        private float _orderDistance;
        [SerializeField] private WeaponHitPopUps weaponHitPopUps;

        [Header("---Compass")] [SerializeField]
        private SpriteRenderer minimapPlayerIcon;

        [SerializeField] private GameObject orderCampassParent;
        [SerializeField] private SpriteRenderer orderCampassSprite;
        [SerializeField] private Transform orderCampassPivot;
        private float _campassHeight = 5;

        #region NetworkProperties

        [field: Header("Networked Properties")]
        [Networked]
        public NetworkString<_32> Username { get; set; }

        [Networked] public int VehicleIndex { get; set; }

        public static Player Local;

        [Networked] public Stage CurrentStage { get; set; }

        [Networked] private int Life { get; set; }

        [Networked] public int OrderCount { get; set; }

        [Networked] public int NFTOrderCount { get; set; }

        [Networked] public int Score { get; set; }

        [Networked] private TickTimer RespawnTimer { get; set; }

        [Networked] private TickTimer IncapableTimer { get; set; }

        [Networked] private TickTimer InvulnerabilityTimer { get; set; }

        [Networked] private TickTimer ShootTimer { get; set; }

        [Networked] public int Lives { get; set; }

        [Networked] public bool Ready { get; set; }

        [Networked] public int Health { get; set; }

        [Networked] public int UIHealth { get; set; }

        [Networked] public int BulletCount { get; set; }

        [Networked] public bool IsMapLoaded { get; set; }

        [Networked] public bool IsBot { get; set; } = false;

        #endregion

        #region Public Variables

        public bool IsActivated => (gameObject.activeInHierarchy &&
                                    (CurrentStage == Stage.Active || CurrentStage == Stage.TeleportIn));

        public bool IsRespawningDone => CurrentStage == Stage.TeleportIn && RespawnTimer.Expired(Runner);

        public Material PlayerMaterial { get; set; }

        public static Texture2D ProfilePic { get; private set; }

        public Color PlayerColor { get; set; }

        public Vector3 Velocity => Object != null && Object.IsValid ? kcc.RealVelocity : Vector3.zero;

        public InGameUIHUD gameUI;

        public int Munches { get; set; }

        public int Chips { get; set; }

        public float totalTimeMultiplier;


        public static readonly List<Player> Players = new List<Player>();
        public static List<Player> LeaderboardPlayers = new List<Player>();
        public bool IsIdle => playerMovementHandler.IsIdle();

        #endregion

        #region Private Variables

        private CapsuleCollider _collider;

        private GameObject _deathExplosionInstance;

        private float _respawnInSeconds = -1;

        private Camera mainCamera;

        private Camera minimapCamera;

        private RoomPlayerUI _roomPlayerUI;

        private ChangeDetector _changes;

        private NetworkInputData _oldInput;
        private NetworkInputData Inputs { get; set; }

        private MobileInput mobileInput;

        #endregion

        #region Actions

        public static Action<Player> PlayerJoined;

        public static Action<Player> PlayerLeft;

        public static Action<Player> PlayerChanged;

        public event Action<int> OnOrderCountChanged;
        public event Action<int> OnNFTOrderCountChanged;
        public event Action<int> OnScoreChanged;
        public event Action<int> OnBoosterTimeChanged;

        #endregion

        //BOT CONFIG

        public int botID;

        public struct DamageEvent : INetworkEvent
        {
            public Vector3 impulse;
            public int damage;
        }

        public struct PickupEvent : INetworkEvent
        {
            public int elapsedTime;
            public int nftLevel;
            public int nftPoint;
        }

        public enum Stage
        {
            New,
            TeleportOut,
            TeleportIn,
            Active,
            Dead
        }

        private void Awake()
        {
            _collider = GetComponentInChildren<CapsuleCollider>();
            orderCampassParent.SetActive(false);
        }

        public override void InitNetworkState()
        {
            CurrentStage = Stage.New;
            Lives = MaxLives;
            Life = MaxHealth;
            //livesTMP.text = Life.ToString();
        }

        private GameManager _gameManager;

        public override void Spawned()
        {
            base.Spawned();
            if (Runner.TryGetSingleton(out GameManager gameManager))
            {
                _gameManager = gameManager;
            }

            _playerAIRivalMp.enabled = false;
            kcc.SetGravity(Physics.gravity.y * 2f);

            _changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
            DontDestroyOnLoad(gameObject);


            AddPlayer();

            vehicleDamageSmoke.SetActive(false);

            Ready = false;
            _respawnInSeconds = 0;

            //SetMaterial();
            OnStageChanged();

            minimapPlayerIcon.color = ColorManager.Instance.Red;

            if (Object.HasStateAuthority)
            {
                SetUpLocalPlayer();
                RegisterEventListener((DamageEvent evt) => { ApplyAreaDamage(evt.impulse, evt.damage); });
                RegisterEventListener((PickupEvent evt) => OnPickup(evt.elapsedTime, evt.nftLevel, evt.nftPoint));
                _gameManager.OnPlayStateChanged += InstanceOnOnChallengeStarted;
            }

            //VehicleIndex = GetVehicleIndex() + PlayerIndex;
            SpawnVehicleModel(VehicleIndex);
            playerMovementHandler.Initialize(this);
            UIHealth = 100;

            PlayerJoined?.Invoke(this);
            //
            Invoke("SetPlayerNameOnTopAcrossClients", 1f);
        }

        bool _isChallengeStarted = false;

        private void InstanceOnOnChallengeStarted(GameManager.PlayState playState)
        {
            if (playState == GameManager.PlayState.LOBBY && !_isChallengeStarted)
            {
                ShootTimer = TickTimer.CreateFromSeconds(Runner, 5f);
                if (mobileInput == null)
                {
                    mobileInput = FindObjectOfType<MobileInput>();
                }

                _isChallengeStarted = true;
            }
        }

        void AddPlayer()
        {
            Players.Add(this);
            LeaderboardPlayers.Add(this);
            LeaderboardPlayers.Sort((p1, p2) => p1.PlayerId.PlayerId - p2.PlayerId.PlayerId);
        }


        void RemovePlayer()
        {
            Players.Remove(this);
            LeaderboardPlayers.Remove(this);
        }

        void SetPlayerNameOnTopAcrossClients()
        {
            gameObject.name = $"___{Username}";
            playernameTMP.text = Local == this ? string.Empty : Username.ToString();
            playernameTMP.gameObject.SetActive(!(Local == this));
            playernameTMP.transform.localScale = new Vector3(-1, 1, 1);
            mainCamera = Camera.main;
        }

        private int GetVehicleIndex()
        {
            // Implement your logic here to choose or assign a vehicle index
            // For example, randomly or based on player input
            return GameData.Instance.PlayerData.Data.currentVehicle;
        }

        private void SpawnVehicleModel(int index)
        {
            VehicleModel _model = Instantiate(vehicleModels[index], transform);
            SetBody(_model.body, _model.handle);

            playerMovementHandler.model = _model.transform;
        }

        [Header("-----Vehicle Sound Collection ")] [SerializeField]
        private VehicleSoundCollectionSO vehicleSoundCollectionSO;

        [SerializeField] private SoundSO orderCollectSound;
        [SerializeField] private SoundSO weaponCollectSound;
        [SerializeField] public SoundSO boosterCollectSound;

        [Header("Audio Curves")] [SerializeField]
        private AnimationCurve crashVolumeCurve;

        [SerializeField] private AudioSource lowHealthAudioSource;
        [SerializeField] private AudioSource lowFuelAudioSource;
        [SerializeField] private AudioSource scooterAudioSource;
        [SerializeField] private float minPitch = 0.8f; // Pitch when the scooter is idle

        [SerializeField] private float maxPitch = 2.0f;

        //[SerializeField] private Action<float> onSpeedChanged;
        //[SerializeField] private List<AudioModuleWithHint<float>> AudioModuleListThatIsAddedToOnSpeedChanged;
        public void ApplyAreaDamage(Vector3 impulse, int damage)
        {
            if (!IsActivated || InvulnerabilityTimer.Expired(Runner) == false &&
                InvulnerabilityTimer.RemainingTime(Runner).HasValue)
                return;

            UIHealth = Life;

            if (damage >= Life)
            {
                if (CurrentStage != Stage.Dead)
                    CurrentStage = Stage.Dead;
                Life = 0;
                //UIHealth = Life;
                if (gameUI)
                {
                    gameUI.UpdateHealthText(UIHealth);
                }
            }
            else
            {
                Life -= (byte)damage;
                UIHealth = Life;
                //livesTMP.text = Life.ToString();
                if (gameUI)
                {
                    gameUI.UpdateHealthText(UIHealth);
                }

                if (!IsBot)
                {
                    HapticsManager.StrongHaptic();
                    AudioSource audioSource =
                        AudioUtils.CreateAudioSource(vehicleSoundCollectionSO.Crash, transform.position);
                    audioSource.volume += crashVolumeCurve.Evaluate(damage);
                    audioSource.Play();
                    //Debug.Log("Played a crash sound ");
                    Destroy(audioSource.gameObject, audioSource.clip.length);
                }
            }

            if (Life < 21)
            {
                vehicleDamageSmoke.SetActive(true);
                playerMovementHandler.MaxSpeed = 350;
                lowHealthAudioSource.Play();
            }
            else
            {
                lowHealthAudioSource.Stop();
            }

            InvulnerabilityTimer = TickTimer.CreateFromSeconds(Runner, 0.3f);
            if (IsBot)
                return;
            if (damage == 10)
            {
                OnWeaponHit(WeaponManager.WeaponInstallationType.SUBMISSILE);
            }
            else if (damage == 9)
            {
                OnWeaponHit(WeaponManager.WeaponInstallationType.KETCHUP);
            }
        }

        //private WeaponHitPopUps weaponUihud;
        public void OnWeaponHit(WeaponManager.WeaponInstallationType weaponInstallationType)
        {
            _weaponHitPopUps.OnWeaponHit(weaponInstallationType);
        }


        void ShowResultScreenAfterDelay()
        {
            gameUI.ShowResultScreen(Player.Local);
        }

        public override void Render()
        {
            foreach (var change in _changes.DetectChanges(this))
            {
                switch (change)
                {
                    case nameof(CurrentStage):
                        OnStageChanged();
                        break;
                    case nameof(OrderCount):
                        OnOrderCountChangedCallback(this);
                        break;
                    case nameof(NFTOrderCount):
                        OnNFTOrderCountChangedCallback(this);
                        break;
                    case nameof(Score):
                        OnScoreChangedCallback(this);
                        break;
                    case nameof(UIHealth):
                        OnFinalHealthChangedCallback(this);
                        break;
                }
            }

            UpdateCampass();
            UpdatePlayerNameTextView();
            UpdateScooterSound();
        }

        void UpdateScooterSound()
        {
            if (IsBot || !Object.HasInputAuthority)
            {
                scooterAudioSource.Stop();
                return;
            }

            if (ChallengeManager.Instance != null)
            {
                if (ChallengeManager.Instance.IsMatchOver)
                {
                    scooterAudioSource.Stop();
                }
            }

            if (scooterAudioSource != null || playerMovementHandler != null)
            {
                float pitch = Mathf.Lerp(minPitch, maxPitch,
                    playerMovementHandler.AppliedSpeed / playerMovementHandler.MaxSpeed);

                // Set the pitch of the AudioSource
                scooterAudioSource.pitch = pitch;

                // Optionally adjust volume based on speed
                scooterAudioSource.volume = Mathf.Lerp(0.05f, 0.3f,
                    playerMovementHandler.AppliedSpeed / playerMovementHandler.MaxSpeed);
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (IsBot)
            {
                if (_gameManager != null)
                    CheckRespawn(_gameManager.CurrentPlayState);
                if (IsRespawningDone)
                    ResetPlayer();
            }

            if (Object.HasInputAuthority)
            {
                if (_gameManager != null)
                    CheckRespawn(_gameManager.CurrentPlayState);

                if (IsRespawningDone)
                    ResetPlayer();
            }

            if (!IsActivated)
                return;

            if (IsBot)
                MoveBot();
            else
            {
                HandleInputs();
            }

            playerMovementHandler.GroundNormalRotation();


            if (HasInputAuthority)
            {
                if (mobileInput != null)
                    CheckIsAllowedToShoot();

                var _mapStatus = AddressableLoader.MapLoadStatus;
                IsMapLoaded = _mapStatus == AddressableLoader.LoadSceneResult.Success;
                if (_mapStatus == AddressableLoader.LoadSceneResult.Error)
                {
                    //Debug.Log("MAP COULD NOT LOAD. SEND THE USER TO HOME SCREEN");
                }
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);
            SpawnTeleportOutFx();
            if (gameUI)
            {
                gameObject.SetActive(false);
            }

            Destroy(orderCampassParent);
            Destroy(_deathExplosionInstance);
            if (gameUI)
            {
                Destroy(gameUI.gameObject, 0.05f);
            }

            //AudioUtils.RemoveModulesFromAction<float>(AudioModuleListThatIsAddedToOnSpeedChanged, ref onSpeedChanged);
            RemovePlayer();
        }


        private static void OnOrderCountChangedCallback(Player changed)
        {
            changed.OnOrderCountChanged?.Invoke(changed.OrderCount);
        }

        private static void OnNFTOrderCountChangedCallback(Player changed)
        {
            changed.OnNFTOrderCountChanged?.Invoke(changed.NFTOrderCount);
        }

        private static void OnScoreChangedCallback(Player changed)
        {
            changed.OnScoreChanged?.Invoke(changed.Score);
        }

        private static void OnFinalHealthChangedCallback(Player changed)
        {
            changed.OnScoreChanged?.Invoke(changed.UIHealth);
        }

        public void OnBoosterTimeUpdated(int timeLeft)
        {
            OnBoosterTimeChanged?.Invoke(timeLeft);
        }

        public void ToggleReady()
        {
            Ready = !Ready;
        }

        public void HideRoomPlayerUI()
        {
            UIManager.Instance.RemoveRoomPlayerUI();
            //Loader.Instance.HideLoader();
        }

        public void ResetReady()
        {
            Ready = false;
        }

        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All)]
        private void RPC_SetPlayerStats(NetworkString<_32> username, int vehicleIndex /*, Texture2D profilePic*/)
        {
            Username = username;
            VehicleIndex = vehicleIndex;
        }

        private WeaponHitPopUps _weaponHitPopUps;

        void SetUpLocalPlayer()
        {
            vehicleFuelMp.Init();

            // Health = GameData.Instance.GameSettings.defaultEngineHealth;
            BulletCount = 1;
            ShootTimer = TickTimer.CreateFromSeconds(Runner, 1f);
            int _shieldLevel = GameData.Instance.PlayerData.Data.GetCurrentVehicleLevels().shieldLevel;
            Life = UIHealth = Health = _shieldLevel == 0
                ? GameData.Instance.GameSettings.defaultEngineHealth
                : GameData.Instance.GameSettings.defaultEngineHealth +
                  vehicleFuelMp.VehicleData.shieldUpgrades[_shieldLevel - 1];
            //AudioUtils.AddModulesToAction<float>(AudioModuleListThatIsAddedToOnSpeedChanged, ref onSpeedChanged);

            foreach (GameObject component in commonVehicleComponents)
            {
                component.SetActive(false);
            }

            var nickname = "null";
            if (IsBot)
            {
                nickname = GameData.Instance.GameSettings.GetRandomBotName();
                Username = nickname;
                _playerAIRivalMp.enabled = true;
                _playerAIRivalMp.isBotActive = true;
                SetupBot();
            }
            else
            {
                nickname = GameData.Instance.PlayerData.Data.username;
                Username = nickname;
                Local = this;
                SetupRealPlayer();
                gameUI = Instantiate(hudPrefab);
                //_roomPlayerUI = Instantiate(roomPlayerPrefab);
                gameUI.Init(this);
                gameUI.UpdatePlayerNameOnHud(nickname, GameData.ProfilePic);
                UIHealth = Life;
                gameUI.UpdateHealthText(UIHealth);
                gameUI.ShowOrHideOdometer(true);
                _weaponHitPopUps = Instantiate(weaponHitPopUps);
                _weaponHitPopUps.Init(this);
                vehicleFuelMp.OnFuelChanged += OnFuelChanged;
            }
            //UIManager.Instance.GenerateRandomNickname();  
            //_roomPlayerUI.SetPlayerProfile(nickname, GameData.ProfilePic);
            //var pic = GameData.ProfilePic;

            if (IsBot)
            {
                //RPC_SetPlayerStats(nickname, VehicleIndex);
                VehicleIndex = UnityEngine.Random.Range(6, 15);
                return;
            }
            else
            {
                VehicleIndex = GameData.Instance.PlayerData.Data.currentVehicle - 1;
            }

            RPC_SetPlayerStats(nickname, VehicleIndex);
            // = $"___{Username}";
        }

        void SetupBot()
        {
            ToggleReady();
            NetworkObject bots = GetComponent<NetworkObject>();
            bots.RequestStateAuthority();
            // tile = TileManager.Instance.GetRandomTile();
        }

        void SetupRealPlayer()
        {
            minimapPlayerIcon.color = ColorManager.Instance.LightGreen;
            mainCamera = Camera.main;

            if (mainCamera != null)
                mainCamera.GetComponent<MultiplayerCameraController>().target = transform;

            minimapCamera = FindObjectOfType<MinimapCameraMP>().GetComponent<Camera>();
            if (minimapCamera != null)
                minimapCamera.GetComponent<MinimapCameraMP>().target = transform;

            orderCampassParent.transform.parent = null;
            orderCampassParent.transform.rotation = Quaternion.identity;
            StartCoroutine(APIManager.GetTexture(GameData.Instance.PlayerData.Data.profilePic, _result =>
            {
                if (_result == null)
                {
                    ProfilePic = defaultPic;
                    Debug.Log("Could not get profile pic!");
                    OverlayWarningPopup.Instance.ShowWarning("Could not get profile pic!");
                }
                else ProfilePic = _result;
            }));
        }

        public void ResetBot()
        {
            Debug.Log("========= BOT IS BEING RESET");
            _playerAIRivalMp.enabled = true;
            _playerAIRivalMp.isBotActive = true;

            RegisterEventListener((DamageEvent evt) => { ApplyAreaDamage(evt.impulse, evt.damage); });
            RegisterEventListener((PickupEvent evt) => OnPickup(evt.elapsedTime, evt.nftLevel, evt.nftPoint));
        }

        public void SetBody(Transform _body, Transform _handle)
        {
            body = _body;
            handle = _handle;
            playerMovementHandler.body = _body;
            playerMovementHandler.handle = _handle;
        }

        private void SetMaterial()
        {
            PlayerMaterial = Instantiate(playerMaterials[Players.IndexOf(Local)]);
            PlayerColor = PlayerMaterial.GetColor("_Color");
            //VehicleModel _model = Instantiate(vehicleModels.First(v => v.VehicleID == VehicleIndex), transform);
            //part.material = PlayerMaterial; //  SetMaterials(playerMaterial);
        }

        public void OnFuelChanged(float _current, float _initial)
        {
            GameData.Instance.PlayerData.Data.fuel = (int)_current;
            gameUI.UpdateFuelBar(_current, _initial);
        }

        #region COMPASS

        Vector3 orderDirection;
        bool activeState;
        float orderInterval;
        private OrderMP order;
        private int time;

        private void UpdateCampass()
        {
            if (!orderCampassParent || !Object.HasInputAuthority)
                return;

            if (ChallengeManager.Instance == null)
                return;
            Transform nearestOrder = GetNearestOrder();
            if (nearestOrder == null)
                return;
            order = nearestOrder.GetComponent<OrderMP>();
            if (order != null && order.IsSpawned)
            {
                time = order.RemainingTimeInSeconds;
            }
            else
            {
                return;
            }

            _targetOrderTransorm = nearestOrder.position;
            _orderDistance = Vector3.Distance(transform.position, _targetOrderTransorm);

            float interval = time / (GameData.Instance.LevelData.levels[1].orderTime * 1f);
            orderCampassSprite.color = interval < 0.33f ? ColorManager.Instance.Red :
                interval < 0.66 ? ColorManager.Instance.Yellow : ColorManager.Instance.DarkGreen;

            orderDistanceTMP.text = $"{Mathf.FloorToInt(_orderDistance)}m";
            activeState = _orderDistance > _orderRange;
            if (orderCampassParent.activeSelf != activeState) orderCampassParent.SetActive(activeState);

            if (!activeState) return;
            orderCampassPivot.position = transform.position + Vector3.up * _campassHeight;
            orderDistanceTMP.transform.position = transform.position + Vector3.up * (_campassHeight + 2);
            //playernameTMP.transform.position = transform.position + Vector3.up * (_campassHeight - 2.5f);
            //livesTMP.transform.position = transform.position + Vector3.up * (_campassHeight + 5);
            orderDirection = _targetOrderTransorm - transform.position;
            orderDirection.y = orderCampassPivot.localRotation.y;
            orderCampassPivot.rotation = Quaternion.Slerp(orderCampassPivot.rotation,
                Quaternion.LookRotation(orderDirection), Time.deltaTime);
        }

        void UpdatePlayerNameTextView()
        {
            if (!mainCamera) return;
            playernameTMP.transform.position = transform.position + Vector3.up * (_campassHeight - 2f);
            playernameTMP.transform.LookAt(mainCamera.transform);
        }

        public List<OrderMP> allOrders = new List<OrderMP>();

        private Transform GetNearestOrder()
        {
            allOrders = ChallengeManager.Instance.activeOrders;
            Transform nearestOrder = null;
            float minDistance = float.MaxValue;

            foreach (OrderMP order in allOrders)
            {
                float distance = Vector3.Distance(transform.position, order.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestOrder = order.transform;
                }
            }

            return nearestOrder;
        }

        #endregion

        #region INPUTS

        void HandleInputs()
        {
            if (InputController.fetchInput == false)
                return;

            playerMovementHandler.Steer(Inputs);
            playerMovementHandler.Move(Inputs);
            playerMovementHandler.Boost();
            if (GetInput(out NetworkInputData input))
            {
                if (Object.HasStateAuthority && input.WasPressed(NetworkInputData.BUTTON_TOGGLE_READY, Inputs))
                    ToggleReady();
                if (input.IsShoot)
                {
                    HandleShooting();
                }

                Inputs = input;
            }
        }

        void HandleShooting()
        {
            if (mobileInput == null)
            {
                mobileInput = FindObjectOfType<MobileInput>();
            }

            if (currentWeapon == WeaponManager.WeaponInstallationType.SUBMISSILE)
            {
                if (ShootTimer.Expired(Runner) == false && ShootTimer.RemainingTime(Runner).HasValue)
                    return;
                weaponManager.FireRocket(currentWeapon);
            }
            else
            {
                //Debug.LogError($"CurrWeapon{currentWeapon}");
                if (BulletCount < 1) return;
                weaponManager.FireRocket(currentWeapon);
                currentWeapon = WeaponManager.WeaponInstallationType.SUBMISSILE;
                mobileInput.UpdateWeaponImage(currentWeapon);
            }

            BulletCount = 0;
            ShootTimer = TickTimer.CreateFromSeconds(Runner, 8f);
            mobileInput.EnableShootButton(false);
        }

        #endregion

        #region COLLISION HANDLING

        private void OnTriggerEnter(Collider other)
        {
            if (!HasInputAuthority && !IsBot)
                return;
            if (other.gameObject.layer == LayerMask.NameToLayer("Player") ||
                other.gameObject.layer == LayerMask.NameToLayer("CityCollider")) // Collided with player or city
            {
                if (IsBot)
                    return;
                int damage = Mathf.Clamp((int)(playerMovementHandler.AppliedSpeed / 100f), 2, 4);

                ApplyAreaDamage(Vector3.zero, damage);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            //if (!HasInputAuthority && !isBot)
            //    return;

            //Debug.Log($"OnTriggerStay - {Username}");
            if (other.gameObject.layer == LayerMask.NameToLayer("CityCollider"))
                return;
            if (other.gameObject.TryGetComponent(out ICollidable collidable))
            {
                collidable.Collide(this);
            }
        }

        public void Collide(Player player)
        {
            if (player.IsBot)
                return;
            player.ReduceHealth();
        }

        private void OnPickup(int elapsedTime, int nftLevel, int nftScore)
        {
            if (!IncapableTimer.Expired(Runner) || ChallengeManager.Instance.IsMatchOver)
                return;

            if (!IsBot)
            {
                HapticsManager.MediumHaptic();
                AudioUtils.PlayOneShotAudio(orderCollectSound, transform.position);
            }

            if (HasStateAuthority)
            {
                CalculateAndApplyPoints(elapsedTime, nftLevel, nftScore);
                OrderCount++;
                if (nftLevel > 1)
                {
                    NFTOrderCount++;
                }
            }

            IncapableTimer = TickTimer.CreateFromSeconds(Runner, 1f);
        }

        public void OnOrderCollectByBots(int elapsedTime, int nftLevel, int nftScore)
        {
            if (!IncapableTimer.Expired(Runner) || ChallengeManager.Instance.IsMatchOver)
                return;

            if (HasStateAuthority)
            {
                CalculateAndApplyPoints(elapsedTime, nftLevel, nftScore);
                OrderCount++;
                if (nftLevel > 0)
                {
                    NFTOrderCount++;
                }
            }

            IncapableTimer = TickTimer.CreateFromSeconds(Runner, 1f);
        }

        public WeaponManager.WeaponInstallationType currentWeapon = WeaponManager.WeaponInstallationType.SUBMISSILE;

        public void WeaponCollected(WeaponManager.WeaponInstallationType collectedWeapon)
        {
            if (IsBot || ChallengeManager.Instance.IsMatchOver || !Object.HasStateAuthority)
                return;

            HapticsManager.MediumHaptic();
            AudioUtils.PlayOneShotAudio(weaponCollectSound, transform.position);
            Debug.Log($"WeaponCollected - {Username}");
            BulletCount = 1;
            currentWeapon = collectedWeapon;
            ShootTimer = TickTimer.CreateFromSeconds(Runner, 0.1f);
            mobileInput.EnableShootButton(true);
            mobileInput.UpdateWeaponImage(currentWeapon);
        }

        void CheckIsAllowedToShoot()
        {
            if (ShootTimer.Expired(Runner))
            {
                BulletCount = 1;
                mobileInput.EnableShootButton(true);
            }
        }

        public void CalculateAndApplyPoints(float elapsedTime, int nftLevel, int nftScore)
        {
            //float elapsedPercentage = elapsedTime * 1.67f; /* elapsedTime/60 * 100*/
            int rank = CalculateRank();
            float rankMultiplier = GetRankMultiplier(rank);
            int pointsEarned = CalculatePoints(elapsedTime);
            Score += pointsEarned;
            int munchesEarned = CalculateMunches(rankMultiplier, elapsedTime, nftScore);
            Munches += munchesEarned;
            Chips += CalculateChips(elapsedTime, nftLevel);
        }

        private int score = 0;

        private int CalculatePoints(float elapsedPercentage)
        {
            if (elapsedPercentage >= 40f)
            {
                score = 25;
            }
            else if (elapsedPercentage >= 20f)
            {
                score = 20;
            }
            else
            {
                score = 15;
            }

            return score;
        }

        float healthMultiplier = 0f;
        private float _timeMultiplier = 0;

        private int CalculateMunches(float rankMultiplier, float elapsedPercentage, int nftScore)
        {
            int nftMunchScore = nftScore != 0 ? nftScore : 2;
            totalTimeMultiplier += elapsedPercentage;
            float _timeMultiplier = CalculateTimeMultiplierForMunches(elapsedPercentage);
            // Calculate Health Multiplier
            healthMultiplier = CalculateHealthMultiplier(Life);
            return (int)(nftMunchScore * rankMultiplier * _timeMultiplier * healthMultiplier);
        }

        private float CalculateHealthMultiplier(float life)
        {
            float healthPercentage = life / 100f;

            if (healthPercentage >= 0.8f)
            {
                return 1.5f;
            }
            else if (healthPercentage >= 0.6f)
            {
                return 1.25f;
            }
            else
            {
                return 1.1f;
            }
        }

        private float timeMultiplierForMunches = 0f;

        public float CalculateTimeMultiplierForMunches(float deliveryTime)
        {
            if (deliveryTime >= 40f)
                timeMultiplierForMunches = 1.5f; // Green band
            else if (deliveryTime >= 20f)
                timeMultiplierForMunches = 1.25f; // Yellow band
            else
                timeMultiplierForMunches = 1.1f; // Red band

            return timeMultiplierForMunches;
        }

        private float timeMultiplierForChips = 0f;

        public float CalculateTimeMultiplierForChips(float deliveryTime)
        {
            if (deliveryTime >= 40f)
                timeMultiplierForChips = 1f; // Green band
            else if (deliveryTime >= 20f)
                timeMultiplierForChips = 0.75f; // Yellow band
            else
                timeMultiplierForChips = 0.5f; // Red band
            return timeMultiplierForChips;
        }

        private int CalculateChips(float elapsedPercentage, int nftLevel)
        {
            int baseChips;

            // Determine the base chips based on the order type and NFT level

            switch (nftLevel)
            {
                case 2:
                    baseChips = 15;
                    break;
                case 3:
                    baseChips = 27;
                    break;
                case 4:
                    baseChips = 41;
                    break;
                case 5:
                    baseChips = 59;
                    break;
                case 6:
                    baseChips = 79;
                    break;
                case 7:
                    baseChips = 100;
                    break;
                case 8:
                    baseChips = 125;
                    break;
                default:
                    Debug.LogWarning("Invalid NFT level, defaulting to 0 chips.");
                    baseChips = 5;
                    break;
            }


            float _timeMultiplier = CalculateTimeMultiplierForChips(elapsedPercentage);
            int totalChips = Mathf.CeilToInt(_timeMultiplier * baseChips);
            return totalChips; // Example conversion rate
        }

        private int CalculateRank()
        {
            var orderedPlayers = Players
                .OrderByDescending(p => p.OrderCount)
                .ToList();
            return orderedPlayers.IndexOf(Local) + 1;
        }

        private float GetRankMultiplier(int rank)
        {
            switch (rank)
            {
                case 1: return 2.0f;
                case 2: return 1.85f;
                case 3: return 1.75f;
                case 4: return 1.5f;
                case 5: return 1f;
                case 6: return 1f;
                default: return 1.0f; // For ranks beyond 6, if needed
            }
        }

        public void ReduceHealth()
        {
            if (Health > 0)
                Health -= 3;
            if (gameUI)
            {
                gameUI.UpdateHealthText(UIHealth);
            }
        }

        #endregion

        #region STAGE HANDLING

        public void EliminatePlayer()
        {
            CurrentStage = Stage.Dead;
        }

        public void OnStageChanged()
        {
            switch (CurrentStage)
            {
                case Stage.TeleportIn:
                    //Debug.Log($"Starting teleport for player {PlayerId} @ {transform.position} cc@ {_cc.Data.Position}, tick={Runner.Tick}");
                    StartCoroutine("TeleportIn");
                    break;
                case Stage.Active:
                    EndTeleport();
                    break;
                case Stage.Dead:
                    DisableObjectsOnPlayerDead();
                    //StartCoroutine("ShowSplashPopupThenResultScreen");
                    if (Runner.TryGetSingleton(out GameManager gameManager))
                    {
                        gameManager.OnTankDeath(Username.ToString());
                    }

                    //UIManager.Instance.ShowSplashPopUpForEliminated();
                    //gameUI.ShowSplashPopUpForEliminated();
                    //gameUI.ShowResultScreen(Local);
                    //ShowSplashPopupThenResultScreen();
                    break;
                case Stage.TeleportOut:
                    SpawnTeleportOutFx();
                    break;
            }

            //visualParent.gameObject.SetActive(CurrentStage == Stage.Active);
            _collider.enabled = CurrentStage != Stage.Dead;
        }

        void DisableObjectsOnPlayerDead()
        {
            body.gameObject.SetActive(false);
            minimapPlayerIcon.gameObject.SetActive(false);
            Destroy(orderCampassParent);
            vehicleDamageSmoke.gameObject.SetActive(false);
            playernameTMP.gameObject.SetActive(false);
            //Destroy(vehicleDamageSmoke);
        }


        public void Reset()
        {
            //Debug.Log($"Resetting player #{PlayerIndex} ID:{PlayerId}");
            Ready = false;
            Lives = MaxLives;
        }

        private void ResetPlayer()
        {
            Debug.Log($"---------------ResetPlayer - {Username}");
            //Debug.Log($"Resetting player {PlayerId}, tick={Runner.Tick}, timer={RespawnTimer.IsRunning}:{RespawnTimer.TargetTick}, life={Life}, lives={Lives}, hasStateAuth={Object.HasStateAuthority} to state={CurrentStage}");
            CurrentStage = Stage.Active;
        }

        public void Respawn(float inSeconds = 0)
        {
            _respawnInSeconds = inSeconds;
        }


        private void CheckRespawn(GameManager.PlayState playState)
        {
            if (_respawnInSeconds >= 0)
            {
                _respawnInSeconds -= Runner.DeltaTime;

                if (_respawnInSeconds <= 0)
                {
                    SpawnPoint spawnpt = Runner.GetLevelManager().GetPlayerSpawnPoint(6);
                    // Debug.Log($"CheckRespawn - {spawnpt.gameObject.name} {spawnpt.transform.position}");

                    if (playState != GameManager.PlayState.LOBBY)
                    {
                        Debug.Log($"In{playState}Case Respawning Player Name{Username},#{PlayerIndex}");
                        if (IsBot)
                            spawnpt = Runner.GetLevelManager().GetPlayerSpawnPoint(5 - botID);
                        else
                        {
                            spawnpt = Runner.GetLevelManager().GetPlayerSpawnPoint(Players.IndexOf(Local));
                        }
                    }

                    if (spawnpt == null)
                    {
                        _respawnInSeconds = Runner.DeltaTime;
                        Debug.LogWarning($"No Spawn Point for player #{PlayerIndex} ID:{PlayerId} - trying again in {_respawnInSeconds} seconds");
                        return;
                    }

                    Debug.Log(
                        $"Respawning Player Name{Username},#{PlayerIndex} ID:{PlayerId},from state={CurrentStage} @{spawnpt}");

                    // Make sure we don't get in here again, even if we hit exactly zero
                    _respawnInSeconds = -1;

                    // Restore health
                    Life = MaxHealth;

                    // Start the respawn timer and trigger the teleport in effect
                    RespawnTimer = TickTimer.CreateFromSeconds(Runner, 1);
                    IncapableTimer = TickTimer.CreateFromSeconds(Runner, 1);

                    // Place the tank at its spawn point. This has to be done in FUN() because the transform gets reset otherwise

                    Transform spawn = spawnpt.transform;
                    Vector3 spawnPos = spawn.position;
                    if (playState == GameManager.PlayState.LOBBY)
                    {
                        if (!IsBot)
                            spawnPos = new Vector3(spawnPos.x + (5 * (PlayerIndex)), -45, spawnPos.z);
                        else
                        {
                            spawnPos = Runner.GetLevelManager().GetPlayerSpawnPoint(5 + botID).transform.position;
                        }
                    }

                    Teleport(spawnPos, spawn.rotation);

                    // If the player was already here when we joined, it might already be active, in which case we don't want to trigger any spawn FX, so just leave it ACTIVE
                    if (CurrentStage != Stage.Active)
                        CurrentStage = Stage.TeleportIn;

                    Debug.Log(
                        $"PlayState{playState}, Respawned player {PlayerId} @ {spawnpt.gameObject.name}, tick={Runner.Tick}, timer={RespawnTimer.IsRunning}:{RespawnTimer.TargetTick}, life={Life}, lives={Lives}, hasStateAuth={Object.HasStateAuthority} to state={CurrentStage}");
                    /*if (playState == GameManager.PlayState.TRANSITION)
                    {
                        InstanceOnOnChallengeStarted(GameManager.PlayState.TRANSITION);
                    }*/
                }
            }
        }

        #endregion

        #region TELEPORT

        void Teleport(Vector3 position, Quaternion rotation)
        {
            kcc.SetPosition(position);
            kcc.SetLookRotation(rotation);
        }

        private void SpawnTeleportOutFx()
        {
            //TankTeleportOutEffect teleout = LocalObjectPool.Acquire(_teleportOutPrefab, transform.position, transform.rotation, null);
            //teleout.StartTeleport(playerColor, turretRotation, hullRotation);
        }

        public void TeleportOut()
        {
            if (CurrentStage == Stage.Dead || CurrentStage == Stage.TeleportOut)
                return;

            if (Object.HasStateAuthority)
                CurrentStage = Stage.TeleportOut;
        }

        private void EndTeleport()
        {
            _endTeleportation = true;
        }

        private bool _endTeleportation;

        private IEnumerator TeleportIn()
        {
            yield return new WaitForSeconds(0.2f);
            while (!_endTeleportation)
                yield return null;
        }

        private void OnDisable()
        {
            // OnDestroy does not get called for pooled objects
            vehicleFuelMp.OnFuelChanged -= OnFuelChanged;
            PlayerLeft?.Invoke(this);
            RemovePlayer();
            //ChallengeManager.Instance.OnActiveOrdersUpdated -= UpdateAllOrders;
        }

        #endregion

        private OrderMP botOrder;

        void MoveBot()
        {
            if (ChallengeManager.Instance == null)
                return;
            Transform nearestOrder = GetNearestOrder();
            if (nearestOrder == null)
                return;
            botOrder = nearestOrder.GetComponent<OrderMP>();
            Vector2 abc = _playerAIRivalMp.HandleInputsBotTest();
            playerMovementHandler.SteerBot(abc.y);
            playerMovementHandler.MoveBot(abc.x);
            //playerMovementHandler.Boost();
        }

        string GenerateRandomBotName()
        {
            List<char> alphabets = new System.Collections.Generic.List<char>()
            {
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k',
                'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
            };
            List<int> numbers = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            string username =
                $"6{GetRandomNumber()}{GetRandomLetter()}{GetRandomLetter()}{GetRandomLetter()}{GetRandomLetter()}...{GetRandomLetter()}{GetRandomLetter()}{GetRandomLetter()}{GetRandomLetter()}";

            int GetRandomNumber()
            {
                return numbers[UnityEngine.Random.Range(0, 9)];
            }

            char GetRandomLetter()
            {
                return alphabets[UnityEngine.Random.Range(0, alphabets.Count)];
            }

            return username;
        }

        public void KillBot()
        {
            if (CurrentStage != Stage.Dead)
                CurrentStage = Stage.Dead;
            Life = 0;
            //UIHealth = Life;
            if (gameUI)
            {
                gameUI.UpdateHealthText(UIHealth);
            }
        }

        public void KillPlayer()
        {
            if (CurrentStage != Stage.Dead)
                CurrentStage = Stage.Dead;
            Life = 0;
            //UIHealth = Life;
            if (gameUI)
            {
                gameUI.UpdateHealthText(UIHealth);
            }
        }
    }
}