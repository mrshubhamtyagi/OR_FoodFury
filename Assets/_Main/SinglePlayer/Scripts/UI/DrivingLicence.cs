using System;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    public class DrivingLicence : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI errorTMP;

        [SerializeField] private RawImage pic;
        [SerializeField] private TextMeshProUGUI nameTMP, countryTMP, dobTMP, genderTMP, favFoodTMP;
        [SerializeField] private TextMeshProUGUI dateTMP;
        [SerializeField] private TextMeshProUGUI referralCodeTMP;

        [Header("-----Inputs")]
        [SerializeField] private InputField nameInput;
        [SerializeField] private InputField countryInput, faveFoodInput;
        [SerializeField] private InputField dInput, mInput, yInput;
        [SerializeField] private Image[] genders;

        public int _genderIndex = 0;
        public bool setInput = false;

        // [DllImport("__Internal")] private static extern void Unity_CopyToClipboard(string _code);

        private void Start()
        {
            if (errorTMP) errorTMP.gameObject.SetActive(false);
            SetProfilePic(GameData.ProfilePic);

            if (setInput)
                SetInputDetails(GameData.Instance.PlayerData.Data.driverDetails, "");
        }

        public void OnClick_Copy()
        {

        }



        private void SetInputDetails(ModelClass.PlayerData.DriverDetails _data, string _referralCode)
        {
            if (!string.IsNullOrWhiteSpace(_data.name)) nameInput.text = _data.name;
            if (!string.IsNullOrWhiteSpace(_data.country)) countryInput.text = _data.country;
            if (!string.IsNullOrWhiteSpace(_data.favFood)) faveFoodInput.text = _data.favFood;
            if (!string.IsNullOrWhiteSpace(_data.gender))
            {
                if (_data.gender.ToLower().Equals("m")) OnClick_Gender(1);
                else if (_data.gender.ToLower().Equals("f")) OnClick_Gender(2);
                else OnClick_Gender(0);
            }

            if (!string.IsNullOrWhiteSpace(_data.dob))
            {
                string[] _dob = _data.dob.Split('.');
                dInput.text = _dob[0];
                mInput.text = _dob[1];
                yInput.text = _dob[2];
            }
        }


        public void SetDetails(ModelClass.PlayerData.DriverDetails _data, string _referralCode)
        {
            if (!string.IsNullOrWhiteSpace(_data.name)) nameTMP.text = _data.name;
            if (!string.IsNullOrWhiteSpace(_data.country)) countryTMP.text = _data.country;
            if (!string.IsNullOrWhiteSpace(_data.gender)) genderTMP.text = _data.gender;
            if (!string.IsNullOrWhiteSpace(_data.favFood)) favFoodTMP.text = _data.favFood;

            if (!string.IsNullOrWhiteSpace(_data.dob))
            {
                string[] _dob = _data.dob.Split('.');
                dobTMP.text = $"{_dob[0]}   {_dob[1]}   {_dob[2]}";
            }

            referralCodeTMP.text = _referralCode;
        }

        public void SetProfilePic(Texture2D _texture) => pic.texture = _texture;



        public void OnClick_Gender(int _index)
        {
            _genderIndex = _index;
            for (int i = 0; i < genders.Length; i++)
                genders[i].color = i == _index ? ColorManager.Instance.Yellow : ColorManager.Instance.DarkGrey;
        }




        public async void OnClick_LetsBegin()
        {
            if (!ValidateDate())
            {
                errorTMP.text = "Please check your Date!";
                errorTMP.gameObject.SetActive(true);
                return;
            }

            errorTMP.gameObject.SetActive(false);

            GameData.Instance.PlayerData.Data.driverDetails.name = nameInput.text.Trim();
            GameData.Instance.PlayerData.Data.driverDetails.country = countryInput.text.Trim();
            GameData.Instance.PlayerData.Data.driverDetails.favFood = faveFoodInput.text.Trim();
            GameData.Instance.PlayerData.Data.driverDetails.gender = _genderIndex == 1 ? "M" : _genderIndex == 2 ? "F" : "X";
            if (!string.IsNullOrWhiteSpace(dInput.text) && !string.IsNullOrWhiteSpace(mInput.text) && !string.IsNullOrWhiteSpace(yInput.text))
                GameData.Instance.PlayerData.Data.driverDetails.dob = $"{dInput.text}.{mInput.text}.{yInput.text}";


            var _response = await OtherAPIs.SaveDriverDetails_API();
            if (!_response) Debug.Log("Could not save driver details");
            else GameData.Instance.PlayerData.Data.username = nameInput.text.Trim();

            TutorialScreen.Instance.OnClick_TapToContinue();
        }


        private bool ValidateDate()
        {
            if (int.TryParse(dInput.text, out int _day))
            {
                if (_day <= 0 || _day > 31) return false;
            }

            if (int.TryParse(mInput.text, out int _month))
            {
                if (_month <= 0 || _month > 12) return false;
            }

            return true;
        }
    }

}