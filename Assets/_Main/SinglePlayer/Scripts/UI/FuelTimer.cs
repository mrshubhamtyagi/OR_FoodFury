using System;
using UnityEngine;

namespace FoodFury
{
    public class FuelTimer : MonoBehaviour
    {
        private float remainingTime;
        private bool isRunning;
        private bool isPaused;
        public static Action onTimerComplete;
        private bool runningRemaningTime;
        private void OnEnable()
        {
            runningRemaningTime = false;
            GameData.OnLevelSelected += PauseTimer;
            GameData.OnPlayerDataUpdate += CheckIfTimerNeedToBeStoppedOrNot;

        }
        private void OnDisable()
        {

            PauseTimer();
            GameData.OnLevelSelected -= PauseTimer;
            GameData.OnPlayerDataUpdate -= CheckIfTimerNeedToBeStoppedOrNot;
        }
        public void Init()
        {
            CheckIfTimerNeedToBeStoppedOrNot();
        }
        public void CheckIfTimerNeedToBeStoppedOrNot()
        {
            if (ScreenManager.Instance.CurrentScreen == Enums.Screen.Home)
            {
                if (IsTankFull())
                {
                    StopTimer();
                    return;
                }
                else
                {
                    if (GameData.Instance.FuelRestoreRemainingTime > 0 && runningRemaningTime == false)
                    {
                        runningRemaningTime = true;
                        StopTimer();
                        StartTimer(GameData.Instance.FuelRestoreRemainingTime);
                    }
                    else if (remainingTime <= 0)
                    {
                        StartTimer(GameData.Instance.GameSettings.restoreFuelInterval);
                    }

                    else
                        ResumeTimer();
                }


            }
            else
            {
                PauseTimer();
            }
        }
        public bool IsTankFull()
        {
            return GameData.Instance.PlayerData.Data.fuel >= (GameData.Instance.GetCurrentFuelTankCapacity() * 60);
        }
        public void StartTimer(float durationInSeconds)
        {
            if (!isRunning && !isPaused)
            {
                remainingTime = durationInSeconds;
                isRunning = true;
                isPaused = false;
            }
        }
        public void ResumeTimer()
        {
            if (isPaused)
            {
                isRunning = true;
                isPaused = false;
            }
        }
        public void PauseTimer()
        {
            if (isRunning && !isPaused)
            {
                isRunning = false;
                isPaused = true;
            }
        }
        public void StopTimer()
        {
            isRunning = false;
            isPaused = false;
            remainingTime = 0f;

        }

        public float GetRemainingTime()
        {
            return remainingTime;
        }
        void Update()
        {
            if (isRunning)
            {
                remainingTime -= Time.deltaTime;
                if (remainingTime <= 0)
                {
                    if (GameData.Instance.FuelRestoreRemainingTime > 0 && runningRemaningTime)
                    {
                        GameData.Instance.SetRemaingFuelTimer(0);
                        runningRemaningTime = false;
                    }
                    remainingTime = 0;
                    isRunning = false;
                    GameData.Instance.RestoreFuel();
                    CheckIfTimerNeedToBeStoppedOrNot();
                }
            }
        }
        [ContextMenu("Restore Fuel")]
        public void RestoreFuelImmediately()
        {
            GameData.Instance.RestoreFuel();
        }


    }
}
