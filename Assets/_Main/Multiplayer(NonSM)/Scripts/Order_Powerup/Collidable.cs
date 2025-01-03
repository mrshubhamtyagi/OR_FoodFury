using OneRare.FoodFury.Multiplayer;
using UnityEngine;

namespace OneRare.FoodFury.Multiplayer
{
    public class Collidable : MonoBehaviour, ICollidable
    {
        public void Collide(Player player)
        {
            player.ReduceHealth();
        }
    }
}