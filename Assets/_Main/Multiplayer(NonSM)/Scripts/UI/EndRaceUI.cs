using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FoodFury;

namespace OneRare.FoodFury.Multiplayer
{
    public class EndRaceUI : MonoBehaviour
    {
        //public PlayerResultItem resultItemPrefab;
        //public GameObject resultsContainer;
        public Button goHomeButton;
        public Button replayButton;
        //[SerializeField] private TextMeshProUGUI goHomeButtonText;
        [SerializeField] private TextMeshProUGUI replayButtonText;
        [SerializeField] private TextMeshProUGUI playerRank;
        [SerializeField] private TextMeshProUGUI chipsText;
        [SerializeField] private TextMeshProUGUI munchesText;
        [SerializeField] private TextMeshProUGUI totalDeliveriesText;
        [SerializeField] private TextMeshProUGUI nftDeliveriesText;
        [SerializeField] private Image playerHealthFiller;
        [SerializeField] private Image timeFiller;
        [SerializeField] private GameObject crownImageGO;
        [SerializeField] private TextMeshProUGUI remotePlayerRankTextPrefab;
        [SerializeField] private TextMeshProUGUI localPlayerRankTextPrefab;
        [SerializeField] private GameObject rankHolderGameObject;

        int chips, munches, fuel;

        private float countdownTime;
        public void Init()
        {
            goHomeButton.onClick.AddListener(ReEnterRoom);
            replayButton.onClick.AddListener(OnClickReplayButton);

            
            countdownTime = Application.isEditor ? GameData.Instance.MultiplayerGameConfig.resultScreenTimer : 10;
            //DontDestroyOnLoad(gameObject);
        }

        private MobileInput mobileInput;

        public void ReEnterRoom()
        {
            gameObject.SetActive(false);
            mobileInput = FindObjectOfType<MobileInput>();
            mobileInput.OnDisconnect();
        }


        private static void ClearParent(Transform parent)
        {
            var len = parent.childCount;
            for (var i = 0; i < len; i++)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }
        public void UpdateResultScreen(Player player, bool isEliminated)
        {
            // Update the UI text elements with the provided data
            if (isEliminated)
            {
                playerRank.text = "ELIMINATED";
                crownImageGO.SetActive(false);
            }
            else
            {
                playerRank.text = GetPlayerRank(player, crownImageGO);
            }

            chips = player.Chips;
            munches = player.Munches;

            chipsText.text = player.Chips.ToString();
            munchesText.text = player.Munches.ToString();
            totalDeliveriesText.text = player.OrderCount.ToString();
            nftDeliveriesText.text = player.NFTOrderCount.ToString();
            playerHealthFiller.fillAmount = (player.UIHealth) / 100f;
            timeFiller.fillAmount = player.totalTimeMultiplier == 0 ? 0 : player.totalTimeMultiplier / (player.OrderCount * 1.5f * 60);
            UpdateLeaderboardView();
            HideButtons();
            OnAPIHit();
        }

        private void HideButtons()
        {
            goHomeButton.gameObject.SetActive(false);
            replayButton.gameObject.SetActive(false);
            replayButtonText.text = "REPLAY";
            //goHomeButton.interactable = false;
        }

        private async void OnAPIHit()
        {
            //float fuel = vehicleFuelMp.CurrentFuel;
            Debug.Log($"Sending player data to server: Chips:{chips} | Munches:{munches}");
            await PlayerAPIs.UpdatePlayerDataGameOverMultiplayer_API(chips, munches);

            StartCoroutine(StartCountdown());
        }

        private IEnumerator StartCountdown()
        {
            goHomeButton.gameObject.SetActive(true);
            replayButton.gameObject.SetActive(true);
            //goHomeButton.interactable = true;
            while (countdownTime > 0)
            {
                // Update the button text with the current countdown value
                replayButtonText.text = $"PLAY AGAIN ({countdownTime})";

                // Wait for 1 second
                yield return new WaitForSeconds(1f);

                // Decrease the countdown
                countdownTime--;
            }

            if (countdownTime <= 0)
            {
                replayButtonText.text = "PLAY AGAIN";
                OnClickReplayButton();
            }

        }

        void OnClickReplayButton()
        {
            HideButtons();
            StartCoroutine("RestartMultiplayerGame");
        }

        IEnumerator RestartMultiplayerGame()
        {
            mobileInput = FindObjectOfType<MobileInput>();
            mobileInput.DisconnectAndRestartMPGame();

            yield return new WaitForEndOfFrame();
            gameObject.SetActive(false);
        }

        public static string GetPlayerRank(Player player, GameObject crownImage)
        {
            var orderedPlayers = Player.LeaderboardPlayers;

            int rank = orderedPlayers.IndexOf(player) + 1;

            if (rank == 1)
            {
                crownImage.SetActive(true);
            }
            else
            {
                crownImage.SetActive(false);
            }

            /*switch (rank)
            {
                case 0:
                    return "2nd Place";
                case 1:
                    return "Battle Won"; // First position
                case 2:
                    return "2nd Place";
                case 3:
                    return "3rd Place";
                default:
                    return $"{rank}th Place"; // For ranks 4 and beyond
            }*/
            return $"BATTLE RESULTS";
        }

          private void UpdateLeaderboardView()
        {
            var parent = rankHolderGameObject.transform;
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
                TextMeshProUGUI rankItem;
                string rankWithSuffix = GetRankSuffix(i + 1); // Use helper function to get rank with suffix

       
                if (kart == currentPlayer)
                {
                    // Other eliminated players
                    rankItem = Instantiate(localPlayerRankTextPrefab, parent); 
                    rankItem.text = $"{i+1}<size=80%>{rankWithSuffix}   {kart.Username}";
                }
                else
                {
                    // Active remote players
                    rankItem = Instantiate(remotePlayerRankTextPrefab, parent); 
                    rankItem.text =  $"{i+1}<size=80%>{rankWithSuffix}   {kart.Username}";
                }
            }
            Debug.Log(_log);
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
        private Player GetCurrentPlayer()
        {
            return Player.Local; // Adjust this based on how you manage the current player in your game
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
        private string GetRankSuffix(int rank)
        {
            if (rank % 100 >= 11 && rank % 100 <= 13) return "th";
            return (rank % 10) switch
            {
                1 => "st",
                2 => "nd",
                3 => "rd",
                _ => "th",
            };
        }

        /*private void EnsureContinueButton(List<Player> karts)
		{
			var allFinished = karts.Count == VehicleEntity.Vehicles.Count;
			if (RoomPlayer.Local.IsLeader) {
				continueEndButton.gameObject.SetActive(allFinished);
			}
		}*/
    }
}