using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    public class FuelMeter : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        [SerializeField] private TextMeshProUGUI timeTMP;

        private void OnEnable() => UpdateMeterData();

        public void UpdateMeterData()
        {
            TimeSpan timespan = TimeSpan.FromSeconds(GameData.Instance.PlayerData.Data.fuel);

            if (timeTMP != null) timeTMP.text = $"{(int)timespan.TotalMinutes} : {timespan.Seconds}";
            if (fillImage) fillImage.fillAmount = GameData.Instance.PlayerData.Data.fuel / (float)(GameData.Instance.TankCapacityInSeconds);
        }

    }
}