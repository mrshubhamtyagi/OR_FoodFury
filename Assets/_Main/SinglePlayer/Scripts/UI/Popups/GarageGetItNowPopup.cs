using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FoodFury
{
    public class GarageGetItNowPopup : PopupBehaviour
    {
        [SerializeField] private string buyNftsUrl;
        [SerializeField] private string buyTokensUrl;

        void Awake() => Init(GetComponent<CanvasGroup>());


        public override void Init()
        {
            base.Init();
            MenuBar.Instance.Hide();
        }

        public void OnClick_BuyNFTs() => Application.OpenURL(buyNftsUrl);


        public void OnClick_BuyTokens() => Application.OpenURL(buyTokensUrl);

        public override void Close()
        {
            base.Close();
            MenuBar.Instance.Show();
        }
    }
}