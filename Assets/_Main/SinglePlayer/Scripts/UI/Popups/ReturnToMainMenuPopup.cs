using System;
using UnityEngine;

namespace FoodFury
{
    public class ReturnToMainMenuPopup : PopupBehaviour
    {
        [SerializeField] private bool showMeuOnHide;


        private Action callback;

        void Awake() => Init(GetComponent<CanvasGroup>());

        public override void Init()
        {
            base.Init();
            MenuBar.Instance.Hide();
        }

        public void ShowPopup(bool _showMenuOnHide, Action _callback)
        {
            //if (ScreenManager.Instance.CurrentScreen == Enums.Screen.Garage)
            //    GarageScreen.Instance.SetGlassObject(false);

            callback = _callback;
            MenuBar.Instance.Hide();
            showMeuOnHide = _showMenuOnHide;
            Show();
        }


        public void OnClick_Yes()
        {
            callback?.Invoke();
            callback = null;
        }

        public void OnClick_No()
        {
            //if (ScreenManager.Instance.CurrentScreen == Enums.Screen.Garage)
            //    GarageScreen.Instance.SetGlassObject(true);

            if (showMeuOnHide) MenuBar.Instance.Show();
            showMeuOnHide = false;
            Hide();
        }



    }

}