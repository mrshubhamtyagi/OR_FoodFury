using System;
using UnityEngine;

namespace FoodFury
{
    [DefaultExecutionOrder(-4)]
    [RequireComponent(typeof(Vehicle))]
    public class VehicleHealth : MonoBehaviour
    {
        public event Action OnHealthFinished;
        public event Action<int, int> OnHealthChanged; //CurrentHealth, InitialHealth
        public event Action<int, int> OnArmourChanged; //CurrentArmour, InitialArmour

        [field: SerializeField] public int CurrentHealth { get; private set; }
        [field: SerializeField] public int CurrentArmour { get; private set; }
        [field: SerializeField] public int InitialHealth { get; private set; }
        [field: SerializeField] public int InitialArmour { get; private set; }
        [field: SerializeField] public int BoosterShield { get; private set; }
        [field: SerializeField] public float Damage { get; private set; }

        private Vehicle vehicle;
        void Awake() => vehicle = GetComponent<Vehicle>();

        void Start()
        {
            if (vehicle.RiderType == Enums.RiderType.Player)
            {
                int _shieldLevel = GameData.Instance.PlayerData.Data.GetCurrentVehicleLevels().shieldLevel;
                InitialHealth = CurrentHealth = _shieldLevel == 0 ? GameData.Instance.GameSettings.defaultEngineHealth : GameData.Instance.GameSettings.defaultEngineHealth + vehicle.VehicleData.shieldUpgrades[_shieldLevel - 1];

                int _armourLevel = GameData.Instance.PlayerData.Data.GetCurrentVehicleLevels().armourLevel;
                InitialArmour = CurrentArmour = _armourLevel == 0 ? 0 : GameData.Instance.GameSettings.defaultArmourMultiplier * vehicle.VehicleData.armourUpgrades[_armourLevel - 1];
                //_armourLevel == 0 ? 0 : GameData.Instance.GameSettings.defaultArmourHealth + GameData.Instance.GarageData.Find(v => v.id == GameData.Instance.PlayerData.currentVehicle).armourUpgrades[_armourLevel - 1];

                Damage = GameData.Instance.GameSettings.defaultDamage;
                //int _damageLevel = GameData.Instance.PlayerData.GetCurrentVehicle().damageLevel;
                //Damage = _damageLevel == 0 ? vehicle.VehicleData.initial.armour : vehicle.VehicleData.upgrades[_damageLevel - 1].damage;

                OnHealthChanged?.Invoke(CurrentHealth, InitialHealth + BoosterShield);
                OnArmourChanged?.Invoke(CurrentArmour, InitialArmour);
            }
            else
            {
                InitialHealth = CurrentHealth = vehicle.VehicleConfig.Health;
                Damage = vehicle.VehicleConfig.Damage;
            }
        }

        private void OnEnable()
        {
            vehicle.Rider.OnBoosterCollected += OnBoosterCollected;
            GameplayScreen.OnGameReady += OnGameReady;
        }

        private void OnDisable()
        {
            vehicle.Rider.OnBoosterCollected -= OnBoosterCollected;
            GameplayScreen.OnGameReady -= OnGameReady;
        }

        private void OnGameReady()
        {
            if (vehicle.RiderType != Enums.RiderType.Player) return;

            BoosterShield = GameController.Instance.LevelInfo.LevelBooster == Enums.LevelBoosterType.Shield ? 25 : 0;
            CurrentHealth = InitialHealth + BoosterShield;
            CurrentArmour = InitialArmour;
            OnHealthChanged?.Invoke(CurrentHealth, InitialHealth + BoosterShield);
            OnArmourChanged?.Invoke(CurrentArmour, InitialArmour);
        }

        private void OnBoosterCollected(ModelClass.BoosterData _booster)
        {
            if (_booster.type == Enums.BoosterType.Health)
            {
                CurrentHealth += Mathf.FloorToInt(_booster.value);
                OnHealthChanged?.Invoke(CurrentHealth, InitialHealth + BoosterShield);
            }
        }

        public void OnLevelComplete(bool _success)
        {
            if (_success)
            {
                CurrentHealth = InitialHealth;
                CurrentArmour = InitialArmour;
                OnHealthChanged?.Invoke(CurrentHealth, InitialHealth + BoosterShield);
                OnArmourChanged?.Invoke(CurrentArmour, InitialArmour);
            }
        }

        public void TakeDamage(float _value)
        {
            CurrentHealth -= Mathf.Clamp(Mathf.FloorToInt(_value * Damage), 0, CurrentHealth);
            OnHealthChanged?.Invoke(CurrentHealth, InitialHealth + BoosterShield);

            if (CurrentHealth <= 0) OnHealthFinished?.Invoke();
        }

        [ContextMenu("TakeWeaponDamage_Debug")]
        private void TakeWeaponDamage_Debug() => TakeWeaponDamage(15);
        public void TakeWeaponDamage(int _value)
        {
            if (vehicle.RiderType != Enums.RiderType.Player) return;

            int _armourLevel = GameData.Instance.PlayerData.Data.GetCurrentVehicleLevels().armourLevel;
            if (CurrentArmour <= 0 || _armourLevel == 0)
            {
                CurrentHealth -= Mathf.Clamp(_value, 0, CurrentHealth);
            }
            else
            {
                int _armourDamage = GameData.Instance.GarageData.GetVehicleGarageData(GameData.Instance.PlayerData.Data.currentVehicle).armourUpgrades[_armourLevel - 1];
                int _armourLeftDamage = CurrentArmour > _armourDamage ? 0 : _armourDamage - CurrentArmour;

                CurrentArmour = Mathf.Clamp(CurrentArmour - _armourDamage, 0, CurrentArmour);
                OnArmourChanged?.Invoke(CurrentArmour, InitialArmour);

                int _healthDamage = _value - _armourDamage + _armourLeftDamage;
                CurrentHealth -= Mathf.Clamp(_healthDamage, 0, CurrentHealth);
            }

            OnHealthChanged?.Invoke(CurrentHealth, InitialHealth + BoosterShield);
            if (CurrentHealth <= 0) OnHealthFinished?.Invoke();
        }
    }

}