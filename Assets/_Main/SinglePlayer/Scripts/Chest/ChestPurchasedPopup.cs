using System;
using UnityEngine;

namespace FoodFury
{
    public class ChestPurchasedPopup : PopupBehaviour
    {
        private void Awake()
        {
            Init(GetComponent<CanvasGroup>());
        }
        public override void Init()
        {
            base.Init();
            if (MenuBar.Instance) MenuBar.Instance.BlockRaycasts(false);


        }
        public override void Close()
        {

            if (MenuBar.Instance) MenuBar.Instance.BlockRaycasts(true);
            base.Close();
        }
    }
}
