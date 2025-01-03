using TMPro;
using UnityEngine;

public class OTPInputHandler : MonoBehaviour
{
    [SerializeField] private TMP_InputField OtpInputField;

    public bool IsInputValid() => OtpInputField.text.Length == 6;

    public string GetCodeFromInput() => OtpInputField.text;

}
