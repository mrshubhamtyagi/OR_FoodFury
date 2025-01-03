using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using FoodFury;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace OneRare.FoodFury.Multiplayer
{
    public enum LeaderboardState
    {
        Small,
        Full
    }
    public class UIManager : NetworkBehaviour
    {
        public static UIManager Instance { get; set; }
        public EndRaceUI endRaceUI { get; private set; }
        public IntermediateLevelScoreUI interimRaceUI { get; private set; }


        #region SERIALIZABLE FIELDS
        [Header("UI Popups")]
        [SerializeField] private EndRaceUI endRaceUIPrefab;
        [SerializeField] private IntermediateLevelScoreUI interimRaceUIPrefab;
        [SerializeField] private SplashUIItem splashUIItemPrefab;
        [Header("Order")]
        [SerializeField] private OrderCollectedItem orderCollectedPrefab;
        [SerializeField] private OrderStatus orderStatus;
        [SerializeField] private GameObject orderUITopPanel;
        public OrderUIItem[] orderCardContainer;
        [SerializeField] private Texture2D defaultThumbnail;
        [Header("Lobby")]
        [SerializeField] private RoomPlayerUI roomPlayerUIPrefab;
        [SerializeField] private RoomPlayerUI awaitingPlayerPrefab;
        [SerializeField] private GameObject roomPlayerPanel;
        [SerializeField] private LeaderboardUIItem leaderboardUIItemRemotePrefab;
        [SerializeField] private LeaderboardUIItem leaderboardUIItemLocalPrefab;
        [SerializeField] private LeaderboardUIItem leaderboardUIItemEliminatedPlayerPrefab;
        public GameObject leaderboardPanel;
        [Header("In Game UI")]
        [SerializeField] private GameObject timerPanel;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI roundText;
        [SerializeField] private Image timeFillerImage;
        [SerializeField] private Image redAuraImage;
        [SerializeField] private Image redAuraFullScreenImage;
        [SerializeField] private float blinkDuration = 0.75f;
        // Time per frame in seconds
        // Start is called before the first frame update
        #endregion

        private Image[] dishLevel;
        private OrderCollectedItem orderCollectedItem;
        private SplashUIItem splashUIItem;
        private List<GameObject> orderCards = new List<GameObject>();
        private int currentFrame;
        private float timer;
        private Color timerFillerDefaultColor;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(this);
            orderUITopPanel.SetActive(false);
            redAuraImage.enabled = false;
            redAuraFullScreenImage.enabled = false;
            Player.PlayerJoined += AddPlayer;
            Player.PlayerLeft += RemovePlayer;
            timerFillerDefaultColor = timeFillerImage.color;
        }

        void Spawned()
        {
            ShowFullLeaderboard();
        }
        private void AddPlayer(Player player)
        {
            Instance.ShowRoomPlayerUI();
        }
        private void RemovePlayer(Player player)
        {
            Instance.ShowRoomPlayerUI();
        }
        public void ShowOrderCollected(string playerName, Color orderColor)
        {
            orderStatus.ShowOrderStatus(global::Enums.OrderStatus.Delivered, _initialDelay: 0, _autoHideDelay: 1);
            if (Object.HasStateAuthority)
            {
                RPC_ShowOrderCollectedPopUp(playerName, orderColor);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        void RPC_ShowOrderCollectedPopUp(string playerName, Color orderColor)
        {
            orderCollectedItem = Instantiate(orderCollectedPrefab);
            orderCollectedItem.SetResult(playerName, orderColor);
        }

        public void ShowSplashPopUpForEliminated()
        {
            splashUIItem = Instantiate(splashUIItemPrefab);
            splashUIItem.ShowEliminated();
        }

        public void ShowSplashPopUpForVictory()
        {
            splashUIItem = Instantiate(splashUIItemPrefab);
            splashUIItem.ShowVictory();
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_ShowInterimResult(int roundNumber)
        {
            //interimRaceUI = Instantiate(interimRaceUIPrefab);
            //interimRaceUI.Initialize();
            // interimRaceUI.ShowRoundOver(roundNumber);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_ShowInterimUIForRounds(string message, int round)
        {
            timerPanel.SetActive(true);
            roundText.text = "ROUND " + round;
            interimRaceUI = Instantiate(interimRaceUIPrefab);
            interimRaceUI.Initialize();
            interimRaceUI.ShowRoundMessage(message);
        }


        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_UpdateRoundsOnTimer(int round)
        {
            timerPanel.SetActive(true);
            roundText.text = "ROUND " + round;
        }


        //private int containerIndex = 0;
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_OnNewOrderGenerated(OrderMP orderNew)
        {
            orderUITopPanel.SetActive(true);

            /*// Clear existing cards properly
            ClearParent(orderCardContainer);
            orderCards.Clear();*/

            //Invoke("UpdateOrderInUI",0.2f);

            // Display status for the new order
            if (orderNew.Thumbnail == null)
            {
                orderStatus.ShowOrderStatus(global::Enums.OrderStatus.NewOrder, orderNew.OrderNFTDetails,
                   defaultThumbnail, _initialDelay: 2, _autoHideDelay: 3);
            }
            else
            {
                orderStatus.ShowOrderStatus(global::Enums.OrderStatus.NewOrder, orderNew.OrderNFTDetails,
                    (Texture2D)orderNew.Thumbnail, _initialDelay: 2, _autoHideDelay: 3);
            }


        }


        private List<RoomPlayerUI> awaitingPlayerPlaceholders = new List<RoomPlayerUI>();
        public void ShowRoomPlayerUI()
        {
            ClearParent(roomPlayerPanel.transform);
            awaitingPlayerPlaceholders.Clear();
            int maxPlayers = 4;
            for (int i = 0; i < maxPlayers; i++)
            {
                RoomPlayerUI placeholder = Instantiate(awaitingPlayerPrefab, roomPlayerPanel.transform);
                awaitingPlayerPlaceholders.Add(placeholder);
            }

            // Replace placeholders with actual player names if players have joined
            UpdateRoomPlayerUI();
        }

        public void UpdateRoomPlayerUI()
        {
            var players = Player.LeaderboardPlayers;

            for (int i = 0; i < players.Count; i++)
            {
                var player = players[i];

                // Replace the correct placeholder with the player's information
                if (awaitingPlayerPlaceholders[i] != null)
                {
                    // Destroy the placeholder
                    Destroy(awaitingPlayerPlaceholders[i].gameObject);
                    awaitingPlayerPlaceholders.RemoveAt(i);

                    // Instantiate the player UI at the same position in the hierarchy
                    var playerUI = Instantiate(roomPlayerUIPrefab, roomPlayerPanel.transform);
                    playerUI.transform.SetSiblingIndex(i); // Set it at the correct index
                    playerUI.SetPlayerProfile(player.Username.ToString(), i); //GameData.ProfilePic);

                    // Optionally: insert playerUI into awaitingPlayerPlaceholders at index `i` if needed for future updates
                    awaitingPlayerPlaceholders.Insert(i, playerUI);
                }
            }
        }

        public void RemoveRoomPlayerUI()
        {
            ClearParent(roomPlayerPanel.transform);
            roomPlayerPanel.SetActive(false);
        }
        public static List<Player> GetFinishedPlayers()
        {
            // Active players by Score
            var activePlayersByScore = Player.LeaderboardPlayers
                .Where(x => x.CurrentStage == Player.Stage.Active)
                .OrderByDescending(x => x.Score)
                .ToList();

            var deadPlayers = Player.LeaderboardPlayers
                .Where(x => x.CurrentStage == Player.Stage.Dead)
                .ToList();

            Player.LeaderboardPlayers = activePlayersByScore.Concat(deadPlayers).ToList();


            //.OrderByDescending(x => x.CurrentStage == Player.Stage.Active) // Active players first
            //.ThenByDescending(x => x.Score)                             // Sort by score 
            //.ThenBy(x => Player.LeaderboardPlayers.IndexOf(x))          // Original order for eliminated players
            //.ToList();

            return Player.LeaderboardPlayers;
        }
        private List<Player> GetEliminatedPlayers()
        {
            return Player.LeaderboardPlayers.Where(player => player.CurrentStage == Player.Stage.Dead).ToList();
        }
        public void ClearParent(Transform parent)
        {
            foreach (Transform child in parent.transform)
            {
                Destroy(child.gameObject);
            }
        }

        private int leaderboardStateInt = 0;

        public void OnShowFullLeaderboardButtonClicked()
        {
            ShowFullLeaderboard();
        }

        public void ShowFullLeaderboard()
        {
            leaderboardStateInt = (int)LeaderboardState.Full;
            UpdateLeaderboardView();
            //StartCoroutine(AutoCloseLeaderboard());
        }
        private IEnumerator AutoCloseLeaderboard()
        {
            yield return new WaitForSeconds(3f);
            leaderboardStateInt = (int)LeaderboardState.Small;
            UpdateLeaderboardView();
        }
        private void UpdateLeaderboardView()
        {
            var parent = leaderboardPanel.transform;
            ClearParent(parent);

            var karts = GetFinishedPlayers();
            var currentPlayer = GetCurrentPlayer();

            // Separate active and eliminated players
            var activePlayers = karts.Where(kart => kart.CurrentStage != Player.Stage.Dead).ToList();
            var eliminatedPlayers = karts.Where(kart => kart.CurrentStage == Player.Stage.Dead).ToList();

            // Combine active players and eliminated players (active players first)
            var sortedPlayers = activePlayers.Concat(eliminatedPlayers).ToList();
            string _log = "----RPC_RedrawLeaderboardList - ";
            for (var i = 0; i < sortedPlayers.Count; i++)
            {
                var kart = sortedPlayers[i];
                _log += $"\nSortedPlayers[{i}] - {kart.Username} - {kart.Score} -  {kart.CurrentStage}";
                //Debug.Log($"-----SortedPlayers[{i}] - {kart.Username} - {kart.CurrentStage}");
                LeaderboardUIItem leaderboardItem;
                string rankWithSuffix = GetOrdinalSuffix(i + 1); // Use helper function to get rank with suffix

                if (kart == currentPlayer && kart.CurrentStage == Player.Stage.Active)
                {
                    // Current active player
                    leaderboardItem = Instantiate(leaderboardUIItemLocalPrefab, parent);
                    leaderboardItem.transform.localScale = new Vector3(1f, 1f, 1f);
                    leaderboardItem.GetComponent<LeaderboardUIItem>().SetLeaderBoardData("You", rankWithSuffix, sortedPlayers.Count, kart.Score, false);
                }
                else if (kart == currentPlayer && kart.CurrentStage == Player.Stage.Dead)
                {
                    // Current eliminated player
                    leaderboardItem = Instantiate(leaderboardUIItemEliminatedPlayerPrefab, parent);
                    leaderboardItem.transform.localScale = new Vector3(1f, 1f, 1f);
                    leaderboardItem.GetComponent<LeaderboardUIItem>().SetLeaderBoardData("You", rankWithSuffix, sortedPlayers.Count, kart.Score, false);
                }
                else if (kart != currentPlayer && kart.CurrentStage == Player.Stage.Dead)
                {
                    // Other eliminated players
                    leaderboardItem = Instantiate(leaderboardUIItemEliminatedPlayerPrefab, parent);
                    leaderboardItem.transform.localScale = new Vector3(1f, 1f, 1f);
                    leaderboardItem.GetComponent<LeaderboardUIItem>().SetLeaderBoardData(kart.Username.ToString(), rankWithSuffix, sortedPlayers.Count, kart.Score, false);
                }
                else
                {
                    // Active remote players
                    leaderboardItem = Instantiate(leaderboardUIItemRemotePrefab, parent);
                    leaderboardItem.transform.localScale = new Vector3(1f, 1f, 1f);
                    leaderboardItem.GetComponent<LeaderboardUIItem>().SetLeaderBoardData(kart.Username.ToString(), rankWithSuffix, sortedPlayers.Count, kart.Score, false);
                }
            }
            Debug.Log(_log);
        }

        private string GetOrdinalSuffix(int rank)
        {
            if (rank % 100 >= 11 && rank % 100 <= 13)
            {
                return rank + "th";
            }

            switch (rank % 10)
            {
                case 1: return rank + "st";
                case 2: return rank + "nd";
                case 3: return rank + "rd";
                default: return rank + "th";
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_RedrawLeaderboardList()
        {
            UpdateLeaderboardView();
        }

        public void RedrawLeaderboardList()
        {
            UpdateLeaderboardView();
        }

        private Player GetCurrentPlayer()
        {
            return Player.Local; // Adjust this based on how you manage the current player in your game
        }


        private Tweener blinkTween;
        private Tweener redAuraTween;
        private Tweener redAuraFullScreenTween;

        public void UpdateTime(string time, float fillAmount)
        {
            if (!timeText)
                return;

            timeText.text = time;

            if (timeFillerImage != null)
            {
                timeFillerImage.fillAmount = fillAmount;

                if (fillAmount < 0.20f)
                {
                    if (blinkTween == null || !blinkTween.IsActive() || !blinkTween.IsPlaying())
                    {
                        StartBlinking();
                    }

                    if (redAuraImage != null && (redAuraTween == null || !redAuraTween.IsActive() || !redAuraTween.IsPlaying()))
                    {
                        redAuraImage.enabled = true;
                        redAuraFullScreenImage.enabled = true;
                        StartRedAuraTween();
                    }
                }
                else
                {
                    StopBlinking();
                    StopRedAuraTween();
                }
            }
        }

        private void StartBlinking()
        {
            if (blinkTween == null || !blinkTween.IsActive())
            {
                blinkTween = timeFillerImage.DOColor(ColorManager.Instance.Red, blinkDuration)
                    .SetLoops(-1, LoopType.Yoyo)
                    .OnKill(() => timeFillerImage.color = timerFillerDefaultColor);
            }
        }

        private void StopBlinking()
        {
            if (blinkTween != null && blinkTween.IsActive())
            {
                blinkTween.Kill();
                blinkTween = null;
                timeFillerImage.color = timerFillerDefaultColor;
            }
        }

        private void StartRedAuraTween()
        {
            redAuraTween = redAuraImage.DOFade(1, blinkDuration).SetLoops(-1, LoopType.Yoyo);
            redAuraFullScreenTween = redAuraFullScreenImage.DOFade(1, blinkDuration).SetLoops(-1, LoopType.Yoyo);
        }

        private void StopRedAuraTween()
        {
            if (redAuraTween != null && redAuraTween.IsActive())
            {
                redAuraTween.Kill();
                redAuraFullScreenTween.Kill();
                redAuraTween = null;
                redAuraFullScreenTween = null;
                redAuraImage.enabled = false;
                redAuraFullScreenImage.enabled = false;
            }
        }
        
        private void OnDestroy()
        {
            redAuraTween.Kill();
            redAuraFullScreenTween.Kill();
            blinkTween.Kill();
        }

        private void OnDisable()
        {
            redAuraTween.Kill();
            redAuraFullScreenTween.Kill();
            blinkTween.Kill();
        }


    }
}