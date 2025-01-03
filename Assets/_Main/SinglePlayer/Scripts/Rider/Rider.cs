using System;
using UnityEngine;
namespace FoodFury
{
    [DefaultExecutionOrder(-8)]
    [RequireComponent(typeof(Vehicle))]
    public class Rider : RiderBehaviour, IDriver
    {
        [Header("---Inputs")]
        [Range(1f, 10f)] public float accelerationLerp = 3;
        [Range(1f, 10f)] public float steerLerp = 2;
        [Range(0.0001f, 0.01f)] public float deadZoneValue = 0.001f;


        [Header("---Order")]
        [SerializeField] private TMPro.TextMeshPro orderDistanceTMP;
        [SerializeField] private Order targetOrder;
        [SerializeField] public Transform targetBoosterTransform;
        private Transform targetOrderTransorm;
        private float orderRange = 10;
        private float orderDistance;


        [Header("---Campass")]
        [SerializeField] private GameObject orderCampassParent;
        [SerializeField] private Transform orderCampassCanvasParent;
        [SerializeField] private Transform orderCampassPivot;
        [SerializeField] private SpriteRenderer orderCampassSprite;
        [SerializeField] private Transform boosterCampass;
        private float campassHeight = 5;


        [Header("---Booster")]
        [SerializeField] private ModelClass.BoosterData collectedBooster;

        [Header("Weapon")]
        private PlayerWeaponTriggerComponent playerWeaponTriggerComponent;

        private void Start()
        {
            orderCampassParent.transform.parent = null;
            orderCampassParent.transform.rotation = Quaternion.identity;

            boosterCampass.parent = null;
            boosterCampass.gameObject.SetActive(false);
            boosterCampass.transform.rotation = Quaternion.identity;

            collectedBooster.Reset();
            playerWeaponTriggerComponent = GetComponent<PlayerWeaponTriggerComponent>();
        }

        private void OnEnable()
        {
            Booster.OnBoosterCollected += OnBoosterCollectedEvent;
            OrderManager.OnNewOrderGenerated += OnNewOrderGenerated;
            BoosterManager.OnBoosterGenerated += OnBoosterGenerated;
            GameplayScreen.OnGameReady += OnGameReady;
        }


        private void OnDisable()
        {
            Booster.OnBoosterCollected -= OnBoosterCollectedEvent;
            OrderManager.OnNewOrderGenerated -= OnNewOrderGenerated;
            BoosterManager.OnBoosterGenerated -= OnBoosterGenerated;
            GameplayScreen.OnGameReady -= OnGameReady;
        }


        private void OnGameReady()
        {
            collectedBooster.Reset();
            //SetInitialPosition();
        }


        void Update()
        {
            HandleInputs();

            UpdateDistance();
            UpdateCampass();
        }

        public void SetInitialPosition()
        {
            //transform.position = startingPosition;
            transform.rotation = Quaternion.identity;
        }

        public void SetBoosterCampass(bool _flag) => boosterCampass.gameObject.SetActive(false);


        #region Inputs
        private void HandleInputs()
        {
            if (GameData.Instance.IsGamePaused)
            {
                Vehicle.SetAccerateInput(accelerateInput = 0);
                Vehicle.SetSteerInput(steerInput = 0);
                return;
            }

            if (!overrideInputs)
            {
                if (GameData.Instance.IsGamePaused)
                {
                    accelerateInput = Mathf.Lerp(accelerateInput, 0, Time.deltaTime * accelerationLerp);
                    steerInput = Mathf.Lerp(steerInput, 0, Time.deltaTime * steerLerp);
                }
                else
                {
                    accelerateInput = Mathf.Lerp(accelerateInput, InputManager.Instance.GetInputs().y, Time.deltaTime * accelerationLerp);
                    steerInput = Mathf.Lerp(steerInput, InputManager.Instance.GetInputs().x, Time.deltaTime * steerLerp);
                }

                accelerateInput = ClampDeadzone(accelerateInput);
                steerInput = ClampDeadzone(steerInput);
            }

            Vehicle.SetAccerateInput(accelerateInput);
            Vehicle.SetSteerInput(steerInput);
        }

        private float ClampDeadzone(float _value) => HelperFunctions.GetAbs(_value) < deadZoneValue ? 0 : _value;
        #endregion



        #region Distance and Compass
        private void UpdateDistance()
        {
            if (targetOrderTransorm == null) return;
            orderDistance = HelperFunctions.GetDistance(transform.position, targetOrderTransorm.position);
        }


