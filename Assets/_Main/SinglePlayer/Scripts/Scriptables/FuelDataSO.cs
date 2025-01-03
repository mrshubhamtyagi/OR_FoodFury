using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodFury
{
    [CreateAssetMenu(fileName = "FuelData", menuName = "FoodFury/FuelData")]
    public class FuelDataSO : ScriptableObject
    {
        public InitialValues initialValues;
        public List<FuelUpgrade> tankUpgrades;
        public List<FuelUpgrade> restoreUpgrades;
        public int TotalTankUpgrades() => tankUpgrades.Count;
        public int TotalRestoreUpgrades() => restoreUpgrades.Count;

        [Serializable]
        public class InitialValues
        {
            public int tank;
            public int restore;
        }

        [Serializable]
        public class FuelUpgrade
        {
            public int value;
            public int cost;
            public Enums.CostType costType;
        }
    }
}
