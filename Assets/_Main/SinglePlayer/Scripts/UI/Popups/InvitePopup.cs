using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

namespace FoodFury
{
    public class InvitePopup : PopupBehaviour
    {
        [SerializeField] private TextMeshProUGUI codeTMP;
        [SerializeField] private TextMeshProUGUI redeemedTMP;


        // [DllImport("__Internal")] private static extern void Unity_CopyToClipboard(string _code);


        void Awake() => Init(GetComponent<CanvasGroup>());

        public void OnClick_Copy()
        {

        }


        public override void Init()
        {
            base.Init();
            MenuBar.Instance.BlockRaycasts(false);
            //codeTMP.text = GameData.Instance.PlayerData.referralCode.code;
            //redeemedTMP.text = $"{GameData.Instance.PlayerData.referralCode.count}/3 Redeemed";
        }


        public override void Close()
        {
            MenuBar.Instance.BlockRaycasts(true);
            base.Close();
        }
    }

}