        Vector3 _orderDirection;
        bool _activeState;
        float _orderInterval;
        private void UpdateCampass()
        {
            //if (targetBoosterTransform != null) CampassBooster();
            if (targetOrder == null || targetOrderTransorm == null) return;

            orderDistanceTMP.text = $"{Mathf.FloorToInt(orderDistance)}m";
            float _interval = targetOrder.RemainingTime / (GameController.Instance.LevelInfo.CurrentLevel.orderTime * 1f);
            orderCampassSprite.color = _interval < 0.33f ? ColorManager.Instance.Red : _interval < 0.66 ? ColorManager.Instance.Yellow : ColorManager.Instance.DarkGreen;


            // Active State
            _activeState = orderDistance > orderRange;
            if (orderCampassParent.activeSelf != _activeState) orderCampassParent.SetActive(_activeState);

            if (!_activeState) return;

            // Position and rotation
            orderCampassPivot.position = transform.position + Vector3.up * campassHeight;
            orderCampassCanvasParent.position = orderCampassPivot.position;

            _orderDirection = targetOrderTransorm.position - transform.position;
            _orderDirection.y = orderCampassPivot.localRotation.y;
            orderCampassPivot.rotation = Quaternion.Slerp(orderCampassPivot.rotation, Quaternion.LookRotation(_orderDirection), Time.deltaTime * accelerationLerp);
        }

        Vector3 _boosterDirection;
        private void CampassBooster()
        {
            boosterCampass.position = transform.position + Vector3.up * (campassHeight - 2);

            _boosterDirection = targetBoosterTransform.position - transform.position;
            _boosterDirection.y = boosterCampass.localRotation.y;
            boosterCampass.rotation = Quaternion.Slerp(boosterCampass.rotation, Quaternion.LookRotation(_boosterDirection), Time.deltaTime * accelerationLerp);
        }
        #endregion



        #region Order
        private void Event_OnOrderCollected(Order _order)
        {
            OnOrderCollected?.Invoke(_order);
            HapticsManager.MediumHaptic();
            targetOrder = null;
            //orderToCollect =
            targetOrderTransorm = null;
            orderCampassParent.SetActive(false);
        }


        private void OnNewOrderGenerated(Order _order)
        {
            targetOrder = _order;
            //targetOrder = OrderManagement.Instance.GetNearestOrder(transform.position);
            targetOrderTransorm = targetOrder.transform;
            _orderInterval = _order.RemainingTime / 3f;
            orderCampassParent.SetActive(true);
        }
        #endregion



        #region Boosters
        private void OnBoosterCollectedEvent(Booster _booster)
        {
            switch (_booster.BoosterData.type)
            {
                case Enums.BoosterType.Speed:
                    collectedBooster = _booster.BoosterData;
                    OnBoosterCollected?.Invoke(_booster.BoosterData);
                    if (GameplayScreen.Instance != null)
                        GameplayScreen.Instance.StartBoosterTimer(collectedBooster.duration, () =>
                        {
                            OnBoosterEnd?.Invoke(collectedBooster.type);
                            collectedBooster.Reset();
                        });
                    break;
            }

            AnalyticsManager.Instance.FireBoosterCollectedEvent(_booster.BoosterData.type.ToString(), _booster.BoosterData.duration);

            targetBoosterTransform = null;
        }

        private void OnBoosterGenerated(Booster _booster) => targetBoosterTransform = _booster.transform;
        #endregion



        #region Damage
        public void TakeDamage(float _damage)
        {
            Vehicle.VehicleHealth.TakeDamage(_damage);
            SetVehicleChanges();
            OnVehicleDamage?.Invoke(_damage);
        }

        private void SetVehicleChanges()
        {
            if (Vehicle.VehicleHealth.CurrentHealth < 10)
                Vehicle.SetMaxSpeed(Vehicle.MaxSpeed * 0.7f);
            else if (Vehicle.VehicleHealth.CurrentHealth < 20)
                Vehicle.SetMaxSpeed(Vehicle.MaxSpeed * 0.8f);
            else if (Vehicle.VehicleHealth.CurrentHealth < 30)
                Vehicle.SetMaxSpeed(Vehicle.MaxSpeed * 0.9f);
        }

        private void RestoreVehicleChanges()
        {
            if (collectedBooster.type == Enums.BoosterType.None) return;

            if (Vehicle.VehicleHealth.CurrentHealth > 70)
                Vehicle.SetMaxSpeed(Vehicle.InitialSpeed);
            else if (Vehicle.VehicleHealth.CurrentHealth > 50)
                Vehicle.SetMaxSpeed(Vehicle.InitialSpeed * 0.8f);
            else if (Vehicle.VehicleHealth.CurrentHealth > 30)
                Vehicle.SetMaxSpeed(Vehicle.InitialSpeed * 0.7f);
        }
        #endregion



