using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VerifyAndResendScreen : MonoBehaviour
{
    [SerializeField] private OTPInputHandler inputHandler;
    [SerializeField] private AudioButton verifyOtpButton;
    [SerializeField] private AudioButton resendCodeButton;
    [SerializeField] private AudioButton editNumberButton;
    [SerializeField] private GameObject error;
    public static Action<string> OnVerifyOTPButtonClicked;
    public static Action OnResendCodeButtonClicked, OnEditNumberButtonClicked;
    [SerializeField] private TextMeshProUGUI dialCode, mobileNumber;
    private void OnEnable()
    {
        verifyOtpButton.onClick.AddListener(OnVerifyButtonClicked);
        resendCodeButton.onClick.AddListener(ResendCodeClicked);
        editNumberButton.onClick.AddListener(EditNumberButtonClicked);
    }

    private void EditNumberButtonClicked() => OnEditNumberButtonClicked?.Invoke();
    private void OnDisable()
    {
        verifyOtpButton.onClick.RemoveListener(OnVerifyButtonClicked);
        resendCodeButton.onClick.RemoveListener(ResendCodeClicked);
        editNumberButton.onClick.RemoveListener(EditNumberButtonClicked);

    }
    private void OnVerifyButtonClicked()
    {
        if (inputHandler.IsInputValid() == false)
        {
            error.SetActive(true);
        }
        OnVerifyOTPButtonClicked?.Invoke(inputHandler.GetCodeFromInput());
    }
    private void ResendCodeClicked() => OnResendCodeButtonClicked?.Invoke();
    public void SetDialCode(string dialCode) => this.dialCode.text = dialCode;
    public void SetMobileNumber(string mobileNumber) => this.mobileNumber.text = mobileNumber;
}
