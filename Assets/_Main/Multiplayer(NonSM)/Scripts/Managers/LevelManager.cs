using System;
using System.Collections;
using System.Linq;
using FoodFury;
using Fusion;
using FusionHelpers;
using UnityEngine;
using SceneManagerUnity = UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using System.Threading.Tasks;

namespace OneRare.FoodFury.Multiplayer
{
    //Adding A few comments here to resolve git issue
    /// <summary>
    /// The LevelManager controls the map - keeps track of spawn points for players.
    /// </summary>
    /// TODO: This is partially left over from previous SDK versions which had a less capable SceneManager, so could probably be simplified quite a bit
    public class LevelManager : NetworkSceneManagerDefault
    {
        public ReadyUpManager readyUpManager;

        [Header("General Settings")]
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private CountdownManager countdownManager;
        [SerializeField] private int lobby;
        [SerializeField] private int[] levels;
        [SerializeField] private PowerUpSpawnManager powerupSpawnManagerPrefab;
        [SerializeField] private ChallengeManager _challengeManagerPrefab;
        [SerializeField] private CounterObject _counterObjectPrefab;
        [SerializeField] private GameObject startButton;
        private LevelBehaviour currentLevel;
        private SceneRef loadedScene = SceneRef.None;

        public Action<NetworkRunner, FusionLauncher.ConnectionStatus, string> onStatusUpdate { get; set; }
        public ReadyUpManager ReadyUpManager => readyUpManager;

        public CountdownManager CountdownManager => countdownManager;

        private void Awake()
        {
            countdownManager.Reset();
            startButton.SetActive(false);
            //			_scoreManager.ResetAllGameScores();
        }


        public override void Shutdown()
        {
            currentLevel = null;
            if (loadedScene.IsValid)
            {
                SceneManagerUnity.SceneManager.UnloadSceneAsync(loadedScene.AsIndex);
                loadedScene = SceneRef.None;
            }

            scoreManager.ResetAllGameScores();
            base.Shutdown();
        }

        // Get a random level
        public int GetRandomLevelIndex()
        {
            int idx = Random.Range(0, levels.Length);
            // Make sure it's not the same level again. This is partially because it's more fun to try different levels and partially because scene handling breaks if trying to load the same scene again.
            if (levels[idx] == loadedScene.AsIndex)
                idx = (idx + 1) % levels.Length;
            return idx;
        }

        public SpawnPoint GetPlayerSpawnPoint(int playerIndex)
        {
            if (currentLevel != null)
                return currentLevel.GetPlayerSpawnPoint(playerIndex);
            return null;
        }

        public void LoadLevel(int nextLevelIndex)
        {
            //GameManager.IsGamePaused = true;
            //print($"LoadLevel IsGamePaused - {GameManager.IsGamePaused}");

            currentLevel = null;
            if (loadedScene.IsValid)
            {
                UnloadScene(loadedScene);
                loadedScene = SceneRef.None;
            }

            if (nextLevelIndex == 3)
            {
                Runner.LoadScene(SceneRef.FromIndex(lobby), new SceneManagerUnity.LoadSceneParameters(SceneManagerUnity.LoadSceneMode.Additive));
            }
            else
            {
                Runner.LoadScene(SceneRef.FromIndex(levels[nextLevelIndex]),
                    new SceneManagerUnity.LoadSceneParameters(SceneManagerUnity.LoadSceneMode.Additive));
            }
        }

        protected override IEnumerator UnloadSceneCoroutine(SceneRef prevScene)
        {

            GameManager gameManager;
            while (!Runner.TryGetSingleton(out gameManager))
            {
                yield return null;
            }

            if (Runner.IsServer || Runner.IsSharedModeMasterClient)
                gameManager.CurrentPlayState = GameManager.PlayState.TRANSITION;

            if (prevScene.AsIndex > 0)
            {
                yield return new WaitForSeconds(1.0f);

                InputController.fetchInput = false;

                foreach (FusionPlayer fusionPlayer in gameManager.AllPlayers)
                {
                    Player player = (Player)fusionPlayer;
                    player.TeleportOut();
                    yield return new WaitForSeconds(0.1f);
                }

                yield return new WaitForSeconds(1.5f - gameManager.PlayerCount * 0.1f);

                scoreManager.ResetAllGameScores();
                if (gameManager.LastPlayerStanding != null)
                {
                    scoreManager.ShowIntermediateLevelScore(gameManager);
                    yield return new WaitForSeconds(1.5f);
                    scoreManager.ResetAllGameScores();
                }
            }

            yield return base.UnloadSceneCoroutine(prevScene);
        }

