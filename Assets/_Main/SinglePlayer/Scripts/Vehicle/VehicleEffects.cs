using System;
using UnityEngine;

namespace FoodFury
{
    [DefaultExecutionOrder(-2)]
    [RequireComponent(typeof(Vehicle))]
    public class VehicleEffects : MonoBehaviour
    {
        [Header("---Drift")]
        [Range(0.01f, 0.2f)] public float skidThreshold = 0.1f;
        [SerializeField] private TrailRenderer primaryWheel, otherWheel;
        public bool IsDrifting { get; private set; }
        [Header("---Smoke")]
        [SerializeField] private GameObject[] smokeParticles;

        private bool useSmoke;

        private Vehicle vehicle;

        void Awake() => vehicle = GetComponent<Vehicle>();

        void Start()
        {
            useSmoke = false;
            foreach (var item in smokeParticles)
                item.SetActive(false);
            //smokeParticles.main.maxParticles = Mathf.Clamp(100 * vehicle.CurrentSpeed01, 50, 100);
        }

        private void OnEnable() => vehicle.VehicleHealth.OnHealthChanged += OnHealthChanged;
        private void OnDisable() => vehicle.VehicleHealth.OnHealthChanged -= OnHealthChanged;


        //public void SetWheels(Transform _primary, Transform _other)
        //{
        //    primaryWheel = _primary.GetComponent<TrailRenderer>();
        //    if (_other) otherWheel = _other.GetComponent<TrailRenderer>();
        //}



        private void OnHealthChanged(int _health, int _maxHealth) => useSmoke = _health < 30;

        void Update()
        {
            if (GameData.Instance.IsGamePaused || primaryWheel == null) return;

            if (useSmoke && primaryWheel.emitting)
            {
                primaryWheel.emitting = false;
                if (otherWheel != null) otherWheel.emitting = false;

                foreach (var item in smokeParticles)
                    item.SetActive(true);
            }
            else if (!useSmoke && smokeParticles[0].activeSelf)
                foreach (var item in smokeParticles)
                    item.SetActive(false);

            IsDrifting = vehicle.IsGrounded && vehicle.CurrentSpeed01 > 0.1f && HelperFunctions.GetAbs(vehicle.GetSideVelocity()) > skidThreshold;

            SetEmitting();
        }

        private void SetEmitting()
        {
            primaryWheel.emitting = IsDrifting;
            if (otherWheel != null) otherWheel.emitting = IsDrifting;
        }
    }

}