using UnityEngine;

namespace FoodFury
{
    public class SpawnPositions : MonoBehaviour
    {
        [Header("-----Player & Rival")]
        public PositionAndRotation[] Player;

        [Header("-----Corners")]
        public PositionAndRotation[] Corner;

        [Header("-----Mascot")]
        public PositionAndRotation[] Mascot;

        [Header("-----Gizmo")]
        public Color riderColor;
        public Color mascotColor;
        public float length;

        private void OnDrawGizmos()
        {
            Gizmos.color = riderColor;
            foreach (var item in Player) Gizmos.DrawLine(item.position, item.position + (Vector3.up * length));
        }
    }

    [System.Serializable]
    public class PositionAndRotation
    {
        public Vector3 position;
        public Vector3 rotation;
    }
}