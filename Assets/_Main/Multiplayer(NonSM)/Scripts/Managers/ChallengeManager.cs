using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FoodFury;
using Fusion;
using FusionHelpers;
using Mono.CSharp;
using Newtonsoft.Json;
using Unity.Burst.Intrinsics;
using UnityEngine;
using Tile = FoodFury.Tile;

namespace OneRare.FoodFury.Multiplayer
{
    public enum ChallengeType
    {
        RaceToDeliveries,
        RaceToPoints,
        MostPointsInTime,
        RaceToCollectItems
    }

    public class ChallengeManager : NetworkBehaviour, ISceneLoadDone
    {
        public static ChallengeManager Instance;
        public event Action<ChallengeType> OnChallengeStarted;
        public event Action OnTimerEnd;
        public event Action OnActiveOrdersUpdated;

        public static event Action OnGameOver;

        [Header("General Settings")] public Texture2D defaultDishImage;
        [SerializeField] private GameObject orderPrefab;
        [SerializeField] private ModelClass.Dish dishData;
        [SerializeField] private ModelClass.Dish dummyOrder;
        [SerializeField] private bool spawnUserDish;
        [SerializeField] private OrderMP orderPrefabSP;
        [SerializeField] private EndRaceUI endRaceUIPrefab;
        [SerializeField] private SplashUIItem splashUIItemPrefab;
        [field: Header("Networked Properties")]
        [Networked]
        public string Time { get; set; } = "00:00";

        [Networked] public TickTimer MatchTimer { get; set; }
        [Networked] public int RandomIndex { get; set; }
        [Networked] public Vector3 OrderPosition { get; set; } = Vector3.zero;
        [Networked] public bool IsRoundOver { get; set; }
        [Networked] public bool IsMatchOver { get; set; } = false;
        [Networked] public bool IsMatchStarted { get; set; } = false;
        [Networked] public int CurrentEliminationRound { get; set; } = 0;
        [Networked] public float FillAmount { get; set; }

        [Networked] private Player EliminatedPlayer { get; set; }
        [Networked] private Player LastPlayerStanding { get; set; }
        #region PRIVATE VARIABLES

        [Networked] private bool IsChallengeActive { get; set; }
        private InGameUIHUD gameUI;
        private ChangeDetector changeDetector;
        private float initiationDuration = 45f;//35f; // 75 seconds initiation
        private float eliminationDuration = 60f;//30f; // 60 seconds for each elimination round
        private GameManager gameManager;
        private LevelManager levelManager;
        private OrderUIItem[] orderUICards;
        private int maxPlayers = 4;
        private PlayerAIRivalMP[] _playerAIRivalMp;
        public List<OrderMP> activeOrders = new List<OrderMP>();
        #endregion

        private void Awake()
        {
            Instance = this;
            initiationDuration = GameData.Instance.MultiplayerGameConfig.initiationDuration;
            eliminationDuration = GameData.Instance.MultiplayerGameConfig.eliminationDuration;
        }

        public override void Spawned()
        {
            changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
            gameUI = FindObjectOfType<InGameUIHUD>();
            gameManager = FindObjectOfType<GameManager>();
            levelManager = FindObjectOfType<LevelManager>();

            Debug.Log("ChallengeManager.Instance is Spawned");
            if (HasStateAuthority) RPC_ShowCountdownOnLevelStart();
            spawnUserDish = false;
        }

