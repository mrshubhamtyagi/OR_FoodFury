using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using FoodFury;
using Fusion;
using OneRare.FoodFury.Multiplayer;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace FusionHelpers
{
    /// <summary>
    /// Base class for you per-session state class.
    /// You can use this to track and access player avatars on all peers.
    /// Override OnPlayerAvatarAdded/Removed to be notified of players joining/leaving *after* their avatar is created or removed.
    /// Use GetPlayer/GetPlayerByIndex/AllPlayers to access or iterate over players on all peers.
    /// Use Runner.GetSingleton/Runner.WaitForSingleton to get your custom session instance on all peers.
    /// </summary>

    public abstract class FusionSession : NetworkBehaviour
    {
        private const int MAX_PLAYERS = 4;

        [SerializeField] private FusionPlayer _playerPrefab;
        [SerializeField] private FusionPlayer _botPrefab;
        [SerializeField] private Image _waitingPlayerLoaderImage;
        [SerializeField] public float duration = 40f;
        [Networked] public TickTimer LobbyTimer { get; set; }
        [Networked] public float FillAmount { get; set; }
        [Networked, Capacity(MAX_PLAYERS)] public NetworkDictionary<int, PlayerRef> playerRefByIndex { get; }
        private Dictionary<PlayerRef, FusionPlayer> _players = new();

        protected abstract void OnPlayerAvatarAdded(FusionPlayer fusionPlayer);
        protected abstract void OnPlayerAvatarRemoved(FusionPlayer fusionPlayer);

        public List<FusionPlayer> AllPlayers => _players.Values.ToList();
        public int SessionCount => playerRefByIndex.Count;
        public int PlayerCount => _players.Count;

        private static int botPlayerId = 100;
        private Tween _loaderTween;
        [Networked] private bool InitBot { get; set; }
        private ChangeDetector changeDetector;
        public override void Spawned()
        {
            Debug.Log($"Spawned Network Session for Runner: {Runner}");
            Runner.RegisterSingleton(this);
            // Ensure the fill amount starts at 0
            _waitingPlayerLoaderImage.fillAmount = 0;
            changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
            if (Runner.IsSharedModeMasterClient)
            {
                LobbyTimer = TickTimer.CreateFromSeconds(Runner, duration);
            }
            
        }

        public override void Render()
        {
            if (Runner && Runner.Topology == Topologies.Shared && PlayerCount != playerRefByIndex.Count)
                MaybeSpawnNextAvatar();

            foreach (var change in changeDetector.DetectChanges(this))
            {
                switch (change)
                {
                    case nameof(FillAmount):
                        SetFillAmount(FillAmount);
                        break;
                }
            }
        }
        private void SetFillAmount(float fillAmount)
        {
            _waitingPlayerLoaderImage.fillAmount = fillAmount;
        }


        public override void FixedUpdateNetwork()
        {
            if (LobbyTimer.Expired(Runner) == false && LobbyTimer.RemainingTime(Runner).HasValue)
            {
                var remainingTime = LobbyTimer.RemainingTime(Runner).Value;
               
                float fillAmount = (float) (duration - remainingTime) / duration;
                FillAmount = fillAmount;
                _waitingPlayerLoaderImage.fillAmount = FillAmount;
            
            }
            
        }

        private int botID = 0;
        private void MaybeSpawnNextAvatar(bool isBot = false)
        {
            if (isBot)
            {
                foreach (KeyValuePair<int, PlayerRef> refByIndex in playerRefByIndex)
                {
                    if (Runner.IsServer || (Runner.Topology == Topologies.Shared))
                    {
                        //Debug.Log($"-------Spawning BOT-  _players.Count:{_players.Count} | refByIndex.Key:{refByIndex.Key} | refByIndex.Value.PlayerId:{refByIndex.Value.PlayerId} | _players.Keys.Contains:{_players.Keys.Contains(refByIndex.Value)}");
                        if (!_players.TryGetValue(refByIndex.Value, out _))
                        {
                            Debug.Log($"-------Spawning BOT IF {refByIndex.Key} | {refByIndex.Value}");
                            Runner.Spawn(_botPrefab, Vector3.zero, Quaternion.identity, refByIndex.Value, (runner, o) =>
                            {
                                Runner.SetPlayerObject(refByIndex.Value, o);
                                FusionPlayer player = o.GetComponent<FusionPlayer>();
                                if (player != null)
                                {
                                    player.NetworkedPlayerIndex = refByIndex.Key;
                                    player.InitNetworkState();
                                    player.GetComponent<Player>().IsBot = true;
                                    player.GetComponent<Player>().botID = botID;
                                    botList.Add(player);
                                    botID++;
                                }
                            });
                        }
                        //else Debug.Log($"-------MaybeSpawnNextAvatar ALREADY PRESENT in PLAYERS - {refByIndex.Key}");
                    }
                    //else Debug.Log($"-------MaybeSpawnNextAvatar NOT SERVER - {refByIndex.Key}");
                }
            }
            else
            {
                foreach (KeyValuePair<int, PlayerRef> refByIndex in playerRefByIndex)
                {
                    if (Runner.IsServer || (Runner.Topology == Topologies.Shared && refByIndex.Value == Runner.LocalPlayer))
                    {
                        if (!_players.TryGetValue(refByIndex.Value, out _))
                        {
                            Debug.Log($"-------Spawning PLAYER {refByIndex.Key} | {refByIndex.Value}");
                            Runner.Spawn(_playerPrefab, Vector3.zero, Quaternion.identity, refByIndex.Value, (runner, o) =>
                            {
                                Runner.SetPlayerObject(refByIndex.Value, o);
                                FusionPlayer player = o.GetComponent<FusionPlayer>();
                                if (player != null)
                                {
                                    player.NetworkedPlayerIndex = refByIndex.Key;
                                    player.InitNetworkState();
                                }
                            });
                        }
                    }
                }
            }


        }


        public void AddPlayerAvatar(FusionPlayer fusionPlayer)
        {
            Debug.Log($"Adding PlayerRef {fusionPlayer.PlayerId}");
            _players[fusionPlayer.PlayerId] = fusionPlayer;

            if (gameManager == null)
            {
                if (!Runner.TryGetSingleton(out gameManager))
                {
                    Debug.Log("Game Manager is null!");
                    return;
                }
            }

            OnPlayerAvatarAdded(fusionPlayer);
        }

        public void RemovePlayerAvatar(FusionPlayer fusionPlayer)
        {
            Debug.Log($"Removing PlayerRef {fusionPlayer.PlayerId} | SessionCount: {SessionCount} | {gameManager == null}");
            _players.Remove(fusionPlayer.PlayerId);
            if (Object != null && Object.IsValid)
                playerRefByIndex.Remove(fusionPlayer.PlayerIndex);


            if (SessionCount <= MAX_PLAYERS - 1 && gameManager != null && gameManager.CurrentPlayState == GameManager.PlayState.LOBBY)
            {
                Debug.Log("---ROOM REOPENED");
                Runner.SessionInfo.IsOpen = true;
                Runner.SessionInfo.IsVisible = true;
                InitBot = false;
                Invoke(nameof(CheckAndSpawnBots), GetBotRandomInvervalTime());
            }

            OnPlayerAvatarRemoved(fusionPlayer);
        }

        public T GetPlayer<T>(PlayerRef plyRef) where T : FusionPlayer
        {
            _players.TryGetValue(plyRef, out FusionPlayer ply);
            return (T)ply;
        }

        public T GetPlayerByIndex<T>(int idx) where T : FusionPlayer
        {
            foreach (FusionPlayer player in _players.Values)
            {
                if (player.Object != null && player.Object.IsValid && player.PlayerIndex == idx)
                    return (T)player;
            }
            return default;
        }

        private int NextPlayerIndex()
        {
            //Debug.LogWarning($"NextPlayerIndex ---- {Runner.Config.Simulation.PlayerCount}");
            for (int idx = 0; idx < Runner.Config.Simulation.PlayerCount; idx++)
            {
                if (!playerRefByIndex.TryGet(idx, out _))
                    return idx;
            }
            Debug.Log("No free player index!");
            return -1;
        }

        public void PlayerLeft(PlayerRef playerRef)
        {
            Debug.Log($"Player {playerRef} Left");
            if (Runner == null)
            {
                //SceneManager.SwitchToHomeScene();

                /*Loader.Instance.ShowLoader();
				SceneManager.SwitchToHomeScene(_callback: (op) =>
				{
					ScreenManager.Instance.SwitchScreen(HomeScreen.Instance, null);
				});*/

                /*App app = FindObjectOfType<App>();
					app.ReEnterRoom();
				
				Destroy(app.gameObject);*/

                /*LocalObjectPool localObjectPool = FindObjectOfType<LocalObjectPool>();
				Destroy(localObjectPool.gameObject);*/
                //Destroy(app.gameObject);

                return;
            }
            if (!Runner.IsShutdown)
            {
                FusionPlayer player = GetPlayer<FusionPlayer>(playerRef);
                if (player && player.Object.IsValid)
                {
                    Debug.Log($"Despawning PlayerAvatar for PlayerRef {player.PlayerId}");
                    Runner.Despawn(player.Object);
                }
            }

        }

        public void PlayerJoined(PlayerRef player)
        {
            Debug.Log($"PlayerJoined: Before {player} | PlayerCount : {PlayerCount} | SessionCount: {SessionCount}");

            if (SessionCount >= MAX_PLAYERS) // Last Player Joined
            {
                Debug.Log($"---ROOM FULL COUNT: PlayerCount : {PlayerCount} | SessionCount: {SessionCount} | Joined Player - {player}");  // Disconnect Joined Player
                Runner.SessionInfo.IsOpen = false;
                Runner.SessionInfo.IsVisible = false;
                RPC_OnPlayerJoinedExtra(player);
            }
            else
            {
                int nextIndex = NextPlayerIndex();
                playerRefByIndex.Set(nextIndex, player);
                MaybeSpawnNextAvatar();

                if (!InitBot)
                {

                    botsSpawned = 0;
                    Invoke(nameof(CheckAndSpawnBots), GameData.Instance.MultiplayerGameConfig.botInitialSpawnTime); // Schedule bot check after 20 seconds
                }

                //Debug.Log($"PlayerJoined: After {player} | PlayerCount : {PlayerCount} | SessionCount: {SessionCount}");
            }
        }



        private List<FusionPlayer> botList = new List<FusionPlayer>();
        private int numBotsToBeSpawned = 3; // Total bots to spawn, if needed
        private Vector2Int botSpawnIntervalRange = new Vector2Int(3, 6);
        int botsSpawned = 0;
        GameManager gameManager;
        private void CheckAndSpawnBots()
        {
            if (InitBot) return;
            Debug.Log($"---------------CheckAndSpawnBots PlayerCount : {PlayerCount} | SessionCount: {SessionCount}");

            InitBot = true;

            if (gameManager.CurrentPlayState != GameManager.PlayState.LOBBY)
            {
                Debug.Log("Game State is Not LOBBY");
                return;
            }

            //int currentPlayerCount = PlayerCount;
            //int botsNeeded = MAX_PLAYERS - currentPlayerCount; // Number of bots to reach max players

            if (SessionCount >= MAX_PLAYERS - 1) // Last Bot to be spawned
            {
                StartCoroutine(SpawnBotsGradually());
                Runner.SessionInfo.IsOpen = false;
                Runner.SessionInfo.IsVisible = false;
                Debug.Log($"---ROOM FULL COUNT: PlayerCount : {PlayerCount} | SessionCount: {SessionCount}");
            }
            else
                StartCoroutine(SpawnBotsGradually());
        }



        private IEnumerator SpawnBotsGradually()
        {
            for (int i = 0; i < MAX_PLAYERS - 1; i++)
            {
                if (SessionCount >= MAX_PLAYERS)
                {
                    Runner.SessionInfo.IsOpen = false;
                    Runner.SessionInfo.IsVisible = false;
                    Debug.Log($"---ROOM FULL COUNT: PlayerCount : {PlayerCount} | SessionCount: {SessionCount}");
                    yield break;
                }

                if (gameManager.CurrentPlayState != GameManager.PlayState.LOBBY)
                {
                    Debug.Log($"---Game Has Started");
                    yield break;
                }

                int nextIndex = botPlayerId++;
                PlayerRef bot = PlayerRef.FromIndex(nextIndex);
                Debug.Log($"SpawnBotsGradually ----- {bot} Current Players: {PlayerCount}, Bots Spawned: {botsSpawned}, Total Players: {PlayerCount + botsSpawned}");

                playerRefByIndex.Set(nextIndex, bot);

                yield return new WaitForEndOfFrame();
                MaybeSpawnNextAvatar(true);

                if (SessionCount >= MAX_PLAYERS)
                {
                    Runner.SessionInfo.IsOpen = false;
                    Runner.SessionInfo.IsVisible = false;
                    Debug.Log($"---ROOM FULL COUNT: PlayerCount : {PlayerCount} | SessionCount: {SessionCount}");
                    yield break;
                }

                yield return new WaitForSeconds(GetBotRandomInvervalTime()); // Wait before attempting to spawn the next bot
            }
        }

        private int GetBotRandomInvervalTime() => Random.Range(botSpawnIntervalRange.x, botSpawnIntervalRange.y);


        [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
        private void RPC_OnPlayerJoinedExtra(PlayerRef _player)
        {
            if (Runner.LocalPlayer == _player)
            {
                Debug.Log($"I Am the Extra Player {_player}");
                Runner.Shutdown(shutdownReason: ShutdownReason.ConnectionTimeout);
            }
        }
        
        private void OnDestroy()
        {
            // Kill the tween safely when the object is destroyed
            if (_loaderTween != null && _loaderTween.IsActive())
            {
                _loaderTween.Kill();
            }
        }
    }
}