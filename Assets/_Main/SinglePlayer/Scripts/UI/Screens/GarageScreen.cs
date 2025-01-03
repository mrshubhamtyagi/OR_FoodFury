using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    public class GarageScreen : MonoBehaviour, IScreen
    {
        public static Action<int> OnVehicleSelected;

        [SerializeField] private GarageTabs tabs;
        [SerializeField] private CanvasGroup confirmationCanvasGroup;
        [SerializeField] private TextMeshProUGUI confirmationBikeText;
        [SerializeField] private VehicleItemPrefab vehicleItemPrefab;
        [SerializeField] private Transform contentParent;
        [SerializeField] private TextMeshProUGUI purchaseTMP;

        [Header("-----Links")]
        [SerializeField] private string getItNowUrl;
        [SerializeField] private string useUnlockCodeUrl;


        [Header("-----Upgrades")]
        [SerializeField] private GarageUpgradePrefab speedUpgrade;
        [SerializeField] private GarageUpgradePrefab shieldUpgrade;
        [SerializeField] private GarageUpgradePrefab mileageUpgrade;
        [SerializeField] private GarageUpgradePrefab armourUpgrade;


        [Header("-----Buttons")]
        [SerializeField] private Button unlockForFreeBtn;
        [SerializeField] private GameObject purchaseBtn, selectBtn, selected;


        [Header("-----Other")]
        [SerializeField] private GameObject setup;
        [SerializeField] private Transform vehicleModels;
        [SerializeField] private TextMeshProUGUI nameTMP;
        [SerializeField] private TextMeshProUGUI tagTMP;
        [SerializeField] private GameObject congratsObj;
        [SerializeField] private GameObject missingInfoObj;
        public Dictionary<int, Texture2D> VehicleThumbnailDictionary { get; private set; }


        [Header("-----Debug")]
        [SerializeField] private VehicleItemPrefab currentItem;
        [field: SerializeField] public ModelClass.PlayerData.VehicleLevels PlayerVehicleLevels { get; private set; }
        [field: SerializeField] public ModelClass.GarageVehicleData SelectedVehicle { get; private set; }
        [field: SerializeField] public ModelClass.GarageVehicleData CurrentVehicle { get; private set; }
        [field: SerializeField] public int TotalUpgrades { get; private set; }
        private OrbitManager orbitManager;
        private ScrollRect scrollRect;
        private CanvasGroup canvasGroup;

        public static GarageScreen Instance { get; private set; }
        void Awake()
        {
            Instance = this;
            canvasGroup = GetComponent<CanvasGroup>();
            orbitManager = vehicleModels.GetComponent<OrbitManager>();
            scrollRect = contentParent.GetComponentInParent<ScrollRect>();
            VehicleThumbnailDictionary = new Dictionary<int, Texture2D>();
        }

        public IEnumerator InitVehicles(Enums.GarageVehicleType _type)
        {
            Loader.Instance.ShowLoader();
            yield return new WaitForSeconds(0.1f);

            foreach (Transform item in contentParent)
                Destroy(item.gameObject);

            foreach (Transform item in vehicleModels)
                item.gameObject.SetActive(false);

            var _vehicles = _type == Enums.GarageVehicleType.Regular ? GameData.Instance.GarageData.GetRegularBikes() : GameData.Instance.GarageData.GetSpecialBikes();
            foreach (var item in _vehicles)
            {
                VehicleItemPrefab _vehicle = Instantiate(vehicleItemPrefab, contentParent);
                _vehicle.gameObject.name = item.name;

                bool _isPlayerVehicle = item.id == PlayerVehicleLevels.id;
                bool _isSelected = item.id == SelectedVehicle.id;
                bool _isLocked = GameData.Instance.PlayerData.Data.vehicles.Find(v => v.id == item.id) == null;

                // Set as First
                if (_isSelected) _vehicle.transform.SetSiblingIndex(0);
                _vehicle.SetDetails(item, _isPlayerVehicle, _isSelected, _isLocked, () =>
                {
                    if (_isSelected) currentItem = _vehicle;
                });
            }

            yield return new WaitForSeconds(0.1f);
            OnVehiclePreview(contentParent.GetChild(0).GetComponent<VehicleItemPrefab>());

            scrollRect.verticalNormalizedPosition = 1;
            yield return new WaitForSeconds(0.1f);
            Loader.Instance.HideLoader();
        }

        private void UpdateNameAndTagUI(string _name, string _tagline)
        {
            nameTMP.text = _name;
            tagTMP.text = _tagline;
        }


        public void SwitchModel(int _prev, int _now)
        {
            OrbitManager.Instance.autoRotate = false;
            vehicleModels.GetChild(_prev - 1).gameObject.SetActive(false);
            //OrbitManager.Instance.ResetRotation();


            vehicleModels.GetChild(_now - 1).gameObject.SetActive(true);
            OrbitManager.Instance.autoRotate = true;
        }


        private void InitUpgrades(ModelClass.PlayerData.VehicleLevels _playerVehicleLevels)
        {
            speedUpgrade.SetUpgrade(_playerVehicleLevels.speedLevel,
                new ModelClass.GarageVehicleData.Upgrade
                {
                    level = _playerVehicleLevels.speedLevel,
                    value = _playerVehicleLevels.speedLevel == 0 ? SelectedVehicle.initial.speed : SelectedVehicle.speedUpgrades[_playerVehicleLevels.speedLevel - 1],
                    upgradeCost = _playerVehicleLevels.speedLevel == TotalUpgrades ? SelectedVehicle.upgradesCost[_playerVehicleLevels.speedLevel - 1] : SelectedVehicle.upgradesCost[_playerVehicleLevels.speedLevel]
                });

            shieldUpgrade.SetUpgrade(_playerVehicleLevels.shieldLevel,
                new ModelClass.GarageVehicleData.Upgrade
                {
                    level = _playerVehicleLevels.shieldLevel,
                    value = _playerVehicleLevels.shieldLevel == 0 ? SelectedVehicle.initial.shield : SelectedVehicle.shieldUpgrades[_playerVehicleLevels.shieldLevel - 1],
                    upgradeCost = _playerVehicleLevels.shieldLevel == TotalUpgrades ? SelectedVehicle.upgradesCost[_playerVehicleLevels.shieldLevel - 1] : SelectedVehicle.upgradesCost[_playerVehicleLevels.shieldLevel]
                });

            mileageUpgrade.SetUpgrade(_playerVehicleLevels.mileageLevel,
                new ModelClass.GarageVehicleData.Upgrade
                {
                    level = _playerVehicleLevels.mileageLevel,
                    value = _playerVehicleLevels.mileageLevel == 0 ? SelectedVehicle.initial.mileage : SelectedVehicle.mileageUpgrades[_playerVehicleLevels.mileageLevel - 1],
                    upgradeCost = _playerVehicleLevels.mileageLevel == TotalUpgrades ? SelectedVehicle.upgradesCost[_playerVehicleLevels.mileageLevel - 1] : SelectedVehicle.upgradesCost[_playerVehicleLevels.mileageLevel]
                });

            armourUpgrade.SetUpgrade(_playerVehicleLevels.armourLevel,
                new ModelClass.GarageVehicleData.Upgrade
                {
                    level = _playerVehicleLevels.armourLevel,
                    value = _playerVehicleLevels.armourLevel == 0 ? SelectedVehicle.initial.armour : SelectedVehicle.armourUpgrades[_playerVehicleLevels.armourLevel - 1],
                    upgradeCost = _playerVehicleLevels.armourLevel == TotalUpgrades ? SelectedVehicle.upgradesCost[_playerVehicleLevels.armourLevel - 1] : SelectedVehicle.upgradesCost[_playerVehicleLevels.armourLevel]
                });
        }


        public async void UpgradePlayerVehicle(ModelClass.GarageVehicleData.Upgrade _upgrade, Enums.UpgradeType _type, Action<int, ModelClass.GarageVehicleData.Upgrade, bool> _callback)
        {
            int _newLevel = 0;
            ModelClass.GarageVehicleData.Upgrade _newUpgrade = null;
            var _vehicle = GameData.Instance.GarageData.GetVehicleGarageData(SelectedVehicle.id);
            switch (_type)
            {
                case Enums.UpgradeType.Speed:
                    //GameData.Instance.PlayerData.SpeedLevel = (GameData.Instance.PlayerData.SpeedLevel + 1) % (totalUpgrades + 1);
                    _newLevel = (PlayerVehicleLevels.speedLevel + 1) % (TotalUpgrades + 1);
                    _newUpgrade = new ModelClass.GarageVehicleData.Upgrade
                    {
                        level = _newLevel,
                        value = _vehicle.speedUpgrades[_newLevel >= TotalUpgrades ? TotalUpgrades - 1 : _newLevel],
                        upgradeCost = _vehicle.upgradesCost[_newLevel >= TotalUpgrades ? TotalUpgrades - 1 : _newLevel]
                    };
                    break;

                case Enums.UpgradeType.Shield:
                    _newLevel = (PlayerVehicleLevels.shieldLevel + 1) % (TotalUpgrades + 1);
                    _newUpgrade = new ModelClass.GarageVehicleData.Upgrade
                    {
                        level = _newLevel,
                        value = _vehicle.shieldUpgrades[_newLevel >= TotalUpgrades ? TotalUpgrades - 1 : _newLevel],
                        upgradeCost = _vehicle.upgradesCost[_newLevel >= TotalUpgrades ? TotalUpgrades - 1 : _newLevel]
                    };
                    break;

                case Enums.UpgradeType.Mileage:
                    _newLevel = (PlayerVehicleLevels.mileageLevel + 1) % (TotalUpgrades + 1);
                    _newUpgrade = new ModelClass.GarageVehicleData.Upgrade
                    {
                        level = _newLevel,
                        value = _vehicle.mileageUpgrades[_newLevel >= TotalUpgrades ? TotalUpgrades - 1 : _newLevel],
                        upgradeCost = _vehicle.upgradesCost[_newLevel >= TotalUpgrades ? TotalUpgrades - 1 : _newLevel]
                    };
                    break;

                case Enums.UpgradeType.Armour:
                    _newLevel = (PlayerVehicleLevels.armourLevel + 1) % (TotalUpgrades + 1);
                    _newUpgrade = new ModelClass.GarageVehicleData.Upgrade
                    {
                        level = _newLevel,
                        value = _vehicle.armourUpgrades[_newLevel >= TotalUpgrades ? TotalUpgrades - 1 : _newLevel],
                        upgradeCost = _vehicle.upgradesCost[_newLevel >= TotalUpgrades ? TotalUpgrades - 1 : _newLevel]
                    };
                    break;
            }

            var _result = await GarageAndVehiclesAPIs.UpdateVehicleData_API(_type, _upgrade);
            if (_result)
            {
                AudioManager.Instance.PlayUpgradeSound();
                _callback?.Invoke(_newLevel, _newUpgrade, false);
            }
        }
        public void AddVehicleImage(int vehicleId, Texture2D image)
        {
            VehicleThumbnailDictionary.Add(vehicleId, image);
        }
        public bool hasVehicleThumbnail(int vehicleId)
        {
            return VehicleThumbnailDictionary.ContainsKey(vehicleId);
        }


        #region OnClick
        public void OnClick_Purchase()
        {
            confirmationBikeText.text = $"ARE YOU SURE YOU WANT TO\r\nUNLOCK <color=#00FFF0>{currentItem.name}</color> ?";
            TweenHandler.CanvasGroupAlpha(confirmationCanvasGroup, 1, ScreenManager.Instance.TweenDuration, ScreenManager.Instance.TweenEase,
                () => { confirmationCanvasGroup.blocksRaycasts = true; });

        }

        public async void OnClick_UnlockUsingNFT()
        {
            var _result = await GarageAndVehiclesAPIs.UnlockVehicleUsingNFT(CurrentVehicle);
            if (_result)
            {
                currentItem.Unlock();
                selectBtn.SetActive(true);
            }
        }

        public void OnClick_GetItNow() => PopUpManager.Instance.ShowPopup(PopUpManager.Instance.GetItNowPopup);
        public void OnClick_UseUnlockCode() => PopUpManager.Instance.ShowPopup(PopUpManager.Instance.UseUnlockCodePopup);


        // Select Vehicle - Active
        public async void OnClick_Select()
        {
            selectBtn.SetActive(false);
            var _success = await PlayerAPIs.UpdatePlayerDataCurrentVehicle_API(CurrentVehicle.id);
            if (_success)
            {
                selected.SetActive(true);
                PlayerVehicleLevels = GameData.Instance.PlayerData.Data.GetCurrentVehicleLevels();
                SelectedVehicle = CurrentVehicle;
                InitUpgrades(PlayerVehicleLevels);
                OnVehicleSelected?.Invoke(CurrentVehicle.id);
            }
            else
            {
                selectBtn.SetActive(true);
                OverlayWarningPopup.Instance.ShowWarning("Something went wrong!");
            }
        }



        public void OnClick_ConfirmationNo()
        {
            confirmationCanvasGroup.blocksRaycasts = false;
            TweenHandler.CanvasGroupAlpha(confirmationCanvasGroup, 0, ScreenManager.Instance.TweenDuration, ScreenManager.Instance.TweenEase);
        }

        public void OnClick_ConfirmationYes()
        {
            confirmationCanvasGroup.blocksRaycasts = false;
            TweenHandler.CanvasGroupAlpha(confirmationCanvasGroup, 0, ScreenManager.Instance.TweenDuration, ScreenManager.Instance.TweenEase,
                () => { PurchaseVehicle(); });
        }
        #endregion



        private async void PurchaseVehicle()
        {
            purchaseBtn.SetActive(false);
            var _result = await GarageAndVehiclesAPIs.VehiclePurchased_API(CurrentVehicle);
            if (_result)
            {
                selectBtn.SetActive(true);
                currentItem.Unlock();

                speedUpgrade.SetUpgrade(0, new ModelClass.GarageVehicleData.Upgrade
                {
                    level = 0,
                    value = CurrentVehicle.speedUpgrades[0],
                    upgradeCost = CurrentVehicle.upgradesCost[0]
                }, true);

                shieldUpgrade.SetUpgrade(0, new ModelClass.GarageVehicleData.Upgrade
                {
                    level = 0,
                    value = CurrentVehicle.shieldUpgrades[0],
                    upgradeCost = CurrentVehicle.upgradesCost[0]
                }, true);

                mileageUpgrade.SetUpgrade(0, new ModelClass.GarageVehicleData.Upgrade
                {
                    level = 0,
                    value = CurrentVehicle.mileageUpgrades[0],
                    upgradeCost = CurrentVehicle.upgradesCost[0]
                }, true);

                armourUpgrade.SetUpgrade(0, new ModelClass.GarageVehicleData.Upgrade
                {
                    level = 0,
                    value = CurrentVehicle.armourUpgrades[0],
                    upgradeCost = CurrentVehicle.upgradesCost[0]
                }, true);
            }
            else
            {
                purchaseBtn.SetActive(true);
                //OverlayWarningPopup.Instance.SetWarning("Something went wrong!");
            }
        }


        // On Vehicle Preview
        public void OnVehiclePreview(VehicleItemPrefab _currentItem)
        {
            //print("Item ID - " + _currentItem.Id);
            if (currentItem == null) SwitchModel(1, _currentItem.Id);
            else SwitchModel(currentItem.Id, _currentItem.Id);

            if (currentItem != null) currentItem.Deselect();
            currentItem = _currentItem;
            currentItem.Select();

            CurrentVehicle = GameData.Instance.GarageData.GetVehicleGarageData(currentItem.Id);
            UpdateNameAndTagUI(CurrentVehicle.name, CurrentVehicle.tagline);

            //if (CurrentVehicle.IsSpecialVehicle())
            //{
            //    purchaseBtn.SetActive(false);
            //    unlockForFreeBtn.gameObject.SetActive(currentItem.IsLocked);
            //    //unlockForFreeBtn.interactable = true;
            //    //missingInfoObj.SetActive(false);
            //}
            //else
            //{
            //Debug.Log("(Comment)jiski bhi id >6 woh locked yaha hai");
            //unlockForFreeBtn.gameObject.SetActive(currentItem.Id > 6);
            purchaseTMP.text = CurrentVehicle.cost.ToString();
            purchaseBtn.SetActive(currentItem.IsLocked);
            //}

            selectBtn.SetActive(!currentItem.IsLocked && !currentItem.IsSelected);
            selected.SetActive(currentItem.IsSelected);

            if (currentItem.IsPlayerVehicle)
            {
                InitUpgrades(GameData.Instance.PlayerData.Data.vehicles.Find(v => v.id == currentItem.Id));
            }
            else
            {
                var _playerVehicle = GameData.Instance.PlayerData.Data.vehicles.Find(v => v.id == currentItem.Id);
                //_currentItem.IsLocked ? CurrentVehicle.upgrades[0] : CurrentVehicle.upgrades[_playervehicle.speedLevel >= TotalUpgrades ? TotalUpgrades - 1 : _playervehicle.speedLevel],
                speedUpgrade.SetUpgrade(_currentItem.IsLocked ? 0 : _playerVehicle.speedLevel,
                    new ModelClass.GarageVehicleData.Upgrade
                    {
                        level = _currentItem.IsLocked ? 0 : _playerVehicle.speedLevel,
                        value = _currentItem.IsLocked ? CurrentVehicle.speedUpgrades[0] : CurrentVehicle.speedUpgrades[_playerVehicle.speedLevel >= TotalUpgrades ? TotalUpgrades - 1 : _playerVehicle.speedLevel],
                        upgradeCost = _currentItem.IsLocked ? CurrentVehicle.upgradesCost[0] : CurrentVehicle.upgradesCost[_playerVehicle.speedLevel >= TotalUpgrades ? TotalUpgrades - 1 : _playerVehicle.speedLevel]
                    }, true);

                shieldUpgrade.SetUpgrade(_currentItem.IsLocked ? 0 : _playerVehicle.shieldLevel,
                    new ModelClass.GarageVehicleData.Upgrade
                    {
                        level = _currentItem.IsLocked ? 0 : _playerVehicle.shieldLevel,
                        value = _currentItem.IsLocked ? CurrentVehicle.shieldUpgrades[0] : CurrentVehicle.shieldUpgrades[_playerVehicle.shieldLevel >= TotalUpgrades ? TotalUpgrades - 1 : _playerVehicle.shieldLevel],
                        upgradeCost = _currentItem.IsLocked ? CurrentVehicle.upgradesCost[0] : CurrentVehicle.upgradesCost[_playerVehicle.shieldLevel >= TotalUpgrades ? TotalUpgrades - 1 : _playerVehicle.shieldLevel]
                    }, true);

                mileageUpgrade.SetUpgrade(_currentItem.IsLocked ? 0 : _playerVehicle.mileageLevel,
                    new ModelClass.GarageVehicleData.Upgrade
                    {
                        level = _currentItem.IsLocked ? 0 : _playerVehicle.mileageLevel,
                        value = _currentItem.IsLocked ? CurrentVehicle.mileageUpgrades[0] : CurrentVehicle.mileageUpgrades[_playerVehicle.mileageLevel >= TotalUpgrades ? TotalUpgrades - 1 : _playerVehicle.mileageLevel],
                        upgradeCost = _currentItem.IsLocked ? CurrentVehicle.upgradesCost[0] : CurrentVehicle.upgradesCost[_playerVehicle.mileageLevel >= TotalUpgrades ? TotalUpgrades - 1 : _playerVehicle.mileageLevel]
                    }, true);

                armourUpgrade.SetUpgrade(_currentItem.IsLocked ? 0 : _playerVehicle.armourLevel,
                    new ModelClass.GarageVehicleData.Upgrade
                    {
                        level = _currentItem.IsLocked ? 0 : _playerVehicle.armourLevel,
                        value = _currentItem.IsLocked ? CurrentVehicle.armourUpgrades[0] : CurrentVehicle.armourUpgrades[_playerVehicle.armourLevel >= TotalUpgrades ? TotalUpgrades - 1 : _playerVehicle.armourLevel],
                        upgradeCost = _currentItem.IsLocked ? CurrentVehicle.upgradesCost[0] : CurrentVehicle.upgradesCost[_playerVehicle.armourLevel >= TotalUpgrades ? TotalUpgrades - 1 : _playerVehicle.armourLevel]
                    }, true);
            }
        }





        [ContextMenu("Init")]
        private void Init()
        {
            PlayerVehicleLevels = GameData.Instance.PlayerData.Data.GetCurrentVehicleLevels();
            SelectedVehicle = GameData.Instance.GarageData.GetVehicleGarageData(PlayerVehicleLevels.id);
            TotalUpgrades = SelectedVehicle.upgradesCost.Count;

            selected.SetActive(true);
            purchaseBtn.SetActive(false);
            unlockForFreeBtn.gameObject.SetActive(false);
            missingInfoObj.SetActive(false);

            Enums.GarageVehicleType _type = SelectedVehicle.IsSpecialVehicle() ? Enums.GarageVehicleType.Special : Enums.GarageVehicleType.Regular;
            StartCoroutine(InitVehicles(_type));
            tabs.UpdateUI(_type);

            orbitManager.ResetRotation();
            orbitManager.autoRotate = true;
            setup.SetActive(true);

        }


        public void Show(Action _callback = null)
        {
            ScreenManager.Instance.CurrentScreen = Enums.Screen.Garage;
            Init();
            MenuBar.Instance.Show();
            TweenHandler.CanvasGroupAlpha(canvasGroup, 1, ScreenManager.Instance.TweenDuration, ScreenManager.Instance.TweenEase, () =>
            {
                canvasGroup.blocksRaycasts = true;
                _callback?.Invoke();

                Loader.Instance.HideLoader();
            });
        }

        public void Hide(Action _callback = null)
        {
            Loader.Instance.ShowLoader();
            setup.SetActive(false);
            canvasGroup.blocksRaycasts = false;
            TweenHandler.CanvasGroupAlpha(canvasGroup, 0, ScreenManager.Instance.TweenDuration, ScreenManager.Instance.TweenEase, () =>
            {
                _callback?.Invoke();
            });
        }
    }
}
