using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace FoodFury
{
    [DefaultExecutionOrder(-6)]
    [RequireComponent(typeof(Vehicle))]
    public class TrafficAgentAI : RiderBehaviour, IDriver
    {
        [field: SerializeField] public NavMeshAgent Agent { get; private set; }
        [field: SerializeField] public Vector3 Destination { get; private set; }
        [field: SerializeField] public Vector3 NextPosition { get; private set; }

        [Header("-----Debug")]
        [Range(1f, 10f)] [SerializeField] private float nextTargetRadius = 4;
        [SerializeField] private float distanceToDestination;
        public bool drawGizmo = true;


        private void Start()
        {
            //if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            //print("Sample Position NavMeshHit - " + hit.position);
            //print(gameObject.name + "- ChangeAgentDestination ->  " + Agent.enabled);
        }


        private void OnDrawGizmos()
        {
            if (!drawGizmo) return;
            // Next Position
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(NextPosition, 2);

            // Final Destination
            Gizmos.DrawLine(Destination, Destination + (Vector3.up * 20));
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3.up * 30));
        }


        void Update()
        {
            if (GameData.Instance.IsGamePaused) return;

            UpdateDistination();
            HandleInputs();
            UpdateAgent();
        }


        private void OnEnable()
        {
            Vehicle.VehicleHealth.OnHealthFinished += OnFuelFinished;
            Vehicle.VehicleFuel.OnFuelFinished += OnFuelFinished;
            GameplayScreen.OnGameReady += OnGameReady;
        }

        private void OnDisable()
        {
            Vehicle.VehicleHealth.OnHealthFinished -= OnFuelFinished;
            Vehicle.VehicleFuel.OnFuelFinished -= OnFuelFinished;
            GameplayScreen.OnGameReady -= OnGameReady;
        }

        private void OnGameReady()
        {
            Agent.gameObject.SetActive(true);
            ChangeAgentDestination();
        }

        private void OnFuelFinished()
        {
            accelerateInput = steerInput = 0;
            Vehicle.SetAccerateInput(accelerateInput);
            Vehicle.SetSteerInput(steerInput);
        }



        private void UpdateAgent()
        {
            Agent.transform.localPosition = Vector3.zero;
            Agent.transform.localRotation = Quaternion.identity;
            if (Agent.path.corners.Length > 0)
                NextPosition = Agent.path.corners.Length > 1 ? Agent.path.corners[1] : Agent.path.corners[0];
        }

        #region Inputs
        Vector3 _direction;
        private void HandleInputs()
        {
            if (!overrideInputs)
            {
                accelerateInput = Mathf.Lerp(accelerateInput, distanceToDestination < nextTargetRadius ? -0.5f : 1, Time.deltaTime);

                _direction = NextPosition - transform.position;
                steerInput = Mathf.Lerp(steerInput, Vector3.Dot(transform.right, _direction.normalized), Time.deltaTime * 10);
            }

            Vehicle.SetAccerateInput(accelerateInput);
            Vehicle.SetSteerInput(steerInput);
        }
        #endregion




        #region Destination
        [ContextMenu("ChangeAgentDestination")]
        private void ChangeAgentDestination() => Agent.SetDestination(Destination = TileManager.Instance.GetRandomTile().transform.position);

        private void UpdateDistination()
        {
            distanceToDestination = Vector3.Distance(transform.position, Destination);

            if (distanceToDestination < nextTargetRadius)
            {
                ChangeAgentDestination();
            }
        }
        #endregion



        #region Damage
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