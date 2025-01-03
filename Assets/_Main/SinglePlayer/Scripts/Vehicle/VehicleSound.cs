using System;
using System.Collections.Generic;
using UnityEngine;

namespace FoodFury
{
    [DefaultExecutionOrder(-1)]
    [RequireComponent(typeof(Vehicle))]
    public class VehicleSound : MonoBehaviour
    {
        [Header("-----Vehicle Sound Collection ")]
        [SerializeField] private VehicleSoundCollectionSO vehicleSoundCollectionSO;
        [Header("-----Unity Events")]
        [SerializeField] private Action<float> onSpeedChanged;
        [SerializeField] private Action<float> onMaxSpeedChanged;
        [SerializeField] private Action<bool> onVehicleBroken;
        [SerializeField] private Action<bool> onBeingPlayer;
        [Header("Audio module List for each unity event")]
        [SerializeField] private List<AudioModuleWithHint<float>> AudioModuleListThatIsAddedToOnSpeedChanged;
        [SerializeField] private List<AudioModuleWithHint<float>> AudioModuleListThatIsAddedToOnMaxSpeedChanged;
        [SerializeField] private List<AudioModuleWithHint<bool>> AudioModuleListThatIsAddedToOnVehicleBroken;
        [SerializeField] private List<AudioModuleWithHint<bool>> AudioModuleListThatIsAddedToOnBeingPlayer;

        [SerializeField] private ChangeVehicleSoundSOBasedOnVariableModule enginevehicleSound;

        [Header("-----Audio Sources")]
        [SerializeField] private AudioSource engineStarting;
        [SerializeField] private AudioSource engineInBetween;
        [SerializeField] private AudioSource lowHealthAudioSource;
        [SerializeField] private AudioSource lowFuelAudioSource;
        [SerializeField] private AudioSource drift;

        [Header("Audio Curves")]
        [SerializeField] private AnimationCurve crashVolumeCurve;

        private Vehicle vehicle;
        private float previousSpeed;
        private bool gameStarted;

        private void OnEnable()
        {
            gameStarted = false;
            GameController.OnLevelStart += SetupSound;
            if (vehicle == null)
            {
                vehicle = GetComponent<Vehicle>();
            }
            AudioUtils.SetAudioSourceDataUsingSoundSO(lowFuelAudioSource, vehicleSoundCollectionSO.LowFuel);
            AudioUtils.SetAudioSourceDataUsingSoundSO(lowHealthAudioSource, vehicleSoundCollectionSO.LowHealth);
            AudioUtils.SetAudioSourceDataUsingSoundSO(drift, vehicleSoundCollectionSO.Drift);
            vehicle.Rider.OnVehicleDamage += PlayCrashSound;
            vehicle.VehicleHealth.OnHealthChanged += SetToBrokenState;
            vehicle.OnMaxSpeedChanged += UpdateEngineSound;
            GameController.OnLevelComplete += StopSound;
            GameController.OnLevelFailed += StopSound;
            if (vehicle.RiderType == Enums.RiderType.Player)
            {
                vehicle.Rider.OnBoosterCollected += OnBoosterCollected;
                Order.OnOrderFailed += OnOrderFailed;
                vehicle.Rider.OnOrderCollected += OnOrderCollected;

            }

            AudioUtils.AddModulesToAction<float>(AudioModuleListThatIsAddedToOnSpeedChanged, ref onSpeedChanged);
            AudioUtils.AddModulesToAction<bool>(AudioModuleListThatIsAddedToOnVehicleBroken, ref onVehicleBroken);
            AudioUtils.AddModulesToAction<bool>(AudioModuleListThatIsAddedToOnBeingPlayer, ref onBeingPlayer);
            AudioUtils.AddModulesToAction<float>(AudioModuleListThatIsAddedToOnMaxSpeedChanged, ref onMaxSpeedChanged);
            // Booster.OnBoosterCollected += OnBoosterCollected;
        }

        private void OnDisable()
        {
            if (vehicle.RiderType == Enums.RiderType.Player)
            {
                vehicle.Rider.OnBoosterCollected -= OnBoosterCollected;
                Order.OnOrderFailed -= OnOrderFailed;
                vehicle.Rider.OnOrderCollected -= OnOrderCollected;

            }
            vehicle.Rider.OnVehicleDamage -= PlayCrashSound;
            vehicle.VehicleHealth.OnHealthChanged -= SetToBrokenState;
            vehicle.OnMaxSpeedChanged -= UpdateEngineSound;
            GameController.OnLevelStart -= SetupSound;
            GameController.OnLevelComplete -= StopSound;
            GameController.OnLevelFailed -= StopSound;
            AudioUtils.RemoveModulesFromAction<float>(AudioModuleListThatIsAddedToOnSpeedChanged, ref onSpeedChanged);
            AudioUtils.RemoveModulesFromAction<bool>(AudioModuleListThatIsAddedToOnVehicleBroken, ref onVehicleBroken);
            AudioUtils.RemoveModulesFromAction<bool>(AudioModuleListThatIsAddedToOnBeingPlayer, ref onBeingPlayer);
            AudioUtils.RemoveModulesFromAction<float>(AudioModuleListThatIsAddedToOnMaxSpeedChanged, ref onMaxSpeedChanged);

        }
        void Awake() => vehicle = GetComponent<Vehicle>();

