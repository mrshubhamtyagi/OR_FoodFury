using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    public class FuelPopup : PopupBehaviour
    {
        [Header("----Buttons")]
        [SerializeField] private Button tankUpgradeButton;
        [SerializeField] private Button restoreUpgradeButton;

        [Header("----Images")]
        [SerializeField] private List<ImagePair> tankUpgradeImagePair;
        [SerializeField] private List<ImagePair> restoreUpgradeImagePair;

        [Header("---- Tank Fill Image")]
        [SerializeField] private Image tankFillImage;

        [Header("-------Texts")]
        [SerializeField] private TextMeshProUGUI remaningingFuelText;

        [SerializeField] private TextMeshProUGUI tankCapacityText;
        [SerializeField] private TextMeshProUGUI tankUpgradeCostText;
        [SerializeField] private TextMeshProUGUI tankUpgradeValueText;

        [SerializeField] private TextMeshProUGUI tankRestoreCostText;
        [SerializeField] private TextMeshProUGUI tankRestoreValueText;
        [SerializeField] private TextMeshProUGUI infoText;

        public void UpdateImageOnButton(List<ImagePair> imagePairs, bool interactable)
        {
            foreach (var pair in imagePairs)
                pair.imageToTarget.sprite = interactable ? pair.interactableVersion : pair.disabledVersion;
        }

        void Awake() => Init(GetComponent<CanvasGroup>());


        private void OnEnable() => GameData.OnPlayerDataUpdate += UpdateFuelRelatedVisual;
        private void OnDisable() => GameData.OnPlayerDataUpdate -= UpdateFuelRelatedVisual;

        private void UpdateFuelRelatedVisual()
        {
            TimeSpan timespan = TimeSpan.FromSeconds(GameData.Instance.PlayerData.Data.fuel);
            remaningingFuelText.text = $"{(int)timespan.TotalMinutes}m {timespan.Seconds}s";

            UpdateUiRelatedToTankRestore();
            UpdateUiRelatedToTankCapacity();
        }

        private void UpdateUiRelatedToTankRestore()
        {
            UpdateButtonState(restoreUpgradeButton, restoreUpgradeImagePair, GameData.Instance.PlayerData.Data.fuelRestoreLevel, GameData.Instance.FuelData.TotalRestoreUpgrades());
            int restoreValue = GameData.Instance.PlayerData.Data.fuelRestoreLevel > 0 ? GameData.Instance.GetFuelTankRestore(GameData.Instance.PlayerData.Data.fuelRestoreLevel).value : GameData.Instance.FuelData.initialValues.restore;
            infoText.text = $"{restoreValue} seconds of fuel will be refilled every {GameData.Instance.FuelRestoreIntervalInMinute} minutes!";
            if (GameData.Instance.PlayerData.Data.fuelRestoreLevel + 1 > GameData.Instance.FuelData.TotalRestoreUpgrades())
            {
                return;
            }
            FuelDataSO.FuelUpgrade tankRestore = GameData.Instance.GetFuelTankRestore(GameData.Instance.PlayerData.Data.fuelRestoreLevel + 1);
            tankRestoreCostText.text = tankRestore.cost.ToString();
            tankRestoreValueText.text = $"+{tankRestore.value} secs \nper 5 min";
        }

        private void UpdateUiRelatedToTankCapacity()
        {
            UpdateButtonState(tankUpgradeButton, tankUpgradeImagePair, GameData.Instance.PlayerData.Data.fuelTankLevel, GameData.Instance.FuelData.TotalTankUpgrades());
            tankFillImage.fillAmount = GameData.Instance.PlayerData.Data.fuel / (float)(GameData.Instance.TankCapacityInSeconds);
            //Debug.Log($"{tankFillImage.fillAmount} - {GameData.Instance.PlayerData.fuel} - {GameData.Instance.TankCapacityInSeconds}");
            tankCapacityText.text = $"{GameData.Instance.TankCapacityInSeconds / 60} m";
            if (GameData.Instance.PlayerData.Data.fuelTankLevel + 1 > GameData.Instance.FuelData.TotalTankUpgrades())
            {
                return;
            }
            FuelDataSO.FuelUpgrade tankUpgrade = GameData.Instance.GetTankUpgrade(GameData.Instance.PlayerData.Data.fuelTankLevel + 1);
            tankUpgradeCostText.text = tankUpgrade.cost.ToString();
            tankUpgradeValueText.text = $"+{tankUpgrade.value}mins";
        }


        public override void Init()
        {
            base.Init();
            if (MenuBar.Instance) MenuBar.Instance.BlockRaycasts(false);
            UpdateUiRelatedToTankCapacity();
            UpdateUiRelatedToTankRestore();
            UpdateFuelRelatedVisual();

        }
        private void UpdateButtonState(Button button, List<ImagePair> _imagePairs, int currentLevel, int totalUpgrades)
        {
            //Debug.Log($"{currentLevel} total {totalUpgrades}");
            if (currentLevel >= totalUpgrades)
            {
                button.interactable = false;
                UpdateImageOnButton(_imagePairs, false);
            }
            else
            {
                button.interactable = true;
                UpdateImageOnButton(_imagePairs, true);
            }
        }
        public override void Close()
        {
            if (MenuBar.Instance) MenuBar.Instance.BlockRaycasts(true);

            base.Close();
        }

        [ContextMenu("Toggle Button")]
        public void ToggleButton()
        {
            bool value = !tankUpgradeButton.interactable;
            tankUpgradeButton.interactable = value;
            UpdateImageOnButton(tankUpgradeImagePair, value);

        }


        [ContextMenu("Increase Fuel Tank Capacity")]
        public void IncreaseFuelTankCapacity()
        {
            tankUpgradeButton.interactable = false;
            UpdateImageOnButton(tankUpgradeImagePair, false);
            GameData.Instance.FuelPurchased_API(Enums.FuelPurchaseType.Tank, _result =>
            {
                UpdateUiRelatedToTankCapacity();

            });
        }
        [ContextMenu("Increase Fuel Restore Speed")]
        public void IncreaseFuelRestoreSpeed()
        {
            restoreUpgradeButton.interactable = false;
            UpdateImageOnButton(restoreUpgradeImagePair, false);
            GameData.Instance.FuelPurchased_API(Enums.FuelPurchaseType.Restore, _result =>
            {
                UpdateUiRelatedToTankRestore();

            });
        }
    }
}




[Serializable]
public class ImagePair
{
    public Image imageToTarget;
    public Sprite interactableVersion;
    public Sprite disabledVersion;
}