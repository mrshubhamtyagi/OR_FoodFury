using System;
using System.Collections;
using UnityEngine;
namespace FoodFury
{
    [DefaultExecutionOrder(-10)]
    public class Vehicle : MonoBehaviour
    {
        [field: SerializeField] public Enums.RiderType RiderType { get; private set; }
        [field: SerializeField] public bool IsIdle { get; private set; }
        [field: SerializeField] public bool IsGrounded { get; private set; }
        [field: SerializeField] public float CurrentSpeed { get; private set; }
        [field: SerializeField] public float CurrentSpeed01 { get; private set; }

        #region VehicleComponents
        [field: SerializeField] public VehicleConfig VehicleConfig { get; private set; }
        [field: SerializeField] public ModelClass.GarageVehicleData VehicleData { get; private set; }
        public VehicleHealth VehicleHealth { get; private set; }
        public VehicleFuel VehicleFuel { get; private set; }
        public VehicleEffects VehicleEffects { get; private set; }
        public VehicleSound VehicleSound { get; private set; }
        #endregion

        #region Controls
        [Header("---Controls")]
        public Enums.VehicleType vehicleType = Enums.VehicleType.TwoWheeler;
        [field: SerializeField] public float MaxSpeed { get; private set; }
        [field: SerializeField] public float AccelerateFactor { get; private set; }
        [field: SerializeField] public float TurnFactor { get; private set; }
        [field: SerializeField] public float DriftFactor { get; private set; }
        [field: SerializeField] public float BreakFactor { get; private set; }
        [field: SerializeField] public float MaxBodyTileAngle { get; set; }


        [field: SerializeField] public float InitialSpeed { get; private set; }
        public float CustomGravity { get; private set; }
        public float GroundedDistance { get; private set; }
        #endregion


        [Header("---Body Parts")]
        [SerializeField] private Transform handle;
        [SerializeField] private Transform body;
        [SerializeField] private Transform raycastCenter;


        public bool stabilizeVehicle;
        public bool isStabalizing;

        private float accelerateInput = 0;
        private float steerInput = 0;
        private float inputDeadZoneValue = 0.001f;
        private BoxCollider boxCollider;

        [SerializeField] private float lastMaxSpeed;
        public float rotationAngleY;

        public RiderBehaviour Rider { get; private set; }
        private Rigidbody rigidBody;
        public Action<float> OnMaxSpeedChanged;
        void Awake()
        {
            Rider = GetComponent<RiderBehaviour>();
            rigidBody = GetComponent<Rigidbody>();
            VehicleHealth = GetComponent<VehicleHealth>();
            VehicleFuel = GetComponent<VehicleFuel>();
            VehicleEffects = GetComponent<VehicleEffects>();
            VehicleSound = GetComponent<VehicleSound>();
            if (RiderType == Enums.RiderType.Player) boxCollider = GetComponent<BoxCollider>();
        }

        void Start() => rotationAngleY = transform.eulerAngles.y;

        public void SetVehicleConfig()
        {
            RiderType = VehicleConfig.RiderType;
            vehicleType = VehicleConfig.vehicleType;

            AccelerateFactor = VehicleConfig.Acceleration;
            TurnFactor = VehicleConfig.Turn;
            BreakFactor = VehicleConfig.Break;
            DriftFactor = VehicleConfig.Drift;

            MaxBodyTileAngle = VehicleConfig.MaxBodyTileAngle;

            CustomGravity = VehicleConfig.CustomGravity;
            if (CustomGravity == 0) CustomGravity = 0.2f;

            inputDeadZoneValue = VehicleConfig.DeadZoneValue;
            //if (GroundedDistance == 0) 
            GroundedDistance = 0.5f;


            // Max Speed
            if (RiderType == Enums.RiderType.Player)
            {
                VehicleData = GameData.Instance.GarageData.GetVehicleGarageData(GameData.Instance.PlayerData.Data.currentVehicle);
                int _speedLevel = GameData.Instance.PlayerData.Data.GetCurrentVehicleLevels().speedLevel;
                InitialSpeed = _speedLevel == 0 ? VehicleData.initial.speed : VehicleData.speedUpgrades[_speedLevel - 1];
                SetMaxSpeed(InitialSpeed);
            }
            else
            {
                InitialSpeed = VehicleConfig.Speed;
                MaxSpeed = InitialSpeed;
                VehicleEffects.skidThreshold = Mathf.Clamp(MaxSpeed * 0.006f, 0.12f, 0.18f);
            }
        }


