using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    public class DummyLogin : MonoBehaviour
    {
        public TMP_InputField emailInput;
        public TMP_InputField passInput;
        public Button signInBtn;

        [SerializeField] private string dummyEmail;
        [SerializeField] private string dummyPass;


        private void Awake() => signInBtn.onClick.AddListener(OnSubmit);

        private void OnSubmit()
        {
            string _email = emailInput.text;
            string _pass = passInput.text;

            if (string.IsNullOrEmpty(_email) || !_email.Trim().Equals(dummyEmail))
            {
                OverlayWarningPopup.Instance.ShowWarning("Invalid Inputs!");
                return;
            }

            if (string.IsNullOrEmpty(_pass) || !_pass.Trim().Equals(dummyPass))
            {
                OverlayWarningPopup.Instance.ShowWarning("Invalid Inputs!");
                return;
            }


            LoginManager.Instance.UsernameAndPasswordAppleLogin(_email);
            signInBtn.interactable = false;
        }



    }
}
