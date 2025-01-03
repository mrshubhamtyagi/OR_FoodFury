using UnityEngine;
using Fusion;
using OneRare.FoodFury.Multiplayer;
using UnityEngine.Serialization;
using System.Collections.Generic;
using DG.Tweening;
using FoodFury;

namespace OneRare.FoodFury.Multiplayer
{
    public class RocketHandler : NetworkBehaviour
    {

        //Timing
        TickTimer maxLiveDurationTickTimer = TickTimer.None;

        //Rocket info
        int rocketSpeed = 30;

        //Other components
        NetworkObject networkObject;
        private Player _player;
        public LayerMask hitMask;
        public WeaponManager.WeaponInstallationType weaponInstallationType;
        [SerializeField] private List<GameObject> childs;
        [SerializeField] private ParticleSystem hitParticles;
        [SerializeField] private SoundSO spawnSoundSO;
        [SerializeField] private SoundSO travellingSoundSO;
        [SerializeField] private SoundSO hitSoundSO;
        [SerializeField] private AudioSource travellingAudioSound;
        
        private Tween animationTween;
        public override void Spawned()
        {
            //Debug.Log("RH: " + transform.position + " <> " + transform.rotation);
            foreach (var _child in childs)
            {
                _child.SetActive(true);
            }
            networkObject = GetComponent<NetworkObject>();
            
            if (spawnSoundSO != null)
            {
                AudioUtils.PlayOneShotAudio(spawnSoundSO, transform.position);
                if (travellingSoundSO != null)
                {
                    animationTween =
                        
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

        bool impact;
        Vector3 hitPoint;

        public void Fire(Player player)
        {
            maxLiveDurationTickTimer = TickTimer.CreateFromSeconds(Runner, GameData.Instance.MultiplayerGameConfig.rocketLiveDuration);
            //transform.position = player.transform.position;
            transform.rotation = player.transform.rotation;
        }

        /*void OnDrawGizmos()
        {
            // Draws a 5 unit long red line in front of the object
            Gizmos.color = Color.red;
            Vector3 direction = transform.TransformDirection(Vector3.forward) * 2;
            Gizmos.DrawRay(transform.position + transform.up * 0.2f, direction);
        }*/

        private Collider[] _areaHits = new Collider[4];

        public override void FixedUpdateNetwork()
        {
            transform.position += transform.forward * Runner.DeltaTime * rocketSpeed;

            if (Object.HasStateAuthority)
            {
                if (maxLiveDurationTickTimer.Expired(Runner))
                {
                    Runner.Despawn(networkObject);

                    return;
                }
            }

            hitPoint = transform.position + 1 * transform.forward;
            if (Runner.GameMode == GameMode.Shared)
            {
                impact = Runner.GetPhysicsScene().Raycast(transform.position + transform.up * 0.2f, transform.forward,
                    out var hitinfo, 1.2f, hitMask.value);
                hitPoint = hitinfo.point;
            }

            if (impact)
            {
                int cnt = Physics.OverlapSphereNonAlloc(hitPoint, 1.5f, _areaHits, hitMask.value, QueryTriggerInteraction.Ignore);
                if (cnt > 0)
                {
                    for (int i = 0; i < cnt; i++)
                    {
                        GameObject other = _areaHits[i].gameObject;
                        if (other)
                        {
                            Player target = other.GetComponent<Player>();
                            if (target != null && target != _player)
                            {
                                if(target.IsBot)
                                    return;
                                Vector3 impulse = other.transform.position - hitPoint;
                                target.RaiseEvent(new Player.DamageEvent { impulse = impulse, damage = 10 });
                                //target.OnWeaponHit(weaponInstallationType);
                            }
                            //Instantiate(hitParticles, other.transform.position, Quaternion.identity);
                            if (HasStateAuthority)
                                Runner.Despawn(Object);
                        }
                        else
                        {
                            if (!maxLiveDurationTickTimer.Expired(Runner) && HasStateAuthority)
                            {
                                Runner.Despawn(Object);
                            }
                        }
                    }
                }
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            PlayHitSFXAndVFX();
            foreach (var _child in childs)
            {
                _child.SetActive(false);
            }
            //Instantiate(explosionParticleSystemPrefab, checkForImpactPoint.position, Quaternion.identity);
        }

        void PlayHitSFXAndVFX()
        {
            Instantiate(hitParticles, transform.position, Quaternion.identity);
            if (hitSoundSO != null)
            {
                if (travellingSoundSO != null)
                {
                    travellingAudioSound.Stop();
                }
                AudioUtils.PlayOneShotAudio(hitSoundSO, transform.position);
            }
        }
        private void OnDestroy()
        {
            // Kill the tween to prevent errors
            if (animationTween != null && animationTween.IsActive())
            {
                animationTween.Kill();
            }
        }

        private void OnDisable()
        {
            // Kill the tween to prevent errors
            if (animationTween != null)
            {
                animationTween.Kill();
            }
        }

    }
}