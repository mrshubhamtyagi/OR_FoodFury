using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodFury
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "FoodFury/PlayerData")]
    public class PlayerDataSO : ScriptableObject
    {
        public ModelClass.PlayerData Data;
    }
}