        public void ToggleSound(bool _flag)
        {
            if (_flag)
            {
                engineStarting.Play();
                engineInBetween.Play();
            }
            else
            {
                engineStarting.Stop();
                engineInBetween.Stop();
            }
        }

        public void PlayLowFuel()
        {
            if (vehicle.RiderType != Enums.RiderType.Player) return;
            lowFuelAudioSource.Play();// lowFuelInstance.start();
        }

        private void FixedUpdate()
        {
            if (!gameStarted) return;
            PlayEngineSound();

            if (vehicle.RiderType == Enums.RiderType.Player)
            {
                if (!drftSoundDelay)
                {
                    drftSoundDelay = true;
                    Invoke("SetDriftDelay", 2);
                    PlayDriftSound(vehicle.VehicleEffects.IsDrifting);
                }
            }
        }
        public void StopSound()
        {
            ToggleSound(false);
            PlayDriftSound(false);
            gameStarted = false;
            previousSpeed = 0;
            if (vehicle.RiderType == Enums.RiderType.Player)
            {
                lowHealthAudioSource.Stop();
                lowFuelAudioSource.Stop();
                onSpeedChanged?.Invoke(vehicle.CurrentSpeed01);
            }

        }
        public void SetupSound()
        {
            ToggleSound(true);
            previousSpeed = 0;
            gameStarted = true;
            onVehicleBroken?.Invoke(false);
            onBeingPlayer?.Invoke(vehicle.RiderType == Enums.RiderType.Player);
            lowHealthAudioSource.Stop();
            lowFuelAudioSource.Stop();
            onSpeedChanged?.Invoke(vehicle.CurrentSpeed01);
        }

        private void UpdateEngineSound(float percentage)
        {
            if (vehicle.RiderType != Enums.RiderType.Player) return;
            onMaxSpeedChanged?.Invoke(percentage);
        }


        //private void OnVehicleDamage(float _damage) => PlayCrashSound(_damage);


        private void OnOrderCollected(Order _order)
        {
            if (vehicle.RiderType != Enums.RiderType.Player) return;
            AudioUtils.PlayOneShotAudio(vehicleSoundCollectionSO.DeliveryComplete);
            //RuntimeManager.PlayOneShot(orderSuccess, transform.position);
        }

        private void OnOrderFailed(Order _order)
        {
            if (vehicle.RiderType != Enums.RiderType.Player) return;
            AudioUtils.PlayOneShotAudio(vehicleSoundCollectionSO.DeliveryFailed);
            //  RuntimeManager.PlayOneShot(orderFailed, transform.position);
        }

        private void OnBoosterCollected(ModelClass.BoosterData _booster)
        {
            if (vehicle.RiderType != Enums.RiderType.Player) return;
            AudioUtils.PlayOneShotAudio(vehicleSoundCollectionSO.DeliveryComplete);
            HapticsManager.LightHaptic();
        }


        public void OnVehicleModelChanged(VehicleModel model)
        {
            enginevehicleSound.SetNormalEngineSound(model.normalEngineSound);
            enginevehicleSound.SetBrokenEngineSound(model.brokenEngineSound);
        }


        private void PlayEngineSound()
        {
            if (MathF.Abs(vehicle.CurrentSpeed01 - previousSpeed) > 0.02)
            {
                onSpeedChanged.Invoke(vehicle.CurrentSpeed01);
                previousSpeed = vehicle.CurrentSpeed01;
            }
        }

        public void SetToBrokenState(int currentHealth, int initialHealth)
        {
            //Debug.Log("Vehicle Health:" + currentHealth);
            if (vehicle.RiderType != Enums.RiderType.Player) return;

            if (currentHealth < 30)
            {
                //Debug.Log("Vehicle Broken");
                onVehicleBroken?.Invoke(true);
                lowHealthAudioSource.Play();
            }
            else
            {
                onVehicleBroken?.Invoke(false);
                lowHealthAudioSource.Stop();
            }
        }

        public void PlayCrashSound(float damage)
        {
            if (vehicle.RiderType != Enums.RiderType.Player) return;
            AudioSource audioSource = AudioUtils.CreateAudioSource(vehicleSoundCollectionSO.Crash, transform.position);
            audioSource.volume += crashVolumeCurve.Evaluate(damage);
            audioSource.Play();
            HapticsManager.StrongHaptic();
            //Debug.Log("Played a crash sound ");
            Destroy(audioSource.gameObject, audioSource.clip.length);
        }



        private bool drftSoundDelay = false;
        public void PlayDriftSound(bool value)
        {

            if ((value == true && drift.isPlaying) || (value == false && drift.isPlaying == false))
            {
                return;
            }

            if (value)
            {
                drift.Play();
            }
            else
                drift.Stop();
        }
        private void SetDriftDelay() => drftSoundDelay = false;
    }




}
[Serializable]
public class AudioModuleWithHint<T>
{
    public string Hint;
    public AudioModule<T> module;
}