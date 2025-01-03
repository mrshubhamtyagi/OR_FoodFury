using System;
using TMPro;
using UnityEngine;

namespace FoodFury
{
    public class ConfirmPurchasePopup : PopupBehaviour
    {
        [SerializeField] private TextMeshProUGUI infoTMP;
        [SerializeField] private TextMeshProUGUI costTMP;

        private Action<bool> OnConfirmed;
        public static ConfirmPurchasePopup Instance;
        private void Awake()
        {
            Instance = this;
            Init(GetComponent<CanvasGroup>());
        }

        public void Show(string _info, int _cost, Action<bool> _onConfirmed)
        {
            infoTMP.text = _info;
            costTMP.text = _cost.ToString();
            OnConfirmed = _onConfirmed;
            Show();
        }

        public void OnClick_Confirmed()
        {
            OnConfirmed?.Invoke(true);
            OnConfirmed = null;
            base.Close();
        }



        public override void Close()
        {
            if (MenuBar.Instance) MenuBar.Instance.BlockRaycasts(true);
            OnConfirmed?.Invoke(false);
            OnConfirmed = null;
            base.Close();
        }
    }
}
