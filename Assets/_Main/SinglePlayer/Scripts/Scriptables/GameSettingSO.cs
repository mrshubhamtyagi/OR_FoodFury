using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace FoodFury
{
    [CreateAssetMenu(fileName = "GameSetting", menuName = "FoodFury/GameSetting")]
    public class GameSettingSO : ScriptableObject
    {
        public string GameVersion;
        public string gameVersioniOS;
        public string gameVersionAndroid;
        public int defaultFuel;
        public int restoreFuelInterval = 5;

        [Header("---Default Health---")] public int defaultDamage = 10;
        public int defaultEngineHealth = 100;
        public int defaultArmourMultiplier = 5;
        public int defaultSubMissileDamage = 15;

        [Header("---Map---")] public int currentMapIdWebGL = 1;
        public int currentMapIdAndroid = 1;
        public int currentMapIdiOS = 1;
        public int multiplayerMapId = 1;
        public int levelSpeedBoosterValue = 3;
        public bool showMascot;

        [Header("---Chest---")] public int chestSnackCost = 100;
        public int chestTreatCost = 400;
        public int chestFeastCost = 1000;
        public float chestUnlockDiscount = 0.9f;

        [Header("---Others---")] public bool resetPrefsOnUpdate = false;

        public List<string> botNames;

        public string GetRandomBotName(bool _addSufix = false)
        {
            int _index = UnityEngine.Random.Range(0, botNames.Count);
            return _addSufix ? $"{botNames[_index]}_Bot" : botNames[_index];
        }
    }
}