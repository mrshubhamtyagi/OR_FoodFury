using System;
using UnityEngine;

namespace FoodFury
{
    public class GameTimer : MonoBehaviour
    {
        protected float remainingTime;
        protected bool isRunning;
        protected bool isPaused;
        public Action onTimerComplete;
        public Action<TimeSpan> onTimerUpdate;
        [SerializeField] private int subtractMinute;




        public void StartTimerInSeconds(int durationInSeconds)
        {
            if (!isRunning && !isPaused)
            {
                remainingTime = durationInSeconds;
                isRunning = true;
                isPaused = false;
            }
        }
        public void StartTimerInMinutes(int durationInMinutes)
        {
            if (!isRunning && !isPaused)
            {
                remainingTime = durationInMinutes * 60;
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
        [ContextMenu("Reduce Time")]
        public void ReduceTimeByTenMinutes()
        {
            remainingTime -= subtractMinute * 60;
        }

        private void Update()
        {
            if (isRunning)
            {
                remainingTime -= Time.deltaTime;
                onTimerUpdate?.Invoke(TimeSpan.FromSeconds(remainingTime));
                if (remainingTime <= 0)
                {
                    remainingTime = 0;
                    isRunning = false;
                    onTimerComplete?.Invoke();

                }
            }
        }
    }
}
