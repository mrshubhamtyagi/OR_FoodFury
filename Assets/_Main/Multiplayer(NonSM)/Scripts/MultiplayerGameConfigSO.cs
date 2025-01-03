using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneRare.FoodFury.Multiplayer
{
   
    [CreateAssetMenu(fileName = "MultiplayerGameConfig", menuName = "FoodFury/MultiplayerGameConfig")]
    public class MultiplayerGameConfigSO : ScriptableObject
    {
        [Header("Challenge Manager Settings")]
        public float initiationDuration = 10f;
        public float eliminationDuration = 5f;

        [Header("Challenge Manager Settings")]
        public float powerupSpawnInterval = 10f;
        
        [Header("Fusion Session Settings")]
        public float botInitialSpawnTime = 12f;
        public float botSpawnInterval = 10f;
        
        [Header("Result Screen Settings")]
        public float resultScreenTimer = 30f;
        
        [Header("Rocket Settings")]
        public float rocketLiveDuration = 3f;
    }
}
