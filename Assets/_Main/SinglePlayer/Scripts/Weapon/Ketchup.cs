using DG.Tweening;
using ETFXPEL;
using System;
using UnityEngine;
namespace FoodFury
{
    public class Ketchup : Weapon
    {
        private Rigidbody rb;
        private Vector3 offset;
        private float speed;
        [SerializeField] private bool rotate = false;
        [SerializeField] private float rotateAmount = 45;
        private Collider Collider;

        [SerializeField] private AnimationCurve overideAccelerationCurve;
        [SerializeField] private AnimationCurve overideLeftRightMovementCurve;
        [SerializeField] private float maxHitTimeDuration;
        public static Action OnKetchupTouchedPlayer;

        [SerializeField] private SoundSO spawnSoundSO;
        [SerializeField] private SoundSO travellingSoundSO;
        [SerializeField] private SoundSO hitSoundSO;
        [SerializeField] private AudioSource travellingAudioSound;
        [SerializeField] private ParticleSystem KetchupSpawn;
        [SerializeField] private ParticleSystem KetchupMuzzle;
        [SerializeField] private ParticleSystem KetchupExplosion;
        private bool weaponAlreadyReturnedToPool;


        private bool startTimer;

        public override void Trigger(Enums.RiderType _riderType)
        {
            weaponFiredBy = _riderType;
            if (weaponFiredBy == Enums.RiderType.Player)
            {
                HapticsManager.LightHaptic();
            }
            OnWeaponTriggered?.Invoke(WeaponType.ketchup, weaponFiredBy);

        }
        void FixedUpdate()
        {
            if (!startTimer)
            {
                if (rotate)
                    transform.Rotate(0, 0, rotateAmount, Space.Self);
                if (speed != 0 && rb != null)
                    rb.position += (transform.forward + offset) * (speed * Time.deltaTime);
            }

        }
        private void OnEnable()
        {
            KetchupMuzzle.Stop(true);
            KetchupSpawn.Stop(true);
            startTimer = false;
            rb = GetComponent<Rigidbody>();
            speed = Mathf.Max(GameController.Instance.Rider.Vehicle.MaxSpeed + 5, 25);
            Collider = GetComponent<Collider>();
            Collider.enabled = true;
            weaponAlreadyReturnedToPool = false;

            KetchupMuzzle.Play();
            KetchupSpawn.Play();

            if (spawnSoundSO != null)
            {
                AudioUtils.PlayOneShotAudio(spawnSoundSO, transform.position);
                if (travellingSoundSO != null)
                {
                    DOTween.Sequence()
                                .AppendInterval(0f) // Delay for the specified duration
                                .AppendCallback(() =>
                                {
                                    travellingAudioSound = AudioUtils.SetAudioSourceDataUsingSoundSO(travellingAudioSound, travellingSoundSO);
                                    travellingAudioSound.Play();
                                });

                }
            }
        }
        private void Update()
        {

            if (startTimer)
            {
                if (timer > 0)
                {
                    timer -= Time.deltaTime;
                }
                else
                {
                    startTimer = false;
                    KetchupExplosion.Stop(true);
                    DestroyInstance();
                }
            }

        }
        private void OnTriggerEnter(Collider collider)
        {
            if (collider.transform.TryGetComponent(out RiderBehaviour riderBehaviour))
            {
                OnHitVehicle(riderBehaviour);
                StartDestroySequence();
            }
            else
            {
                Debug.Log("Touched City");
                StartDestroySequence();
            }
        }

        private void StartDestroySequence()
        {
            Collider.enabled = false;
            KetchupMuzzle.Stop(true);
            KetchupSpawn.Stop(true);
            timer = KetchupExplosion.main.duration;
            KetchupExplosion.Play();
            startTimer = true;
            DisableTravellingSoundAndPlayHitSound();
        }

        private void DisableTravellingSoundAndPlayHitSound()
        {
            if (hitSoundSO != null)
            {
                if (travellingSoundSO != null)
                {
                    travellingAudioSound.Stop();
                }
                AudioUtils.PlayOneShotAudio(hitSoundSO, transform.position);
            }
        }

        public override void OnHitVehicle(RiderBehaviour rider)
        {
            if (rider.Vehicle.RiderType != Enums.RiderType.Player)
            {
                OnHitAI?.Invoke(WeaponType.ketchup, weaponFiredBy);
                if (weaponFiredBy == Enums.RiderType.Player)
                {
                    HapticsManager.MediumHaptic();
                }
                rider.OnVehicleHitByKetchup(false, overideLeftRightMovementCurve, overideAccelerationCurve, maxHitTimeDuration);
            }
            else
            {
                HapticsManager.MediumHaptic();
                OnHitPlayer?.Invoke(WeaponType.ketchup);
                OnKetchupTouchedPlayer?.Invoke();
            }
            //ketchupAnimator.SetTrigger(ANIMATION_HIT_STRING);
            //isHitAnimationPlaying = true;

        }

        public override void DestroyInstance()
        {
            if (weaponAlreadyReturnedToPool || gameObject == null || gameObject.Equals(null))
                return;

            weaponAlreadyReturnedToPool = true;
            WeaponManager.Instance.ReturnWeapon(this, WeaponType);
        }
    }
}

