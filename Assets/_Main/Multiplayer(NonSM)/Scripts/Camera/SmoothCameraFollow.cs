
using UnityEngine;

namespace OneRare.FoodFury.Multiplayer
{
    public class SmoothCameraFollow : MonoBehaviour
    {
        public Transform player; // The player transform to follow
        public Vector3 offset; // Offset of the camera from the player
        public float smoothSpeed = 0.125f; // Smoothness factor
        private Vector3 velocity = Vector3.zero;

        private void LateUpdate()
        {
            Vector3 desiredPosition = player.position + offset;
            Vector3 smoothedPosition =
                Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}