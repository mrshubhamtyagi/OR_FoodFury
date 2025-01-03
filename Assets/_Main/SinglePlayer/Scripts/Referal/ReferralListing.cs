using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace FoodFury
{
    public class ReferralListing : MonoBehaviour
    {
        private ModelClass.ReferralDataResponse.FinalTask task;
        [SerializeField] private TMP_Text inviteFriendsText, chipText;
        [SerializeField] private Button claimBtn;
        [SerializeField] private Image progressImg;
        [SerializeField] private Sprite claimButtonActiveImg;
        [SerializeField] private Sprite claimButtonInActiveImg;
        [SerializeField] private GameObject stars;


        public void Init(ModelClass.ReferralDataResponse.FinalTask _task, int totalReferals)
        {
            task = _task;
            chipText.text = "x" + task.reward.ToString();
            inviteFriendsText.text = task.text;
            stars.SetActive(false);
            if (task.isClaimed)
            {
                claimBtn.gameObject.SetActive(false);
                progressImg.fillAmount = 1;
            }
            else
            {
                claimBtn.gameObject.SetActive(true);
                claimBtn.interactable = false;
                progressImg.fillAmount = totalReferals / (float)task.inviteLimit;
                Debug.Log($"Fill Amount {totalReferals / (float)task.inviteLimit} task {task.taskNumber} invite limit {task.inviteLimit}");
                claimBtn.interactable = ((progressImg.fillAmount >= 1) && task.isClaimed == false) ? true : false;
                stars.SetActive(claimBtn.interactable);
                claimBtn.image.sprite = claimBtn.interactable ? claimButtonActiveImg : claimButtonInActiveImg;
            }
        }


        public void OnClick_ClaimBtn()
        {
            ClaimReferal();
        }
        public async void ClaimReferal()
        {
            Loader.Instance.ShowLoader();
            var _result = await ReferalAPIs.ClaimReferal(task.taskNumber);
            if (_result.error)
            {
                Loader.Instance.HideLoader();
                PopUpManager.Instance.ShowWarningPopup("ERROR");
            }
            else
            {
                PopUpManager.Instance.ShowPopup(PopUpManager.Instance.RewardClaimedPopup);
                PopUpManager.Instance.RewardClaimedPopup.Init(task.reward);
                GameData.Instance.PlayerData.Data.chips += task.reward;
                GameData.Invoke_OnPlayerDataUpdate();
                Loader.Instance.HideLoader();
            }
        }
    }
}
