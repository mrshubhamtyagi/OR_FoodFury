using System;
using System.Collections;
using FoodFury;
using UnityEngine;

namespace OneRare.FoodFury.Multiplayer
{
    public class VehicleFuelMP : MonoBehaviour
    {
        public event Action OnFuelFinished;
        public event Action<float, float> OnFuelChanged;
        public float InitialFuel => GameData.Instance.GameSettings.defaultFuel;

        [field: SerializeField] public float CurrentFuel { get; private set; }
        [field: SerializeField] public float Mileage { get; private set; }

        private WaitForSeconds waitFor1Sec = new WaitForSeconds(1);

        [field: SerializeField] public ModelClass.GarageVehicleData VehicleData { get; private set; }

        //private Vehicle vehicle;
        private Player player;
        void Awake() => player = GetComponent<Player>();

        public void Init()
        {
            CurrentFuel = GameData.Instance.PlayerData.Data.fuel;
            VehicleData =
                GameData.Instance.GarageData.GetVehicleGarageData(GameData.Instance.PlayerData.Data.currentVehicle);
            int _mileageLevel = GameData.Instance.PlayerData.Data.GetCurrentVehicleLevels().mileageLevel;
            Mileage = _mileageLevel == 0
                ? VehicleData.initial.mileage
                : VehicleData.mileageUpgrades[_mileageLevel - 1];
            //Mileage = 1;
            waitFor1Sec = new WaitForSeconds(Mileage);
            OnFuelChanged?.Invoke(CurrentFuel, InitialFuel);
        }


        void Start()
        {
            if (!player.IsBot) return;
            CurrentFuel = InitialFuel;
            Mileage = 1;
            waitFor1Sec = new WaitForSeconds(Mileage);
        }


        private void OnEnable()
        {
            //  if (vehicle.RiderType == Enums.RiderType.Player) GameData.OnFuelReset += OnFuelReset;
            //player.OnBoosterTimeChanged += OnBoosterCollected;
        }

        private void OnDisable()
        {
            // if (vehicle.RiderType == Enums.RiderType.Player) GameData.OnFuelReset -= OnFuelReset;
            //player.OnBoosterTimeChanged -= OnBoosterCollected;
        }


        private void OnFuelReset(int _updatedFuel)
        {
            CurrentFuel = _updatedFuel;
            OnFuelChanged?.Invoke(CurrentFuel, InitialFuel);
        }


        float _remainingTime = 0;

        private void Update()
        {
            if (player.IsBot || CurrentFuel <= 0 || player.CurrentStage != Player.Stage.Active ||
                GameData.Instance.IsGamePaused) return;

            if (Player.Local && Player.Local.IsIdle)
                return;


            if (_remainingTime <= 0)
            {
                CurrentFuel -= 1;
                if (CurrentFuel <= 0)
                {
                    CurrentFuel = 0;
                    OnFuelChanged?.Invoke(CurrentFuel, InitialFuel);
                    OnFuelFinished?.Invoke();
                }

                //if (CurrentFuel == 100) vehicle.VehicleSound.PlayLowFuel();
                OnFuelChanged?.Invoke(CurrentFuel, InitialFuel);
                _remainingTime = Mileage;
            }
            else _remainingTime -= Time.deltaTime;
        }


        /*private void OnBoosterCollected(ModelClass.BoosterData _booster)
        {
            if (_booster.type == Enums.BoosterType.Fuel)
            {
                CurrentFuel += _booster.value;
                OnFuelChanged?.Invoke(CurrentFuel, InitialFuel);
            }
        }*/

        private IEnumerator StartFuelCountdown()
        {
            while (CurrentFuel > 0)
            {
                yield return waitFor1Sec;

                if (player.CurrentStage != Player.Stage.Active || GameData.Instance.IsGamePaused) yield return null;
                else
                {
                    CurrentFuel -= 1;
                    if (CurrentFuel <= 0) CurrentFuel = 0;

                    //if (CurrentFuel == 100) vehicle.VehicleSound.PlayLowFuel();
                    OnFuelChanged?.Invoke(CurrentFuel, InitialFuel);
                }
            }

            OnFuelFinished?.Invoke();
        }

        //private void OnDestroy() => StopCoroutine("StartFuelCountdown");
    }
}