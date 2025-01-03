using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    public class UIPlayerControls : MonoBehaviour
    {
        [SerializeField] private GameObject arrowPrefab;
        [SerializeField] private GameObject arrows;
        //[SerializeField] private FixedJoystick fixedJoystickLeft, fixedJoystickRight, fixedJoystick;
        //[SerializeField] private GameObject arrowObj, joystickObj;
        //[SerializeField] private PointerButton forward, reverse, right, left; //, breaks, handle, reverseBtn;
        //[SerializeField] private TMPro.TextMeshProUGUI debuggerText;
        //public bool useMain;


        [Header("-----Weapon")]
        [SerializeField] private RectTransform weaponObj;
        [SerializeField] private Vector2 weaponObjMobilePos;
        [SerializeField] private Vector2 weaponObjDesktopPos;

        private bool controlsSpawned;

        private void Awake()
        {
            //forward.OnPressed += OnPressedForward;
            //reverse.OnPressed += OnPressedReverse;
            //right.OnPressed += OnPressedRight;
            //left.OnPressed += OnPressedLeft;

            //forward.OnReleased += OnReleasedForwardReverse;
            //reverse.OnReleased += OnReleasedForwardReverse;
            //right.OnReleased += OnReleasedLeftRight;
            //left.OnReleased += OnReleasedLeftRight;
        }

        //private void OnPressedForward()
        //{
        //    InputManager.InputY = 1;
        //    debuggerText.text += "OnPressed -- Forward\n";
        //}
        //private void OnPressedReverse()
        //{
        //    InputManager.InputY = -1;
        //    debuggerText.text += "OnPressed -- Reverse\n";
        //}
        //private void OnReleasedForwardReverse()
        //{
        //    InputManager.InputY = 0;
        //    debuggerText.text += "OnReleased -- ForwardReverse\n";
        //}


        //private void OnPressedLeft()
        //{
        //    InputManager.InputX = -1;
        //    debuggerText.text += "OnPressed -- Left\n";
        //}
        //private void OnPressedRight()
        //{
        //    InputManager.InputX = 1;
        //    debuggerText.text += "OnPressed -- Right\n";
        //}
        //private void OnReleasedLeftRight()
        //{
        //    InputManager.InputX = 0;
        //    debuggerText.text += "OnReleased -- LeftRight\n";
        //}




        //private void Update()
        //{
        //    if (GameData.Instance == null || InputManager.Instance == null) return;
        //    if (GameData.Instance.Platform == Enums.Platform.Desktop || GameData.Instance.Controls == Enums.InputType.Arrows) return;


        //    InputManager.InputX = useMain ? fixedJoystick.Horizontal : fixedJoystickRight.Horizontal;
        //    InputManager.InputY = useMain ? fixedJoystick.Vertical : fixedJoystickLeft.Vertical;
        //}

        public void SwitchControl(Enums.InputType _type)
        {
            //arrowObj.SetActive(_type == Enums.InputType.Arrows);
            //joystickObj.SetActive(_type == Enums.InputType.Joystick);
        }


        public void SwitchInput(Enums.Platform _platform)
        {
            if (weaponObj)
                weaponObj.anchoredPosition = _platform == Enums.Platform.WebGl ? weaponObjDesktopPos : weaponObjMobilePos;

            if (arrows) arrows.gameObject.SetActive(!(_platform == Enums.Platform.WebGl));

            //if (_platform != Enums.Platform.Desktop && !controlsSpawned)
            //{
            //    controlsSpawned = true;
            //    Instantiate(arrowPrefab, transform);
            //}

        }


    }

}