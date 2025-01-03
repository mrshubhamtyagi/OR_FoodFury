using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

namespace FoodFury
{
    public class CounterObject : NetworkBehaviour
    {
        [Networked] public TickTimer MatchTimer { get; set; }
        public TextMeshProUGUI counterText;
        [Networked] public string Time { get; set; } = "00:00";
        private ChangeDetector changeDetector;
        public override void Spawned()
        {
            changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
            
            if(Object.HasStateAuthority)
                StartMatch();
        }
        
        bool matchStarted = false;
        public void StartMatch()
        {
            Debug.LogError("MATCH START:"+Time);
            MatchTimer = TickTimer.CreateFromSeconds(Runner, 100);
            matchStarted = true;
        }
        
        // Update is called once per frame
        public override void FixedUpdateNetwork()
        {
            if(!matchStarted)
                return;
            //var remainingTime = MatchTimer.RemainingTime(Runner).Value;
            var timeSpan = TimeSpan.FromSeconds(MatchTimer.RemainingTime(Runner).Value);
            var outPut = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
            Time = outPut;
            
        }

        public override void Render()
        {
            foreach (var change in changeDetector.DetectChanges(this))
            {
                switch (change)
                {
                    case nameof(Time):
                        SetTime(Time);
                        break;
                }
            }
        }
        
        private void SetTime(string time)
        {
            
            Debug.Log("TIME REMAINING:"+Time);
            counterText.text = time;
            
        }
    }
}
