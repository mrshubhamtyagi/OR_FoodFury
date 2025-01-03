using UnityEngine;

namespace FoodFury
{
    public class DrivingTestOrderCampass : MonoBehaviour
    {
        [SerializeField] private Rider rider;
        [SerializeField] private TMPro.TextMeshPro orderDistanceTMP;
        [SerializeField] private Transform orderCampassCanvasParent;
        [SerializeField] private Transform orderCampassPivot;

        public Transform targetOrderTransorm;
        private float orderDistance;
        private float campassHeight = 5;

        private void Start()
        {
            targetOrderTransorm = null;
            gameObject.SetActive(false);
        }

        void Update()
        {
            UpdateDistance();
            UpdateCampass();
        }

        public void SetTarget(Transform _target) => targetOrderTransorm = _target;


        private void UpdateDistance()
        {
            if (targetOrderTransorm == null) return;
            orderDistance = HelperFunctions.GetDistance(rider.transform.position, targetOrderTransorm.position);
        }


        Vector3 _orderDirection;
        private void UpdateCampass()
        {
            if (targetOrderTransorm == null) return;

            orderDistanceTMP.text = $"{Mathf.FloorToInt(orderDistance)}m";

            // Position and rotation
            orderCampassPivot.position = orderCampassCanvasParent.position = rider.transform.position + Vector3.up * campassHeight;

            _orderDirection = targetOrderTransorm.position - rider.transform.position;
            _orderDirection.y = orderCampassPivot.localRotation.y;
            orderCampassPivot.rotation = Quaternion.Slerp(orderCampassPivot.rotation, Quaternion.LookRotation(_orderDirection), Time.deltaTime * 3);
        }
    }
}
