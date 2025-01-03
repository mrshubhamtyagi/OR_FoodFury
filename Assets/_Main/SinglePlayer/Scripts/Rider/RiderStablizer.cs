using UnityEngine;

namespace FoodFury
{
    [ExecuteInEditMode]
    public class RiderStablizer : MonoBehaviour
    {
        public enum CollisionType { None, Front, Right, Left, FrontRight, FrontLeft, BothSides, All };
        public CollisionType collisionState;
        public Transform raycastObject;


        public float turn;
        public float distance = 10;
        public float angle = 10;


        RiderAI rider;

        void Awake() => rider = GetComponent<RiderAI>();
        void Start() => gameObject.SetActive(Application.isEditor);


        void Update()
        {
            HandlerRaycast();
        }

        Vector3 _direction;
        Vector3 _startOffset;
        private void HandlerRaycast()
        {
            collisionState = CollisionType.None;

            // Front
            _direction = transform.forward;
            _startOffset = Vector3.zero;
            if (Physics.Raycast(raycastObject.position + _startOffset, _direction, out RaycastHit hit, distance))
            {
                collisionState = CollisionType.Front;
            }
            Debug.DrawRay(raycastObject.position + _startOffset, _direction * distance,
                collisionState == CollisionType.Front ? Color.green : Color.red);




            // Right
            _direction = Quaternion.AngleAxis(angle, transform.up) * transform.forward;
            _startOffset = (Vector3.right * 0.1f);
            if (Physics.Raycast(raycastObject.position + _startOffset, _direction, out hit, distance))
            {
                collisionState = collisionState == CollisionType.None ? CollisionType.Right : CollisionType.FrontRight;
            }
            Debug.DrawRay(raycastObject.position + _startOffset, _direction * distance,
                collisionState == CollisionType.Right || collisionState == CollisionType.FrontRight ? Color.green : Color.red);



            // Left
            _direction = Quaternion.AngleAxis(-angle, transform.up) * transform.forward;
            _startOffset = (Vector3.right * 0.1f);
            if (Physics.Raycast(raycastObject.position - _startOffset, _direction, out hit, distance))
            {
                if (collisionState == CollisionType.None) collisionState = CollisionType.Left;
                else if (collisionState == CollisionType.Right) collisionState = CollisionType.BothSides;
                else if (collisionState == CollisionType.Front) collisionState = CollisionType.FrontLeft;
                else collisionState = CollisionType.All;
            }
            Debug.DrawRay(raycastObject.position - _startOffset, _direction * distance,
                            collisionState == CollisionType.Left || collisionState == CollisionType.FrontLeft || collisionState == CollisionType.All ? Color.green : Color.red);



            SetTurning();
        }



        private void SetTurning()
        {
            switch (collisionState)
            {
                case CollisionType.None:
                    turn = 0;
                    break;
                case CollisionType.Front:
                    break;
                case CollisionType.Right:
                    turn = -0.5f;
                    break;
                case CollisionType.Left:
                    turn = 0.5f;
                    break;
                case CollisionType.FrontRight:
                    turn = -1f;
                    break;
                case CollisionType.FrontLeft:
                    turn = 1f;
                    break;
                case CollisionType.BothSides:
                    break;
                case CollisionType.All:
                    break;
                default:
                    break;
            }
        }


    }

}