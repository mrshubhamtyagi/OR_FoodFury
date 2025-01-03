using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FoodFury
{
    public class RewardClaimedPopup : PopupBehaviour
    {
        [SerializeField] private TextMeshProUGUI rewardText;
        void Awake()
        {
            Init(GetComponent<CanvasGroup>());
        }
        public void Init(int rewardAmount)
        {
            Init();
            rewardText.text = rewardAmount.ToString();
        }
        public override void Init()
        {
            base.Init();
        }
        public override void Close()
        {
            ReferalPopup.instance.Init();
            MenuBar.Instance.BlockRaycasts(true);
            base.Close();
        }
    }
}
