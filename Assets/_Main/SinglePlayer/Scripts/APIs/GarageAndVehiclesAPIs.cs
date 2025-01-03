using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace FoodFury
{
    public static class GarageAndVehiclesAPIs
    {
        public static async Task<ModelClass.ErrorAndResultResponse<List<ModelClass.GarageVehicleData>>> GetGarageData_API()
        {
            var _response = await APIManager.GetGarageDataAsync();
            //Debug.Log($"GetGarageData_API - Status {_response.error} | Response {_response.result}");
            return _response.error ? new() : JsonConvert.DeserializeObject<ModelClass.ErrorAndResultResponse<List<ModelClass.GarageVehicleData>>>(_response.result);
        }


        public static async Task<bool> VehiclePurchased_API(ModelClass.GarageVehicleData _vehicleData)
        {
            if (GameData.Instance.releaseMode == Enums.ReleaseMode.Debug)
            {
                if (!GameData.Instance.DeductCost(_vehicleData.costType, _vehicleData.cost))
                {
                    OverlayWarningPopup.Instance.ShowWarning("Insufficient Balance!");
                    return false;
                }

                GameData.Instance.PlayerData.Data.vehicles.Add(new ModelClass.PlayerData.VehicleLevels()
                {
                    id = _vehicleData.id,
                    speedLevel = 0,
                    shieldLevel = 0,
                    mileageLevel = 0,
                    armourLevel = 0
                });
                GameData.Invoke_OnPlayerDataUpdate();

                AnalyticsManager.Instance.FireVehiclePurchasedSuccessfulEvent(_vehicleData.id, _vehicleData.name, _vehicleData.IsSpecialVehicle(), _vehicleData.nftName);
                return true;
            }


            Dictionary<string, object> _params = new() { { "userId", GameData.Instance.PlayerId } };
            _params.Add("vehicleId", _vehicleData.id);
            //Debug.Log($"VehiclePurchased - {JsonConvert.SerializeObject(_params)}");


            var _response = await APIManager.PurchaseVehicleAsync(_params.ToJSONObject());
            //Debug.Log($"VehiclePurchased_API - Status {_response.error} | Response {_response.result}");
            if (_response.error)
            {
                Debug.Log($"Could not Purchase Vehicle - {_response}");
                return false;
            }

            var _data = JsonConvert.DeserializeObject<ModelClass.ErrorAndResultResponse<string>>(_response.result);
            if (_data.error)
            {
                if (_vehicleData.costType == Enums.CostType.POINTS) GameData.Instance.PlayerData.Data.chips = int.Parse(_data.result.Split('-')[1].Trim());
                else GameData.Instance.PlayerData.Data.gameBalance = float.Parse(_data.result.Split('-')[1].Trim());
                GameData.Invoke_OnPlayerDataUpdate();
                OverlayWarningPopup.Instance.ShowWarning($"Could not purchase Vehicle - {_data.result}");
                Debug.Log($"Could not purchase Vehicle - {_data.result}");
                return false;
            }


            GameData.Instance.DeductCost(_vehicleData.costType, _vehicleData.cost);
            GameData.Instance.PlayerData.Data.vehicles.Add(new ModelClass.PlayerData.VehicleLevels()
            {
                id = _vehicleData.id,
                speedLevel = 0,
                shieldLevel = 0,
                mileageLevel = 0,
                armourLevel = 0
            });
            GameData.Invoke_OnPlayerDataUpdate();

            AnalyticsManager.Instance.FireVehiclePurchasedSuccessfulEvent(_vehicleData.id, _vehicleData.name, _vehicleData.IsSpecialVehicle(), _vehicleData.nftName);
            return true;
        }


        public static async Task<bool> UpdateVehicleData_API(Enums.UpgradeType _type, ModelClass.GarageVehicleData.Upgrade _upgrade)
        {
            if (GameData.Instance.releaseMode == Enums.ReleaseMode.Debug)
            {
                if (!GameData.Instance.DeductCost(_upgrade.upgradeCost.costType, _upgrade.upgradeCost.cost))
                {
                    OverlayWarningPopup.Instance.ShowWarning("Insufficient Balance!");
                    return false;
                }

                switch (_type)
                {
                    case Enums.UpgradeType.Speed:
                        GameData.Instance.PlayerData.Data.GetCurrentVehicleLevels().speedLevel++;
                        break;

                    case Enums.UpgradeType.Shield:
                        GameData.Instance.PlayerData.Data.GetCurrentVehicleLevels().shieldLevel++;
                        break;

                    case Enums.UpgradeType.Mileage:
                        GameData.Instance.PlayerData.Data.GetCurrentVehicleLevels().mileageLevel++;
                        break;

                    case Enums.UpgradeType.Armour:
                        GameData.Instance.PlayerData.Data.GetCurrentVehicleLevels().armourLevel++;
                        break;
                }
                GameData.Invoke_OnPlayerDataUpdate();
                return true;
            }


            Dictionary<string, object> _params = new() { { "userId", GameData.Instance.PlayerId } };
            _params.Add("vehicleId", GameData.Instance.PlayerData.Data.currentVehicle);
            _params.Add("upgradeType", _type.ToString());
            _params.Add("levelId", _upgrade.level + 1);
            //Debug.Log($"VehicleData Params - {JsonConvert.SerializeObject(_params)}");

            var _response = await APIManager.UpdateVehicleDataAsync(_params.ToJSONObject());
            //Debug.Log($"UpdateVehicleData - Status {_response.error} | Response {_response.result}");
            if (_response.error)
            {
                Debug.Log($"Could not upgrade Vehicle - {_response}");
                return false;
            }


            var _data = JsonConvert.DeserializeObject<ModelClass.ErrorAndResultResponse<string>>(_response.result);
            if (_data.error)
            {
                //OverlayWarningPopup.Instance.ShowWarning(_data.result);
                if (_upgrade.upgradeCost.costType == Enums.CostType.POINTS) GameData.Instance.PlayerData.Data.chips = int.Parse(_data.result.Split('-')[1].Trim());
                else GameData.Instance.PlayerData.Data.gameBalance = float.Parse(_data.result.Split('-')[1].Trim());
                GameData.Invoke_OnPlayerDataUpdate();

                Debug.Log($"Could not upgrade Vehicle - {_data.result}");
                return false;
            }
            else
            {
                GameData.Instance.DeductCost(_upgrade.upgradeCost.costType, _upgrade.upgradeCost.cost);
                switch (_type)
                {
                    case Enums.UpgradeType.Speed:
                        GameData.Instance.PlayerData.Data.GetCurrentVehicleLevels().speedLevel++;
                        break;

                    case Enums.UpgradeType.Shield:
                        GameData.Instance.PlayerData.Data.GetCurrentVehicleLevels().shieldLevel++;
                        break;

                    case Enums.UpgradeType.Mileage:
                        GameData.Instance.PlayerData.Data.GetCurrentVehicleLevels().mileageLevel++;
                        break;

                    case Enums.UpgradeType.Armour:
                        GameData.Instance.PlayerData.Data.GetCurrentVehicleLevels().armourLevel++;
                        break;
                }
                var currentVehicle = GameData.Instance.GarageData.GetCurrentVehicleGarageData();
                GameData.Invoke_OnPlayerDataUpdate();

                AnalyticsManager.Instance.FireVehicleUpgradeSuccessfulEvent(currentVehicle.id, currentVehicle.name, currentVehicle.IsSpecialVehicle(), currentVehicle.nftName, (int)_params["levelId"], _type.ToString());
                return true;
            }

        }



        public static async Task<bool> UnlockVehicleUsingNFT(ModelClass.GarageVehicleData _vehicleData)
        {
            //if (releaseMode == Enums.ReleaseMode.Debug)
            //{
            //    if (!DeductCost(_vehicleData.costType, _vehicleData.cost))
            //    {
            //        OverlayWarningPopup.Instance.SetWarning("Insufficient Balance!");
            //        _callback?.Invoke(false);
            //        return;
            //    }

            //    PlayerData.vehicles.Add(new ModelClass.PlayerData.VehicleLevels()
            //    {
            //        id = _vehicleData.id,
            //        speedLevel = 0,
            //        shieldLevel = 0,
            //        mileageLevel = 0,
            //        armourLevel = 0
            //    });
            //    _callback?.Invoke(true);
            //    OnPlayerDataUpdate?.Invoke();
            //    return;
            //}

            var _response = await APIManager.UnlockVehicleUsingNFTAsync(GameData.Instance.PlayerId, _vehicleData.nftName);
            //Debug.Log($"UnlockVehicleUsingNFT - Status {_status} | Response {_response}");
            if (_response.error)
            {
                Debug.Log($"Could not Purchase Vehicle - {_response}");
                return false;
            }

            var _data = JsonConvert.DeserializeObject<ModelClass.ErrorAndResultResponse<string>>(_response.result);
            if (_data.error)
            {
                Debug.Log($"Could not Purchase Vehicle - {_data.result}");
                return false;
            }

            //DeductCost(_vehicleData.costType, _vehicleData.cost);
            GameData.Instance.PlayerData.Data.vehicles.Add(new ModelClass.PlayerData.VehicleLevels()
            {
                id = _vehicleData.id,
                speedLevel = 0,
                shieldLevel = 0,
                mileageLevel = 0,
                armourLevel = 0
            });
            GameData.Invoke_OnPlayerDataUpdate();

            AnalyticsManager.Instance.FireVehiclePurchasedSuccessfulEvent(_vehicleData.id, _vehicleData.name, _vehicleData.IsSpecialVehicle(), _vehicleData.nftName);
            return true;
        }
    }
}
