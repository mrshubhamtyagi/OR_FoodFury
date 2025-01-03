using System;

namespace FoodFury
{
    public interface IDriver
    {
        public Vehicle Vehicle { get; }
        public void TakeDamage(float _damage);
    }

}