        public override void Render()
        {
            foreach (var change in changeDetector.DetectChanges(this))
            {
                switch (change)
                {
                    case nameof(OrderPosition):
                        SetTargetObject(OrderPosition);
                        break;
                    case nameof(RandomIndex):
                        SetRandomIndex(RandomIndex);
                        break;
                    case nameof(Time):
                        SetTime(Time, FillAmount);
                        break;
                    case nameof(IsMatchOver):
                        SetGameOver();
                        break;
                    case nameof(IsRoundOver):
                        SetRoundOver();
                        break;
                    case nameof(CurrentEliminationRound):
                        SetCurrentRound(CurrentEliminationRound);
                        break;
                    case nameof(FillAmount):
                        SetTime(Time, FillAmount);
                        break;
                    case nameof(EliminatedPlayer):
                        Debug.Log($"SetEliminatedPlayer ONRENDER: {EliminatedPlayer} | {EliminatedPlayer == null}");
                        SetEliminatedPlayer(EliminatedPlayer);
                        break;
                    case nameof(LastPlayerStanding):
                        Debug.Log($"LastPlayerStanding ONRENDER: {LastPlayerStanding} | {LastPlayerStanding == null}");
                        SetLastPlayerStanding(LastPlayerStanding);
                        break;
                    case nameof(IsChallengeActive):
                        SetIsChallengeActive(IsChallengeActive);
                        break;
                }
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (MatchTimer.Expired(Runner) == false && MatchTimer.RemainingTime(Runner).HasValue)
            {
                var remainingTime = MatchTimer.RemainingTime(Runner).Value;
                var timeSpan = TimeSpan.FromSeconds(MatchTimer.RemainingTime(Runner).Value);
                var outPut = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
                Time = outPut;

                // Calculate the fill amount
                float fillAmount = (float)remainingTime /
                                   (IsChallengeActive ? eliminationDuration : initiationDuration);

                FillAmount = fillAmount;
            }
            else if (MatchTimer.Expired(Runner))
            {
                MatchTimer = TickTimer.None;
                if (!IsChallengeActive)
                {
                    StartEliminationRounds();
                }
                else
                {
                    HandleEliminationRound();
                }
            }

        }

        public void SetIsChallengeActive(bool isActive)
        {
            IsChallengeActive = isActive;
        }
        public void SetEliminatedPlayer(Player eliminatedPlayer)
        {
            EliminatedPlayer = eliminatedPlayer;
        }
        public void SetLastPlayerStanding(Player lastPlayerStanding)
        {
            //Debug.Log($"SetLastPlayerStanding: {lastPlayerStanding} | {lastPlayerStanding == null}");
            LastPlayerStanding = lastPlayerStanding;
        }
        public ModelClass.Dish GetDishData(int tokenId = -1)
        {
            ModelClass.Dish _dish = null;
            if (tokenId == -1)
                _dish = GetRandomDishDataMP();
            else
            {
                _dish = GameData.Instance.GetFilteredDishes().Find(d => d.tokenId == tokenId);
            }

            return _dish == null ? dummyOrder : _dish;
        }

        public ModelClass.Dish GetRandomDishDataMP()
        {
            var list = GameData.Instance.GetFilteredDishes();
            if (list.Count == 0)
                return null;
            return list[UnityEngine.Random.Range(0, list.Count)];
        }


        public void DestroyInstance()
        {
            Destroy(gameObject);
        }

        void StartChallengeAfterDelay()
        {


            if (!Object.HasStateAuthority)
                return;

            StartChallenge(ChallengeType.RaceToPoints);
        }

        public void StartChallenge(ChallengeType challengeType)
        {
            OnChallengeStarted?.Invoke(challengeType);
            MatchTimer = TickTimer.CreateFromSeconds(Runner, initiationDuration);
            switch (challengeType)
            {
                case ChallengeType.RaceToDeliveries:
                    Invoke("StartRaceToDeliveries", 1f);
                    break;
                case ChallengeType.RaceToPoints:
                    Invoke("StartRaceToPoints", 0.2f);
                    break;
                default:
                    Debug.LogWarning("Unknown challenge type!");
                    break;
            }
        }

        public void SetTargetObject(Vector3 newTarget)
        {
            OrderPosition = newTarget;
        }

        public void SetRandomIndex(int index)
        {
            RandomIndex = index;
        }

        public void SetCurrentRound(int index)
        {
            CurrentEliminationRound = index;
        }


        private void StartEliminationRounds()
        {
            // Set isChallengeActive to true after Round 1 (Initiation Round)
            IsChallengeActive = true;

            StartNextEliminationRound(); // Start Round 2, the first elimination round
        }

        private void StartNextEliminationRound()
        {
            CurrentEliminationRound++; // Move to the next round
            if (CurrentEliminationRound <= maxPlayers) // Adjusted to match the new round count
            {
                if (Object.HasStateAuthority)
                {
                    MatchTimer = TickTimer.CreateFromSeconds(Runner, eliminationDuration);
                    UIManager.Instance.RPC_UpdateRoundsOnTimer(CurrentEliminationRound);
                    // Display message for the next elimination round
                     UIManager.Instance.RPC_ShowInterimUIForRounds($"Strap in for Round {CurrentEliminationRound}   <sprite=66> ", CurrentEliminationRound);
                }
            }
            else
            {
                EndMatch(); // End the match after the last round
            }

            Invoke("SetRoundOverFalse", 2f);
        }

        void SetRoundOverFalse()
        {
            IsRoundOver = false;
        }

        /*
        private Player lastPlayerStanding = null;
        private Player */

        public void CheckGameOverCondition()
        {
            if (IsRoundOver)
                return;

            if (Object.HasStateAuthority)
            {
                int remainingPlayersCount = GetActivePlayersCount();
                if (remainingPlayersCount >= 2)
                {
                    Debug.Log("More Than 2 Players");
                }
                else
                {
                    //EliminatedPlayer = PlayerToBeEliminated();
                    LastPlayerStanding = GetLastPlayerStanding();
                    Debug.Log($"-----CheckGameOverCondition LP - {LastPlayerStanding}");
                    EndMatch();
                }

                RPC_ShowSplashPopUp();
                RPC_ShowResultScreen();
                Invoke("Invoke_RedrawLeaderboardList", 1.5f);
            }
        }
        private void HandleEliminationRound()
        {
            if (IsMatchOver)
                return;
            IsRoundOver = true;
            if (Object.HasStateAuthority)
            {
                int remainingPlayersCount = GetActivePlayersCount();
                if (CurrentEliminationRound < 2)
                {
                    StartNextEliminationRound();
                    return;
                }

                if (remainingPlayersCount > 2)
                {
                    EliminatedPlayer = PlayerToBeEliminated();
                    StartNextEliminationRound();
                }
                else
                {
                    EliminatedPlayer = PlayerToBeEliminated();
                    LastPlayerStanding = GetLastPlayerStanding();
                    EndMatch();
                }

                Debug.Log($"-----HandleEliminationRound - EP - {EliminatedPlayer} | LP -{LastPlayerStanding}");
                RemoveEliminatedPlayer();
                RPC_ShowSplashPopUp();
                RPC_ShowResultScreen();
                Invoke("EnsureEliminatedPlayerRemoved", 4f); // 2.1f
                Invoke("Invoke_RedrawLeaderboardList", 1.5f);
            }
        }

        public void Invoke_RedrawLeaderboardList() => UIManager.Instance.RPC_RedrawLeaderboardList();


        private int GetActivePlayersCount()
        {
            int _count = Player.Players.Where(x => x.CurrentStage == Player.Stage.Active).Count();
            //Debug.Log($"-----GetActivePlayersCount - {_count}");
            return _count;
        }

        private void RemoveEliminatedPlayer()
        {
            if (EliminatedPlayer != null)
            {
                // Eliminate the player with the lowest score
                //EliminatedPlayer.EliminatePlayer();
                UIManager.Instance.RPC_ShowInterimUIForRounds($"{EliminatedPlayer.Username} Eliminated", CurrentEliminationRound);
                Invoke("RemovePlayer", 0.1f); // 2.1f
            }
        }

        void RemovePlayer()
        {
            if (EliminatedPlayer != null)
            {
                if (EliminatedPlayer.IsBot)
                {
                    EliminatedPlayer.KillBot();
                }
                else
                {
                    EliminatedPlayer.RaiseEvent(new Player.DamageEvent { impulse = Vector3.zero, damage = 1000 });
                    EliminatedPlayer.KillPlayer();
                }
               
            }
        }
        
        private void EnsureEliminatedPlayerRemoved()
        {
            if (EliminatedPlayer != null && Player.Players.Contains(EliminatedPlayer))
            {
                if (EliminatedPlayer.CurrentStage != Player.Stage.Dead)
                {
                    Debug.Log($"-----Ensuring {EliminatedPlayer.Username} is removed. Retrying elimination...");
                    RemovePlayer();
                    Invoke("Invoke_RedrawLeaderboardList", 1.5f);
                }
            }
        }


        public static Player GetPlayerWithLowestScore()
        {
            var _activePlayersList = Player.LeaderboardPlayers.Where(p => p.CurrentStage == Player.Stage.Active).ToList();
            if (_activePlayersList.Count <= 0) return null;

            int minScore = _activePlayersList.Min(x => x.Score);
            var playersWithMinScore = _activePlayersList.Where(x => x.Score == minScore).ToList();
            if (playersWithMinScore.Count <= 0) return null;

            return playersWithMinScore.Last();
        }

        private Player PlayerToBeEliminated()
        {
            if (Player.Players.Count < 2) return null;
            Player playerWithLowestScore = null;
            if (Object.HasStateAuthority)
            {
                playerWithLowestScore = GetPlayerWithLowestScore();
                if (playerWithLowestScore != null)
                    Debug.Log($"-----EliminatePlayer  {playerWithLowestScore.Username}");
                else
                    Debug.Log($"-----EliminatePlayer IS NULL");
            }

            return playerWithLowestScore;
        }

        private Player GetLastPlayerStanding()
        {
            var _player = Player.LeaderboardPlayers.FirstOrDefault(p => p.CurrentStage == Player.Stage.Active && p != EliminatedPlayer);
            //Debug.Log($"GetLastPlayerStanding - {_player} | {_player == null}");
            return _player;
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_ShowResultScreen()
        {
            StartCoroutine(Co_ShowResultScreen(LastPlayerStanding));
        }

        private EndRaceUI endRaceUI;
        public IEnumerator Co_ShowResultScreen(Player _lastPlayerStanding)
        {
            yield return new WaitForSeconds(1.5f);
            print($"RPC_ShowResultScreen: EP - {EliminatedPlayer} | LP - {_lastPlayerStanding}");
            if (Player.Local.CurrentStage == Player.Stage.Dead || Player.Local == EliminatedPlayer || Player.Local == _lastPlayerStanding)
            {
                if (FindObjectOfType<EndRaceUI>() == null)
                {
                    endRaceUI = Instantiate(endRaceUIPrefab);
                    endRaceUI.Init();
                    endRaceUI.UpdateResultScreen(Player.Local, false);

                }
            }
        }
        
        private SplashUIItem splashUIItem;
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_ShowSplashPopUp()
        {
            print($"RPC_ShowSplashPopUp: EP - {EliminatedPlayer} | LP - {LastPlayerStanding}");

            if (Player.Local.CurrentStage == Player.Stage.Dead || Player.Local == EliminatedPlayer)
            {
                splashUIItem = Instantiate(splashUIItemPrefab);
                splashUIItem.ShowEliminated();
            }

            else if (Player.Local == LastPlayerStanding)
            {
                splashUIItem = Instantiate(splashUIItemPrefab);
                splashUIItem.ShowVictory();
            }

        }

        private void EndMatch()
        {
            OnTimerEnd?.Invoke();
            IsMatchOver = true;
            OnGameOver?.Invoke();
        }

        public void SpawnNextOrder()
        {
            if (!Object.HasStateAuthority)
                return;

            Invoke("SpawnOrderForMP", 0.5f);
        }

        private int containerIndex = 0;

        void SpawnOrderForMP()
        {
            if (activeOrders.Count >= 3 || IsMatchOver)
                return;

            // Find the first available index (0, 1, or 2)
            int availableIndex = -1;
            for (int i = 0; i < 3; i++)
            {
                if (!activeOrders.Any(order => order.ContainerIndex == i))
                {
                    availableIndex = i;
                    break;
                }
            }

            if (availableIndex == -1)
                return; // No available index, should not happen

            Tile tileOne = TileManager.Instance.GetRandomTileInRange(50, 180);
            if (tileOne == null)
            {
                Invoke("SpawnOrderForMP", 2f);
                return;
            }
            int tileIndex = tileOne.Index;
            ModelClass.Dish dishOne = GetDishData();
            int dishTokenId = dishOne.tokenId;
            OrderPosition = tileOne.transform.position;

            NetworkObject obj = Runner.Spawn(orderPrefab, OrderPosition, Quaternion.identity);
            OrderMP orderOne = obj.GetComponent<OrderMP>();

            //Debug.Log($"TO:----{orderOne.Dish.name}------{orderOne.Tile.name}");
            orderOne.SetContainerIndex(availableIndex); // Set the available index

            AddOrder(orderOne);
            RPC_SpawnTileAndDish(orderOne, tileIndex, dishTokenId); // Send tileIndex instead of tileOne

            StartCoroutine(SetRivalTargetOrder());
        }

        IEnumerator SetRivalTargetOrder()
        {
            yield return new WaitForSeconds(1.5f);
            _playerAIRivalMp = FindObjectsOfType<PlayerAIRivalMP>();

            // Ensure there are active orders and AI rivals
            if (activeOrders == null || activeOrders.Count == 0 || _playerAIRivalMp.Length == 0)
                yield break;

            int orderIndex = 0;

            for (int i = 0; i < _playerAIRivalMp.Length; i++)
            {
                // Assign orders to AI rivals in a cyclic manner
                _playerAIRivalMp[i].targetOrder = activeOrders[orderIndex];
                _playerAIRivalMp[i].GeneratePath();

                // Move to the next order (wrap around if necessary)
                orderIndex = (orderIndex + 1) % activeOrders.Count;
            }
        }


        public void UpdateRivalTargetOrder()
        {
            StartCoroutine(SetRivalTargetOrder());
        }

        public void AddOrder(OrderMP order)
        {
            if (Object.HasStateAuthority)
            {
                RPC_AddOrder(order);
            }

            // Ensure we only add orders if there are less than 3 active
            if (activeOrders.Count < 3)
            {
                SpawnNextOrder();
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_AddOrder(OrderMP orderOne, RpcInfo info = default)
        {
            activeOrders.Add(orderOne);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_RemoveOrderFromUI(OrderMP order, RpcInfo info = default)
        {
            Debug.Log($"---RPC_RemoveOrderFromUI null - {order == null} | contains - {activeOrders.Contains(order)}");
            if (order == null) return;
            if (!activeOrders.Contains(order)) return;

            activeOrders.Remove(order);
        }


        private void StartRaceToDeliveries()
        {
            IsMatchStarted = true;
        }

        private void StartRaceToPoints()
        {
            IsMatchStarted = true;

            if (Object.HasStateAuthority)
            {
                // Start Round 1, which is the Initiation Round, lasting 75 seconds
                CurrentEliminationRound = 1; // Now Round 1 is the initiation round
                MatchTimer = TickTimer.CreateFromSeconds(Runner, initiationDuration);

                // Display initiation round message
                UIManager.Instance.RPC_ShowInterimUIForRounds("Strap in for Round 1   <sprite=66>", CurrentEliminationRound);
                UIManager.Instance.RPC_UpdateRoundsOnTimer(CurrentEliminationRound);

            }
            // Spawn Multiple Orders
            StartCoroutine(SpawnInGaps());
        }

        

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_ShowCountdownOnLevelStart()
        {
            if (levelManager == null)
            {
                Debug.Log("LevelManager is NULL---------------");
                return;
            }

            Loader.Instance.HideLoader();
            StartCoroutine(levelManager.CountdownManager.Countdown(() =>
            {
                if (HasStateAuthority)
                {
                    Debug.Log("Countdown END---------------");
                    Invoke("StartChallengeAfterDelay", 1f);
                }
                Invoke("FetchInputs", 1.2f);
            }));
        }

        private void FetchInputs() => InputController.fetchInput = true;

        IEnumerator SpawnInGaps()
        {
            for (int i = 0; i < 3; i++)
            {
                yield return new WaitForSeconds(1f);
                SpawnOrderForMP();
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_SpawnTileAndDish(OrderMP orderOne, int tileIndex, int dishTokenId, RpcInfo info = default)
        {
            Tile tile = TileManager.Instance.GetTileByIndex(tileIndex); // Fetch tile using the index
            if (tile == null)
            {
                Debug.LogError("Tile not found");
                return;
            }

            orderOne.SetTileAndDish(tileIndex, dishTokenId);
        }


        public void SceneLoadDone(in SceneLoadDoneArgs sceneInfo)
        {
            Debug.Log("OnSceneLoadDone");
        }

        private void SetGameOver()
        {
            /*if(HasStateAuthority)
                UIManager.Instance.RPC_ShowResultScreen(Player.Local);*/
        }

        private void SetRoundOver()
        {
            if (Object.HasStateAuthority)
            {
                UIManager.Instance.RPC_ShowInterimResult(CurrentEliminationRound);
            }
        }

        private void SetTime(string time, float fillAmount)
        {
            gameUI.UpdateTime(time, fillAmount);
        }

        public string SerializeDish(ModelClass.Dish dish)
        {
            return JsonConvert.SerializeObject(dish);
        }

        public void HandleOrderExpiration(OrderMP order)
        {
            if (HasStateAuthority)
            {
                if (order.Tile != null)
                {
                    order.Tile.hasOrder = false;
                    if (TileManager.Instance.spawnedOrderPositions.Contains(order.Tile.transform.position))
                    {
                        StartCoroutine(RemoveSpawnedOrderPositionFromList(order));
                    }
                }

                RPC_RemoveOrderFromUI(order);
                order.DespawnAfterDelay();

                // Remove the order from the activeOrders list
                //activeOrders.Remove(order);

                // Ensure we keep 3 orders active
                SpawnNextOrder();
            }
        }

        IEnumerator RemoveSpawnedOrderPositionFromList(OrderMP order)
        {
            yield return new WaitForSeconds(2f);
            TileManager.Instance.spawnedOrderPositions.Remove(order.Tile.transform.position);
        }


    }
}