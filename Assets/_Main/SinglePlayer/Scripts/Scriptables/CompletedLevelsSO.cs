using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static FoodFury.ModelClass;

namespace FoodFury
{
    [CreateAssetMenu(fileName = "CompletedLevels", menuName = "FoodFury/CompletedLevels")]
    public class CompletedLevelsSO : ScriptableObject
    {
        [field: SerializeField] public List<ModelClass.CompletedLevels> Data;
        public ModelClass.CompletedLevels GetCompletedLevels(int mapIndex) => Data.FirstOrDefault(l => l.mapId == mapIndex);
    }
}
