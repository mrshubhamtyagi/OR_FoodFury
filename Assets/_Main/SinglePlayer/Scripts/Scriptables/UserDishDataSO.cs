using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FoodFury
{
    [CreateAssetMenu(fileName = "UserDishData", menuName = "FoodFury/UserDishData")]
    public class UserDishDataSO : ScriptableObject
    {
        public List<ModelClass.Dish> Data;
        public ModelClass.Dish GetDishByBaseId(int baseId) => Data.FirstOrDefault(d => d.baseId == baseId);
        public ModelClass.Dish GetDishByTokenId(int tokenId) => Data.FirstOrDefault(d => d.tokenId == tokenId);


    }
}
