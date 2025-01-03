using System;
using FoodFury;
using Fusion;
using Fusion.Addons.SimpleKCC;
using OneRare.FoodFury.Multiplayer;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace OneRare.FoodFury.Multiplayer
{
    public class PlayerMovementHandler : NetworkBehaviour
    {
        [SerializeField] private SimpleKCC kcc;
        [SerializeField] private TrailRenderer primaryWheel;

        [Header("---Movement")]
        //[SerializeField] private float skidThreshold = 0.002f;
        [SerializeField]
        private float acceleration;

        [SerializeField] private float reverseSpeed;
        [SerializeField] private float deceleration;
        [SerializeField] public float maxSpeedNormal;
        [SerializeField] private float maxSpeedBoosting;
        private float _currentSpeed;


        [Header("---Steering")] public Transform body;
        public Transform handle;
        [SerializeField] private float maxBodyTileAngle = 40;
        [SerializeField] private float steerAcceleration;
        [SerializeField] private float steerDeceleration;
        public Transform model;
        [SerializeField] private float driftRotationLerpFactor = 10f;
        [field: SerializeField] public float DriftFactor { get; private set; }
        //private float GroundResistance { get; set; }

        [field: Header("Networked Properties")]
        [Networked]
        public float MaxSpeed { get; set; }

        public float odometerSpeed;


        private float _currentSpeed01;

        [Networked] public int BoostEndTick { get; set; } = -1;
        [Networked] public float AppliedSpeed { get; set; }
        [Networked] private float SteerAmount { get; set; }

        public NetworkBool IsGrounded { get; set; }

        public NetworkBool IsAllowedToAccelerate { get; set; }
        private Player _player = null;

        public override void Spawned()
        {
            MaxSpeed = maxSpeedNormal;
        }

        public bool IsIdle() => AppliedSpeed / MaxSpeed < 0.03f;

        public void Initialize(Player player)
        {
            _player = player;
        }

        private void Update()
        {
            Drift();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out ICollidable collidable) &&
                other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                collidable.Collide(gameObject.GetComponentInParent<Player>());
            }
        }

        public void GroundNormalRotation()
        {
            if (body == null)
                return;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -transform.up, out hit, 2f, ~LayerMask.GetMask("Booster")))
            {
                IsGrounded = true;
            }

            if (IsGrounded)
            {
                //GroundResistance = hit.collider.material.dynamicFriction;

                model.transform.rotation = Quaternion.Lerp(
                    model.transform.rotation,
                    Quaternion.FromToRotation(model.transform.up * 2, hit.normal) * model.transform.rotation,
                    7.5f * Time.deltaTime);
            }
        }


        private float _inputDeadZoneValue = 0.001f;

        private float collisionAngle = 0;

        //private bool isCollidingWithCityWall = false;
        private Vector3 direction;
        private float steerAway;
        private float angleToWall;


        private float accelerationTime = 1.5f; // Reduced time in seconds to reach top speed
        private float currentAccelerationTime = 0f;
        private float decelerationFactor = 4f; // Factor to control deceleration speed
        private float reverseAccelerationTime = 3f; // Time in seconds to reach top reverse speed
        private float currentReverseAccelerationTime = 0f;
        private float decelerationDuration = 5f; // Time in seconds to decelerate to 0 from max speed

        public void Move(NetworkInputData input)
        {
            if (input.IsReverse)
            {
                direction = -transform.forward;
            }
            else
            {
                direction = transform.forward;
            }

            /*// SphereCast to detect collision with walls or players
            if (Physics.SphereCast(transform.position + Vector3.up * 1.5f, 0.75f, direction, out RaycastHit hitCityWall, 1f, LayerMask.GetMask("CityCollider", "Player")))
            {
                isCollidingWithCityWall = true;

                // Reflect the direction when colliding
                Vector3 reflectDirection = Vector3.Reflect(transform.forward, hitCityWall.normal);
                Vector3 steerDirection = Vector3.ProjectOnPlane(transform.forward, hitCityWall.normal.normalized);

                angleToWall = Vector3.SignedAngle(transform.forward, reflectDirection, Vector3.up);
                steerAway = Mathf.Sign(Vector3.Dot(steerDirection, reflectDirection));

                // DO NOT modify speed here to prevent slowdown
                // Remove this: AppliedSpeed = Mathf.Lerp(AppliedSpeed, maxSpeedNormal, decelerationFactor * Runner.DeltaTime);
            }
            else
            {
                isCollidingWithCityWall = false;
            }*/

            // Acceleration and deceleration logic remains the same
            if (input.IsAccelerate /*&& !isCollidingWithCityWall*/)
            {
                currentAccelerationTime += Runner.DeltaTime;
                float progress = Mathf.Clamp01(currentAccelerationTime / accelerationTime);
                AppliedSpeed = Mathf.Lerp(0, MaxSpeed, progress);
            }
            else if (input.IsReverse)
            {
                currentReverseAccelerationTime += Runner.DeltaTime;
                float reverseProgress = Mathf.Clamp01(currentReverseAccelerationTime / reverseAccelerationTime);
                AppliedSpeed = Mathf.Lerp(0, -reverseSpeed, reverseProgress);
            }
            else
            {
                // Gradually reduce speed to 0 over `decelerationDuration` seconds
                float decelerationProgress = Mathf.Clamp01(Runner.DeltaTime / decelerationDuration);
                AppliedSpeed = Mathf.Lerp(AppliedSpeed, 0, decelerationProgress);

                currentAccelerationTime = 0f; // Reset acceleration time when not accelerating
                currentReverseAccelerationTime = 0f; // Reset reverse acceleration time when not reversing
            }

            // Moving the object based on the applied speed
            Vector3 localDirection = new Vector3(0, 0, AppliedSpeed);
            Vector3 worldDirection = transform.TransformDirection(localDirection);
            kcc.Move(worldDirection * Runner.DeltaTime);

            var resistance = 1 - (IsGrounded ? 1 : 0);
            if (resistance < 1)
            {
                AppliedSpeed = Mathf.Lerp(AppliedSpeed, AppliedSpeed * resistance,
                    Runner.DeltaTime * (_isDrifting ? 8 : 2));
            }

            odometerSpeed = AppliedSpeed / MaxSpeed;
        }

        //Steer
        //odometerSpeed = AppliedSpeed / MaxSpeed;
        private bool _canDrive = true;
        private bool _isDrifting;
        [SerializeField] private float turnDecelerationFactor = 0.3f; // Factor to control deceleration during turns

        public void Steer(NetworkInputData input)
        {
            var steerTarget = input.Steer * AppliedSpeed / (BoostTime > 0 ? 1.8f : 1.6f);
            //Debug.Log($"ST: {steerTarget}");
            if (SteerAmount != steerTarget)
            {
                var steerLerp = Mathf.Abs(SteerAmount) < Mathf.Abs(steerTarget) ? steerAcceleration : steerDeceleration;
                SteerAmount = Mathf.Lerp(SteerAmount, steerTarget, Runner.DeltaTime * steerLerp);
            }

            // Decelerate when turning
            float turnInput = Mathf.Abs(input.Steer); // Assuming input.Steer ranges from -1 to 1
            float turnDeceleration = Mathf.Lerp(1, turnDecelerationFactor, turnInput);

            AppliedSpeed *= turnDeceleration;

            if (_isDrifting)
            {
                model.localEulerAngles = LerpAxis(Axis.Y, model.localEulerAngles, SteerAmount * 0.2f,
                    driftRotationLerpFactor * Runner.DeltaTime);
            }
            else
            {
                model.localEulerAngles = LerpAxis(Axis.Y, model.localEulerAngles, 0, 6 * Runner.DeltaTime);
            }

            if (_canDrive)
            {
                kcc.AddLookRotation(0, SteerAmount * Runner.DeltaTime);
                /*if (!isCollidingWithCityWall)
                {
                    kcc.AddLookRotation(0, SteerAmount * Runner.DeltaTime);
                }
                else
                {
                    kcc.AddLookRotation(0, steerAway * 1.2f * angleToWall * Runner.DeltaTime);
                }*/
            }

            HandleTilting(SteerAmount);
        }
        //Boost


        private float _si;
        float _bodyAngle;
        float _handleAngle;
        Vector3 _currentRotationBody;
        Vector3 _currentRotationHandle;

        private void HandleTilting(float steerInput)
        {
            SetMaxBodyAngle();
            _si = steerInput / 50f;

            if (body)
            {
                _bodyAngle = Mathf.Lerp(_bodyAngle,
                    Mathf.Clamp(_si * maxBodyTileAngle, -maxBodyTileAngle, maxBodyTileAngle), Runner.DeltaTime * 12);
                _currentRotationBody = body.eulerAngles;
                body.eulerAngles = new Vector3(_currentRotationBody.x, _currentRotationBody.y, -_bodyAngle * 1.75f);
            }

            if (handle)
            {
                _handleAngle = Mathf.Lerp(_handleAngle, Mathf.Clamp(_si * 40, -35, 35), Runner.DeltaTime * 12);
                _currentRotationHandle = handle.localEulerAngles;
                handle.localEulerAngles =
                    new Vector3(_currentRotationHandle.x, _currentRotationHandle.y, _handleAngle + 180);
            }
        }

        //Drift
        private void SetMaxBodyAngle() => maxBodyTileAngle = Mathf.Lerp(10, 45, _currentSpeed01);

        private static Vector3 LerpAxis(Axis axis, Vector3 euler, float tgtVal, float t)
        {
            if (axis == Axis.X) return new Vector3(Mathf.LerpAngle(euler.x, tgtVal, t), euler.y, euler.z);
            if (axis == Axis.Y) return new Vector3(euler.x, Mathf.LerpAngle(euler.y, tgtVal, t), euler.z);
            return new Vector3(euler.x, euler.y, Mathf.LerpAngle(euler.z, tgtVal, t));
        }

        // private float GetSideVelocity() => Vector3.Dot(rigidbody.velocity.normalized, transform.right);
        Vector3 _forwardVelocity;
        Vector3 _sideVelocity;
        Vector3 _finalVelocity;

        public void Drift()
        {
            /*if (!IsGrounded) return;
            _forwardVelocity = Vector3.Dot(rigidbody.velocity, transform.forward) * transform.forward;
            _sideVelocity = Vector3.Dot(rigidbody.velocity, transform.right) * transform.right;
            _finalVelocity = _forwardVelocity + (DriftFactor * _sideVelocity);
            _finalVelocity.y = rigidbody.velocity.y;
            rigidbody.velocity = _finalVelocity;

            _isDrifting = IsGrounded && _currentSpeed01 > 0.1f && HelperFunctions.GetAbs(GetSideVelocity()) > skidThreshold;

            primaryWheel.emitting = _isDrifting;*/
        }

        // boost

        private int BoostTime => (int)(BoostEndTick == -1 ? 0f : (BoostEndTick - Runner.Tick) * Runner.DeltaTime);
        //private int _boostCount = 0;

        public void GiveBoost()
        {
            HapticsManager.MediumHaptic();
            AudioUtils.PlayOneShotAudio(_player.boosterCollectSound, transform.position);
            /*if (_boostCount > 0)
                return;*/
            if (BoostEndTick != -1)
            {
                return;
            }

            //_boostCount++;
            BoostEndTick = Runner.Tick;
            BoostEndTick += (int)(20f / Runner.DeltaTime);
        }

        public void Boost()
        {
            _player.OnBoosterTimeUpdated(BoostTime);
            if (BoostTime > 0)
            {
                MaxSpeed = maxSpeedBoosting;
                _player.gameUI.ShowOrHideBoostPanel(true);
                //AppliedSpeed = Mathf.Lerp(AppliedSpeed, MaxSpeed, Runner.DeltaTime);
            }
            else if (BoostEndTick != -1)
            {
                StopBoosting();
            }
        }

        private void StopBoosting()
        {
            BoostEndTick = -1;
            MaxSpeed = maxSpeedNormal;
            _player.gameUI.ShowOrHideBoostPanel(false);
        }

        public enum Axis
        {
            X,
            Y,
            Z
        }

        [SerializeField] private float accLerp = 1f;
        public void MoveBot(float input)
        {
            if (input < 0)
            {
                direction = -transform.forward;
            }
            else
            {
                direction = transform.forward;
            }

            /*if (Physics.SphereCast(transform.position + Vector3.up * 1.5f, 1.1f, direction, out RaycastHit hitCityWall, 1f, LayerMask.GetMask("CityCollider", "Player")))
            {
                AppliedSpeed = Mathf.Lerp(AppliedSpeed, 120f, decelerationFactor * Runner.DeltaTime);
                isCollidingWithCityWall = true;

                Vector3 reflectDirection = Vector3.Reflect(transform.forward, hitCityWall.normal);
                Vector3 steerDirection = Vector3.ProjectOnPlane(transform.forward, hitCityWall.normal.normalized);

                angleToWall = Vector3.SignedAngle(transform.forward, reflectDirection, Vector3.up);
                steerAway = Mathf.Sign(Vector3.Dot(steerDirection, reflectDirection));
            }
            else
            {
                isCollidingWithCityWall = false;
            }*/

            if (input > 0 /*&& !isCollidingWithCityWall*/)
            {
                currentAccelerationTime += Runner.DeltaTime;
                float progress = Mathf.Clamp01(currentAccelerationTime / accelerationTime);
                AppliedSpeed = Mathf.Lerp(0, MaxSpeed, progress);
            }
            else if (input < 0)
            {
                currentReverseAccelerationTime += Runner.DeltaTime;
                float reverseProgress = Mathf.Clamp01(currentReverseAccelerationTime / reverseAccelerationTime);
                AppliedSpeed = Mathf.Lerp(0, -reverseSpeed, reverseProgress);
            }
            else
            {
                // Gradually reduce speed to 0 over `decelerationDuration` seconds
                float decelerationProgress = Mathf.Clamp01(Runner.DeltaTime / decelerationDuration);
                AppliedSpeed = Mathf.Lerp(AppliedSpeed, 0, decelerationProgress);

                currentAccelerationTime = 0f; // Reset acceleration time when not accelerating
                currentReverseAccelerationTime = 0f; // Reset reverse acceleration time when not reversing
            }

            Vector3 localDirection = new Vector3(0, 0, AppliedSpeed / 1.7f);
            Vector3 worldDirection = transform.TransformDirection(localDirection);
            kcc.Move(worldDirection * Runner.DeltaTime * accLerp);

            var resistance = 1 - (IsGrounded ? 1 : 0);
            if (resistance < 1)
            {
                AppliedSpeed = Mathf.Lerp(AppliedSpeed, AppliedSpeed * resistance,
                    Runner.DeltaTime * (_isDrifting ? 8 : 2));
            }

            odometerSpeed = AppliedSpeed / MaxSpeed;
        }

        [SerializeField] private float steerLerp = 1f;
        public void SteerBot(float input)
        {
            var steerTarget = input * AppliedSpeed / 3f;

            if (SteerAmount != steerTarget)
            {
                var steerLerp = Mathf.Abs(SteerAmount) < Mathf.Abs(steerTarget) ? steerAcceleration : steerDeceleration;
                SteerAmount = Mathf.Lerp(SteerAmount, steerTarget, Runner.DeltaTime * steerLerp);
            }

            // Decelerate when turning
            float turnInput = Mathf.Abs(input); // Assuming input.Steer ranges from -1 to 1
            float turnDeceleration = Mathf.Lerp(1, turnDecelerationFactor, turnInput);

            AppliedSpeed *= turnDeceleration;

            if (_isDrifting)
            {
                model.localEulerAngles = LerpAxis(Axis.Y, model.localEulerAngles, SteerAmount * 0.2f,
                    driftRotationLerpFactor * Runner.DeltaTime);
            }
            else
            {
                model.localEulerAngles = LerpAxis(Axis.Y, model.localEulerAngles, 0, 6 * Runner.DeltaTime);
            }

            if (_canDrive)
            {
                kcc.AddLookRotation(0, SteerAmount * Runner.DeltaTime * steerLerp);
                /*if (!isCollidingWithCityWall)
                {
                    kcc.AddLookRotation(0, SteerAmount * Runner.DeltaTime);
                }
                else
                {
                    kcc.AddLookRotation(0, steerAway * 2f * angleToWall * Runner.DeltaTime);
                }*/
            }

            HandleTilting(SteerAmount);
        }
    }
}