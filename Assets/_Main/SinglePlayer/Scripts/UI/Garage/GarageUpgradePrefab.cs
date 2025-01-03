using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    public class GarageUpgradePrefab : MonoBehaviour
    {
        public static event Action OnUpgrade;

        [SerializeField] private Enums.UpgradeType upgradeType;
        [SerializeField] private TextMeshProUGUI levelTMP;
        [SerializeField] private TextMeshProUGUI costTMP;
        [SerializeField] private Image costType;
        [SerializeField] private Slider current;
        [SerializeField] private Slider next;
        [SerializeField] private GameObject info;
        [SerializeField] private Button upgradeBtn;

        [SerializeField] private int currentLevel;
        [SerializeField] private ModelClass.GarageVehicleData.Upgrade upgrade;

        [Header("-----Debug")]
        [SerializeField] private GameObject debugGo;
        [SerializeField] private TextMeshProUGUI debugNextTMP;
        [SerializeField] private TextMeshProUGUI debugCurrentTMP;


        private void OnEnable() => OnUpgrade += SetInteractable;
        private void OnDisable() => OnUpgrade -= SetInteractable;


        private void Start()
        {
            if (GameData.Instance)
                debugGo.SetActive(GameData.Instance.releaseMode == Enums.ReleaseMode.Debug);
        }

        public void SetUpgrade(int _currentLevel, ModelClass.GarageVehicleData.Upgrade _upgrade, bool _disableBtn = false)
        {
            upgrade = _upgrade;
            currentLevel = _currentLevel;
            SetUpgrade(_disableBtn);
        }


        private void SetUpgrade(bool _disable = false)
        {
            if (currentLevel < GarageScreen.Instance.TotalUpgrades)
            {
                levelTMP.text = "Lvl " + (currentLevel + 1);
                costTMP.text = upgrade.upgradeCost.cost.ToString();
            }
            else
            {
                levelTMP.text = "Final Lvl";
                costTMP.text = "-";
            }

            costType.sprite = upgrade.upgradeCost.costType == Enums.CostType.ORARE ? SpriteManager.Instance.orare : SpriteManager.Instance.chips;
            var _currentVehicle = GarageScreen.Instance.CurrentVehicle;


            switch (upgradeType)
            {
                case Enums.UpgradeType.Speed:
                    current.minValue = next.minValue = _currentVehicle.initial.speed;
                    current.maxValue = next.maxValue = _currentVehicle.speedUpgrades[GarageScreen.Instance.TotalUpgrades - 1];

                    current.value = currentLevel == 0 ? _currentVehicle.initial.speed : _currentVehicle.speedUpgrades[currentLevel - 1];
                    next.value = currentLevel == GarageScreen.Instance.TotalUpgrades ? current.value : _currentVehicle.speedUpgrades[currentLevel];
                    break;

                case Enums.UpgradeType.Shield:
                    current.minValue = next.minValue = _currentVehicle.initial.shield;
                    current.maxValue = next.maxValue = _currentVehicle.shieldUpgrades[GarageScreen.Instance.TotalUpgrades - 1];

                    current.value = currentLevel == 0 ? _currentVehicle.initial.shield : _currentVehicle.shieldUpgrades[currentLevel - 1];
                    next.value = currentLevel == GarageScreen.Instance.TotalUpgrades ? current.value : _currentVehicle.shieldUpgrades[currentLevel];
                    break;

                case Enums.UpgradeType.Mileage:
                    current.minValue = next.minValue = _currentVehicle.initial.mileage;
                    current.maxValue = next.maxValue = _currentVehicle.mileageUpgrades[GarageScreen.Instance.TotalUpgrades - 1];

                    current.value = currentLevel == 0 ? _currentVehicle.initial.mileage : _currentVehicle.mileageUpgrades[currentLevel - 1];
                    next.value = currentLevel == GarageScreen.Instance.TotalUpgrades ? current.value : _currentVehicle.mileageUpgrades[currentLevel];
                    break;

                case Enums.UpgradeType.Armour:
                    current.minValue = next.minValue = _currentVehicle.initial.armour;
                    current.maxValue = next.maxValue = _currentVehicle.armourUpgrades[GarageScreen.Instance.TotalUpgrades - 1];

                    current.value = currentLevel == 0 ? _currentVehicle.initial.armour : _currentVehicle.armourUpgrades[currentLevel - 1];
                    next.value = currentLevel == GarageScreen.Instance.TotalUpgrades ? current.value : _currentVehicle.armourUpgrades[currentLevel];
                    break;
            }

            debugCurrentTMP.text = current.value.ToString();
            debugNextTMP.text = next.value.ToString();


            if (_disable) upgradeBtn.interactable = false;
            else OnUpgrade?.Invoke();
        }



        private void SetInteractable()
        {
            if (currentLevel >= GarageScreen.Instance.TotalUpgrades)
            {
                upgradeBtn.interactable = false;
                return;
            }

            upgradeBtn.interactable = (upgrade.upgradeCost.costType == Enums.CostType.ORARE ? GameData.Instance.PlayerData.Data.gameBalance : GameData.Instance.PlayerData.Data.chips) >= upgrade.upgradeCost.cost;
        }

        public void OnClick_Upgrade()
        {
            upgradeBtn.interactable = false;
            GarageScreen.Instance.UpgradePlayerVehicle(upgrade, upgradeType, SetUpgrade);
        }



        public void OnClick_Info()
        {
            info.SetActive(true);
            Invoke("HideInfo", 3);
        }
        private void HideInfo() => info.SetActive(false);

    }

}