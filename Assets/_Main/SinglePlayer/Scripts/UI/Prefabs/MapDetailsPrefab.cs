using FoodFury;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapDetailsPrefab : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleTMP;
    [SerializeField] private RawImage thumbnail;

    [Header("-----Stats")]
    [SerializeField] private TextMeshProUGUI levelTMP;
    [SerializeField] private TextMeshProUGUI pointsTMP;
    [SerializeField] private TextMeshProUGUI deliveriesTMP;
    [SerializeField] private TextMeshProUGUI rankTMP;


    void OnEnable()
    {
        // SetStatsDetails(GameData.Instance.PlayerData.Level, GameData.Instance.PlayerData.Points, GameData.Instance.PlayerData.Deliveries, GameData.Instance.PlayerData.Rank);
    }

    public void SetMapDetails(Texture2D _texture, string _mapTitle)
    {
        titleTMP.text = _mapTitle;
        thumbnail.texture = _texture;
    }

    public void SetStatsDetails(int _level, int _points, int _deliveries, int _rank)
    {
        levelTMP.text = "Level: " + HelperFunctions.FormatNumber(_level);
        pointsTMP.text = "Points: " + HelperFunctions.FormatNumber(_points);
        deliveriesTMP.text = "Deliveries: " + HelperFunctions.FormatNumber(_deliveries);
        rankTMP.text = "Rank: " + HelperFunctions.FormatNumber(_rank);
    }

    //public void OnClick_Play() => LevelScreen.Instance.OnClick_Play();

}
