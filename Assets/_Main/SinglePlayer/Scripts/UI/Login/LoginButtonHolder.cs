using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    public class LoginButtonHolder : MonoBehaviour
    {
        [SerializeField] private GameObject usernameAndPasswordLoginPanel;
        [SerializeField] private GameObject socialLoginHolder;
        [SerializeField] private Button userNameAndPasswordButton, appleSignInBtn;

        private void OnEnable()
        {
            userNameAndPasswordButton.gameObject.SetActive(GameData.Instance.Platform == Enums.Platform.iOS ? true : false);
            appleSignInBtn.gameObject.SetActive(GameData.Instance.Platform == Enums.Platform.iOS);
            if (GameData.Instance.Platform == Enums.Platform.iOS)
            {
                userNameAndPasswordButton.onClick.AddListener(OnUserNameAndPasswordButtonClicked);
            }
        }
        private void OnDisable()
        {
            if (GameData.Instance.Platform == Enums.Platform.iOS)
            {
                userNameAndPasswordButton.onClick.RemoveListener(OnUserNameAndPasswordButtonClicked);
            }
        }
        private void OnUserNameAndPasswordButtonClicked()
        {
            socialLoginHolder.SetActive(false);
            usernameAndPasswordLoginPanel.SetActive(true);
        }
    }
}
