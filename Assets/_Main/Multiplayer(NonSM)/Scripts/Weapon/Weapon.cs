using Fusion;
using FusionExamples.Utility;
using FusionHelpers;
using OneRare.FoodFury.Multiplayer;
using UnityEngine;

namespace OneRare.FoodFury.Multiplayer
{
    public class Weapon : NetworkBehaviourWithState<Weapon.NetworkState>
    {
        [Networked] public override ref NetworkState State => ref MakeRef<NetworkState>();

        public struct NetworkState : INetworkStruct
        {
            [Networked, Capacity(12)] public NetworkArray<ShotState> bulletStates => default;
        }

        [SerializeField] private Transform[] _gunExits;
        [SerializeField] private float _rateOfFire;
        [SerializeField] private byte _ammo;

        [SerializeField] private bool _infiniteAmmo;

        //[SerializeField] private PowerupType _powerupType = PowerupType.DEFAULT;
        [SerializeField] private Shot _bulletPrefab;

        private SparseCollection<ShotState, Shot> bullets;
        private float _visible;
        private bool _active;
        private Collider[] _areaHits = new Collider[4];
        private Player _player;

        public float delay => _rateOfFire;
        public bool isShowing => _visible >= 1.0f;
        public byte ammo => _ammo;
        public bool infiniteAmmo => _infiniteAmmo;

        private void Awake()
        {
            _player = GetComponentInParent<Player>();
        }

        public override void Spawned()
        {
            bullets = new SparseCollection<ShotState, Shot>(State.bulletStates, _bulletPrefab);
        }

        public override void FixedUpdateNetwork()
        {
            bullets.Process(this, (ref ShotState bullet, int tick) =>
            {
                if (bullet.Position.y < -.15f)
                {
                    bullet.EndTick = Runner.Tick;
                    return true;
                }

                if (!_bulletPrefab.IsHitScan && bullet.EndTick > Runner.Tick)
                {
                    Vector3 dir = bullet.Direction.normalized;
                    float length = Mathf.Max(_bulletPrefab.Radius, _bulletPrefab.Speed * Runner.DeltaTime);
                    if (Physics.Raycast(bullet.Position - length * dir, dir, out var hitinfo, length,
                            _bulletPrefab.HitMask.value, QueryTriggerInteraction.Ignore))
//					if (Runner.LagCompensation.Raycast(bullet.Position - length*dir, dir, length, Object.InputAuthority, out var hitinfo, _bulletPrefab.HitMask.value, HitOptions.IgnoreInputAuthority | HitOptions.IncludePhysX))
                    {
                        bullet.Position = hitinfo.point;
                        bullet.EndTick = Runner.Tick;
                        ApplyAreaDamage(hitinfo.point);
                        return true;
                    }
                }

                return false;
            });
        }

        public override void Render()
        {
            if (TryGetStateChanges(out var from, out var to))
                OnFireTickChanged();
            else
                TryGetStateSnapshots(out from, out _, out _, out _, out _);

            bullets.Render(this, from.bulletStates);
        }

        public void Show(bool show)
        {
            if (_active && !show)
            {
                ToggleActive(false);
            }
            else if (!_active && show)
            {
                ToggleActive(true);
            }

            _visible = Mathf.Clamp(_visible + (show ? Time.deltaTime : -Time.deltaTime) * 5f, 0, 1);

            if (show)
                transform.localScale = Tween.easeOutElastic(0, 1, _visible) * Vector3.one;
            else
                transform.localScale = Tween.easeInExpo(0, 1, _visible) * Vector3.one;
        }

        private void ToggleActive(bool value)
        {
            _active = value;

        }

        public void Fire(NetworkRunner runner, PlayerRef owner, Vector3 ownerVelocity)
        {
            Transform exit = GetExitPoint(Runner.Tick);

            //Debug.DrawLine(exit.position, exit.position+exit.forward, Color.blue, 1.0f);
            //Debug.Log($"Bullet fired in tick {runner.Tick} from position {exit.position} weapon is at {transform.position}");
            SpawnNetworkShot(runner, owner, exit, ownerVelocity);
        }

        private void OnFireTickChanged()
        {
            // Recharge the laser sight if this weapon has it
        }

        private void SpawnNetworkShot(NetworkRunner runner, PlayerRef owner, Transform exit, Vector3 ownerVelocity)
        {
            if (_bulletPrefab.IsHitScan)
            {
                bool impact;
                Vector3 hitPoint = exit.position + _bulletPrefab.Range * exit.forward;
                if (runner.GameMode == GameMode.Shared)
                {
                    impact = runner.GetPhysicsScene().Raycast(exit.position, exit.forward, out var hitinfo,
                        _bulletPrefab.Range, _bulletPrefab.HitMask.value);
                    hitPoint = hitinfo.point;

                    // Debug.LogError("HIT IMPACT: "+impact+"  "+hitPoint);
                }
                else
                {
                    impact = Runner.LagCompensation.Raycast(exit.position, exit.forward, _bulletPrefab.Range,
                        Object.InputAuthority, out var hitinfo, _bulletPrefab.HitMask.value,
                        HitOptions.IgnoreInputAuthority | HitOptions.IncludePhysX);
                    hitPoint = hitinfo.Point;
                    // Debug.LogError("HIT IMPACT: "+impact+"  "+hitPoint);
                }

                if (impact)
                {
                    ApplyAreaDamage(hitPoint);
                }

                bullets.Add(runner, new ShotState(exit.position, hitPoint - exit.position), 0);
            }
            else
                bullets.Add(runner, new ShotState(exit.position, exit.forward), _bulletPrefab.TimeToLive);
        }

        private void ApplyAreaDamage(Vector3 hitPoint)
        {
            int cnt = Physics.OverlapSphereNonAlloc(hitPoint, _bulletPrefab.AreaRadius, _areaHits,
                _bulletPrefab.HitMask.value, QueryTriggerInteraction.Ignore);
            if (cnt > 0)
            {
                for (int i = 0; i < cnt; i++)
                {
                    GameObject other = _areaHits[i].gameObject;
                    // Debug.LogError("ApplyAreaDamage: "+other);
                    if (other)
                    {
                        Player target = other.GetComponentInParent<Player>();
                        if (target != null && target != _player)
                        {
                            if(target.IsBot)
                                return;
                            
                            Vector3 impulse = other.transform.position - hitPoint;
                            float l = Mathf.Clamp(_bulletPrefab.AreaRadius - impulse.magnitude, 0,
                                _bulletPrefab.AreaRadius);
                            impulse = _bulletPrefab.AreaImpulse * l * impulse.normalized;
                            //  Debug.LogError("ApplyAreaDamage: TARGET:  "+target+"  "+impulse);
                            target.RaiseEvent(new Player.DamageEvent { impulse = impulse, damage = 20 });
                        }
                    }
                }
            }
        }

        public Transform GetExitPoint(int tick)
        {
            Transform exit = _gunExits[0];
            exit.position += (exit.forward * 3f + exit.up * 1);
            return exit;
        }
    }
}