using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodFury
{
    [RequireComponent(typeof(RiderBehaviour))]
    public class RivalRaycaster : MonoBehaviour
    {
        public Transform raycastObject;
        [Range(-1f, 1f)] public float accelerateInput = 0;
        [Range(-1f, 1f)] public float steerInput = 0;


        private RiderAIRival Rider;



        private void Awake()
        {
            Rider = (RiderAIRival)GetComponent<RiderBehaviour>();
        }

        void Start()
        {
            //Rider.OverrideInputs = true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Vector3 _front = (transform.forward * 10);
            Gizmos.DrawLine(raycastObject.position, raycastObject.position + _front);

            Vector3 _right = transform.forward * 5 + transform.right * 3;
            Gizmos.DrawLine(raycastObject.position, raycastObject.position + _right);

            Vector3 _left = transform.forward * 5 + -transform.right * 3;
            Gizmos.DrawLine(raycastObject.position, raycastObject.position + _left);
        }

        void Update()
        {
            HandlerRaycast();
            SetInputs();
        }


        private void HandlerRaycast()
        {
            Vector3 _frontDirection = raycastObject.position - transform.forward;
            if (Physics.Raycast(raycastObject.position, _frontDirection.normalized, out RaycastHit hit, 10))
            {
                if (hit.collider != null)
                {

                }
            }
        }



        private void SetInputs() => Rider.SetInputs(accelerateInput, steerInput);
    }

}