        public void SetBody(Transform _body, Transform _handle)
        {
            body = _body;
            handle = _handle;
        }

        private void OnEnable()
        {
            Rider.OnBoosterCollected += OnBoosterCollected;
            Rider.OnBoosterEnd += OnBoosterEnded;
            GameController.OnLevelComplete += StopVehicle;
            GameController.OnLevelFailed += StopVehicle;
            GameplayScreen.OnGameReady += OnGameReady;
        }

        private void StopVehicle()
        {
            rigidBody.linearVelocity = Vector3.zero;
            CurrentSpeed = 0;
            CurrentSpeed01 = 0;
        }

        private void OnDisable()
        {
            Rider.OnBoosterCollected -= OnBoosterCollected;
            Rider.OnBoosterEnd -= OnBoosterEnded;
            GameController.OnLevelComplete -= StopVehicle;
            GameController.OnLevelFailed -= StopVehicle;
            GameplayScreen.OnGameReady -= OnGameReady;
        }

        private void OnGameReady()
        {
            if (RiderType != Enums.RiderType.Player) return;

            //rotationAngleY = 0;
            SetMaxSpeed(GameController.Instance.LevelInfo.LevelBooster == Enums.LevelBoosterType.Speed ? InitialSpeed + GameData.Instance.GameSettings.levelSpeedBoosterValue : InitialSpeed);
        }

        void Update()
        {
            if (RiderType != Enums.RiderType.Player && GameData.Instance.IsGamePaused) return;

            HandleTilting();
            CheckIsIdle();
            //StabilizeVehicle();
        }

        private void FixedUpdate()
        {
            if (RiderType != Enums.RiderType.Player && GameData.Instance.IsGamePaused) return;

            CheckIsGrounded();
            Accelerate();
            UpdateCurrentSpeed();
            Breaks();
            Drift();
            Steer();
            ApplyGravityAndBalance();
        }


        #region Vehicle Factors
        private void SetMaxBodyAngle() => MaxBodyTileAngle = Mathf.Lerp(10, VehicleConfig.MaxBodyTileAngle, CurrentSpeed01);
        public void ForceDriftTo(float _value) => DriftFactor = Mathf.Clamp(_value, VehicleConfig.Drift, 1);

        public void SetMaxSpeed(float _value)
        {
            float _percentage;
            if (RiderType == Enums.RiderType.Player)
            {
                MaxSpeed = Mathf.Clamp(_value, VehicleData.initial.speed, VehicleData.speedUpgrades[VehicleData.speedUpgrades.Count - 1]);
                _percentage = Mathf.Clamp(MaxSpeed / 40f, 0.1f, 1f);
                AccelerateFactor = Mathf.Lerp(MaxSpeed, MaxSpeed * 2, _percentage);
                DriftFactor = Mathf.Lerp(VehicleConfig.Drift, 0.9f, _percentage);
                TurnFactor = Mathf.Lerp(VehicleConfig.Turn, VehicleConfig.Turn + 1, _percentage);
                //_percentage = (MaxSpeed - VehicleData.initial.speed) / (VehicleData.speedUpgrades[VehicleData.speedUpgrades.Count - 1] - VehicleData.initial.speed);
                // AccelerateFactor = Mathf.Lerp(VehicleData.initial.speed, VehicleData.initial.speed + 10, _percentage);
            }
            else
            {
                MaxSpeed = Mathf.Clamp(_value, VehicleConfig.Speed, VehicleConfig.Speed * 2);
                _percentage = (MaxSpeed - VehicleConfig.Speed) / (VehicleConfig.Speed * 2 - VehicleData.initial.speed);
                AccelerateFactor = Mathf.Lerp(VehicleConfig.Acceleration, VehicleConfig.Acceleration + 10, _percentage);
                DriftFactor = Mathf.Lerp(VehicleConfig.Drift, 0.8f, _percentage);
            }

            VehicleEffects.skidThreshold = Mathf.Clamp(MaxSpeed * 0.006f, 0.12f, 0.18f);
            SetMaxBodyAngle();
            OnMaxSpeedChanged?.Invoke(_percentage);
        }

        //private void OnValidate()
        //{
        //    SetMaxSpeed(MaxSpeed);
        //    SetMaxBodyAngle();
        //}

        #endregion



        #region Controls
        [HideInInspector] public bool overrideInput = false;
        private void Accelerate()
        {
            if (!IsGrounded) return;

            rigidBody.AddForce((overrideInput && accelerateInput > 0 ? 0.5f : accelerateInput) * AccelerateFactor * transform.forward, ForceMode.Acceleration);
            rigidBody.linearVelocity = Vector3.ClampMagnitude(rigidBody.linearVelocity, MaxSpeed);
        }