        private bool alreadyActivated = false;
        protected override IEnumerator OnSceneLoaded(SceneRef newScene, SceneManagerUnity.Scene loadedScene, NetworkLoadSceneParameters sceneFlags)
        {
            yield return base.OnSceneLoaded(newScene, loadedScene, sceneFlags);

            InputController.fetchInput = false;
            if (newScene.AsIndex == 0) yield break;
            yield return new WaitForSeconds(0.1f);

            onStatusUpdate?.Invoke(Runner, FusionLauncher.ConnectionStatus.Loading, "");
            yield return null;
            this.loadedScene = newScene;
            yield return null;
            onStatusUpdate?.Invoke(Runner, FusionLauncher.ConnectionStatus.Loaded, "");


            currentLevel = FindFirstObjectByType<LevelBehaviour>();
            if (currentLevel != null && alreadyActivated == false)
            {
                currentLevel.Activate();
                alreadyActivated = true;
            }
            yield return new WaitForSeconds(0.1f);

            GameManager gameManager;
            while (!Runner.TryGetSingleton(out gameManager))
            {
                Debug.Log($"Waiting for GameManager to Spawn!");
                yield return null;
            }

            for (int i = 0; i < gameManager.AllPlayers.Count(); i++)
            {
                Player player = (Player)gameManager.AllPlayers[i];
                //Debug.Log($"Initiating Respawn of Player #{player.PlayerIndex} ID:{player.PlayerId}:{player}");
                player.Reset();
                player.Respawn();
                yield return new WaitForSeconds(0.1f);
            }

            if (this.loadedScene.AsIndex == 2 || this.loadedScene.AsIndex == 3)
            {
                OnLobbyAndMainSceneLoaded(gameManager);
                //		    Debug.Log($"Switched Scene from {prevScene} to {newScene}");
            }
            else
            {
                OnGameSceneLoaded(gameManager);
            }
        }

        private void OnLobbyAndMainSceneLoaded(GameManager gameManager)
        {
            if (Runner.IsServer || Runner.IsSharedModeMasterClient)
                gameManager.CurrentPlayState = GameManager.PlayState.LOBBY;

            InputController.fetchInput = true;
        }
        private void OnGameSceneLoaded(GameManager gameManager)
        {
            if (Runner != null && (Runner.IsServer || Runner.IsSharedModeMasterClient))
            {
                gameManager.CurrentPlayState = GameManager.PlayState.LEVEL;
                /*if (_challengeManager != null)
                    _challengeManager.StartChallenge(ChallengeType.RaceToDeliveries);*/
                SpawnChallengeManager(Runner);
                SpawnPowerupSpawner(Runner);
                //SpawnCounterObject(Runner);
            }


            //if (ChallengeManager.Instance == null)
            //{
            //    Debug.Log("ChallengeManager.Instance is NULLLL------");
            //    return;
            //}

            //ChallengeManager.Instance.ShowCountdownOnLevelStart(countdownManager);
            //Loader.Instance.HideLoader();
            //StartCoroutine(countdownManager.Countdown(() =>
            //{
            //}));
            //InputController.fetchInput = true;
        }

        public void SpawnChallengeManager(NetworkRunner runner)
        {
            var point = TileManager.Instance.transform;

            //var prefabId = player.KartId;
            var prefab = _challengeManagerPrefab;

            // Spawn player
            var entity = runner.Spawn(
                prefab,
                point.position,
                point.rotation
            );

            //InputController.fetchInput = true;
        }

        public void SpawnPowerupSpawner(NetworkRunner runner)
        {
            var point = TileManager.Instance.transform;

            //var prefabId = player.KartId;
            var prefab = powerupSpawnManagerPrefab;

            var entity = runner.Spawn(
                prefab,
                point.position,
                point.rotation
            );
        }
    }
}