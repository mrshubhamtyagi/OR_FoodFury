using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FoodFury
{
    [CreateAssetMenu(fileName = "GarageData", menuName = "FoodFury/GarageData")]
    public class GarageDataSO : ScriptableObject
    {
        public List<ModelClass.GarageVehicleData> Data;
        public ModelClass.GarageVehicleData GetCurrentVehicleGarageData() => Data.FirstOrDefault(x => x.id == GameData.Instance.PlayerData.Data.currentVehicle);
        public ModelClass.GarageVehicleData GetVehicleGarageData(int index) => Data.FirstOrDefault(x => x.id == index);
        public IOrderedEnumerable<ModelClass.GarageVehicleData> GetRegularBikes() => Data.Where(v => !v.IsSpecialVehicle()).OrderBy(x => x.id);
        public IOrderedEnumerable<ModelClass.GarageVehicleData> GetSpecialBikes() => Data.Where(v => v.IsSpecialVehicle()).OrderBy(x => x.id);
    }
}
