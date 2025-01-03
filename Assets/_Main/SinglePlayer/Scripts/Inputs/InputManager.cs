using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FoodFury
{
    public class InputManager : MonoBehaviour
    {
        private PlayerInputs playerInputs;

        public static event Action<Vector2, float> OnStartTouch;
        public static event Action<Vector2, float> OnEndTouch;
        public static Action OnShootButtonPressed;

        public static float InputX { get; set; }
        public static float InputY { get; set; }

        public static InputManager Instance { get; private set; }
        private void Awake()
        {
            Instance = this;
            playerInputs = new PlayerInputs();
        }

        private void OnEnable()
        {
            playerInputs.Enable();
            playerInputs.TouchControls.Primary.started += context => StartTouchPrimary(context);
            playerInputs.TouchControls.Primary.canceled += context => EndTouchPrimary(context);
            playerInputs.Movement.Shoot.performed += Shoot_performed;
        }

        private void Shoot_performed(InputAction.CallbackContext obj)
        {
            OnShootButtonPressed?.Invoke();
        }

        private void OnDisable()
        {
            playerInputs.TouchControls.Primary.started -= context => StartTouchPrimary(context);
            playerInputs.TouchControls.Primary.canceled -= context => EndTouchPrimary(context);
            playerInputs.Movement.Shoot.performed -= Shoot_performed;
            playerInputs.Disable();
        }

        private void StartTouchPrimary(InputAction.CallbackContext context) => OnStartTouch?.Invoke(GetPrimaryPosition(), (float)context.startTime);
        private void EndTouchPrimary(InputAction.CallbackContext context) => OnEndTouch?.Invoke(GetPrimaryPosition(), (float)context.time);

        public Vector2 GetPrimaryPosition() => playerInputs.TouchControls.PrimaryPosition.ReadValue<Vector2>();
        public Vector2 GetDeltaPosition() => playerInputs.TouchControls.DeltaPosition.ReadValue<Vector2>();

        //private Vector2 GetJoystickInput() => playerInputs.Movement.Joystick.ReadValue<Vector2>();
        private Vector2 GetMobileInput() => new Vector2(InputX, InputY);

        private Vector2 GetDesktopInput() => playerInputs.Movement.Keyboard.ReadValue<Vector2>();

        public Vector2 GetInputs()
        {

            return GetDesktopInput();
            //if (GameData.Instance == null) return GetMobileInput();
            //else return GameData.Instance.Platform == Enums.Platform.Desktop ? GetDesktopInput() : GetMobileInput();
        }

        //inputType == Enums.InputType.Arrows ? new Vector2(InputX, InputY) : GetJoystickInput();


        public Vector3 ScreenToWorldPosition(Camera cam, Vector3 position)
        {
            position.z = cam.nearClipPlane;
            return cam.ScreenToWorldPoint(position);
        }


        public void DestroyInstance() => Destroy(gameObject);
    }


}
