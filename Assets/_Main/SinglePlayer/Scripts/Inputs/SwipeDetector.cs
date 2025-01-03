using FoodFury;
using System;
using UnityEngine;

namespace CustomSwipeDetector
{
    public class SwipeDetector : MonoBehaviour
    {
        [SerializeField] private Enums.SwipeDirection direction = Enums.SwipeDirection.None;
        [SerializeField] private float distance = 0.1f; // in world space
        [SerializeField] private float duration = 1; // sec
        [SerializeField][Range(0f, 1f)] private float directionThreshold = 0.9f; // sec


        [Header("---Debug---")]
        public float debugSwipeDistance;
        public float dotValue;
        public float angle;
        public Vector2 swipePos;


        private Vector2 startPosition;
        private float startTime;

        private Vector2 endPosition;
        private float endTime;

        public static event Action<Enums.SwipeDirection> OnSwipeDetected;

        private Camera cam;

        public static SwipeDetector instance { get; private set; }
        private void Awake()
        {
            instance = this;
            cam = Camera.main;
        }


        void OnEnable()
        {
            InputManager.OnStartTouch += OnSwipeStart;
            InputManager.OnEndTouch += OnSwipeEnd;
        }

        void OnDisable()
        {
            InputManager.OnStartTouch -= OnSwipeStart;
            InputManager.OnEndTouch -= OnSwipeEnd;
        }

        private void OnSwipeStart(Vector2 _position, float _time)
        {
            endPosition = Vector2.zero;
            startPosition = _position;
            startTime = _time;
        }

        private void OnSwipeEnd(Vector2 _position, float _time)
        {

            endPosition = _position;
            endTime = _time;
            DetectSwipe();
        }


        private void DetectSwipe()
        {
            debugSwipeDistance = Vector3.Distance(endPosition, startPosition);
            swipePos = (endPosition - startPosition).normalized;
            swipePos = new Vector2(float.Parse(swipePos.x.ToString("f1")), float.Parse(swipePos.y.ToString("f1")));
            angle = Vector3.Angle(swipePos, startPosition);
            if (Vector3.Distance(startPosition, endPosition) > distance && (endTime - startTime) < duration)
            {
                Debug.DrawLine(startPosition, endPosition, Color.yellow, 3f);
                DetectDirection((endPosition - startPosition).normalized);
            }
            else
            {
                direction = Enums.SwipeDirection.None;
                OnSwipeDetected?.Invoke(direction);
            }
        }


        private void DetectDirection(Vector2 _direction)
        {
            //print($"DetectDirectionPosition -> {_direction}");
            if (Vector2.Dot(Vector2.up, _direction) > directionThreshold) direction = Enums.SwipeDirection.Up;
            else if (Vector2.Dot(Vector2.down, _direction) > directionThreshold) direction = Enums.SwipeDirection.Down;
            else if (Vector2.Dot(Vector2.left, _direction) > directionThreshold) direction = Enums.SwipeDirection.Left;
            else if (Vector2.Dot(Vector2.right, _direction) > directionThreshold) direction = Enums.SwipeDirection.Right;
            else direction = Enums.SwipeDirection.None;

            OnSwipeDetected?.Invoke(direction);
        }
    }
}