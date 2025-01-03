using System.Collections;
using System.Collections.Generic;
using FoodFury;
using UnityEngine;

namespace OneRare.FoodFury.Multiplayer
{
    public class PlayerAIRivalMP : MonoBehaviour
    {

        public bool doReverse = false;
        public bool isBotActive = false;
        [SerializeField] private Transform raycastCenter;
        [SerializeField] private Tile riderTile;
        [SerializeField] public OrderMP targetOrder;
        [SerializeField] public OrderMP orderToCollect;


        [Header("---Waypoints")]
        [SerializeField] private int pathCount = 100;
        [SerializeField] private float pathDistance = 0;
        [SerializeField] private float distanceToTargetWaypoint;
        [SerializeField] private int targetIndex;
        [SerializeField] private Vector3 targetWaypoint;
        [SerializeField] private List<Vector3> waypointPositions = new List<Vector3>();

        [Header("---Pickups")]
        [SerializeField] private List<Booster> activePickups;


        [Header("---Inputs")]
        [Range(1f, 10f)] [SerializeField] private float nextTargetRadius = 4;
        [Range(0f, 5f)] [SerializeField] private int randomness = 3;
        [Range(1f, 10f)] public float lerpValue = 3;
        [Range(0.0001f, 0.01f)] public float deadZoneValue = 0.001f;

        private PathGenerator pathGenerator = new PathGenerator();

        int recalculatePathCounter = 0;
        // Start is called before the first frame update
        void Start()
        {
            if (!Application.isEditor) pathCount = 50;
        }

        float _waitTime = 0.01f;
        float _remainingTime = 0;
        void Update()
        {
            if (GameManager.IsGamePaused || !isBotActive) return;
            if (targetOrder == null)
            {
                targetOrder = FindObjectOfType<OrderMP>();
            }
            if (targetWaypoint == null || orderToCollect != null) return;

            //HandleInputs();
            if (_remainingTime <= 0)
            {
                UpdateTarget();
                _remainingTime = _waitTime;
            }
            else _remainingTime -= Time.deltaTime;
        }

        #region Inputs

        Vector3 _direction;
        float _stopValue = 2.2f;// Increase to stop quickly
        float _dot;
        [SerializeField] private float accelerateInput;
        [SerializeField] private float steerInput;
        public Vector2 HandleInputs(OrderMP order)
        {
            //targetOrder = order;
            //GeneratePath();
            if (waypointPositions.Count == 0 || targetOrder.Tile == null) return Vector2.zero;

            // Accelerate
            if (doReverse)
                accelerateInput = Mathf.Lerp(accelerateInput, -1f, Time.deltaTime);
            else if (targetIndex < waypointPositions.Count - 1)
                accelerateInput = Mathf.Lerp(accelerateInput, distanceToTargetWaypoint < nextTargetRadius ? -1f : 1f, Time.deltaTime);
            else
                accelerateInput = Mathf.Lerp(accelerateInput, distanceToTargetWaypoint < nextTargetRadius * _stopValue ? 0f : 1f, Time.deltaTime * distanceToTargetWaypoint);


            // Steer
            _direction = targetWaypoint - transform.position;
            _dot = Vector3.Dot(transform.right, _direction.normalized);
            steerInput = Mathf.Lerp(steerInput, doReverse ? -_dot : _dot, Time.deltaTime * lerpValue);
            //steerInput = Mathf.Lerp(steerInput, doReverse ? 0 : GetFinalSteerInput(_direction), Time.deltaTime * lerpValue);
            return new Vector2(accelerateInput, steerInput);
        }
        public Vector2 HandleInputsBotTest()
        {
            //targetOrder = order;
            //GeneratePath();
            if (waypointPositions.Count == 0 || targetOrder == null || targetOrder.Tile == null) return Vector2.zero;

            // Accelerate
            if (doReverse)
                accelerateInput = Mathf.Lerp(accelerateInput, -1f, Time.deltaTime);
            else if (targetIndex < waypointPositions.Count - 1)
                accelerateInput = Mathf.Lerp(accelerateInput, distanceToTargetWaypoint < nextTargetRadius ? -1f : 1f, Time.deltaTime);
            else
                accelerateInput = Mathf.Lerp(accelerateInput, distanceToTargetWaypoint < nextTargetRadius * _stopValue ? 0f : 1f, Time.deltaTime * distanceToTargetWaypoint);


            // Steer
            _direction = targetWaypoint - transform.position;
            _dot = Vector3.Dot(transform.right, _direction.normalized);
            steerInput = Mathf.Clamp(doReverse ? -_dot : _dot, -1, 1);
            //steerInput = Mathf.Lerp(steerInput, doReverse ? 0 : GetFinalSteerInput(_direction), Time.deltaTime * lerpValue);
            return new Vector2(accelerateInput, steerInput);
        }
        #endregion


