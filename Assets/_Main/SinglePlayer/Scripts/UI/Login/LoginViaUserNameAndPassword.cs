using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    public class LoginViaUserNameAndPassword : MonoBehaviour
    {
        [SerializeField] private TMP_InputField emailInput;
        [SerializeField] private TMP_InputField passInput;
        [SerializeField] private Button signInBtn;
        [SerializeField] private Button backBtn;
        [SerializeField] private GameObject socialLoginHolder;
        [SerializeField] private string dummyEmail;
        [SerializeField] private string dummyPass;


        private void OnEnable()
        {
            signInBtn.onClick.AddListener(OnSubmit);
            backBtn.onClick.AddListener(OnBackButtonClicked);
        }

        private void OnBackButtonClicked()
        {
            socialLoginHolder.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }

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
            signInBtn.interactable = backBtn.interactable = false;
        }

    }
}