        float minSpeedToAllowTurn;
        float _multi;
        private void Steer()
        {
            if (!IsGrounded) return;

            //_multi = Mathf.Lerp(10, 15, (MaxSpeed - VehicleConfig.Speed) / (VehicleConfig.Speed * 2f - VehicleConfig.Speed));
            _multi = Mathf.Lerp(8, 12, MaxSpeed);
            minSpeedToAllowTurn = Mathf.Clamp01(overrideInput ? 0.4f : CurrentSpeed / _multi);
            rotationAngleY += steerInput * TurnFactor * minSpeedToAllowTurn * GetMovingDirection();
            rotationAngleY %= 360;
            rigidBody.MoveRotation(Quaternion.Euler(0, rotationAngleY, 0));
            //rigidBody.MoveRotation(Quaternion.Slerp(rigidBody.rotation, Quaternion.Euler(0, rotationAngleY, 0), Time.fixedDeltaTime * 8));

            int GetMovingDirection() => accelerateInput < 0 ? -1 : 1;
        }

        Vector3 forwardVelocity;
        Vector3 sideVelocity;
        Vector3 finalVelocity;
        private void Drift()
        {
            if (!IsGrounded) return;

            forwardVelocity = Vector3.Dot(rigidBody.linearVelocity, transform.forward) * transform.forward;
            sideVelocity = Vector3.Dot(rigidBody.linearVelocity, transform.right) * transform.right;
            finalVelocity = forwardVelocity + (DriftFactor * sideVelocity);
            finalVelocity.y = rigidBody.linearVelocity.y;
            rigidBody.linearVelocity = finalVelocity;
        }

        float _drag;
        private void Breaks()
        {
            _drag = Mathf.Lerp(rigidBody.linearDamping, (1 - HelperFunctions.GetAbs(accelerateInput)) * BreakFactor, Time.fixedDeltaTime * 3);
            if (_drag < inputDeadZoneValue) _drag = 0;
            else if (_drag > BreakFactor - inputDeadZoneValue) _drag = BreakFactor;
            rigidBody.linearDamping = IsIdle ? 5 : _drag;
            rigidBody.angularDamping = IsIdle ? 10 : 0.1f;
        }

        private void ApplyGravityAndBalance()
        {
            if (!IsGrounded)
            {
                rigidBody.linearVelocity += Vector3.down * CustomGravity;
                rigidBody.rotation = Quaternion.Lerp(rigidBody.rotation, Quaternion.Euler(0, rotationAngleY, 0), Time.fixedDeltaTime);
            }

            if (IsIdle)
            {
                rigidBody.rotation = Quaternion.Lerp(rigidBody.rotation, Quaternion.Euler(0, rotationAngleY, 0), Time.fixedDeltaTime);
            }
        }

        RaycastHit[] raycastHits = new RaycastHit[5];
        private void CheckIsGrounded() => IsGrounded = Physics.RaycastNonAlloc(raycastCenter.position, -transform.up, raycastHits, GroundedDistance, LayerAndTagManager.Instance.LayerGround) > 0;

        private void CheckIsIdle() => IsIdle = CurrentSpeed01 < 0.03f;

        private void StabilizeVehicle()
        {
            if (RiderType != Enums.RiderType.Player || isStabalizing) return;

            if (IsIdle && stabilizeVehicle)
            {
                isStabalizing = true;
                StartCoroutine("Co_Stabalize");
                return;
            }

            if (!IsIdle && !stabilizeVehicle) stabilizeVehicle = true;
        }

        private IEnumerator Co_Stabalize()
        {
            stabilizeVehicle = false;
            rigidBody.isKinematic = true;

            yield return new WaitForSeconds(0.1f);

            rigidBody.isKinematic = false;
            yield return new WaitForEndOfFrame();
            rigidBody.WakeUp();

            yield return new WaitForEndOfFrame();
            isStabalizing = false;
        }
        #endregion



        #region Inputs
        public void SetAccerateInput(float _input) => accelerateInput = _input < -0.9f ? -0.9f : _input; // Limit Reverse speed
        public void SetSteerInput(float _input) => steerInput = _input;
        //public void ApplyHandbrakes(bool _flag) => BreakFactor = _flag ? BreakFactor * 2 : VehicleConfig.Break;
        #endregion



