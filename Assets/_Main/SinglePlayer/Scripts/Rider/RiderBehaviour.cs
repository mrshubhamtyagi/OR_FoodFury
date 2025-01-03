using DG.Tweening;
using System;
using System.Security.Cryptography;
using UnityEngine;

namespace FoodFury
{
    public abstract class RiderBehaviour : MonoBehaviour
    {
        public bool overrideInputs = false;
        [Range(-1f, 1f)] public float accelerateInput = 0;
        [Range(-1f, 1f)] public float steerInput = 0;

        public Vehicle Vehicle { get; private set; }
        public Action<float> OnVehicleDamage;

        public Action<Order> OnOrderCollected;


        // Booster
        public Action<ModelClass.BoosterData> OnBoosterCollected;
        public Action<Enums.BoosterType> OnBoosterEnd;


        // Weapon
        //public Action<Weapon> OnWeaponCollected;
        public Action<Weapon> OnGotHitByAWeapon;



        private void Awake()
        {
            Vehicle = GetComponent<Vehicle>();
            Vehicle.SetVehicleConfig();
        }
        public void OnVehicleHitByOil(AnimationCurve _leftRightCurve, AnimationCurve _accelerationCurve, float duration)
        {
            AnimationCurve leftRightCurve = new AnimationCurve(_leftRightCurve.keys);
            AnimationCurve accelerationCurve = new AnimationCurve(_accelerationCurve.keys);

            overrideInputs = true;
            if (Vehicle.vehicleType == Enums.VehicleType.FourWheeler)
            {
                overrideInputs = true;
                DOTween.To(() => 0f, x =>
                {
                    accelerateInput = 0;
                    steerInput = 0;
                    // Perform action when value updates

                }, 1f, 0.5f);
            }
            DOTween.To(() => 0f, x =>
            {

                if (Vehicle.vehicleType == Enums.VehicleType.TwoWheeler)
                {
                    steerInput = Mathf.Clamp(leftRightCurve.Evaluate(x) + UnityEngine.Random.Range(-0.1f, 0.1f), 0f, 1f);
                    accelerateInput = Mathf.Clamp(accelerationCurve.Evaluate(x) + UnityEngine.Random.Range(-0.1f, 0.1f), 0f, 1f);
                    // Perform action when value updates
                }
            }, 1f, duration)
            .SetEase(Ease.InBounce)
            .SetUpdate(true) // Ensure that the animation is updated even in FixedUpdate
            .OnComplete(() => overrideInputs = false);
        }

        public void OnVehicleHitByKetchup(bool isPlayer, AnimationCurve _leftRightCurve, AnimationCurve _accelerationCurve, float duration)
        {
            AnimationCurve leftRightCurve = new AnimationCurve(_leftRightCurve.keys);
            AnimationCurve accelerationCurve = new AnimationCurve(_accelerationCurve.keys);
            if (!isPlayer)
            {
                overrideInputs = true;
                DOTween.To(() => 0f, x =>
                {
                    accelerateInput = 0;
                    steerInput = 0;
                    // Perform action when value updates

                }, 1f, 0.5f);

                DOTween.To(() => 0f, x =>
                {
                    if (Vehicle.vehicleType == Enums.VehicleType.TwoWheeler)
                    {
                        steerInput = Mathf.Clamp(leftRightCurve.Evaluate(x) + UnityEngine.Random.Range(-0.05f, 0.05f), 0f, 1f);
                        accelerateInput = Mathf.Clamp(accelerationCurve.Evaluate(x) + UnityEngine.Random.Range(-0.05f, 0.05f), 0f, 1f);

                    }
                    // Perform action when value updates

                }, 1f, duration)
                .SetEase(Ease.InBounce)
                .SetUpdate(true) // Ensure that the animation is updated even in FixedUpdate
                .OnComplete(() =>
                {
                    overrideInputs = false;


                });
            }

        }
        public void OnVehicleHitBySubmissile(Vector3 force)
        {
            GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
            OnVehicleDamage?.Invoke(GameData.Instance.GameSettings.defaultSubMissileDamage);
        }
    }
}