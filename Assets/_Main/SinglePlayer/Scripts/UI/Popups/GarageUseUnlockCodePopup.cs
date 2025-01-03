using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodFury
{
    public class GarageUseUnlockCodePopup : PopupBehaviour
    {
        [SerializeField] private string joinCommUrl;

        void Awake() => Init(GetComponent<CanvasGroup>());


        public override void Init()
        {
            base.Init();
            MenuBar.Instance.Hide();
        }


        public void OnClick_JoinCommunity() => Application.OpenURL(joinCommUrl);

        public void OnClick_UnlockNow()
        {

        }


        public override void Close()
        {
            base.Close();
            MenuBar.Instance.Show();
        }
    }
}
