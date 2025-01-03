using System.Collections;
using System.Collections.Generic;
using Fusion;
using OneRare.FoodFury.Multiplayer;
using UnityEngine;

namespace OneRare.FoodFury.Multiplayer
{
    public class OilSpill : NetworkBehaviour
    {
        [Header("Oil Parameters")]
        [SerializeField] private Animator puddleAnimator;
        //[SerializeField] private SoundSO spawnSoundSO;
        [SerializeField] private float oilDuration;
        private Collider Collider;
        [Header("Oil Effects Parameter")]
        //[SerializeField] private AnimationCurve overideAccelerationCurve;
        //[SerializeField] private AnimationCurve overideLeftRightMovementCurve;
        public const string ANIMATION_COMPLETED_STRING = "Completed";
        public const string ANIMATION_SPAWNED_STRING = "Spawned";
        [SerializeField] private float maxHitTimeDuration;

        private bool timeCompleted;
        NetworkObject _networkObject;
        TickTimer maxLiveDurationTickTimer = TickTimer.None;
        public override void Spawned()
        {
            Collider = GetComponent<Collider>();
            Collider.enabled = true;
            timeCompleted = false;
            //AudioUtils.PlayOneShotAudio(spawnSoundSO, transform.position);
            puddleAnimator.SetTrigger(ANIMATION_SPAWNED_STRING);
            _networkObject = GetComponent<NetworkObject>();
            maxLiveDurationTickTimer = TickTimer.CreateFromSeconds(Runner, oilDuration);
        }

        public override void FixedUpdateNetwork()
        {
            
            if (Object.HasStateAuthority)
            {
                if (maxLiveDurationTickTimer.Expired(Runner))
                {
                    timeCompleted = true;
                    puddleAnimator.SetTrigger(ANIMATION_COMPLETED_STRING);
                    Runner.Despawn(_networkObject);
                }
            }
        }

        private void OnTriggerEnter(Collider collision)
        {
            Player target = collision.GetComponent<Player>();
            if (target != null)
            {
                if(target.IsBot)
                    return;
                Vector3 impulse = collision.transform.position - this.transform.position;
                target.RaiseEvent(new Player.DamageEvent { impulse = impulse, damage = 9 });
                //target.OnWeaponHit(OneRare.FoodFury.Multiplayer.WeaponManager.WeaponInstallationType.OILSPILL, target);
            }
            
            //
        }

        public void ShowDebug()
        {
            Debug.Log("Oil SpillPos:"+this.transform.position+" "+this.transform.rotation);
        }
        
    }
}

