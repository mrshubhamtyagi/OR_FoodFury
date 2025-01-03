using UnityEngine;

namespace FoodFury
{
    public class FeedbackPopup : PopupBehaviour
    {
        [SerializeField] private string feedbackUrl = "https://forms.gle/BL74rFZWKugwh3X9A";
        [SerializeField] private string telegramLink = "https://t.me/onerarenft";
        [SerializeField] private string discordLink = "https://discord.gg/Uv6XBynzXp";

        void Awake() => Init(GetComponent<CanvasGroup>());

        public override void Init()
        {
            base.Init();
            MenuBar.Instance.BlockRaycasts(false);
        }


        public override void Close()
        {
            MenuBar.Instance.BlockRaycasts(true);
            base.Close();
        }


        public void OnClick_FeedbackForm() => Application.OpenURL(feedbackUrl);
        public void OnClick_Telegram() => Application.OpenURL(telegramLink);
        public void OnClick_Discord() => Application.OpenURL(discordLink);
    }

}
