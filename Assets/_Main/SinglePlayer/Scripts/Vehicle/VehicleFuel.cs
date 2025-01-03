using System;
using System.Collections;
using UnityEngine;

namespace FoodFury
{
    [DefaultExecutionOrder(-3)]
    [RequireComponent(typeof(Vehicle))]
    public class VehicleFuel : MonoBehaviour
    {
        public event Action OnFuelFinished;
        public event Action<float, float> OnFuelChanged;
        public float InitialFuel => GameData.Instance.GameSettings.defaultFuel;

        [field: SerializeField] public float CurrentFuel { get; private set; }
        [field: SerializeField] public float Mileage { get; private set; }

        private WaitForSeconds waitFor1Sec = new WaitForSeconds(1);
        private Vehicle vehicle;

        void Awake() => vehicle = GetComponent<Vehicle>();

        void Start()
        {
            if (vehicle.RiderType == Enums.RiderType.Player)
            {
                CurrentFuel = GameData.Instance.PlayerData.Data.fuel;

                int _mileageLevel = GameData.Instance.PlayerData.Data.GetCurrentVehicleLevels().mileageLevel;
                Mileage = _mileageLevel == 0 ? vehicle.VehicleData.initial.mileage : vehicle.VehicleData.mileageUpgrades[_mileageLevel - 1];
                waitFor1Sec = new WaitForSeconds(Mileage);
                OnFuelChanged?.Invoke(CurrentFuel, InitialFuel);
            }
            else
            {
                CurrentFuel = InitialFuel;
                Mileage = 1;
                waitFor1Sec = new WaitForSeconds(Mileage);
            }
        }


        private void OnEnable()
        {
            //  if (vehicle.RiderType == Enums.RiderType.Player) GameData.OnFuelReset += OnFuelReset;
            vehicle.Rider.OnBoosterCollected += OnBoosterCollected;
        }

        private void OnDisable()
        {
            // if (vehicle.RiderType == Enums.RiderType.Player) GameData.OnFuelReset -= OnFuelReset;
            vehicle.Rider.OnBoosterCollected -= OnBoosterCollected;
        }


        private void OnFuelReset(int _updatedFuel)
        {
            CurrentFuel = _updatedFuel;
            OnFuelChanged?.Invoke(CurrentFuel, InitialFuel);
        }



        float _remainingTime = 0;
        private void Update()
        {
            if (vehicle.RiderType != Enums.RiderType.Player || CurrentFuel <= 0 || vehicle.IsIdle || GameData.Instance.IsGamePaused) return;

            if (_remainingTime <= 0)
            {
                CurrentFuel -= 1;
                if (CurrentFuel <= 0)
                {
                    CurrentFuel = 0;
                    OnFuelChanged?.Invoke(CurrentFuel, InitialFuel);
                    OnFuelFinished?.Invoke();
                }

                if (CurrentFuel == 100) vehicle.VehicleSound.PlayLowFuel();
                OnFuelChanged?.Invoke(CurrentFuel, InitialFuel);
                _remainingTime = Mileage;
            }
            else _remainingTime -= Time.deltaTime;
        }


        private void OnBoosterCollected(ModelClass.BoosterData _booster)
        {
            if (_booster.type == Enums.BoosterType.Fuel)
            {
                CurrentFuel += _booster.value;
                OnFuelChanged?.Invoke(CurrentFuel, InitialFuel);
            }
        }

        private IEnumerator StartFuelCountdown()
        {
            while (CurrentFuel > 0)
            {
                yield return waitFor1Sec;

                if (vehicle.IsIdle || GameData.Instance.IsGamePaused) yield return null;
                else
                {
                    CurrentFuel -= 1;
                    if (CurrentFuel <= 0) CurrentFuel = 0;

                    if (CurrentFuel == 100) vehicle.VehicleSound.PlayLowFuel();
                    OnFuelChanged?.Invoke(CurrentFuel, InitialFuel);
                }
            }

            OnFuelFinished?.Invoke();
        }

        //private void OnDestroy() => StopCoroutine("StartFuelCountdown");
    }

}