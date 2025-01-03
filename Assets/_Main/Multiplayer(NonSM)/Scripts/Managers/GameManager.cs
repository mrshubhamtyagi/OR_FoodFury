using System;
using System.Collections.Generic;
using DG.Tweening;
using FoodFury;
using UnityEngine;
using Fusion;
using FusionHelpers;
using Tanknarok.UI;

//using UnityEngine.SceneManagement;

namespace OneRare.FoodFury.Multiplayer
{
    public class GameManager : FusionSession
    {
        public static bool IsGamePaused = true;

        public enum PlayState
        {
            LOBBY,
            LEVEL,
            TRANSITION,
            GAMEOVER
        }

        [Header("General Settings")]
        [SerializeField] private ForceField _forceField;
        [SerializeField] private GameObject lobbyCanvas;
        [Networked, Capacity(8)] private NetworkArray<int> score => default;
        public event Action<PlayState> OnPlayStateChanged;
        private PlayState _currentPlayState;

        [Networked]
        public PlayState CurrentPlayState
        {
            get => _currentPlayState;
            set
            {
                if (_currentPlayState != value) // Detect change
                {
                    _currentPlayState = value;
                    // Raise the event
                }
            }
        }
        public const byte MAX_SCORE = 3;
        private bool restart;
        public Player LastPlayerStanding { get; set; }
        public Player MatchWinner
        {
            get
            {
                for (int i = 0; i < score.Length; i++)
                {
                    if (score[i] >= MAX_SCORE)
                        return GetPlayerByIndex<Player>(i);
                }

                return null;
            }
        }

        public override void Spawned()
        {
            base.Spawned();
            Runner.RegisterSingleton(this);
            lobbyCanvas.SetActive(true);

            if (Object.HasStateAuthority)
            {
                LoadLevel(3);
            }
            else if (CurrentPlayState != PlayState.LOBBY)
            {
                Debug.Log("Rejecting Player, game is already running!");
                restart = true;
            }
        }

        private List<Player> players;
        protected override void OnPlayerAvatarAdded(FusionPlayer fusionPlayer)
        {
            //Runner.GetLevelManager()?.cameraStrategy.AddTarget(((Player)fusionPlayer).cameraTarget);
        }

        protected override void OnPlayerAvatarRemoved(FusionPlayer fusionPlayer)
        {
            //Runner.GetLevelManager()?.cameraStrategy.RemoveTarget(((Player)fusionPlayer).cameraTarget);
        }

        public void OnTankDeath(string _username)
        {
            ChallengeManager.Instance.CheckGameOverCondition();
        }

        public void Restart(ShutdownReason shutdownReason)
        {
            if (!Runner.IsShutdown)
            {
                // Calling with destroyGameObject false because we do this in the OnShutdown callback on FusionLauncher
                Runner.Shutdown(false, shutdownReason);
                restart = false;
            }
        }

        public const ShutdownReason ShutdownReason_GameAlreadyRunning = (ShutdownReason)100;

        private void Update()
        {
            if (CurrentPlayState == PlayState.LOBBY)
            {
                for (int i = 0; i < 3; i++)
                {
                    _forceField.SetPlayer(i, GetPlayerByIndex<Player>(i));
                }

                LevelManager lm = Runner.GetLevelManager();
                lm.readyUpManager.UpdateUI(CurrentPlayState, AllPlayers, OnAllPlayersReady);
            }
            //else lobbyCanvas.SetActive(false);

            //if (restart || Keyboard.current[Key.P].wasPressedThisFrame)
            //{
            //    Restart(restart ? ShutdownReason_GameAlreadyRunning : ShutdownReason.Ok);
            //    restart = false;
            //    ShutdownOnInput();
            //}

            if (CurrentPlayState == PlayState.LEVEL)
            {
                OnPlayStateChanged?.Invoke(_currentPlayState);
            }
        }

        public void ShutdownOnInput()
        {
            DOTween.KillAll();
            if (Runner != null)
            {
                Runner.Shutdown(true);
            }
            ErrorBox eb = FindObjectOfType<ErrorBox>();
            eb.OnClose();
            //Invoke("GoBackToHomeScreen", 1f);
            Destroy(eb.gameObject, 0.1f);
            
            //OLD CODE
            /*DOTween.KillAll();
            if (Runner != null)
            {
                Runner.Shutdown(true);
            }*/
            
            //------------------
            
        }
        void GoBackToHomeScreen()
        {

            DOTween.KillAll();
            Loader.Instance.ShowLoader();
            SceneManager.SwitchToHomeScene(_callback: (op) =>
            {
                ScreenManager.Instance.SwitchScreen(HomeScreen.Instance, null);
            });

            App app = FindObjectOfType<App>();
            app.ShowRoomOptionUI();
            Destroy(app.gameObject, 0.1f);
        }

        private void ResetStats()
        {
            if (!HasStateAuthority)
                return;
            for (int i = 0; i < score.Length; i++)
                score.Set(i, 0);
        }

        // Transition from lobby to level
        public void OnAllPlayersReady()
        {
            Debug.Log("All players are ready");
            RPC_GetReadyForFirstRound();
            IsGamePaused = false;
            // close and hide the session from matchmaking / lists. this demo does not allow late join.
            Runner.SessionInfo.IsOpen = false;
            Runner.SessionInfo.IsVisible = false;
            ResetStats();
            LoadLevel(Runner.GetLevelManager().GetRandomLevelIndex());

            //Invoke("SetPaused", 2);//
            //LoadGame();
        }

        [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
        private void RPC_GetReadyForFirstRound()
        {
            Debug.Log("RPC_GetReadyForFirstRound");
            Loader.Instance.ShowLoader(true);
            Invoke("Invoke_HideLobby", 0.5f);
            IsGamePaused = false;
        }

        private void Invoke_HideLobby() => lobbyCanvas.SetActive(false);

        private void SetPaused()
        {
            IsGamePaused = false;
            return;
            print($"SetPaused IsGamePaused - {IsGamePaused}");
        }

        private void LoadLevel(int nextLevelIndex)
        {
            if (Object.HasStateAuthority)
                Runner.GetLevelManager().LoadLevel(nextLevelIndex);
        }

        public int GetScore(Player player)
        {
            return score[player.PlayerIndex];
        }

    }
}