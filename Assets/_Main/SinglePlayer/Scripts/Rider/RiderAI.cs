using System.Collections.Generic;
using UnityEngine;

namespace FoodFury
{
    [DefaultExecutionOrder(-6)]
    [RequireComponent(typeof(Vehicle))]
    public class RiderAI : RiderBehaviour, IDriver
    {
        [Range(1f, 10f)][SerializeField] private float nextTargetRadius = 4;
        public bool doReverse = false;

        [Header("---Targets")]
        public bool loop = true;
        [SerializeField] private Transform WayPointParent;
        [SerializeField] public Transform currentTarget;
        [SerializeField] private float distanceToTarget;
        private List<Transform> targets = new List<Transform>();

        [SerializeField] private float steerLerp;
        public bool drawGizmo = true;

        private void Start()
        {
            //print("- RiderAI ->  " + gameObject.name);
            if (WayPointParent == null) return;

            WayPointParent.GetComponentsInChildren(targets);
            targets.Remove(WayPointParent);
            currentTarget = targets[0];
            steerLerp = Random.Range(5f, 8f);
        }


        private void OnDrawGizmos()
        {
            if (!drawGizmo) return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3.up * 30));
        }

        void Update()
        {
            if (GameData.Instance.IsGamePaused) return;
            if (currentTarget == null) return;

            UpdateTarget();
            HandleInputs();
        }

        private void OnEnable()
        {
            Vehicle.VehicleHealth.OnHealthFinished += OnFuelFinished;
            Vehicle.VehicleFuel.OnFuelFinished += OnFuelFinished;
        }

        private void OnDisable()
        {
            Vehicle.VehicleHealth.OnHealthFinished -= OnFuelFinished;
            Vehicle.VehicleFuel.OnFuelFinished -= OnFuelFinished;
        }

        private void OnFuelFinished()
        {
            accelerateInput = steerInput = 0;
            Vehicle.SetAccerateInput(accelerateInput);
            Vehicle.SetSteerInput(steerInput);
        }

        private void OnValidate()
        {
            if (WayPointParent != null)
                WayPointParent.GetComponent<Waypoints>().nextTargetRadius = nextTargetRadius;
        }


        #region Inputs
        Vector3 _direction;
        float _dot;
        private void HandleInputs()
        {
            if (!overrideInputs)
            {
                if (doReverse)
                    accelerateInput = Mathf.Lerp(accelerateInput, -1f, Time.deltaTime);
                else
                    accelerateInput = Mathf.Lerp(accelerateInput, distanceToTarget < nextTargetRadius ? -1 : 1, Time.deltaTime);


                _direction = currentTarget.position - transform.position;
                _dot = Vector3.Dot(transform.right, _direction.normalized);
                steerInput = Mathf.Lerp(steerInput, doReverse ? -_dot : _dot, Time.deltaTime * steerLerp);
            }

            Vehicle.SetAccerateInput(accelerateInput);
            Vehicle.SetSteerInput(steerInput);
        }
        #endregion



        #region Target
        int _index;
        private void UpdateTarget()
        {
            distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);

            if (distanceToTarget < nextTargetRadius && !overrideInputs)
            {
                _index = targets.IndexOf(currentTarget);

                if (loop)
                    _index = (_index + 1) % targets.Count;
                else if (_index != targets.Count - 1)
                    _index++;

                currentTarget = targets[_index];
            }
        }
        #endregion



        #region Damage
        private void StartReverse()
        {
            doReverse = true;
            Invoke("StopReverse", 4);
        }
        private void StopReverse() => doReverse = false;


        public void TakeDamage(float _damage)
        {
            Vehicle.VehicleHealth.TakeDamage(_damage * 5);
            SetVehicleChanges();
            OnVehicleDamage?.Invoke(_damage * 5);
        }
        private void SetVehicleChanges()
        {
            if (Vehicle.VehicleHealth.CurrentHealth < 30)
                Vehicle.SetMaxSpeed(Vehicle.MaxSpeed * 0.5f);
            else if (Vehicle.VehicleHealth.CurrentHealth < 50)
                Vehicle.SetMaxSpeed(Vehicle.MaxSpeed * 0.6f);
            else if (Vehicle.VehicleHealth.CurrentHealth < 70)
                Vehicle.SetMaxSpeed(Vehicle.MaxSpeed * 0.8f);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag(LayerAndTagManager.Instance.TagDamageCollider) || collision.collider.CompareTag(LayerAndTagManager.Instance.TagPlayer))
                Vehicle.overrideInput = true;

            if (collision.collider.CompareTag(LayerAndTagManager.Instance.TagDamageCollider))
                StartReverse();
        }

        private void OnCollisionExit(Collision collision)
        {
            if (!Vehicle.overrideInput) return;

            if (collision.collider.CompareTag(LayerAndTagManager.Instance.TagDamageCollider) || collision.collider.CompareTag(LayerAndTagManager.Instance.TagPlayer))
                Vehicle.overrideInput = false;
        }
        #endregion
    }
}