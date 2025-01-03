using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FoodFury
{
    [CreateAssetMenu(fileName = "MapData", menuName = "FoodFury/MapData")]
    public class MapDataSO : ScriptableObject
    {
        public List<ModelClass.MapDetail> Data;
        public ModelClass.MapDetail GetMapDetails(int index) => Data.FirstOrDefault(m => m.mapId == index);
        public bool HasMap(int index) => Data.Any(m => m.mapId == index) ; //Data.FirstOrDefault(m => m.mapId == index);
    }
}
