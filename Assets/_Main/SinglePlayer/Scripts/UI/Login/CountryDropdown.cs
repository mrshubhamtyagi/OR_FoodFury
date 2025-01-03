using Newtonsoft.Json;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
[RequireComponent(typeof(TMP_Dropdown))]
public class CountryDropdown : MonoBehaviour
{
    [SerializeField] private TextAsset countryCodeJsonFile;
    private TMP_Dropdown dropdown;
    private CountryDataArray countryData;
    public CountryData SelectedCountry { get; private set; }

    private void OnEnable()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        string json = countryCodeJsonFile.text;
        countryData = JsonConvert.DeserializeObject<CountryDataArray>(json);
        dropdown.ClearOptions();


        foreach (CountryData country in countryData.countryCodes)
        {

            string displayText = $"{country.name}";
            displayText += " ";
            displayText += $"{country.dial_code}";
            dropdown.options.Add(new TMP_Dropdown.OptionData(displayText));
        }

        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        StartCoroutine(ChangeDropDownOption());
    }
    private IEnumerator ChangeDropDownOption()
    {
        yield return new WaitForEndOfFrame();
        int index = Array.FindIndex(countryData.countryCodes, country => country.name == "India");
        dropdown.onValueChanged?.Invoke(index);
        //Canvas.ForceUpdateCanvases();
    }
    void OnDropdownValueChanged(int index)
    {
        dropdown.captionText.text = countryData.countryCodes[index].dial_code;
        // Get selected country's data
        SelectedCountry = countryData.countryCodes[index];
        string countryCode = SelectedCountry.code;
        string countryName = SelectedCountry.name;
        string dialCode = SelectedCountry.dial_code;

        //  Debug.Log("Selected Country: " + countryName);
        //  Debug.Log("Country Code: " + countryCode);
        // Debug.Log("Dial Code: " + dialCode);

        // You can use countryCode, countryName, and dialCode as needed in your application
    }

}
[System.Serializable]
public class CountryData
{
    public string name;
    public string dial_code;
    public string code;
}
[System.Serializable]
public class CountryDataArray
{
    public CountryData[] countryCodes;
}
