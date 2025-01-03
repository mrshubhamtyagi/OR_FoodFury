using DG.Tweening;
using UnityEngine;
namespace FoodFury
{
    public class SubMissile : Weapon
    {

        private Rigidbody rb;
        private Vector3 offset;
        private float speed;
        [SerializeField] private bool rotate = false;
        [SerializeField] private float rotateAmount = 45;

        private Collider Collider;

        [SerializeField] private float forceMagnitude;
        [SerializeField] private SoundSO spawnSoundSO;
        [SerializeField] private SoundSO travellingSoundSO;
        [SerializeField] private SoundSO hitSoundSO;
        [SerializeField] private AudioSource travellingAudioSound;
        [SerializeField] private ParticleSystem SubmissileSpawn;
        [SerializeField] private ParticleSystem SubmissileMuzzle;
        [SerializeField] private ParticleSystem SubmissileExplosion;
        [SerializeField] private GameObject visual;
        private bool startTimer;
        private bool weaponAlreadyReturnedToPool;
        Vector3 forceToBeAddedToRider;

        public override void Trigger(Enums.RiderType _riderType)
        {
            weaponFiredBy = _riderType;
            if (weaponFiredBy == Enums.RiderType.Player)
            {
                HapticsManager.LightHaptic();
            }
            OnWeaponTriggered?.Invoke(WeaponType.subMissile, weaponFiredBy);

            // ammo.transform.position = startingPostition.position;
            // ammo.GetComponent<ProjectileBehaviour>().SetWeapon(this);
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
            SubmissileMuzzle.Stop(true);
            SubmissileSpawn.Stop(true);
            startTimer = false;
            rb = GetComponent<Rigidbody>();
            speed = Mathf.Max(GameController.Instance.Rider.Vehicle.MaxSpeed + 5, 25);
            Collider = GetComponent<Collider>();
            Collider.enabled = true;
            visual.SetActive(true);
            weaponAlreadyReturnedToPool = false;
            SubmissileMuzzle.Play();
            SubmissileSpawn.Play();

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
                    SubmissileExplosion.Stop(true);
                    DestroyInstance();
                }
            }

        }
        private void OnTriggerEnter(Collider collider)
        {
            if (collider.transform.TryGetComponent(out RiderBehaviour riderBehaviour))
            {
                if (riderBehaviour.Vehicle.RiderType == Enums.RiderType.Player)
                {
                    OnHitPlayer?.Invoke(WeaponType);
                }
                else
                {

                    OnHitAI?.Invoke(WeaponType, weaponFiredBy);
                }


                Vector3 forceDirection = transform.forward;
                forceToBeAddedToRider = forceDirection * forceMagnitude;
                OnHitVehicle(riderBehaviour);
                StartDestroySequence();
            }
            else
            {
                StartDestroySequence();
            }

        }

        private void StartDestroySequence()
        {
            Collider.enabled = false;
            SubmissileMuzzle.Stop(true);
            SubmissileSpawn.Stop(true);
            visual.SetActive(false);
            timer = SubmissileExplosion.main.duration;
            SubmissileExplosion.Play();
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
            if (rider.Vehicle.RiderType == Enums.RiderType.Player)
                rider.Vehicle.VehicleHealth.TakeWeaponDamage(GameData.Instance.GameSettings.defaultSubMissileDamage);
            rider.OnVehicleHitBySubmissile(forceToBeAddedToRider);

            // DestroyInstance();
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

