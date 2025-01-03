using UnityEngine;

namespace FoodFury
{
    public class OverlayWarningPopup : PopupBehaviour
    {
        [SerializeField] public TMPro.TextMeshProUGUI warningTMP;


        public static OverlayWarningPopup Instance;
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                Init(GetComponent<CanvasGroup>());
            }
        }

        public void ShowWarning(string _warning, int _timeout = 3)
        {
            print($"[WARNING] {_warning}");
            warningTMP.text = _warning;
            Show();
            Invoke("HideDelay", _timeout);
        }


        public void HideDelay() => Hide();
    }

}