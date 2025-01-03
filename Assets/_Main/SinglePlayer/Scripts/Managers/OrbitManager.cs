using UnityEngine;
using UnityEngine.InputSystem;

namespace FoodFury
{
    public class OrbitManager : MonoBehaviour
    {
        public static OrbitManager Instance;
        private void Awake() => Instance = this;

        public bool autoRotate = false;
        public float angle;

        private float sensitivity;
        private Vector2 inputPosition;
        private Vector3 inputOffset;
        private Vector3 rotation;

        void Start()
        {
            sensitivity = GameData.Instance.Platform == Enums.Platform.WebGl ? 0.1f : 0.05f;
            rotation = Vector3.zero;
        }

        private void Update()
        {
            HandleInput();

            if (autoRotate)
            {
                transform.Rotate(Vector3.up, angle * Time.deltaTime);
            }
            else
            {
                inputOffset = InputOffset();
                rotation.y = -(inputOffset.x + inputOffset.y) * sensitivity;

                transform.Rotate(rotation);

                inputPosition = GetCurrentInputPosition();
            }
        }

        void HandleInput()
        {
            if (IsPressDown())
            {
                HandlePressDown();
            }
            else if (IsPressUp())
            {
                HandlePressUp();
            }
        }

        bool IsPressDown() => (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame) ||
                   (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame);

        bool IsPressUp() => (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasReleasedThisFrame) ||
                   (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame);

        Vector2 GetCurrentInputPosition()
        {
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
                return Touchscreen.current.primaryTouch.position.ReadValue();
            else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
                return Mouse.current.position.ReadValue();

            return Vector2.zero;
        }

        Vector3 InputOffset()
        {
            Vector2 currentInputPosition = GetCurrentInputPosition();
            return currentInputPosition - inputPosition;
        }

        void HandlePressDown()
        {
            Vector2 currentPosition = GetCurrentInputPosition();
            Ray ray = Camera.main.ScreenPointToRay(currentPosition);
            if (!Physics.Raycast(ray, out RaycastHit hit)) return;
            autoRotate = false;
            inputPosition = currentPosition;

        }

        void HandlePressUp() => autoRotate = true;

        [ContextMenu("ResetRotation")]
        public void ResetRotation()
        {
            rotation = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

    }
}