        #region OnTrigger
        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.TryGetComponent(out Order _order))
            {
                if (_order.collectingBy != Enums.OrderCollectingBy.None) return;

                _order.collectingBy = Enums.OrderCollectingBy.Player;
                _order.ToggleLocator(false);
                _order.OnOrderCollected += Event_OnOrderCollected;
                Debug.Log("Enums.OrderStatus.Delivering -> Enums.OrderStatus.Delivered done here");
                if (GameplayScreen.Instance != null) GameplayScreen.Instance.orderStatus.ShowOrderStatus(Enums.OrderStatus.Delivered, _initialDelay: 0, _autoHideDelay: 0);
            }
            else if (other.transform.TryGetComponent(out Booster _booster))
            {
                if (_booster.BoosterData.type != collectedBooster.type)
                    _booster.Collect();
            }
            else if (other.transform.TryGetComponent(out WeaponItem _weaponItem))
            {
                if (playerWeaponTriggerComponent.Weapon != null && GameData.Instance.releaseMode == Enums.ReleaseMode.Release) return;

                _weaponItem.Collect();
            }
        }

        //private void OnTriggerStay(Collider other)
        //{
        //    if (other.transform.TryGetComponent(out Order _order) && _order.collectingBy == Enums.OrderCollectingBy.None)
        //    {
        //        _order.collectingBy = Enums.OrderCollectingBy.Player;
        //        _order.ToggleLocator(false);
        //        if (GameplayScreen.Instance != null) GameplayScreen.Instance.orderStatus.ShowOrderStatus("Delivering", ColorManager.Instance.Orange, _autoHideDelay: 0);

        //    }
        //}

        private void OnTriggerExit(Collider other)
        {
            if (other.transform.TryGetComponent(out Order _order))
            {
                _order.ToggleLocator(true);
                _order.OnOrderCollected -= Event_OnOrderCollected;
                if (GameplayScreen.Instance != null) GameplayScreen.Instance.orderStatus.HideOrderStatus();
                _order.collectingBy = Enums.OrderCollectingBy.None;
            }
        }
        #endregion



        #region OnCollision
        float _damage;
        float _speedFactor;
        float _directionFactor;
        bool detectCollision = true;
        private void OnCollisionEnter(Collision collision)
        {
            if (!detectCollision) return;

            detectCollision = false;
            Invoke("DetectCollision", 0.5f);

            // Collided with Other Rider
            if (collision.gameObject.TryGetComponent(out IDriver _other))
            {
                _speedFactor = (Vehicle.CurrentSpeed01 + _other.Vehicle.CurrentSpeed01) * 0.5f;
                _directionFactor = Vector3.Dot(transform.forward, collision.transform.forward);
                if (HelperFunctions.GetAbs(_directionFactor) < 0.4f) // Side collision almost perpendicular
                {
                    _damage = _speedFactor * (HelperFunctions.GetAbs(_directionFactor) < 0.05 ? 1 : HelperFunctions.GetAbs(_directionFactor));
                }
                else if (_directionFactor > .7f) // Same Direction
                {
                    _damage = (_speedFactor < 0.5f ? (1f - _speedFactor) : _speedFactor) * HelperFunctions.GetAbs(_directionFactor); // if one rider is stop take max damage else take calculated damage
                }
                else if (_directionFactor < -.7f) // Opposite Direction
                {
                    _damage = _speedFactor * HelperFunctions.GetAbs(_directionFactor); // Full Damage
                }
                else
                    _damage = _speedFactor * (1 - HelperFunctions.GetAbs(_directionFactor));

                _damage = Mathf.Clamp(_damage, 0.1f, 1f);
                TakeDamage(_damage);
                _other.TakeDamage(.8f);

                //Debug.Log($"Collided with {collision.collider.name} | SpeedFactor {_speedFactor} | DirectionFactor  {_directionFactor} | Damage({_d}) {_damage}");
            }
            else if (collision.collider.CompareTag(LayerAndTagManager.Instance.TagDamageCollider))// Collided with city
            {
                _speedFactor = Vehicle.CurrentSpeed01;
                _directionFactor = Vector3.Dot(transform.forward, collision.GetContact(0).normal.normalized);
                _damage = _speedFactor * HelperFunctions.GetAbs(_directionFactor);

                _damage = Mathf.Clamp(_damage, 0.1f, 1f);

                TakeDamage(_damage);

                //Debug.Log($"Collided with {collision.collider.name} | SpeedFactor {_speedFactor} | DirectionFactor  {_directionFactor} | Damage {_damage}");
            }
            //else
            //    Debug.Log("Collided with " + collision.collider.name);

        }

        private void OnCollisionStay(Collision collision)
        {
            if (Vehicle.overrideInput) return;

            if (collision.collider.CompareTag(LayerAndTagManager.Instance.TagDamageCollider) || collision.collider.CompareTag(LayerAndTagManager.Instance.TagPlayer))
                Vehicle.overrideInput = true;
        }

        private void OnCollisionExit(Collision collision)
        {
            if (!Vehicle.overrideInput) return;

            if (collision.collider.CompareTag(LayerAndTagManager.Instance.TagDamageCollider) || collision.collider.CompareTag(LayerAndTagManager.Instance.TagPlayer))
                Vehicle.overrideInput = false;
        }

        private void DetectCollision() => detectCollision = true;
        #endregion

    }
}