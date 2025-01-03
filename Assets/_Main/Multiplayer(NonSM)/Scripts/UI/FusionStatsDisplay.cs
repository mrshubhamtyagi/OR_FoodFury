using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Statistics;
using OneRare.FoodFury.Multiplayer;
using TMPro;
using UnityEngine;

namespace FoodFury
{
    public class FusionStatsDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI rttText;
        [SerializeField] private TextMeshProUGUI warningMessageText;// Text element for RTT
        private NetworkRunner runner;
        private float waitTimer = 0.5f;
        private const int PingThreshold = 200; // 200 ms threshold
        private const float DisconnectDuration = 5f; // Duration to disconnect
        private float highPingTimer = 0f;
        private GameManager _gameManager;
        private bool _isChallengeStarted= false;

        private void Start()
        {
            rttText.text = " ";
            warningMessageText.text = "";
            Invoke(nameof(DelayAccess), 5f);
        }
        
        private void InstanceOnOnChallengeStarted(GameManager.PlayState playState)
        {
            if (playState == GameManager.PlayState.LOBBY && !_isChallengeStarted)
            {
                Invoke(nameof(SetChallengeStarted), 5f);
            }
        }

        private void DelayAccess()
        {
            
            runner = FindObjectOfType<NetworkRunner>();
            if (runner == null || GameData.Instance.serverMode == global::Enums.ServerMode.MainNet)
            {
                enabled = false;
                return;
            }
            _gameManager = FindObjectOfType<GameManager>();
            _gameManager.OnPlayStateChanged += InstanceOnOnChallengeStarted;
        }

        void SetChallengeStarted()
        {
            _isChallengeStarted = true;
        }

        private void LateUpdate()
        {
            if (runner == null || runner.LocalPlayer == PlayerRef.None || rttText == null)
                return;

            if(!_isChallengeStarted)
                return;
            
            if (waitTimer <= 0)
            {
                int ping = (int)(runner.GetPlayerRtt(runner.LocalPlayer) * 1000);

                // Display RTT
                rttText.text = $"Ping: {ping} ms";

                // Check if ping is above threshold
                if (ping > PingThreshold)
                {
                    Debug.LogWarning("Check your internet speed!");
                    warningMessageText.text = "Check your internet speed!";
                    // Start the high ping timer
                    highPingTimer += waitTimer;

                    // If high ping persists for the disconnect duration, disconnect the player
                    if (highPingTimer >= DisconnectDuration)
                    {
                        Debug.LogError("Ping too high for too long. Disconnecting the player...");
                        warningMessageText.text = "Ping too high for too long. Disconnecting the player...";
                        runner.Shutdown(); // Disconnects the player from the session
                    }
                }
                else
                {
                    // Reset the high ping timer if ping is below threshold
                    highPingTimer = 0f;
                }

                waitTimer = 0.5f; // Reset wait timer
            }
            else
            {
                waitTimer -= Time.deltaTime;
            }
        }
    }
}
