using UnityEngine;

namespace FoodFury
{
    [CreateAssetMenu(fileName = "VehicleConfig", menuName = "FoodFury/Vehicle Config", order = 1)]
    public class VehicleConfig : ScriptableObject
    {
        public Enums.VehicleType vehicleType = Enums.VehicleType.TwoWheeler;
        [field: SerializeField] public Enums.RiderType RiderType { get; private set; }
        [field: SerializeField] public float Acceleration { get; private set; }
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public float Turn { get; private set; }
        [field: SerializeField] public float Break { get; private set; }
        [field: SerializeField] public float Drift { get; private set; }
        [field: SerializeField] public float Drag { get; private set; }
        [field: SerializeField] public float Damage { get; private set; }

        [field: SerializeField] public int MaxBodyTileAngle { get; private set; }
        [field: SerializeField] public int MaxSteeringAngle { get; private set; }
        [field: SerializeField] public float CustomGravity { get; private set; }
        public float DeadZoneValue => 0.001f;

        public int Health => 100;
        public float Mileage => 1;
    }
}