        RaycastHit _tileHit;
        Vector3 _raycastRightPos => raycastCenter.position + new Vector3(1.5f, 0, 0);
        Vector3 _raycastleftPos => raycastCenter.position + new Vector3(-1.5f, 0, 0);
        [ContextMenu("CurrentRiderTile")]
        public Tile CurrentRiderTile()
        {
            //Debug.DrawRay(raycastCenter.position, -transform.up, Color.red, 2);
            //Debug.DrawRay(_raycastRightPos, -transform.up, Color.red, 2);
            //Debug.DrawRay(_raycastleftPos, -transform.up, Color.red, 2);
            if (Physics.Raycast(raycastCenter.position, -transform.up, out _tileHit, 0.5f, LayerAndTagManager.Instance.LayerRoadTile, QueryTriggerInteraction.Collide))
                riderTile = _tileHit.collider.GetComponent<Tile>();
            else if (Physics.Raycast(_raycastRightPos, -transform.up, out _tileHit, 0.5f, LayerAndTagManager.Instance.LayerRoadTile, QueryTriggerInteraction.Collide))
                riderTile = _tileHit.collider.GetComponent<Tile>();
            else if (Physics.Raycast(_raycastleftPos, -transform.up, out _tileHit, 0.5f, LayerAndTagManager.Instance.LayerRoadTile, QueryTriggerInteraction.Collide))
                riderTile = _tileHit.collider.GetComponent<Tile>();
            else
                riderTile = null;

            return riderTile;
        }
        #region Waypoints
        [ContextMenu("GeneratePath")]
        public void GeneratePath()
        {
            CurrentRiderTile();

            if (CurrentRiderTile() == null || targetOrder == null || targetOrder.Tile == null)
            {
                Debug.Log("Could not generate Rival's path");
                return;
            }
            waypointPositions = pathGenerator.GenerateQuickPathPositions(riderTile, targetOrder.Tile, out pathDistance, pathCount);

            // Add End Tile
            waypointPositions.Add(targetOrder.transform.position);
            targetWaypoint = waypointPositions[targetIndex = 0];
        }

        private void UpdateTarget()
        {
            distanceToTargetWaypoint = Vector3.Distance(transform.position, targetWaypoint);

            if (distanceToTargetWaypoint < nextTargetRadius)
            {
                if (targetIndex != waypointPositions.Count - 1) targetIndex++;
                targetWaypoint = waypointPositions[targetIndex];
            }
        }
        #endregion

        #region Damage
        private void StartReverse()
        {
            recalculatePathCounter++;
            if (recalculatePathCounter > 2)
            {
                GeneratePath();
                recalculatePathCounter = 0;
            }
            doReverse = true;
            Invoke("StopReverse", 4);
        }
        private void StopReverse() => doReverse = false;


        /*private void OnCollisionEnter(Collision collision)
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


        public void TakeDamage(float _damage)
        {
            Vehicle.VehicleHealth.TakeDamage(_damage);
            SetVehicleChanges();
        }

        private void SetVehicleChanges()
        {
            if (Vehicle.VehicleHealth.CurrentHealth < 30)
                Vehicle.SetMaxSpeed(Vehicle.MaxSpeed * 0.7f);
            else if (Vehicle.VehicleHealth.CurrentHealth < 50)
                Vehicle.SetMaxSpeed(Vehicle.MaxSpeed * 0.8f);
            else if (Vehicle.VehicleHealth.CurrentHealth < 70)
                Vehicle.SetMaxSpeed(Vehicle.MaxSpeed * 0.9f);
        }*/

        private void OnDrawGizmosSelected()
        {
            if (waypointPositions.Count == 0) return;

            Gizmos.DrawWireSphere(waypointPositions[0], 2);
            Gizmos.color = Color.yellow;
            for (int i = 0; i < waypointPositions.Count - 1; i++)
            {

                Vector3 _from = waypointPositions[i] + Vector3.up * 0.5f;
                Vector3 _to = waypointPositions[i + 1] + Vector3.up * 0.5f;
                Gizmos.DrawLine(_from, _to);

                Gizmos.DrawWireSphere(_to, 2);
            }
        }
        #endregion
    }
}
