using UnityEngine;
namespace FoodFury
{
    public class OilPuddle : Weapon
    {
        [Header("Oil Parameters")]
        [SerializeField] private Animator puddleAnimator;
        [SerializeField] private SoundSO spawnSoundSO;
        [SerializeField] private float oilDuration;
        private Collider Collider;

        [Header("Oil Effects Parameter")]
        [SerializeField] private AnimationCurve overideAccelerationCurve;
        [SerializeField] private AnimationCurve overideLeftRightMovementCurve;
        public const string ANIMATION_COMPLETED_STRING = "Completed";
        public const string ANIMATION_SPAWNED_STRING = "Spawned";
        [SerializeField] private float maxHitTimeDuration;

        private bool timeCompleted;

        public override void Trigger(Enums.RiderType _riderType)
        {
            weaponFiredBy = _riderType;
            if (weaponFiredBy == Enums.RiderType.Player)
            {
                HapticsManager.LightHaptic();
            }
            OnWeaponTriggered?.Invoke(WeaponType.oilspill, weaponFiredBy);
        }
        private void OnEnable()
        {
            Collider = GetComponent<Collider>();
            timer = oilDuration;
            Collider.enabled = true;
            timeCompleted = false;
            AudioUtils.PlayOneShotAudio(spawnSoundSO, transform.position);
            puddleAnimator.SetTrigger(ANIMATION_SPAWNED_STRING);

        }
        private void OnTriggerEnter(Collider collision)
        {
            if (collision.transform.TryGetComponent<RiderBehaviour>(out RiderBehaviour riderBehaviour))
            {
                if (riderBehaviour.Vehicle.RiderType == Enums.RiderType.Player)
                {
                    OnHitPlayer?.Invoke(WeaponType);
                }
                else
                {
                    OnHitAI?.Invoke(WeaponType, weaponFiredBy);
                }
                //riderbehaviour got hit 
                //it Hit Something That is Rider
                // riderBehaviour.OnGotHitByAWeapon?.Invoke(this);
                OnHitVehicle(riderBehaviour);

                Collider.enabled = false;

            }
        }
        private void Update()
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else if (timer < 0 && timeCompleted == false)
            {
                timeCompleted = true;
                puddleAnimator.SetTrigger(ANIMATION_COMPLETED_STRING);

            }
            else if (timeCompleted)
            {
                AnimatorStateInfo stateInfo = puddleAnimator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.IsName(ANIMATION_COMPLETED_STRING))
                {
                    if (stateInfo.normalizedTime >= 1f)
                    {
                        Collider.enabled = false;
                        timeCompleted = false;
                        DestroyInstance();
                    }
                }


            }

        }
        public override void OnHitVehicle(RiderBehaviour rider)
        {
            rider.OnVehicleHitByOil(overideLeftRightMovementCurve, overideAccelerationCurve, maxHitTimeDuration);
        }
        public override void DestroyInstance()
        {
            if (gameObject == null || gameObject.Equals(null))
            {
                return;
            }
            WeaponManager.Instance.ReturnWeapon(this, WeaponType);
        }
    }
}

