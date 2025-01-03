using System.Collections;
using TMPro;
using UnityEngine;

public class UpdateItemLabel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI startingText;
    [SerializeField] private TextMeshProUGUI numberText;

    private void OnEnable() => StartCoroutine(UpdateText());

    public IEnumerator UpdateText()
    {
        yield return new WaitForEndOfFrame();
        int firtPostition = startingText.text.TrimEnd().LastIndexOf(' ');
        Debug.Log(firtPostition);
        string country = startingText.text.Substring(0, firtPostition).Trim();
        string countryCode = startingText.text.Substring(firtPostition).Trim();

        numberText.text = startingText.text.Substring(firtPostition);
        startingText.text = startingText.text.Substring(0, firtPostition);
    }
}
