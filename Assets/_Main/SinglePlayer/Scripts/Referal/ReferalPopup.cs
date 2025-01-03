using TMPro;
using UnityEngine;

namespace FoodFury
{
    public class ReferalPopup : PopupBehaviour
    {
        [SerializeField] private Transform referralListingParent;
        [SerializeField] private ReferralListing claimListing;
        [SerializeField]
        private TMP_Text referralLinkText, referralPointsText, successfullRefText;
        public static ReferalPopup instance;

        public void OnClick_SendReferralLinkBtn()
        {
#if UNITY_WEBGL
            Debug.Log($"Called TG Url unity side :{GameData.Instance.ReferalUrl}");
            JavaScriptCallbacks.SendTG_URL(GameData.Instance.ReferalUrl);
#endif
        }

        public void OnClick_CopyReferralLinkBtn()
        {
#if UNITY_WEBGL
            Debug.Log($"copy clipboard unity side:{GameData.Instance.ReferalUrl}");
            JavaScriptCallbacks.CallReactClipboard(GameData.Instance.ReferalUrl);
#endif
        }

        void Awake()
        {
            instance = this;
            Init(GetComponent<CanvasGroup>());
        }
        [ContextMenu("Init")]
        public override void Init()
        {
            base.Init();
            MenuBar.Instance.BlockRaycasts(false);
            GetReferalData();
        }

        public void ShowListing(ModelClass.ReferralDataResponse.Result result)
        {
            DestroyAllListings();

            referralPointsText.text = result.DATA.referralPoints.ToString();
            successfullRefText.text = result.DATA.totalReferrals.ToString();
            referralLinkText.text = GameData.Instance.ReferalUrl;

            foreach (var item in result.finalTasks)
            {
                var listing = Instantiate(claimListing, referralListingParent);
                listing.Init(item, result.DATA.totalReferrals);
            }
        }
        public override void Close()
        {
            MenuBar.Instance.BlockRaycasts(true);

            base.Close();
        }
        public async void GetReferalData()
        {
            var _result = await ReferalAPIs.GetReferalData();
            if (_result.error)
            {
                PopUpManager.Instance.ShowWarningPopup("ERROR");
            }
            else
            {
                Debug.Log("Total Tasks:" + _result.result.finalTasks.Count);
                ShowListing(_result.result);
            }
        }


        public void DestroyAllListings()
        {
            foreach (Transform child in referralListingParent)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