        #region Booster
        private void OnBoosterCollected(ModelClass.BoosterData _booster)
        {
            print("OnBoosterCollected " + _booster.type);
            switch (_booster.type)
            {
                case Enums.BoosterType.Speed:
                    lastMaxSpeed = MaxSpeed;
                    //print(MaxSpeed + (MaxSpeed * (_booster.value / 100f)));
                    SetMaxSpeed(MaxSpeed + (MaxSpeed * (_booster.value / 100f)));
                    break;

                default:
                    break;
            }
        }

        private void OnBoosterEnded(Enums.BoosterType _type)
        {
            print("OnBoosterEnded " + _type);
            switch (_type)
            {
                case Enums.BoosterType.Speed:
                    SetMaxSpeed(lastMaxSpeed);
                    lastMaxSpeed = MaxSpeed;
                    break;

                default:
                    break;
            }
        }
        #endregion




        #region Other
        private void UpdateCurrentSpeed()
        {
            CurrentSpeed = rigidBody.linearVelocity.magnitude;
            CurrentSpeed01 = CurrentSpeed / MaxSpeed;
            if (CurrentSpeed < inputDeadZoneValue) CurrentSpeed01 = CurrentSpeed = 0;
            //CurrentSpeed01 = accelerateInput == 0 ? 0 : Mathf.Clamp01(CurrentSpeed / (accelerateInput < 0 ? MaxSpeed * reverseSpeedMultiplier : MaxSpeed));
        }

        RaycastHit _tileHit;
        Vector3 _raycastRightPos => raycastCenter.position + new Vector3(1.5f, 0, 0);
        Vector3 _raycastleftPos => raycastCenter.position + new Vector3(-1.5f, 0, 0);
        public Tile GetRiderTile()
        {
            //Debug.DrawRay(raycastCenter.position, -transform.up, Color.red, 2);
            //Debug.DrawRay(_raycastRightPos, -transform.up, Color.red, 2);
            //Debug.DrawRay(_raycastleftPos, -transform.up, Color.red, 2);
            if (Physics.Raycast(raycastCenter.position, -transform.up, out _tileHit, GroundedDistance, LayerAndTagManager.Instance.LayerRoadTile, QueryTriggerInteraction.Collide))
                return _tileHit.collider.GetComponent<Tile>();
            else if (Physics.Raycast(_raycastRightPos, -transform.up, out _tileHit, GroundedDistance, LayerAndTagManager.Instance.LayerRoadTile, QueryTriggerInteraction.Collide))
                return _tileHit.collider.GetComponent<Tile>();
            else if (Physics.Raycast(_raycastleftPos, -transform.up, out _tileHit, GroundedDistance, LayerAndTagManager.Instance.LayerRoadTile, QueryTriggerInteraction.Collide))
                return _tileHit.collider.GetComponent<Tile>();
            else
                return null;
        }



        float _bodyAngle;
        float _handleAngle;
        Vector3 currentRotationBody;
        Vector3 currentRotationHandle;
        private void HandleTilting()
        {
            if (RiderType == Enums.RiderType.Player || RiderType == Enums.RiderType.Rival) SetMaxBodyAngle();
            if (vehicleType == Enums.VehicleType.FourWheeler) return;


            if (body)
            {
                //_bodyAngle = Mathf.Lerp(_bodyAngle, Mathf.Clamp(steerInput * MaxBodyTileAngle, -MaxBodyTileAngle, MaxBodyTileAngle), Time.deltaTime * 10);
                _bodyAngle = Mathf.Clamp(steerInput * MaxBodyTileAngle, -MaxBodyTileAngle, MaxBodyTileAngle);
                currentRotationBody = body.eulerAngles;
                body.eulerAngles = new Vector3(currentRotationBody.x, currentRotationBody.y, -_bodyAngle);
            }

            if (handle)
            {
                //_handleAngle = Mathf.Lerp(_handleAngle, Mathf.Clamp(steerInput * VehicleConfig.MaxSteeringAngle, -VehicleConfig.MaxSteeringAngle, VehicleConfig.MaxSteeringAngle), Time.deltaTime * 10);
                _handleAngle = Mathf.Clamp(steerInput * VehicleConfig.MaxSteeringAngle, -VehicleConfig.MaxSteeringAngle, VehicleConfig.MaxSteeringAngle);
                currentRotationHandle = handle.localEulerAngles;
                handle.localEulerAngles = new Vector3(currentRotationHandle.x, currentRotationHandle.y, _handleAngle + 180);
            }
        }
        #endregion


        public float GetSideVelocity() => Vector3.Dot(rigidBody.linearVelocity.normalized, transform.right);



    }




}
